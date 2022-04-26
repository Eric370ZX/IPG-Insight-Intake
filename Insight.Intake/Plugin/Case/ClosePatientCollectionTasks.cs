using Insight.Intake.Plugin.Managers;
using static Insight.Intake.Helpers.Constants;

namespace Insight.Intake.Plugin.Case
{
    public class ClosePatientCollectionTasks: PluginBase
    {
        //patient collection was renamed to patient outreach
        public ClosePatientCollectionTasks():base(typeof(ClosePatientCollectionTasks))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Incident.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var target = localPluginContext.Target<Incident>();
            if (target.Contains(Incident.Fields.ipg_RemainingPatientBalance) && (target.ipg_RemainingPatientBalance?.Value ?? 0) == 0)
            {
                var taskManager = new TaskManager(localPluginContext.OrganizationService, localPluginContext.TracingService, null);
                taskManager.CloseCategoryTasks(target.Id, TaskCategoryNames.PatientOutreach);
            }
        }
    }
}