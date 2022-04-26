using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Payment
{
    public class CreatePaymentNote : PluginBase
    {

        public CreatePaymentNote() : base(typeof(UpdateCaseFields))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_payment.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                var payment = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_payment>();

                string applyTo = "";
                var claimResponseHeader = GetClaimResponceHeader(payment.ipg_ClaimResponseHeader.Id, service);

                if (claimResponseHeader.ipg_PaymentFrom == null)
                {
                    var claimResponseBatch = service.Retrieve(ipg_claimresponsebatch.EntityLogicalName, claimResponseHeader.ipg_ClaimResponseBatchId.Id, new ColumnSet(nameof(ipg_claimresponsebatch.ipg_PaymentFrom).ToLower())).ToEntity<ipg_claimresponsebatch>();
                    if (claimResponseBatch.ipg_PaymentFrom != null)
                    {
                        applyTo = (claimResponseBatch.ipg_PaymentFrom.Value == (int)ipg_PaymentFrom.Carrier ? "Carrier" : "Patient");
                    }
                }
                else
                {
                    applyTo = (claimResponseHeader.ipg_PaymentFrom.Value == (int)ipg_PaymentFrom.Carrier ? "Carrier" : "Patient");
                }
                string paymentType = "";
                decimal paymentAmount = 0;
                int type = (claimResponseHeader.ipg_PaymentType == null ? 0 : ((OptionSetValue)claimResponseHeader.ipg_PaymentType).Value);
                if (claimResponseHeader.ipg_PaymentType != null)
                {
                    switch (type)
                    {
                        case (int)ipg_ClaimResponseHeaderType.Payment:
                        case 0:
                            paymentType = "Payment";
                            paymentAmount = GetPaymentAmount(service, payment.ipg_ClaimResponseHeader);
                            break;
                        case (int)ipg_ClaimResponseHeaderType.Interest:
                            paymentType = "Interest";
                            paymentAmount = payment.ipg_InterestPayment.Value;
                            break;
                        case (int)ipg_ClaimResponseHeaderType.Refund:
                            paymentType = "Refund";
                            decimal? amount = claimResponseHeader.ipg_RefundType ?? false ? -payment?.ipg_MemberPaid_new?.Value : Convert.ToDecimal(-payment?.ipg_TotalInsurancePaid);
                            paymentAmount = amount.HasValue ? amount.Value: 0;
                            break;
                    }

                    var annotation = new Annotation();
                    annotation.ObjectId = payment.ipg_CaseId;
                    annotation.ObjectTypeCode = payment.ipg_CaseId.LogicalName;
                    if (type == (int)ipg_ClaimResponseHeaderType.Refund)
                    {
                        annotation.Subject = "Refund";
                        annotation.NoteText = "Refund of " + string.Format("{0:C}", paymentAmount) + " applied on Check # " + claimResponseHeader.ipg_CheckNumber + ". Notes: " + claimResponseHeader.ipg_PaymentNotes;
                    }
                    else
                    {
                        var paymentNote = payment.ipg_Note ?? payment.ipg_Notes;
                        annotation.Subject = "Payment";
                        annotation.NoteText = applyTo + " " + paymentType + " of " + string.Format("{0:C}", paymentAmount) + " applied." + (String.IsNullOrWhiteSpace(paymentNote) ? "" : "Notes: " + paymentNote);
                    }
                    service.Create(annotation);
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
        private ipg_claimresponseheader GetClaimResponceHeader(Guid id, IOrganizationService service)
        {
            return service.Retrieve(ipg_claimresponseheader.EntityLogicalName, id,
                    new ColumnSet(nameof(ipg_claimresponseheader.ipg_PaymentFrom).ToLower(),
                    nameof(ipg_claimresponseheader.ipg_ClaimResponseBatchId).ToLower(),
                    nameof(ipg_claimresponseheader.ipg_PaymentType).ToLower(),
                    nameof(ipg_claimresponseheader.ipg_PaymentNotes).ToLower(),
                    nameof(ipg_claimresponseheader.ipg_CheckNumber).ToLower(),
                    nameof(ipg_claimresponseheader.ipg_RefundType).ToLower())).ToEntity<ipg_claimresponseheader>();
        }
        private decimal GetPaymentAmount(IOrganizationService service, EntityReference header)
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
            return claimResponseLines.Entities.Sum(claimResponseLine => (claimResponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountPaid_new).ToLower()) == null ? 0 : claimResponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountPaid_new).ToLower()).Value));
        }
    }
}