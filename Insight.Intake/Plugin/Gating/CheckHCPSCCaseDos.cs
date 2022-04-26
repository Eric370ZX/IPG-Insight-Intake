using Insight.Intake.Extensions;
using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckHCPSCCaseDos : GatingPluginBase
    {
        public CheckHCPSCCaseDos() : base("ipg_IPGGatingCheckHCPSCCaseDos") { }

        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx)
        {
            var caseDos = crmService.Retrieve(targetRef.LogicalName, targetRef.Id,
             new ColumnSet("ipg_actualdos", "ipg_surgerydate")).GetCaseDos();
            if (caseDos == null)
            {
                return new GatingResponse(true, "Case dos is empty");
            }

            var actualPartsQuery = new QueryExpression(ipg_casepartdetail.EntityLogicalName)
            {
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression("ipg_caseid", ConditionOperator.Equal, targetRef.Id)
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(ipg_casepartdetail.EntityLogicalName, ipg_masterhcpcs.EntityLogicalName, 
                    "ipg_hcpcscode", "ipg_masterhcpcsid", JoinOperator.Inner)
                    {
                        EntityAlias = "m",
                        LinkCriteria =
                        {
                            FilterOperator = LogicalOperator.Or,
                            Conditions =
                            {
                                new ConditionExpression("ipg_effectivedate", ConditionOperator.GreaterThan, caseDos.Value),
                                new ConditionExpression("ipg_expirationdate", ConditionOperator.LessThan, caseDos.Value)
                            }
                        }
                    }
                }
            };
            if (crmService.RetrieveMultiple(actualPartsQuery).Entities.Any())
            {
                return new GatingResponse(false, "At least one HCPCS code is expired for the DOS.");
            }
            return new GatingResponse(true);
        }
    }
}
