using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckAutoCarrier : GatingPluginBase
    {
        public CheckAutoCarrier() : base("ipg_IPGGatingCheckAutoCarrier") { }

        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx)
        {
            Guid? carrierId = null;
            Guid? secondaryCarrierId = null;
            if (targetRef.LogicalName == Incident.EntityLogicalName)
            {
                var caseEn = crmService.Retrieve(targetRef.LogicalName, targetRef.Id,
                 new ColumnSet(Incident.Fields.ipg_CarrierId, Incident.Fields.ipg_SecondaryCarrierId))
                        .ToEntity<Incident>();
                carrierId = caseEn.ipg_CarrierId?.Id;
                secondaryCarrierId = caseEn.ipg_SecondaryCarrierId?.Id;
            }
            else if (targetRef.LogicalName == ipg_referral.EntityLogicalName)
            {
                var caseEn = crmService.Retrieve(targetRef.LogicalName, targetRef.Id,
                new ColumnSet(ipg_referral.Fields.ipg_CarrierId))
                       .ToEntity<ipg_referral>();
                carrierId = caseEn.ipg_CarrierId?.Id;
            }
            else
            {
                return new GatingResponse(true);
            }
            if (carrierId != null)
            {
                var result = HandleCarrier((Guid)carrierId);
                if (result != null)
                {
                    return result;
                }
            }
            if (secondaryCarrierId != null)
            {
                var result = HandleCarrier((Guid)secondaryCarrierId);
                if (result != null)
                {
                    return result;
                }
            }

            return new GatingResponse(true);
        }
        private GatingResponse HandleCarrier(Guid carrierId)
        {
            var carrier = crmService.Retrieve(Intake.Account.EntityLogicalName, carrierId,
                    new ColumnSet(Intake.Account.Fields.ipg_CarrierType, Intake.Account.Fields.Name))
                    .ToEntity<Intake.Account>();
            if (carrier.ipg_CarrierType?.Value == (int)ipg_CarrierType.Auto)
            {
                return new GatingResponse(false)
                {
                    CodeOutput = (int)AutoCarrier_Output.AutoCarrierFound,
                    TaskDescripton = $"Auto Carrier {carrier.Name} is listed on this Case. Please verify benefits and confirm this is a billable IPG Case."
                };

            }
            if (carrier.ipg_CarrierType?.Value == (int)ipg_CarrierType.WorkersComp)
            {
                return new GatingResponse(false)
                {
                    CodeOutput = (int)AutoCarrier_Output.WCCarrierFound
                };
            }
            return null;
        }
        enum AutoCarrier_Output
        {
            AutoCarrierFound = 1,
            WCCarrierFound = 2,
        }
    }
}
