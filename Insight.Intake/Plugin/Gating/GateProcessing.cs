using Insight.Intake.Plugin.Gating.Common;
using Insight.Intake.Plugin.Gating.WFTGroups;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    public class GateProcessing : PluginBase
    {
        ITracingService _tracingService;
        OrganizationServiceContext _crmContext;
        IPluginExecutionContext _context;
        IOrganizationService _service;
        string _sessionId;

        EntityReference _caseRef;
        EntityReference _referralRef;

        public GateProcessing() : base(typeof(GateProcessing))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingStartGateProcessing", null, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGRunGateForCase", null, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            //Extract the tracing service for use in debugging sandboxed plug-ins. 
            _tracingService = localPluginContext.TracingService;
            _context = localPluginContext.PluginExecutionContext;
            _service = localPluginContext.OrganizationService;
            _crmContext = new OrganizationServiceContext(_service);
            _sessionId = Guid.NewGuid().ToString();

            if (_context.InputParameters.Contains("Target"))
            {
                try
                {
                    EntityReference targetRef = _context.InputParameters["Target"] as EntityReference;
                    var gateConfigurationRef = _context.InputParameters.Contains("Gate") ? _context.InputParameters["Gate"] as EntityReference : null;
                    _context.OutputParameters["Succeeded"] = false;
                    _context.OutputParameters["AllowReject"] = false;

                    Entity target = _service.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet(true));
                    var gateManager = new GateManager(_service, _tracingService, targetRef);
                    var caseNumber = gateManager.GetCaseNumber(target);
                    var caseGateExecutionId = CreateCaseGateExecution(targetRef, gateConfigurationRef, caseNumber, _sessionId);
                    var isManual = _context.InputParameters.Contains("IsManual") ? (bool)_context.InputParameters["IsManual"] : false;

                    if (gateConfigurationRef == null)
                    {
                        if (!target.Contains("ipg_lifecyclestepid"))
                        {
                            _context.OutputParameters["Output"] = "Lifecycle step field is empty. Please contact administrator to resolve this issue.";
                            FailedGatingPostProcess(caseGateExecutionId);
                            return;
                        }

                        if (target.LogicalName == Incident.EntityLogicalName)
                        {
                            _caseRef = target.ToEntityReference();
                            _referralRef = target.Contains(Incident.Fields.ipg_ReferralId) ? target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_ReferralId) : null;
                        }
                        else if (target.LogicalName == ipg_referral.EntityLogicalName)
                        {
                            _caseRef = target.Contains("ipg_associatedcaseid") ? target.GetAttributeValue<EntityReference>("ipg_associatedcaseid") : null;
                            _referralRef = target.ToEntityReference();
                            if (target.GetAttributeValue<OptionSetValue>(ipg_referral.Fields.ipg_casestatus).Value == (int)ipg_CaseStatus.Closed)
                            {
                                _context.OutputParameters["Output"] = "Referral is closed.";
                                if (target.FormattedValues.Contains(ipg_referral.Fields.ipg_Reasons))
                                {
                                    var reason = target.FormattedValues[ipg_referral.Fields.ipg_Reasons];
                                    _context.OutputParameters["Output"] += " Reason: " + reason;
                                }
                                FailedGatingPostProcess(caseGateExecutionId);
                                return;
                            }
                        }

                        var lifecycleStepRef = target.GetAttributeValue<EntityReference>("ipg_lifecyclestepid");
                        var lifecycleStep = _service.Retrieve(lifecycleStepRef.LogicalName, lifecycleStepRef.Id, new ColumnSet("ipg_gateconfigurationid"));
                        int? status = (target.LogicalName == Incident.EntityLogicalName ? _service.Retrieve(target.LogicalName, target.Id, new ColumnSet(Incident.Fields.ipg_StateCode)).ToEntity<Incident>().ipg_StateCode?.Value : _service.Retrieve(target.LogicalName, target.Id, new ColumnSet(ipg_referral.Fields.ipg_statecode)).ToEntity<ipg_referral>().ipg_statecode?.Value);
                        if (status.HasValue)
                        {
                            gateConfigurationRef = (from gateDetermination in _crmContext.CreateQuery<ipg_gatedetermination>()
                                                    where gateDetermination.ipg_LifecycleStepId.Id == lifecycleStepRef.Id
                                                    && gateDetermination.ipg_CaseState.Value == status
                                                    && gateDetermination.ipg_TriggeredBy == isManual
                                                    select gateDetermination.ipg_GateConfigurationId).FirstOrDefault();
                        }
                    }

                    if (gateConfigurationRef == null)
                    {
                        _context.OutputParameters["Output"] = "Gate Configuration field is empty. Please contact administrator to resolve this issue.";
                        FailedGatingPostProcess(caseGateExecutionId);
                        return;
                    }
                    UpdateGateConfigurationCaseGateExecution(caseGateExecutionId, gateConfigurationRef);
                    var gateConfiguration = (_service.Retrieve(gateConfigurationRef.LogicalName, gateConfigurationRef.Id, new ColumnSet(true))).ToEntity<ipg_gateconfiguration>();

                    //If Gate Configuration is Inactive
                    if (gateConfiguration.GetAttributeValue<OptionSetValue>("statecode").Value.Equals(1))
                    {
                        _context.OutputParameters["Output"] = "Gate Configuration is Inactive. Please contact administrator to resolve this issue.";
                        FailedGatingPostProcess(caseGateExecutionId);
                        return;
                    }

                    if (gateConfiguration.ipg_preactionprocessid != null)
                    {
                        var result = RunPreProcessAction(gateConfiguration.ipg_preactionprocessid, _caseRef ?? _referralRef);
                        if (result.Succeeded == false)
                        {
                            _context.OutputParameters["Output"] = $"Error with pre processing: {result.Output}";
                            FailedGatingPostProcess(caseGateExecutionId);
                            return;
                        }
                    }
                    //Get all Active Gate Configuration Detail Ascending(ipg_executionorder)
                    var gateConfigurationDetails = GetActiveGateConfigurationDetail(gateConfiguration.Id)
                        .Entities
                        .Select(p => p.ToEntity<ipg_gateconfigurationdetail>());
                    if (gateConfigurationDetails.Count() == 0)
                    {
                        _context.OutputParameters["Output"] = "There are no Active Gate Configuration Detail records. Please contact administrator to resolve this issue.";
                        FailedGatingPostProcess(caseGateExecutionId);
                        return;
                    }

                    EntityCollection tasksToCreate = new EntityCollection();
                    int highestSeverityLevel = (int)ipg_SeverityLevel.Info;
                    var processResults = new List<ProcessResult>();
                    var taskManager = new TaskManager(_caseRef, _referralRef, _service);
                    var blockingTasks = GetBlockingTasks(gateConfiguration.Id, target.Id);
                    if (blockingTasks.Any())
                    {
                        var subjects = string.Join(", ", blockingTasks.Select(p => p.Subject));
                        _context.OutputParameters["Output"] = $"There are blocking tasks for current gate: {subjects}";
                        FailedGatingPostProcess(caseGateExecutionId);
                        return;
                    }
                    var gateManagerPostAction = new GateManagerPostAction(target, _caseRef, _service, _tracingService, _context);
                    ProcessDocuments(gateConfiguration, targetRef, taskManager, gateManagerPostAction, caseGateExecutionId);
                    FinishCasePreGateExecution(caseGateExecutionId);

                    var treeManager = new WFGTreeManager(_service, _tracingService);
                    var wfTaskGroups = treeManager.GetGatingTreeV3(gateConfigurationDetails);
                    var portalHeader = new StringBuilder();
                    foreach (var iWFGroup in wfTaskGroups)
                    {
                        var isWfGroupFailed = false;
                        foreach (var gateConfigurationDetail in iWFGroup.GateDetails)
                        {
                            DateTime startExecutionTime = DateTime.Now;
                            EntityReference processRef = RetrieveProcessRef(gateConfigurationDetail);
                            var gatingResponse = new GatingResponse(false);

                            int severityLevel = gateConfigurationDetail.GetAttributeValue<OptionSetValue>("ipg_severitylevel").Value;
                            var cwt = gateManager.GetCaseWorkflowTask(gateConfigurationDetail, caseNumber);
                            var workflowTask = gateConfigurationDetail.ipg_WorkflowTaskId;

                            var skipWorkflowTaskExecution = true;
                            if (HasExceptionApprovedUserTask(targetRef, gateConfigurationDetail.Id))
                            {
                                processResults.Add(new ProcessResult()
                                {
                                    successed = true,
                                    gateconfigdetail = gateConfigurationDetail,
                                    gatingOutcome = "The related user task was approved",
                                    caseNote = "",
                                    portalNote = "",
                                    severityLevel = severityLevel,
                                    resultMessage = "The related user task was approved"
                                });
                                gatingResponse.Succeeded = true;
                                //WF task is considered as failed if at least one is failed
                                isWfGroupFailed = true;
                            }
                            else
                            {
                                if (processRef != null)
                                {
                                    try
                                    {
                                        var parameters = RetrieveParameters(processRef);
                                        var caseInputFields = GetCaseInputFields(parameters, caseNumber);
                                        if ((!SkipWorkflowTask(cwt, caseInputFields) || AlwaysExecute(cwt)) && !HasExceptionApprovedUserTaskV3(targetRef, cwt))
                                        {
                                            Entity process = _service.Retrieve(processRef.LogicalName, processRef.Id, new ColumnSet("uniquename"));
                                            //var response = ExecuteAction($"ipg_{process.GetAttributeValue<string>("uniquename")}", targetRef, FillParameters(parameters, caseInputFields, gateManager));
                                            var response = ExecuteAction($"ipg_{process.GetAttributeValue<string>("uniquename")}", targetRef);
                                            gatingResponse = new GatingResponse(response.Results, cwt == null ? null : cwt.ipg_WorkflowTaskId, _crmContext);
                                            gatingResponse.CaseNote = FillTemplate(gatingResponse.CaseNote, caseNumber, target);
                                            gatingResponse.PortalNote = FillTemplate(gatingResponse.PortalNote, caseNumber, target);

                                            string message;
                                            if (string.IsNullOrEmpty(gatingResponse.CustomMessage) == false)
                                            {
                                                message = gatingResponse.CustomMessage;
                                            }
                                            else if (string.IsNullOrEmpty(gatingResponse.CrmReason) == false)
                                            {
                                                message = gatingResponse.CrmReason;
                                            }
                                            else
                                            {
                                                message = (bool)response.Results["Succeeded"] ?
                                                            gateConfigurationDetail.GetAttributeValue<string>(nameof(ipg_gateconfigurationdetail.ipg_passmessage))
                                                            : gateConfigurationDetail.GetAttributeValue<string>(nameof(ipg_gateconfigurationdetail.ipg_failmessage));
                                            }

                                            highestSeverityLevel = (highestSeverityLevel < severityLevel && !gatingResponse.Succeeded) ? severityLevel : highestSeverityLevel;

                                            if (!string.IsNullOrEmpty(gatingResponse.PortalNote) &&
                                                (highestSeverityLevel == (int)ipg_SeverityLevel.Critical || highestSeverityLevel == (int)ipg_SeverityLevel.Error))
                                            {
                                                portalHeader.Append(gatingResponse.PortalNote + ",");
                                            }

                                            processResults.Add(new ProcessResult()
                                            {
                                                successed = (bool)response.Results["Succeeded"],
                                                gateconfigdetail = gateConfigurationDetail,
                                                gatingOutcome = gatingResponse.GatingOutcome,
                                                caseNote = gatingResponse.CaseNote,
                                                portalNote = gatingResponse.PortalNote,
                                                severityLevel = severityLevel,
                                                resultMessage = message,
                                                caseWorkflowTask = cwt,
                                                workflowTask = workflowTask,
                                                codeOutput = gatingResponse.CodeOutput
                                            });
                                            gatingResponse.Succeeded = (bool)response.Results["Succeeded"];
                                            //WF task is considered as failed if at least one is failed
                                            isWfGroupFailed = isWfGroupFailed || !gatingResponse.Succeeded;
                                            UpdateCaseWorkflowTask(cwt, processRef, caseNumber, gatingResponse.Succeeded, targetRef);
                                            //UpdateCaseInputFields(caseInputFields);
                                            skipWorkflowTaskExecution = false;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        var traceMesasge = $"Error with running gates. WF step: {gateConfigurationDetail.GetAttributeValue<string>(nameof(ipg_gateconfigurationdetail.ipg_name))}. Error: {ex.Message}";
                                        _tracingService.Trace(traceMesasge);
                                        _context.OutputParameters["Output"] = traceMesasge;
                                        throw;
                                    }
                                }

                                else
                                {
                                    gatingResponse.CaseNote = "there is no assigned process for WT";
                                    gatingResponse.PortalNote = "there is an issue in the system. Contact system administrator";
                                    processResults.Add(new ProcessResult()
                                    {
                                        successed = false,
                                        gateconfigdetail = gateConfigurationDetail,
                                        gatingOutcome = gatingResponse.GatingOutcome,
                                        caseNote = gatingResponse.CaseNote,
                                        portalNote = gatingResponse.PortalNote,
                                        severityLevel = severityLevel,
                                        resultMessage = gateConfigurationDetail.GetAttributeValue<string>(nameof(ipg_gateconfigurationdetail.ipg_failmessage)),
                                        codeOutput = gatingResponse.CodeOutput
                                    });
                                    highestSeverityLevel = (highestSeverityLevel < severityLevel) ? severityLevel : highestSeverityLevel;
                                    skipWorkflowTaskExecution = false;
                                }
                            }
                            Guid.TryParse(gatingResponse?.GatingOutcome, out var gatingOutcome);
                            if (!skipWorkflowTaskExecution)
                            {
                                taskManager.CreateGatingTasksV3(tasksToCreate, gatingResponse.Succeeded, workflowTask, gatingResponse, _tracingService);
                                CreateCaseGateExecutionDetail(cwt, workflowTask, caseGateExecutionId, gateConfigurationDetail, startExecutionTime, gatingResponse.Succeeded, severityLevel, gatingResponse.CaseNote, gatingResponse.PortalNote, gatingResponse.TaskDescripton, gatingResponse.CodeOutput, portalHeader);
                            }

                            //CreatePortalComment(service, crmContext, target, gateConfigurationDetail, succeeded, new EntityReference(SystemUser.EntityLogicalName, context.InitiatingUserId));
                            if (!gatingResponse.Succeeded && iWFGroup.Group?.ipg_validationtype == ipg_wftaskgroup_ipg_validationtype.Untilfirstfailure)
                            {
                                break;
                            }

                        }
                        if (isWfGroupFailed && iWFGroup.Group != null && iWFGroup.Group.ipg_post_process_if_failed_id != null)
                        {

                            Entity postProcess = _service.Retrieve(iWFGroup.Group.ipg_post_process_if_failed_id.LogicalName, iWFGroup.Group.ipg_post_process_if_failed_id.Id, new ColumnSet("uniquename"));
                            var postProcessResponse = ExecuteAction($"ipg_{postProcess.GetAttributeValue<string>(Intake.Workflow.Fields.UniqueName)}", targetRef, null);
                        }
                    }

                    UpdateCaseGateExecution(caseGateExecutionId, highestSeverityLevel);
                    UpdateCaseInputFields(caseNumber);
                    CloseObsoleteTasks(processResults, targetRef, taskManager);

                    var areGatesPassed = !processResults
                                .Any(p => (p.severityLevel == (int)ipg_SeverityLevel.Critical ||
                                p.severityLevel == (int)ipg_SeverityLevel.Error) && !p.successed);

                    UpdateTarget(target, portalHeader.ToString());

                    if (!isManual || areGatesPassed)
                    {
                        gateManagerPostAction.StartCasePostGateExecution(caseGateExecutionId);
                        gateManagerPostAction.RunPostAction(processResults, gateConfiguration, tasksToCreate);
                        gateManagerPostAction.FinishCasePostGateExecution(caseGateExecutionId);

                        //set case, on gate 2 it is empty and after creation we can get it for post action

                        _caseRef = _caseRef ?? gateManagerPostAction.CaseRef;
                        _context.OutputParameters["AllowReject"] = false;
                    }
                    else
                    {
                        var severityLevel = processResults
                                .Any(p => (p.severityLevel == (int)ipg_SeverityLevel.Critical && p.successed == false))
                                ? ipg_SeverityLevel.Critical
                                : processResults.Any(p => (p.severityLevel == (int)ipg_SeverityLevel.Error && p.successed == false))
                                    ? ipg_SeverityLevel.Error
                                    : ipg_SeverityLevel.Warning;

                        IEnumerable<ProcessResult> outputProcessResults;
                        if (severityLevel == ipg_SeverityLevel.Critical)
                        {
                            outputProcessResults = processResults.Where(pr =>
                                new[] { ipg_SeverityLevel.Critical, ipg_SeverityLevel.Error }.Contains((ipg_SeverityLevel)pr.severityLevel));
                        }
                        else
                        {
                            outputProcessResults = processResults;
                        }

                        string rejectionResult = gateManagerPostAction.GetProcessingResult(outputProcessResults);

                        _context.OutputParameters["SeverityLevel"] = (int)severityLevel;
                        _context.OutputParameters["AllowReject"] = true;
                        _context.OutputParameters["Output"] = rejectionResult;
                    }
                    _context.OutputParameters["SeverityLevel"] = (int)highestSeverityLevel;
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException($"An error occurred in the ipg_IPGGatingStartGateProcessing plug-in: {ex.Message}");
                }
                catch (Exception ex)
                {
                    _tracingService.Trace("ipg_IPGGatingStartGateProcessing: {0}", ex.ToString());
                    throw;
                }

            }
        }

        private bool HasExceptionApprovedUserTask(EntityReference targetRef, Guid gateConfigId)
        {
            if (targetRef == null)
                return false;
            var query = new QueryExpression(Intake.Task.EntityLogicalName);
            query.ColumnSet = new ColumnSet(Intake.Task.Fields.ipg_is_exception_approved);
            query.Criteria.AddCondition(Intake.Task.Fields.RegardingObjectId, ConditionOperator.Equal, targetRef.Id);
            query.Criteria.AddCondition(Intake.Task.Fields.ipg_gateconfigdetailid, ConditionOperator.Equal, gateConfigId);
            query.Criteria.AddCondition(Intake.Task.Fields.ipg_is_exception_approved, ConditionOperator.Equal, true);
            var results = _service.RetrieveMultiple(query);
            return results.Entities.Any();
        }

        private bool HasExceptionApprovedUserTaskV3(EntityReference targetRef, ipg_caseworkflowtask cwt)
        {
            if (targetRef == null || cwt == null)
            {
                return false;
            }
            var query = new QueryExpression(Intake.Task.EntityLogicalName);
            query.ColumnSet = new ColumnSet(Intake.Task.Fields.ipg_is_exception_approved);
            query.Criteria.AddCondition(Intake.Task.Fields.RegardingObjectId, ConditionOperator.Equal, targetRef.Id);
            query.Criteria.AddCondition(Intake.Task.Fields.ipg_WorkflowTaskId, ConditionOperator.Equal, cwt.ipg_WorkflowTaskId.Id);
            query.Criteria.AddCondition(Intake.Task.Fields.ipg_is_exception_approved, ConditionOperator.Equal, true);
            var results = _service.RetrieveMultiple(query);
            return results.Entities.Any();
        }

        private void CreateObtainRequiredDocTask(EntityReference targetRef, ipg_documentbygate documentByGate)
        {
            /// Check if task exists
            var taskForDocument = RetrieveTaskForDocument(targetRef, documentByGate);
            var ifCloseTask = documentByGate.ipg_closetask ?? false;

            if (!(ifCloseTask ^ taskForDocument != null))
            {
                if (ifCloseTask)
                {
                    /// Close task if "Close task option" is true
                    var taskToUpdate = new Task
                    {
                        Id = taskForDocument.Id,
                        StateCode = TaskState.Completed,
                        StatusCode = new OptionSetValue((int)TaskStatus.RanToCompletion),
                    };
                    _service.Update(taskToUpdate);
                }
                else
                {
                    var task = new Task
                    {
                        Subject = "Obtain Required Document",
                        ipg_tasktypecode = new OptionSetValue((int)ipg_TaskType1.Missingrequireddocument),
                        Description = documentByGate.ipg_name,
                        RegardingObjectId = targetRef,
                        ipg_DocumentType = documentByGate.ipg_documenttypeid
                    };
                    _service.Create(task);
                }
            }
        }

        private EntityReference RetrieveProcessRef(Entity gateConfigurationDetail)
        {
            var workflowTaskRef = gateConfigurationDetail.GetAttributeValue<EntityReference>(ipg_gateconfigurationdetail.Fields.ipg_WorkflowTaskId);
            //workflowTaskRef.Id = new Guid("65565f73-dad2-eb11-bacc-000d3a3b9cd7");
            if (workflowTaskRef != null)
            {
                var workflowTask = _service.Retrieve(workflowTaskRef.LogicalName, workflowTaskRef.Id, new ColumnSet(ipg_workflowtask.Fields.ipg_Process)).ToEntity<ipg_workflowtask>();
                if (workflowTask.ipg_Process != null)
                {
                    return workflowTask.ipg_Process;
                }
            }

            //else use default value from GateConfigurationDetail
            return gateConfigurationDetail.GetAttributeValue<EntityReference>(ipg_gateconfigurationdetail.Fields.ipg_processid);
        }

        private Task RetrieveTaskForDocument(EntityReference targetRef, ipg_documentbygate iDoc)
        {
            if (iDoc.ipg_documenttypeid == null)
            {
                return null;
            }
            var query = new QueryExpression
            {
                EntityName = Task.EntityLogicalName,
                ColumnSet = new ColumnSet(Task.Fields.StateCode),
                Criteria = {
                    Conditions = {
                        new ConditionExpression(Task.Fields.ipg_DocumentType, ConditionOperator.Equal, iDoc.ipg_documenttypeid.Id),
                        new ConditionExpression(Task.Fields.RegardingObjectId, ConditionOperator.Equal, targetRef.Id)
                    }
                },
                Orders = {
                    new OrderExpression(Task.Fields.CreatedOn, OrderType.Descending),
                },
                TopCount = 1,
                NoLock = true
            };
            return _service.RetrieveMultiple(query).Entities.FirstOrDefault()?.ToEntity<Task>();
        }

        private IEnumerable<Task> GetBlockingTasks(Guid gateId, Guid targetId)
        {
            var query = new QueryExpression(Task.EntityLogicalName);
            query.Criteria.AddCondition("ipg_blockedgateid", ConditionOperator.Equal, gateId);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)TaskState.Open);
            query.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, targetId);
            query.ColumnSet = new ColumnSet(true);
            var result = _service.RetrieveMultiple(query).Entities.Select(p => p.ToEntity<Task>());
            return result;
        }

        private void UpdateTarget(Entity target, string portalHeader)
        {
            var targetUpd = new Entity(target.LogicalName, target.Id);
            targetUpd["ipg_cr_gatesessionid"] = _sessionId;
            if (target.LogicalName == Incident.EntityLogicalName)
            {
                targetUpd["ipg_portalheadermultiplelines"] = portalHeader.TrimEnd(',');
            }
            _service.Update(targetUpd);
        }

        private PreProcessResult RunPreProcessAction(EntityReference processRef, EntityReference caseRef)
        {
            var process = _service.Retrieve(processRef.LogicalName, processRef.Id, new ColumnSet("uniquename"));

            var response = ExecuteAction($"ipg_{process.GetAttributeValue<string>("uniquename")}", caseRef);
            return new PreProcessResult()
            {
                Succeeded = response.Results["Succeeded"] as bool?,
                Output = response.Results["Output"] as string
            };
        }



        private EntityCollection GetActiveGateConfigurationDetail(Guid gateConfigurationId)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = ipg_gateconfigurationdetail.EntityLogicalName,
                ColumnSet = new ColumnSet(true)
            };
            query.AddOrder("ipg_executionorder", OrderType.Ascending);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("ipg_gateconfigurationid", ConditionOperator.Equal, gateConfigurationId);

            return _service.RetrieveMultiple(query);
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

        private Guid CreateCaseGateExecution(EntityReference targetRef, EntityReference gateConfigurationRef, string caseNumber, string sessionId)
        {
            var now = DateTime.Now;
            var caseGateExecution = new ipg_casegateexecution()
            {
                ipg_name = caseNumber,
                ipg_ReferralId = (targetRef.LogicalName == ipg_referral.EntityLogicalName) ? targetRef : null,
                ipg_CaseId = (targetRef.LogicalName == Incident.EntityLogicalName) ? targetRef : null,
                ipg_CaseNumber = caseNumber,
                ipg_StartDatetime = now,
                ipg_GateConfigurationId = gateConfigurationRef,
                ipg_SessionID = sessionId,
                ipg_StartDatetimePreExecution = now
            };
            return _service.Create(caseGateExecution);
        }

        private void UpdateCaseGateExecution(Guid caseGateExecutionId, int severityLevel)
        {
            var caseGateExecution = new ipg_casegateexecution()
            {
                Id = caseGateExecutionId,
                ipg_EndDatetimeWTExecution = DateTime.Now,
                ipg_GateHighestSeverity = new OptionSetValue(severityLevel)
            };
            _service.Update(caseGateExecution);
        }

        private void CreateCaseGateExecutionDetail(ipg_caseworkflowtask cwt, EntityReference workflowTaskRef, Guid caseGateExecutionId, ipg_gateconfigurationdetail gateConfigurationDetail, DateTime startExecutionTime, bool succeeded, int severityLevel, string caseNote, string portalNote, string taskDescription, int codeOutput, StringBuilder portalHeader = null)
        {
            var limitedCaseNote = caseNote?.Length > 1000 ? caseNote.Substring(0, 999) : caseNote;
            var limitedPortalNote = portalNote?.Length > 1000 ? portalNote.Substring(0, 999) : portalNote;
            //if (!string.IsNullOrEmpty(limitedPortalNote) && 
            //    (severityLevel == (int)ipg_SeverityLevel.Critical || severityLevel == (int)ipg_SeverityLevel.Error))
            //{
            //    portalHeader.Append(limitedPortalNote + ",");
            //}
            var caseGateExecutionDetail = new ipg_casegateexecutiondetail()
            {
                ipg_CaseGateExecutionId = new EntityReference(ipg_casegateexecution.EntityLogicalName, caseGateExecutionId),
                ipg_WorkflowTaskId = workflowTaskRef,
                ipg_OutcomeType = succeeded,
                ipg_SeverityLevel = new OptionSetValue(severityLevel),
                ipg_StartDatetime = startExecutionTime,
                ipg_EndDatetime = DateTime.Now,
                ipg_Order = gateConfigurationDetail.ipg_executionorder,
                ipg_OutputCRMReason = limitedCaseNote,
                ipg_OutputPortalReason = limitedPortalNote,
                ipg_CodeOutput = codeOutput,
                ipg_TaskDescription = taskDescription,
                ipg_ExecutionTime = (int)((DateTime.Now - startExecutionTime).TotalMilliseconds)
            };
            _service.Create(caseGateExecutionDetail);

            if (succeeded && (cwt != null))
            {
                var record = new ipg_caseworkflowtask()
                {
                    Id = cwt.Id,
                    ipg_Passed = true
                };
                _service.Update(record);
            }
        }

        private void CloseObsoleteTasks(List<ProcessResult> processResults, EntityReference targetRef, TaskManager taskManager)
        {
            foreach (var processResult in processResults)
            {
                if (processResult.successed)
                {
                    EntityCollection taskConfigurations = taskManager.GetActiveTaskConfiguration(processResult.gateconfigdetail.Id);
                    foreach (Entity taskConfiguration in taskConfigurations.Entities)
                    {
                        List<Task> openedTasks = taskManager.GetOpenTasks(taskConfiguration, targetRef);
                        foreach (var task in openedTasks)
                        {
                            if (taskConfiguration.Contains(nameof(ipg_taskconfiguration.ipg_createif).ToLower()) && taskConfiguration.GetAttributeValue<OptionSetValue>(nameof(ipg_taskconfiguration.ipg_createif).ToLower()).Value == (int)ipg_CreateIf.Failed)
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
                            }
                        }
                    }
                }
            }
        }

        public class PostProcessResult
        {
            public bool? Succeeded { get; set; }
            public string Output { get; set; }
        }

        public class PreProcessResult
        {
            public bool? Succeeded { get; set; }
            public string Output { get; set; }
        }

        public class GatingDocumentActionResponse
        {
            public bool? Succeeded { get; set; }
        }

        private IEnumerable<ipg_workflowtaskinputfield> RetrieveParameters(EntityReference processRef)
        {
            return (from relationship in _crmContext.CreateQuery<ipg_ipg_workflowtask_ipg_workflowtaskinput>()
                    join workflowtask in _crmContext.CreateQuery<ipg_workflowtask>() on relationship[nameof(ipg_workflowtask.ipg_workflowtaskId).ToLower()] equals workflowtask.ipg_workflowtaskId
                    join workflowtaskinputfield in _crmContext.CreateQuery<ipg_workflowtaskinputfield>() on relationship[nameof(ipg_workflowtaskinputfield.ipg_workflowtaskinputfieldId).ToLower()] equals workflowtaskinputfield.ipg_workflowtaskinputfieldId
                    where workflowtask.ipg_Process.Id == processRef.Id
                    select workflowtaskinputfield);
        }

        private IEnumerable<ipg_caseinputfield> GetCaseInputFields(IEnumerable<ipg_workflowtaskinputfield> parameters, string caseNumber)
        {
            if (parameters != null && parameters.Any())
            {
                var list = parameters.Select(e => e.ipg_workflowtaskinputfieldId).ToList();
                var query = new QueryExpression
                {
                    EntityName = ipg_caseinputfield.EntityLogicalName,
                    ColumnSet = new ColumnSet(true),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(nameof(ipg_caseinputfield.ipg_InputFieldId).ToLower(), ConditionOperator.In, list),
                            new ConditionExpression(nameof(ipg_caseinputfield.ipg_CaseNumber).ToLower(), ConditionOperator.Equal, caseNumber)
                        }
                    }
                };
                return _service.RetrieveMultiple(query).Entities.Select(e => (ipg_caseinputfield)e);
            }
            return Enumerable.Empty<ipg_caseinputfield>();
        }

        private Dictionary<string, object> FillParameters(IEnumerable<ipg_workflowtaskinputfield> parameters, IEnumerable<ipg_caseinputfield> caseInputFields, GateManager gateManager)
        {
            var dict = new Dictionary<string, string>();
            if (caseInputFields != null)
            {
                foreach (var caseInputField in caseInputFields)
                {
                    dict.Add(parameters.First(e => e.ipg_workflowtaskinputfieldId == caseInputField.ipg_InputFieldId.Id).ipg_name, gateManager.ObjectToString(caseInputField.ipg_CaseInputFieldValueId));
                }
                if (dict.Count > 0)
                {
                    var ser = new DataContractJsonSerializer(typeof(Dictionary<string, string>), new DataContractJsonSerializerSettings()
                    {
                        UseSimpleDictionaryFormat = true
                    });
                    var stream = new MemoryStream();
                    ser.WriteObject(stream, dict);
                    stream.Position = 0;
                    StreamReader sr = new StreamReader(stream);
                    return new Dictionary<string, object>() { { "Arguments", sr.ReadToEnd() } };
                }
            }
            return null;
        }

        private bool SkipWorkflowTask(ipg_caseworkflowtask cwt, IEnumerable<ipg_caseinputfield> caseInputFields)
        {
            var result = ((cwt == null) ? false : (cwt.ipg_Passed ?? false));
            if (result == true)
            {
                result = !caseInputFields.Any(e => e.ipg_CaseInputFieldChanged ?? false);
            }
            return result;
        }

        private bool AlwaysExecute(ipg_caseworkflowtask cwt)
        {
            //return (cwt == null ? true : _service.Retrieve(cwt.ipg_WorkflowTaskId.LogicalName, cwt.ipg_WorkflowTaskId.Id, new ColumnSet(nameof(ipg_workflowtask.ipg_AlwaysExecuted).ToLower())).ToEntity<ipg_workflowtask>().ipg_AlwaysExecuted ?? false);
            return (cwt == null ? true : cwt.ipg_AlwaysExecuted ?? false);
        }

        private void UpdateCaseWorkflowTask(ipg_caseworkflowtask cwt, EntityReference processRef, string caseNumber, bool passed, EntityReference targetRef)
        {
            var entity = new ipg_caseworkflowtask()
            {
                ipg_Passed = passed
            };
            if (cwt == null)
            {
                var wft = (from workflowtask in _crmContext.CreateQuery<ipg_workflowtask>()
                           where workflowtask.ipg_Process.Id == processRef.Id
                           select workflowtask).FirstOrDefault();
                if (wft != null)
                {
                    entity.ipg_WorkflowTaskId = wft.ToEntityReference();
                    entity.ipg_CaseNumber = caseNumber;
                    if (targetRef.LogicalName == ipg_referral.EntityLogicalName)
                    {
                        entity.ipg_ReferralId = targetRef;
                    }
                    else
                    {
                        entity.ipg_CaseId = targetRef;
                    }
                    _service.Create(entity);
                }
            }
            else
            {
                entity.Id = cwt.Id;
                _service.Update(entity);
            }
        }

        private void UpdateCaseInputFields(IEnumerable<ipg_caseinputfield> caseinputfields)
        {
            foreach (var caseinputfield in caseinputfields)
            {
                if (caseinputfield.ipg_CaseInputFieldChanged ?? false)
                {
                    var record = new ipg_caseinputfield()
                    {
                        Id = caseinputfield.Id,
                        ipg_CaseInputFieldChanged = false
                    };
                    _service.Update(record);
                }
            }
        }

        private void UpdateCaseInputFields(string caseNumber)
        {
            var list = (from cif in _crmContext.CreateQuery<ipg_caseinputfield>()
                        where cif.ipg_CaseNumber == caseNumber
                        && cif.ipg_CaseInputFieldChanged == true
                        select cif);
            foreach (var cif in list)
            {
                var record = new ipg_caseinputfield()
                {
                    Id = cif.Id,
                    ipg_CaseInputFieldChanged = false
                };
                _service.Update(record);
            }
        }

        private string FillTemplate(string template, string caseNumber, Entity target)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                return string.Empty;
            }

            var result = template;
            Regex rgx = new Regex(@"\[\w+\]");
            foreach (Match m in rgx.Matches(template))
            {
                var fieldName = m.Value.Substring(1, m.Value.Length - 2);
                var field = (from wtif in _crmContext.CreateQuery<ipg_workflowtaskinputfield>()
                             join cif in _crmContext.CreateQuery<ipg_caseinputfield>() on wtif.Id equals cif.ipg_InputFieldId.Id
                             where wtif.ipg_name == fieldName
                             && cif.ipg_CaseNumber == caseNumber
                             select cif).FirstOrDefault();
                if (field != null)
                {
                    result = result.Replace(m.Value, field.ipg_CaseInputFieldValue);
                }
                else
                {
                    var workflowTaskInputField = (from wtif in _crmContext.CreateQuery<ipg_workflowtaskinputfield>()
                                                  join wtifs in _crmContext.CreateQuery<ipg_workflowtaskinputfieldsource>() on wtif.Id equals wtifs.ipg_WorkflowTaskInputFieldId.Id
                                                  where wtif.ipg_name == fieldName
                                                  && wtifs.ipg_Entity == target.LogicalName
                                                  select wtifs).FirstOrDefault();
                    if (workflowTaskInputField != null)
                    {
                        if (target.Contains(workflowTaskInputField.ipg_Field))
                        {
                            result = result.Replace(m.Value, target[workflowTaskInputField.ipg_Field].ToString());
                        }
                    }
                }
            }
            return result;
        }

        private void FailedGatingPostProcess(Guid caseGateExecutionId)
        {
            _service.Delete(ipg_casegateexecution.EntityLogicalName, caseGateExecutionId);
        }

        private void FinishCasePreGateExecution(Guid caseGateExecutionId)
        {
            var now = DateTime.Now;
            var caseGateExecution = new ipg_casegateexecution()
            {
                Id = caseGateExecutionId,
                ipg_EndDatetimePreExecution = now,
                ipg_StartDatetimeWTExecution = now
            };
            _service.Update(caseGateExecution);
        }

        private void UpdateGateConfigurationCaseGateExecution(Guid caseGateExecutionId, EntityReference gateConfigurationRef)
        {
            var now = DateTime.Now;
            var caseGateExecution = new ipg_casegateexecution()
            {
                Id = caseGateExecutionId,
                ipg_GateConfigurationId = gateConfigurationRef
            };
            _service.Update(caseGateExecution);
        }

        private void ProcessDocuments(ipg_gateconfiguration gateConfiguration, EntityReference targetRef, TaskManager taskManager, GateManagerPostAction gateManagerPostAction, Guid caseGateExecutionId)
        {
            if (gateConfiguration.ipg_documentsvalidationdetailid == null)
            {
                return;
            }

            var gatingDocumentsManager = new GatingDocumentManager(_service);
            var gateDocuments = gatingDocumentsManager.GetGateDocuments(gateConfiguration);
            var documentsProcessingGateDetail = _service.Retrieve(ipg_gateconfigurationdetail.EntityLogicalName, gateConfiguration.ipg_documentsvalidationdetailid.Id, new ColumnSet(true)).ToEntity<ipg_gateconfigurationdetail>();
            var processRef = RetrieveProcessRef(documentsProcessingGateDetail);
            var process = _service.Retrieve(processRef.LogicalName, processRef.Id, new ColumnSet("uniquename"));
            var tasksToCreate = new EntityCollection();
            foreach (var documentByGate in gateDocuments)
            {
                DateTime startExecutionTime = DateTime.Now;
                var response = ExecuteAction($"ipg_{process.GetAttributeValue<string>("uniquename")}", targetRef, new Dictionary<string, object>() { { "DocumentType", documentByGate.ipg_documenttypeid } });
                var gatingResponse = new GatingResponse(response.Results, documentsProcessingGateDetail.GetAttributeValue<EntityReference>(ipg_gateconfigurationdetail.Fields.ipg_WorkflowTaskId), _crmContext);

                if (documentByGate.ipg_closetask ?? false)
                {
                    var taskForDocument = RetrieveTaskForDocument(targetRef, documentByGate);
                    if (taskForDocument != null && taskForDocument.StateCode == TaskState.Open)
                    {
                        var taskReason = Managers.TaskManager.GetTaskReasonByName(_service, Managers.TaskManager.TaskReasonsNames.NotNeeded)?.ToEntityReference();
                        _service.Update(new Task()
                        {
                            Id = taskForDocument.Id,
                            ipg_taskreason = taskReason,
                            StateCode = TaskState.Completed,
                            StatusCodeEnum = Task_StatusCode.Cancelled
                        });
                    }
                }
                else
                {
                    taskManager.CreateGatingTasksV3(tasksToCreate, gatingResponse.Succeeded, (ipg_gateconfigurationdetail)documentsProcessingGateDetail, gatingResponse, _tracingService);
                }
                CreateCaseGateExecutionDetail(null, documentsProcessingGateDetail?.ipg_WorkflowTaskId, caseGateExecutionId, documentsProcessingGateDetail, startExecutionTime, gatingResponse.Succeeded, (int)documentByGate.ipg_requirment.Value, gatingResponse.CaseNote, gatingResponse.PortalNote, gatingResponse.TaskDescripton, gatingResponse.CodeOutput);
            }
            gateManagerPostAction.CreateTasks(tasksToCreate, targetRef);
        }
    }
}