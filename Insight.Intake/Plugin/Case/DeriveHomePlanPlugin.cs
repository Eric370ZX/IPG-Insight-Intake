using Insight.Intake.Extensions;
using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Insight.Intake.Plugin.Case
{
    public class DeriveHomePlanPlugin : PluginBase
    {
        readonly string[] COLUMNSETINCIDENT = {
                            Incident.Fields.ipg_CarrierId,
                            Incident.Fields.ipg_SurgeryDate,
                            Incident.Fields.ipg_SecondaryCarrierId,
                            Incident.Fields.ipg_MemberIdNumber,
                            Incident.Fields.ipg_SecondaryMemberIdNumber
        };

        IOrganizationService _service;
        IPluginExecutionContext _context;
        ITracingService _tracingService;

        public DeriveHomePlanPlugin() : base(typeof(DeriveHomePlanPlugin))
        {
            //RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Incident.EntityLogicalName, PostOperationHandlerUpdate);
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingDeriveHomePlanPlugin", null, PostOperationHandlerWorkflowTask);
        }

        private void PostOperationHandlerUpdate(LocalPluginContext localPluginContext)
        {
            _context = localPluginContext.PluginExecutionContext;
            _service = localPluginContext.OrganizationService;
            _tracingService = localPluginContext.TracingService;
            var target = (Entity)_context.InputParameters["Target"];
            var caseRecord = _service.Retrieve(target.LogicalName, target.Id, new ColumnSet(COLUMNSETINCIDENT));

            DeleteAllDeriveHomePlanOnCase(caseRecord.ToEntityReference());
            DeriveHomePlan(caseRecord, target);
        }

        private void PostOperationHandlerWorkflowTask(LocalPluginContext localPluginContext)
        {
            _context = localPluginContext.PluginExecutionContext;
            _service = localPluginContext.OrganizationService;
            var targetRef = (EntityReference)_context.InputParameters["Target"];
            if (targetRef?.LogicalName == ipg_referral.EntityLogicalName)
            {
                _context.OutputParameters["Succeeded"] = true;
                return;
            }
            _context.OutputParameters["Succeeded"] = false;

            if (targetRef != null)
            {
                var caseRecord = _service.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet(COLUMNSETINCIDENT));

                DeleteAllDeriveHomePlanOnCase(caseRecord.ToEntityReference());
                var result = DeriveHomePlan(caseRecord, null);
                _context.OutputParameters["Succeeded"] = result.Succeeded;
                _context.OutputParameters["CodeOutput"] = result.CodeOutput;

            }
        }

        public GatingResponse DeriveHomePlan(Entity caseRecord, Entity target)
        {
            if (caseRecord != null)
            {
                var isActionCall = _context.MessageName == "ipg_IPGGatingDeriveHomePlanPlugin";
                EntityReference PrimaryCarrier = caseRecord.GetAttributeValue<EntityReference>("ipg_carrierid");
                string homePlanNetwork = string.Empty;
                string MemberIdNumber = caseRecord.GetAttributeValue<string>("ipg_memberidnumber");
                var dos = caseRecord.GetCaseDos();
                //if (target != null)
                //{
                //    if (target.Contains("ipg_surgerydate") && dos == null)
                //    {
                //        CreateTask($"Determine Home Plan Carrier: Date Of surgery is missing",
                //                      $"Date Of Surgery is missing"
                //                      , caseRecord.Id);
                //        return false;
                //    }
                //}
                //if (dos == null)
                //{
                //    return false;
                //}
                var surgeryDate = (DateTime)dos;

                if (target == null || target.Contains("ipg_carrierid") || target.Contains("ipg_secondarycarrierid")
                        || target.Contains("ipg_memberidnumber") || target.Contains("ipg_secondarymemberidnumber") || isActionCall)
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
                                RemoveHomePlanCarrier(_service, caseRecord.Id);
                                return new GatingResponse()
                                {
                                    Succeeded = false,
                                    CodeOutput = (int)DeriveHomePlenOutput.MemberIdIsEmpty
                                };
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
                                if (!m.Success) // number is not found in the member id
                                {
                                    RemoveHomePlanCarrier(_service, caseRecord.Id);
                                    return new GatingResponse()
                                    {
                                        Succeeded = false,
                                        CodeOutput = (int)DeriveHomePlenOutput.MemberIdFormatIsInvalid
                                    };
                                }
                                else //a number is found in the memberid
                                {
                                    positionOfNumberInMemberId = m.Index; // The first position of number the member id

                                    //Invalid - BCBS never uses more than 3-- was 5 but users felt maintaining up to 5 was too difficult
                                    if (positionOfNumberInMemberId > 4)
                                    {
                                        RemoveHomePlanCarrier(_service, caseRecord.Id);
                                        return new GatingResponse()
                                        {
                                            Succeeded = false,
                                            CodeOutput = (int)DeriveHomePlenOutput.moreThan5CharacterAlphaPrefix
                                        };
                                    }
                                    else if (positionOfNumberInMemberId == 0)
                                    {
                                        RemoveHomePlanCarrier(_service, caseRecord.Id);
                                        return new GatingResponse()
                                        {
                                            Succeeded = false,
                                            CodeOutput = (int)DeriveHomePlenOutput.NoNumbersFoundInThePrimaryMemberId
                                        };
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
                                                //CreateInactiveHomePlanTask(taskManager, caseRecord.ToEntityReference(), InsertedPrimaryCarrier.Name);
                                                RemoveHomePlanCarrier(_service, caseRecord.Id);
                                                return new GatingResponse()
                                                {
                                                    Succeeded = false,
                                                    CodeOutput = (int)DeriveHomePlenOutput.InactiveHomePlan
                                                };

                                            }
                                            else
                                            {
                                                //CreateNoHomePlanValueFoundTask(taskManager,
                                                //    caseRecord.ToEntityReference(),
                                                //    InsertedPrimaryCarrier.Name);

                                                RemoveHomePlanCarrier(_service, caseRecord.Id);
                                                return new GatingResponse()
                                                {
                                                    Succeeded = false,
                                                    CodeOutput = (int)DeriveHomePlenOutput.NoHomePlanFound
                                                };
                                            }
                                        }
                                        else
                                        {
                                            Incident updateEntity = new Incident
                                            {
                                                Id = caseRecord.Id,
                                                ipg_HomePlanCarrierId = homePlanCarrierMap.GetAttributeValue<EntityReference>(ipg_homeplancarriermap.Fields.ipg_CarrierId)
                                            };
                                            _service.Update(updateEntity);
                                            //var medicareAdvantage = homePlanCarrierMap.GetAttributeValue<bool>(ipg_homeplancarriermap.Fields.ipg_MedicareAdvantage);
                                            //var carrierName = homePlanCarrierMap.GetAttributeValue<AliasedValue>("carrier.name").Value;
                                            //CreateTask($"Determine Home Plan Carrier: Mapped to carrier"
                                            //    , $"BCBS: Code [{memberIdPrefix}] mapped to {carrierName} for the DOS {surgeryDate.Date}"
                                            //    , caseRecord.Id);

                                            return new GatingResponse()
                                            {
                                                Succeeded = true,
                                                CodeOutput = (int)DeriveHomePlenOutput.HomePlanDetermined
                                            };
                                        }
                                    }
                                }
                            }
                        }
                        else // homeplan network on carrier is not BSBS
                        {
                            //CreateTask("Determine Home Plan Carrier: No Homeplan required"
                            //                   , $"Carrier is not part of BCBS. So Homeplan Carrier is not required", caseRecord.Id);
                            return new GatingResponse()
                            {
                                Succeeded = true,
                                CodeOutput = (int)DeriveHomePlenOutput.NoHomePlanDeterminationRequired
                            };
                        }
                    }
                    else //No primary carrier is present
                    {
                        //CreateTask("Determine Home Plan Carrier: No primary Carrier is present"
                        //                       , $"Carrier is not present on the case", caseRecord.Id);
                        RemoveHomePlanCarrier(_service, caseRecord.Id);

                        return new GatingResponse()
                        {
                            Succeeded = false,
                            CodeOutput = (int)DeriveHomePlenOutput.NoPrimaryCarrierIsPresent
                        };
                    }
                }
            }
            return new GatingResponse()
            {
                Succeeded = true,
                CodeOutput = (int)DeriveHomePlenOutput.NoHomePlanDeterminationRequired
            };
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

        private bool IsTaskUnique(string subject, Guid caseId, TaskState state = TaskState.Open)
        {
            var taskQuery = new QueryExpression(Task.EntityLogicalName)
            {
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(Task.Fields.Subject, ConditionOperator.Equal, subject),
                        new ConditionExpression(Task.Fields.RegardingObjectId, ConditionOperator.Equal, caseId),
                        new ConditionExpression(Task.Fields.StateCode, ConditionOperator.Equal, (int)state)
                    }
                }
            };
            return _service.RetrieveMultiple(taskQuery).Entities.Count == 0;
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

        private void CreateNoHomePlanValueFoundTask(TaskManager taskManager, EntityReference regardingObject, string carrierName)
        {
            var taskType = taskManager.GetTaskTypeByName(Constants.TaskTypeNames.HomePlanCouldNotBeDetermined);
            var task = new Task()
            {
                RegardingObjectId = regardingObject,
                ipg_caseid = regardingObject,
                ipg_tasktypeid = taskType.ToEntityReference(),
                Description = string.Format(taskType.ipg_description, carrierName)
            };
            _service.Create(task);
        }

        private void CreateInactiveHomePlanTask(TaskManager taskManager, EntityReference regardingObject, string carrierName)
        {
            var taskType = taskManager.GetTaskTypeByName(Constants.TaskTypeNames.InactiveHomePlan);
            var task = new Task()
            {
                RegardingObjectId = regardingObject,
                ipg_caseid = regardingObject,
                ipg_tasktypeid = taskType.ToEntityReference(),
                Description = string.Format(taskType.ipg_description, carrierName)
            };
            _service.Create(task);
        }
    }
    public enum DeriveHomePlenOutput
    {
        MemberIdIsEmpty = 1,
        MemberIdFormatIsInvalid = 2,
        moreThan5CharacterAlphaPrefix = 3,
        NoNumbersFoundInThePrimaryMemberId = 4,
        InactiveHomePlan = 5,
        NoHomePlanFound = 6,
        HomePlanDetermined = 7,
        NoHomePlanDeterminationRequired = 8,
        NoPrimaryCarrierIsPresent = 9
    }
}