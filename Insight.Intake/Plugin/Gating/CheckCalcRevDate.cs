using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckCalcRevDate : GatingPluginBase
    {
        public CheckCalcRevDate() : base("ipg_IPGGatingCheckCalcRevDate") { }

        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx)
        {
            var initCase = crmService.Retrieve(targetRef.LogicalName, targetRef.Id,
              new ColumnSet("ipg_lastcalcrevon"))
              .ToEntity<Incident>();
            if (initCase.ipg_lastcalcrevon == null)
            {
                return new GatingResponse(false, "Last calc rev date is empty");
            }

            var actualPartsQuery = new QueryExpression(ipg_casepartdetail.EntityLogicalName)
            {
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression("ipg_caseid", ConditionOperator.Equal, targetRef.Id),
                        new ConditionExpression("modifiedon", ConditionOperator.GreaterEqual, initCase.ipg_lastcalcrevon.Value)
                    }
                }
            };
            if (crmService.RetrieveMultiple(actualPartsQuery).Entities.Any())
            {
                return new GatingResponse(false, "Last calc rev date is earlier then actual parts last modified date");
            }
            return new GatingResponse(true);
        }
    }
}
