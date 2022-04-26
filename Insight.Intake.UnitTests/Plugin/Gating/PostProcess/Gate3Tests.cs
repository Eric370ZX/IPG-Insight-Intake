using FakeXrmEasy;
using Insight.Intake.Plugin.Gating.PostProcess;
using Insight.Intake.Repositories;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating.PostProcess
{
    public class Gate3Tests
    {
        [Fact]
        public void Gate3Tests_ResultSuccessfull_CreatesCaseAcceptedStatementTask()
        {
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            ipg_statementgenerationeventconfiguration taskconfig = new ipg_statementgenerationeventconfiguration().Fake(PSEvents.CaseApproved);

            var listForInit = new List<Entity>() { caseEntity, taskconfig };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() }, { "MinimumSeverityLevel", (int)ipg_SeverityLevel.Warning } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingPostProcessGate3",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<Gate3>(pluginContext);

            var service = fakedContext.GetOrganizationService();
            var context = new CrmServiceContext(service);
            var PSTask = (from pstasks in context.ipg_statementgenerationtaskSet
                          join configevent in context.ipg_statementgenerationeventconfigurationSet on pstasks.ipg_eventid.Id equals configevent.ipg_statementgenerationeventconfigurationId
                          where pstasks.ipg_caseid.Id == caseEntity.Id && configevent.ipg_name == PSEvents.CaseApproved
                          select pstasks).FirstOrDefault();

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.NotNull(PSTask);
        }

        [Fact]
        public void Gate3Tests_ResultFail_Not_CreatesCaseAcceptedStatementTask()
        {
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();

            var listForInit = new List<Entity>() { caseEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() }, { "MinimumSeverityLevel", (int)ipg_SeverityLevel.Error} };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingPostProcessGate3",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            var service = fakedContext.GetOrganizationService();
            var context = new CrmServiceContext(service);
            var PSTask = (from pstasks in context.ipg_statementgenerationtaskSet
                          where pstasks.ipg_caseid.Id == caseEntity.Id
                          where pstasks.ipg_caseid.Id == caseEntity.Id
                          select pstasks).FirstOrDefault();

            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.Null(PSTask);
        }
    }
}
