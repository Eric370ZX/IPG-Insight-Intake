using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CheckClaimGenerationOverrideTests : PluginTestsBase
    {
        [Fact]
        public void CheckClaimGenerationOverrideTests_Success()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            ipg_cptcode cptCode = new ipg_cptcode().Fake();
            Intake.Incident incident = new Intake.Incident().Fake()
                .RuleFor(p => p.ipg_islocked, false);


            var listForInit = new List<Entity>() { incident };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckClaimGenerationOverride",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckClaimGenerationOverride>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }

        [Fact]
        public void CheckClaimGenerationOverrideTests_ReopenTasks()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident incident = new Incident().Fake();
            ipg_statementgenerationtask task1 = new ipg_statementgenerationtask().Fake().WithCaseReference(incident).RuleFor(p => p.StatusCode, new OptionSetValue((int)ipg_statementgenerationtask_StatusCode.Canceled)).RuleFor(p => p.ipg_StartDate, DateTime.Now);
            ipg_statementgenerationtask task2 = new ipg_statementgenerationtask().Fake().WithCaseReference(incident).RuleFor(p => p.StatusCode, new OptionSetValue((int)ipg_statementgenerationtask_StatusCode.Canceled)).RuleFor(p => p.ipg_StartDate, DateTime.Now.AddDays(-1));
            var cgo = new ipg_claimgenerationoverride()
            {
                Id = Guid.NewGuid(),
                ipg_caseid = incident.ToEntityReference(),
                ipg_claimtogenerate = ipg_claimgenerationoverride_ipg_claimtogenerate.NoClaim,
            };

            var listForInit = new List<Entity>() { incident, task1, task2, cgo };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckClaimGenerationOverride",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckClaimGenerationOverride>(pluginContext);

            //Assert
            var fakedService = fakedContext.GetOrganizationService();
            var task1Faked = fakedService.Retrieve(task1.LogicalName, task1.Id, new ColumnSet(nameof(ipg_statementgenerationtask.StatusCode).ToLower())).ToEntity<ipg_statementgenerationtask>();            
            Assert.Equal(task1Faked.StatusCode.Value, (int)ipg_statementgenerationtask_StatusCode.Open);
            var task2Faked = fakedService.Retrieve(task2.LogicalName, task2.Id, new ColumnSet(nameof(ipg_statementgenerationtask.StatusCode).ToLower())).ToEntity<ipg_statementgenerationtask>();
            Assert.Equal(task2Faked.StatusCode.Value, (int)ipg_statementgenerationtask_StatusCode.Canceled);
        }
    }
}
