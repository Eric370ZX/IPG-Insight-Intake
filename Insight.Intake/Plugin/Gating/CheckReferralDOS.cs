using Insight.Intake.Extensions;
using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckReferralDOS : GatingPluginBase
    {
        public CheckReferralDOS() : base("ipg_IPGGatingCheckReferralDOS") { }

        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx)
        {
            var referral = crmService.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet("ipg_surgerydate")).ToEntity<ipg_referral>();
            if (referral.GetCaseDos().HasValue && (referral.GetCaseDos().Value >= DateTime.Now.AddDays(-180)))
            {
                return new GatingResponse(true);
            }
            return new GatingResponse(false);
        }
    }
}
