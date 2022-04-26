using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk.Client;
using System.Linq;

namespace Insight.Intake.Plugin.Account
{
    public class VerifyBenefitByCarrier : PluginBase
    {
        public VerifyBenefitByCarrier() : base(typeof(VerifyBenefitByCarrier))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Insight.Intake.Account.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var orgService = new OrganizationServiceContext(service);
            var target = localPluginContext.Target<Insight.Intake.Account>();

            var cases = orgService.CreateQuery<Incident>()
                .Where(incident => incident.ipg_CarrierId.Id == target.Id)
                .Where(incident => incident.ipg_EBVResultEnum != ipg_EBVResults.FAILED)
                .Select(incident => new Incident { Id = incident.Id })
                .ToList();

            cases.ForEach(c =>
            {
                localPluginContext.Trace($"{nameof(VerifyBenefitByCarrier)} - start benefit verification execution");
                service.ExecuteWorkflow(Insight.Intake.Helpers.Constants.Workflows.VerifyBenefitsAsyncId, c.Id);
            });
        }
    }
}