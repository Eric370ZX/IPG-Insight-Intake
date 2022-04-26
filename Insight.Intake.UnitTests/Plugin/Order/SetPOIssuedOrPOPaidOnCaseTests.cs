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
    public class SetPOIssuedOrPOPaidOnCaseTests : PluginTestsBase
    {
        [Fact]
        public void Test_SetPOIssuedOrPOPaidOnCaseTestsTests_Create()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            Incident incident = new Incident().Fake();
            incident.ipg_portalalerts = "Missing Information";
            SalesOrder targetOrder = new SalesOrder().Fake().WithCaseReference(incident);
            targetOrder.ipg_po_issue_date = DateTime.Now;
            #endregion

            #region Setup execution context

            var entities = new List<Entity>() { incident, targetOrder};
            fakedContext.Initialize(entities);
            var inputParameters = new ParameterCollection { { "Target", targetOrder } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };
            #endregion

            #region Execute plugin
            fakedContext.ExecutePluginWith<SetPOIssuedOrPOPaidOnCase>(pluginContext);

            var query = new QueryExpression(Incident.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(Incident.Fields.ipg_portalalerts)
            };
            var updatedCase = fakedService.RetrieveMultiple(query).Entities.SingleOrDefault();
            #endregion
            Assert.NotNull(updatedCase);
            Assert.Equal("Missing Information", updatedCase[Incident.Fields.ipg_portalalerts]);
        }

        [Fact]
        public void Test_SetPOIssuedOrPOPaidOnCaseTests_Update()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            Incident incident = new Incident().Fake();
            incident.ipg_portalalerts = "PO Paid";
            SalesOrder targetOrder = new SalesOrder().Fake();
            targetOrder.ipg_po_issue_date = DateTime.Now;
            SalesOrder postOrder = new SalesOrder().Fake().WithCaseReference(incident);
            postOrder.ipg_po_issue_date = System.DateTime.Now;
            #endregion

            #region Setup execution context

            var entities = new List<Entity>() { incident, postOrder};
            fakedContext.Initialize(entities);
            var inputParameters = new ParameterCollection { { "Target", targetOrder } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection() { { "PostImage", postOrder } },
                PreEntityImages = null
            };
            #endregion

            #region Execute plugin
            fakedContext.ExecutePluginWith<SetPOIssuedOrPOPaidOnCase>(pluginContext);

            var query = new QueryExpression(Incident.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(Incident.Fields.ipg_portalalerts)
            };
            var updatedCase = fakedService.RetrieveMultiple(query).Entities.SingleOrDefault();
            #endregion
            Assert.NotNull(updatedCase);
            Assert.NotEqual(incident.ipg_portalalerts, updatedCase[Incident.Fields.ipg_portalalerts]);
            Assert.Equal("PO Issued", updatedCase[Incident.Fields.ipg_portalalerts]);
        }
    }
}
