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
    public class CheckPropelNasalStentOnProcedureNameTests : PluginTestsBase
    {
        [Fact]
        public void CheckPropelNasalStentOnProcedureName_HasCPTCodes_returnSuccess()
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
                MessageName = "ipg_IPGGatingCheckPropelNasalStentOnProcedureName",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckPropelNasalStentOnProcedureName>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }
        [Fact]
        public void CheckPropelNasalStentOnProcedureName_HasProcedureNoPropel_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();


            ipg_procedurename procedurename = new ipg_procedurename().Fake();
            ipg_cptcode cptCode = new ipg_cptcode().Fake()
                .RuleFor(p => p.ipg_procedurename, k => procedurename.ToEntityReference());
            cptCode.ipg_PropelNasalStent = false;

            Incident caseEntity = new Incident().Fake()
                .RuleFor(p => p.ipg_procedureid, k => procedurename.ToEntityReference());


            var listForInit = new List<Entity>() { caseEntity, procedurename, cptCode };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckPropelNasalStentOnProcedureName",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckPropelNasalStentOnProcedureName>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }
        [Fact]
        public void CheckPropelNasalStentOnProcedureName_HasProcedureWithPropel_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();


            ipg_procedurename procedurename = new ipg_procedurename().Fake();
            ipg_cptcode cptCode = new ipg_cptcode().Fake()
                .RuleFor(p => p.ipg_procedurename, k => procedurename.ToEntityReference());
            cptCode.ipg_PropelNasalStent = true;

            Incident caseEntity = new Incident().Fake()
                .RuleFor(p => p.ipg_procedureid, k => procedurename.ToEntityReference());


            var listForInit = new List<Entity>() { caseEntity, procedurename, cptCode };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckPropelNasalStentOnProcedureName",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckPropelNasalStentOnProcedureName>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }
        [Fact]
        public void CheckPropelNasalStentOnProcedureName_NoProcedureName_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            caseEntity.ipg_procedureid = null;

            var listForInit = new List<Entity>() { caseEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckPropelNasalStentOnProcedureName",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckPropelNasalStentOnProcedureName>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }
    }
}