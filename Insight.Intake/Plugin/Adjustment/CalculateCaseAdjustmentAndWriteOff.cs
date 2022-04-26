using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;

namespace Insight.Intake.Plugin.Adjustment
{
    public class CalculateCaseAdjustmentAndWriteOff : PluginBase
    {

        public CalculateCaseAdjustmentAndWriteOff() : base(typeof(CalculateCaseAdjustmentAndWriteOff))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_adjustment.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_adjustment.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Delete, ipg_adjustment.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                ipg_adjustment adjustment = null;
                if (context.InputParameters["Target"] is Entity)
                {
                    var target = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_adjustment>();
                    adjustment = service.Retrieve(target.LogicalName, target.Id, new ColumnSet(nameof(ipg_adjustment.ipg_CaseId).ToLower(), nameof(ipg_adjustment.ipg_AdjustmentType).ToLower(), nameof(ipg_adjustment.ipg_ApplyTo).ToLower(), nameof(ipg_adjustment.ipg_AmountToApply).ToLower(), nameof(ipg_adjustment.ipg_TransferofPaymentType).ToLower(), nameof(ipg_adjustment.ipg_Payment).ToLower(), nameof(ipg_adjustment.ipg_ToCase).ToLower(), nameof(ipg_adjustment.ipg_From).ToLower(), nameof(ipg_adjustment.ipg_To).ToLower())).ToEntity<ipg_adjustment>();
                }
                else
                {
                    var target = ((EntityReference)context.InputParameters["Target"]);
                    adjustment = service.Retrieve(target.LogicalName, target.Id, new ColumnSet(nameof(ipg_adjustment.ipg_CaseId).ToLower(), nameof(ipg_adjustment.ipg_AdjustmentType).ToLower(), nameof(ipg_adjustment.ipg_ApplyTo).ToLower(), nameof(ipg_adjustment.ipg_AmountToApply).ToLower(), nameof(ipg_adjustment.ipg_TransferofPaymentType).ToLower(), nameof(ipg_adjustment.ipg_Payment).ToLower(), nameof(ipg_adjustment.ipg_ToCase).ToLower(), nameof(ipg_adjustment.ipg_From).ToLower(), nameof(ipg_adjustment.ipg_To).ToLower())).ToEntity<ipg_adjustment>();
                }

                decimal carrierAdjSum = 0;
                decimal secondaryCarrierAdjSum = 0;
                decimal memberAdjSum = 0;
                decimal carrierWOSum = 0;
                decimal secondaryCarrierWOSum = 0;
                decimal memberWOSum = 0;
                EntityReference toCase = null;

                switch (adjustment.ipg_AdjustmentType.Value)
                {
                    case (int)ipg_AdjustmentTypes.WriteOff:
                        switch(adjustment.ipg_ApplyTo.Value)
                        {
                            case (int)ipg_PayerType.PrimaryCarrier:
                                carrierAdjSum += adjustment.ipg_AmountToApply.Value;
                                carrierWOSum += adjustment.ipg_AmountToApply.Value;
                                break;
                            case (int)ipg_PayerType.SecondaryCarrier:
                                secondaryCarrierAdjSum += adjustment.ipg_AmountToApply.Value;
                                secondaryCarrierWOSum += adjustment.ipg_AmountToApply.Value;
                                break;
                            case (int)ipg_PayerType.Patient:
                                memberAdjSum += adjustment.ipg_AmountToApply.Value;
                                memberWOSum += adjustment.ipg_AmountToApply.Value;
                                break;
                        }
                        break;
                    case (int)ipg_AdjustmentTypes.SalesAdjustment:
                        switch (adjustment.ipg_ApplyTo.Value)
                        {
                            case (int)ipg_PayerType.PrimaryCarrier:
                                carrierAdjSum += adjustment.ipg_AmountToApply.Value;
                                break;
                            case (int)ipg_PayerType.SecondaryCarrier:
                                secondaryCarrierAdjSum += adjustment.ipg_AmountToApply.Value;
                                break;
                            case (int)ipg_PayerType.Patient:
                                memberAdjSum += adjustment.ipg_AmountToApply.Value;
                                break;
                        }
                        break;
                    case (int)ipg_AdjustmentTypes.TransferofPayment:
                        switch (adjustment.ipg_TransferofPaymentType)
                        {
                            case false:
                                ipg_payment payment = service.Retrieve(ipg_payment.EntityLogicalName, adjustment.ipg_Payment.Id, new ColumnSet(nameof(ipg_payment.ipg_TotalInsurancePaid).ToLower(), nameof(ipg_payment.ipg_InterestPayment).ToLower(), nameof(ipg_payment.ipg_MemberPaid_new).ToLower())).ToEntity<ipg_payment>();
                                bool isSecondaryCarrierPayment = IsSecondaryCarrierPayment(service, payment.ToEntityReference());
                                decimal? payorCash = (decimal)payment.GetAttributeValue<double>(nameof(ipg_payment.ipg_TotalInsurancePaid).ToLower());
                                decimal? memberPaid = (payment.GetAttributeValue<Money>(nameof(ipg_payment.ipg_MemberPaid_new).ToLower()) == null ? 0 : payment.GetAttributeValue<Money>(nameof(ipg_payment.ipg_MemberPaid_new).ToLower()).Value);
                                toCase = adjustment.ipg_ToCase;
                                if (payorCash.HasValue && payorCash != 0)
                                {
                                    if(isSecondaryCarrierPayment)
                                    {
                                        secondaryCarrierAdjSum -= (payorCash ?? 0);
                                    }
                                    else
                                    {
                                        carrierAdjSum -= (payorCash ?? 0);
                                    }
                                }
                                if (memberPaid.HasValue && memberPaid != 0)
                                {
                                    memberAdjSum -= (payorCash ?? 0);
                                }
                                break;
                            case true:
                                switch (adjustment.ipg_From.Value)
                                {
                                    case (int)ipg_PayerType.PrimaryCarrier:
                                        carrierAdjSum -= adjustment.ipg_AmountToApply.Value;
                                        break;
                                    case (int)ipg_PayerType.SecondaryCarrier:
                                        secondaryCarrierAdjSum -= adjustment.ipg_AmountToApply.Value;
                                        break;
                                    case (int)ipg_PayerType.Patient:
                                        memberAdjSum -= adjustment.ipg_AmountToApply.Value;
                                        break;
                                }
                                switch (adjustment.ipg_To.Value)
                                {
                                    case (int)ipg_PayerType.PrimaryCarrier:
                                        carrierAdjSum += adjustment.ipg_AmountToApply.Value;
                                        break;
                                    case (int)ipg_PayerType.SecondaryCarrier:
                                        secondaryCarrierAdjSum += adjustment.ipg_AmountToApply.Value;
                                        break;
                                    case (int)ipg_PayerType.Patient:
                                        memberAdjSum += adjustment.ipg_AmountToApply.Value;
                                        break;
                                }
                                break;
                        }
                        break;
                    case (int)ipg_AdjustmentTypes.TransferofResponsibility:
                        switch (adjustment.ipg_From.Value)
                        {
                            case (int)ipg_PayerType.PrimaryCarrier:
                                carrierAdjSum -= adjustment.ipg_AmountToApply.Value;
                                break;
                            case (int)ipg_PayerType.SecondaryCarrier:
                                secondaryCarrierAdjSum -= adjustment.ipg_AmountToApply.Value;
                                break;
                            case (int)ipg_PayerType.Patient:
                                memberAdjSum -= adjustment.ipg_AmountToApply.Value;
                                break;
                        }
                        switch (adjustment.ipg_To.Value)
                        {
                            case (int)ipg_PayerType.PrimaryCarrier:
                                carrierAdjSum += adjustment.ipg_AmountToApply.Value;
                                break;
                            case (int)ipg_PayerType.SecondaryCarrier:
                                secondaryCarrierAdjSum += adjustment.ipg_AmountToApply.Value;
                                break;
                            case (int)ipg_PayerType.Patient:
                                memberAdjSum += adjustment.ipg_AmountToApply.Value;
                                break;
                        }
                        break;
                }

                if (context.MessageName == MessageNames.Delete)
                {
                    carrierAdjSum = -carrierAdjSum;
                    secondaryCarrierAdjSum = -secondaryCarrierAdjSum;
                    memberAdjSum = -memberAdjSum;
                    carrierWOSum = -carrierWOSum;
                    secondaryCarrierWOSum = -secondaryCarrierWOSum;
                    memberWOSum = -memberWOSum;
                }

                Incident incident = service.Retrieve(Incident.EntityLogicalName, adjustment.ipg_CaseId.Id, new ColumnSet(nameof(Incident.ipg_TotalCarrierRespAdjustments).ToLower(), nameof(Incident.ipg_TotalSecondaryCarrierRespAdjustments).ToLower(), nameof(Incident.ipg_TotalPatientRespAdjustments).ToLower(), nameof(Incident.ipg_TotalCarrierWriteoff).ToLower(), nameof(Incident.ipg_TotalSecondaryCarrierWriteoff).ToLower(), nameof(Incident.ipg_TotalPatientWriteoff).ToLower())).ToEntity<Incident>();
                incident.ipg_TotalCarrierRespAdjustments = new Money(carrierAdjSum + (incident.ipg_TotalCarrierRespAdjustments != null ? incident.ipg_TotalCarrierRespAdjustments.Value : 0));
                incident.ipg_TotalSecondaryCarrierRespAdjustments = new Money(secondaryCarrierAdjSum + (incident.ipg_TotalSecondaryCarrierRespAdjustments != null ? incident.ipg_TotalSecondaryCarrierRespAdjustments.Value : 0));
                incident.ipg_TotalPatientRespAdjustments = new Money(memberAdjSum + (incident.ipg_TotalPatientRespAdjustments != null ? incident.ipg_TotalPatientRespAdjustments.Value : 0));
                incident.ipg_TotalCarrierWriteoff = new Money(carrierWOSum + (incident.ipg_TotalCarrierWriteoff != null ? incident.ipg_TotalCarrierWriteoff.Value : 0));
                incident.ipg_TotalSecondaryCarrierWriteoff = new Money(secondaryCarrierWOSum + (incident.ipg_TotalSecondaryCarrierWriteoff != null ? incident.ipg_TotalSecondaryCarrierWriteoff.Value : 0));
                incident.ipg_TotalPatientWriteoff = new Money(memberWOSum + (incident.ipg_TotalPatientWriteoff != null ? incident.ipg_TotalPatientWriteoff.Value : 0));
                service.Update(incident);

                if(toCase != null)
                {
                    Incident toIncident = service.Retrieve(Incident.EntityLogicalName, toCase.Id, new ColumnSet(nameof(Incident.ipg_TotalCarrierRespAdjustments).ToLower(), nameof(Incident.ipg_TotalSecondaryCarrierRespAdjustments).ToLower(), nameof(Incident.ipg_TotalPatientRespAdjustments).ToLower())).ToEntity<Incident>();
                    toIncident.ipg_TotalCarrierRespAdjustments = new Money(-carrierAdjSum + (toIncident.ipg_TotalCarrierRespAdjustments != null ? toIncident.ipg_TotalCarrierRespAdjustments.Value : 0));
                    toIncident.ipg_TotalSecondaryCarrierRespAdjustments = new Money(-secondaryCarrierAdjSum + (toIncident.ipg_TotalSecondaryCarrierRespAdjustments != null ? toIncident.ipg_TotalSecondaryCarrierRespAdjustments.Value : 0));
                    toIncident.ipg_TotalPatientRespAdjustments = new Money(-memberAdjSum + (toIncident.ipg_TotalPatientRespAdjustments != null ? toIncident.ipg_TotalPatientRespAdjustments.Value : 0));
                    service.Update(toIncident);
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }

        private bool IsSecondaryCarrierPayment(IOrganizationService orgService, EntityReference paymentRef)
        {
            ipg_payment payment = orgService.Retrieve(paymentRef.LogicalName, paymentRef.Id, new ColumnSet(nameof(ipg_payment.ipg_Claim).ToLower(), nameof(ipg_payment.ipg_CaseId).ToLower())).ToEntity<ipg_payment>();
            EntityReference claimRef = payment.GetAttributeValue<EntityReference>(nameof(ipg_payment.ipg_Claim).ToLower());
            EntityReference caseRef = payment.GetAttributeValue<EntityReference>(nameof(ipg_payment.ipg_CaseId).ToLower());
            if (claimRef != null && caseRef != null)
            {
                Incident incident = orgService.Retrieve(caseRef.LogicalName, caseRef.Id, new ColumnSet(nameof(Incident.ipg_SecondaryCarrier).ToLower(), nameof(Incident.ipg_SecondaryCarrierId).ToLower())).ToEntity<Incident>();
                if (incident.GetAttributeValue<bool>(nameof(Incident.ipg_SecondaryCarrier).ToLower()) && (incident.GetAttributeValue<EntityReference>(nameof(Incident.ipg_SecondaryCarrierId).ToLower()) != null))
                {
                    Invoice claim = orgService.Retrieve(claimRef.LogicalName, claimRef.Id, new ColumnSet(nameof(Invoice.CustomerId).ToLower())).ToEntity<Invoice>();
                    return Guid.Equals(incident.GetAttributeValue<EntityReference>(nameof(Incident.ipg_SecondaryCarrierId).ToLower()).Id, claim.GetAttributeValue<EntityReference>(nameof(Invoice.CustomerId).ToLower()).Id);
                }
            }
            return false;
        }

    }
}