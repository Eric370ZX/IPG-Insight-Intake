using System;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Xunit;
using Insight.Intake.Plugin.Case;

namespace Insight.Intake.UnitTests.Plugin.Referral
{
    public class SavePatientDemographicInformationPluginTest : PluginTestsBase
    {
        [Fact]
        public void ValidateIfContactUpdatedIfPatientDemographicsSaved()
        {
            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;

            //Create a fake patient contact and incident. Binf the contact with the incident.
            Contact patient = new Contact().Fake();
            Incident createdIncident = new Incident().Fake().WithPatientReference(patient);

            //Change demographics fields on case.
            createdIncident.ipg_PatientAddress = "Address to validate";
            createdIncident.ipg_PatientCity = "City to validate";
            createdIncident.ipg_PatientState = "State to validate";
            createdIncident.ipg_PatientZip = "Zip to validate";
            createdIncident.ipg_PatientHomePhone = "123456Home";
            createdIncident.ipg_PatientWorkPhone = "123456Work";
            createdIncident.ipg_PatientCellPhone = "123456Cell";
            createdIncident.ipg_PatientEmail = "test@email.com";
            
            OrganizationServiceMock.WithRetrieveCrud<Contact>(patient);
            OrganizationServiceMock.WithRetrieveCrud<Incident>(createdIncident);

            OrganizationService = OrganizationServiceMock.Object;
            #endregion

            #region Setup execution context
            PluginExecutionContextMock.Setup(x => x.MessageName).Returns("Create");

            var request = new CreateRequest
            {
                Target = createdIncident.ToEntity<Entity>()
            };

            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(request.Parameters);

            var outputParameters = new ParameterCollection
            {
                { "id", createdIncident.Id }
            };

            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(outputParameters);
            #endregion

            #region Execute plugin

            var plugin = new SavePatientDemographicInformation();

            plugin.Execute(ServiceProvider);

            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));

            if (!pluginExecutionContext.InputParameters.Contains("Target") || !(pluginExecutionContext.InputParameters["Target"] is Entity))
            {
                throw new Exception("Input target should be Entity.");
            }

            var incident = ((Entity)pluginExecutionContext.InputParameters["Target"]).ToEntity<Incident>();

            //Validate if fields have updated.
            Assert.Equal(patient.Address1_Line1, incident.ipg_PatientAddress);
            Assert.Equal(patient.Address1_City, incident.ipg_PatientCity);
            Assert.Equal(patient.Address1_StateOrProvince, incident.ipg_PatientState);
            //Assert.Equal(patient.Address1_PostalCode, incident.ipg_PatientZip);
            Assert.Equal(patient.Address1_Telephone1, incident.ipg_PatientHomePhone);
            Assert.Equal(patient.Address1_Telephone2, incident.ipg_PatientWorkPhone);
            Assert.Equal(patient.Address1_Telephone3, incident.ipg_PatientCellPhone);
            Assert.Equal(patient.EMailAddress1, incident.ipg_PatientEmail);
            Assert.Equal(patient.Id, incident.ipg_PatientId.Id);

            #endregion
        }
    }
}
