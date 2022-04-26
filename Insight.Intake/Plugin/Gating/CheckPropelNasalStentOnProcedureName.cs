using Insight.Intake.Plugin.Gating.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    [Obsolete("This validation doesn't used anymore")]
    public class CheckPropelNasalStentOnProcedureName : GatingPluginBase
    {
        public CheckPropelNasalStentOnProcedureName() : base("ipg_IPGGatingCheckPropelNasalStentOnProcedureName") { }
        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx) => gateManager.CheckPropelNasalStentOnProcedureName();
    }
}