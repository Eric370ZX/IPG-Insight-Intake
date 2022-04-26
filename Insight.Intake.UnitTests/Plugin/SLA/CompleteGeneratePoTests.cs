using FakeXrmEasy;
using Insight.Intake.Plugin.SLA;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.SLA
{
    public class CompleteGeneratePoTests
    {
        [Fact]
        public void CheckGeneratePo()
        {
            DateTime createdDate = new DateTime(2020, 09, 01, 12, 00, 00);
            var fakedContext = new XrmFakedContext();

            Incident incident = new Incident()
                .Fake()
                .Generate();

            SalesOrder salesOrder = new SalesOrder()
                .Fake()
                    .WithCaseReference(incident)
                    .WithStatusCode(SalesOrder_StatusCode.Generated)
                .Generate();

            SalesOrder target = new SalesOrder()
                .Fake(salesOrder.Id)
                    .WithStatusCode(SalesOrder_StatusCode.Generated)
                .Generate();

            ipg_taskcategory taskCategory = new ipg_taskcategory().Fake();
            ipg_tasktype taskType = new ipg_tasktype().Fake()
                .WithName("SLA - Generate PO")
                .WithCategory(taskCategory)
                .WithStartDate(0)
                .WithDueDate(3);

            Task slaGeneratePoTask = new Task()
                .Fake()
                .WithSubject("SLA - Decision Retro Case")
                .WithRegarding(incident.ToEntityReference())
                .WithCase(incident.ToEntityReference())
                .WithTaskCategoryRef(taskCategory.ToEntityReference())
                .WithTypeRef(taskType.ToEntityReference())
                .WithScheduledEnd(createdDate.AddDays(3))
                .Generate();

            var fakedEntities = new List<Entity>() { incident, salesOrder, slaGeneratePoTask, taskCategory, taskType };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection { { "Target", target } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 40,
                PrimaryEntityName = target.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CompleteGeneratePo>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                var slaTask = context.TaskSet
                        .FirstOrDefault(t =>
                            t.RegardingObjectId.Id == incident.Id && t.ipg_tasktypeid.Id == taskType.Id &&
                            t.StateCode.Value == TaskState.Completed);

                Assert.NotNull(slaTask);
                Assert.Equal(slaTask.ActualEnd <= slaTask.ScheduledEnd ? "SLA Met" : "SLA Not Met", slaTask.Subcategory);
                Assert.Equal(TaskState.Completed, slaTask.StateCode);
            }
        }
    }
}
