using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.Linq;

namespace Insight.Intake.Plugin.TaskEntity
{
    public class CloseByRegarding : PluginBase
    {
        public CloseByRegarding() : base(typeof(CloseByRegarding))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGTaskCloseByRegarding", null, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;
            var regarding = localPluginContext.GetInput<EntityReference>("Regarding");
            var closeReason = (Task_StatusCode)localPluginContext.GetInput<int>("CloseReason");
            var closeNote = localPluginContext.GetInput<string>("CloseNote");
            var produceTaskNote = localPluginContext.GetInput<bool>("ProduceTaskNote");
            var taskReason = localPluginContext.PluginExecutionContext.InputParameters.Contains("TaskReason")
                ? localPluginContext.GetInput<EntityReference>("TaskReason") : null;
            var initiatingUserId = localPluginContext.PluginExecutionContext.InitiatingUserId;
            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = false;

            new OrganizationServiceContext(service)
                .CreateQuery<Task>()
                .Where(task => task.StateCode == TaskState.Open)
                .Where(task => task.RegardingObjectId.Id == regarding.Id)
                .Select(task => new Intake.Task { Id = task.Id })
                .ToList()
                .ForEach(task =>
                {
                    var taskManager = new TaskManager(service, localPluginContext.TracingService, task.ToEntityReference(), initiatingUserId);
                    taskManager.CloseTask(closeNote, closeReason, produceTaskNote, taskReason);
                });

            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = true;
        }
    }
}