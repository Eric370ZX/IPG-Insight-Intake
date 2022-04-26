using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.QueueItem
{
    public class RouteQueueItemPlugin : PluginBase
    {
        const int DefaultThreshHold = 4;

        public RouteQueueItemPlugin() : base(typeof(RouteQueueItemPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Intake.QueueItem.EntityLogicalName, RouteItem);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Intake.QueueItem.EntityLogicalName, RouteItem);
        }

        private void RouteItem(LocalPluginContext localContext)
        {
            var target = localContext.Target<Intake.QueueItem>();
            var crmService = localContext.OrganizationService;
            var tracingService = localContext.TracingService;
            target = crmService.Retrieve(target.LogicalName, target.Id, new ColumnSet(true)).ToEntity<Intake.QueueItem>();

            tracingService.Trace($"{nameof(RouteItem)} started");

            tracingService.Trace($"workerid : {target.WorkerId?.Id} targetItem: {target.ObjectId?.LogicalName} QueueID = {target.QueueId.Id} QueueItem id: {target.Id}");
            if (target.WorkerId == null 
                && target.ObjectId?.LogicalName == Incident.EntityLogicalName 
                && target.QueueId != null)
            {
                var queue = crmService.Retrieve(target.QueueId.LogicalName, target.QueueId.Id, new ColumnSet("ownerid", "name")).ToEntity<Queue>();

                tracingService.Trace($"Check queue owner is team");

                tracingService.Trace($"queue owner is  {queue.OwnerId.LogicalName} with id {queue.OwnerId.Id}");
                if (queue.OwnerId.LogicalName == Team.EntityLogicalName)
                {
                    var caseEnt = crmService.Retrieve(target.ObjectId.LogicalName, target.ObjectId.Id, new ColumnSet(nameof(Incident.ipg_CarrierId).ToLower())).ToEntity<Incident>();

                    tracingService.Trace($"Check if Carrier not null");
                    if (caseEnt.ipg_CarrierId != null)
                    {
                        var team = crmService.Retrieve(queue.OwnerId.LogicalName, queue.OwnerId.Id, new ColumnSet(nameof(Team.ipg_threshold).ToLower())).ToEntity<Team>();

                        if (team.ipg_threshold == null)
                        {
                            team.ipg_threshold = DefaultThreshHold;


                            tracingService.Trace($"Update team treashhold");
                            crmService.Update(new Team() { Id = team.Id, ipg_threshold = team.ipg_threshold });
                        }

                        tracingService.Trace($"Retrieve users by Carrier and Team fetch = {GetGrouppedUserFetchByTeamAndCarrier(caseEnt.ipg_CarrierId, queue.OwnerId)}");
                        var response = (RetrieveMultipleResponse)crmService.Execute(new RetrieveMultipleRequest() 
                        { Query = new FetchExpression(GetGrouppedUserFetchByTeamAndCarrier(caseEnt.ipg_CarrierId, queue.OwnerId)) });

                        var users = response.EntityCollection;

                        tracingService.Trace($"Sorting Users");

                        var availibleUserGuid = users.Entities
                            .Where(e => e.GetAttributeValue<Microsoft.Xrm.Sdk.AliasedValue>("CaseCount")?.Value as int? < team.ipg_threshold)
                            .OrderByDescending(e => e.GetAttributeValue<Microsoft.Xrm.Sdk.AliasedValue>("isprimary")?.Value as bool?)
                            .ThenBy(e => e.GetAttributeValue<Microsoft.Xrm.Sdk.AliasedValue>("CaseCount")?.Value as int?)
                            .Select(e => e.GetAttributeValue<Microsoft.Xrm.Sdk.AliasedValue>("systemuserid")?.Value as Guid?)
                            .FirstOrDefault();

                        tracingService.Trace($"targetuser {(availibleUserGuid.HasValue ? availibleUserGuid.Value.ToString() : "empty")}");

                        if (availibleUserGuid.HasValue && availibleUserGuid.Value != Guid.Empty)
                        {
                            tracingService.Trace($"set up workerid");

                            target.WorkerId = new EntityReference(SystemUser.EntityLogicalName, availibleUserGuid.Value);

                            tracingService.Trace($"Updateding QueueItem");
                            crmService.Update(new Intake.QueueItem() { Id = target.Id, WorkerId = target.WorkerId });
                            
                            tracingService.Trace($"Updateding Case");
                            crmService.Update(new Incident() {Id = target.ObjectId.Id, OwnerId = target.WorkerId });
                        }
                    }
                }
            }

            tracingService.Trace($"{nameof(RouteItem)} finished");
        }

        private string GetGrouppedUserFetchByTeamAndCarrier(EntityReference carrierId, EntityReference teamId)
        {
            if (carrierId == null) throw new ArgumentException(nameof(carrierId));
            if (teamId == null) throw new ArgumentException(nameof(teamId));

            return $@"<fetch no-lock='true' aggregate='true' >
                  <entity name='teammembership' >
                    <attribute name='systemuserid' alias='systemuserid' groupby='true' />
                    <filter>
                      <condition entityname='accountUser' attribute='ipg_accountid' operator='eq' value='{carrierId.Id}' />
                      <condition attribute='teamid' operator='eq' value='{teamId.Id}'/>
                    </filter>
                    <link-entity name='ipg_accountuser' from='ipg_userid' to='systemuserid' link-type='inner' alias='accountUser' >
                      <attribute name='ipg_isprimary' alias='isprimary' groupby='true' />
                    </link-entity>
                    <link-entity name='incident' from='ownerid' to='systemuserid' link-type='outer' alias='Case' >
                      <attribute name='incidentid' alias='CaseCount' aggregate='count' distinct='true' />
                    </link-entity>
                  </entity>
                </fetch>";
        }
    }
}
