using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckActualProcedureDate : GatingPluginBase
    {
        public CheckActualProcedureDate() : base("ipg_IPGGatingCheckActualProcedureDate")
        {
        }
        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx)
        {
            var caseEntity = crmService.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet(Incident.Fields.ipg_ActualDOS)).ToEntity<Incident>();
            return (caseEntity.ipg_ActualDOS != null)
                ? new GatingResponse(true)
                : new GatingResponse(false);
        }
    }
}
