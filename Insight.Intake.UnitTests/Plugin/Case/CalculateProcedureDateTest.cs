using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class CalculateProcedureDateTest : PluginTestsBase
    {
        [Fact]
        public void ActualAndScheduleDOSNotEmptyCreate()
        {
            //ARRANGE
            var date = System.DateTime.Today;
            var incident = new Incident().Fake()
                    .WithActualDos(date)
                    .WithScheduledDos(date.AddDays(1))
                    .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { incident });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters
            };

            //ACT
            fakedContext.ExecutePluginWith<CalculateProcedureDate>(pluginContext);

            //ASSERT
            Assert.Equal(incident.ipg_ProcedureDateNew, date);
        }

        [Fact]
        public void ActualDOSEmptyCreate()
        {
            //ARRANGE
            var date = System.DateTime.Today;
            var incident = new Incident().Fake()
                    .WithScheduledDos(date)
                    .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { incident });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters
            };

            //ACT
            fakedContext.ExecutePluginWith<CalculateProcedureDate>(pluginContext);

            //ASSERT
            Assert.Equal(incident.ipg_ProcedureDateNew, date);
        }

        [Fact]
        public void ActualAndScheduleDOSNotEmptyUpdate()
        {
            //ARRANGE
            var date = System.DateTime.Today;
            var incident = new Incident().Fake()
                    .WithActualDos(date)
                    .WithScheduledDos(date.AddDays(1))
                    .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { incident });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters
            };

            //ACT
            fakedContext.ExecutePluginWith<CalculateProcedureDate>(pluginContext);

            //ASSERT
            Assert.Equal(incident.ipg_ProcedureDateNew, date);
        }

        [Fact]
        public void ActualDOSEmptyUpdate()
        {
            //ARRANGE
            var date = System.DateTime.Today;
            var incident = new Incident().Fake()
                    .WithScheduledDos(date)
                    .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { incident });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters
            };

            //ACT
            fakedContext.ExecutePluginWith<CalculateProcedureDate>(pluginContext);

            //ASSERT
            Assert.Equal(incident.ipg_ProcedureDateNew, date);
        }
    }
}
