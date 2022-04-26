using FakeXrmEasy;
using Insight.Intake.Models;
using Insight.Intake.Plugin.Case;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class GatingCompletedPluginTest : PluginTestsBase
    {
        [Fact]
        public void GatingCompletedPluginTest_CaseClosed()
        {
            var fakedContext = new XrmFakedContext();
            Incident targetIncident = new Incident().Fake();
            targetIncident.ipg_gateoutcome = new OptionSetValue((int)ipg_SeverityLevel.Error);

            Incident postImageIncident = new Incident().Fake();
            postImageIncident.ipg_ehrupdatestatus = new OptionSetValue((int)Incident_ipg_ehrupdatestatus.EHRclosedcasererun);

            var listForInit = new List<Entity>() { targetIncident };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", targetIncident } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };
            var postImage = new EntityImageCollection { { "PostImage", postImageIncident } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = postImage,
                PreEntityImages = null
            };
            fakedContext.ExecutePluginWith<GatingCompletedPlugin>(pluginContext);

            var service = fakedContext.GetFakedOrganizationService();
            var resultIncident = service
                .Retrieve(Incident.EntityLogicalName, targetIncident.Id, new ColumnSet(true))
                .ToEntity<Incident>();

            Assert.Null(resultIncident.ipg_ehrupdatestatus);
            Assert.Equal(Incident_StatusCode.Canceled, resultIncident.StatusCodeEnum);
            Assert.Equal(IncidentState.Canceled, resultIncident.StateCode);
            Assert.Equal(ipg_CaseStatus.Closed, resultIncident.ipg_CaseStatusEnum);
        }
    }
}
