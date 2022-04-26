using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Insight.Intake.Helpers;
using System.Linq;
using Microsoft.Xrm.Sdk.Client;

namespace Insight.Intake.Plugin.Payment
{
    public class PreValidatePayment : PluginBase
    {
        private IOrganizationService service;
        private ITracingService tracingService;
        private ipg_payment payment;
        private List<string> messageList;

        public PreValidatePayment() : base(typeof(ProcessPayment))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGPaymentPreValidatePayment", null, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            service = localPluginContext.OrganizationService;
            tracingService = localPluginContext.TracingService;
            tracingService.Trace($"{typeof(PreValidatePayment)} plugin started");

            messageList = new List<string>();
            PaymentManager paymentManager = new PaymentManager(service, tracingService);
            var paymentRef = (EntityReference)context.InputParameters["Target"];
            payment = service.Retrieve(paymentRef.LogicalName, paymentRef.Id, new ColumnSet(true)).ToEntity<ipg_payment>();
            if (paymentManager.isManualBatch(payment))
            {
                int headerType = paymentManager.GetHeaderType(payment);
                decimal paymentAmount = paymentManager.GetPaymentAmount(payment);
                CheckBillToPatient(headerType, paymentAmount);
                ProcessHeaderType(headerType);
                context.OutputParameters["Message"] = string.Join("\n", messageList);
            }
        }

        private void CheckBillToPatient(int headerType, decimal paymentAmount)
        {
            if (headerType == (int)ipg_ClaimSubEvent.FullPatientResponsibility)
            {
                decimal adjustmentAmount = 0;
                Incident incident = service.Retrieve(payment.ipg_CaseId.LogicalName, payment.ipg_CaseId.Id, new ColumnSet(nameof(Incident.ipg_RemainingCarrierBalance).ToLower(), nameof(Incident.ipg_RemainingSecondaryCarrierBalance).ToLower(), nameof(Incident.ipg_RemainingPatientBalance).ToLower(), nameof(Incident.ipg_SecondaryCarrierId).ToLower(), nameof(Incident.ipg_BillToPatient).ToLower())).ToEntity<Incident>();
                if (payment.ipg_ApplyAdjustment ?? false)
                {
                    bool ipg_IsSecondaryCarrier = false;
                    if ((payment.ipg_Claim != null) && (incident.ipg_SecondaryCarrierId != null))
                    {
                        Invoice claim = service.Retrieve(Invoice.EntityLogicalName, payment.ipg_Claim.Id, new ColumnSet(nameof(Invoice.CustomerId).ToLower())).ToEntity<Invoice>();
                        ipg_IsSecondaryCarrier = (claim.CustomerId != null) && claim.CustomerId.Equals(incident.ipg_SecondaryCarrierId);
                    }

                    if ((payment.ipg_AdjustmentType.Value == (int)ipg_AdjustmentTypes.SalesAdjustment)
                        || (payment.ipg_AdjustmentType.Value == (int)ipg_AdjustmentTypes.WriteOff))
                    {
                        if (!ipg_IsSecondaryCarrier && payment.ipg_ApplyTo.Value == (int)ipg_PayerType.PrimaryCarrier)
                        {
                            adjustmentAmount = (payment.ipg_AmountToApply == null ? 0 : payment.ipg_AmountToApply.Value);
                        }
                    }
                    else if (payment.ipg_AdjustmentType.Value == (int)ipg_AdjustmentTypes.TransferofResponsibility)
                    {
                        if (!ipg_IsSecondaryCarrier && payment.ipg_From.Value == (int)ipg_PayerType.PrimaryCarrier)
                        {
                            adjustmentAmount = (payment.ipg_AmountToApply == null ? 0 : payment.ipg_AmountToApply.Value);
                        }
                    }
                }

                decimal amount = (incident.GetAttributeValue<Money>(nameof(Incident.ipg_RemainingCarrierBalance).ToLower()) == null ? 0 : incident.GetAttributeValue<Money>(nameof(Incident.ipg_RemainingCarrierBalance).ToLower()).Value) - paymentAmount - adjustmentAmount;

                if (amount > 0)
                {
                    if (headerType == (int)ipg_ClaimSubEvent.FullPatientResponsibility && (incident.ipg_BillToPatient != null) && (incident.ipg_BillToPatient.Value == (int)ipg_TwoOptions.No))
                    {
                        messageList.Add("A payment for the Case has been received and posted. Remaining balance cannot be transferred to Patient due to “Bill to Patient” being set to “No”. Please review and adjust manually.");
                    }
                }
            }
        }

        private void ProcessHeaderType(int headerType)
        {
            var headerRef = payment.ipg_ClaimResponseHeader;
            if (headerRef == null)
            {
                tracingService.Trace("The payment contains an empty header");
                return;
            }

            Dictionary<int, string> mapping = new Dictionary<int, string>();
            mapping.Add((int)ipg_ClaimSubEvent.PartialDenialFromCarrier, "Carrier Collections - Partial Denial");
            mapping.Add((int)ipg_ClaimSubEvent.FullDenialFromCarrier, "Carrier Collections - Full Denial");
            mapping.Add((int)ipg_ClaimSubEvent.PatientResponsibilitybuthasSecondary, "Create Secondary Claim");
            mapping.Add((int)ipg_ClaimSubEvent.Recoupment, "Carrier Collections - Recoupment follow-up");

            if (mapping.ContainsKey(headerType))
            {
                messageList.Add(mapping[headerType]);
            }
        }
    }
}