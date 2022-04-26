using FakeXrmEasy;
using Insight.Intake.Plugin.GateProcessingRule;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.GateProcessingRule
{
    public class CheckUniquenessTests : PluginTestsBase
    {
        [Fact]
        public void CheckUniqueness_RequiredFieldsIsMissing_returnSuccessTrue()
        {
            var fakedContext = new XrmFakedContext();

            Entity gateProcessingRule = new Entity("ipg_gateprocessingrule") { Id = Guid.NewGuid() };

            var listForInit = new List<Entity> { gateProcessingRule };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", gateProcessingRule.ToEntityReference() } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 40,
                PrimaryEntityName = gateProcessingRule.LogicalName,
                PrimaryEntityId = gateProcessingRule.Id,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            Assert.Throws<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<CheckUniqueness>(pluginContext));
        }
    }
}