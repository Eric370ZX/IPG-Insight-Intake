using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CheckDisplayedStatusForCancelTests : PluginTestsBase
    {
        [Fact]
        public void CheckDisplayedStatusForCancelTests_StatusIsSuccess_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            var status = new ipg_casestatusdisplayed().Fake().WithName("Success");

            ipg_referral refEntity = new ipg_referral().Fake()
                .WithDisplayedStatus(status);

            var listForInit = new List<Entity>() { refEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", refEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckDisplayedStatusForCancel",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckDisplayedStatusForCancel>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CheckDisplayedStatusForCancelTests_StatusIsCancel_returnError()
        {
            //arrange
            var fakedContext = new XrmFakedContext();
            var status = new ipg_casestatusdisplayed().Fake().WithName("CancelStatus");

            ipg_referral refEntity = new ipg_referral().Fake()
                .WithDisplayedStatus(status);

            var listForInit = new List<Entity>() { refEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", refEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckDisplayedStatusForCancel",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckDisplayedStatusForCancel>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }

        [Fact]
        public void CheckDisplayedStatusForCancelTests_NoStatus_returnError()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            ipg_referral refEntity = new ipg_referral().Fake();

            var listForInit = new List<Entity>() { refEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", refEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckDisplayedStatusForCancel",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckDisplayedStatusForCancel>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }
    }
}
