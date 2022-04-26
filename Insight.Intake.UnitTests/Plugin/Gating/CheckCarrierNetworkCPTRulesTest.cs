using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CheckCarrierNetworkCPTRulesTest : PluginTestsBase
    {
        [Fact]
        public void CheckCarrierNetworkCPTRulesTest_ReturnSuccess()
        {
            /// Arrange
            var fakedContext = new XrmFakedContext();
            ipg_carriernetwork healthPlanNetwork = new ipg_carriernetwork().Fake();

            

            Intake.Account carrier = new Intake.Account().Fake()
                .WithCarrierNetworkReference(healthPlanNetwork);

            ipg_referral referral = new ipg_referral().Fake()
                .WithCarrierReference(carrier)
                .WithSurgeryDate(DateTime.Now.AddDays(-90));

            ipg_carriernetworkcptrule carrierNetworkCptRule = new ipg_carriernetworkcptrule().Fake()
                .WithNetworkCarrierRelationshipId(healthPlanNetwork.ToEntityReference())
                .WithEffectivesDates(DateTime.Now.AddDays(-120), DateTime.Now);

            var listForInit = new List<Entity>() { referral, healthPlanNetwork, carrier, carrierNetworkCptRule };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckCarrierNetworkCPTRules",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            /// Act
            fakedContext.ExecutePluginWith<CheckCarrierNetworkCPTRules>(pluginContext);

            /// Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }

        [Fact]
        public void CheckCarrierNetworkCPTRulesTest_ReturnError()
        {
            /// Arrange
            var fakedContext = new XrmFakedContext();
            ipg_carriernetwork healthPlanNetwork = new ipg_carriernetwork().Fake();

            Intake.Account carrier = new Intake.Account().Fake()
                        .WithCarrierNetworkReference(healthPlanNetwork);

            ipg_referral referral = new ipg_referral().Fake()
                .WithCarrierReference(carrier)
                .WithSurgeryDate(DateTime.Now.AddDays(-120));

            ipg_carriernetworkcptrule carrierNetworkCptRule = new ipg_carriernetworkcptrule().Fake()
                .WithNetworkCarrierRelationshipId(carrier.ToEntityReference())
                .WithEffectivesDates(DateTime.Now.AddDays(-90), DateTime.Now);

            var listForInit = new List<Entity>() { referral, healthPlanNetwork, carrier, carrierNetworkCptRule };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckCarrierNetworkCPTRules",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            /// Act
            fakedContext.ExecutePluginWith<CheckCarrierNetworkCPTRules>(pluginContext);

            /// Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }
    }
}