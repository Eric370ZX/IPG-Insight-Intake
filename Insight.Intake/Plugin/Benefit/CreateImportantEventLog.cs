using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Benefit
{
    public class CreateImportantEventLog : PluginBase
    {
        public CreateImportantEventLog() : base(typeof(CreateImportantEventLog))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_benefit.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_benefit.EntityLogicalName, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var benefit = localPluginContext.Target<ipg_benefit>();
            if (benefit.ipg_BenefitSource != null)
            {
                if (benefit.ipg_CaseId != null)
                {
                    ImportantEventManager eventManager = new ImportantEventManager(localPluginContext.OrganizationService);
                    string[] descriptionParams = null;
                    var assosiatedCase = localPluginContext.OrganizationService.Retrieve(Incident.EntityLogicalName, benefit.ipg_CaseId.Id, new ColumnSet(Incident.Fields.Title));
                    if (benefit.ipg_BenefitSource.Value == (int)ipg_BenefitSources.EBV)
                    {
                        descriptionParams = new[] { "EBV" };
                    }
                    if (benefit.ipg_BenefitSource.Value == (int)ipg_BenefitSources.BVF)
                    {
                        descriptionParams = new[] { "BVF" };
                    }
                    eventManager.CreateImportantEventLog(assosiatedCase, localPluginContext.PluginExecutionContext.InitiatingUserId, "ET2", descriptionParams);
                    eventManager.SetCaseOrReferralPortalHeader(assosiatedCase, "ET2");
                }
            }
        }
    }
}