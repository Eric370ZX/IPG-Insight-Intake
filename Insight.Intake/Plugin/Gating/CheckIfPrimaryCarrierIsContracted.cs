using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckIfPrimaryCarrierIsContracted : GatingPluginBase
    {
        public CheckIfPrimaryCarrierIsContracted() : base("ipg_IPGGatingCheckIfPrimaryCarrierIsContracted") { }

        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext context)
        {
            Guid? carrierId = null;
            if (targetRef.LogicalName == Incident.EntityLogicalName)
            {
                var caseEn = crmService.Retrieve(targetRef.LogicalName, targetRef.Id,
                 new ColumnSet(Incident.Fields.ipg_CarrierId, Incident.Fields.ipg_SecondaryCarrierId))
                        .ToEntity<Incident>();
                carrierId = caseEn.ipg_CarrierId?.Id;
            }
            else if (targetRef.LogicalName == ipg_referral.EntityLogicalName)
            {
                var caseEn = crmService.Retrieve(targetRef.LogicalName, targetRef.Id,
                new ColumnSet(ipg_referral.Fields.ipg_CarrierId))
                       .ToEntity<ipg_referral>();
                carrierId = caseEn.ipg_CarrierId?.Id;
            }
            if (carrierId == null)
            {
                return new GatingResponse(false, "Carrier is null");
            }
            var carrier = crmService
                .Retrieve(Intake.Account.EntityLogicalName, carrierId.Value, new ColumnSet(Intake.Account.Fields.ipg_contract, Intake.Account.Fields.Name))
                .ToEntity<Intake.Account>();

            if (carrier.ipg_contract == false)
            {
                return new GatingResponse(false)
            {
                    TaskDescripton = $"Primary Carrier {carrier.Name} is not contracted with IPG. Please review to determine if this is a billable IPG Case."
                };
            }
            return new GatingResponse(true);
        }
    }
}
