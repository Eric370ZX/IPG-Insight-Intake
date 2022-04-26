using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CheckBSAVerificationTests : PluginTestsBase
    {
        [Fact]
        public void CheckBSAVerificationTests_HasBSA_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            var initDate = new DateTime(2019, 2, 1);

            Insight.Intake.Account facility = new Intake.Account().Fake();
            facility.ipg_FacilitySignedBSA = new OptionSetValue((int)ipg_SignedBSA.Yes);
            facility.ipg_bsasigneddate = initDate.AddDays(-5);

            Incident caseEntity = new Incident().Fake().WithFacilityReference(facility);
            caseEntity.ipg_ActualDOS = initDate;

            Entitlement bsaEnt = new Entitlement().Fake()
                .WithFacilityReference(facility)
                .RuleFor(p=>p.CustomerId,p=>facility.ToEntityReference());
            bsaEnt.ipg_EntitlementType = new OptionSetValue((int)ipg_EntitlementTypes.BSA);
            bsaEnt.StartDate = initDate.AddDays(-10);
            bsaEnt.EndDate = initDate.AddDays(10);

            

            var listForInit = new List<Entity>() { caseEntity, facility, bsaEnt };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckBSAVerification",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckBSAVerification>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CheckBSAVerificationTests_NoBSA_SignedBSA_is_not_required_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            var initDate = new DateTime(2019, 2, 1);

            Intake.Account facility = new Intake.Account().Fake();
            facility.ipg_FacilitySignedBSA = new OptionSetValue((int)ipg_SignedBSA.NotRequired);

            Incident caseEntity = new Incident().Fake().WithFacilityReference(facility);
            caseEntity.ipg_ActualDOS = initDate;

            Entitlement bsaEnt = new Entitlement().Fake()
                .WithFacilityReference(facility)
                .RuleFor(p => p.CustomerId, p => facility.ToEntityReference());
            bsaEnt.ipg_EntitlementType = new OptionSetValue((int)ipg_EntitlementTypes.BSA);
            bsaEnt.StartDate = initDate.AddDays(-10);
            bsaEnt.EndDate = initDate.AddDays(10);



            var listForInit = new List<Entity>() { caseEntity, facility, bsaEnt };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckBSAVerification",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckBSAVerification>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CheckBSAVerificationTests_OutOfRange__returnError()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            var initDate = new DateTime(2019, 2, 1);

            Insight.Intake.Account facility = new Intake.Account().Fake();
            facility.ipg_FacilitySignedBSA = new OptionSetValue((int)ipg_SignedBSA.No);

            Incident caseEntity = new Incident().Fake().WithFacilityReference(facility);
            caseEntity.ipg_ActualDOS = new DateTime(2019, 2, 1);

            Entitlement bsaEnt = new Entitlement().Fake()
                .WithFacilityReference(facility)
                .RuleFor(p => p.CustomerId, p => facility.ToEntityReference());
            bsaEnt.ipg_EntitlementType = new OptionSetValue((int)ipg_EntitlementTypes.BSA);
            bsaEnt.StartDate = initDate.AddDays(10);
            bsaEnt.EndDate = initDate.AddDays(20);



            var listForInit = new List<Entity>() { caseEntity, facility, bsaEnt };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckBSAVerification",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckBSAVerification>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }
    }
}
