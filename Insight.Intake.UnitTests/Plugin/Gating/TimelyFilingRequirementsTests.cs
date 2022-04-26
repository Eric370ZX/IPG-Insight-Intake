using FakeXrmEasy;
using Insight.Intake.Extensions;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class TimelyFilingRequirementsTests : PluginTestsBase
    {
        [Fact]
        public void TimelyFilingRequirementsTests_Success()
        {
            var fakedContext = new XrmFakedContext();
            var incidentId = Guid.NewGuid();
            var procedureDate = DateTime.Now.AddDays(1);

            Intake.Account carrier = new Intake.Account()
                .Fake()
                .RuleFor(p => p.ipg_timelyfilingrule, 2);

            Incident incident = new Incident().Fake(incidentId)
                .RuleFor(p => p.ipg_ProcedureDateNew, p => procedureDate)
                .RuleFor(p => p.ipg_CarrierId, p => carrier.ToEntityReference());




            var listForInit = new List<Entity>() { incident, carrier };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckTimelyFilingRequirements",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<TimelyFilingRequirements>(pluginContext);

            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void TimelyFilingRequirementsTests_Fails()
        {
            var fakedContext = new XrmFakedContext();
            var incidentId = Guid.NewGuid();
            var procedureDate = DateTime.Now.AddDays(3);

            Intake.Account carrier = new Intake.Account()
                .Fake()
                .RuleFor(p => p.ipg_timelyfilingrule, 2);

            Incident incident = new Incident().Fake(incidentId)
                .RuleFor(p => p.ipg_ProcedureDateNew, p => procedureDate)
                .RuleFor(p => p.ipg_CarrierId, p => carrier.ToEntityReference());




            var listForInit = new List<Entity>() { incident, carrier };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckTimelyFilingRequirements",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<TimelyFilingRequirements>(pluginContext);

            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }
        [Fact]
        public void TimelyFilingRequirementsReferralTests_Success()
        {
            var fakedContext = new XrmFakedContext();
            var incidentId = Guid.NewGuid();
            var procedureDate = DateTime.Now.AddDays(-3);

            Intake.Account carrier = new Intake.Account()
                .Fake()
                .RuleFor(p => p.ipg_timelyfilingrule, 2);

            ipg_referral referral = new ipg_referral().Fake(incidentId)
                .RuleFor(p => p.ipg_SurgeryDate, p => procedureDate)
                .RuleFor(p => p.ipg_CarrierId, p => carrier.ToEntityReference());




            var listForInit = new List<Entity>() { referral, carrier };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckTimelyFilingRequirements",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<TimelyFilingRequirements>(pluginContext);

            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }
    }
}
