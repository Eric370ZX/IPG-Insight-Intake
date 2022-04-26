using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class UpdateCollectionVendorValueToIPGTest : PluginTestsBase
    {
        [Fact]
        public void CheckCollectionVendor()
        {
            #region Arrange
            var fakedContext = new XrmFakedContext();

            ServiceProvider = ServiceProviderMock.Object;
            Incident incident = new Incident().Fake();
            incident.ipg_StateCode = new OptionSetValue((int)ipg_CaseStateCodes.CarrierServices);

            var preImage = new Incident()
            {
                Id = incident.Id,
                ipg_StateCode = new OptionSetValue((int)ipg_CaseStateCodes.PatientServices),
                ipg_CollectionVendor = new OptionSetValue((int)Incident_ipg_CollectionVendor.SCG)
            };
            #endregion

            #region Act
            fakedContext.Initialize(incident);

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", incident } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection() { { "PreImage", preImage } },
                InitiatingUserId = Guid.NewGuid()
            };

            fakedContext.ExecutePluginWith<UpdateCollectionVendorValueToIPG>(pluginContext);

            #endregion

            #region Assert
            Assert.Equal(incident.ipg_CollectionVendor.Value, (int)Incident_ipg_CollectionVendor.IPG);
            #endregion
        }

    }
}
