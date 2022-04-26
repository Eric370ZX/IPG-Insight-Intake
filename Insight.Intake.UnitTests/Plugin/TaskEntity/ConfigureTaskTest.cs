using FakeXrmEasy;
using Insight.Intake.Helpers;
using Insight.Intake.Plugin.TaskEntity;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.TaskEntity
{
    public class ConfigureTaskTest
    {
        [Fact]
        public void ConfigureTaskTest_InfoCopiedFromTaskType()
        {
            var fakedContext = new XrmFakedContext();

            Team team = new Team().Fake("Carrier Collection User");
            SystemUser systemuser = new SystemUser().Fake("systemuser");
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithStartDate(1).WithDueDate(3).WithAssignToteam(team.ToEntityReference());
            Incident incident = new Incident().Fake();
            Task task = new Task().Fake().WithCase(incident.ToEntityReference()).WithTypeRef(taskType.ToEntityReference()).WithOwner(systemuser.ToEntityReference());

            var listForInit = new List<Entity>() { team, taskType, incident, task, systemuser };
            fakedContext.Initialize(listForInit);


            var inputParameters = new ParameterCollection { { "Target", task } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreValidation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection(),
            };

            fakedContext.ExecutePluginWith<ConfigureTask>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();

            var createdTask = task;

            Assert.Equal(createdTask.OwnerId.Id, team.Id);

            Assert.Equal(createdTask.ScheduledStart.Value.Date, BusinessDayHelper.AddBusinessDays(DateTime.Today,1, null));
            Assert.Equal(createdTask.ScheduledEnd.Value.Date, BusinessDayHelper.AddBusinessDays(DateTime.Today, 4, null));
        }
    }
}
