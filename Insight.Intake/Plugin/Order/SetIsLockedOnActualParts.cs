using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Order
{
    public class SetIsLockedOnActualParts : PluginBase
    {
        private readonly List<int> LockingStatusCodes = new List<int> { (int)SalesOrder_StatusCode.InvoiceReceived, (int)SalesOrder_StatusCode.InReview,
                                                       (int)SalesOrder_StatusCode.VerifiedforPayment, (int)SalesOrder_StatusCode.Partial_Active,
                                                       (int)SalesOrder_StatusCode.CommtoMFG, (int)SalesOrder_StatusCode.CommtoFacility,
                                                       (int)SalesOrder_StatusCode.Generated};

        public SetIsLockedOnActualParts() : base(typeof(SetIsLockedOnActualParts))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, SalesOrder.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PreValidation, MessageNames.Delete, SalesOrder.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            if (context.MessageName == MessageNames.Update && context.PostEntityImages.Contains("PostImage"))
            {
                var postImage = localPluginContext.PostImage<SalesOrder>();
                UpdateActualParts(service, postImage);
            }
            else if(context.MessageName == MessageNames.Delete)
            {
                var order = localPluginContext.TargetRef();
                var actualParts = GetActualPartsOnPO(service, order.Id);
                foreach (var part in actualParts)
                {
                    part.ipg_islocked = false;
                    service.Update(part);                  
                }
            }
        }

        private void UpdateActualParts(IOrganizationService service, SalesOrder order)
        {
            var isActualPartLocked = IsActualPartLocked((int)order?.StateCode, (int)order?.StatusCode.Value);
            var actualParts = GetActualPartsOnPO(service, order.Id);
            foreach (var part in actualParts)
            {
                if (!part.ipg_islocked.HasValue || part.ipg_islocked != isActualPartLocked)
                {
                    part.ipg_islocked = isActualPartLocked;
                    service.Update(part);
                }
            }
        }

        private bool IsActualPartLocked(int? stateCode, int? statusCode)
        {
            return stateCode.HasValue && 
                (stateCode.Value == (int)SalesOrderState.Fulfilled || stateCode.Value == (int)SalesOrderState.Active) &&
                LockingStatusCodes.Contains(statusCode.Value)
                   ? true
                   : false;
        }

        private IEnumerable<ipg_casepartdetail> GetActualPartsOnPO(IOrganizationService service, Guid orderId)
        {
            var query = new QueryExpression(ipg_casepartdetail.EntityLogicalName) { Distinct = true };
            query.ColumnSet.AddColumns(ipg_casepartdetail.Fields.ipg_casepartdetailId, ipg_casepartdetail.Fields.ipg_islocked);
            query.Criteria.AddCondition(ipg_casepartdetail.Fields.ipg_PurchaseOrderId, ConditionOperator.Equal, orderId);
            return service.RetrieveMultiple(query).Entities.Select(entity=>entity.ToEntity<ipg_casepartdetail>());
        }
    }
}

