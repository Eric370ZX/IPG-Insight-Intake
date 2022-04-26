using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Claim
{
    public class UpdateExpectedReimbursement : PluginBase
    {

        public UpdateExpectedReimbursement() : base(typeof(UpdateExpectedReimbursement))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_claimlineitem.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_claimlineitem.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Delete, ipg_claimlineitem.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                ipg_claimlineitem claimLineItemEnt = null;
                if (context.InputParameters["Target"] is Entity)
                {
                    var claimLineItem = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_claimlineitem>();
                    claimLineItemEnt = service.Retrieve(claimLineItem.LogicalName, claimLineItem.Id, new ColumnSet(nameof(ipg_claimlineitem.ipg_claimid).ToLower())).ToEntity<ipg_claimlineitem>();
                }
                else
                {
                    var claimLineItem = ((EntityReference)context.InputParameters["Target"]);
                    claimLineItemEnt = service.Retrieve(claimLineItem.LogicalName, claimLineItem.Id, new ColumnSet(nameof(ipg_claimlineitem.ipg_claimid).ToLower())).ToEntity<ipg_claimlineitem>();
                }

                var queryExpression = new QueryExpression(ipg_claimlineitem.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(nameof(ipg_claimlineitem.ipg_ExpectedReimbursement).ToLower()),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(nameof(ipg_claimlineitem.ipg_claimid).ToLower(), ConditionOperator.Equal, claimLineItemEnt.ipg_claimid.Id)
                        }
                    }
                };
                if (context.MessageName == MessageNames.Delete)
                    queryExpression.Criteria.Conditions.Add(new ConditionExpression(nameof(ipg_claimlineitem.ipg_claimlineitemId).ToLower(), ConditionOperator.NotEqual, claimLineItemEnt.Id));
                EntityCollection claimLineItems = service.RetrieveMultiple(queryExpression);
                decimal expectedReimbursement = claimLineItems.Entities.Sum(claimLineItem => (claimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_ExpectedReimbursement).ToLower()) == null ? 0 : claimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_ExpectedReimbursement).ToLower()).Value));

                Invoice invoice = new Invoice();
                invoice.Id = claimLineItemEnt.ipg_claimid.Id;
                invoice.ipg_ExpectedReimbursement = new Money(expectedReimbursement);
                service.Update(invoice);
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}