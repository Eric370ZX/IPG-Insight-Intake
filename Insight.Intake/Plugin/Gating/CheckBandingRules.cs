using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckBandingRules : GatingPluginBase
    {
        public CheckBandingRules() : base("ipg_IPGGatingCheckBandingRules")
        {
        }
        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx)
        {
            var taskQuery = new QueryExpression(Task.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(Task.Fields.StateCode),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(Task.Fields.RegardingObjectId, ConditionOperator.Equal, targetRef.Id),
                        new ConditionExpression(Task.Fields.ipg_tasktypecode, ConditionOperator.Equal, (int)ipg_TaskType1.Openbandingworkflowtask),
                    }
                }
            };
            var tasks = crmService.RetrieveMultiple(taskQuery).Entities.Select(s => s.ToEntity<Task>());
            //if no tasks of OpenBanding rules, return positive result
            if (!tasks.Any())
            {
                return new GatingResponse
                {
                    Succeeded = true,
                    CustomMessage = "No Banding Rules encountered",
                };
            }
            if (tasks.Any(t => t.StateCode == TaskState.Completed))
            {
                return new GatingResponse
                {
                    Succeeded = true,
                    TaskSubject = "No Banding Rules encountered.",
                    CustomMessage = "No Banding Rules encountered."
                };
            }
            else
            {
                return new GatingResponse
                {
                    Succeeded = false,
                    TaskSubject = "Banding - Approval Required",
                    CustomMessage = "Banding Rules encountered.",
                    TaskDescripton = "At least one Banding rule was triggered for this Case. Please resolve the underlying issue or approve the Banding rule to resolve this issue."
                };
            }
        }
    }
}
