using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckMissingInfoCPTCodes : GatingPluginBase
    {
        public CheckMissingInfoCPTCodes() : base("ipg_IPGGatingCheckMissingInfoCPTCodes") { }
        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx = null)
        {
            var targetRef = ctx.TargetRef();
            var columns = new[] { 
                Incident.Fields.ipg_CPTCodeId1, 
                Incident.Fields.ipg_CPTCodeId2, 
                Incident.Fields.ipg_CPTCodeId3,
                Incident.Fields.ipg_CPTCodeId4, 
                Incident.Fields.ipg_CPTCodeId5, 
                Incident.Fields.ipg_CPTCodeId6 };

            var target = ctx.SystemOrganizationService.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet(columns));
            var hasAtLeastOneCptCode = target.Attributes.Any(p => columns.Contains(p.Key) && p.Value != null);

            return new GatingResponse(hasAtLeastOneCptCode);
        }
    }
}
