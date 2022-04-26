using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using System.Linq;

namespace Insight.Intake.Plugin.Case
{
    public class CaseDisplayStatusUpdated : PluginBase
    {
        public CaseDisplayStatusUpdated() : base(typeof(CaseDisplayStatusUpdated))
        {
            RegisterEvent(PipelineStages.PostOperation, "Update", Incident.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PreOperation, "Update", Incident.EntityLogicalName, PreOperationHandler);

        }
        private void PreOperationHandler(LocalPluginContext localPluginContext) {
            var targetCase = localPluginContext.Target<Incident>();
            if (targetCase.ipg_casestatusdisplayedid == null)
            {
                return; 
            }

            if (targetCase.ipg_casestatusdisplayedid.Id == Constants.CaseStatusDisplayedGuids.BillingInProgress) {
                targetCase.ipg_islocked = true;
            }
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var targetCase = localPluginContext.Target<Incident>();
            var postImage = localPluginContext.PostImage<Incident>();
            if (targetCase.ipg_casestatusdisplayedid == null)
            {
                return;
            }

            var caseManager = new CaseManager(localPluginContext.OrganizationService, localPluginContext.TracingService, targetCase.ToEntityReference());
            var holdConfigs = caseManager.GetCaseholdParameters();
            var actualHoldConfigs = holdConfigs
                .Where(p => p.CaseStatus?.Id == targetCase.ipg_casestatusdisplayedid.Id
                && (int)p.CaseHoldReason == postImage.GetAttributeValue<OptionSetValue>("ipg_caseholdreason")?.Value);
            if (actualHoldConfigs.Any())
            {
                caseManager.ClouseOutstangindTasks();
            }
        }
    }
}