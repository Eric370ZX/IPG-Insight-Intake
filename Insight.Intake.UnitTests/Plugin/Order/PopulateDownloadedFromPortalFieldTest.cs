using FakeXrmEasy;
using Insight.Intake.Plugin.Order;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Order
{
    public class PopulateDownloadedFromPortalFieldTest : PluginTestsBase
    {
        [Fact]
        public void PO_CPA()
        {
            var fakedContext = new XrmFakedContext();

            SalesOrder order = new SalesOrder().Fake();
            order.ipg_potypecode = new OptionSetValue((int)ipg_PurchaseOrderTypes.CPA);
            order.ipg_downloadedfromportal = true;

            fakedContext.Initialize(new List<Entity> { order });

            var inputParameters = new ParameterCollection { { "Target", order } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = order.LogicalName,
                InputParameters = inputParameters
            };
            fakedContext.ExecutePluginWith<PopulateDownloadedFromPortalField>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var orderFaked = fakedService.Retrieve(order.LogicalName, order.Id, new ColumnSet(nameof(SalesOrder.ipg_downloadedfromportal).ToLower())).ToEntity<SalesOrder>();
            Assert.False(order.ipg_downloadedfromportal);
        }

        [Fact]
        public void PO_MPO()
        {
            var fakedContext = new XrmFakedContext();

            SalesOrder order = new SalesOrder().Fake();
            order.ipg_potypecode = new OptionSetValue((int)ipg_PurchaseOrderTypes.MPO);
            order.ipg_downloadedfromportal = true;

            fakedContext.Initialize(new List<Entity> { order });

            var inputParameters = new ParameterCollection { { "Target", order } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = order.LogicalName,
                InputParameters = inputParameters
            };
            fakedContext.ExecutePluginWith<PopulateDownloadedFromPortalField>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var orderFaked = fakedService.Retrieve(order.LogicalName, order.Id, new ColumnSet(nameof(SalesOrder.ipg_downloadedfromportal).ToLower())).ToEntity<SalesOrder>();
            Assert.False(order.ipg_downloadedfromportal);
        }

        [Fact]
        public void PO_TPO()
        {
            var fakedContext = new XrmFakedContext();

            SalesOrder order = new SalesOrder().Fake();
            order.ipg_potypecode = new OptionSetValue((int)ipg_PurchaseOrderTypes.TPO);
            order.ipg_downloadedfromportal = true;

            fakedContext.Initialize(new List<Entity> { order });

            var inputParameters = new ParameterCollection { { "Target", order } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = order.LogicalName,
                InputParameters = inputParameters
            };
            fakedContext.ExecutePluginWith<PopulateDownloadedFromPortalField>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var orderFaked = fakedService.Retrieve(order.LogicalName, order.Id, new ColumnSet(nameof(SalesOrder.ipg_downloadedfromportal).ToLower())).ToEntity<SalesOrder>();
            Assert.True(order.ipg_downloadedfromportal);
        }

        [Fact]
        public void PO_ZPO()
        {
            var fakedContext = new XrmFakedContext();

            SalesOrder order = new SalesOrder().Fake();
            order.ipg_potypecode = new OptionSetValue((int)ipg_PurchaseOrderTypes.ZPO);
            order.ipg_downloadedfromportal = true;

            fakedContext.Initialize(new List<Entity> { order });

            var inputParameters = new ParameterCollection { { "Target", order } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = order.LogicalName,
                InputParameters = inputParameters
            };
            fakedContext.ExecutePluginWith<PopulateDownloadedFromPortalField>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var orderFaked = fakedService.Retrieve(order.LogicalName, order.Id, new ColumnSet(nameof(SalesOrder.ipg_downloadedfromportal).ToLower())).ToEntity<SalesOrder>();
            Assert.True(order.ipg_downloadedfromportal);
        }

    }
}
