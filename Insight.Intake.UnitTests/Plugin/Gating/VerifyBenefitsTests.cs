using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class VerifyBenefitsTests : PluginTestsBase
    {
        [Fact]
        public void VerifyBenefits_IfNoBenefits_ReturnsFalse()
        {
            var fakedContext = new XrmFakedContext();
            Intake.Account carrier = new Intake.Account().Fake();
            Incident incident = new Incident().FakeInsuredPatient(carrier);
            
            var listForInit = new List<Entity> { incident, carrier };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() } };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingVerifyBenefits",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<VerifyBenefits>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.False(pluginContext.OutputParameters["Succeeded"] as bool?);
        }

        [Fact]
        public void VerifyBenefits_IfOutdatedBenefits_ReturnsFalse()
        {
            var fakedContext = new XrmFakedContext();
            Intake.Account carrier = new Intake.Account().Fake();
            Incident incident = new Incident().FakeInsuredPatient(carrier);
            var ebvBenefit = new ipg_benefit().Fake()
                .WithCaseReference(incident)
                .WithCarrierReference(carrier)
                .WithMemberId(incident.ipg_MemberIdNumber)
                .WithBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage)
                .WithBenefitSource(ipg_BenefitSources.EBV)
                .RuleFor(b => b.OverriddenCreatedOn, x => DateTime.Now.AddDays(-5))
                .Generate();
            var bvfBenefit = new ipg_benefit().Fake()
                .WithCaseReference(incident)
                .WithCarrierReference(carrier)
                .WithMemberId(incident.ipg_MemberIdNumber)
                .WithBenefitType(ipg_BenefitType.DurableMedicalEquipment_DME)
                .WithBenefitSource(ipg_BenefitSources.BVF)
                .RuleFor(b => b.OverriddenCreatedOn, x => DateTime.Now.AddDays(-35))
                .Generate();

            var listForInit = new List<Entity> { incident, carrier, ebvBenefit, bvfBenefit };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() } };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingVerifyBenefits",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<VerifyBenefits>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.False(pluginContext.OutputParameters["Succeeded"] as bool?);
        }

        [Fact]
        public void VerifyBenefits_IfValidEbvBenefit_ReturnsTrue()
        {
            var fakedContext = new XrmFakedContext();
            Intake.Account carrier = new Intake.Account().Fake();
            Incident incident = new Incident().FakeInsuredPatient(carrier);
            var ebvBenefit = new ipg_benefit().Fake()
                .WithCaseReference(incident)
                .WithCarrierReference(carrier)
                .WithMemberId(incident.ipg_MemberIdNumber)
                .WithBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage)
                .WithBenefitSource(ipg_BenefitSources.EBV)
                .RuleFor(b => b.OverriddenCreatedOn, x => DateTime.Now.AddDays(-2))
                .Generate();
            
            var listForInit = new List<Entity> { incident, carrier, ebvBenefit };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() } };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingVerifyBenefits",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<VerifyBenefits>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool?);
        }

        [Fact]
        public void VerifyBenefits_IfValidBvfBenefit_ReturnsTrue()
        {
            var fakedContext = new XrmFakedContext();
            Intake.Account carrier = new Intake.Account().Fake();
            Incident incident = new Incident().FakeInsuredPatient(carrier);
            var ebvBenefit = new ipg_benefit().Fake()
                .WithCaseReference(incident)
                .WithCarrierReference(carrier)
                .WithMemberId(incident.ipg_MemberIdNumber)
                .WithBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage)
                .WithBenefitSource(ipg_BenefitSources.BVF)
                .RuleFor(b => b.OverriddenCreatedOn, x => DateTime.Now.AddDays(-26))
                .Generate();

            var listForInit = new List<Entity> { incident, carrier, ebvBenefit };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() } };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingVerifyBenefits",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<VerifyBenefits>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool?);
        }
    }
}