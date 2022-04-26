using FakeXrmEasy;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Insight.Intake.Plugin.Case;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class OnUpdateAllRecievedPluginTest: PluginTestsBase
    {
        [Fact]
        public void OnAllRecievedTrueUpdateTest()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();
            
            Incident incident = new Incident().Fake();
            incident.ipg_isallreceived = true;

            Task task = new Task().Fake().WithType(ipg_TaskType1.VerifyAllreceived).WithRegarding(incident.ToEntityReference());




            var listForInit = new List<Entity> { incident, task };

            fakedContext.Initialize(listForInit);

            #endregion

            #region Setup execution context

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", incident } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection() { { "PostImage", incident } },
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin

            fakedContext.ExecutePluginWith<OnUpdateAllRecievedPlugin>(pluginContext);

            #endregion

            #region Asserts
            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            task = fakedService.Retrieve(task.LogicalName, task.Id, new ColumnSet(true)).ToEntity<Task>();
            var log = crmContext.CreateQuery<ipg_gateactivity>().ToList();
            incident = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();

            Assert.Equal(Task_StatusCode.Resolved, task.StatusCodeEnum);
            Assert.Equal(TaskState.Completed, task.StateCode);
            Assert.Single(log);
            Assert.Equal(ipg_gateactivity_ipg_taskstatuscode.Completed, log.First().ipg_taskstatuscodeEnum);
            Assert.Equal("Verify All Received", log.First().Subject);
            Assert.Equal(incident.ModifiedBy, log.First().OwnerId);
            Assert.Equal(incident.ModifiedOn, log.First().ActualStart);

            #endregion
        }

        [Fact]
        public void OnAllRecievedFalseUpdateTest()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            Incident incident = new Incident().Fake();
            incident.ipg_isallreceived = false;

            Task task = new Task().Fake().WithType(ipg_TaskType1.VerifyAllreceived).WithRegarding(incident.ToEntityReference());




            var listForInit = new List<Entity> { incident, task };

            fakedContext.Initialize(listForInit);

            #endregion

            #region Setup execution context

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", incident } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection() { { "PostImage", incident } },
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin

            fakedContext.ExecutePluginWith<OnUpdateAllRecievedPlugin>(pluginContext);

            #endregion

            #region Asserts
            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            task = fakedService.Retrieve(task.LogicalName, task.Id, new ColumnSet(true)).ToEntity<Task>();
            var log = crmContext.CreateQuery<ipg_gateactivity>().ToList();
            incident = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();

            Assert.Equal(Task_StatusCode.InProgress, task.StatusCodeEnum);
            Assert.Equal(TaskState.Open, task.StateCode);
            Assert.Single(log);
            Assert.Equal(ipg_gateactivity_ipg_taskstatuscode.InProgress, log.First().ipg_taskstatuscodeEnum);
            Assert.Equal("Verify All Received", log.First().Subject);
            Assert.Equal(incident.ModifiedBy, log.First().OwnerId);
            Assert.Equal(incident.ModifiedOn, log.First().ActualStart);
            #endregion
        }
    }
}
