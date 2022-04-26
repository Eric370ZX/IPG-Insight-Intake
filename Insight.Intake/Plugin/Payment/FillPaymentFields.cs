using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Insight.Intake.Helpers;
using System.Linq;

namespace Insight.Intake.Plugin.Payment
{
    public class FillPaymentFields : PluginBase
    {

        public FillPaymentFields() : base(typeof(FillPaymentFields))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, ipg_payment.EntityLogicalName, PreOperationCreateHandler);
        }

        private void PreOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                var payment = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_payment>();

                Incident caseEnt = service.Retrieve(payment.ipg_CaseId.LogicalName, payment.ipg_CaseId.Id, new ColumnSet(nameof(Incident.ipg_ActualCarrierResponsibility).ToLower(), nameof(Incident.ipg_collectiondate).ToLower(), nameof(Incident.ipg_RemainingCarrierBalance).ToLower())).ToEntity<Incident>();
                if (caseEnt.GetAttributeValue<DateTime>(nameof(Incident.ipg_collectiondate).ToLower()) != null)
                {
                    payment.ipg_PaymentAge = (DateTime.Now - caseEnt.GetAttributeValue<DateTime>(nameof(Incident.ipg_collectiondate).ToLower())).Days;
                }
                payment.ipg_PaymentDate = DateTime.Now;
                payment.ipg_PaymentType = new OptionSetValue((int)ipg_PaymentType.Actual);

                if (payment.ipg_ClaimResponseHeader == null)
                {
                    return;
                }

                ipg_claimresponseheader header = service.Retrieve(ipg_claimresponseheader.EntityLogicalName, payment.ipg_ClaimResponseHeader.Id, new ColumnSet(nameof(ipg_claimresponseheader.ipg_PaymentFrom).ToLower(), nameof(ipg_claimresponseheader.ipg_ClaimResponseBatchId).ToLower(), nameof(ipg_claimresponseheader.ipg_PaymentType).ToLower())).ToEntity<ipg_claimresponseheader>();
                double paymentAmount = payment.ipg_TotalInsurancePaid ?? 0;
                if (header.ipg_PaymentType.Value != (int)ipg_ClaimResponseHeaderType.Refund)
                {
                    paymentAmount = GetPaymentAmount(service, payment.ipg_ClaimResponseHeader);
                    if ((header.ipg_PaymentFrom != null) && (header.ipg_PaymentFrom.Value == (int)ipg_PaymentFrom.Patient))
                    {
                        payment.ipg_TotalInsurancePaid = 0;
                    }
                    else
                    {
                        payment.ipg_TotalInsurancePaid = paymentAmount;
                    }
                    if (header.ipg_ClaimResponseBatchId != null)
                    {
                        ipg_claimresponsebatch batch = service.Retrieve(ipg_claimresponsebatch.EntityLogicalName, header.ipg_ClaimResponseBatchId.Id, new ColumnSet(nameof(ipg_claimresponsebatch.ipg_CheckNumber).ToLower())).ToEntity<ipg_claimresponsebatch>();
                        payment.ipg_CheckNumber = batch.ipg_CheckNumber;
                    }
                }
                payment.ipg_PaymentAmount = new Money((decimal)(payment.ipg_TotalInsurancePaid == null ? 0 : payment.ipg_TotalInsurancePaid) + (payment.ipg_MemberPaid_new == null ? 0 : payment.ipg_MemberPaid_new.Value) + (decimal)(payment.ipg_WriteOffamount == null ? 0 : payment.ipg_WriteOffamount));
                payment.ipg_name = "Payment: $" + payment.ipg_PaymentAmount.Value.ToString("0.00");
                payment.ipg_InterestAdjustment = (payment.ipg_InterestPayment == null ? 0 : -payment.ipg_InterestPayment.Value);
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }

        private double GetPaymentAmount(IOrganizationService service, EntityReference header)
        {
            QueryExpression queryExpression = new QueryExpression(ipg_claimresponseline.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(ipg_claimresponseline.ipg_AmountPaid_new).ToLower()),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(ipg_claimresponseline.ipg_ClaimResponseHeaderId).ToLower(), ConditionOperator.Equal, header.Id)
                        }
                }
            };
            EntityCollection claimResponseLines = service.RetrieveMultiple(queryExpression);
            return (double)claimResponseLines.Entities.Sum(claimResponseLine => (claimResponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountPaid_new).ToLower()) == null ? 0 : claimResponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountPaid_new).ToLower()).Value));
        }
    }
}