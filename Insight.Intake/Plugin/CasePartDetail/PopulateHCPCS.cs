using System.Linq;

namespace Insight.Intake.Plugin.CasePartDetail
{
    public class PopulateHCPCS : PluginBase
    {
        public PopulateHCPCS() : base(typeof(PopulateHCPCS))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, ipg_casepartdetail.EntityLogicalName, PreValidationRetrieveMultipleHandler);
        }

        private void PreValidationRetrieveMultipleHandler(LocalPluginContext localPluginContext)
        {
            var target = localPluginContext.Target<ipg_casepartdetail>();

            if (string.IsNullOrEmpty(target.ipg_hcpcs) && target.ipg_productid != null)
            {
                using (CrmServiceContext context = new CrmServiceContext(localPluginContext.OrganizationService))
                {
                    var hcpcsCodeName = context.ProductSet
                        .FirstOrDefault(x => x.Id == target.ipg_productid.Id)?.ipg_HCPCSCodeId?.Name;
                    target.ipg_hcpcs = hcpcsCodeName;
                }
            }
        }
    }
}
