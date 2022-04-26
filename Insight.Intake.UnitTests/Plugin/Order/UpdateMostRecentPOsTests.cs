using FakeXrmEasy;
using Insight.Intake.Plugin.Order;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Order
{
    public class UpdateMostRecentPOsTests : PluginTestsBase
    {
        [Fact]
        public void GenerateNewPO_ShouldUpdatePreviousPOsMostRecentValueToFalse()
        {
            // Arrange
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            Incident fakedCase = new Incident().Fake();

            SalesOrder fakedTargetPO = new SalesOrder().Fake().WithPoTypeCode(ipg_PurchaseOrderTypes.CPA);
            fakedTargetPO.ipg_CaseId = fakedCase.ToEntityReference();
            fakedTargetPO.ipg_MostRecent = true;

            SalesOrder fakedMostRecentPO = new SalesOrder().Fake().WithPoTypeCode(ipg_PurchaseOrderTypes.CPA);
            fakedMostRecentPO.ipg_CaseId = fakedCase.ToEntityReference();
            fakedMostRecentPO.ipg_MostRecent = true;

            SalesOrder fakedOldPO = new SalesOrder().Fake().WithPoTypeCode(ipg_PurchaseOrderTypes.CPA);
            fakedOldPO.ipg_CaseId = fakedCase.ToEntityReference();
            fakedOldPO.ipg_MostRecent = false;

            
            fakedContext.Initialize(new List<Entity>() { fakedCase, fakedTargetPO, fakedMostRecentPO, fakedOldPO });

            var inputParameters = new ParameterCollection { { "Target", fakedTargetPO } };

            var fakedPluginExecutionContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = SalesOrder.EntityLogicalName,
                InputParameters = inputParameters
            };

            // Act
            fakedContext.ExecutePluginWith<UpdateMostRecentPOs>(fakedPluginExecutionContext);

            // Assert
            var query = new QueryExpression(SalesOrder.EntityLogicalName) { ColumnSet = new ColumnSet(SalesOrder.Fields.ipg_MostRecent) }; // replace hardcoded field
            var orders = fakedService.RetrieveMultiple(query).Entities?.Select(o => o.ToEntity<SalesOrder>());

            Assert.NotNull(orders);

            foreach (var order in orders)
            {
                if (order.Id == fakedTargetPO.Id)
                {
                    Assert.True(order.ipg_MostRecent);
                    continue;
                }

                Assert.False(order.ipg_MostRecent);
            }
        }

        [Fact]
        public void GenerateNewPOWithoutRelatedCase_ShouldThrowException()
        {
            // Arrange
            var fakedContext = new XrmFakedContext();

            SalesOrder fakedTargetPO = new SalesOrder().Fake();
            fakedTargetPO.ipg_CaseId = null;

            var fakedPluginExecutionContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = SalesOrder.EntityLogicalName,
                InputParameters = new ParameterCollection { { "Target", fakedTargetPO } }
            };

            // Act & Assert
            Exception ex = Assert.Throws<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<UpdateMostRecentPOs>(fakedPluginExecutionContext));
            Assert.Equal("Error: Purchase Order 'ipg_CaseId' field cannot be null.", ex.Message);
        }
    }
}
