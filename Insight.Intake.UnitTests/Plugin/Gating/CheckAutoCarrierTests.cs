using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CheckAutoCarrierTests : PluginTestsBase
    {
        [Fact]
        public void CheckAutoCarrierTests_CompletedTaskExist_Success()
        {
            var fakedContext = new XrmFakedContext();
            Intake.Account account = new Intake.Account().Fake();
            account.ipg_CarrierType = new OptionSetValue((int)ipg_CarrierType.Government);
            Incident refCase = new Incident().Fake()
                .WithCarrierReference(account);
           
            var listForInit = new List<Entity>() { refCase, account };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", refCase.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckAutoCarrier",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckAutoCarrier>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CheckAutoCarrierTests_AutoCarrier_Error()
        {
            var fakedContext = new XrmFakedContext();
            Intake.Account account = new Intake.Account().Fake();
            account.ipg_CarrierType = new OptionSetValue((int)ipg_CarrierType.Auto);
            Incident refCase = new Incident().Fake()
                .WithCarrierReference(account);

            var listForInit = new List<Entity>() { refCase, account };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", refCase.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckAutoCarrier",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckAutoCarrier>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }
    }
}
