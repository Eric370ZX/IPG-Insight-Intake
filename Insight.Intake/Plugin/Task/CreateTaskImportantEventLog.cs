using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Insight.Intake.Plugin.TaskEntity
{
    public class CreateTaskImportantEventLog : PluginBase
    {
        private const string CheckBandingRulesTaskTypeName = "Banding - Approval Required";
        public CreateTaskImportantEventLog() : base(typeof(CreateTaskImportantEventLog))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Task.EntityLogicalName, PostUpdateHeandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Task.EntityLogicalName, PostCreateHeandler);
        }
        #region PostCreate
        private void PostCreateHeandler(LocalPluginContext localPluginContext)
        {
            ProcessET3LogCreation(localPluginContext);
        }

        private void ProcessET3LogCreation(LocalPluginContext localContext)
        {
            var targetTask = localContext.Target<Task>();

            if (IsBindingRulesTaskType(localContext, targetTask?.ipg_tasktypeid?.Id))
            {
                var assosiatedCase = localContext.OrganizationService.Retrieve(Incident.EntityLogicalName, targetTask.ipg_caseid.Id, new ColumnSet(Incident.Fields.Title));
                if (assosiatedCase != null)
                {
                    var eventManager = new ImportantEventManager(localContext.OrganizationService);
                    eventManager.CreateImportantEventLog(assosiatedCase, localContext.PluginExecutionContext.InitiatingUserId, "ET3");
                    eventManager.SetCaseOrReferralPortalHeader(assosiatedCase, "ET3");
                }
            }
        }
        #endregion

        #region PostUpdate
        private void PostUpdateHeandler(LocalPluginContext localPluginContext)
        {
            ProcessET4LogCreation(localPluginContext);
            ProcessET9LogCreation(localPluginContext);
        }
        private void ProcessET4LogCreation(LocalPluginContext localContext)
        {
            var targetTask = localContext.Target<Task>();
            var preImageTaskEntity = localContext.PreImage<Task>();

            if (targetTask.StatusCodeEnum == Task_StatusCode.Resolved &&
                IsBindingRulesTaskType(localContext, preImageTaskEntity?.ipg_tasktypeid?.Id))
            {
                var assosiatedCase = localContext.OrganizationService.Retrieve(Incident.EntityLogicalName, preImageTaskEntity.ipg_caseid.Id, new ColumnSet(Incident.Fields.Title));
                if (assosiatedCase != null)
                {
                    var eventManager = new ImportantEventManager(localContext.OrganizationService);
                    eventManager.CreateImportantEventLog(assosiatedCase, localContext.PluginExecutionContext.InitiatingUserId, "ET4");
                    eventManager.SetCaseOrReferralPortalHeader(assosiatedCase, "ET4");
                }
            }
        }
        private void ProcessET9LogCreation(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;
            var context = localPluginContext.PluginExecutionContext;
            var taskEntity = localPluginContext.Target<Task>();
            var preImageTaskEntity = localPluginContext.PreImage<Task>();

            if ((preImageTaskEntity.Subcategory == Constants.TaskSubCategoryNames.NoClaimHolds))
                return;

            var logId = Guid.Empty;
            var userRef = context.InitiatingUserId;
            var userEntity = (SystemUser)service.Retrieve(SystemUser.EntityLogicalName, userRef, new ColumnSet(SystemUser.Fields.FullName));

            var caseRef = preImageTaskEntity.ipg_caseid;
            if (taskEntity != null && caseRef != null && userEntity != null && preImageTaskEntity != null)
            {
                var caseEntity = service.Retrieve(caseRef.LogicalName, caseRef.Id, new ColumnSet(Incident.Fields.Title));
                var importantEventManager = new ImportantEventManager(service);
                logId = importantEventManager.CreateImportantEventLog(caseEntity, userRef, Constants.EventIds.ET9, new[] { userEntity.FullName });
                importantEventManager.SetCaseOrReferralPortalHeader(caseEntity, Constants.EventIds.ET9);
            }

            if (logId != Guid.Empty)
                context.OutputParameters["Success"] = true;
        }
        #endregion
        private bool IsBindingRulesTaskType(LocalPluginContext localContext, Guid? SuorceTaskTypeId)
        {
            var checkBandingRulesTaskTypeId = new TaskManager(localContext.OrganizationService, localContext.TracingService).GetTaskTypeByName(CheckBandingRulesTaskTypeName)?.Id;
            return checkBandingRulesTaskTypeId.HasValue && SuorceTaskTypeId.HasValue && SuorceTaskTypeId == checkBandingRulesTaskTypeId;
        }
    }
}
