using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Insight.Intake.Plugin.Payment
{
    public class ProcessClaimLineTems : PluginBase
    {

        public ProcessClaimLineTems() : base(typeof(ProcessClaimLineTems))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGIntakeActionProcessClaimLineItems", null, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            EntityReference claimRef = (EntityReference)context.InputParameters["Claim"];
            EntityReference headerRef = (EntityReference)context.InputParameters["Header"];
            var header = service.Retrieve(headerRef.LogicalName, headerRef.Id, new ColumnSet(nameof(ipg_claimresponseheader.ipg_claimresponseheaderId).ToLower(), nameof(ipg_claimresponseheader.ipg_ClaimResponseBatchId).ToLower())).ToEntity<ipg_claimresponseheader>();
            var batch = service.Retrieve(header.ipg_ClaimResponseBatchId.LogicalName, header.ipg_ClaimResponseBatchId.Id, new ColumnSet(nameof(ipg_claimresponsebatch.ipg_IsManualBatch).ToLower())).ToEntity<ipg_claimresponsebatch>();

            UpdateClaimLineItems(service, header, claimRef, batch.ipg_IsManualBatch);

            context.OutputParameters["Succeeded"] = true;
        }

        private void UpdateClaimLineItems(IOrganizationService service, Entity header, EntityReference claimRef, bool? manualBatch)
        {
            QueryExpression queryExpression = new QueryExpression(ipg_claimresponseline.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(ipg_claimresponseline.ipg_claimresponselineId).ToLower(), nameof(ipg_claimresponseline.ipg_AdjProc).ToLower(), nameof(ipg_claimresponseline.ipg_AllowedActual_new).ToLower(), nameof(ipg_claimresponseline.ipg_AmountPaid_new).ToLower(), nameof(ipg_claimresponseline.ipg_AmountSubmitted_new).ToLower(), nameof(ipg_claimresponseline.ipg_PaidUnits).ToLower(), nameof(ipg_claimresponseline.ipg_DenialCodeString).ToLower(), nameof(ipg_claimresponseline.ipg_RemarkCodeString).ToLower()),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(ipg_claimresponseline.ipg_ClaimResponseHeaderId).ToLower(), ConditionOperator.Equal, header.GetAttributeValue<Guid>(nameof(ipg_claimresponseheader.ipg_claimresponseheaderId).ToLower()))
                        }
                }
            };
            EntityCollection claimResponseLines = service.RetrieveMultiple(queryExpression);

            queryExpression = new QueryExpression(ipg_claimlineitem.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(ipg_claimlineitem.ipg_claimlineitemId).ToLower(), nameof(ipg_claimlineitem.ipg_hcpcsid).ToLower(), nameof(ipg_claimlineitem.ipg_ExpectedReimbursement).ToLower(), nameof(ipg_claimlineitem.ipg_allowed).ToLower(), nameof(ipg_claimlineitem.ipg_paid).ToLower(), nameof(ipg_claimlineitem.ipg_claimlineitemId).ToLower(), nameof(ipg_claimlineitem.ipg_billedchg).ToLower(), nameof(ipg_claimlineitem.ipg_quantity).ToLower()),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(ipg_claimlineitem.ipg_claimid).ToLower(), ConditionOperator.Equal, claimRef.Id)
                        }
                }
            };
            EntityCollection claimLineItems = service.RetrieveMultiple(queryExpression);

            List<Guid> claimLineItemsList = new List<Guid>();
            foreach (Entity claimReponseLine in claimResponseLines.Entities)
            {
                IEnumerable<Entity> claimLineItemsHCPCS = claimLineItems.Entities.Where(claimLineItem => claimLineItem.GetAttributeValue<EntityReference>(nameof(ipg_claimlineitem.ipg_hcpcsid).ToLower()).Name.ToLower() == claimReponseLine.GetAttributeValue<string>(nameof(ipg_claimresponseline.ipg_AdjProc).ToLower()).ToLower());
                if (claimLineItemsHCPCS.Count() == 1)
                {
                    Entity corrClaimLineItem = claimLineItemsHCPCS.FirstOrDefault();
                    UpdateClaimLineItem(service, corrClaimLineItem, claimReponseLine, manualBatch);
                    continue;
                }
                else if (claimLineItemsHCPCS.Count() > 1)
                {
                    claimLineItemsHCPCS = claimLineItems.Entities.Where(claimLineItem => (claimLineItem.GetAttributeValue<EntityReference>(nameof(ipg_claimlineitem.ipg_hcpcsid).ToLower()).Name.ToLower() == claimReponseLine.GetAttributeValue<string>(nameof(ipg_claimresponseline.ipg_AdjProc).ToLower()).ToLower()) && (MoneyToDecimal(claimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_billedchg).ToLower())) == (claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountSubmitted_new).ToLower()) == null ? 0 : claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountSubmitted_new).ToLower()).Value)));
                    if (claimLineItemsHCPCS.Count() == 1)
                    {
                        Entity corrClaimLineItem = claimLineItemsHCPCS.FirstOrDefault();
                        UpdateClaimLineItem(service, corrClaimLineItem, claimReponseLine, manualBatch);
                        continue;
                    }
                }

                claimLineItemsHCPCS = claimLineItems.Entities.Where(claimLineItem => (claimLineItem.GetAttributeValue<decimal>(nameof(ipg_claimlineitem.ipg_quantity).ToLower()) == claimReponseLine.GetAttributeValue<int>(nameof(ipg_claimresponseline.ipg_PaidUnits).ToLower())) && (MoneyToDecimal(claimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_billedchg).ToLower())) == (claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountSubmitted_new).ToLower()) == null ? 0 : claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountSubmitted_new).ToLower()).Value)));
                if (claimLineItemsHCPCS.Count() == 1)
                {
                    Entity corrClaimLineItem = claimLineItemsHCPCS.FirstOrDefault();
                    UpdateClaimLineItem(service, corrClaimLineItem, claimReponseLine, manualBatch);
                }
                else if (claimLineItemsHCPCS.Count() > 1)
                {
                    foreach (Entity claimLineItemEntity in claimLineItemsHCPCS)
                    {
                        if (claimLineItemsList.FindIndex(claimLineItem => claimLineItem.Equals(claimLineItemEntity.GetAttributeValue<Guid>(nameof(ipg_claimlineitem.ipg_claimlineitemId).ToLower()))) < 0)
                        {
                            UpdateClaimLineItem(service, claimLineItemEntity, claimReponseLine, manualBatch);
                            claimLineItemsList.Add(claimLineItemEntity.GetAttributeValue<Guid>(nameof(ipg_claimlineitem.ipg_claimlineitemId).ToLower()));
                            break;
                        }
                    }
                }
            }
        }

        private void UpdateClaimLineItem(IOrganizationService service, Entity corrClaimLineItem, Entity claimReponseLine, bool? manualBatch)
        {
            ipg_claimlineitem cli = new ipg_claimlineitem();
            cli.Id = corrClaimLineItem.Id;
            if (manualBatch ?? false)
            {
                cli.ipg_AllowedTemp = new Money(claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AllowedActual_new).ToLower()) == null ? 0 : claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AllowedActual_new).ToLower()).Value);
                cli.ipg_PaidTemp = new Money(claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountPaid_new).ToLower()) == null ? 0 : claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountPaid_new).ToLower()).Value);
            }
            else
            {
                cli.ipg_AllowedTemp = new Money((corrClaimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_allowed).ToLower()) == null ? 0 : corrClaimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_allowed).ToLower()).Value) + (claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AllowedActual_new).ToLower()) == null ? 0 : claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AllowedActual_new).ToLower()).Value));
                cli.ipg_PaidTemp = new Money((corrClaimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_paid).ToLower()) == null ? 0 : corrClaimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_paid).ToLower()).Value) + (claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountPaid_new).ToLower()) == null ? 0 : claimReponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountPaid_new).ToLower()).Value));
            }
            cli.ipg_DenialCodeString = claimReponseLine.GetAttributeValue<string>(nameof(ipg_claimresponseline.ipg_DenialCodeString).ToLower());
            cli.ipg_RemarkCodeString = claimReponseLine.GetAttributeValue<string>(nameof(ipg_claimresponseline.ipg_RemarkCodeString).ToLower());
            service.Update(cli);
        }

        private decimal MoneyToDecimal(Money money)
        {
            return (money == null ? 0 : money.Value);
        }
    }
}