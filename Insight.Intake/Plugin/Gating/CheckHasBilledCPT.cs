using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckHasBilledCPT : GatingPluginBase
    {
        public CheckHasBilledCPT() : base("ipg_IPGGatingCheckHasBilledCPT") { }

        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx)
        {
            // This validation should occur for incidents only
            if (ctx.TargetRef().LogicalName == ipg_referral.EntityLogicalName)
            {
                return new GatingResponse(true);
            }
            var incident = crmService.Retrieve(targetRef.LogicalName, targetRef.Id,
                    new ColumnSet(Incident.Fields.ipg_BilledCPTId))
                    .ToEntity<Incident>();
            var hasBilledCpt = incident.ipg_BilledCPTId != null;

            return new GatingResponse(hasBilledCpt);
}
    }
}
