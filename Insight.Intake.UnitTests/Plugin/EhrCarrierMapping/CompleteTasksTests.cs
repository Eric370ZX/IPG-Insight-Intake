using FakeXrmEasy;
using Insight.Intake.Plugin.EhrCarrierMapping;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Insight.Intake.Plugin.Managers.TaskManager;

namespace Insight.Intake.UnitTests.Plugin.EhrCarrierMapping
{
    public class CompleteTasksTests
    {
        [Fact]
        public void CompletesTasks()
        {
            var fakedContext = new XrmFakedContext();

            var ehrMappingTaskType = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.UNKNOWN_FACILITY_CARRIER_COMBINATION_FROM_EHR).Generate();
            var notEhrMappingTaskType = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.MISSING_REQUIRED_FACILLITY_ACCOUNT_NUMBER).Generate();

            var targetMapping = new ipg_ehrcarriermap()
                .Fake()
                    .WithStatus(ipg_EHRCarrierMappingStatuses.Complete)
                .Generate();
            var notTargetMapping = new ipg_ehrcarriermap()
                .Fake()
                .Generate();

            var task1 = new Task().Fake()
                .WithTypeRef(ehrMappingTaskType.ToEntityReference())
                .WithState(TaskState.Open)
                .WithStatus(Task_StatusCode.NotStarted)
                .WithRegarding(targetMapping.ToEntityReference())
                .Generate();
            var task2 = new Task().Fake()
                .WithTypeRef(notEhrMappingTaskType.ToEntityReference())
                .WithState(TaskState.Open)
                .WithStatus(Task_StatusCode.NotStarted)
                .WithRegarding(targetMapping.ToEntityReference())
                .Generate();
            var task3 = new Task().Fake()
                .WithTypeRef(ehrMappingTaskType.ToEntityReference())
                .WithState(TaskState.Open)
                .WithStatus(Task_StatusCode.NotStarted)
                .WithRegarding(notTargetMapping.ToEntityReference())
                .Generate();
            
            var fakedEntities = new List<Entity>() { ehrMappingTaskType, notEhrMappingTaskType, targetMapping, notTargetMapping, task1, task2, task3 };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection() { { "Target", targetMapping } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = targetMapping.LogicalName,
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

                Assert.Equal(TaskState.Completed, task1FromContext.StateCode);
                Assert.Equal((int)Task_StatusCode.Resolved, task1FromContext.StatusCode.Value);

                Assert.Equal(TaskState.Open, task2FromContext.StateCode);
                Assert.Equal((int)Task_StatusCode.NotStarted, task2FromContext.StatusCode.Value);

                Assert.Equal(TaskState.Open, task3FromContext.StateCode);
                Assert.Equal((int)Task_StatusCode.NotStarted, task3FromContext.StatusCode.Value);
            }
        }

    }
}
