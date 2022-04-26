using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Insight.Intake.Extensions;
using Insight.Intake.Plugin.Managers;

namespace Insight.Intake.Plugin.SLA
{
    public class CreateReferral : PluginBase
    {
        public CreateReferral() : base(typeof(CreateReferral))
        {
            RegisterEvent(
                PipelineStages.PostOperation,
                "Create",
                ipg_referral.EntityLogicalName,
                OnCreateAsync);
        }

        private void OnCreateAsync(LocalPluginContext pluginContext)
        {
            ipg_referral target = (pluginContext.PluginExecutionContext.InputParameters["Target"] as Entity).ToEntity<ipg_referral>();

            if (target.ipg_Origin != null && target.ipg_Origin.Value == (int)Incident_CaseOriginCode.Fax &&
                target.ipg_SourceDocumentId != null)
            {
                var service = pluginContext.SystemOrganizationService;
                var taskManager = new TaskManager(pluginContext.OrganizationService,
                    pluginContext.TracingService,
                    null,
                    pluginContext.PluginExecutionContext.InitiatingUserId);

                var sourceDocument = service.Retrieve<ipg_document>(target.ipg_SourceDocumentId.Id, new ColumnSet("ipg_originaldocdate", "createdon"));
                var taskTypeRef = taskManager.GetTaskTypeByTaskTypeId(TaskManager.TaskTypeIds.SLA_Create_Referral, new ColumnSet(false)).ToEntityReference();

                var startDate = sourceDocument.ipg_originaldocdate ?? sourceDocument.CreatedOn.Value;

                var tasktemplate = new Task()
                {
                    ScheduledStart = startDate,
                    ActualStart = target.CreatedOn,
                    ActualEnd = target.CreatedOn,
                };

                tasktemplate.Subcategory = tasktemplate.ActualEnd <= tasktemplate.ScheduledEnd ? "SLA Met" : "SLA Not Met";

                var taskid = taskManager.CreateTask(target.ToEntityReference(), taskTypeRef, tasktemplate);

                pluginContext.SystemOrganizationService.Update(new Task()
                {
                    Id = taskid,
                    StateCode = TaskState.Completed,
                    StatusCode = Task_StatusCode.Resolved.ToOptionSetValue()
                });
            }
        }
    }
}