using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Client;

namespace Insight.Intake.Plugin.Order
{
    public class PopulateDownloadedFromPortalField : PluginBase
    {

        public PopulateDownloadedFromPortalField() : base(typeof(PopulateDownloadedFromPortalField))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, SalesOrder.EntityLogicalName, PreOperationHandler);
        }

        private void PreOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;

            var entity = ((Entity)context.InputParameters["Target"]).ToEntity<SalesOrder>();
            if(entity.ipg_downloadedfromportal == true)
            {
                var order = service.Retrieve(entity.LogicalName, entity.Id, new ColumnSet(nameof(SalesOrder.ipg_potypecode).ToLower())).ToEntity<SalesOrder>();
                if(order.ipg_potypecode?.Value == (int)ipg_PurchaseOrderTypes.CPA || order.ipg_potypecode?.Value == (int)ipg_PurchaseOrderTypes.MPO)
                {
                    entity.ipg_downloadedfromportal = false;
                }
            }
        }
    }
}