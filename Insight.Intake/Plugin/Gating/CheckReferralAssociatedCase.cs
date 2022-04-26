using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Insight.Intake.Plugin.Gating
{
    [Obsolete("This validation was replaced with method in CheckReferralForm class")]
    public class CheckReferralAssociatedCase : GatingPluginBase
    {
        public CheckReferralAssociatedCase() : base("ipg_IPGGatingCheckReferralAssociatedCase") { }

        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx)
        {
            return new GatingResponse(true);
        }
    }
}
