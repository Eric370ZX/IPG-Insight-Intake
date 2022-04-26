using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.TaskEntity
{
    public class RouteTaskPlugin : PluginBase
    {
        public RouteTaskPlugin() : base(typeof(RouteTaskPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Intake.Incident.EntityLogicalName, Route);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Intake.ipg_referral.EntityLogicalName, Route);
        }

        private void Route(LocalPluginContext localPluginContext)
        {
            var tracing = localPluginContext.TracingService;
            var service = localPluginContext.OrganizationService;

            tracing.Trace($"{nameof(Route)} started");

            var postImage = localPluginContext.PostImage<Entity>();
            var preImage = localPluginContext.PreImage<Entity>();

            var owner = postImage.GetAttributeValue<EntityReference>(Intake.Incident.Fields.OwnerId);
            var preOwner = preImage.GetAttributeValue<EntityReference>(Intake.Incident.Fields.OwnerId);

            var assignedToTeam = postImage?.GetAttributeValue<EntityReference>(Intake.Incident.Fields.ipg_assignedtoteamid);


            
            if(owner?.Id != preOwner?.Id && owner?.LogicalName == SystemUser.EntityLogicalName && assignedToTeam != null)
            {
                localPluginContext.TracingService.Trace($"{postImage.LogicalName} ({postImage.Id}) Reassigned to {owner.Id}");

                var tasks = service.RetrieveMultiple(new QueryExpression(Task.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(false),
                    Criteria = new FilterExpression()
                    {
                        Conditions =
                        {
                            new ConditionExpression(Task.Fields.StateCode, ConditionOperator.Equal, (int)TaskState.Open),
                            new ConditionExpression(Task.Fields.RegardingObjectId, ConditionOperator.Equal, postImage.Id),
                            new ConditionExpression(Task.Fields.ipg_assignedtoteamid, ConditionOperator.Equal, assignedToTeam.Id),
                            new ConditionExpression(Task.Fields.OwnerId, ConditionOperator.NotEqual, owner.Id)
                        }
                    }
                }).Entities;

                localPluginContext.TracingService.Trace($"Found {tasks.Count} Task to be reassigned");

                foreach (var task in tasks)
                {
                    service.Update(new Task() { Id = task.Id, OwnerId = owner });
                }
            }

            localPluginContext.TracingService.Trace($"{nameof(Route)} Done");
        }
    }
}
