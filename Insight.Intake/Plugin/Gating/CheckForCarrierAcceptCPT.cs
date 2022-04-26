using Insight.Intake.Extensions;
using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Gating
{
    [Obsolete("This validation was replaced with method in CheckReferralForm class")]
    public class CheckForCarrierAcceptCPT : GatingPluginBase
    {
        public CheckForCarrierAcceptCPT() : base("ipg_IPGGatingCheckForCarrierAcceptCPT") { }

        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx)
        {
             return new GatingResponse(true);
        }
    }
}
