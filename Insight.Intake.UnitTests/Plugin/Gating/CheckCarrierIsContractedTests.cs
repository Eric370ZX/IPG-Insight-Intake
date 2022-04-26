using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CheckIfPrimaryCarrierIsContractedTests : PluginTestsBase
    {
        [Fact]
        public void CheckIfPrimaryCarrierIsContracted_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Insight.Intake.Account carrier = new Insight.Intake.Account().Fake();
            carrier.ipg_contract = true;


            Incident caseEntity = new Incident().Fake()
                .RuleFor(p => p.ipg_CarrierId, p => carrier.ToEntityReference());


            var listForInit = new List<Entity>() { caseEntity, carrier };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckIfPrimaryCarrierIsContracted",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckIfPrimaryCarrierIsContracted>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CheckIfPrimaryCarrierIsContracted_returnError()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Insight.Intake.Account carrier = new Insight.Intake.Account().Fake();
            carrier.ipg_contract = false;


            Incident caseEntity = new Incident().Fake()
                .RuleFor(p => p.ipg_CarrierId, p => carrier.ToEntityReference());


            var listForInit = new List<Entity>() { caseEntity, carrier };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckIfPrimaryCarrierIsContracted",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckIfPrimaryCarrierIsContracted>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }
    }
}
