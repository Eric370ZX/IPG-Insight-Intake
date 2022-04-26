using FakeXrmEasy;
using Insight.Intake.Plugin.Order;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests
{
    public class SetIsLockedOnActualPartsTests : PluginTestsBase
    {
        [Fact]
        public void SetIsLockedOnActualParts_POStateOpenStatusInReview_isLockedTrue()
        {
            // Arrange
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            SalesOrder order = new SalesOrder().Fake().WithStateCode(SalesOrderState.Active).WithStatusCode(SalesOrder_StatusCode.InReview);
            ipg_casepartdetail firstActualPart = new ipg_casepartdetail().Fake().WithOrderRef(order);
            firstActualPart.ipg_islocked = false;
            ipg_casepartdetail secondActualPart = new ipg_casepartdetail().Fake().WithOrderRef(order);
            secondActualPart.ipg_islocked = false;

            fakedContext.Initialize(new List<Entity>() { order, firstActualPart, secondActualPart });

            var postImages = new EntityImageCollection { { "PostImage", order } };

            var fakedPluginExecutionContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = SalesOrder.EntityLogicalName,
                PostEntityImages = postImages
            };

            // Act
            fakedContext.ExecutePluginWith<SetIsLockedOnActualParts>(fakedPluginExecutionContext);

            // Assert
            var query = new QueryExpression(ipg_casepartdetail.EntityLogicalName) { ColumnSet = new ColumnSet(ipg_casepartdetail.Fields.ipg_islocked) };
            var actualParts = fakedService.RetrieveMultiple(query).Entities?.Select(o => o.ToEntity<ipg_casepartdetail>());

            foreach (var part in actualParts)
            {
                Assert.True(part.ipg_islocked);
            }
        }

        [Fact]
        public void SetIsLockedOnActualParts_POStateOpenStatusUnlocked_isLockedFalse()
        {
            // Arrange
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            SalesOrder order = new SalesOrder().Fake().WithStateCode(SalesOrderState.Active).WithStatusCode(SalesOrder_StatusCode.Unlocked);
            ipg_casepartdetail firstActualPart = new ipg_casepartdetail().Fake().WithOrderRef(order);
            firstActualPart.ipg_islocked = false;
            ipg_casepartdetail secondActualPart = new ipg_casepartdetail().Fake().WithOrderRef(order);
            secondActualPart.ipg_islocked = false;

            fakedContext.Initialize(new List<Entity>() { order, firstActualPart, secondActualPart });

            var postImages = new EntityImageCollection { { "PostImage", order } };

            var fakedPluginExecutionContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = SalesOrder.EntityLogicalName,
                PostEntityImages = postImages
            };

            // Act
            fakedContext.ExecutePluginWith<SetIsLockedOnActualParts>(fakedPluginExecutionContext);

            // Assert
            var query = new QueryExpression(ipg_casepartdetail.EntityLogicalName) { ColumnSet = new ColumnSet(ipg_casepartdetail.Fields.ipg_islocked) };
            var actualParts = fakedService.RetrieveMultiple(query).Entities?.Select(o => o.ToEntity<ipg_casepartdetail>());

            foreach (var part in actualParts)
            {
                Assert.False(part.ipg_islocked);
            }
        }

        [Fact]
        public void SetIsLockedOnActualParts_RelatedPoIsDeleted_isLockedFalse()
        {
            //Arrange
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            SalesOrder order = new SalesOrder().Fake().WithStateCode(SalesOrderState.Active).WithStatusCode(SalesOrder_StatusCode.VerifiedforPayment);
            ipg_casepartdetail firstActualPart = new ipg_casepartdetail().Fake().WithOrderRef(order);
            firstActualPart.ipg_islocked = true;
            ipg_casepartdetail secondActualPart = new ipg_casepartdetail().Fake().WithOrderRef(order);
            secondActualPart.ipg_islocked = true;

            fakedContext.Initialize(new List<Entity>() { order, firstActualPart, secondActualPart });
            var inputParameters = new ParameterCollection { { "Target", order.ToEntityReference() } };
            var fakedPluginExecutionContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Delete,
                Stage = PipelineStages.PreValidation,
                PrimaryEntityName = SalesOrder.EntityLogicalName,
                InputParameters = inputParameters
            };

            // Act
            fakedContext.ExecutePluginWith<SetIsLockedOnActualParts>(fakedPluginExecutionContext);

            // Assert
            var query = new QueryExpression(ipg_casepartdetail.EntityLogicalName) { ColumnSet = new ColumnSet(ipg_casepartdetail.Fields.ipg_islocked) };
            var actualParts = fakedService.RetrieveMultiple(query).Entities?.Select(o => o.ToEntity<ipg_casepartdetail>());

            foreach (var part in actualParts)
            {
                Assert.False(part.ipg_islocked);
            }
        }
    }
}
