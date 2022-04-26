using Insight.Intake.Plugin.Managers;
using System.Linq;

namespace Insight.Intake.Plugin.Account
{
    public class FacilityDIFDocumentValidation : PluginBase
    {
        public FacilityDIFDocumentValidation() : base(typeof(FacilityDIFDocumentValidation))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Insight.Intake.Account.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Insight.Intake.Account.EntityLogicalName, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var target = localPluginContext.Target<Insight.Intake.Account>();
            var postImage = localPluginContext.PostImage<Insight.Intake.Account>();
            if (postImage.CustomerTypeCodeEnum != Account_CustomerTypeCode.Facility || 
                (postImage.ipg_EHREffectiveDate == null && postImage.ipg_EHRExpirationDate == null))
            {
                return;
            }
            var targetRef = target.ToEntityReference();
            var documentManager = new DocumentManager(localPluginContext.OrganizationService, localPluginContext.TracingService);
            var relatedDocs = documentManager.GetDocsByAccountOfType(targetRef, "DIF");
            if (!relatedDocs.Any())
            {
                var taskManager = new TaskManager(localPluginContext.OrganizationService, localPluginContext.TracingService, null, localPluginContext.PluginExecutionContext.UserId);
                var relatedTasks = taskManager.GetTasks(targetRef);
                if (!relatedTasks.Any(p => p.ipg_tasktypecodeEnum == ipg_TaskType1.FacilityDIFnotfound))
                {
                    taskManager.CreateTask(targetRef, "Facility DIF document not found", "Facility DIF document not found", ipg_TaskType1.FacilityDIFnotfound);
                }
            }
        }
    }
}
