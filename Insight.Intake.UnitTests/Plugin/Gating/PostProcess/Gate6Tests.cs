using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.Plugin.Gating.PostProcess;
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
    public class Gate6Tests : PluginTestsBase
    {
        [Fact]
        public void Gate6Tests_ResultSuccessfull_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();

            var listForInit = new List<Entity>() { caseEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingPostProcessGate6",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<Gate6>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void Gate6Tests_HasTask__returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();

            var listForInit = new List<Entity>() { caseEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingPostProcessGate6",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<Gate6>(pluginContext);
            var tasks = fakedContext.CreateQuery<Insight.Intake.Task>().ToList();
            //Assert
            Assert.False(tasks.Any());
        }

        [Fact]
        public void Gate6Tests_SetLockCase()
        {
            /// Arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            caseEntity.Id = Guid.NewGuid();

            var listForInit = new List<Entity>() { caseEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingPostProcessGate6",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            /// Act
            fakedContext.ExecutePluginWith<Gate6>(pluginContext);
            var result = fakedContext.CreateQuery<Incident>()
                .Where(incident => incident.Id == caseEntity.Id)
                .Where(incident => incident.ipg_islocked != true)
                .FirstOrDefault();

            /// Assert
            Assert.True(result != null);
        }
    }
}
