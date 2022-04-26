using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CheckCriticalFieldsLastChangeDateTests : PluginTestsBase
    {
        [Fact]
        public void CheckCriticalFieldsLastChangeDateTests_CriticalFieldsLastChangeDateIsEmpty_Error()
        {
            /// Arrange
            var fakedContext = new XrmFakedContext();
            Incident caseEntity = new Incident().Fake();
            var listForInit = new List<Entity>() { caseEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckCriticalFieldsLastChangeDate",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            /// Act
            fakedContext.ExecutePluginWith<CheckCriticalFieldsLastChangeDate>(pluginContext);

            /// Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }

        [Fact]
        public void CheckCriticalFieldsLastChangeDateTests_CalcRevUpdateNotRequired_Success()
        {
            /// Arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            caseEntity.ipg_lastcalcrevon = DateTime.Now.AddDays(5);
            caseEntity.ipg_criticalfieldslastchangedate = DateTime.Now.AddDays(2);

            var listForInit = new List<Entity>() { caseEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckCriticalFieldsLastChangeDate",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            /// Act
            fakedContext.ExecutePluginWith<CheckCriticalFieldsLastChangeDate>(pluginContext);

            /// Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CheckCriticalFieldsLastChangeDateTests_NewCalcRevRequired_Error()
        {
            /// Arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            caseEntity.ipg_lastcalcrevon = DateTime.Now.AddDays(2);
            caseEntity.ipg_criticalfieldslastchangedate = DateTime.Now.AddDays(5);

            var listForInit = new List<Entity>() { caseEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckCriticalFieldsLastChangeDate",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            /// Act
            fakedContext.ExecutePluginWith<CheckCriticalFieldsLastChangeDate>(pluginContext);

            /// Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }
    }
}