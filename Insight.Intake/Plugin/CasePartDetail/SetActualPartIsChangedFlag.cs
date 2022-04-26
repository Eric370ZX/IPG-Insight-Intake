using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.CasePartDetail
{
    public class SetActualPartIsChangedFlag : PluginBase
    {
        public SetActualPartIsChangedFlag() : base(typeof(SetActualPartIsChangedFlag))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_casepartdetail.EntityLogicalName, PostOperationHandlerOnUpdate);
            RegisterEvent(PipelineStages.PreValidation, MessageNames.Delete, SalesOrder.EntityLogicalName, PostOperationHandlerOnDelete);
        }

        private void PostOperationHandlerOnDelete(LocalPluginContext context)
        {
            var service = context.OrganizationService;
            var target = context.TargetRef();

            var query = new QueryExpression(ipg_casepartdetail.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(ipg_casepartdetail.PrimaryIdAttribute
                , ipg_casepartdetail.Fields.ipg_IsChanged),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_casepartdetail.Fields.ipg_PurchaseOrderId, ConditionOperator.Equal, target.Id)
                    }
                }
            };

            var actualParts = service.RetrieveMultiple(query).Entities;
            foreach(var part in actualParts)
            {
                var actualPart = part.ToEntity<ipg_casepartdetail>();
                actualPart.ipg_IsChanged = true;
                service.Update(actualPart);
            }
        }

        private void PostOperationHandlerOnUpdate(LocalPluginContext context)
        {
            if (context.PluginExecutionContext.Depth > 1)
                return;

            var service = context.OrganizationService;                  
            var target = context.Target<ipg_casepartdetail>();
            var actualPartPostImage = context.PostImage<ipg_casepartdetail>();

            if (actualPartPostImage.ipg_SalesOrderDetail == null || actualPartPostImage.ipg_PurchaseOrderId == null)
            {
                target.ipg_IsChanged = true;
            }
            else
            {
                target.ipg_IsChanged = IsActualPartChanged(service, actualPartPostImage);
            }

            service.Update(target);
        }

        private bool IsActualPartChanged(IOrganizationService service, ipg_casepartdetail target)
        {
            SalesOrderDetail salesOrderDetail = service.Retrieve(SalesOrderDetail.EntityLogicalName, target.ipg_SalesOrderDetail.Id, new ColumnSet("quantity", "ipg_serialnumber", "ipg_lotnumber", "ipg_enteredunitcost", "ipg_enteredshipping", "tax")).ToEntity<SalesOrderDetail>();
            SalesOrder salesOrder = service.Retrieve(SalesOrder.EntityLogicalName, target.ipg_PurchaseOrderId.Id, new ColumnSet("ipg_potypecode")).ToEntity<SalesOrder>();
            return target.ipg_potypecode.Value != salesOrder.ipg_potypecode.Value ||
                   target.ipg_quantity != salesOrderDetail.Quantity ||
                   target.ipg_serialnumber != salesOrderDetail.ipg_serialnumber ||
                   target.ipg_lotnumber != salesOrderDetail.ipg_lotnumber ||
                   target.ipg_enteredunitcost.Value != salesOrderDetail.ipg_enteredunitcost.Value ||
                   target.ipg_enteredshipping.Value != salesOrderDetail.ipg_enteredshipping.Value ||
                   target.ipg_enteredtax.Value != salesOrderDetail.Tax.Value;
        }
    }
}
