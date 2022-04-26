using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Document
{
    public class UpdateTaskPlugin : PluginBase
    {
        public UpdateTaskPlugin() : base(typeof(UpdateTaskPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_document.EntityLogicalName, UpdateTask);
        }

        private void UpdateTask(LocalPluginContext localContext)
        {
            var doc = localContext.PostImage<ipg_document>();

            if (doc.ipg_originatingtaskid != null)
            {
                var task = localContext.OrganizationService.Retrieve(Task.EntityLogicalName, doc.ipg_originatingtaskid.Id, new ColumnSet(Task.Fields.StateCode))?.ToEntity<Task>();
                if (task != null && task.StateCode != TaskState.Completed)
                {
                    localContext.OrganizationService.Update(
                        new Task()
                        {
                            Id = doc.ipg_originatingtaskid.Id,
                            ipg_reviewstatuscodeEnum = doc.ipg_ReviewStatusEnum == ipg_document_ipg_ReviewStatus.Approved ?
                        Task_ipg_reviewstatuscode.Approved : doc.ipg_ReviewStatusEnum == ipg_document_ipg_ReviewStatus.Rejected ?
                        Task_ipg_reviewstatuscode.Rejected : Task_ipg_reviewstatuscode.PendingReview
                        });
                }
            }

        }
    }
}
