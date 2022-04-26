using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Case
{
    public class ValidateCarrierChange : PluginBase
    {
        public ValidateCarrierChange() : base(typeof(ValidateCarrierChange))
        {
            RegisterEvent(PipelineStages.PreValidation, MessageNames.Update, Incident.EntityLogicalName, PreOperationHandler);
        }
        private void PreOperationHandler(LocalPluginContext localPluginContext)
        {
            var target = localPluginContext.Target<Incident>();
            var preImage = localPluginContext.PreImage<Incident>();

            if(target != null)
            {
                if(target.ipg_CarrierId != null 
                    && preImage != null && preImage.ipg_CarrierId != null
                    && target.ipg_CarrierId.Id != preImage.ipg_CarrierId.Id)
                {
                    var carrierBeforeChange = localPluginContext.OrganizationService.Retrieve(Intake.Account.EntityLogicalName, preImage.ipg_CarrierId.Id, new ColumnSet(Intake.Account.Fields.ipg_CarrierType)).ToEntity<Intake.Account>();
                    var carrierAfterChange = localPluginContext.OrganizationService.Retrieve(Intake.Account.EntityLogicalName, target.ipg_CarrierId.Id, new ColumnSet(Intake.Account.Fields.ipg_CarrierType)).ToEntity<Intake.Account>();

                    if (carrierBeforeChange.ipg_CarrierTypeEnum == ipg_CarrierType.Auto
                        && carrierAfterChange.ipg_CarrierTypeEnum == ipg_CarrierType.WorkersComp)
                    {
                        throw new System.Exception("Workers Comp carrier cannot replace Auto carrier");
                    }
                }

                if (target.ipg_SecondaryCarrierId != null)
                {
                    var secondaryCarrier = localPluginContext.OrganizationService.Retrieve(Intake.Account.EntityLogicalName, target.ipg_SecondaryCarrierId.Id, new ColumnSet(Intake.Account.Fields.ipg_CarrierType)).ToEntity<Intake.Account>();
                    if (secondaryCarrier != null && secondaryCarrier.ipg_CarrierTypeEnum != null
                        && secondaryCarrier.ipg_CarrierTypeEnum == ipg_CarrierType.WorkersComp)
                    {
                        throw new InvalidPluginExecutionException("Workers Comp Carrier cannot be set as a carrier #2");
                    }

                    if(target.ipg_CarrierId != null)
                    {
                        var carrier1 = localPluginContext.OrganizationService.Retrieve(Intake.Account.EntityLogicalName, target.ipg_CarrierId.Id,
                            new ColumnSet(Intake.Account.Fields.ipg_CarrierType)).ToEntity<Intake.Account>();
                        if(carrier1?.ipg_CarrierTypeEnum == ipg_CarrierType.WorkersComp)
                        {
                            throw new InvalidPluginExecutionException("Workers Comp Carrier is already set as the first carrier");
                        }
                    }
                }

            }
        }
    }
}
