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
    public class PayProviderTests
    {
        [Fact]
        public void CheckPayProvider()
        {
            DateTime createdDate = new DateTime(2020, 09, 08, 12, 00, 00);

            var fakedContext = new XrmFakedContext();

            Guid salesOrderId = Guid.NewGuid();

            Incident incident = new Incident()
                .Fake()
                .Generate();

            SalesOrder salesOrder = new SalesOrder()
                .Fake(salesOrderId)
                    .WithCaseReference(incident)
                    .WithStatusCode(SalesOrder_StatusCode.CommtoFacility)
                .Generate();

            SalesOrder target = new SalesOrder()
                .Fake(salesOrderId)
                    .WithStatusCode(SalesOrder_StatusCode.CommtoFacility)
                .Generate();

            ipg_taskcategory taskCategory = new ipg_taskcategory().Fake();
            ipg_tasktype taskType = new ipg_tasktype().Fake()
                .WithName("SLA - Pay Provider")
                .WithCategory(taskCategory)
                .WithStartDate(0)
                .WithDueDate(30);

            var fakedEntities = new List<Entity>() { incident, salesOrder, taskCategory, taskType };

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

            fakedContext.ExecutePluginWith<PayProvider>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                var task = context.TaskSet
                        .FirstOrDefault(t =>
                            t.RegardingObjectId != null && t.RegardingObjectId.Id == salesOrder.ipg_CaseId.Id &&
                            t.ipg_tasktypeid != null && t.ipg_tasktypeid.Id == taskType.Id);

                Assert.NotNull(task);
                Assert.Equal("SLA - Pay Provider", task.Subject);
                Assert.Equal(TaskState.Open, task.StateCode);
            }
        }
    }
}
