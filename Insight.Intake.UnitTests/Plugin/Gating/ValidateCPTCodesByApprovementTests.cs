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
    
    public class ValidateCPTCodesByApprovementTests : PluginTestsBase
    {
        [Fact]
        public void ValidateCPTCodesByApprovement_ReferralHasValideCPTCodes_returnSuccessTrue()
        {
            var fakedContext = new XrmFakedContext();

            ipg_cptcode cptCode = new ipg_cptcode().Fake().WithStatusCode(2);
            ipg_referral referral = new ipg_referral().Fake().WithCptCodeReferences(new List<ipg_cptcode> { cptCode });

            var listForInit = new List<Entity> { referral, cptCode };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() } };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingValidateCPTCodesByApprovement",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<ValidateCPTCodesByApprovement>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void ValidateCPTCodesByApprovement_ReferralHasNotCPTCodes_returnSuccessTrue()
        {
            var fakedContext = new XrmFakedContext();

            ipg_referral referral = new ipg_referral().Fake();

            var listForInit = new List<Entity> { referral };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() } };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingValidateCPTCodesByApprovement",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<ValidateCPTCodesByApprovement>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void ValidateCPTCodesByApprovement_ReferralHasInvalideCPTCode_returnSuccessFalse()
        {
            var fakedContext = new XrmFakedContext();

            ipg_cptcode cptCode = new ipg_cptcode().Fake().WithStatusCode(1);
            ipg_referral referral = new ipg_referral().Fake().WithCptCodeReferences(new List<ipg_cptcode> { cptCode });

            var listForInit = new List<Entity> { referral, cptCode };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() } };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingValidateCPTCodesByApprovement",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<ValidateCPTCodesByApprovement>(pluginContext);

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }
    }
}
