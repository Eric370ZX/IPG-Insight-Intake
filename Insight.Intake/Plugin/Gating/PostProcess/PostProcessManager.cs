using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Insight.Intake.Helpers;
using Insight.Intake.Extensions;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk.Client;
using Insight.Intake.Repositories;
using static Insight.Intake.Helpers.Constants;

namespace Insight.Intake.Plugin.Gating.PostProcess
{
    public class PostProcessManager
    {
        private readonly IOrganizationService _crmService;
        private readonly ITracingService _tracingService;
        private readonly EntityReference _caseRef;
        private readonly OrganizationServiceContext _crmContext;
        private readonly PatientStatementTaskRepository _PSTaskRepo;
        private readonly DocumentRepository _docRepo;

        public PostProcessManager(IOrganizationService organizationService, ITracingService tracingService, EntityReference targetRef)
        {
            this._crmService = organizationService;
            this._tracingService = tracingService;
            this._caseRef = targetRef;
            this._crmContext = new OrganizationServiceContext(this._crmService);
            _PSTaskRepo = new PatientStatementTaskRepository(this._crmService, this._tracingService);
            _docRepo = new DocumentRepository(this._crmService);
        }

        public ipg_statementgenerationtask GetSystemTask(string systemTaskSubject)
        {
            var fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                  <entity name='ipg_statementgenerationtask'>
                    <attribute name='ipg_statementgenerationtaskid' />
                    <attribute name='ipg_name' />
                    <attribute name='createdon' />
                    <order attribute='ipg_name' descending='false' />
                    <filter type='and'>
                      <condition attribute='ipg_caseid' operator='eq' value='{_caseRef.Id}' />
                      <condition attribute='statecode' operator='eq' value='0' />
                      <condition attribute='ipg_name' operator='eq' value='{systemTaskSubject}' />
                    </filter>
                  </entity>
                </fetch>";
            var targetTask = _crmService.RetrieveMultiple(new FetchExpression(fetch))
                .Entities
                .FirstOrDefault()
                ?.ToEntity<ipg_statementgenerationtask>();
            return targetTask;
        }

        internal void RunGating(EntityReference targetRef)
        {
            var actionParams = new Dictionary<string, object>() { { "Target", targetRef } };
            _crmService.ExecuteAction(Insight.Intake.Helpers.Constants.ActionNames.GatingStartGateProcessing, actionParams);
        }

        public IEnumerable<Task> GetTasks(ipg_TaskType1 taskType)
        {
            var fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                          <entity name='task'>
                            <all-attributes />
                            <order attribute='subject' descending='false' />
                            <filter type='and'>
                              <condition attribute='regardingobjectid' operator='eq' uitype='incident' value='{_caseRef.Id}' />
                              <condition attribute='ipg_tasktypecode' operator='eq' value='{(int)taskType}' />
                            </filter>
                          </entity>
                        </fetch>";
            var docs = _crmService.RetrieveMultiple(new FetchExpression(fetchXml))
               .Entities
               .Select(p => p.ToEntity<Task>());
            return docs;
        }

        public IEnumerable<Task> GetOpenTasks(ipg_tasktype taskType)
        {
            if (taskType == null)
            {
                return Enumerable.Empty<Task>();
            }
            return (from task in _crmContext.CreateQuery<Task>()
                    where task.StateCode == TaskState.Open
                    && task.RegardingObjectId.Id == _caseRef.Id
                    && task.ipg_tasktypeid.Id == taskType.Id
                    orderby task.Subject descending
                    select task);
        }

        public void CloseSystemTask(EntityReference taskRef, string description)
        {
            var targetTask = new ipg_statementgenerationtask()
            {
                Id = taskRef.Id,
                StateCode = ipg_statementgenerationtaskState.Inactive,
                StatusCodeEnum = ipg_statementgenerationtask_StatusCode.Completed,
                ["ipg_description"] = description
            };
            _crmService.Update(targetTask);
        }
        public void CloseOutstandingUserTasks(IEnumerable<Task> tasks, string closureNote)
        {
            foreach (var iTask in tasks)
            {
                var updtask = new Task()
                {
                    Id = iTask.Id,
                    StateCode = TaskState.Completed,
                    StatusCodeEnum = Task_StatusCode.Cancelled,
                    ipg_closurenote = closureNote,
                };
                _crmService.Update(updtask);
            }
        }
        public Intake.Account GetCarrier()
        {
            var fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                          <entity name='account'>
                            <attribute name='name' />
                            <attribute name='accountid' />
                            <order attribute='name' descending='false' />
                            <link-entity name='incident' from='ipg_carrierid' to='accountid' link-type='inner' alias='aa'>
                              <filter type='and'>
                                <condition attribute='incidentid' operator='eq' value='{_caseRef.Id}' />
                              </filter>
                            </link-entity>
                          </entity>
                        </fetch>";
            var accounts = _crmService.RetrieveMultiple(new FetchExpression(fetch)).Entities;
            return accounts.FirstOrDefault()?.ToEntity<Intake.Account>();
        }

        public Intake.Account GetFacility()
        {
            var fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                          <entity name='account'>
                            <attribute name='ipg_claimholddays' />
                            <attribute name='ipg_os_claimhold' />
                            <attribute name='accountid' />                            
                            <link-entity name='incident' from='ipg_facilityid' to='accountid' link-type='inner' alias='aa'>
                              <filter type='and'>
                                <condition attribute='incidentid' operator='eq' value='{_caseRef.Id}' />
                              </filter>
                            </link-entity>
                          </entity>
                        </fetch>";
            var accounts = _crmService.RetrieveMultiple(new FetchExpression(fetch)).Entities;
            return accounts.FirstOrDefault()?.ToEntity<Intake.Account>();
        }

        public Team GetTeam(string teamName)
        {
            var query = new QueryExpression(Team.EntityLogicalName);
            query.Criteria.AddCondition("name", ConditionOperator.Equal, teamName);
            var teams = _crmService.RetrieveMultiple(query).Entities;
            return teams.FirstOrDefault()?.ToEntity<Team>();
        }

        internal void CreateUserTask(string subject, string body, int followUpTask, EntityReference owner, EntityReference objectId, ipg_TaskType1? taskType = null)
        {
            var query = new QueryExpression(Task.EntityLogicalName);
            query.ColumnSet = new ColumnSet(false);
            query.Criteria.AddCondition(Task.Fields.Subject, ConditionOperator.BeginsWith, subject);
            query.Criteria.AddCondition(Task.Fields.StateCode, ConditionOperator.Equal, (int)TaskState.Open);
            query.Criteria.AddCondition(Task.Fields.RegardingObjectId, ConditionOperator.Equal, objectId.Id);
            if (taskType.HasValue)
            {
                query.Criteria.AddCondition(Task.Fields.ipg_tasktypecode, ConditionOperator.Equal, taskType.Value.ToOptionSetValue().Value);
            }
            if (owner != null)
            {
                query.Criteria.AddCondition(Task.Fields.OwnerId, ConditionOperator.Equal, owner.Id);
            }
            var existingTasks = _crmService.RetrieveMultiple(query).Entities;

            if (existingTasks.Count == 0)
            {
                var targetTask = new Task()
                {
                    RegardingObjectId = objectId,
                    Subject = subject,
                    Description = body,
                    ScheduledStart = DateTime.Now.AddDays(followUpTask),
                    ipg_tasktypecodeEnum = taskType
                };
                if (owner != null)
                {
                    targetTask.OwnerId = owner;
                }
                _crmService.Create(targetTask);
            }
        }

        internal void CreateGenerateSubmitClaimTask(DateTime ScheduledStartDate, EntityReference owner, bool isSecondaryCarrier, string taskReason = null)
        {
            new TaskManager(_crmService, _tracingService, null, Guid.Empty)
                .CreateGenerateSubmitClaimTask(_caseRef.Id, ScheduledStartDate, owner, isSecondaryCarrier, taskReason);
        }

        internal void ScheduleGenerateSubmitClaimTask()
        {
            var taskReasonName = TaskManager.TaskReasonsNames.NoClaimsHoldReason;
            var incident = _crmService.Retrieve(Incident.EntityLogicalName, _caseRef.Id,
                new ColumnSet(Incident.Fields.ipg_ActualDOS, Incident.Fields.ipg_deductibleremaining, Incident.Fields.ipg_SurgeryDate, Incident.Fields.ipg_CarrierId, Incident.Fields.ipg_SecondaryCarrierId, Incident.Fields.ipg_FacilityId)).ToEntity<Incident>();

            var surgeryDate = incident.ipg_SurgeryDate ?? incident.ipg_ActualDOS ?? DateTime.Now;
            var isSecondaryCarrier = incident.ipg_CarrierId == null && incident.ipg_SecondaryCarrierId != null;

            var existingGenerateSubmitClaimTask = GetGenerateSubmitClaimTask();
            if (existingGenerateSubmitClaimTask == null)
            {
                int numberOfHoldDaysInt = 0;

                Intake.Account facility = incident.ipg_FacilityId != null
                    ? _crmService.Retrieve(Intake.Account.EntityLogicalName, incident.ipg_FacilityId.Id
                        , new ColumnSet(Intake.Account.Fields.AccountId, Intake.Account.Fields.ipg_ClaimHoldDays, Intake.Account.Fields.ipg_os_claimhold))
                            ?.ToEntity<Intake.Account>()
                    : null;

                if (facility?.ipg_os_claimhold?.Value == (int)Account_ipg_os_claimhold.Yes)
                {
                    numberOfHoldDaysInt = facility.ipg_ClaimHoldDays.Value;
                    taskReasonName = TaskManager.TaskReasonsNames.ClaimHoldFacilityRequests;
                }
                else
                {
                    var patientsRemainingDeductibleString = D365Helpers.GetGlobalSettingValueByKey(_crmService, "DelayedBilling.PatientsRemainingDeductible", "0");
                    int patientsRemainingDeductibleInt = Int32.Parse(patientsRemainingDeductibleString);
                    _tracingService.Trace(" patientsRemainingDeductibleInt: " + patientsRemainingDeductibleInt + " vs refIncident.ipg_deductibleremaining.Value: " + incident.ipg_deductibleremaining?.Value);

                    if (incident.ipg_deductibleremaining?.Value > patientsRemainingDeductibleInt)
                    {
                        var numberOfHoldDaysString = D365Helpers.GetGlobalSettingValueByKey(_crmService, "DelayedBilling.NumberOfHoldDays", "0");
                        numberOfHoldDaysInt = Int32.Parse(numberOfHoldDaysString);
                        taskReasonName = TaskManager.TaskReasonsNames.ClaimHoldPatientDeductible;
                    }
                }

                _tracingService.Trace(" numberOfHoldDaysInt: " + numberOfHoldDaysInt);
                var scheduledStartDate = surgeryDate.AddDays(numberOfHoldDaysInt);
                CreateGenerateSubmitClaimTask(scheduledStartDate, null, isSecondaryCarrier, taskReasonName);
            }
        }

        public Task GetGenerateSubmitClaimTask()
        {
            var taskManager = new TaskManager(_crmService, _tracingService);
            var taskType = taskManager.GetTaskTypeByName("Generate and Submit Claim");
            return GetOpenTasks(taskType).FirstOrDefault<Task>();
        }

        public PostProcessResult SetCollectionDate()
        {
            var result = new PostProcessResult(true, "");
            try
            {
                var sourceCase = _crmService.Retrieve(_caseRef.LogicalName, _caseRef.Id, new ColumnSet(Incident.Fields.ipg_collectiondate)).ToEntity<Incident>();
                if (sourceCase.ipg_collectiondate == null)
                {
                    var updEntity = new Incident()
                    {
                        Id = sourceCase.Id,
                        ipg_collectiondate = DateTime.UtcNow
                    };
                    _crmService.Update(updEntity);
                }
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Output = ex.Message;
            }
            return result;
        }

        public PostProcessResult SetBillingDate()
        {
            var result = new PostProcessResult(true, "");
            try
            {
                var sourceCase = _crmService.Retrieve(_caseRef.LogicalName, _caseRef.Id, new ColumnSet(Incident.Fields.ipg_billingdate)).ToEntity<Incident>();
                if (sourceCase.ipg_billingdate == null)
                {
                    var updEntity = new Incident()
                    {
                        Id = sourceCase.Id,
                        ipg_billingdate = DateTime.UtcNow
                    };
                    _crmService.Update(updEntity);
                }
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Output = ex.Message;
            }
            return result;
        }

        public void CreateNote(string subject, string noteText)
        {
            var note = new Annotation()
            {
                ObjectId = _caseRef,
                Subject = subject,
                NoteText = noteText
            };
            _crmService.Create(note);
        }

        #region statement generation tasks

        public void CreateStatementGenerationTaskIfNotExists(EntityReference incidentReference, string eventName)
        {
            _PSTaskRepo.CreateStatementGenerationTaskIfNotExists(incidentReference, eventName);
        }

        public void CreateCarrierPaymentStatementGenerationTask(EntityReference incidentReference)
        {
            _PSTaskRepo.CreateStatementGenerationTaskIfNotExists(incidentReference, PSEvents.CarrierPayment);
        }

        public void CancelStatementTasks(EntityReference incidentReference, params string[] statementNames)
        {
            _PSTaskRepo.CancelStatementTasks(incidentReference, statementNames);
        }

        public void UpdateStatementTasksStartAndDueDate(EntityReference incidentReference)
        {
            var statements = _PSTaskRepo.GetActiveStatements(incidentReference);
            var now = DateTime.Now;
            foreach(var statement in statements)
            {
                var updatedStatement = new ipg_statementgenerationtask
                {
                    Id = statement.Id,
                    ipg_StartDate = now,
                    ipg_EndDate = now
                };
                _crmService.Update(updatedStatement);
            }
        }
        public void DeactivateStatementRecordsExceptP1(EntityReference incidentReference)
        {
            _docRepo.DeactivatePSDocsExceptP1(incidentReference);
        }

        #endregion

        internal void UpdateLifecycleStepAndCaseState()
        {
            var query = new QueryExpression(ipg_claimgenerationoverride.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                TopCount = 1,
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_claimgenerationoverride.Fields.ipg_caseid, ConditionOperator.Equal, _caseRef.Id),
                        new ConditionExpression(ipg_claimgenerationoverride.Fields.ipg_claimtogenerate,  ConditionOperator.Equal, (int)ipg_claimgenerationoverride_ipg_claimtogenerate.NoClaim)
                    }
                },
            };
            var ec = _crmService.RetrieveMultiple(query);
            if (ec.Entities.Count > 0)
            {
                var updEntity = new Incident()
                {
                    Id = _caseRef.Id,
                    ipg_lifecyclestepid = new EntityReference(ipg_lifecyclestep.EntityLogicalName, new Guid(_crmService.GetGlobalSettingValueByKey(Settings.Collection_LF_Step_Id)?.ToLower())),
                    ipg_StateCodeEnum = ipg_CaseStateCodes.CarrierServices,
                    ipg_casestatusdisplayedid = new EntityReference(ipg_casestatusdisplayed.EntityLogicalName, CaseStatusDisplayedGuids.CarrierCollectionsInProgress),
                    ipg_ReasonsEnum = ipg_CaseReasons.RevenueBookedClaimSubmitted
                };
                _crmService.Update(updEntity);
                var taskManager = new TaskManager(_crmService, _tracingService);
                taskManager.CreateTask(_caseRef, TaskManager.TaskTypeIds.CHECK_CARRIER_BALANCE);
            }
            else
            {
                ScheduleGenerateSubmitClaimTask();
            }
        }
    }
    public class PostProcessResult
    {
        public PostProcessResult(bool succeeded, string output)
        {
            Succeeded = succeeded;
            Output = output;
        }

        public bool Succeeded { get; set; }
        public string Output { get; set; }
    }
}
