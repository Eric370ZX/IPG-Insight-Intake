using FakeXrmEasy;
using Insight.Intake.Plugin.TaskEntity;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.TaskEntity
{
    public class RouteTaskPluginTests: PluginTestsBase
    {
        [Fact]
        public void TaskReassignedonCaseOwnerChange()
        {
            var fakedContext = new XrmFakedContext();

            SystemUser preownerEnt = new SystemUser().Fake();
            SystemUser newownerEnt = new SystemUser().Fake();
            Team team = new Team().Fake("Case Management");
            Incident caseEnt = new Incident().Fake().WithOwner(preownerEnt).WithAssignedToTeam(team);

            Task task = new Task().Fake().WithRegarding(caseEnt.ToEntityReference()).WithTaskCategory().WithAssignedToTeam(team);
            Task task2 = new Task().Fake().WithRegarding(caseEnt.ToEntityReference()).WithTaskCategory().WithAssignedToTeam(team);

            fakedContext.Initialize(new List<Entity> { preownerEnt, newownerEnt, caseEnt, task, task2, team });
            var target = new Incident() { Id = caseEnt.Id, OwnerId = newownerEnt.ToEntityReference(), ipg_assignedtoteamid = caseEnt.ipg_assignedtoteamid };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection { { "Target", target } },
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection() { { "PostImage", target } },
                PreEntityImages = new EntityImageCollection() { { "PreImage", caseEnt } },
            };

            fakedContext.ExecutePluginWith<RouteTaskPlugin>(pluginContext);

            var crmServiece = fakedContext.GetOrganizationService();
            var UpdTask1 = crmServiece.Retrieve(task.LogicalName, task.Id, new ColumnSet(Task.Fields.OwnerId)).ToEntity<Task>();
            var UpdTask2 = crmServiece.Retrieve(task2.LogicalName, task2.Id, new ColumnSet(Task.Fields.OwnerId)).ToEntity<Task>();

            Assert.Equal(UpdTask1.OwnerId, target.OwnerId);
            Assert.Equal(UpdTask2.OwnerId, target.OwnerId);
        }

        [Fact]
        public void TasksReassignedonReferralOwnerChange()
        {
            var fakedContext = new XrmFakedContext();

            SystemUser preownerEnt = new SystemUser().Fake();
            SystemUser newownerEnt = new SystemUser().Fake();
            Team team = new Team().Fake("Case Management");

            ipg_referral referral = new ipg_referral().Fake().WithOwner(preownerEnt).WithAssignedToTeam(team);

            Task task = new Task().Fake().WithRegarding(referral.ToEntityReference()).WithTaskCategory().WithAssignedToTeam(team);
            Task task2 = new Task().Fake().WithRegarding(referral.ToEntityReference()).WithTaskCategory().WithAssignedToTeam(team);

            fakedContext.Initialize(new List<Entity> { preownerEnt, newownerEnt, referral, task, task2, team });
            var target = new Incident() { Id = referral.Id, OwnerId = newownerEnt.ToEntityReference(), ipg_assignedtoteamid = referral.ipg_assignedtoteamid };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_referral.EntityLogicalName,
                InputParameters = new ParameterCollection { { "Target", target } },
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection() { { "PostImage", target } },
                PreEntityImages = new EntityImageCollection() { { "PreImage", referral } },
            };

            fakedContext.ExecutePluginWith<RouteTaskPlugin>(pluginContext);

            var crmServiece = fakedContext.GetOrganizationService();
            var UpdTask1 = crmServiece.Retrieve(task.LogicalName, task.Id, new ColumnSet(Task.Fields.OwnerId)).ToEntity<Task>();
            var UpdTask2 = crmServiece.Retrieve(task2.LogicalName, task2.Id, new ColumnSet(Task.Fields.OwnerId)).ToEntity<Task>();

            Assert.Equal(UpdTask1.OwnerId, target.OwnerId);
            Assert.Equal(UpdTask2.OwnerId, target.OwnerId);
        }

        [Fact]
        public void TasksNotReassignedonReferralOwnerChange()
        {
            var fakedContext = new XrmFakedContext();

            SystemUser preownerEnt = new SystemUser().Fake();
            SystemUser newownerEnt = new SystemUser().Fake();
            Team team = new Team().Fake("Case Management");
            Team team2 = new Team().Fake("Carrier Processing");


            ipg_referral referral = new ipg_referral().Fake().WithOwner(preownerEnt).WithAssignedToTeam(team2);

            Task task = new Task().Fake().WithRegarding(referral.ToEntityReference()).WithTaskCategory().WithAssignedToTeam(team);
            Task task2 = new Task().Fake().WithRegarding(referral.ToEntityReference()).WithTaskCategory().WithAssignedToTeam(team);

            fakedContext.Initialize(new List<Entity> { preownerEnt, newownerEnt, referral, task, task2, team, team2 });
            var target = new Incident() { Id = referral.Id, OwnerId = newownerEnt.ToEntityReference() };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_referral.EntityLogicalName,
                InputParameters = new ParameterCollection { { "Target", target } },
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection() { { "PostImage", target } },
                PreEntityImages = new EntityImageCollection() { { "PreImage", referral } },
            };

            fakedContext.ExecutePluginWith<RouteTaskPlugin>(pluginContext);

            var crmServiece = fakedContext.GetOrganizationService();
            var UpdTask1 = crmServiece.Retrieve(task.LogicalName, task.Id, new ColumnSet(Task.Fields.OwnerId)).ToEntity<Task>();
            var UpdTask2 = crmServiece.Retrieve(task2.LogicalName, task2.Id, new ColumnSet(Task.Fields.OwnerId)).ToEntity<Task>();

            Assert.NotEqual(UpdTask1.OwnerId, target.OwnerId);
            Assert.NotEqual(UpdTask2.OwnerId, target.OwnerId);
        }

        [Fact]
        public void TasksNotReassignedonCaseOwnerChange()
        {
            var fakedContext = new XrmFakedContext();

            SystemUser preownerEnt = new SystemUser().Fake();
            SystemUser newownerEnt = new SystemUser().Fake();
            Team team = new Team().Fake("Case Management");
            Team team2 = new Team().Fake("Carrier Processing");


            Incident incident = new Incident().Fake().WithOwner(preownerEnt);

            Task task = new Task().Fake().WithRegarding(incident.ToEntityReference()).WithTaskCategory().WithAssignedToTeam(team);
            Task task2 = new Task().Fake().WithRegarding(incident.ToEntityReference()).WithTaskCategory().WithAssignedToTeam(team);

            fakedContext.Initialize(new List<Entity> { preownerEnt, newownerEnt, incident, task, task2, team, team2 });
            var target = new Incident() { Id = incident.Id, OwnerId = newownerEnt.ToEntityReference() };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection { { "Target", target } },
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection() { { "PostImage", target } },
                PreEntityImages = new EntityImageCollection() { { "PreImage", incident } },
            };

            fakedContext.ExecutePluginWith<RouteTaskPlugin>(pluginContext);

            var crmServiece = fakedContext.GetOrganizationService();
            var UpdTask1 = crmServiece.Retrieve(task.LogicalName, task.Id, new ColumnSet(Task.Fields.OwnerId)).ToEntity<Task>();
            var UpdTask2 = crmServiece.Retrieve(task2.LogicalName, task2.Id, new ColumnSet(Task.Fields.OwnerId)).ToEntity<Task>();

            Assert.NotEqual(UpdTask1.OwnerId, target.OwnerId);
            Assert.NotEqual(UpdTask2.OwnerId, target.OwnerId);
        }
    }
}
