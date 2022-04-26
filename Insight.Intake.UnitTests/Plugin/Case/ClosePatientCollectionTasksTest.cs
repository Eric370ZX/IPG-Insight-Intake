using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Insight.Intake.Helpers.Constants;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class ClosePatientCollectionTasksTest: PluginTestsBase
    {
        //patient collection was renamed to patient outreach
        [Fact]
        public void TestClosePatientCollectionTaskOnFullPatientPayment()
        {
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake().WithPatientCarrierBalance(0, 0);
            ipg_taskcategory taskCategory = new ipg_taskcategory().Fake().WithName(TaskCategoryNames.PatientOutreach);
            
            Task relTask = new Task().Fake().WithCase(caseEntity.ToEntityReference())
                .WithRegarding(caseEntity.ToEntityReference()).WithTaskCategory(taskCategory.ToEntityReference());
            Task relTask2 = new Task().Fake().WithCase(caseEntity.ToEntityReference())
                .WithRegarding(caseEntity.ToEntityReference()).WithTaskCategory(taskCategory.ToEntityReference());

            var listForInit = new List<Entity>() { caseEntity, relTask, relTask2, taskCategory};
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", caseEntity },
            };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            
            //ACT
            fakedContext.ExecutePluginWith<ClosePatientCollectionTasks>(pluginContext);
            var context = new CrmServiceContext(fakedContext.GetOrganizationService());
            var notCompletedTasks = (from task in context.TaskSet
                                     where task.StateCode != TaskState.Completed && task.StatusCodeEnum != Task_StatusCode.Resolved
                                     select task).ToList();

            //Assert
            Assert.Empty(notCompletedTasks);
        }
    }
}
