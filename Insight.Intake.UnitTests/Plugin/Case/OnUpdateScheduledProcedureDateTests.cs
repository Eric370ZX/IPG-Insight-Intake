using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class OnUpdateScheduledProcedureDateTests : PluginTestsBase
    {
        private const string IntakeStep2Id = "871532c4-d3c1-e911-a983-000d3a37043b";
        private const string AuthorizationId = "20a244cb-d3c1-e911-a983-000d3a37043b";
        [Fact]
        public void UpdateScheduledProcedureDate_FromPortal_updateCase()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            ipg_lifecyclestep lifecycleStep = new Entity(ipg_lifecyclestep.EntityLogicalName, new Guid(IntakeStep2Id)).ToEntity<ipg_lifecyclestep>();
            Incident preImage = new Incident().Fake().WithScheduledDos(DateTime.Now).WithLFStep(lifecycleStep).WithState((int)ipg_CaseStateCodes.Intake);
            Incident incident = new Incident().Fake().WithScheduledDos(DateTime.Now.AddDays(5));
            incident.ipg_isportalrequest = true;

            var listForInit = new List<Entity> { lifecycleStep, preImage, incident };

            fakedContext.Initialize(listForInit);

            #endregion

            #region Setup execution context

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", incident } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection() { { "PreImage", preImage } },
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin

            fakedContext.ExecutePluginWith<OnUpdateScheduledProcedureDate>(pluginContext);

            #endregion

            #region Asserts

            var updatedIncident = incident;

            Assert.Equal(new Guid(AuthorizationId), updatedIncident.ipg_lifecyclestepid.Id);
            Assert.Equal((int)ipg_CaseStateCodes.Authorization, updatedIncident.ipg_StateCode.Value);
            Assert.Equal(false, updatedIncident.ipg_isportalrequest);
            #endregion
        }

        [Fact]
        public void UpdateScheduledProcedureDate_NotFromPortal_caseNotUpdated()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            ipg_lifecyclestep lifecycleStep = new Entity(ipg_lifecyclestep.EntityLogicalName, new Guid(IntakeStep2Id)).ToEntity<ipg_lifecyclestep>();
            Incident preImage = new Incident().Fake().WithScheduledDos(DateTime.Now).WithLFStep(lifecycleStep).WithState((int)ipg_CaseStateCodes.Intake);
            preImage.ipg_isportalrequest = false;
            Incident incident = new Incident().Fake().WithScheduledDos(DateTime.Now.AddDays(5));

            var listForInit = new List<Entity> { lifecycleStep, preImage, incident };

            fakedContext.Initialize(listForInit);

            #endregion

            #region Setup execution context

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", incident } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection() { { "PreImage", preImage } },
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin

            fakedContext.ExecutePluginWith<OnUpdateScheduledProcedureDate>(pluginContext);

            #endregion

            #region Asserts

            var updatedIncident = incident;

            Assert.Null(updatedIncident.ipg_lifecyclestepid);
            Assert.Null(updatedIncident.ipg_StateCode);
            Assert.Equal(false, updatedIncident.ipg_isportalrequest);
            #endregion
        }
    }
}
