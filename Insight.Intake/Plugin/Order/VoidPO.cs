using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.Linq;

namespace Insight.Intake.Plugin.Order
{
    public class VoidPO : PluginBase
    {
        public VoidPO() : base(typeof(VoidPO))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGIntakeActionsVoidPO", null, PostOperationCreateHandler);
        }

        private void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var crmContext = new OrganizationServiceContext(service);
            var target = (EntityReference)context.InputParameters["Target"];

            var parts = (from part in crmContext.CreateQuery<ipg_casepartdetail>()
                         where part.ipg_PurchaseOrderId.Id == target.Id
                         select part).ToList();
            var partFields = typeof(ipg_casepartdetail.Fields).GetFields();
            foreach (var part in parts)
            {
                /*var actualPart = new ipg_casepartdetail()
                {
                    ipg_quantity = -part.ipg_quantity,
                    ipg_enteredunitcost = part.ipg_enteredunitcost,
                    ipg_caseid = part.ipg_caseid,
                    ipg_potypecode = part.ipg_potypecode,
                    ipg_enteredshipping = part.ipg_enteredshipping,
                    ipg_pounitcost = part.ipg_pounitcost,
                    ipg_enteredtax = part.ipg_enteredtax,
                    ipg_productid = part.ipg_productid,
                    ipg_uomid = part.ipg_uomid,
                    ipg_manufacturerid = part.ipg_manufacturerid,
                    StateCode = part.StateCode,
                    StatusCode = part.StatusCode
                };*/
                var actualPart = new ipg_casepartdetail();
                foreach (var attr in part.Attributes)
                {
                    foreach (var partField in partFields)
                    {
                        if (partField.GetValue(null).ToString() == attr.Key)
                        {
                            var setMethod = typeof(ipg_casepartdetail).GetProperty(partField.Name).GetSetMethod();
                            if (attr.Value != null && attr.Key != ipg_casepartdetail.PrimaryIdAttribute && setMethod != null)
                            {
                                actualPart[attr.Key] = attr.Value;
                            }
                            break;
                        }
                    }
                }
                if (actualPart.ipg_quantity != null)
                {
                    actualPart.ipg_quantity = -actualPart.ipg_quantity;
                }
                service.Create(actualPart);
            }

            var request = new FulfillSalesOrderRequest
            {
                OrderClose = new OrderClose
                {
                    SalesOrderId = target

                },
                Status = new OptionSetValue((int)SalesOrder_StatusCode.Voided),
            };
            service.Execute(request);
        }

    }

}
