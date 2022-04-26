using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Xunit;


namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class CreateDefaultAuthRecordTests : PluginTestsBase
    {
        [Fact]
        public void AuthIsCreated_Success()
        {
            //ARRANGE

            Incident incident = new Incident()
                .Fake()
                .RuleFor(p => p.ipg_facilityauthnumber, p => "123");

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { incident });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PreEntityImages = null,
                PostEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<CreateDefaultAuthRecord>(pluginContext);

            //ASSERT
            Assert.NotNull(incident.ipg_AuthorizationId);
        }
        [Fact]
        public void AuthNotCreated_Success()
        {
            //ARRANGE

            Incident incident = new Incident()
                .Fake()
                .RuleFor(p => p.ipg_facilityauthnumber, p => null);

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { incident });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PreEntityImages = null,
                PostEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<CreateDefaultAuthRecord>(pluginContext);

            //ASSERT
            Assert.Null(incident.ipg_AuthorizationId);
        }
    }
}
