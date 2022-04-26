using FakeXrmEasy;
using FakeXrmEasy.FakeMessageExecutors;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.Plugin.Gating.PostProcess;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace Insight.Intake.UnitTests.Plugin.Gating.PostProcess
{
    public class Gate2Tests : PluginTestsBase
    {
        [Fact]
        public void Gate2Tests_ResultSuccessfull_returnSuccess()
        {
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();

            var listForInit = new List<Entity>() { caseEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingPostProcessGate2",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            var wfExecutor = new FakeExecuteWorkflowRequest();
            fakedContext.AddFakeMessageExecutor<ExecuteWorkflowRequest>(wfExecutor);
            //ACT
            fakedContext.ExecutePluginWith<Gate2>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(wfExecutor.isExecuted);
        }
    }
    
}
