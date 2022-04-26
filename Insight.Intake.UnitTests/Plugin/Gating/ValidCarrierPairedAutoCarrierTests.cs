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
    public class ValidCarrierPairedAutoCarrierTests : PluginTestsBase
    {
        [Fact]
        public void ValidCarrierPairedAutoCarrierTests_returnSuccessTrue()
        {
            var fakedContext = new XrmFakedContext();

            var carrier = new Intake.Account().Fake();
            Incident incident = new Incident().Fake()
                .WithCarrierReference(carrier);


            var listForInit = new List<Entity> { incident, carrier };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() } };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingValidCarrierPairedAutoCarrier",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<ValidCarrierPairedAutoCarrier>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }
        [Fact]
        public void ValidCarrierPairedAutoCarrierTests_returnFails()
        {
            var fakedContext = new XrmFakedContext();

            Intake.Account carrier = new Intake.Account().Fake();

            Incident incident = new Incident().Fake()
                .RuleFor(p => p.ipg_SecondaryCarrierId, p => carrier.ToEntityReference());


            var listForInit = new List<Entity> { incident, carrier };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() } };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingValidCarrierPairedAutoCarrier",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<ValidCarrierPairedAutoCarrier>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }
    }
}
