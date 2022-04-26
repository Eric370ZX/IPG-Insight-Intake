using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckCriticalFieldsLastChangeDate : GatingPluginBase
    {
        public CheckCriticalFieldsLastChangeDate() : base("ipg_IPGGatingCheckCriticalFieldsLastChangeDate") { }

        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx)
        {
            var initCase = crmService.Retrieve(targetRef.LogicalName, targetRef.Id,
                new ColumnSet(Incident.Fields.ipg_lastcalcrevon, Incident.Fields.ipg_criticalfieldslastchangedate))
                    .ToEntity<Incident>();

            if (initCase?.ipg_criticalfieldslastchangedate == null)
            {
                return new GatingResponse(false, "Critical fields last change date is empty",
                    "Critical fields last change date is empty",
                    GatingOutcomes.CriticalFieldsLastChangeDateIsEmpty);
            }

            if (initCase.ipg_lastcalcrevon > initCase.ipg_criticalfieldslastchangedate)
            {
                return new GatingResponse(true, "Most recent Calc Rev after the most recent critical field",
                    "Most recent Calc Rev after the most recent critical field",
                    GatingOutcomes.CalcRevUpdateNotRequired);
            }
            else
            {
                return new GatingResponse(false, "Most recent Calc Rev done before at least one critical field changed",
                    "Most recent Calc Rev done before at least one critical field changed",
                    GatingOutcomes.NewCalcRevRequired);
            }
        }
    }
}