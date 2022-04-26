using Insight.Intake.Plugin.Gating.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckPreAuthMemberId : GatingPluginBase
    {
        public CheckPreAuthMemberId() : base("ipg_IPGGatingCheckPreAuthMemberId") { }
        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx) => gateManager.CheckPreAuthMemberId();

    }
}
