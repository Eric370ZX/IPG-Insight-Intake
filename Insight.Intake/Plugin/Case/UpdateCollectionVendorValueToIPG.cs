using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace Insight.Intake.Plugin.Case
{
    public class UpdateCollectionVendorValueToIPG : PluginBase
    {
        public UpdateCollectionVendorValueToIPG() : base(typeof(UpdateCollectionVendorValueToIPG))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, Incident.EntityLogicalName, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var crmContext = new OrganizationServiceContext(service);

            var preImage = localPluginContext.PreImage<Incident>();
            if ((preImage.ipg_StateCode?.Value == (int)ipg_CaseStateCodes.PatientServices) &&
                (preImage.ipg_CollectionVendor?.Value == (int)Incident_ipg_CollectionVendor.SCG))
            {
                var target = ((Entity)context.InputParameters["Target"]).ToEntity<Incident>();
                if (target.ipg_StateCode?.Value == (int)ipg_CaseStateCodes.CarrierServices)
                {
                    target.ipg_CollectionVendor = new OptionSetValue((int)Incident_ipg_CollectionVendor.IPG);
                }
            }
        }
    }
}
