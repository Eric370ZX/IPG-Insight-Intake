using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CheckMedicalRecordsTests : PluginTestsBase
    {
        [Fact]
        public void CheckMedicalRecordsTests_MemberIdBeginsFromNSA()
        {
            var fakedContext = new XrmFakedContext();
            Incident refCase = new Incident().Fake();
            refCase["ipg_memberidnumber"] = "NSB00000";
            var listForInit = new List<Entity>() { refCase};
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", refCase.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckMedicalRecords",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckMedicalRecords>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);

        }
    }
}
