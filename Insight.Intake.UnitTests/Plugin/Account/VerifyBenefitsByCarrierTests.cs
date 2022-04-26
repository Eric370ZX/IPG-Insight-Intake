using FakeXrmEasy;
using Insight.Intake.Plugin.Account;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Insight.Intake.UnitTests.Plugin.Gating.PostProcess;
using Microsoft.Crm.Sdk.Messages;

namespace Insight.Intake.UnitTests.Plugin.Account
{
    public class VerifyBenefitsByCarrierTests : PluginTestsBase
    {
        [Fact]
        public void VerifyBenefitsByCarrierTests_Success()
        {
            var fakedContext = new XrmFakedContext();
            Intake.Account carrier = new Intake.Account().Fake();

            Incident caseEntity = new Incident().Fake()
                .WithCarrierReference(carrier);

            var listForInit = new List<Entity>() { caseEntity, carrier };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", carrier } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
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
            fakedContext.ExecutePluginWith<VerifyBenefitByCarrier>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(wfExecutor.isExecuted);
        }
    }
}
