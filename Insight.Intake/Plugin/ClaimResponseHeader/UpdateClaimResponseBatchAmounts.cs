using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.ClaimResponseHeader
{
    public class UpdateClaimResponseBatchAmounts : PluginBase
    {
        public UpdateClaimResponseBatchAmounts() : base(typeof(UpdateClaimResponseBatchAmounts))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_claimresponseheader.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_claimresponseheader.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Delete, ipg_claimresponseheader.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                ipg_claimresponseheader claimResponseHeader = null;

                if (context.InputParameters["Target"] is Entity)
                {
                    var claimResponseHeaderRef = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_claimresponseheader>();
                    claimResponseHeader = service.Retrieve(claimResponseHeaderRef.LogicalName, claimResponseHeaderRef.Id, new ColumnSet(nameof(ipg_claimresponseheader.ipg_ClaimResponseBatchId).ToLower(), nameof(ipg_claimresponseheader.ipg_PaymentType).ToLower())).ToEntity<ipg_claimresponseheader>();
                }
                else if (context.InputParameters["Target"] is EntityReference claimResponseHeaderRef)
                {
                    claimResponseHeader = service.Retrieve(claimResponseHeaderRef.LogicalName, claimResponseHeaderRef.Id, new ColumnSet(nameof(ipg_claimresponseheader.ipg_ClaimResponseBatchId).ToLower(), nameof(ipg_claimresponseheader.ipg_PaymentType).ToLower())).ToEntity<ipg_claimresponseheader>();
                }

                if(claimResponseHeader.GetAttributeValue<OptionSetValue>(nameof(ipg_claimresponseheader.ipg_PaymentType).ToLower()).Value != (int)ipg_ClaimResponseHeaderType.Refund)
                {
                    return;
                }

                var queryExpression = new QueryExpression(ipg_claimresponseheader.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(nameof(ipg_claimresponseheader.ipg_AmountPaid_new).ToLower()),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(nameof(ipg_claimresponseheader.ipg_ClaimResponseBatchId).ToLower(), ConditionOperator.Equal, claimResponseHeader.GetAttributeValue<EntityReference>(nameof(ipg_claimresponseheader.ipg_ClaimResponseBatchId).ToLower()).Id),
                            new ConditionExpression(nameof(ipg_claimresponseheader.StateCode).ToLower(), ConditionOperator.Equal, 0) //active
                        }
                    }
                };
                if (context.MessageName == MessageNames.Delete)
                {
                    queryExpression.Criteria.Conditions.Add(new ConditionExpression(nameof(ipg_claimresponseheader.ipg_claimresponseheaderId).ToLower(), ConditionOperator.NotEqual, claimResponseHeader.Id));
                }

                EntityCollection headers = service.RetrieveMultiple(queryExpression);
                decimal sum = headers.Entities.Sum(header => (header.GetAttributeValue<Money>(nameof(ipg_claimresponseheader.ipg_AmountPaid_new).ToLower()) == null ? 0 : header.GetAttributeValue<Money>(nameof(ipg_claimresponseheader.ipg_AmountPaid_new).ToLower()).Value));
                
                var batch = new ipg_claimresponsebatch()
                {
                    Id = claimResponseHeader.GetAttributeValue<EntityReference>(nameof(ipg_claimresponseheader.ipg_ClaimResponseBatchId).ToLower()).Id,
                    ipg_TotalAmount_new = new Money(sum)
                };
                service.Update(batch);
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}
