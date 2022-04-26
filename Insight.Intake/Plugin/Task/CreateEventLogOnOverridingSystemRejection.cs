using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.TaskEntity
{
    public class CreateEventLogOnOverridingSystemRejection : PluginBase
    {
        public CreateEventLogOnOverridingSystemRejection() : base(typeof(CreateEventLogOnOverridingSystemRejection))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Task.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localContext)
        {
            CreateET25Log(localContext);
        }
        private void CreateET25Log(LocalPluginContext localContext)
        {
            if (localContext.PreImage<Task>() != null && localContext.PostImage<Task>() != null)
            {
                var taskBefore = localContext.PreImage<Task>();
                var taskAfter = localContext.PostImage<Task>();
                if (taskBefore.ipg_is_exception_approved != taskAfter.ipg_is_exception_approved)
                {
                    if (taskAfter.ipg_caseid != null)
                    {
                        var assosiatedCase = localContext.OrganizationService.Retrieve(Incident.EntityLogicalName, taskAfter.ipg_caseid.Id, new ColumnSet(Incident.Fields.Title));
                        if (assosiatedCase != null)
                        {
                            var eventManager = new ImportantEventManager(localContext.OrganizationService);
                            eventManager.CreateImportantEventLog(assosiatedCase, localContext.PluginExecutionContext.InitiatingUserId, Constants.EventIds.ET25);
                            eventManager.SetCaseOrReferralPortalHeader(assosiatedCase, Constants.EventIds.ET25);
                        }
                    }
                }
            }
        }
    }
}