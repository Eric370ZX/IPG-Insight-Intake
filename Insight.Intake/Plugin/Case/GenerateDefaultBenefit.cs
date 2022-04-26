using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace Insight.Intake.Plugin.Case
{
    public class GenerateDefaultBenefit : PluginBase
    {
        public GenerateDefaultBenefit() : base(typeof(GenerateDefaultBenefit))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Incident.EntityLogicalName, PreOperationCreateHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Incident.EntityLogicalName, PreOperationCreateHandler);
        }

        void PreOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;

            Incident target = localPluginContext.Target<Incident>();
            if(target == null)
            {
                tracingService.Trace("Target is empty. Exit");
                return;
            }

            EntityReference changedCarrierReference;
            if(target.ipg_CarrierId != null)
            {
                changedCarrierReference = target.ipg_CarrierId;
            }
            else if(target.ipg_SecondaryCarrierId != null)
            {
                changedCarrierReference = target.ipg_SecondaryCarrierId;
            }
            else
            {
                tracingService.Trace("Carriers have not changed. Exit");
                return;
            }

            var changedCarrier = service.Retrieve(Intake.Account.EntityLogicalName, changedCarrierReference.Id, new ColumnSet(
                Intake.Account.Fields.ipg_CarrierType
                )).ToEntity<Intake.Account>();
            if (changedCarrier.ipg_CarrierTypeEnum != ipg_CarrierType.Auto
                && changedCarrier.ipg_CarrierTypeEnum != ipg_CarrierType.WorkersComp)
            {
                tracingService.Trace("Carrier type is not Auto or WorkersComp. Exit");
                return;
            }
            
            ipg_BenefitType benefitType;
            switch (changedCarrier.ipg_CarrierTypeEnum.Value)
            {
                case ipg_CarrierType.Auto:
                    benefitType = ipg_BenefitType.Auto;
                    break;

                case ipg_CarrierType.WorkersComp:
                    benefitType = ipg_BenefitType.WorkersComp;
                    break;
                    
                default:
                    throw new InvalidPluginExecutionException("Unexpected carrier type: " + changedCarrier.ipg_CarrierTypeEnum);
            }

            using (var crmContext = new CrmServiceContext(service))
            {
                tracingService.Trace("Retrieving an existing benefit");

                var existingBenefit = crmContext.ipg_benefitSet.FirstOrDefault(b => 
                    b.ipg_CaseId != null && b.ipg_CaseId.Id == target.Id
                    && b.ipg_CarrierId != null && b.ipg_CarrierId.Id == changedCarrier.Id
                    && b.ipg_BenefitTypeEnum == benefitType
                    && b.ipg_InOutNetwork == false
                    && b.StateCode == ipg_benefitState.Active);

                if(existingBenefit == null)
                {
                    tracingService.Trace($"Creating a new {benefitType} benefit");

                    var newBenefit = new ipg_benefit()
                    {
                        ipg_CaseId = target.ToEntityReference(),
                        ipg_CarrierId = changedCarrierReference,
                        ipg_BenefitTypeEnum = benefitType,
                        ipg_InOutNetwork = false
                    };
                    SetBenefitProperties(newBenefit);
                    crmContext.AddObject(newBenefit);
                    crmContext.SaveChanges();
                }
            }
        }

        private void SetBenefitProperties(ipg_benefit benefit)
        {
            benefit.ipg_Deductible = new Money(0);
            benefit.ipg_DeductibleMet = new Money(0);
            benefit.ipg_MemberOOPMax = new Money(0);
            benefit.ipg_MemberOOPMet = new Money(0);
            benefit.ipg_CarrierCoinsurance = 100;
            benefit.ipg_MemberCoinsurance = 0;
        }
    }
}
