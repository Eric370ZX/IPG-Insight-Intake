using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CheckForCarrierAcceptCPTTests : PluginTestsBase
    {
        [Fact]
        public void CheckForCarrierAcceptCPTTests_CarrierCptExist_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Intake.Account carrier = new Intake.Account().Fake();
            var dos = DateTime.Now.AddDays(-30);
            ipg_cptcode cpt1 = new ipg_cptcode().Fake();
            ipg_cptcode cpt2 = new ipg_cptcode().Fake();

            ipg_associatedcpt carrierCpt = new ipg_associatedcpt().Fake()
                .WithCarrierReference(carrier)
                .WithCptCodeReference(cpt1)
                .WithEffectiveDate(dos.AddDays(-30))
                .WithExpirationDate(dos.AddDays(30));

            ipg_referral refEntity = new ipg_referral().Fake()
                .WithCarrierReference(carrier)
                .WithSurgeryDate(dos)
                .WithCptCodeReferences(new List<ipg_cptcode>() { cpt1, cpt2 });

            var listForInit = new List<Entity>() { refEntity, carrierCpt, carrier, cpt1, cpt2 };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", refEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckForCarrierAcceptCPT",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckForCarrierAcceptCPT>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CheckForCarrierAcceptCPTTests_CarrierCptNotExist_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Intake.Account carrier = new Intake.Account().Fake();
            var dos = DateTime.Now.AddDays(-30);
            ipg_cptcode cpt1 = new ipg_cptcode().Fake();
            ipg_cptcode cpt2 = new ipg_cptcode().Fake();

            ipg_referral refEntity = new ipg_referral().Fake()
                .WithCarrierReference(carrier)
                .WithSurgeryDate(dos)
                .WithCptCodeReferences(new List<ipg_cptcode>() { cpt1, cpt2 });

            var listForInit = new List<Entity>() { refEntity, carrier, cpt1, cpt2 };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", refEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckForCarrierAcceptCPT",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckForCarrierAcceptCPT>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }
    }
}
