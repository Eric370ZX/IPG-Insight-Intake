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
    public class CheckPropelNasalStentOnCPTTests : PluginTestsBase
    {
        [Fact]
        public void CheckPropelNasalStentOnCPT_NoCPTCodes_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();

            var listForInit = new List<Entity>() { caseEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckPropelNasalStentOnCPT",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckPropelNasalStentOnCPT>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }
        [Fact]
        public void CheckPropelNasalStentOnCPT_NoNasalStents_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();


            Insight.Intake.ipg_cptcode cptCode = new Intake.ipg_cptcode().Fake();
            cptCode.ipg_PropelNasalStent = false;
            Incident caseEntity = new Incident().Fake()
                .RuleFor(p => p.ipg_CPTCodeId1, p => cptCode.ToEntityReference());

            var listForInit = new List<Entity>() { caseEntity, cptCode };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckPropelNasalStentOnCPT",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckPropelNasalStentOnCPT>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }
        [Fact]
        public void CheckPropelNasalStentOnCPT_HasNasalStents_returnError()
        {
            //arrange
            var fakedContext = new XrmFakedContext();


            Insight.Intake.ipg_cptcode cptCode = new Intake.ipg_cptcode().Fake();
            cptCode.ipg_PropelNasalStent = true;
            Incident caseEntity = new Incident().Fake()
                .RuleFor(p => p.ipg_CPTCodeId1, p => cptCode.ToEntityReference());

            var listForInit = new List<Entity>() { caseEntity, cptCode };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckPropelNasalStentOnCPT",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckPropelNasalStentOnCPT>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }
    }
}