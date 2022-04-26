using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Insight.Intake.Helpers;
using System.Linq;

namespace Insight.Intake.Plugin.ClaimResponse
{
    public class CheckHeaderTotals : PluginBase
    {

        public CheckHeaderTotals() : base(typeof(CheckHeaderTotals))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, ipg_claimresponseheader.EntityLogicalName, PreOperationCreateHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, ipg_claimresponseheader.EntityLogicalName, PreOperationCreateHandler);
        }

        private void PreOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                var target = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_claimresponseheader>();

                var claimResponseHeader = target;
                if (context.MessageName == MessageNames.Update)
                {
                    claimResponseHeader = service.Retrieve(ipg_claimresponseheader.EntityLogicalName, target.Id, new ColumnSet(nameof(ipg_claimresponseheader.ipg_ClaimResponseBatchId).ToLower())).ToEntity<ipg_claimresponseheader>();
                }

                var batch = service.Retrieve(ipg_claimresponsebatch.EntityLogicalName, claimResponseHeader.ipg_ClaimResponseBatchId.Id, new ColumnSet(nameof(ipg_claimresponsebatch.ipg_TotalAmount_new).ToLower())).ToEntity<ipg_claimresponsebatch>();
                decimal batchTotal = (batch.GetAttributeValue<Money>(nameof(ipg_claimresponsebatch.ipg_TotalAmount_new).ToLower()) == null ? 0 : batch.GetAttributeValue<Money>(nameof(ipg_claimresponsebatch.ipg_TotalAmount_new).ToLower()).Value);

                var queryExpression = new QueryExpression(ipg_claimresponseheader.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(nameof(ipg_claimresponseheader.ipg_AmountPaid_new).ToLower()),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(nameof(ipg_claimresponseheader.ipg_ClaimResponseBatchId).ToLower(), ConditionOperator.Equal, claimResponseHeader.ipg_ClaimResponseBatchId.Id),
                            new ConditionExpression(nameof(ipg_claimresponseheader.ipg_claimresponseheaderId).ToLower(), ConditionOperator.NotEqual, claimResponseHeader.Id)
                        }
                    }
                };
                
                EntityCollection claimResponseHeaders = service.RetrieveMultiple(queryExpression);
                decimal? headersTotal = claimResponseHeaders.Entities.Sum(header => (header.GetAttributeValue<Money>(nameof(ipg_claimresponseheader.ipg_AmountPaid_new).ToLower()) == null ? 0 : header.GetAttributeValue<Money>(nameof(ipg_claimresponseheader.ipg_AmountPaid_new).ToLower()).Value));
                headersTotal += (target.ipg_AmountPaid_new == null ? 0 : target.ipg_AmountPaid_new.Value);
                if (batchTotal < headersTotal)
                {
                    throw new Exception("Batch total amount is less than summary of headers paid amount. The header wasn't saved.");
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}