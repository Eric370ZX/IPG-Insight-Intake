using FakeXrmEasy;
using Insight.Intake.Plugin.Benefit;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Benefit
{
    public class CreateImportantEventLogTests : PluginTestsBase
    {
        [Fact]
        public void CreateImportantEventLogOnBenefitCreation()
        {
            //ARRANGE
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            var incident = new Incident().Fake().Generate();
            var benefit = new ipg_benefit().Fake()
                                           .WithBenefitSource(ipg_BenefitSources.EBV)
                                           .WithCaseReference(incident)
                                           .Generate();

            Incident fakedCase = new Incident().Fake();

            SalesOrder fakedSalesOrder = new SalesOrder().Fake().WithCaseReference(fakedCase);
            fakedSalesOrder.Name = "CIPG600456-12345";
            fakedSalesOrder.StatusCode = new OptionSetValue((int)SalesOrder_StatusCode.Generated);
            fakedSalesOrder.ipg_potypecode = new OptionSetValue((int)ipg_PurchaseOrderTypes.CPA);

            ipg_importanteventconfig fakedImportantEventConfig = new ipg_importanteventconfig()
            {
                Id = new Guid(),
                ipg_name = "ET2",
                ipg_eventdescription = "Benefits verified from <benefit record source>",
                ipg_eventtype = "Benefits Verified"
            };

            var fakedActivityType = new ipg_activitytype()
            {
                Id = new Guid(),
                ipg_name = "Benefits Verified"
            };

            fakedService.Create(fakedImportantEventConfig);
            fakedService.Create(fakedActivityType);
            var listForInit = new List<Entity>() { incident, benefit };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", benefit } };

            var fakedPluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_benefit.EntityLogicalName,
                InputParameters = inputParameters,
                PreEntityImages = null,
                PostEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<CreateImportantEventLog>(fakedPluginContext);
            var query = new QueryExpression(ipg_importanteventslog.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(false)
            };
            var resultLog = fakedService.RetrieveMultiple(query).Entities.SingleOrDefault();

            //ASSERT
            Assert.NotNull(resultLog);
        }
    }
}