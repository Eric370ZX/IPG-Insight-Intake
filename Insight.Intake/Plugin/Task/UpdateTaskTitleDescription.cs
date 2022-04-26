using Microsoft.Xrm.Sdk;


namespace Insight.Intake.Plugin.TaskEntity
{
    public class UpdateTaskTitleDescription : PluginBase
    {
        public UpdateTaskTitleDescription() : base(typeof(UpdateTaskTitleDescription))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, Task.EntityLogicalName, PreOperationHandler);
        }

        private void PreOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;

            var target = ((Entity)context.InputParameters["Target"]).ToEntity<Task>();
            var caseReference = target.ipg_caseid ?? (target.RegardingObjectId?.LogicalName == Incident.EntityLogicalName ? target.RegardingObjectId : null);

            if (caseReference != null)
            {
                target.Subject += $"({caseReference.Name})";
                target.Description = $"({caseReference.Name}) {target.Description}";
            }
        }
    }
}
