using FakeXrmEasy;
using Insight.Intake.Plugin.Referral;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Referral
{
    public class PreventCreationNotFromInitiateReferralTests
    {
        [Fact]
        public void CreateFromInitiate()
        {
            var fakedContext = new XrmFakedContext();

            ipg_referral target = new ipg_referral()
            {
                Id = Guid.NewGuid(),
                ipg_isinitiatereferral = true
            };

            var fakedEntities = new List<Entity>() { target };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection { { "Target", target } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = target.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            try
            {
                fakedContext.ExecutePluginWith<PreventCreationNotFromInitiateReferral>(pluginContext);

                //No exceptions during plug-in execution
                //Everything fine
                Assert.True(true);
            }
            catch (Exception)
            {
                //If any exception occured, test not passed
                Assert.True(false);
            }
        }

        [Fact]
        public void CreateNotFromInitiate()
        {
            var fakedContext = new XrmFakedContext();

            ipg_referral target = new ipg_referral()
            {
                Id = Guid.NewGuid()
            };

            var fakedEntities = new List<Entity>() { target };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection { { "Target", target } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = target.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            try
            {
                fakedContext.ExecutePluginWith<PreventCreationNotFromInitiateReferral>(pluginContext);

                //No exception means plug-in didn't work as excpected
                Assert.True(false);
            }
            catch (InvalidPluginExecutionException)
            {
                //Exception should be fired in this test
                Assert.True(true);
            }
        }
    }
}