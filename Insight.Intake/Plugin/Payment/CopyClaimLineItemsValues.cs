using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Insight.Intake.Helpers;
using System.Linq;

namespace Insight.Intake.Plugin.Payment
{
    public class CopyClaimLineItemsValues : PluginBase
    {

        public CopyClaimLineItemsValues() : base(typeof(CopyClaimLineItemsValues))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_payment.EntityLogicalName, PostOperationCreateHandler);
        }

        private void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                var payment = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_payment>();
                if ((payment.ipg_Claim != null) && (bool)(payment.ipg_ClaimToggle ?? false))
                {
                    QueryExpression queryExpression = new QueryExpression(ipg_claimlineitem.EntityLogicalName)
                    {
                        ColumnSet = new ColumnSet(nameof(ipg_claimlineitem.ipg_AllowedTemp).ToLower(), nameof(ipg_claimlineitem.ipg_PaidTemp).ToLower()),
                        Criteria = new FilterExpression(LogicalOperator.And)
                        {
                            Conditions =
                            {
                                new ConditionExpression(nameof(ipg_claimlineitem.ipg_claimid).ToLower(), ConditionOperator.Equal, payment.ipg_Claim.Id)
                            }
                        }
                    };
                    EntityCollection claimLineItems = service.RetrieveMultiple(queryExpression);
                    foreach (Entity claimLineItem in claimLineItems.Entities)
                    {
                        var cli = new ipg_claimlineitem();
                        cli.Id = claimLineItem.Id;
                        cli.ipg_allowed = claimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_AllowedTemp).ToLower());
                        cli.ipg_paid = claimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_PaidTemp).ToLower());
                        service.Update(cli);
                    }
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}