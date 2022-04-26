using FakeXrmEasy;
using Insight.Intake.Helpers;
using Insight.Intake.Plugin.TaskEntity;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.TaskEntity
{
    public class ManageCaseOnHoldByTaskTests
    {
        [Fact]
        public void CasePutOnHoldByTaskTest()
        {
            ipg_IPGCaseActionsHoldCaseRequest request = null;

            var fakedContext = new XrmFakedContext();
            fakedContext.AddExecutionMock<ipg_IPGCaseActionsHoldCaseRequest>(req => {
                request = req as ipg_IPGCaseActionsHoldCaseRequest;
                return new OrganizationResponse();});

            Incident incident = new Incident().Fake();
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithName("Facility Recovery - Research Pending");
            Task task = new Task().Fake().WithTypeRef(taskType.ToEntityReference()).WithCase(incident.ToEntityReference()).WithRegarding(incident.ToEntityReference());
            

            var listForInit = new List<Entity>() {taskType, task, incident };
            fakedContext.Initialize(listForInit);


            var inputParameters = new ParameterCollection { { "Target", task } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection(),
            };

            fakedContext.ExecutePluginWith<ManageCaseOnHoldByTask>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var updatedCase = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();

            Assert.Equal(true, request?.IsOnHold);

        }

        [Fact]
        public void CasePutUnOnHoldByTaskTest()
        {
            ipg_IPGCaseActionsHoldCaseRequest request = null;

            var fakedContext = new XrmFakedContext();
            fakedContext.AddExecutionMock<ipg_IPGCaseActionsHoldCaseRequest>(req => {
                request = req as ipg_IPGCaseActionsHoldCaseRequest;
                return new OrganizationResponse();
            });

            Incident incident = new Incident().Fake();
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithName("Facility Recovery - Research Pending");
            Task task = new Task().Fake().WithTypeRef(taskType.ToEntityReference())
                .WithCase(incident.ToEntityReference())
                .WithRegarding(incident.ToEntityReference())
                .WithState(TaskState.Completed);


            var listForInit = new List<Entity>() { taskType, task, incident };
            fakedContext.Initialize(listForInit);


            var inputParameters = new ParameterCollection { { "Target", task } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection() { { "PostImage",  task} },
                PreEntityImages = new EntityImageCollection(),
            };

            fakedContext.ExecutePluginWith<ManageCaseOnHoldByTask>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var updatedCase = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();

            Assert.Equal(false, request?.IsOnHold);
        }
    }
}
