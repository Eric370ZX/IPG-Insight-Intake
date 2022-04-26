using Insight.Intake.Plugin.Common.Benefits;

namespace Insight.Intake.Plugin.Case
{
    public class UpdateCaseInOutNetwork : PluginBase
    {
        public UpdateCaseInOutNetwork() : base(typeof(UpdateCaseInOutNetwork))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Incident.EntityLogicalName, PreOperationCreateHandler);
        }

        void PreOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;

            Incident target = localPluginContext.Target<Incident>();
            if (target == null)
            {
                localPluginContext.Trace("Target is empty. Exit");
                return;
            }

            var caseBenefitSwitcher = new CaseBenefitSwitcher(service, localPluginContext.TracingService);

            if(target.ipg_CarrierId != null)
            {
                caseBenefitSwitcher.UpdateInOutNetwork(target.Id, target.ipg_CarrierId.Id);
            }
            if(target.ipg_SecondaryCarrierId != null)
            {
                caseBenefitSwitcher.UpdateInOutNetwork(target.Id, target.ipg_SecondaryCarrierId.Id);
            }
        }
    }
}
