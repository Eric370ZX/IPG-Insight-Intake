using FakeXrmEasy;
using FakeXrmEasy.FakeMessageExecutors;
using Insight.Intake.Plugin.QueueItem;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.QueueItem
{
    public class RouteQueueItemPluginTest: PluginTestsBase
    {
        [Fact]
        public void QueueItemRoutedToPrimaryCarrierUser()
        {
            var fakedContext = new XrmFakedContext();

            Team team = new Team().Fake("Authorizations");

            Intake.Account carrier = new Intake.Account().Fake();
            
            Intake.SystemUser primaryUser = new SystemUser().Fake();
            Intake.SystemUser backUpUser = new SystemUser().Fake();

            fakedContext.AddFakeMessageExecutor<RetrieveMultipleRequest>(new FakeRetrieveMultipleRequestExecutor());

            Intake.Incident incident = new Intake.Incident().Fake().WithCarrierReference(carrier);

            List<TeamMembership> teammembership = new List<TeamMembership>()
            {
                new TeamMembership().Fake(team, primaryUser),
                new TeamMembership().Fake(team, backUpUser)
            };

            List<ipg_accountuser> accountUsers = new List<ipg_accountuser>()
            {
                new ipg_accountuser().Fake(primaryUser, carrier, true),
                new ipg_accountuser().Fake(backUpUser, carrier, true),
            };



            Intake.Queue queue = new Intake.Queue().Fake().WithOwner(team);


            Intake.QueueItem queItem = new Intake.QueueItem().Fake().WithQueue(queue).WithItem(incident);

            var initializedEntity = new List<Entity> { 
                incident, team, primaryUser, backUpUser, queItem
                , carrier, queue};
            initializedEntity.AddRange(teammembership);
            initializedEntity.AddRange(accountUsers);

            fakedContext.Initialize(initializedEntity);


            var crmContext = new OrganizationServiceContext(fakedContext.GetOrganizationService());

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Intake.Product.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", queItem } },
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection()
            };

            fakedContext.ExecutePluginWith<RouteQueueItemPlugin>(pluginContext);

            incident = fakedContext.GetOrganizationService().Retrieve(incident.LogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();
            queItem = fakedContext.GetOrganizationService().Retrieve(queItem.LogicalName, queItem.Id, new ColumnSet(true)).ToEntity<Intake.QueueItem>();

            Assert.True(object.Equals(queItem.WorkerId, primaryUser.ToEntityReference()), "Primary user not assigned to the queueItem!");
            Assert.True(object.Equals(queItem.WorkerId, incident.OwnerId), "Primary user not assigned to the Case!");
        }

        [Fact]
        public void QueueItemRoutedToBckUpCarrierUserWhenPrimaryLoaded()
        {

            var fakedContext = new XrmFakedContext();

            Team team = new Team().Fake("Authorizations");

            Intake.Account carrier = new Intake.Account().Fake();

            Intake.SystemUser primaryUser = new SystemUser().Fake();
            Intake.SystemUser backUpUser = new SystemUser().Fake();

            fakedContext.AddFakeMessageExecutor<RetrieveMultipleRequest>(new FakeRetrieveMultipleRequestExecutor());

            Intake.Incident incident = new Intake.Incident().Fake().WithCarrierReference(carrier);

            List<TeamMembership> teammembership = new List<TeamMembership>()
            {
                new TeamMembership().Fake(team, primaryUser),
                new TeamMembership().Fake(team, backUpUser)
            };

            var incidentlist = new List<Intake.Incident>()
            {
                new Intake.Incident().Fake().WithOwner(primaryUser),
                new Intake.Incident().Fake().WithOwner(primaryUser),
                new Intake.Incident().Fake().WithOwner(primaryUser),
                new Intake.Incident().Fake().WithOwner(primaryUser),
            };

            List<ipg_accountuser> accountUsers = new List<ipg_accountuser>()
            {
                new ipg_accountuser().Fake(primaryUser, carrier, true),
                new ipg_accountuser().Fake(backUpUser, carrier, true),
            };



            Intake.Queue queue = new Intake.Queue().Fake().WithOwner(team);


            Intake.QueueItem queItem = new Intake.QueueItem().Fake().WithQueue(queue).WithItem(incident);

            var initializedEntity = new List<Entity> {
                incident, team, primaryUser, backUpUser, queItem
                , carrier, queue, queItem};
            initializedEntity.AddRange(teammembership);
            initializedEntity.AddRange(accountUsers);
            initializedEntity.AddRange(incidentlist);

            fakedContext.Initialize(initializedEntity);


            var crmContext = new OrganizationServiceContext(fakedContext.GetOrganizationService());

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Intake.Product.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", queItem } },
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection()
            };

            fakedContext.ExecutePluginWith<RouteQueueItemPlugin>(pluginContext);

            incident = fakedContext.GetOrganizationService().Retrieve(incident.LogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();
            queItem = fakedContext.GetOrganizationService().Retrieve(queItem.LogicalName, queItem.Id, new ColumnSet(true)).ToEntity<Intake.QueueItem>();

            Assert.True(object.Equals(queItem.WorkerId, backUpUser.ToEntityReference()), "BackUp user not assigned to the queueItem!");
            Assert.True(object.Equals(queItem.WorkerId, incident.OwnerId), "BackUp user not assigned to the Case!");
        }

        [Fact]
        public void QueueItemRoutedToCarrierUserWithMinLoad()
        {
            var fakedContext = new XrmFakedContext();

            Team team = new Team().Fake("Authorizations");

            Intake.Account carrier = new Intake.Account().Fake();

            Intake.SystemUser primaryUser = new SystemUser().Fake();
            Intake.SystemUser backUpUser = new SystemUser().Fake();
            Intake.SystemUser backUpUser2 = new SystemUser().Fake();

            fakedContext.AddFakeMessageExecutor<RetrieveMultipleRequest>(new FakeRetrieveMultipleRequestExecutor());

            Intake.Incident incident = new Intake.Incident().Fake().WithCarrierReference(carrier);

            List<TeamMembership> teammembership = new List<TeamMembership>()
            {
                new TeamMembership().Fake(team, primaryUser),
                new TeamMembership().Fake(team, backUpUser),
                new TeamMembership().Fake(team, backUpUser2)
            };

            var incidentlist = new List<Intake.Incident>()
            {
                new Intake.Incident().Fake().WithOwner(primaryUser),
                new Intake.Incident().Fake().WithOwner(primaryUser),
                new Intake.Incident().Fake().WithOwner(primaryUser),
                new Intake.Incident().Fake().WithOwner(primaryUser),
                new Intake.Incident().Fake().WithOwner(backUpUser),
                new Intake.Incident().Fake().WithOwner(backUpUser),
                new Intake.Incident().Fake().WithOwner(backUpUser2),
            };

            List<ipg_accountuser> accountUsers = new List<ipg_accountuser>()
            {
                new ipg_accountuser().Fake(primaryUser, carrier, true),
                new ipg_accountuser().Fake(backUpUser, carrier),
                new ipg_accountuser().Fake(backUpUser2, carrier),
            };



            Intake.Queue queue = new Intake.Queue().Fake().WithOwner(team);


            Intake.QueueItem queItem = new Intake.QueueItem().Fake().WithQueue(queue).WithItem(incident);

            var initializedEntity = new List<Entity> {
                incident, team, primaryUser, backUpUser, backUpUser2, queItem
                , carrier, queue};
            initializedEntity.AddRange(teammembership);
            initializedEntity.AddRange(accountUsers);
            initializedEntity.AddRange(incidentlist);

            fakedContext.Initialize(initializedEntity);


            var crmContext = new OrganizationServiceContext(fakedContext.GetOrganizationService());

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Intake.Product.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", queItem } },
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection()
            };

            fakedContext.ExecutePluginWith<RouteQueueItemPlugin>(pluginContext);

            incident = fakedContext.GetOrganizationService().Retrieve(incident.LogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();
            queItem = fakedContext.GetOrganizationService().Retrieve(queItem.LogicalName, queItem.Id, new ColumnSet(true)).ToEntity<Intake.QueueItem>();

            Assert.True(object.Equals(queItem.WorkerId, backUpUser2.ToEntityReference()), "BackUp user not assigned to the queueItem!");
            Assert.True(object.Equals(queItem.WorkerId, incident.OwnerId), "BackUp user not assigned to the Case!");
        }


        protected class FakeRetrieveMultipleRequestExecutor : IFakeMessageExecutor
        {
            public bool CanExecute(OrganizationRequest request)
            {
                return request is RetrieveMultipleRequest;              
            }

            public OrganizationResponse Execute(OrganizationRequest request, XrmFakedContext ctx)
            {
                var retireveMultipleRequest = request as RetrieveMultipleRequest;
                                
                var fetch = retireveMultipleRequest.Query as FetchExpression;

                if (fetch != null
                    && fetch.Query.Contains("aggregate='true'")
                    && fetch.Query.Contains("<attribute name='systemuserid' alias='systemuserid' groupby='true'")
                    && fetch.Query.Contains("<link-entity name='ipg_accountuser' from='ipg_userid' to='systemuserid' link-type='inner'"))
                {
                    var crmContext = new OrganizationServiceContext(ctx.GetOrganizationService());

                    var teammembershipGroup = (from tm in crmContext.CreateQuery<TeamMembership>()
                                                join accountUser in crmContext.CreateQuery<ipg_accountuser>() on tm.SystemUserId equals accountUser.ipg_userid.Id
                                                select new
                                                {
                                                  tm.SystemUserId,
                                                  accountUser.ipg_isprimary
                                                }).ToList().Distinct().Select(tm =>
                                                new Entity()
                                                {

                                                    Attributes =
                                                    {
                                                        {"systemuserid", new AliasedValue("teammembership","systemuserid", tm.SystemUserId) },
                                                        {"isprimary", new AliasedValue("ipg_accountuser","ipg_isprimary", tm.ipg_isprimary) },
                                                        {"CaseCount", new AliasedValue("incident","incidentid", (from inc in crmContext.CreateQuery<Incident>()
                                                                    where inc.OwnerId.Id == tm.SystemUserId
                                                                    select inc.IncidentId).ToList().Count)}
                                                    }
                                                }).ToList();

                    return new RetrieveMultipleResponse()
                    {
                        ResponseName = "Successful",
                        Results = { { "EntityCollection", new EntityCollection(teammembershipGroup) } }
                    };
                }

                return new RetrieveMultipleRequestExecutor().Execute(request, ctx);
            }

            public Type GetResponsibleRequestType()
            {
                return typeof(RetrieveMultipleRequest);
            }
        }
    }
}
