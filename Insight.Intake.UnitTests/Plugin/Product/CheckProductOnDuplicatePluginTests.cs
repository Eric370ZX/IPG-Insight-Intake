using FakeXrmEasy;
using Microsoft.Xrm.Client;
using Insight.Intake.Plugin.Product;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;
using Microsoft.Xrm.Sdk.Client;
using System.Linq;

namespace Insight.Intake.UnitTests.Plugin.Product
{
    public class CheckProductOnDuplicatePluginTests: PluginTestsBase
    {
        [Fact]
        public void CheckWhenDuplicationExists()
        {
            var fakedContext = new XrmFakedContext();

            Intake.Account manufacturer = new Intake.Account().Fake();
            Intake.Product product = new Intake.Product().Fake().RuleFor(p => p.ipg_manufacturerid,p => manufacturer.ToEntityReference());

            var duplicate = product.Clone().ToEntity<Intake.Product>();
            duplicate.Id = Guid.NewGuid();

            fakedContext.Initialize(new List<Entity> { manufacturer, duplicate });

            var inputParameters = new ParameterCollection { { "Target", product } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Intake.Product.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = new EntityImageCollection()
            };

            var error = Assert.Throws<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<CheckProductOnDuplicatePlugin>(pluginContext));
            Assert.Contains($"Product with {product.ipg_manufacturerpartnumber} Number alredy exist for {product.ipg_manufacturerid?.Name} Manufacturer!", error.Message);
        }

        [Fact]
        public void CheckOnUpdateWhenDuplicationExists()
        {
            var fakedContext = new XrmFakedContext();

            Intake.Account manufacturer = new Intake.Account().Fake();
            Intake.Product product = new Intake.Product().Fake().RuleFor(p => p.ipg_manufacturerid, p => manufacturer.ToEntityReference());

            var duplicate = product.Clone().ToEntity<Intake.Product>();
            duplicate.Id= Guid.NewGuid();
            var list = new List<Entity>() { manufacturer, duplicate, product };

            fakedContext.Initialize(list);


            var crmContext = new OrganizationServiceContext(fakedContext.GetOrganizationService());

            var inputParameters = new ParameterCollection { { "Target", product } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Intake.Product.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = new EntityImageCollection()
            };

            var error = Assert.Throws<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<CheckProductOnDuplicatePlugin>(pluginContext));
            Assert.Contains($"Product with {product.ipg_manufacturerpartnumber} Number alredy exist for {product.ipg_manufacturerid?.Name} Manufacturer!", error.Message);
        }

        [Fact]
        public void CheckWhenDuplicationNotExists()
        {
            var fakedContext = new XrmFakedContext();

            Intake.Account manufacturer = new Intake.Account().Fake();
            Intake.Product product = new Intake.Product().Fake().RuleFor(p => p.ipg_manufacturerid, p => manufacturer.ToEntityReference());

            var duplicate = product.Clone().ToEntity<Intake.Product>();
            duplicate.Id = Guid.NewGuid();

            fakedContext.Initialize(new List<Entity> { manufacturer });

            var inputParameters = new ParameterCollection { { "Target", product } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Intake.Product.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = new EntityImageCollection()
            };

            fakedContext.ExecutePluginWith<CheckProductOnDuplicatePlugin>(pluginContext);
        }
    }
}
