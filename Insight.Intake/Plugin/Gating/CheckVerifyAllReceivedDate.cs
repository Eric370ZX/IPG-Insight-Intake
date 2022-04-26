using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckVerifyAllReceivedDate : GatingPluginBase
    {
        public CheckVerifyAllReceivedDate() : base("ipg_IPGGatingCheckVerifyAllReceivedDate") { }

        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx = null)
        {
            var targetRef = ctx.TargetRef();
            if (targetRef.LogicalName == ipg_referral.EntityLogicalName)
            {
                return new GatingResponse(true, "Validation CheckVerifyAllReceivedDate can't be run on referral");
            }
            var targetIncident = ctx.SystemOrganizationService
                .Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet(Incident.Fields.ipg_isallreceiveddate))
                .ToEntity<Incident>();

            var isSucceeded = targetIncident.ipg_isallreceiveddate != null;
            return new GatingResponse(isSucceeded);
        }
    }
}
