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
    public class CheckCarrierIsAcceptedTests : PluginTestsBase
    {
        [Fact]
        public void CheckCarrierIsAcceptedTests_CarrierIsAccepted_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Insight.Intake.Account carrier = new Insight.Intake.Account().Fake();
            carrier.ipg_carrieraccepted = true;
            carrier.Name = "some name";

            Insight.Intake.SystemUser owner = new Insight.Intake.SystemUser().Fake();
            owner.FirstName = "Full name";

            Incident caseEntity = new Incident().Fake()
                .RuleFor(p => p.ipg_CarrierId, p => carrier.ToEntityReference())
                .RuleFor(p => p.OwnerId, p => owner.ToEntityReference());


            var listForInit = new List<Entity>() { caseEntity, carrier, owner };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCarrierIsAccepted",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckCarrierIsAccepted>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CheckCarrierIsAcceptedTests_CarrierIsAccepted_returnError()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Insight.Intake.Account carrier = new Insight.Intake.Account().Fake();
            carrier.ipg_carrieraccepted = false;
            carrier.Name = "some name";

            Insight.Intake.SystemUser owner = new Insight.Intake.SystemUser().Fake();
            owner.FirstName = "Full name";

            Incident caseEntity = new Incident().Fake()
                .RuleFor(p => p.ipg_CarrierId, p => carrier.ToEntityReference())
                .RuleFor(p => p.OwnerId, p => owner.ToEntityReference());


            var listForInit = new List<Entity>() { caseEntity, carrier , owner };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCarrierIsAccepted",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null 
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckCarrierIsAccepted>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }
    }
}
