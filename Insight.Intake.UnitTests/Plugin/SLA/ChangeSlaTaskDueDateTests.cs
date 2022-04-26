using FakeXrmEasy;
using Insight.Intake.Plugin.SLA;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.SLA
{
    public class ChangeSlaTaskDueDateTests : PluginTestsBase
    {
        [Fact]
        public void CheckSlaTaskDueDate()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            //Create fake task category, task type and task to update. Bind the task with task category and task type.
            DateTime scheduledStartDate = new DateTime(2020, 09, 08, 12, 00, 00);
            DateTime newScheduledStartDate = new DateTime(2020, 09, 09, 12, 00, 00);
            ipg_taskcategory taskCategory = new ipg_taskcategory().Fake();
            var taskCategoryRef = taskCategory.ToEntityReference();
            taskCategoryRef.Name = "SLA";
            ipg_tasktype taskType = new ipg_tasktype().Fake()
                .WithName("SLA - Create Referral")
                .WithCategory(taskCategory)
                .WithStartDate(0)
                .WithDueDate(1);
            var taskId = Guid.NewGuid();
            SystemUser systemUser = new SystemUser(Guid.NewGuid())
                .Fake("Test User");
            var taskToUpdate = new Task(taskId).Fake()
                .WithTaskCategoryRef(taskCategoryRef)
                .WithTypeRef(taskType.ToEntityReference())
                .WithScheduledStart(newScheduledStartDate)
                .Generate();
            var preImage = new Task(taskId).Fake()
                .WithTaskCategoryRef(taskCategoryRef)
                .WithTypeRef(taskType.ToEntityReference())
                .WithScheduledStart(scheduledStartDate)
                .WithOwner(systemUser.ToEntityReference())
                .Generate();

            

            var listForInit = new List<Entity> { systemUser, preImage, taskCategory, taskType };

            fakedContext.Initialize(listForInit);

            #endregion

            #region Setup execution context
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", taskToUpdate } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection() { { "PreImage", preImage } },
                InitiatingUserId = systemUser.Id
            };
            #endregion

            #region Execute plugin

            fakedContext.ExecutePluginWith<ChangeSlaTaskDueDate>(pluginContext);

            #endregion

            #region Asserts

            Assert.Equal(newScheduledStartDate.AddDays(1).Date, taskToUpdate.ScheduledEnd.Value.Date);

            #endregion
        }
    }
}
