using Insight.Intake.Plugin.Gating.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    [Obsolete("This validation doesn't used anymore")]
    public class CheckPropelNasalStentOnCPT : GatingPluginBase
    {
        public CheckPropelNasalStentOnCPT() : base("ipg_IPGGatingCheckPropelNasalStentOnCPT") { }
        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx) => gateManager.CheckPropelNasalStentOnCPT();
    }
}