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
    public class CheckCPTorProcedureNameTests : PluginTestsBase
    {
        [Fact]
        public void Referral_ProcedureName()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            ipg_procedurename procedureName = new ipg_procedurename().Fake();
            ipg_referral referral = new ipg_referral().Fake()
                .RuleFor(p => p.ipg_ProcedureNameId, p => procedureName.ToEntityReference());

            var listForInit = new List<Entity>() { procedureName, referral };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckCPTorProcedureName",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckCPTorProcedureName>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
            Assert.True(pluginContext.OutputParameters["CodeOutput"] as int? == 1);
        }

        [Fact]
        public void Case_ProcedureName()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            ipg_procedurename procedureName = new ipg_procedurename().Fake();
            Incident incident = new Incident().Fake()
                .RuleFor(p => p.ipg_procedureid, p => procedureName.ToEntityReference());

            var listForInit = new List<Entity>() { procedureName, incident };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckCPTorProcedureName",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckCPTorProcedureName>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
            Assert.True(pluginContext.OutputParameters["CodeOutput"] as int? == 1);
        }

        [Fact]
        public void Referral_CPTCode()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Insight.Intake.ipg_cptcode cptCode = new Intake.ipg_cptcode().Fake();
            ipg_referral referral = new ipg_referral().Fake()
                .RuleFor(p => p.ipg_CPTCodeId1, p => cptCode.ToEntityReference());

            var listForInit = new List<Entity>() { cptCode, referral };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckCPTorProcedureName",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckCPTorProcedureName>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
            Assert.True(pluginContext.OutputParameters["CodeOutput"] as int? == 2);
        }

        [Fact]
        public void Incident_CPTCode()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Insight.Intake.ipg_cptcode cptCode = new Intake.ipg_cptcode().Fake();
            Incident referral = new Incident().Fake()
                .RuleFor(p => p.ipg_CPTCodeId1, p => cptCode.ToEntityReference());

            var listForInit = new List<Entity>() { cptCode, referral };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckCPTorProcedureName",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckCPTorProcedureName>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
            Assert.True(pluginContext.OutputParameters["CodeOutput"] as int? == 2);
        }

        [Fact]
        public void Referral_NegativeResult()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            ipg_referral referral = new ipg_referral().Fake();

            var listForInit = new List<Entity>() { referral };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", referral.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckCPTorProcedureName",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckCPTorProcedureName>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }

        [Fact]
        public void Incident_NegativeResult()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident incident = new Incident().Fake();

            var listForInit = new List<Entity>() { incident };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckCPTorProcedureName",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckCPTorProcedureName>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }
    }
}
