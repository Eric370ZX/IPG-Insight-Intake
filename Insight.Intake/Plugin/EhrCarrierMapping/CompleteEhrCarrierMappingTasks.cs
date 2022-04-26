using System.Linq;

namespace Insight.Intake.Plugin.EhrCarrierMapping
{
    public class CompleteTasks : PluginBase
    {
        public static readonly int EHR_NEW_CARRIER_MAPPING_TASK_ID = 1011;

        public CompleteTasks() : base(typeof(CompleteTasks))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_ehrcarriermap.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var organizationService = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;

            tracingService.Trace("Getting Target Mapping");
            var targetMapping = localPluginContext.Target<ipg_ehrcarriermap>();

            if (targetMapping.ipg_StatusEnum == ipg_EHRCarrierMappingStatuses.Complete)
            {
                localPluginContext.TracingService.Trace("Creating context");
                using (CrmServiceContext context = new CrmServiceContext(localPluginContext.OrganizationService))
                {
                    localPluginContext.TracingService.Trace("Retrieving open tasks");
                    var openTasks = (from task in context.TaskSet
                                     join taskType in context.ipg_tasktypeSet on task.ipg_tasktypeid.Id equals taskType.Id
                                     where taskType.ipg_typeid == EHR_NEW_CARRIER_MAPPING_TASK_ID
                                         && task.RegardingObjectId != null && task.RegardingObjectId.Id == targetMapping.Id
                                         && task.StateCode == TaskState.Open
                                     select task).ToList();

                    localPluginContext.TracingService.Trace($"Completing {openTasks.Count} tasks");
                    foreach (var openTask in openTasks)
                    {
                        openTask.StateCode = TaskState.Completed;
                        openTask.StatusCodeEnum = Task_StatusCode.Resolved;
                        context.UpdateObject(openTask);
                    }

                    if (openTasks.Any())
                    {
                        localPluginContext.TracingService.Trace("Saving changes");
                        context.SaveChanges();
                    }
                }

            }
        }

    }
}
