using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class ValidateCarrierChangeTest : PluginTestsBase
    {
        [Fact]
        public void FirstCarrier_CanBeChanged()
        {
            //ARRANGE
            var initialCarrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier)
                                        .WithCarrierType(ipg_CarrierType.Commercial).Generate();

            var newCarrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier)
                                        .WithCarrierType(ipg_CarrierType.Auto).Generate();

            var incidentPreImage = new Incident().Fake().WithPrimaryCarrierReference(initialCarrier).Generate();
            var incident = new Incident().Fake().WithPrimaryCarrierReference(newCarrier).Generate();
               
            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { initialCarrier, newCarrier, incident });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreValidation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = null,
                PreEntityImages = new EntityImageCollection { { "PreImage", incidentPreImage } }
            };

            //ACT
            fakedContext.ExecutePluginWith<ValidateCarrierChange>(pluginContext);

        }

        [Fact]
        public void Carrier1_CannotBeChanged_From_Auto_To_WorkersComp()
        {
            //ARRANGE
            var initialCarrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier)
                                        .WithCarrierType(ipg_CarrierType.Auto).Generate();

            var newCarrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier)
                                        .WithCarrierType(ipg_CarrierType.WorkersComp).Generate();

            var incidentPreImage = new Incident().Fake().WithPrimaryCarrierReference(initialCarrier).Generate();
            var incident = new Incident().Fake().WithPrimaryCarrierReference(newCarrier).Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { initialCarrier, newCarrier, incident });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreValidation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = null,
                PreEntityImages = new EntityImageCollection { { "PreImage", incidentPreImage } }
            };

            //ACT, ASSERT
            Assert.ThrowsAny<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<ValidateCarrierChange>(pluginContext));
        }

        [Fact]
        public void Carrier2_CannotBeChanged_To_WorkersComp()
        {
            //ARRANGE
            var newCarrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier)
                                        .WithCarrierType(ipg_CarrierType.WorkersComp).Generate();

            var incidentPreImage = new Incident().Fake().Generate();
            var incident = new Incident().Fake().WithCarrierReference(newCarrier, isPrimaryCarrier: false).Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { newCarrier, incident });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreValidation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = null,
                PreEntityImages = new EntityImageCollection { { "PreImage", incidentPreImage } }
            };

            //ACT, ASSERT
            Assert.ThrowsAny<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<ValidateCarrierChange>(pluginContext));
        }

        [Fact]
        public void Carrier2_CannotBeAdded_If_Carrier1_Is_WorkersComp()
        {
            //ARRANGE
            var carrier1 = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier)
                                        .WithCarrierType(ipg_CarrierType.WorkersComp).Generate();
            var newCarrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier)
                                        .WithCarrierType(ipg_CarrierType.Commercial).Generate();

            var incidentPreImage = new Incident().Fake().WithCarrierReference(carrier1, isPrimaryCarrier: true).Generate();
            var incident = new Incident().Fake().WithCarrierReference(carrier1, isPrimaryCarrier: true)
                .WithCarrierReference(newCarrier, isPrimaryCarrier: false).Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { carrier1, newCarrier, incident });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreValidation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = null,
                PreEntityImages = new EntityImageCollection { { "PreImage", incidentPreImage } }
            };

            //ACT, ASSERT
            Assert.ThrowsAny<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<ValidateCarrierChange>(pluginContext));
        }
    }
}
