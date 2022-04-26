using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk.Client;
using System.Linq;

namespace Insight.Intake.Plugin.TaskEntity
{
    public class CloseByExceptionApprovedStatus : PluginBase
    {
        public CloseByExceptionApprovedStatus() : base(typeof(CloseByExceptionApprovedStatus))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, null, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            if (localPluginContext.PluginExecutionContext.MessageName != MessageNames.Update)
                return;

            var target = localPluginContext.Target<Task>();
            var postImage = localPluginContext.PostImage<Task>();
            var service = localPluginContext.OrganizationService;
            var serviceContext = new OrganizationServiceContext(service);
            var initiatingUserId = localPluginContext.PluginExecutionContext.InitiatingUserId;

            if ((target?.Attributes?.ContainsKey(Task.Fields.ipg_is_exception_approved) ?? false) && target?.ipg_is_exception_approved == true)
            {
                var caseId = postImage?.ipg_caseid?.Id;
                var caseEntity = serviceContext.CreateQuery<Incident>()
                    .FirstOrDefault(incident => incident.Id == caseId);
                var activityType = serviceContext.CreateQuery<ipg_activitytype>()
                    .FirstOrDefault(type => type.ipg_name == Constants.ActivityTypeNames.ExceptionApproved);
                service.Create(new ipg_importanteventslog
                {
                    ipg_name = "Exception override approved",
                    ipg_activity = activityType?.ToEntityReference(),
                    ipg_activitydescription = "Exception override approved",
                    ipg_caseid = caseId?.ToString(),
                    ipg_casenumber = caseEntity?.ToEntityReference(),
                    ipg_casenumbertext = caseEntity?.Title
                });

                var taskManager = new TaskManager(service, localPluginContext.TracingService, target?.ToEntityReference(), initiatingUserId);
                taskManager.CloseTask(Constants.ActivityTypeNames.ExceptionApproved, Task_StatusCode.Resolved, false);
            }
        }
    }
}