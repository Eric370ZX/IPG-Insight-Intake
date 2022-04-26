using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;

namespace Insight.Intake.Plugin.Payment
{
    public class UpdateCaseFields : PluginBase
    {

        public UpdateCaseFields() : base(typeof(UpdateCaseFields))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_payment.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_payment.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Delete, ipg_payment.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                ipg_payment paymentRef = null;
                if (context.InputParameters["Target"] is Entity)
                {
                    var payment = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_payment>();
                    paymentRef = service.Retrieve(payment.LogicalName, payment.Id, new ColumnSet(nameof(ipg_payment.ipg_CaseId).ToLower())).ToEntity<ipg_payment>();
                }
                else
                {
                    var payment = ((EntityReference)context.InputParameters["Target"]);
                    paymentRef = service.Retrieve(payment.LogicalName, payment.Id, new ColumnSet(nameof(ipg_payment.ipg_CaseId).ToLower())).ToEntity<ipg_payment>();
                }

                var queryExpression = new QueryExpression(ipg_payment.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(nameof(ipg_payment.ipg_TotalInsurancePaid).ToLower(), nameof(ipg_payment.ipg_MemberPaid_new).ToLower(), nameof(ipg_payment.ipg_ar_carriersalesadj).ToLower(), nameof(ipg_payment.ipg_ar_carrieradj).ToLower(), nameof(ipg_payment.ipg_ar_membersalesadj).ToLower(), nameof(ipg_payment.ipg_ar_memberadj).ToLower(), nameof(ipg_payment.ipg_PaymentAmount).ToLower(), nameof(ipg_payment.ipg_WriteOffamount).ToLower(), nameof(ipg_payment.ipg_PaymentType).ToLower()),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(nameof(ipg_payment.ipg_CaseId).ToLower(), ConditionOperator.Equal, paymentRef.ipg_CaseId.Id),
                            new ConditionExpression(nameof(ipg_payment.StateCode).ToLower(), ConditionOperator.Equal, 0) //active
                        }
                    }
                };
                if (context.MessageName == MessageNames.Delete)
                    queryExpression.Criteria.Conditions.Add(new ConditionExpression(nameof(ipg_payment.ipg_paymentId).ToLower(), ConditionOperator.NotEqual, paymentRef.Id));
                EntityCollection payments = service.RetrieveMultiple(queryExpression);
                decimal carrierSum = 0;
                decimal secondaryCarrierSum = 0;
                decimal memberSum = 0;
                decimal amountPaid = 0;
                decimal committedAmount = 0;
                List<string> list = new List<string>();
                foreach (Entity entity in payments.Entities)
                {
                    if (IsSecondaryCarrierPayment(service, entity.ToEntityReference()))
                    {
                        secondaryCarrierSum += (decimal)entity.GetAttributeValue<double>(nameof(ipg_payment.ipg_TotalInsurancePaid).ToLower());
                    }
                    else
                    {
                        carrierSum += (decimal)entity.GetAttributeValue<double>(nameof(ipg_payment.ipg_TotalInsurancePaid).ToLower());
                    }
                    memberSum += (entity.GetAttributeValue<Money>(nameof(ipg_payment.ipg_MemberPaid_new).ToLower()) == null ? 0 : entity.GetAttributeValue<Money>(nameof(ipg_payment.ipg_MemberPaid_new).ToLower()).Value);
                    if ((entity.GetAttributeValue<OptionSetValue>(nameof(ipg_payment.ipg_PaymentType).ToLower()) == null) || (entity.GetAttributeValue<OptionSetValue>(nameof(ipg_payment.ipg_PaymentType).ToLower()).Value == (int)ipg_PaymentType.Actual))
                    {
                        amountPaid += (entity.GetAttributeValue<Money>(nameof(ipg_payment.ipg_PaymentAmount).ToLower()) == null ? 0 : entity.GetAttributeValue<Money>(nameof(ipg_payment.ipg_PaymentAmount).ToLower()).Value) -
                           (decimal)(entity.GetAttributeValue<double>(nameof(ipg_payment.ipg_WriteOffamount).ToLower()));
                    }
                    else if (entity.GetAttributeValue<int>(nameof(ipg_payment.ipg_PaymentType).ToLower()) == (int)ipg_PaymentType.Committed)
                    {
                        committedAmount += (entity.GetAttributeValue<Money>(nameof(ipg_payment.ipg_PaymentAmount).ToLower()) == null ? 0 : entity.GetAttributeValue<Money>(nameof(ipg_payment.ipg_PaymentAmount).ToLower()).Value);
                    }
                }

                Incident incident = new Incident();
                incident.Id = paymentRef.ipg_CaseId.Id;
                incident.ipg_TotalReceivedfromCarrier = new Money(carrierSum);
                incident.ipg_TotalReceivedfromSecondaryCarrier = new Money(secondaryCarrierSum);
                incident.ipg_TotalReceivedFromPatient = new Money(memberSum);
                incident.ipg_AmountPaid = new Money(amountPaid);
                incident.ipg_CommittedAmount = new Money(committedAmount);
                service.Update(incident);
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