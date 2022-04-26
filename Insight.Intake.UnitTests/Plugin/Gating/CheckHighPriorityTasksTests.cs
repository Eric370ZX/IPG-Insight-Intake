using FakeXrmEasy;
using Insight.Intake.Extensions;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CheckHighPriorityTasksTests : PluginTestsBase
    {
        [Fact]
        public void CheckHighPriorityTasksTests_HighPriorityTasksExist()
        {
            var fakedContext = new XrmFakedContext();
            var incidentId = Guid.NewGuid();
            Incident incident = new Incident().Fake(incidentId);

            var task = new Task().Fake().WithRegarding(new EntityReference(Incident.EntityLogicalName, incidentId));
            task.RuleFor(x => x.StateCode, x => TaskState.Open);
            task.RuleFor(x => x.ipg_priority, x => ipg_Priority.High.ToOptionSetValue());

            var listForInit = new List<Entity>() { incident, task };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckHighPriorityTasks",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckHighPriorityTasks>(pluginContext);

            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }

        [Fact]
        public void CheckHighPriorityTasksTests_HighPriorityTaskClosed()
        {
            var fakedContext = new XrmFakedContext();
            var incidentId = Guid.NewGuid();
            Incident incident = new Incident().Fake(incidentId);

            var task = new Task().Fake().WithRegarding(new EntityReference(Incident.EntityLogicalName, incidentId));
            task.RuleFor(x => x.StateCode, x => TaskState.Completed);
            task.RuleFor(x => x.ipg_priority, x => ipg_Priority.High.ToOptionSetValue());

            var listForInit = new List<Entity>() { incident, task };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckHighPriorityTasks",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckHighPriorityTasks>(pluginContext);

            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

    }
}
