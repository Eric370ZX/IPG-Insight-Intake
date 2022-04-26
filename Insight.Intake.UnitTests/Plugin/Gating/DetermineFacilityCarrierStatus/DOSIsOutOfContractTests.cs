using FakeXrmEasy;
using Insight.Intake.Plugin.Gating.DetermineFacilityCarrierStatus;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating.DetermineFacilityCarrierStatus
{
    public class DOSIsOutOfContractTests : PluginTestsBase
    {
        [Fact]
        public void DOSIsOutOfContract_RequiredFieldsIsMissing_returnSuccessFalse()
        {
            var fakedContext = new XrmFakedContext();

            Incident incident = new Incident().Fake();

            var listForInit = new List<Entity> { incident };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() } };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingDetermineFacilityCarrierStatusDOSIsOutOfContract",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<DOSIsOutOfContract>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }

        [Fact]
        public void DOSIsOutOfContract_HaveOneActiveContractWithEmptyNetworkStatus_returnSuccessTrue()
        {
            var fakedContext = new XrmFakedContext();

            Intake.Account facility = new Intake.Account().Fake();
            Intake.Account carrier = new Intake.Account().Fake();
            Incident incident = new Incident().Fake().WithFacilityReference(facility).WithCarrierReference(carrier).WithActualDos(DateTime.Now);
            Entitlement entitlement = new Entitlement().Fake()
                .WithFacilityReference(facility)
                .WithCarrierReference(carrier)
                .WithEffectiveDateRange(DateTime.Now.AddDays(-10), DateTime.Now.AddDays(20))                
                .WithEntitlementType(new OptionSetValue(923720001));

            var listForInit = new List<Entity> { incident, entitlement };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() } };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingDetermineFacilityCarrierStatusDOSIsOutOfContract",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<DOSIsOutOfContract>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }
    }
}