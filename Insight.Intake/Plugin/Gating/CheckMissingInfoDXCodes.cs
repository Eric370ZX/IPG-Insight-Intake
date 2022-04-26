using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckMissingInfoDXCodes : GatingPluginBase
    {
        public CheckMissingInfoDXCodes() : base("ipg_IPGGatingCheckMissingInfoDXCodes") { }
        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx = null)
        {
            var targetRef = ctx.TargetRef();
            var columns = new[] { 
                Incident.Fields.ipg_DxCodeId1,  
                Incident.Fields.ipg_DxCodeId2, 
                Incident.Fields.ipg_DxCodeId3,
                Incident.Fields.ipg_DxCodeId4, 
                Incident.Fields.ipg_DxCodeId5, 
                Incident.Fields.ipg_DxCodeId6 };

            var target = ctx.SystemOrganizationService.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet(columns));
            var hasAtLeastOneCode = target.Attributes.Any(p => columns.Contains(p.Key) && p.Value != null);

            return new GatingResponse(hasAtLeastOneCode);
        }
    }
}
