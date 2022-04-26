using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Payment
{
    public class RoundBalance : PluginBase
    {
        public RoundBalance() : base(typeof(RoundBalance))
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

                Incident incident = service.Retrieve(payment.ipg_CaseId.LogicalName, payment.ipg_CaseId.Id, new ColumnSet(nameof(Incident.ipg_RemainingCarrierBalance).ToLower(), nameof(Incident.ipg_RemainingSecondaryCarrierBalance).ToLower(), nameof(Incident.ipg_RemainingPatientBalance).ToLower())).ToEntity<Incident>();

                if((incident.ipg_RemainingCarrierBalance != null) && (((double)incident.ipg_RemainingCarrierBalance.Value != 0) && (double)incident.ipg_RemainingCarrierBalance.Value <= 0.99) && ((double)incident.ipg_RemainingCarrierBalance.Value >= -0.99))
                {
                    CreateAdjustment(service, payment.ipg_CaseId, (int)ipg_PayerType.PrimaryCarrier, incident.ipg_RemainingCarrierBalance.Value);
                }

                if ((incident.ipg_RemainingSecondaryCarrierBalance != null) && (((double)incident.ipg_RemainingSecondaryCarrierBalance.Value != 0) && (double)incident.ipg_RemainingSecondaryCarrierBalance.Value <= 0.99) && ((double)incident.ipg_RemainingSecondaryCarrierBalance.Value >= -0.99))
                {
                    CreateAdjustment(service, payment.ipg_CaseId, (int)ipg_PayerType.SecondaryCarrier, incident.ipg_RemainingSecondaryCarrierBalance.Value);
                }

                if ((incident.ipg_RemainingPatientBalance != null) && (((double)incident.ipg_RemainingPatientBalance.Value != 0) && (double)incident.ipg_RemainingPatientBalance.Value <= 0.99) && ((double)incident.ipg_RemainingPatientBalance.Value >= -0.99))
                {
                    CreateAdjustment(service, payment.ipg_CaseId, (int)ipg_PayerType.Patient, incident.ipg_RemainingPatientBalance.Value);
                }

            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }

        private void CreateAdjustment(IOrganizationService service, EntityReference caseRef, int applyTo, decimal amount)
        {
            ipg_adjustment adjustment = new ipg_adjustment()
            {
                ipg_CaseId = caseRef,
                ipg_ApplyTo = new OptionSetValue(applyTo),
                ipg_AdjustmentType = new OptionSetValue((int)ipg_AdjustmentTypes.WriteOff),
                ipg_AmountType = false,
                ipg_Percent = 100,
                ipg_AmountToApply = new Money(amount),
                ipg_SkipGatingExecution = true
            };
            service.Create(adjustment);
        }
    }
}
