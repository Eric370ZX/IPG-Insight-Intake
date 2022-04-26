using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class SetInitialGateConfigurationValueTest : PluginTestsBase
    {
        [Fact]
        public void CheckSetInitialGateConfigurationValuePreCreate_GateConfigurationIsNull()
        {
            var fakedContext = new XrmFakedContext();

            ipg_referral referral = new ipg_referral().Fake();

            var listForInit = new List<Entity> { referral };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", referral } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 10,
                PrimaryEntityName = ipg_referral.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<SetInitialGateConfigurationValue>(pluginContext);
            Assert.Null(referral.ipg_lifecyclestepid);
        }

        [Fact]
        public void CheckSetInitialGateConfigurationValuePreCreate_GateConfigurationIsNotNull()
        {
            var fakedContext = new XrmFakedContext();

            ipg_referral referral = new ipg_referral().Fake();
            var lifecyclestep = new ipg_lifecyclestep() { Id = Guid.NewGuid(), ipg_executionorder = 10 };

            var listForInit = new List<Entity> { referral, lifecyclestep };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", referral } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 10,
                PrimaryEntityName = ipg_referral.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<SetInitialGateConfigurationValue>(pluginContext);
            Assert.Equal(lifecyclestep.Id, referral.ipg_lifecyclestepid.Id);
        }
    }
}