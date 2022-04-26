using Insight.Intake.Extensions;
using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    public class ValidCarrierPairedAutoCarrier : GatingPluginBase
    {
        public ValidCarrierPairedAutoCarrier() : base("ipg_IPGGatingValidCarrierPairedAutoCarrier") { }
        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx)
        {
            if (ctx.TargetRef() == null || ctx.TargetRef()?.LogicalName != Incident.EntityLogicalName)
            {
                return new GatingResponse(false, "ValidCarrierPairedAutoCarrier can be run in case only");
            }
            var sourceIncident = gateManager.GetCase(ctx.TargetRef().Id, new[] { Incident.Fields.ipg_SecondaryCarrierId, Incident.Fields.ipg_AutoBenefitsExhausted });
            if (sourceIncident.ipg_SecondaryCarrierId == null)
            {
                return new GatingResponse(true);
            }
            var secondaryCarrier = ctx.SystemOrganizationService.Retrieve(Intake.Account.EntityLogicalName, sourceIncident.ipg_SecondaryCarrierId.Id, new ColumnSet(Intake.Account.Fields.ipg_CarrierType)).ToEntity<Intake.Account>();

            var primaryCarrier = gateManager.GetCarrier();
            if (primaryCarrier == null)
            {
                return new GatingResponse(false, "Unable to determine primary carrier");
            }
            var BCBSFLCarrierName = D365Helpers.GetGlobalSettingValueByKey(ctx.SystemOrganizationService, "ValidCarrierPairedAutoCarrier_carrierName");
            if (secondaryCarrier.ipg_CarrierTypeEnum == ipg_CarrierType.Auto && sourceIncident.ipg_AutoBenefitsExhausted != true && primaryCarrier.Name?.ToLower() != BCBSFLCarrierName.ToLower())
            {
                return new GatingResponse(false);
            }
            return new GatingResponse(true);
        }
    }
}

