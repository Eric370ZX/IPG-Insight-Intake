using FakeXrmEasy;
using Insight.Intake.Plugin.Gating.DetermineFacilityCarrierStatus;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating.DetermineFacilityCarrierStatus
{
    public class NoContractTests : PluginTestsBase
    {
        [Fact]
        public void NoContract_RequiredFieldsIsMissing_returnSuccessFalse()
        {
            var fakedContext = new XrmFakedContext();

            Incident incident = new Incident().Fake();

            var listForInit = new List<Entity> { incident };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() } };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingDetermineFacilityCarrierStatusNoContract",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<NoContract>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }

        [Fact]
        public void NoContract_HaveNoActiveContract_returnSuccessFalse()
        {
            var fakedContext = new XrmFakedContext();

            Intake.Account facility = new Intake.Account().Fake();
            Intake.Account carrier = new Intake.Account().Fake();
            Incident incident = new Incident().Fake().WithFacilityReference(facility).WithCarrierReference(carrier).WithActualDos(DateTime.Now);
            Entitlement entitlement = new Entitlement().Fake();                

            var listForInit = new List<Entity> { incident, entitlement };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() } };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingDetermineFacilityCarrierStatusNoContract",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<NoContract>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }
    }
}