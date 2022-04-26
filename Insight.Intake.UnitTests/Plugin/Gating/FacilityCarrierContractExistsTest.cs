using FakeXrmEasy;
using Insight.Intake.Data;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class FacilityCarrierContractExistsTest : PluginTestsBase
    {
        [Fact]
        public void FacilityCarrierContractExists_not_existed()
        {
            var fakedContext = new XrmFakedContext();

            Intake.Account carrier = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Carrier);
            carrier.Name = "Carrier 1";
            Intake.Account facility = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Facility);
            facility.Name = "Facility 1";

            Incident targetIncident = new Incident().Fake()
                .WithFacilityReference(facility)
                .WithPrimaryCarrierReference(carrier);
            Entitlement entitlement = new Entitlement().Fake(EntitlementState.Active)
                .WithCarrierReference(carrier)
                .WithFacilityReference(facility);

            var listForInit = new List<Entity>() { targetIncident, carrier, facility, entitlement };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", targetIncident.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingFacilityCarrierContractExists",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            fakedContext.ExecutePluginWith<FacilityCarrierContractExists>(pluginContext);

            Assert.False((bool)outputParameters["Succeeded"]);
        }

        [Fact]
        public void FacilityCarrierContractExists_existed()
        {
            var fakedContext = new XrmFakedContext();

            Intake.Account carrier = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Carrier);
            carrier.Name = "Carrier 1";
            Intake.Account facility = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Facility);
            facility.Name = "Facility 1";

            var dos = DateTime.Now;

            Incident targetIncident = new Incident().Fake()
                .WithFacilityReference(facility)
                .WithPrimaryCarrierReference(carrier)
                .WithActualDos(dos);
            Entitlement entitlement = new Entitlement().Fake(EntitlementState.Active)
                .WithEntitlementType(new OptionSetValue((int)ipg_EntitlementTypes.FacilityCarrier))
                .WithCarrierReference(carrier)
                .WithFacilityReference(facility)
                .WithEffectiveDateRange(dos.AddDays(-1), dos.AddDays(1))
                .RuleFor(p=>p.ipg_Active, true);

            var listForInit = new List<Entity>() { targetIncident, carrier, facility, entitlement };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", targetIncident.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingFacilityCarrierContractExists",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            fakedContext.ExecutePluginWith<FacilityCarrierContractExists>(pluginContext);

            Assert.True((bool)outputParameters["Succeeded"]);
        }
    }
}
