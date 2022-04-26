using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class UpdateCarrierNetworkOnCaseTest : PluginTestsBase
    {
        [Fact]
        public void ValidateCarrierNetworkIsUpdatedOnCaseCreation()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            ServiceProvider = ServiceProviderMock.Object;

            ipg_carriernetwork carrierNetwork = new ipg_carriernetwork().Fake();

            Intake.Account carrier = new Intake.Account().Fake().WithCarrierNetworkReference(carrierNetwork);

            Incident incident = new Incident().Fake().WithCarrierReference(carrier, true);

            OrganizationService = OrganizationServiceMock.Object;

            OrganizationServiceMock.WithUpdateCrud<Incident>();

            OrganizationServiceMock.WithUpdateCrud<Intake.Account>();

            #endregion

            #region Setup execution context

            var entities = new List<Entity>();

            entities.Add(carrierNetwork);
            entities.Add(carrier);
            entities.Add(incident);

            fakedContext.Initialize(entities);

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var outputParameters = new ParameterCollection { { "Success", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            #endregion

            #region Execute plugin
            fakedContext.ExecutePluginWith<UpdateCarrierNetworkOnCase>(pluginContext);
            Assert.Equal(incident.ipg_carriernetwork, carrier.ipg_carriernetworkid);
            #endregion
        }
    }
}
