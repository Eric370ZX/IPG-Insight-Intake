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
    public class GeneratePoTest
    {
        [Fact]
        public void CheckGeneratePo()
        {
            DateTime createdDate = new DateTime(2020, 09, 08, 12, 00, 00);
            DateTime allReceivedDate = new DateTime(2020, 09, 08, 12, 00, 00);

            var fakedContext = new XrmFakedContext();

            Guid incidentId = Guid.NewGuid();

            ipg_casestatusdisplayed caseStatusDisplayed = new ipg_casestatusdisplayed()
                .Fake()
                    .WithName("Procedure Complete - Queued for Billing")
                .Generate();

            Incident incident = new Incident()
                .Fake(incidentId)
                    .WithIsAllReceivedDate(allReceivedDate)
                    .WithCaseStatusDisplayed(caseStatusDisplayed)
                .Generate();

            Incident target = new Incident()
                .Fake(incidentId)
                    .WithCaseStatusDisplayed(caseStatusDisplayed)
                .Generate();

            ipg_taskcategory taskCategory = new ipg_taskcategory().Fake();
            ipg_tasktype taskType = new ipg_tasktype().Fake()
                .WithName("SLA - Generate PO")
                .WithCategory(taskCategory)
                .WithStartDate(0)
                .WithDueDate(3);

            var fakedEntities = new List<Entity>() { incident, caseStatusDisplayed, taskCategory, taskType };

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

            fakedContext.ExecutePluginWith<GeneratePo>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                var task = context.TaskSet
                    .FirstOrDefault(t =>
                        t.RegardingObjectId != null && t.RegardingObjectId.Id == incident.Id &&
                        t.ipg_tasktypeid != null && t.ipg_tasktypeid.Id == taskType.Id);


                Assert.NotNull(task);
                Assert.Equal("SLA - Generate PO", task.Subject);
                Assert.Equal(allReceivedDate, task.ScheduledStart);
                Assert.Equal(TaskState.Open, task.StateCode);
            }
        }
    }
}
