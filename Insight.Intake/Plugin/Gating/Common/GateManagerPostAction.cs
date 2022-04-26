using Insight.Intake.Extensions;
using Insight.Intake.Helpers;
using Insight.Intake.Models;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating.Common
{
    public class GateManagerPostAction
    {
        private Entity target;
        private EntityReference targetRef;
        private EntityReference _caseRef;

        IPluginExecutionContext _context;
        OrganizationServiceContext _crmContext;
        IOrganizationService _service;
        ITracingService _tracingService;
        Managers.TaskManager _taskManager;

        public EntityReference CaseRef
        {
            get => _caseRef;
        }

        public GateManagerPostAction(Entity target, EntityReference caseRef, IOrganizationService service, ITracingService tracingService, IPluginExecutionContext context)
        {
            this.target = target;
            this.targetRef = target.ToEntityReference();
            this._caseRef = caseRef;
            this._service = service;
            this._tracingService = tracingService;
            this._context = context;
            _crmContext = new OrganizationServiceContext(_service);
            _taskManager = new Managers.TaskManager(service, tracingService);
        }

        public void RunPostAction(List<ProcessResult> processResults, ipg_gateconfiguration gateConfiguration, EntityCollection tasksToCreate, bool clearSessionId = false, bool processTasks = true, bool createImportantEventLog = true)
        {
            CloseOutstandingTasksIfSucceeded(processResults, targetRef);

            if (processTasks)
            {
                var taskManager = new TaskManager(_caseRef, targetRef.LogicalName == ipg_referral.EntityLogicalName ? targetRef : null, _service);
                ProcessTasksV3(processResults, targetRef, taskManager);
            }

            var nextGateProcessingRule = GetGateProcessingRule(processResults, gateConfiguration);
            if (nextGateProcessingRule == null)
            {
                _context.OutputParameters["Output"] = "There is no gate processing rule. Can't move forward.";
                return;
            }
            var gateminimumSeverityLevel = GetMinimumSeverityLevel(processResults);
            var gateProcessingResult = GetProcessingResult(processResults);
            if (createImportantEventLog)
            {
                CreateImportantEventLog(gateConfiguration, targetRef, gateminimumSeverityLevel);
            }
            SetStatusesByGateResults(target, nextGateProcessingRule, gateConfiguration, gateminimumSeverityLevel, gateProcessingResult);

            //if (tasksToCreate.Entities.Any() && _caseRef != null)
            if (tasksToCreate.Entities.Any())
            {
                CreateTasks(tasksToCreate, _caseRef == null ? targetRef : _caseRef);
            }

            CreateCaseAndPortalNotes(target, processResults, gateConfiguration);
            if (clearSessionId)
            {
                ClearSessionIdOnCase(target);
            }

            if (gateminimumSeverityLevel < ipg_SeverityLevel.Critical)
            {
                EntityReference reference = targetRef ?? _caseRef;
                if (targetRef != null && targetRef.LogicalName == ipg_referral.EntityLogicalName)
                {
                    var referral = _service.Retrieve(targetRef.LogicalName, target.Id, new ColumnSet(ipg_referral.Fields.ipg_AssociatedCaseId)).ToEntity<ipg_referral>();
                    reference = referral.ipg_AssociatedCaseId == null ? targetRef : referral.ipg_AssociatedCaseId;

                }
                if (gateConfiguration.ipg_successpostprocessid != null)
                {
                    var result = RunPostProcessAction(gateConfiguration.ipg_successpostprocessid, reference, gateminimumSeverityLevel);
                    if (result.Succeeded == false)
                    {
                        throw new InvalidPluginExecutionException($"Error with post processing: {result.Output}");
                    }
                }
            }
        }

        private void CreateImportantEventLog(ipg_gateconfiguration currentGate, EntityReference targetRef, ipg_SeverityLevel minimumSeverityLevel)
        {
            var importantEvent = (from rule in _crmContext.CreateQuery<ipg_gateprocessingrule>()
                                  where rule.ipg_gateconfigurationid.Id == currentGate.Id
                                  && rule.ipg_severitylevel.Value == (int)minimumSeverityLevel
                                  select rule.ipg_ImportantEventConfigId).FirstOrDefault();

            if (importantEvent != null)
            {
                var eventParams = GetImportantEventDescriptionParams(importantEvent?.Name);
                var importantEventLog = new ipg_importanteventslog()
                {
                    ipg_caseid = targetRef.Id.ToString(),
                    ipg_casenumber = targetRef.LogicalName == Incident.EntityLogicalName ? targetRef : null,
                    ipg_casenumbertext = targetRef.Name,
                    ipg_referralid = targetRef.Id.ToString(),
                    ipg_datetimestamp = DateTime.Now,
                    ipg_activity = currentGate.ipg_importantevent_type,
                    ipg_activitydescription = SetEventDescriptionParameters(currentGate.ipg_importantevent_description, eventParams),
                    ipg_name = importantEvent?.Name,
                    ipg_configId = importantEvent?.Name
                };
                _service.Create(importantEventLog);
            }
        }

        public void RunPostActionDelayed()
        {
            var sessionId = target.GetAttributeValue<string>("ipg_cr_gatesessionid");

            var caseGatingLogs = GetCaseGatingLogsBySessionId(sessionId);
            //Document Validation is the first gate configuration detail
            var caseGateExecutionRef = caseGatingLogs.LastOrDefault()?.ipg_CaseGateExecutionId;
            if (caseGateExecutionRef == null)
            {
                throw new InvalidPluginExecutionException("ipg_CaseGateExecutionId is null on case gate execution log");
            }
            var caseGateExecution = _service.Retrieve(caseGateExecutionRef.LogicalName, caseGateExecutionRef.Id, new ColumnSet(ipg_casegateexecution.Fields.ipg_GateConfigurationId)).ToEntity<ipg_casegateexecution>();
            var gateConfigurationRef = caseGateExecution.ipg_GateConfigurationId;
            if (gateConfigurationRef == null)
            {
                throw new InvalidPluginExecutionException("ipg_GateConfigurationId is null on case gate execution");
            }
            var gateConfiguration = _service.Retrieve(gateConfigurationRef.LogicalName, gateConfigurationRef.Id, new ColumnSet(true)).ToEntity<ipg_gateconfiguration>();

            var processResults = GetProcessResultsByCaseGatingLogs(caseGatingLogs);
            var taskManager = new TaskManager(_caseRef == null ? targetRef : _caseRef, targetRef.LogicalName == ipg_referral.EntityLogicalName ? targetRef : null, _service);
            ProcessTasksV3(processResults, targetRef, taskManager);

            var tasksToCreate = GetTasksToCreateByGateConfigDetails(caseGatingLogs, taskManager);
            RunPostAction(processResults, gateConfiguration, tasksToCreate, true, false, false);
        }

        private void ClearSessionIdOnCase(Entity target)
        {
            var targetUpd = new Entity(target.LogicalName, target.Id);
            targetUpd["ipg_cr_gatesessionid"] = null;
            _service.Update(targetUpd);
        }

        private EntityCollection GetTasksToCreateByGateConfigDetails(IEnumerable<ipg_casegateexecutiondetail> caseGatingLogs, TaskManager taskManager)
        {
            EntityCollection tasksToCreate = new EntityCollection();

            foreach (var iGateLog in caseGatingLogs)
            {
                var succeeded = iGateLog.ipg_OutcomeType ?? false;
                if (iGateLog.ipg_WorkflowTaskId == null)
                {
                    continue;
                }

                taskManager.CreateGatingTasksV3(tasksToCreate,
                    succeeded, iGateLog.ipg_WorkflowTaskId,
                    new GatingResponse() { CodeOutput = iGateLog.ipg_CodeOutput ?? 0, TaskDescripton = iGateLog.ipg_TaskDescription },
                    _tracingService);
            }

            return tasksToCreate;
        }

        private List<ProcessResult> GetProcessResultsByCaseGatingLogs(IEnumerable<ipg_casegateexecutiondetail> caseGatingLogs)
        {
            var result = new List<ProcessResult>();
            foreach (var iGateLog in caseGatingLogs)
            {
                result.Add(new ProcessResult()
                {
                    successed = iGateLog.ipg_OutcomeType ?? false,// (bool)response.Results["Succeeded"],
                    workflowTask = iGateLog.ipg_WorkflowTaskId,
                    caseNote = iGateLog.ipg_OutputCRMReason,
                    portalNote = iGateLog.ipg_OutputPortalReason,
                    severityLevel = iGateLog.ipg_SeverityLevel?.Value ?? -1,
                    resultMessage = string.Empty,
                    codeOutput = iGateLog.ipg_CodeOutput ?? 0
                });
            }
            return result;
        }

        private IEnumerable<ipg_casegateexecutiondetail> GetCaseGatingLogsBySessionId(string sessionId)
        {
            return (from cged in _crmContext.CreateQuery<ipg_casegateexecutiondetail>()
                    join cge in _crmContext.CreateQuery<ipg_casegateexecution>() on cged.ipg_CaseGateExecutionId.Id equals cge.ipg_casegateexecutionId
                    where cge.ipg_SessionID == sessionId
                    select cged);
        }

        private void CloseOutstandingTasksIfSucceeded(List<ProcessResult> processResults, EntityReference caseRef)
        {
            var passedGates = processResults
                .Where(p => ((p.severityLevel != (int)ipg_SeverityLevel.Critical && p.severityLevel != (int)ipg_SeverityLevel.Error) || p.successed) && p.gateconfigdetail != null)
                .Select(p => (object)p.gateconfigdetail.Id)
                .ToArray();
            if (passedGates.Any())
            {
                var query = new QueryExpression(Task.EntityLogicalName);
                query.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, caseRef.Id);
                query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)TaskState.Open);
                query.Criteria.AddCondition("ipg_systemtasktypecode", ConditionOperator.Equal, (int)ipg_SystemTaskType.WorkflowTask_ErrororCritical);
                query.Criteria.AddCondition("ipg_gateconfigdetailid", ConditionOperator.In, passedGates);

                var outstandingTasks = _service.RetrieveMultiple(query).Entities;
                Parallel.ForEach(outstandingTasks, iTask =>
                {
                    var updTask = new Task();
                    updTask.Id = iTask.Id;
                    var setStateRequest = new SetStateRequest()
                    {
                        EntityMoniker = iTask.ToEntityReference(),
                        State = new OptionSetValue((int)TaskState.Completed),
                        Status = new OptionSetValue((int)Task_StatusCode.Cancelled)
                    };
                    _service.Execute(setStateRequest);
                });
            }

            CloseMissingInformationHoldPeriodHasExpiredTask(caseRef);
        }

        private void CloseMissingInformationHoldPeriodHasExpiredTask(EntityReference caseRef)
        {
            var query = new QueryExpression(Task.EntityLogicalName);
            query.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, caseRef.Id);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)TaskState.Open);
            query.Criteria.AddCondition("ipg_tasktypecode", ConditionOperator.Equal, (int)ipg_TaskType1.MissingInformationHoldPeriodHasExpired);

            var missingInformationHoldPeriodHasExpired = _service.RetrieveMultiple(query).Entities;

            if (missingInformationHoldPeriodHasExpired.Any())
            {
                query = new QueryExpression(Task.EntityLogicalName);
                query.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, caseRef.Id);
                query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)TaskState.Open);
                query.Criteria.AddCondition("subcategory", ConditionOperator.Like, "%Missing information%");

                var missingInformationTask = _service.RetrieveMultiple(query).Entities.FirstOrDefault();

                if (missingInformationTask == null)
                {
                    missingInformationHoldPeriodHasExpired.ToList().ForEach(
                        task =>
                        {
                            var updTask = new Task();
                            updTask.Id = task.Id;
                            var setStateRequest = new SetStateRequest()
                            {
                                EntityMoniker = task.ToEntityReference(),
                                State = new OptionSetValue((int)TaskState.Completed),
                                Status = new OptionSetValue((int)Task_StatusCode.Cancelled)
                            };
                            _service.Execute(setStateRequest);

                        });
                }
            }

        }

        private Entity GetGateProcessingRule(List<ProcessResult> processResults, Entity gateConfiguration)
        {
            var gateProcessingRules = (from gateRule in _crmContext.CreateQuery<ipg_gateprocessingrule>()
                                       where gateRule.ipg_gateconfigurationid.Id == gateConfiguration.Id
                                       select gateRule).ToList();

            if (gateProcessingRules != null && gateProcessingRules.Count > 0 && processResults.Where(x => x.successed == false).ToList().Count > 0)
            {
                if (processResults.Where(x => x.severityLevel == (int)ipg_SeverityLevel.Critical && x.successed == false).ToList().Count > 0)
                {
                    _tracingService.Trace("There is a critical results");
                    return gateProcessingRules.Where((x => x.ipg_severitylevel.Value == (int)ipg_SeverityLevel.Critical)).FirstOrDefault();
                }
                else if (processResults.Where(x => x.severityLevel == (int)ipg_SeverityLevel.Error && x.successed == false).ToList().Count > 0)
                {
                    _tracingService.Trace("There is an error results");
                    return gateProcessingRules.Where((x => x.ipg_severitylevel.Value == (int)ipg_SeverityLevel.Error)).FirstOrDefault();
                }
                else if (processResults.Where(x => x.severityLevel == (int)ipg_SeverityLevel.Warning && x.successed == false).ToList().Count > 0)
                {
                    _tracingService.Trace("There is an warning results");
                    return gateProcessingRules.Where((x => x.ipg_severitylevel.Value == (int)ipg_SeverityLevel.Warning)).FirstOrDefault();
                }
            }

            _tracingService.Trace("All checks were completed successfully");
            return gateProcessingRules.Where((x => x.ipg_severitylevel.Value == (int)ipg_SeverityLevel.Info)).FirstOrDefault();
        }

        private ipg_SeverityLevel GetMinimumSeverityLevel(List<ProcessResult> processResults)
        {
            if (processResults.Any(p => p.severityLevel == (int)ipg_SeverityLevel.Critical && !p.successed))
            {
                return ipg_SeverityLevel.Critical;
            }
            else if (processResults.Any(p => p.severityLevel == (int)ipg_SeverityLevel.Error && !p.successed))
            {
                return ipg_SeverityLevel.Error;
            }
            else if (processResults.Any(p => p.severityLevel == (int)ipg_SeverityLevel.Warning && !p.successed))
            {
                return ipg_SeverityLevel.Warning;
            }
            return ipg_SeverityLevel.Info;
        }
        public string GetProcessingResult(IEnumerable<ProcessResult> processResults)
        {
            var stringResults = processResults
                .Where(p => !p.successed)
                .Select(p => $"{(p.gateconfigdetail != null ? p.gateconfigdetail.GetAttributeValue<string>("ipg_name") : (p.workflowTask != null ? p.workflowTask.Name : string.Empty))} ({p.resultMessage}) - {((ipg_SeverityLevel)p.severityLevel).ToString()}");
            var result = string.Join("\n", stringResults);
            return result;
        }

        private void SetStatusesByGateResults(Entity target, Entity nextGateProcessingRule, ipg_gateconfiguration gateConfiguration, ipg_SeverityLevel severityLevel, string gateReason)
        {
            _tracingService.Trace($"Starting {System.Reflection.MethodBase.GetCurrentMethod().Name}");

            Entity targetToUpdate = new Entity(target.LogicalName);
            targetToUpdate.Id = target.Id;
            var finalInfoGateResult = severityLevel == ipg_SeverityLevel.Info && nextGateProcessingRule.GetAttributeValue<bool>("ipg_isfinalgate");
            var allowMoveToNextStep = nextGateProcessingRule.GetAttributeValue<EntityReference>("ipg_nextlifecyclestepid") != null && target.GetAttributeValue<EntityReference>("ipg_lifecyclestepid")?.Id != nextGateProcessingRule.GetAttributeValue<EntityReference>("ipg_nextlifecyclestepid")?.Id;
            var nextLifecycleStepRef = nextGateProcessingRule.GetAttributeValue<EntityReference>("ipg_nextlifecyclestepid");

            if (allowMoveToNextStep || finalInfoGateResult)
            {
                if (target.LogicalName == ipg_referral.EntityLogicalName)
                {
                    if (IsGate3(nextLifecycleStepRef))
                    {
                        _tracingService.Trace("Accepting Referral");

                        var dict = new Dictionary<string, object>() { { "CaseState", nextGateProcessingRule.GetAttributeValue<OptionSetValue>("ipg_casestate") } };
                        var response = ExecuteAction("ipg_IPGIntakeActionsAcceptReferral", target.ToEntityReference(), dict);
                        var succeeded = (bool)response.Results["Succeeded"];
                        if (succeeded)
                        {
                            ipg_referral referral = _service.Retrieve(target.LogicalName, target.Id,
                                new ColumnSet(ipg_referral.Fields.ipg_AssociatedCaseId, ipg_referral.Fields.ipg_Origin, ipg_referral.Fields.ipg_ProcedureNameId)).ToEntity<ipg_referral>();
                            if (referral.ipg_AssociatedCaseId != null)
                            {
                                UpdateReferralReasonIfEhrCptOverride(referral);    

                                _caseRef = referral.ipg_AssociatedCaseId;
                                targetToUpdate = new Entity(_caseRef.LogicalName);
                                targetToUpdate.Id = _caseRef.Id;
                                _context.OutputParameters["CaseId"] = _caseRef;
                            }
                        }
                    }
                    else
                    {
                        _tracingService.Trace("Setting referral outcome code");

                        targetToUpdate[ipg_referral.Fields.ipg_OutcomeCode] =
                            nextGateProcessingRule.ToEntity<ipg_gateprocessingrule>().ipg_casereasonEnum == ipg_CaseReasons.Rejected
                            ? new OptionSetValue((int)ipg_OutcomeCodes.Rejected)
                            : null;
                    }
                }
                else
                {
                    targetToUpdate.Id = target.Id;
                    if (target.GetAttributeValue<bool>(nameof(Incident.ipg_casehold).ToLower()) && String.Equals(gateConfiguration.Id.ToString(), D365Helpers.GetGlobalSettingValueByKey(_service, "Gate 11"), StringComparison.OrdinalIgnoreCase))
                    {
                        _tracingService.Trace("Case is on hold. Return");

                        _context.OutputParameters["Output"] = "Case is on hold";
                        return;
                    }
                }

                if (!finalInfoGateResult)
                {
                    _tracingService.Trace("Setting next lifecycle");

                    targetToUpdate["ipg_lifecyclestepid"] = nextLifecycleStepRef;
                    var nextLifecycleStep = _service.Retrieve(nextLifecycleStepRef.LogicalName, nextLifecycleStepRef.Id, new ColumnSet("ipg_gateconfigurationid"));
                    targetToUpdate["ipg_gateconfigurationid"] = nextLifecycleStep.GetAttributeValue<EntityReference>("ipg_gateconfigurationid");
                }

                if (finalInfoGateResult && target.GetAttributeValue<bool>("ipg_casehold"))
                {
                    _tracingService.Trace("Setting output = Case is on hold");

                    _context.OutputParameters["Output"] = "Case is on hold";
                }
                else
                {
                    _tracingService.Trace("Setting values from gate configuration on passed");

                    targetToUpdate["ipg_caseoutcome"] = gateConfiguration.GetAttributeValue<OptionSetValue>("ipg_caseoutcomepassed");
                    targetToUpdate["ipg_decisionby"] = gateConfiguration.GetAttributeValue<OptionSetValue>("ipg_decisionbypassed");
                    if(target.LogicalName != ipg_referral.EntityLogicalName)
                    {
                        _context.OutputParameters["Succeeded"] = true;
                    }
                }
            }
            else
            {
                if (target.LogicalName == ipg_referral.EntityLogicalName)
                {
                    _tracingService.Trace("Rejecting Referral");
                    targetToUpdate["ipg_outcomecode"] = new OptionSetValue((int)ipg_OutcomeCodes.Rejected);
                }

                _tracingService.Trace("Setting values from gate configuration on failed");
                targetToUpdate["ipg_caseoutcome"] = gateConfiguration.GetAttributeValue<OptionSetValue>("ipg_caseoutcomefailed");
                targetToUpdate["ipg_decisionby"] = gateConfiguration.GetAttributeValue<OptionSetValue>("ipg_decisionbyfailed");
                //_context.OutputParameters["Output"] = $"Some WT have Error or Critical severity levels. Please fix the issues and then run Gating again.\n{gateReason}";
            }



            _tracingService.Trace("Setting target statuses from gate results");
            targetToUpdate["ipg_statecode"] = nextGateProcessingRule.GetAttributeValue<OptionSetValue>("ipg_casestate");
            targetToUpdate["ipg_casestatusdisplayedid"] = nextGateProcessingRule.GetAttributeValue<EntityReference>("ipg_casestatusdisplayedid");
            //targetToUpdate["ipg_casestatus"] = nextGateProcessingRule.GetAttributeValue<OptionSetValue>("ipg_casestatus");
            targetToUpdate["ipg_gateoutcome"] = new OptionSetValue((int)severityLevel);
            targetToUpdate["ipg_lastgaterunid"] = gateConfiguration.ToEntityReference();
            targetToUpdate["ipg_lgereason"] = gateReason;

            if (nextGateProcessingRule.GetAttributeValue<OptionSetValue>("ipg_casereason") != null)
            {
                targetToUpdate["ipg_reasons"] = nextGateProcessingRule.GetAttributeValue<OptionSetValue>("ipg_casereason");
            }
            if (nextGateProcessingRule.GetAttributeValue<OptionSetValue>("ipg_providerstatus") != null)
            {
                targetToUpdate["ipg_providerstatus"] = nextGateProcessingRule.GetAttributeValue<OptionSetValue>("ipg_providerstatus");
            }

            _service.Update(targetToUpdate);   

            if (nextGateProcessingRule.GetAttributeValue<OptionSetValue>("ipg_casestatus")?.Value == (int)ipg_CaseStatus.Closed && !string.Equals(gateConfiguration.ipg_name, "Gate 1", StringComparison.OrdinalIgnoreCase) && !string.Equals(gateConfiguration.ipg_name, "Gate 2", StringComparison.OrdinalIgnoreCase))
            {
                var closeCaseRequest = new ipg_IPGCaseCloseCaseRequest()
                {
                    Target = targetToUpdate.ToEntityReference(),
                    SkipChecks = false,
                    FillTargetFields = false
                };
                _service.Execute(closeCaseRequest);            
            }
        }

        private bool IsGate3(EntityReference nextLifeCyclestepRef)
        {
            var gateconfigRef = nextLifeCyclestepRef != null ?
                _service.Retrieve(nextLifeCyclestepRef.LogicalName, nextLifeCyclestepRef.Id, new ColumnSet(ipg_lifecyclestep.Fields.ipg_gateconfigurationid)).ToEntity<ipg_lifecyclestep>().ipg_gateconfigurationid : null;
            var gateConfigName = gateconfigRef.Name ?? _service.Retrieve(gateconfigRef.LogicalName, gateconfigRef.Id, new ColumnSet(ipg_gateconfiguration.Fields.ipg_name)).ToEntity<ipg_gateconfiguration>().ipg_name;

            return "Gate 3".Equals(gateConfigName);
        }

        public void CreateTasks(EntityCollection tasks, EntityReference caseRef)
        {
            foreach (Task task in tasks.Entities.Select(t => t.ToEntity<Task>()))
            {
                if (caseRef != null)
                {
                    task.RegardingObjectId = caseRef;
                }
                var existingTask = FindExistingTask(task);
                if (existingTask != null)
                {
                    var updTask = new Task()
                    {
                        Id = existingTask.Id,
                        Subject = task.GetAttributeValue<string>(Task.Fields.Subject),
                        Description = task.GetAttributeValue<string>(Task.Fields.Description)
                    };
                    _service.Update(updTask);
                }
                else
                {
                    if (task.Contains(Task.Fields.OwnerId) && task.GetAttributeValue<EntityReference>(Task.Fields.OwnerId) == null)
                    {
                        task.Attributes.Remove(Task.Fields.OwnerId);
                    }
                    _tracingService.Trace($"owner id is {task.GetAttributeValue<EntityReference>(Task.Fields.OwnerId)?.Id}, {task.GetAttributeValue<string>(Task.Fields.Subject)}");

                    //_taskManager.CreateTask require task type id to keep old task creation work jsut create task.
                    if (task.ipg_tasktypeid != null)
                    {
                        _taskManager.CreateTask(caseRef, task);
                    }
                    else
                    {
                        _service.Create(task);
                    }
                }
            }
        }

        private Task FindExistingTask(Task task)
        {
            if (task == null || task.RegardingObjectId == null)
            {
                return null;
            }
            if (task.ipg_gatingoutcomeid == null && task.ipg_tasktypecode == null)
            {
                return null;
            }
            var query = new QueryExpression
            {
                EntityName = Task.EntityLogicalName,
                ColumnSet = new ColumnSet(Task.PrimaryIdAttribute),
                Criteria = {
                    Conditions = {
                        new ConditionExpression(Task.Fields.StateCode, ConditionOperator.Equal, (int)TaskState.Open),
                        new ConditionExpression(Task.Fields.RegardingObjectId, ConditionOperator.Equal, task.RegardingObjectId.Id),
                        task.ipg_gatingoutcomeid != null
                            ? new ConditionExpression(Task.Fields.ipg_gatingoutcomeid, ConditionOperator.Equal, task.ipg_gatingoutcomeid.Id)
                            : task.ipg_tasktypecode != null ?
                            new ConditionExpression(Task.Fields.ipg_tasktypecode, ConditionOperator.Equal, task.ipg_tasktypecode.Value)
                            : null
                    }
                }
            };
            return _service.RetrieveMultiple(query).Entities.FirstOrDefault()?.ToEntity<Task>();
        }

        private void CreateCaseAndPortalNotes(Entity target, List<ProcessResult> processResults, ipg_gateconfiguration gateConfiguration)
        {
            var caseNote = String.Join(System.Environment.NewLine, processResults.Where(x => !string.IsNullOrEmpty(x.caseNote)).Select(x => x.caseNote));
            var portalNote = String.Join(System.Environment.NewLine, processResults.Where(x => !string.IsNullOrEmpty(x.portalNote)).Select(x => x.portalNote));
            if (!string.IsNullOrEmpty(portalNote.Trim()))
            {
                CreatePortalComment(target, portalNote);
            }

            if (!string.IsNullOrEmpty(caseNote.Trim()))
            {
                CreateCaseNote(target, caseNote);
            }
        }

        private void CreateCaseNote(Entity target, string message)
        {
            var annotation = new Annotation();
            annotation.ObjectId = target.ToEntityReference();
            annotation.ObjectTypeCode = target.LogicalName;
            annotation.Subject = "Gate process results";
            annotation.NoteText = message;
            _service.Create(annotation);
        }



        private void CreatePortalComment(Entity target, string message)
        {
            if (target.Contains("ipg_facilityid"))
            {
                var accountContacts = (from accountContact in _crmContext.CreateQuery<ipg_contactsaccounts>()
                                       where accountContact.ipg_accountid.Id == target.GetAttributeValue<EntityReference>("ipg_facilityid").Id
                                       && accountContact.ipg_contactid != null
                                       select new ActivityParty { PartyId = accountContact.ipg_contactid }).ToList();

                if (accountContacts.Count > 0)
                {
                    var portalComment = new adx_portalcomment();

                    portalComment.RegardingObjectId = target.ToEntityReference();
                    portalComment.From = new List<ActivityParty>() { new ActivityParty() { PartyId = new EntityReference(SystemUser.EntityLogicalName, _context.InitiatingUserId) } };
                    portalComment.To = accountContacts;
                    portalComment.Subject = "Gating process message";
                    portalComment.Description = message;

                    _service.Create(portalComment);
                }
            }
        }
        private OrganizationResponse ExecuteAction(string processUniqueName, EntityReference targetRef, Dictionary<string, object> parameters = null)
        {
            OrganizationRequest organizationRequest = new OrganizationRequest(processUniqueName);
            organizationRequest["Target"] = targetRef;
            if (parameters != null)
            {
                foreach (var iParam in parameters)
                {
                    organizationRequest[iParam.Key] = iParam.Value;
                }
            }
            OrganizationResponse response = _service.Execute(organizationRequest);

            return response;
        }

        private void ProcessTasksV3(List<ProcessResult> processResults, EntityReference targetRef, TaskManager taskManager)
        {
            foreach (var processResult in processResults)
            {
                if (processResult.workflowTask == null)
                {
                    return;
                }

                var configurations = (from wtoc in _crmContext.CreateQuery<ipg_workflowtaskoutputconfig>()
                                      join tc in _crmContext.CreateQuery<ipg_taskconfiguration>() on wtoc.ipg_TaskConfigurationId.Id equals tc.Id
                                      where wtoc.ipg_WorkflowTaskId.Id == processResult.workflowTask.Id
                                      && tc.StateCode == ipg_taskconfigurationState.Active
                                      select new { taskConfiguration = tc, outcomeType = wtoc.ipg_OutcomeType, taskType = tc.ipg_tasktypeid });
                foreach (var configuration in configurations)
                {
                    var openedTasks = taskManager.GetOpenTasksV3(configuration.taskType, targetRef, processResult.workflowTask);
                    foreach (var task in openedTasks)
                    {
                        if (processResult.successed && configuration.outcomeType == false) //Positive Outcome
                        {
                            taskManager.CancelTask(task.ToEntityReference());
                        }
                        else if (!processResult.successed && configuration.outcomeType == true) // Negative Outcome
                        {
                            if (task.ipg_tasktypeid != null)
                            {
                                var taskTypeRef = taskManager.GetWorkflowTaskTaskType(processResult.workflowTask, processResult.successed, processResult.codeOutput);
                                if (taskTypeRef == null || !Guid.Equals(taskTypeRef.Id, task.ipg_tasktypeid))
                                {
                                    var updTask = new Task()
                                    {
                                        Id = task.Id,
                                        Description = "Replaced with a new task"
                                    };
                                    _service.Update(updTask);
                                    taskManager.CancelTask(task.ToEntityReference());
                                }
                            }
                            else
                            {
                                var updTask = new Task()
                                {
                                    Id = task.Id,
                                    Description = task.Description + "\r\n" + processResult.caseNote
                                };
                                _service.Update(updTask);
                            }
                        }
                    }
                }
            }
        }

        private PostProcessResult RunPostProcessAction(EntityReference processRef, EntityReference caseRef, ipg_SeverityLevel minimumSeverityLevel)
        {
            var process = _service.Retrieve(processRef.LogicalName, processRef.Id, new ColumnSet("uniquename"));
            var paramCollection = new Dictionary<string, object>() {
                { "MinimumSeverityLevel",(int)minimumSeverityLevel}
            };
            var response = ExecuteAction($"ipg_{process.GetAttributeValue<string>("uniquename")}", caseRef, paramCollection);
            return new PostProcessResult()
            {
                Succeeded = response.Results["Succeeded"] as bool?,
                Output = response.Results["Output"] as string
            };
        }

        public class PostProcessResult
        {
            public bool? Succeeded { get; set; }
            public string Output { get; set; }
        }

        public void StartCasePostGateExecution(Guid? caseGateExecutionId)
        {
            if (caseGateExecutionId != null)
            {
                var caseGateExecution = new ipg_casegateexecution()
                {
                    Id = (Guid)caseGateExecutionId,
                    ipg_StartDatetimePostExecution = DateTime.Now
                };
                _service.Update(caseGateExecution);
            }
        }

        public void FinishCasePostGateExecution(Guid? caseGateExecutionId)
        {
            if (caseGateExecutionId != null)
            {
                var now = DateTime.Now;
                var caseGateExecution = new ipg_casegateexecution()
                {
                    Id = (Guid)caseGateExecutionId,
                    ipg_EndDatetimePostExecution = now,
                    ipg_EndDatetime = now
                };
                _service.Update(caseGateExecution);
            }
        }

        private void UpdateReferralReasonIfEhrCptOverride(ipg_referral referral)
        {
            if (referral.ipg_OriginEnum == Incident_CaseOriginCode.EHR && referral.ipg_ProcedureNameId != null)
            {
                var defaultProcedureIdString = D365Helpers.GetGlobalSettingValueByKey(_service, GlobalSettingConstants.DefaultProcedureIdSettingName);
                if (Guid.TryParse(defaultProcedureIdString, out Guid defaultProcedureId))
                {
                    if (referral.ipg_ProcedureNameId.Id == defaultProcedureId)
                    {
                        referral.ipg_ReasonsEnum = ipg_CaseReasons.EHRCPTOverrideCaseCreated;
                        _service.Update(referral);
                    }
                }
            }
        }

        public string[] GetImportantEventDescriptionParams(string configId)
        {
            string[] eventDescriptionParam;
            switch (configId)
            {
                case "ET14":
                    eventDescriptionParam = new string[] { Enum.GetName(typeof(Incident_CaseOriginCode), this.target.GetAttributeValue<OptionSetValue>("ipg_origin").Value) };
                    break;
                default:
                    eventDescriptionParam = null;
                    break;
            }
            return eventDescriptionParam;
        }

        private string SetEventDescriptionParameters(string eventDescription, string[] eventDescriptionParam)
        {
            if (eventDescriptionParam != null)
            {
                Regex pattern = new Regex("<.*?>");
                foreach (var param in eventDescriptionParam)
                {
                    if (!String.IsNullOrEmpty(param))
                        eventDescription = pattern.Replace(eventDescription, param, 1);
                }
            }
            return eventDescription;
        }
    }
}