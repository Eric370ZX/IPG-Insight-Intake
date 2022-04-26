using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CheckVerifyAllReceivedDateTests : PluginTestsBase
    {
        [Fact]
        public void CheckVerifyAllReceivedField_HasAllReceived_returnFail()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident targetRef = new Incident()
                .Fake()
                .RuleFor(p => p.ipg_isallreceiveddate, p => null);

            var listForInit = new List<Entity>() { targetRef };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", targetRef.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckVerifyAllReceivedDate",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckVerifyAllReceivedDate>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }

        [Fact]
        public void CheckVerifyAllReceivedField_HasAllReceived_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident targetRef = new Incident()
                .Fake()
                .RuleFor(p => p.ipg_isallreceiveddate, p => DateTime.Now);

            var listForInit = new List<Entity>() { targetRef };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", targetRef.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckVerifyAllReceivedDate",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckVerifyAllReceivedDate>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

    }
}
