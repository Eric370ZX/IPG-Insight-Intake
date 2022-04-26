using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;

namespace Insight.Intake.Plugin.TaskEntity
{
    public class ManageCaseOnHoldByTask : PluginBase
    {
        public ManageCaseOnHoldByTask() : base(typeof(ManageCaseOnHoldByTask))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Task.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Task.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var task = localPluginContext.PluginExecutionContext.MessageName == MessageNames.Create ? localPluginContext.Target<Task>() : localPluginContext.PostImage<Task>();
             
            var taskManager = new TaskManager(localPluginContext.OrganizationService, localPluginContext.TracingService, null);

            if (task.RegardingObjectId?.LogicalName == Incident.EntityLogicalName && task.ipg_tasktypeid != null && taskManager.CheckTaskByTaskType(task, Constants.TaskTypeNames.FacilityRecoveryResearchPending))
            {
                taskManager.UpdateCaseHoldStatusByTaskk(task.RegardingObjectId, task);      
            }
        }
    }
}