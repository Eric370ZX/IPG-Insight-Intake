using FakeXrmEasy;
using Insight.Intake.Helpers;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.CasePartDetail
{
    public class VoidTests
    {
        [Fact]
        public void Void_PaidCpaPo()
        {
            var fakedContext = new XrmFakedContext();

            Incident incident = new Incident()
                .Fake()
                .Generate();

            SalesOrder order = new SalesOrder()
                .Fake()
                    .WithCaseReference(incident)
                    .WithStatusCode(SalesOrder_StatusCode.Paid)
                .Generate();

            ipg_casepartdetail partDetail = new ipg_casepartdetail()
                .Fake()
                    .WithCaseReference(incident)
                    .WithPOType((int)ipg_PurchaseOrderTypes.CPA)
                    .WithOrderReference(order.ToEntityReference())
                .Generate();

            Task submitClaimTask = new Task()
                .FakeSubmitGenerateClaim(incident.Id)
                .Generate();

            ipg_taskcategory caseProcessingCategory = new ipg_taskcategory()
                .Fake()
                    .WithName("Case Processing")
                .Generate();

            var fakedEntities = new List<Entity>() { incident, order, partDetail, submitClaimTask, caseProcessingCategory };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection() { { "Target", partDetail.ToEntityReference() } };
            var outputParameters = new ParameterCollection() { { "IsSuccess", false }, { "Message", "" } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_CasePartDetailActionVoid",
                Stage = 40,
                PrimaryEntityName = partDetail.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<Intake.Plugin.CasePartDetail.Void>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                Task debitAgainstCpa = context.TaskSet.FirstOrDefault(x => x.ipg_caseid.Id == incident.Id && x.Subject == TaskHelper.GenerateDebitAgainstCpaSubject);

                partDetail = context.ipg_casepartdetailSet.FirstOrDefault(x => x.Id == partDetail.Id);
                order = context.SalesOrderSet.FirstOrDefault(x => x.Id == order.Id);
                submitClaimTask = context.TaskSet.FirstOrDefault(x => x.Id == submitClaimTask.Id);

                Assert.True(outputParameters["IsSuccess"] is bool);

                Assert.Equal(0, partDetail.ipg_quantity);
                Assert.Equal((int)ipg_casepartdetail_StatusCode.Inactive, partDetail.StatusCode.Value);
                Assert.Equal((int)SalesOrder_StatusCode.Voided, order.StatusCode.Value);
                Assert.Equal((int)Task_StatusCode.PartChanges, submitClaimTask.StatusCode.Value);
                Assert.NotNull(debitAgainstCpa);
            }
        }

        [Fact]
        public void Void_NotPaidCpaPo()
        {
            var fakedContext = new XrmFakedContext();

            Incident incident = new Incident()
                .Fake()
                .Generate();

            SalesOrder order = new SalesOrder()
                .Fake()
                    .WithCaseReference(incident)
                    .WithStatusCode(SalesOrder_StatusCode.Generated)
                .Generate();

            ipg_casepartdetail partDetail = new ipg_casepartdetail()
                .Fake()
                    .WithCaseReference(incident)
                    .WithPOType((int)ipg_PurchaseOrderTypes.CPA)
                    .WithOrderReference(order.ToEntityReference())
                .Generate();

            Task submitClaimTask = new Task()
                .FakeSubmitGenerateClaim(incident.Id)
                .Generate();

            ipg_taskcategory caseProcessingCategory = new ipg_taskcategory()
                .Fake()
                    .WithName("Case Processing")
                .Generate();

            var fakedEntities = new List<Entity>() { incident, order, partDetail, submitClaimTask, caseProcessingCategory };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection() { { "Target", partDetail.ToEntityReference() } };
            var outputParameters = new ParameterCollection() { { "IsSuccess", false }, { "Message", "" } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_CasePartDetailActionVoid",
                Stage = 40,
                PrimaryEntityName = partDetail.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<Intake.Plugin.CasePartDetail.Void>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                Task debitAgainstCpa = context.TaskSet.FirstOrDefault(x => x.ipg_caseid.Id == incident.Id && x.Subject == TaskHelper.GenerateDebitAgainstCpaSubject);

                partDetail = context.ipg_casepartdetailSet.FirstOrDefault(x => x.Id == partDetail.Id);
                order = context.SalesOrderSet.FirstOrDefault(x => x.Id == order.Id);
                submitClaimTask = context.TaskSet.FirstOrDefault(x => x.Id == submitClaimTask.Id);

                Assert.True(outputParameters["IsSuccess"] is bool);

                Assert.Equal(0, partDetail.ipg_quantity);
                Assert.Equal((int)ipg_casepartdetail_StatusCode.Inactive, partDetail.StatusCode.Value);
                Assert.Equal((int)SalesOrder_StatusCode.Voided, order.StatusCode.Value);
                Assert.Equal((int)Task_StatusCode.PartChanges, submitClaimTask.StatusCode.Value);

                Assert.Null(debitAgainstCpa);
            }
        }
    }
}
