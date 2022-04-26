using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckHighPriorityTasks : GatingPluginBase
    {
        public CheckHighPriorityTasks() : base("ipg_IPGGatingCheckHighPriorityTasks") { }

        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext context)
        {
            var unclosedTasksWithHighPriority = RetrieveOpenTasks();
            return new GatingResponse(!unclosedTasksWithHighPriority.Entities.Any());
        }

        private EntityCollection RetrieveOpenTasks()
        {
            var query = new QueryExpression {
                EntityName = Task.EntityLogicalName,
                Criteria = {
                    Conditions = {
                        new ConditionExpression(Task.Fields.RegardingObjectId, ConditionOperator.Equal, targetRef.Id),
                        new ConditionExpression(Task.Fields.ipg_priority, ConditionOperator.Equal, (int)ipg_Priority.High),
                        new ConditionExpression(Task.Fields.StateCode, ConditionOperator.Equal, (int)TaskState.Open),
                    }
                }
            };
            return crmService.RetrieveMultiple(query);
        }
    }
}
