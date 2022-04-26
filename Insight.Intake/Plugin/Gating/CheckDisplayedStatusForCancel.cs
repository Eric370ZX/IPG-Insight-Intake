using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckDisplayedStatusForCancel : GatingPluginBase
    {
        public CheckDisplayedStatusForCancel() : base("ipg_IPGGatingCheckDisplayedStatusForCancel") { }

        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx)
        {
            var referral = crmService.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet("ipg_casestatusdisplayedid")).ToEntity<ipg_referral>();
            var displayedStatus = referral.ipg_casestatusdisplayedid?.Name;
            if (!string.IsNullOrEmpty(displayedStatus) && !displayedStatus.ToLower().Contains("cancel"))
            {
                return new GatingResponse(true);
            }
            return new GatingResponse(false);
        }
    }
}
