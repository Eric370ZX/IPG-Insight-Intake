using FakeXrmEasy;
using Insight.Intake.Plugin.BVF;
using Insight.Intake.UnitTests.Fakes;
using System.Collections.Generic;
using Xunit;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.UnitTests.Plugin.BVF
{
    public class PopulatePrimaryAttributeTest : PluginTestsBase
    {
        [Fact]
        public void GeneratesName()
        {
            //ARRANGE
            var bvf = new ipg_benefitsverificationform().Fake().Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { bvf });

            var inputParameters = new ParameterCollection { { "Target", bvf } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = ipg_benefitsverificationform.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<PopulatePrimaryAttributePlugin>(pluginContext);

            //ASSERT
            var updatedBvf = ((Entity)pluginContext.InputParameters["Target"]).ToEntity<ipg_benefitsverificationform>();
            Assert.Equal($"BVF - {DateTime.Now.ToString("MMddyyyy")}", updatedBvf.ipg_name);
        }
    }
}
