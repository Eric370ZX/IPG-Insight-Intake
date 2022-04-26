using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.TaskEntity
{
    public class CloseTaskActionPlugin : PluginBase
    {
        private IOrganizationService service;
        public CloseTaskActionPlugin() : base(typeof(CloseTaskActionPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGTaskActionsCloseTask", "task", PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            service = localPluginContext.OrganizationService;
            var taskER = localPluginContext.TargetRef();
            var closeReason = (Task_StatusCode)localPluginContext.GetInput<int>("CloseReason");
            var closeNote = localPluginContext.GetInput<string>("CloseNote");
            var produceTaskNote = localPluginContext.GetInput<bool>("ProduceTaskNote");
            var taskReason = (localPluginContext.PluginExecutionContext.InputParameters.Contains("TaskReason")
                && localPluginContext.PluginExecutionContext.InputParameters["TaskReason"] != null) ? localPluginContext.GetInput<EntityReference>("TaskReason") : null;

            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = false;
            var initiatingUserId = localPluginContext.PluginExecutionContext.InitiatingUserId;
            var taskManager = new TaskManager(localPluginContext.OrganizationService, localPluginContext.TracingService, taskER, initiatingUserId);
            taskManager.CloseTask(closeNote, closeReason, produceTaskNote, taskReason);
            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = true;

            var assosiatedCase = RetrieveCase(taskER.Id);
            if (assosiatedCase != null)
            {
                var importantEventManager = new ImportantEventManager(localPluginContext.OrganizationService);
                importantEventManager.CreateImportantEventLog(assosiatedCase, initiatingUserId, Constants.EventIds.ET4);
                importantEventManager.SetCaseOrReferralPortalHeader(assosiatedCase, Constants.EventIds.ET4);
            }
        }
        private Entity RetrieveCase(Guid taskId)
        {
            var fetchXml = $@"<fetch distinct='true' mapping='logical' version='1.0'>
                                 <entity name='incident'> 
                                     <attribute name='incidentid'/>
                                     <attribute name='title'/>
                                     <link-entity name='task' link-type='inner' to='incidentid' from='ipg_caseid'>
                                           <filter type='and'>                        
                                                <condition attribute='activityid' operator='eq' value='{taskId}'/>                          
                                           </filter>
                                     </link-entity>                           
                                 </entity >               
                             </fetch>";
            var fetchExpression = new FetchExpression(fetchXml);
            return service.RetrieveMultiple(fetchExpression).Entities.FirstOrDefault();
        }
    }
}