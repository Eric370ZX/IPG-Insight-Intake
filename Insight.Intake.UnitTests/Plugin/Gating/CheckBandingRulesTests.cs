using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CheckBandingRulesTests : PluginTestsBase
    {
        [Fact]
        public void CheckBandingRulesTests_CompletedTaskExist_Success()
        {
            var fakedContext = new XrmFakedContext();
            Incident refCase = new Incident().Fake();
            Task task1 = new Task().Fake()
                .WithType(ipg_TaskType1.Openbandingworkflowtask)
                .WithRegarding(refCase.ToEntityReference())
                .WithState(TaskState.Completed);
            Task task2 = new Task().Fake()
                 .WithType(ipg_TaskType1.Openbandingworkflowtask)
                 .WithRegarding(refCase.ToEntityReference())
                 .WithState(TaskState.Open);
            var listForInit = new List<Entity>() { refCase, task1, task2 };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", refCase.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckBandingRules",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckBandingRules>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }
        [Fact]
        public void CheckBandingRulesTests_CompletedTaskNotExist_Error()
        {
            var fakedContext = new XrmFakedContext();
            Incident refCase = new Incident().Fake();
            Task task1 = new Task().Fake()
                .WithType(ipg_TaskType1.Openbandingworkflowtask)
                .WithRegarding(refCase.ToEntityReference())
                .WithState(TaskState.Open);
            Task task2 = new Task().Fake()
                 .WithType(ipg_TaskType1.Openbandingworkflowtask)
                 .WithRegarding(refCase.ToEntityReference())
                 .WithState(TaskState.Open);
            var listForInit = new List<Entity>() { refCase, task1, task2 };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", refCase.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckBandingRules",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckBandingRules>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }
        [Fact]
        public void CheckBandingRulesTests_NoTasksExist_Error()
        {
            var fakedContext = new XrmFakedContext();
            Incident refCase = new Incident().Fake();
            
            var listForInit = new List<Entity>() { refCase };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", refCase.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckBandingRules",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckBandingRules>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }
    }
}
