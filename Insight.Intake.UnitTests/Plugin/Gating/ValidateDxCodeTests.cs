using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class ValidateDxCodeTests : PluginTestsBase
    {
        [Fact]
        public void CheckValidDxCodePassTheGate()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            //DOS inside effective and expiration date.
            caseEntity.ipg_SurgeryDate = new System.DateTime(2019, 5, 20);
            ipg_dxcode dxCode = new ipg_dxcode().Fake();
            dxCode.ipg_EffectiveDate = new System.DateTime(2019, 2, 20);
            dxCode.ipg_ExpirationDate = new System.DateTime(2020, 2, 20);
            caseEntity.ipg_DxCodeId1 = dxCode.ToEntityReference();

            var listForInit = new List<Entity>() { caseEntity, dxCode };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingValidateDxCode",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<ValidateDxCode>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CheckInvalidDxCodeFailsTheGate()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            //DOS outside of effective and expiration date.
            caseEntity.ipg_SurgeryDate = new System.DateTime(2020, 5, 20);
            ipg_dxcode dxCode = new ipg_dxcode().Fake();
            dxCode.ipg_EffectiveDate = new System.DateTime(2019, 2, 20);
            dxCode.ipg_ExpirationDate = new System.DateTime(2020, 2, 20);
            caseEntity.ipg_DxCodeId1 = dxCode.ToEntityReference();

            var listForInit = new List<Entity>() { caseEntity, dxCode };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingValidateDxCode",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<ValidateDxCode>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }
    }
}
