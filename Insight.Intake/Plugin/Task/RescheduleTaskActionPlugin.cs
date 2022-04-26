using Insight.Intake.Plugin.Managers;
using System;

namespace Insight.Intake.Plugin.TaskEntity
{
    public class RescheduleTaskActionPlugin : PluginBase
    {
        public RescheduleTaskActionPlugin() : base(typeof(CloseTaskActionPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGTaskActionsRescheduleTask", "task", PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var taskER = localPluginContext.TargetRef();
            string additionalTasks = "";
            if (localPluginContext.PluginExecutionContext.InputParameters.Contains("AdditionalTasks")
                && localPluginContext.PluginExecutionContext.InputParameters["AdditionalTasks"] != null)
            {
                additionalTasks = localPluginContext.GetInput<string>("AdditionalTasks");
            }
            var newStartDate = localPluginContext.GetInput<DateTime>("NewStartDate");
            var rescheduleNote = localPluginContext.GetInput<string>("RescheduleNote");
            var produceTaskNote = localPluginContext.GetInput<bool>("ProduceTaskNote");
            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = false;
            var initiatingUserId = localPluginContext.PluginExecutionContext.InitiatingUserId;
            var taskManager = new TaskManager(localPluginContext.OrganizationService, localPluginContext.TracingService, taskER, initiatingUserId);
            taskManager.RescheduleTask(newStartDate, rescheduleNote, produceTaskNote, additionalTasks);
            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = true;
        }
    }
}
