using FakeXrmEasy;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.CasePartDetail
{
    public class RemoveTests
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
                .Generate();

            ipg_casepartdetail partDetail = new ipg_casepartdetail()
                .Fake()
                    .WithCaseReference(incident)
                    .WithOrderReference(order.ToEntityReference())
                .Generate();

            var fakedEntities = new List<Entity>() { incident, order, partDetail };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection() { { "Target", partDetail.ToEntityReference() } };
            var outputParameters = new ParameterCollection() { { "IsSuccess", false }, { "Message", "" } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_CasePartDetailActionRemove",
                Stage = 40,
                PrimaryEntityName = partDetail.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<Intake.Plugin.CasePartDetail.Remove>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                partDetail = context.ipg_casepartdetailSet.FirstOrDefault(x => x.Id == partDetail.Id);

                Assert.True(outputParameters["IsSuccess"] is false);

                Assert.NotNull(partDetail);
            }
        }

        [Fact]
        public void Void_NotPaidCpaPo()
        {
            var fakedContext = new XrmFakedContext();

            Incident incident = new Incident()
                .Fake()
                .Generate();

            ipg_casepartdetail partDetail = new ipg_casepartdetail()
                .Fake()
                    .WithCaseReference(incident)
                .Generate();

            var fakedEntities = new List<Entity>() { incident, partDetail };

            fakedContext.Initialize(fakedEntities);

            var inputParameters = new ParameterCollection() { { "Target", partDetail.ToEntityReference() } };
            var outputParameters = new ParameterCollection() { { "IsSuccess", false }, { "Message", "" } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_CasePartDetailActionRemove",
                Stage = 40,
                PrimaryEntityName = partDetail.LogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<Intake.Plugin.CasePartDetail.Remove>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            using (CrmServiceContext context = new CrmServiceContext(fakedService))
            {
                partDetail = context.ipg_casepartdetailSet.FirstOrDefault(x => x.Id == partDetail.Id);

                Assert.True(outputParameters["IsSuccess"] is true);

                Assert.Null(partDetail);
            }
        }
    }
}
