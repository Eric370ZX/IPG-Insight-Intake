using Insight.Intake.Plugin.Gating.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    [Obsolete("This validation was replaced with method in CheckReferralForm class")]
    public class CheckPatientPrimaryInfo : GatingPluginBase
    {
        public CheckPatientPrimaryInfo():base("ipg_IPGGatingCheckPatientPrimaryInfo")
        {
        }
        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx)
        {
            return new GatingResponse(true);
        }
    }
}
