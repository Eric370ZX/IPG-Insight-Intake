using FakeXrmEasy;
using Insight.Intake.Data;
using Insight.Intake.Models;
using Insight.Intake.Plugin.Authorization;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Insight.Intake.Plugin.Managers.TaskManager;

namespace Insight.Intake.UnitTests.Plugin.Authorization
{
    public class CompleteTasksTests
    {
        [Fact]
        public void CompletesTasks()
        {
            var fakedContext = new XrmFakedContext();

            var authorizationTaskType = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.AUTORIZATION_REQUIRED).Generate();
            var notAuthorizationTaskType = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.AUTORIZATION_REQUIRED_FOR_HPCS).Generate();

            var carrier = new Intake.Account()
                .Fake(CustomerTypeCodeOptionSet.Carrier)
                .Generate();
            var carrier2 = new Intake.Account()
                .Fake(CustomerTypeCodeOptionSet.Carrier)
                .Generate();

            var incident = new Incident().Fake().Generate();
            var incident2 = new Incident().Fake().Generate();

            var target = new ipg_authorization()
                .Fake()
                    .WithIncidentRef(incident.ToEntityReference())
                    .WithCarrier(carrier.ToEntityReference())
                .Generate();

            ipg_workflowtask wfTask = new ipg_workflowtask().Fake()
                .RuleFor(p => p.Id, p => WorkflowTasksConstants.PreauthorizationHCPCS);

            var task1 = new Task().Fake()
                .WithTypeRef(authorizationTaskType.ToEntityReference())
                .WithState(TaskState.Open)
                .WithStatus(Task_StatusCode.NotStarted)
                .WithRegarding(incident.ToEntityReference())
                .WithCarrier(carrier.ToEntityReference())
                .Generate();
            var task2 = new Task().Fake()
                .WithTypeRef(authorizationTaskType.ToEntityReference())
                .WithState(TaskState.Open)
                .WithStatus(Task_StatusCode.NotStarted)
                .WithRegarding(incident.ToEntityReference())
                .WithCarrier(carrier.ToEntityReference())
                .RuleFor(p => p.ipg_WorkflowTaskId, p => wfTask.ToEntityReference())
                .Generate();
            var task3 = new Task().Fake()
                .WithTypeRef(notAuthorizationTaskType.ToEntityReference())
                .WithState(TaskState.Open)
                .WithStatus(Task_StatusCode.NotStarted)
                .WithRegarding(incident.ToEntityReference())
                .WithCarrier(carrier.ToEntityReference())
                .Generate();
            var task4 = new Task().Fake()
                .WithTypeRef(authorizationTaskType.ToEntityReference())
                .WithState(TaskState.Open)
                .WithStatus(Task_StatusCode.NotStarted)
                .WithRegarding(incident2.ToEntityReference())
                .WithCarrier(carrier.ToEntityReference())
                .Generate();


            var fakedEntities = new List<Entity>() { authorizationTaskType, notAuthorizationTaskType, carrier, carrier2, incident, incident2, target, task1, task2, task3,  task4 };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection() { { "Target", target } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = target.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CompleteTasks>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                var task1FromContext = context.TaskSet.FirstOrDefault(x => x.Id == task1.Id);
                var task2FromContext = context.TaskSet.FirstOrDefault(x => x.Id == task2.Id);
                var task3FromContext = context.TaskSet.FirstOrDefault(x => x.Id == task3.Id);
                var task4FromContext = context.TaskSet.FirstOrDefault(x => x.Id == task4.Id);

                Assert.Equal(TaskState.Completed, task1FromContext.StateCode);
                Assert.Equal((int)Task_StatusCode.Resolved, task1FromContext.StatusCode.Value);

                Assert.Equal(TaskState.Completed, task2FromContext.StateCode);
                Assert.Equal((int)Task_StatusCode.Resolved, task2FromContext.StatusCode.Value);

                Assert.Equal(TaskState.Open, task3FromContext.StateCode);
                Assert.Equal((int)Task_StatusCode.NotStarted, task3FromContext.StatusCode.Value);

                Assert.Equal(TaskState.Open, task4FromContext.StateCode);
                Assert.Equal((int)Task_StatusCode.NotStarted, task4FromContext.StatusCode.Value);
            }
        }

    }
}
