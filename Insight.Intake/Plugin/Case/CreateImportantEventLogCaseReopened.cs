using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.Plugin.Case
{
    public class CreateImportantEventLogCaseReopened : PluginBase
    {
        public CreateImportantEventLogCaseReopened() : base(typeof(CreateImportantEventLogCaseReopened))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Incident.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;
            var executionContext = localPluginContext.PluginExecutionContext;
            var postImage = localPluginContext.PostImage<Incident>();
            if (postImage == null)
            {
                throw new InvalidPluginExecutionException("PostImage Case entity can not be null.");
            }

            if (Enum.Equals(postImage.ipg_CaseStatusEnum, ipg_CaseStatus.Open))
            {
                ImportantEventManager eventManager = new ImportantEventManager(service);
                var logId = eventManager.CreateImportantEventLog(postImage, executionContext.UserId, Constants.EventIds.ET27);
                eventManager.SetCaseOrReferralPortalHeader(postImage, Constants.EventIds.ET27);

                executionContext.OutputParameters["Success"] = logId != Guid.Empty;
            }
        }
    }
}