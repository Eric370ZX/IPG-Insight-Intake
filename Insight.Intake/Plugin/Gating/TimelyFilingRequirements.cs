using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    public class TimelyFilingRequirements : GatingPluginBase
    {
        public TimelyFilingRequirements() : base("ipg_IPGGatingCheckTimelyFilingRequirements")
        {
        }

        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx = null)
        {
            DateTime? procedureDate = null;

            if (ctx.TargetRef().LogicalName == Incident.EntityLogicalName)
            {
                var incident = ctx.SystemOrganizationService
                    .Retrieve(ctx.TargetRef().LogicalName, ctx.TargetRef().Id, new ColumnSet(Incident.Fields.ipg_ProcedureDateNew))
                    .ToEntity<Incident>();
                procedureDate = incident.ipg_ProcedureDateNew;
            }
            else if (ctx.TargetRef().LogicalName == ipg_referral.EntityLogicalName)
            {
                var referral = ctx.SystemOrganizationService
                    .Retrieve(ctx.TargetRef().LogicalName, ctx.TargetRef().Id, new ColumnSet(ipg_referral.Fields.ipg_SurgeryDate))
                    .ToEntity<ipg_referral>();
                procedureDate = referral.ipg_SurgeryDate;
            }
            if (procedureDate == null)
            {
                return new GatingResponse(false, "Procedure date is null");
            }
            var procedureTimeSpan = (procedureDate.Value - DateTime.Now).TotalDays;

            var carrier = gateManager.GetCarrier();
            if (carrier.ipg_timelyfilingrule == null)
            {
                return new GatingResponse(false, $"Timely filling rull is null on the carrier {carrier.Id}");
            }

            var isIntoLimit = carrier.ipg_timelyfilingrule.Value >= procedureTimeSpan;
            return new GatingResponse(isIntoLimit);
        }
    }
}
