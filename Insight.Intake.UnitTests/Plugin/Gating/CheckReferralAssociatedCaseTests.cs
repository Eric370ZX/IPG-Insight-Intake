using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CheckReferralAssociatedCaseTests : PluginTestsBase
    {
        [Fact]
        public void CheckReferralAssociatedCaseTests_CaseExist_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            ipg_referral refEntity = new ipg_referral().Fake()
                .WithCaseReference(new Incident().Fake());

            var listForInit = new List<Entity>() { refEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", refEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckReferralAssociatedCase",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckReferralAssociatedCase>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CheckReferralAssociatedCaseTests_CaseIsNull_returnError()
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
                MessageName = "ipg_IPGGatingCheckReferralAssociatedCase",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckReferralAssociatedCase>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }
    }
}
