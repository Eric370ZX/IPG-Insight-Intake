using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.CasePartDetail
{
    public class UpdatePOLines : PluginBase
    {
        public UpdatePOLines() : base(typeof(UpdatePOLines))
        {
            RegisterEvent(PipelineStages.PostOperation, "Update", ipg_casepartdetail.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext context)
        {
            var service = context.OrganizationService;
            var target = context.Target<ipg_casepartdetail>();
            var preImage = context.PluginExecutionContext.PreEntityImages["PreImage"].ToEntity<ipg_casepartdetail>();

            if (target.ipg_quantity != null && preImage.ipg_caseid != null && preImage.ipg_PurchaseOrderId != null && preImage.ipg_productid != null)
            {
                var crmContext = new OrganizationServiceContext(service);

                Money enteredUnitCost = preImage.ipg_enteredunitcost ?? new Money(0);

                var orderDetail = new SalesOrderDetail()
                {
                    SalesOrderId = preImage.ipg_PurchaseOrderId,
                    ProductId = preImage.ipg_productid,
                    Quantity = target.ipg_quantity - preImage.ipg_quantity,
                    PricePerUnit = enteredUnitCost,
                    UoMId = preImage.ipg_uomid,
                    ipg_caseid = preImage.ipg_caseid,
                    ipg_enteredshipping = preImage.ipg_enteredshipping ?? new Money(0),
                    ipg_enteredunitcost = enteredUnitCost,
                    ipg_poextcost = new Money((enteredUnitCost.Value + preImage.ipg_enteredtax.Value + preImage.ipg_enteredshipping.Value) * (target.ipg_quantity.Value - preImage.ipg_quantity.Value)),
                    ipg_pounitcost = preImage.ipg_pounitcost,
                    Tax = preImage.ipg_enteredtax
                };
                service.Create(orderDetail);
            }
        }
    }
}
