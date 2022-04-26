using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Payment
{
    public class PaymentManager
    {
        private readonly IOrganizationService service;
        private readonly ITracingService tracingService;

        public PaymentManager(IOrganizationService organizationService, ITracingService tracingService)
        {
            this.service = organizationService;
            this.tracingService = tracingService;
        }

        public int GetHeaderType(ipg_payment payment)
        {
            tracingService.Trace("Get Header Type");
            int headerType = -1;
            var headerRef = payment.ipg_ClaimResponseHeader;
            var claimRef = payment.ipg_Claim;
            if (headerRef == null || claimRef == null)
            {
                tracingService.Trace("The payment contains an empty header or claim");
            }
            else
            {
                QueryExpression queryExpression = new QueryExpression(ipg_claimresponseline.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(nameof(ipg_claimresponseline.ipg_claimresponselineId).ToLower(), nameof(ipg_claimresponseline.ipg_AdjProc).ToLower(), nameof(ipg_claimresponseline.ipg_AllowedActual_new).ToLower(), nameof(ipg_claimresponseline.ipg_AmountPaid_new).ToLower(), nameof(ipg_claimresponseline.ipg_AmountSubmitted_new).ToLower()),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(nameof(ipg_claimresponseline.ipg_ClaimResponseHeaderId).ToLower(), ConditionOperator.Equal, headerRef.Id)
                        }
                    }
                };

                EntityCollection claimResponseLines = service.RetrieveMultiple(queryExpression);
                if (claimResponseLines.Entities.Count == 0)
                {
                    tracingService.Trace("No claim response lines");
                    return 0; //unrecognized header type
                }

                decimal allowed = claimResponseLines.Entities.Sum(claimResponseLine => (claimResponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AllowedActual_new).ToLower()) == null ? 0 : claimResponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AllowedActual_new).ToLower()).Value));
                decimal paid = claimResponseLines.Entities.Sum(claimResponseLine => (claimResponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountPaid_new).ToLower()) == null ? 0 : claimResponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountPaid_new).ToLower()).Value));
                decimal submitted = claimResponseLines.Entities.Sum(claimResponseLine => (claimResponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountSubmitted_new).ToLower()) == null ? 0 : claimResponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountSubmitted_new).ToLower()).Value));

                queryExpression = new QueryExpression(ipg_claimlineitem.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(nameof(ipg_claimlineitem.ipg_claimlineitemId).ToLower(), nameof(ipg_claimlineitem.ipg_hcpcsid).ToLower(), nameof(ipg_claimlineitem.ipg_ExpectedReimbursement).ToLower(), nameof(ipg_claimlineitem.ipg_paid).ToLower()),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(nameof(ipg_claimlineitem.ipg_claimid).ToLower(), ConditionOperator.Equal, claimRef.Id)
                        }
                    }
                };

                EntityCollection claimLineItems = service.RetrieveMultiple(queryExpression);
                if (claimLineItems.Entities.Count == 0)
                {
                    tracingService.Trace("No claim line items");
                    return 0; //unrecognized header type
                }

                if (claimLineItems.Entities.Count != claimResponseLines.Entities.Count)
                {
                    tracingService.Trace("The number of claim line items is not equal to the number of claim response lines");
                    return 0; //unrecognized header type
                }

                decimal expectedReimbursement = claimLineItems.Entities.Sum(claimLineItem => (claimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_ExpectedReimbursement).ToLower()) == null ? 0 : claimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_ExpectedReimbursement).ToLower()).Value));
                Entity claim = service.Retrieve(claimRef.LogicalName, claimRef.Id, new ColumnSet("ipg_paid"));
                //decimal prevPaid = claimLineItems.Entities.Sum(claimLineItem => (claimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_paid).ToLower()) == null ? 0 : claimLineItem.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_paid).ToLower()).Value));
                decimal prevPaid = (claim.GetAttributeValue<Money>("ipg_paid") == null ? 0 : claim.GetAttributeValue<Money>("ipg_paid").Value);

                if (submitted < 0)
                {
                    headerType = (int)ipg_ClaimSubEvent.Recoupment;
                }
                else if ((expectedReimbursement == allowed) && (allowed == (paid + prevPaid)))
                {
                    headerType = (int)ipg_ClaimSubEvent.Fullpayment;
                }
                else if ((expectedReimbursement > allowed) && ((paid + prevPaid) > 0))
                {
                    headerType = (int)ipg_ClaimSubEvent.PartialDenialFromCarrier;
                }
                else if ((expectedReimbursement == allowed) && ((paid + prevPaid) == 0))
                {
                    headerType = (int)ipg_ClaimSubEvent.FullPatientResponsibility;
                }
                else if ((expectedReimbursement > 0) && (allowed == 0) && ((paid + prevPaid) == 0))
                {
                    headerType = (int)ipg_ClaimSubEvent.FullDenialFromCarrier;
                }
                else if ((expectedReimbursement == allowed) && ((paid + prevPaid) > 0))
                {
                    if (HasSecondary(payment.ipg_CaseId) && !IsSecondaryCarrierPayment(payment))
                    {
                        headerType = (int)ipg_ClaimSubEvent.PatientResponsibilitybuthasSecondary;
                    }
                    else
                    {
                        headerType = (int)ipg_ClaimSubEvent.Partialpatientresponsibility;
                    }
                }
                else if (expectedReimbursement < allowed)
                {
                    headerType = (int)ipg_ClaimSubEvent.CarrierOverpayment;
                }
            }
            tracingService.Trace($"Header type=" + headerType.ToString());
            return headerType;
        }

        public bool HasSecondary(EntityReference caseRef)
        {
            Incident caseEnt = service.Retrieve(caseRef.LogicalName, caseRef.Id, new ColumnSet(nameof(Incident.ipg_SecondaryCarrierId).ToLower())).ToEntity<Incident>();
            if (caseEnt.ipg_SecondaryCarrierId != null)
            {
                var queryExpression = new QueryExpression(Invoice.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(nameof(Invoice.InvoiceId).ToLower()),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(nameof(Invoice.ipg_caseid).ToLower(), ConditionOperator.Equal, caseRef.Id),
                            new ConditionExpression(nameof(Invoice.ipg_isprimaryorsecondaryclaim).ToLower(), ConditionOperator.Equal, false)
                        }
                    }
                };
                EntityCollection claims = service.RetrieveMultiple(queryExpression);
                return (claims.Entities.Count == 0);
            }
            return false;
        }

        public decimal GetPaymentAmount(ipg_payment payment)
        {
            tracingService.Trace("Get Payment Amount");
            var headerRef = payment.ipg_ClaimResponseHeader;
            if (headerRef == null)
            {
                return 0;
            }
            else
            {
                QueryExpression queryExpression = new QueryExpression(ipg_claimresponseline.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(nameof(ipg_claimresponseline.ipg_AmountPaid_new).ToLower()),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(nameof(ipg_claimresponseline.ipg_ClaimResponseHeaderId).ToLower(), ConditionOperator.Equal, headerRef.Id)
                        }
                    }
                };
                EntityCollection claimResponseLines = service.RetrieveMultiple(queryExpression);
                return claimResponseLines.Entities.Sum(claimResponseLine => (claimResponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountPaid_new).ToLower()) == null ? 0 : claimResponseLine.GetAttributeValue<Money>(nameof(ipg_claimresponseline.ipg_AmountPaid_new).ToLower()).Value));
            }
        }

        public bool isManualBatch(ipg_payment payment)
        {
            var headerRef = payment.ipg_ClaimResponseHeader;
            if(headerRef == null)
            {
                return false;
            }
            var header = service.Retrieve(headerRef.LogicalName, headerRef.Id, new ColumnSet(nameof(ipg_claimresponseheader.ipg_ClaimResponseBatchId).ToLower())).ToEntity<ipg_claimresponseheader>();
            var batchRef = header.GetAttributeValue<EntityReference>(nameof(ipg_claimresponseheader.ipg_ClaimResponseBatchId).ToLower());
            var batch = service.Retrieve(batchRef.LogicalName, batchRef.Id, new ColumnSet(nameof(ipg_claimresponsebatch.ipg_IsManualBatch).ToLower())).ToEntity<ipg_claimresponsebatch>();
            return batch.GetAttributeValue<bool>(nameof(ipg_claimresponsebatch.ipg_IsManualBatch).ToLower());
        }

        public bool isPostedByCarrier(ipg_payment payment)
        {
            //Carrier Payment can be with null insirance paid - transfer to patient balance
            return payment.ipg_MemberPaid_new == null || payment.ipg_MemberPaid_new.Value <= 0;
        }

        public bool IsAutoCarrierPayment(ipg_payment payment)
        {
            bool result = false;
            Incident incident = service.Retrieve(payment.ipg_CaseId.LogicalName, payment.ipg_CaseId.Id, new ColumnSet(nameof(Incident.ipg_SecondaryCarrierId).ToLower())).ToEntity<Incident>();
            if ((payment.ipg_Claim != null) && (incident.ipg_SecondaryCarrierId != null))
            {
                Invoice claim = service.Retrieve(Invoice.EntityLogicalName, payment.ipg_Claim.Id, new ColumnSet(nameof(Invoice.CustomerId).ToLower())).ToEntity<Invoice>();
                Intake.Account carrier = service.Retrieve(Intake.Account.EntityLogicalName, incident.ipg_SecondaryCarrierId.Id, new ColumnSet(nameof(Intake.Account.ipg_CarrierType).ToLower())).ToEntity<Intake.Account>();
                result = (claim.CustomerId != null) && claim.CustomerId.Equals(incident.ipg_SecondaryCarrierId) && (carrier.ipg_CarrierType?.Value == (int)ipg_CarrierType.Auto);
            }
            return result;
        }

        internal bool IsSecondaryCarrierPayment(ipg_payment payment)
        {
            bool result = false;
            var incident = service.Retrieve(payment.ipg_CaseId.LogicalName, payment.ipg_CaseId.Id, new ColumnSet(nameof(Incident.ipg_SecondaryCarrierId).ToLower())).ToEntity<Incident>();
            if ((payment.ipg_Claim != null) && (incident.ipg_SecondaryCarrierId != null))
            {
                var claim = service.Retrieve(Invoice.EntityLogicalName, payment.ipg_Claim.Id, new ColumnSet(nameof(Invoice.CustomerId).ToLower())).ToEntity<Invoice>();
                result = (claim.CustomerId != null) && claim.CustomerId.Equals(incident.ipg_SecondaryCarrierId);
            }
            return result;
        }
    }
}
