using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CheckActualProcedureDateTest : PluginTestsBase
    {
        [Fact]
        public void CheckActualProcedureDateTest_CaseCheckActualProcedureDateEmpty_returnError()
        {
            var fakedContext = new XrmFakedContext();
            Incident refCase = new Incident().Fake();
            var listForInit = new List<Entity>() { refCase };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", refCase.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckActualProcedureDate",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckActualProcedureDate>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }

        [Fact]
        public void CheckActualProcedureDateTest_CaseCheckActualProcedureDateFilled_returnSuccess()
        {
            var fakedContext = new XrmFakedContext();
            Incident refCase = new Incident().Fake();
            refCase["ipg_actualdos"] = DateTime.Now;
            var listForInit = new List<Entity>() { refCase };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", refCase.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckActualProcedureDate",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckActualProcedureDate>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }
    }
}
