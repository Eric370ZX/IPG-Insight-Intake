using Insight.Intake.Plugin.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Account
{
    public class FacilityBAADocumentValidation : PluginBase
    {
        public FacilityBAADocumentValidation() : base(typeof(FacilityBAADocumentValidation))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Insight.Intake.Account.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Insight.Intake.Account.EntityLogicalName, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var target = localPluginContext.Target<Insight.Intake.Account>();
            var postImage = localPluginContext.PostImage<Insight.Intake.Account>();
            if (postImage.CustomerTypeCodeEnum != Account_CustomerTypeCode.Facility) 
            {
                return;
            }
            var targetRef = target.ToEntityReference();
            var documentManager = new DocumentManager(localPluginContext.OrganizationService, localPluginContext.TracingService);
            var relatedDocs = documentManager.GetDocsByAccountOfType(targetRef, "BAA");
            if (!relatedDocs.Any())
            {
                var taskManager = new TaskManager(localPluginContext.OrganizationService, localPluginContext.TracingService, null, localPluginContext.PluginExecutionContext.UserId);
                var relatedTasks= taskManager.GetTasks(targetRef);
                if (!relatedTasks.Any(p=>p.ipg_tasktypecodeEnum==ipg_TaskType1.FacilityBAAnotfound)) {
                    taskManager.CreateTask(targetRef, "Facility BAA document not found", "Facility BAA document not found", ipg_TaskType1.FacilityBAAnotfound);
                }
            }
        }
    }
}
