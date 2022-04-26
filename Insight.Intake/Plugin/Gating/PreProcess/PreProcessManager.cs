using Insight.Intake.Extensions;
using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Insight.Intake.Plugin.Gating.PreProcess
{
    public class PreProcessManager
    {
        IOrganizationService _service;
        ITracingService _tracingService;
        EntityReference _caseRef;
        public PreProcessManager(IOrganizationService organizationService, EntityReference targetRef)
        {
            _service = organizationService;
            _caseRef = targetRef;
        }
        public bool DeriveHomePlan(Entity caseRecord, Entity target)
        {
            if (caseRecord != null)
            {
                var taskManager = new TaskManager(_service, _tracingService);
                EntityReference PrimaryCarrier = caseRecord.GetAttributeValue<EntityReference>("ipg_carrierid");
                string homePlanNetwork = string.Empty;
                string MemberIdNumber = caseRecord.GetAttributeValue<string>("ipg_memberidnumber");
                var dos = caseRecord.GetCaseDos();
                if (target != null)
                {
                    if (target.Contains("ipg_surgerydate") && dos == null)
                    {
                        var task = CreateTask($"Determine Home Plan Carrier: Date Of surgery is missing",
                                      $"Date Of Surgery is missing"
                                      , caseRecord.Id);
                        _service.Create(task);
                        return false;
                    }
                }
                if (dos == null)
                {
                    return false;
                }
                var surgeryDate = (DateTime)dos;

                if (target == null || target.Contains("ipg_carrierid") || target.Contains("ipg_secondarycarrierid")
                        || target.Contains("ipg_memberidnumber") || target.Contains("ipg_secondarymemberidnumber"))
                {
                    //If primary Carrier is not empty
                    if (PrimaryCarrier != null)
                    {
                        var InsertedPrimaryCarrier = (Intake.Account)_service.Retrieve(PrimaryCarrier.LogicalName, PrimaryCarrier.Id, new ColumnSet(Intake.Account.Fields.ipg_HomePlanNetworkOptionSet, Intake.Account.Fields.Name));
                        if (InsertedPrimaryCarrier.ipg_HomePlanNetworkOptionSet != null)
                            homePlanNetwork = D365Helpers.GetOptionSetValueLabel(Intake.Account.EntityLogicalName, "ipg_homeplannetworkoptionset",
                               InsertedPrimaryCarrier.ipg_HomePlanNetworkOptionSet.Value, _service);

                        if (homePlanNetwork == "BCBS")
                        {
                            //check for the primary insurance id
                            if (MemberIdNumber == null || MemberIdNumber == string.Empty)
                            {
                                var g = 4;
                                var task = new Task();
                                task.Subject = $"Determine Home Plan Carrier: Primary member id is empty";
                                task.Description = $"No member id found on the case";
                                task.RegardingObjectId = new EntityReference(Incident.EntityLogicalName, caseRecord.Id);
                                _service.Create(task);
                                RemoveHomePlanCarrier(_service, caseRecord.Id);
                            }
                            else
                            {
                                //Find the position where a number starts in the memberid
                                int positionOfNumberInMemberId;
                                Regex re = new Regex(@"\d+");
                                Match m = re.Match(MemberIdNumber);
                                // Validate the length of alpha part, but not the whole name
                                //Regex membRegEx = new Regex(@"^[a-zA-Z0-9]+");
                                //Match membRegExMatch = membRegEx.Match(MemberIdNumber);
                                if (!m.Success || m.Length > 5) // number is not found in the member id
                                {
                                    var task = CreateTask($"Determine Home Plan - Member ID format is invalid",
                                        $"The memberid on the case is [{MemberIdNumber}]. This appears to be invalid."
                                        , caseRecord.Id);
                                    _service.Create(task);
                                    RemoveHomePlanCarrier(_service, caseRecord.Id);
                                }
                                else //a number is found in the memberid
                                {
                                    positionOfNumberInMemberId = m.Index; // The first position of number the member id

                                    //Invalid - BCBS never uses more than 3-- was 5 but users felt maintaining up to 5 was too difficult
                                    if (positionOfNumberInMemberId > 4)
                                    {
                                        var task = CreateTask("Determine Home Plan Carrier: MemberId appears to be invalid"
                                            , "The member id has more than 5 character alpha prefix, the alpha pefix on a memberid is limited to 1-5 characters"
                                            , caseRecord.Id);
                                        _service.Create(task);
                                        RemoveHomePlanCarrier(_service, caseRecord.Id);
                                    }
                                    else if (positionOfNumberInMemberId == 0)
                                    {
                                        var task = CreateTask("Determine Home Plan Carrier: MemberId appears to be invalid"
                                            , "There are no numbers found in the primary member id"
                                            , caseRecord.Id);
                                        _service.Create(task);
                                        RemoveHomePlanCarrier(_service, caseRecord.Id);
                                    }
                                    else
                                    {
                                        //This is straight from the stored procedure
                                        int numberOfPrefixLettersToConsider = positionOfNumberInMemberId > 2 ? 3 : positionOfNumberInMemberId;
                                        string memberIdPrefix = MemberIdNumber.Substring(0, numberOfPrefixLettersToConsider);

                                        Entity homePlanCarrierMap = QueryHomeplanCarrierMap(_service, surgeryDate, memberIdPrefix);
                                        //If there are no carriers found or are active for the code 
                                        if (homePlanCarrierMap == null)
                                        {
                                            var res = QueryHomeplanCarrierMapForExpiredHomeplan(_service, surgeryDate, memberIdPrefix);
                                            if (res.Entities.Count > 0)
                                            {
                                                taskManager.CreateTask(caseRecord.ToEntityReference(), "Inactive Home Plan");
                                                RemoveHomePlanCarrier(_service, caseRecord.Id);
                                            }
                                            else
                                            {
                                                CreateNoHomePlanValueFoundTask(taskManager,
                                                    InsertedPrimaryCarrier.GetAttributeValue<string>(Intake.Account.Fields.Name),
                                                    homePlanNetwork,
                                                    memberIdPrefix,
                                                    dos);

                                                /*//CompleteTask
                                                var setStateRequest = new SetStateRequest();
                                                setStateRequest.State = new OptionSetValue((int)TaskState.Completed);
                                                setStateRequest.Status = new OptionSetValue((int)Task_StatusCode.Resolved);
                                                setStateRequest.EntityMoniker = new EntityReference(task.LogicalName, newtaskid);
                                                var stateSet = (SetStateResponse)_service.Execute(setStateRequest);*/

                                                RemoveHomePlanCarrier(_service, caseRecord.Id);
                                            }
                                        }
                                        else
                                        {
                                            Incident updateEntity = new Incident
                                            {
                                                Id = caseRecord.Id,
                                                ipg_HomePlanCarrierId = homePlanCarrierMap.GetAttributeValue<EntityReference>(Intake.ipg_homeplancarriermap.Fields.ipg_CarrierId)
                                            };
                                            _service.Update(updateEntity);
                                            var medicareAdvantage = homePlanCarrierMap.GetAttributeValue<bool>(Intake.ipg_homeplancarriermap.Fields.ipg_MedicareAdvantage);
                                            var carrierName = homePlanCarrierMap.GetAttributeValue<AliasedValue>("carrier.name").Value;
                                            var task = CreateTask($"Determine Home Plan Carrier: Mapped to carrier"
                                                , $"BCBS: Code [{memberIdPrefix}] mapped to {carrierName} for the DOS {surgeryDate.Date}"
                                                , caseRecord.Id);
                                            _service.Create(task);
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                        else // homeplan network on carrier is not BSBS
                        {
                            var task = CreateTask("Determine Home Plan Carrier: No Homeplan required"
                                               , $"Carrier is not part of BCBS. So Homeplan Carrier is not required", caseRecord.Id);
                            _service.Create(task);
                            return true;
                        }
                    }
                    else //No primary carrier is present
                    {
                        var task = CreateTask("Determine Home Plan Carrier: No primary Carrier is present"
                                               , $"Carrier is not present on the case", caseRecord.Id);
                        _service.Create(task);
                        RemoveHomePlanCarrier(_service, caseRecord.Id);
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// This method queries the Homeplancarriermap entity for memberid prefix(e.x. AAK) 
        /// finds corresponding carrier id's
        /// </summary>
        /// <param name="service">IOrganisationService Object</param>
        /// <param name="surgeryDate">The date of surgery</param>
        /// <param name="memberIdPrefix">The first letters of InsuranceID</param>
        /// <returns></returns>
        private Entity QueryHomeplanCarrierMap(IOrganizationService service, DateTime surgeryDate, string memberIdPrefix)
        {
            QueryExpression query = new QueryExpression();
            query.EntityName = ipg_homeplancarriermap.EntityLogicalName;
            query.ColumnSet = new ColumnSet(new string[] { "ipg_carrierid", "ipg_code", "ipg_medicareadvantage", "ipg_effectivedate", "ipg_enddate" });
            query.LinkEntities.Add(new LinkEntity(ipg_homeplancarriermap.EntityLogicalName
                , Intake.Account.EntityLogicalName, "ipg_carrierid", "accountid", JoinOperator.Inner));
            query.LinkEntities[0].Columns = new ColumnSet(new string[] { "name" });
            query.LinkEntities[0].EntityAlias = "carrier";
            query.Criteria = new FilterExpression(LogicalOperator.And);
            query.Criteria.AddCondition(
                    new ConditionExpression(ipg_homeplancarriermap.EntityLogicalName, "ipg_name", ConditionOperator.Equal, "BCBS")
            );
            query.Criteria.AddCondition(
                    new ConditionExpression(ipg_homeplancarriermap.EntityLogicalName, "ipg_code", ConditionOperator.BeginsWith, memberIdPrefix.Substring(0, 1))
            );
            query.Criteria.AddCondition(
                new ConditionExpression(ipg_homeplancarriermap.EntityLogicalName, "ipg_effectivedate", ConditionOperator.LessEqual, surgeryDate.Date)
            );
            query.Criteria.AddCondition(
                new ConditionExpression(ipg_homeplancarriermap.EntityLogicalName, "ipg_enddate", ConditionOperator.GreaterEqual, surgeryDate.Date)
            );
            var results = service.RetrieveMultiple(query).Entities;
            if (results.Any())
            {
                Entity foundhomePlanCarrierMap = null;
                for (int i = 1; i < memberIdPrefix.Length; i++)
                {
                    if (results.Any(x => x.GetAttributeValue<string>("ipg_code").IndexOf(memberIdPrefix.Substring(0, i)) == 0))
                    {
                        foundhomePlanCarrierMap = results.Where(x => x.GetAttributeValue<string>("ipg_code").IndexOf(memberIdPrefix.Substring(0, i)) == 0).First();
                    }
                }
                return foundhomePlanCarrierMap;
            }
            return null;
        }

        /// <summary>
        /// This method queries the Homeplancarriermap entity for memberid prefix(e.x. AAK) 
        /// finds corresponding carrier id's which are expired i.e. might be outside the effective and expiration date
        /// </summary>
        /// <param name="service">IOrganisationService Object</param>
        /// <param name="surgeryDate">The date of surgery</param>
        /// <param name="memberIdPrefix">The first letters of InsuranceID</param>
        /// <returns></returns>
        private EntityCollection QueryHomeplanCarrierMapForExpiredHomeplan(IOrganizationService service, DateTime surgeryDate, string memberIdPrefix)
        {
            QueryExpression query = new QueryExpression();
            query.EntityName = ipg_homeplancarriermap.EntityLogicalName;
            query.ColumnSet = new ColumnSet(new string[] { "ipg_carrierid", "ipg_code", "ipg_medicareadvantage", "ipg_effectivedate", "ipg_enddate" });
            query.LinkEntities.Add(new LinkEntity(ipg_homeplancarriermap.EntityLogicalName
                , Intake.Account.EntityLogicalName, "ipg_carrierid", "accountid", JoinOperator.Inner));
            query.LinkEntities[0].Columns = new ColumnSet(new string[] { "name" });
            query.LinkEntities[0].EntityAlias = "carrier";
            query.Criteria = new FilterExpression(LogicalOperator.And);
            query.Criteria.AddCondition(
                    new ConditionExpression(ipg_homeplancarriermap.EntityLogicalName, "ipg_name", ConditionOperator.Equal, "BCBS")
            );
            query.Criteria.AddCondition(
                    new ConditionExpression(ipg_homeplancarriermap.EntityLogicalName, "ipg_code", ConditionOperator.Equal, memberIdPrefix)
            );
            var results = service.RetrieveMultiple(query);
            return results;
        }

        /// <summary>
        /// This method creates a task and associates it with the case
        /// </summary>
        /// <param name="subject">Subject on the task</param>
        /// <param name="description">Description of the task</param>
        /// <param name="id">The GUID of the related record.</param>
        /// <returns>Task</returns>
        private Task CreateTask(string subject, string description, Guid id, TaskState state = TaskState.Open, Task_StatusCode status = Task_StatusCode.InProgress)
        {
            var task = new Task();
            task.Subject = subject;
            task.Description = description;
            task.RegardingObjectId = new EntityReference(Incident.EntityLogicalName, id);
            task.StateCode = state;
            task.StatusCodeEnum = status;
            return task;
        }

        private void RemoveHomePlanCarrier(IOrganizationService service, Guid id)
        {
            Incident updateEntity = new Incident();
            updateEntity.Id = id;
            updateEntity.ipg_HomePlanCarrierId = null;
            service.Update(updateEntity);
        }

        internal void DeleteAllDeriveHomePlanOnCase(EntityReference incident)
        {
            var DeriveHomePlanTaskQuery = new QueryExpression
            {
                EntityName = Task.EntityLogicalName,
                ColumnSet = new ColumnSet(false),
                Criteria = new FilterExpression(LogicalOperator.And),
                PageInfo = new PagingInfo
                {
                    ReturnTotalRecordCount = true
                },
            };

            DeriveHomePlanTaskQuery.Criteria.AddCondition(nameof(Task.Subject).ToLower(), ConditionOperator.Like, "%Determine Home Plan Carrier:%");
            DeriveHomePlanTaskQuery.Criteria.AddCondition(nameof(Task.RegardingObjectId).ToLower(), ConditionOperator.Equal, incident.Id);

            var DeriveHomePlanTaskResponse = _service.RetrieveMultiple(DeriveHomePlanTaskQuery);
            if (DeriveHomePlanTaskResponse != null)
            {
                foreach (var entity in DeriveHomePlanTaskResponse?.Entities)
                {
                    _service.Delete(Task.EntityLogicalName, entity.Id);
                }
            }
        }

        private void CreateNoHomePlanValueFoundTask(TaskManager taskManager, string carrierName, string homePlanNetwork, string memberIdPrefix, DateTime? dos)
        {
            var taskType = taskManager.GetTaskTypeByName("No Home Plan Value Found");
            var task = new Task()
            {
                ipg_tasktypeid = taskType.ToEntityReference()
            };
            task = taskManager.ConfigureTaskByTaskType(task);
            string.Format(task.Description, carrierName, homePlanNetwork, memberIdPrefix, dos.ToString());
            _service.Create(task);
        }
    }
}
