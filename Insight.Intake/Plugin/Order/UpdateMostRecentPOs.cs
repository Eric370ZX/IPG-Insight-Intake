using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Order
{
    public class UpdateMostRecentPOs : PluginBase
    {
        public UpdateMostRecentPOs() : base(typeof(UpdateMostRecentPOs))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, SalesOrder.EntityLogicalName, PreOperationHandler);
        }


        private void PreOperationHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;
            var salesOrder = localPluginContext.Target<SalesOrder>();

            if (salesOrder.ipg_CaseId == null)
            {
                throw new InvalidPluginExecutionException(OperationStatus.Failed, $"Purchase Order '{nameof(salesOrder.ipg_CaseId)}' field cannot be null.");
            }

            try
            {
                var purchaseOrders = RetrieveCaseMostRecentPOs(salesOrder, service);
                foreach (var po in purchaseOrders)
                {
                    service.Update(new SalesOrder() {Id = po.Id, ipg_MostRecent = false });
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(OperationStatus.Failed, ex.ToString());
            }
        }

        private IEnumerable<SalesOrder> RetrieveCaseMostRecentPOs(SalesOrder target, IOrganizationService service)
        {
            return service.RetrieveMultiple(new QueryExpression(SalesOrder.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                Criteria =
                {
                   Conditions =
                    {
                        new ConditionExpression(SalesOrder.Fields.ipg_CaseId, ConditionOperator.Equal, target.ipg_CaseId.Id),
                        new ConditionExpression(SalesOrder.Fields.ipg_MostRecent, ConditionOperator.Equal, true),
                        new ConditionExpression(SalesOrder.Fields.Id, ConditionOperator.NotEqual, target.Id),
                        new ConditionExpression(SalesOrder.Fields.ipg_potypecode, ConditionOperator.Equal, (int)target.ipg_potypecodeEnum)
                    }
                }

            }).Entities.Select(o => o.ToEntity<SalesOrder>());
        }
    }
}
