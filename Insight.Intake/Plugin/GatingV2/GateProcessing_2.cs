using Insight.Intake.Plugin.Gating;
using Insight.Intake.Plugin.Gating.Common;
using Insight.Intake.Plugin.GatingV2;
using Insight.Intake.Plugin.GatingV2.CommonWfTask;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.GatingV2
{
    public class GateProcessing_2 : PluginBase
    {
        ITracingService _tracingService;
        OrganizationServiceContext _crmContext;
        IPluginExecutionContext _context;
        IOrganizationService _service;
        string _sessionId;

        EntityReference _caseRef;

        public GateProcessing_2() : base(typeof(GateProcessing_2))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingStartGateProcessing", null, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGRunGateForCase", null, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
         
            //Extract the tracing service for use in debugging sandboxed plug-ins. 
            _tracingService = localPluginContext.TracingService;
            _tracingService.Trace($"Gating. Start plugin: {DateTime.UtcNow}");
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

                    if (gateConfigurationRef == null)
                    {
                        if (!target.Contains("ipg_lifecyclestepid"))
                        {
                            _context.OutputParameters["Output"] = "Lifecycle step field is empty. Please contact administrator to resolve this issue.";
                            return;
                        }

                        if (target.LogicalName == Incident.EntityLogicalName)
                        {
                            _caseRef = target.ToEntityReference();
                        }
                        else if (target.LogicalName == ipg_referral.EntityLogicalName)
                        {
                            _caseRef = target.Contains("ipg_associatedcaseid") ? target.GetAttributeValue<EntityReference>("ipg_associatedcaseid") : null;
                        }

                        var lifecycleStepRef = target.GetAttributeValue<EntityReference>("ipg_lifecyclestepid");
                        _tracingService.Trace($"Gating. lifecycleStepName: {lifecycleStepRef.Name}, {DateTime.UtcNow}");
                        //var lifecycleStep = _service.Retrieve(lifecycleStepRef.LogicalName, lifecycleStepRef.Id, new ColumnSet("ipg_gateconfigurationid"));
                        var lifestepId = new Guid("8255a135-3cd5-e911-a9b8-000d3a367d35");
                        var lifecycleStep = _service.Retrieve(lifecycleStepRef.LogicalName, lifestepId, new ColumnSet("ipg_gateconfigurationid"));

                        gateConfigurationRef = lifecycleStep.GetAttributeValue<EntityReference>("ipg_gateconfigurationid");
                    }

                    if (gateConfigurationRef == null)
                    {
                        _context.OutputParameters["Output"] = "Gate Configuration field is empty. Please contact administrator to resolve this issue.";
                        return;
                    }
                    var gateConfiguration = (_service.Retrieve(gateConfigurationRef.LogicalName, gateConfigurationRef.Id, new ColumnSet(true))).ToEntity<ipg_gateconfiguration>();

                    //If Gate Configuration is Inactive
                    if (gateConfiguration.GetAttributeValue<OptionSetValue>("statecode").Value.Equals(1))
                    {
                        _context.OutputParameters["Output"] = "Gate Configuration is Inactive. Please contact administrator to resolve this issue.";
                        return;
                    }

                    _tracingService.Trace($"Gating. GetActiveGateConfigurationDetail Start: {DateTime.UtcNow}");
                    //Get all Active Gate Configuration Detail Ascending(ipg_executionorder)
                    EntityCollection gateConfigurationDetails = GetActiveGateConfigurationDetail(gateConfiguration.Id);
                    if (gateConfigurationDetails.Entities.Count == 0)
                    {
                        _context.OutputParameters["Output"] = "There are no Active Gate Configuration Detail records. Please contact administrator to resolve this issue.";
                        return;
                    }
                    _tracingService.Trace($"Gating. GetActiveGateConfigurationDetail stop: {DateTime.UtcNow}");

                    EntityCollection tasksToCreate = new EntityCollection();
                    var processResults = new List<ProcessResult>();
                    var taskManager = new TaskManager_2(_caseRef, _service);

                    _tracingService.Trace($"Gating. GetBlockingTasks Start: {DateTime.UtcNow}");
                    var blockingTasks = GetBlockingTasks(gateConfiguration.Id, target.Id);
                    _tracingService.Trace($"Gating. GetBlockingTasks Stop: {DateTime.UtcNow}");
                    if (blockingTasks.Any())
                    {
                        var subjects = string.Join(", ", blockingTasks.Select(p => p.Subject));
                        _context.OutputParameters["Output"] = $"There are blocking tasks for current gate: {subjects}";
                        return;
                    }

                    if (gateConfiguration.ipg_documentsvalidationdetailid != null)
                    {
                        var gatingDocumentsManager = new GatingDocumentManager(_service);
                        var documentsManager = new DocumentsManager(_service);
                        var gateDocuments = gatingDocumentsManager.GetGateDocuments(gateConfiguration);
                        var caseDocuments = documentsManager.GetTargetDocuments(targetRef);
                        var missingDocuments = gateDocuments
                            .Where(gd => !caseDocuments.Any(cd => cd.ipg_DocumentTypeId?.Id == gd.ipg_documenttypeid?.Id));
                        var documentsProcessingGateDetail = _service.Retrieve(ipg_gateconfigurationdetail.EntityLogicalName, gateConfiguration.ipg_documentsvalidationdetailid.Id, new ColumnSet(true));
                        foreach (var iDoc in missingDocuments)
                        {
                            var note = $"Missing required document: {iDoc.ipg_documenttypeid?.Name}";
                            processResults.Add(new ProcessResult()
                            {
                                caseNote = note,
                                portalNote = note,
                                severityLevel = (int)iDoc.ipg_requirment.Value,
                                successed = false,
                                gateconfigdetail = documentsProcessingGateDetail
                            });
                            documentsProcessingGateDetail[ipg_gateconfigurationdetail.Fields.ipg_failmessage] = note;

                            /// Create Task #Obtain Required Document
                            CreateObtainRequiredDocTask(targetRef, iDoc);
                        }
                    }
                    _tracingService.Trace($"Gating. RetrieveExecutionHistory start: {DateTime.UtcNow}");
                    var execHistory = RetrieveExecutionHistory(targetRef, _service);
                    _tracingService.Trace($"Gating. RetrieveExecutionHistory stop: {DateTime.UtcNow}");
                    _tracingService.Trace($"Gating. Execute configuration details steps start: {DateTime.UtcNow}");
                    var gatingDBContext = new GatingContext(targetRef.LogicalName == Incident.EntityLogicalName ? (Guid?)targetRef.Id : null,
                        targetRef.LogicalName == ipg_referral.EntityLogicalName ? (Guid?)targetRef.Id : null, _service);
                    foreach (Entity gateConfigurationDetail in gateConfigurationDetails.Entities)
                    {
                        var gateDetailName = gateConfigurationDetail.GetAttributeValue<string>(ipg_gateconfigurationdetail.Fields.ipg_name);
                        DateTime startExecutionTime = DateTime.UtcNow;
                        _tracingService.Trace($"Gating. Execute configuration detail {gateDetailName} start: {DateTime.UtcNow}");
                        EntityReference processRef = gateConfigurationDetail.GetAttributeValue<EntityReference>("ipg_processid");
                        var gatingResponse = new WFTaskResult(false);

                        int severityLevel = gateConfigurationDetail.GetAttributeValue<OptionSetValue>("ipg_severitylevel").Value;

                        var cachedExecutionResult = execHistory.FirstOrDefault(x => x.ipg_wftask?.Id == processRef?.Id);
                        var isResultFromChache = cachedExecutionResult != null;
                        if (isResultFromChache)
                        {
                            gatingResponse.CaseNote = cachedExecutionResult.ipg_casenote;
                            gatingResponse.PortalNote = cachedExecutionResult.ipg_portalnote;
                            processResults.Add(new ProcessResult()
                            {
                                successed = cachedExecutionResult.ipg_succeeded ?? false,
                                gateconfigdetail = gateConfigurationDetail,
                                gatingOutcome = cachedExecutionResult.ipg_gatingoutcome,
                                caseNote = cachedExecutionResult.ipg_casenote,
                                portalNote = cachedExecutionResult.ipg_portalnote,
                                severityLevel = severityLevel,
                                resultMessage = cachedExecutionResult.ipg_succeeded == true ?
                                    gateConfigurationDetail.GetAttributeValue<string>(nameof(ipg_gateconfigurationdetail.ipg_passmessage))
                                    : gateConfigurationDetail.GetAttributeValue<string>(nameof(ipg_gateconfigurationdetail.ipg_failmessage))
                            });
                            gatingResponse.Succeeded = cachedExecutionResult.ipg_succeeded ?? false;
                        }
                        else
                        {
                            if (processRef != null)
                            {
                                try
                                {
                                    Entity process = _service.Retrieve(processRef.LogicalName, processRef.Id, new ColumnSet("uniquename"));
                                    var wfTask = gateConfigurationDetail.ToEntity<ipg_gateconfigurationdetail>().ipg_wftaskId;
                                    if (wfTask == null)
                                    {
                                        continue;
                                    }
                                    //var response = ExecuteAction($"ipg_{process.GetAttributeValue<string>("uniquename")}", targetRef);
                                    var wfTaskInstance = WfTaskFactory.Build(wfTask.Id);
                                    _tracingService.Trace($"Gating. Execute configuration detail {gateDetailName} run start: {DateTime.UtcNow}");
                                    gatingResponse = wfTaskInstance.Run(new WFTaskContext() { CrmService = _service, TraceService = _tracingService, dbContext = gatingDBContext });
                                    _tracingService.Trace($"Gating. Execute configuration detail {gateDetailName} run stop: {DateTime.UtcNow}");
                                    var message = string.IsNullOrEmpty(gatingResponse.CustomMessage)
                                        ? (gatingResponse.Succeeded ?
                                                        gateConfigurationDetail.GetAttributeValue<string>(nameof(ipg_gateconfigurationdetail.ipg_passmessage))
                                                        : gateConfigurationDetail.GetAttributeValue<string>(nameof(ipg_gateconfigurationdetail.ipg_failmessage)))
                                        : gatingResponse.CustomMessage;

                                    processResults.Add(new ProcessResult()
                                    {
                                        successed = gatingResponse.Succeeded,
                                        gateconfigdetail = gateConfigurationDetail,
                                        gatingOutcome = gatingResponse.GatingOutcome,
                                        caseNote = gatingResponse.CaseNote,
                                        portalNote = gatingResponse.PortalNote,
                                        severityLevel = severityLevel,
                                        resultMessage = message
                                    });
                                    gatingResponse.Succeeded = gatingResponse.Succeeded;
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
                                    resultMessage = gateConfigurationDetail.GetAttributeValue<string>(nameof(ipg_gateconfigurationdetail.ipg_failmessage))
                                });
                            }
                        }
                        _tracingService.Trace($"Gating. Execute configuration detail {gateDetailName} CreateGatingTasks: {DateTime.UtcNow}");
                        taskManager.CreateGatingTasks(gateConfiguration, tasksToCreate, gateConfigurationDetail, gatingResponse.Succeeded, gatingResponse);

                        //CreatePortalComment(service, crmContext, target, gateConfigurationDetail, succeeded, new EntityReference(SystemUser.EntityLogicalName, context.InitiatingUserId));
                        _tracingService.Trace($"Gating. Execute configuration detail {gateDetailName} stop: {DateTime.UtcNow}");
                    }
                    _tracingService.Trace($"Gating. Execute configuration details steps stop: {DateTime.UtcNow}");
                    CloseObsoleteTasks(processResults, targetRef, taskManager);

                    var areGatesPassed = !processResults
                                .Any(p => (p.severityLevel == (int)ipg_SeverityLevel.Warning ||
                                p.severityLevel == (int)ipg_SeverityLevel.Critical ||
                                p.severityLevel == (int)ipg_SeverityLevel.Error) && !p.successed);

                    SetSessionIdOnCase(target);
                    var isManual = _context.InputParameters.Contains("IsManual") ? (bool)_context.InputParameters["IsManual"] : false;

                    var gateManagerPostAction = new GateManagerPostAction(target,  _caseRef, _service, _tracingService, _context);
                    if (!gateConfiguration.GetAttributeValue<bool>("ipg_allowrejection") || !isManual || areGatesPassed)
                    {
                        _tracingService.Trace($"Gating. RunPostAction start: {DateTime.UtcNow}");
                        gateManagerPostAction.RunPostAction(processResults, gateConfiguration, tasksToCreate);
                        _context.OutputParameters["AllowReject"] = false;
                        _tracingService.Trace($"Gating. RunPostAction stop: {DateTime.UtcNow}");
                    }
                    else
                    {
                        _tracingService.Trace($"Gating. GetProcessingResult start: {DateTime.UtcNow}");
                        var rejectionResult = gateManagerPostAction.GetProcessingResult(processResults);
                        var severityLevel = processResults
                                .Any(p => (p.severityLevel == (int)ipg_SeverityLevel.Critical && p.successed == false))
                                ? ipg_SeverityLevel.Critical
                                : processResults.Any(p => (p.severityLevel == (int)ipg_SeverityLevel.Error && p.successed == false))
                                    ? ipg_SeverityLevel.Error
                                    : ipg_SeverityLevel.Warning;
                        _context.OutputParameters["SeverityLevel"] = (int)severityLevel;
                        _context.OutputParameters["AllowReject"] = true;
                        _context.OutputParameters["Output"] = rejectionResult;
                        _tracingService.Trace($"Gating. GetProcessingResult stop: {DateTime.UtcNow}");
                    }
                   
                    var isAbleToProceed = !processResults
                                .Any(p => (p.severityLevel == (int)ipg_SeverityLevel.Critical ||
                                p.severityLevel == (int)ipg_SeverityLevel.Error) && !p.successed);
                    if (isAbleToProceed && gateConfiguration.GetAttributeValue<EntityReference>("ipg_successpostprocessid") != null)
                    {
                        var minimumSeverityLevel = processResults
                                .Any(p => p.severityLevel == (int)ipg_SeverityLevel.Warning && p.successed == false) ? ipg_SeverityLevel.Warning : ipg_SeverityLevel.Info;

                        _tracingService.Trace($"Gating. RunPostProcessAction start: {DateTime.UtcNow}");
                        var result = RunPostProcessAction(gateConfiguration.GetAttributeValue<EntityReference>("ipg_successpostprocessid"), _caseRef, minimumSeverityLevel);
                        _tracingService.Trace($"Gating. RunPostProcessAction stop: {DateTime.UtcNow}");
                        if (result.Succeeded == false)
                        {
                            _context.OutputParameters["Output"] = $"Error with post processing: {result.Output}";
                            return;
                        }
                    }
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
                _tracingService.Trace($"Gating. Finish: {DateTime.UtcNow}");
            }
        }

        private void CreateObtainRequiredDocTask(EntityReference targetRef, ipg_documentbygate iDoc)
        {
            /// Check if task exists
            var taskForDocument = RetrieveTaskForDocument(targetRef, iDoc);
            var ifCloseTask = iDoc.ipg_closetask ?? false;

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
                    /// Create task if Task for document doesn`t exist
                    var task = new Task
                    {
                        Subject = "Obtain Required Document",
                        ipg_tasktypecode = new OptionSetValue((int)ipg_TaskType1.Missingrequireddocument),
                        Description = iDoc.ipg_name,
                        RegardingObjectId = targetRef,
                        ipg_DocumentType = iDoc.ipg_documenttypeid
                    };
                    _service.Create(task);
                }
            }
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
                Criteria = {
                    Conditions = {
                        new ConditionExpression(Task.Fields.ipg_DocumentType, ConditionOperator.Equal, iDoc.ipg_documenttypeid.Id),
                        new ConditionExpression(Task.Fields.RegardingObjectId, ConditionOperator.Equal, targetRef.Id),
                    }
                },
                Orders = {
                    new OrderExpression(Task.Fields.CreatedOn, OrderType.Descending),
                },
                TopCount = 1,
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

        private void SetSessionIdOnCase(Entity target)
        {
            var targetUpd = new Entity(target.LogicalName, target.Id);
            targetUpd["ipg_cr_gatesessionid"] = _sessionId;
            _service.Update(targetUpd);
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

        private void CloseObsoleteTasks(List<ProcessResult> processResults, EntityReference targetRef, TaskManager_2 taskManager)
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
                                var taskToUpdate = new Task()
                                {
                                    Id = task.Id,
                                    StateCode = TaskState.Completed,
                                    StatusCode = new OptionSetValue((int)Task_StatusCode.Cancelled)
                                };
                                _service.Update(taskToUpdate);
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<ipg_wftasks> RetrieveExecutionHistory(EntityReference _targetRef, IOrganizationService _service)
        {
            if (_targetRef != null)
            {
                var targetFieldName = (_targetRef.LogicalName == Incident.EntityLogicalName)
                    ? nameof(ipg_wftasks.ipg_case)
                    : nameof(ipg_wftasks.ipg_referral);
                var query = new QueryExpression
                {
                    EntityName = ipg_wftasks.EntityLogicalName,
                    ColumnSet = new ColumnSet(nameof(ipg_wftasks.ipg_succeeded), nameof(ipg_wftasks.ipg_wftask), nameof(ipg_wftasks.ipg_casenote), nameof(ipg_wftasks.ipg_portalnote), nameof(ipg_wftasks.ipg_gatingoutcome)),
                    Criteria = {
                    Conditions = {
                        new ConditionExpression(targetFieldName, ConditionOperator.Equal, _targetRef.Id),
                        new ConditionExpression(nameof(ipg_wftasks.ipg_isvalid), ConditionOperator.Equal, true),
                        }
                    }
                };
                return _service.RetrieveMultiple(query).Entities.Select(p => p.ToEntity<ipg_wftasks>());
            }
            else
            {
                throw new InvalidPluginExecutionException("Target reference is null");
            }
        }


        public class PostProcessResult
        {
            public bool? Succeeded { get; set; }
            public string Output { get; set; }
        }
    }
}
