using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class UpdateCollectionVendorValueToSCGTest : PluginTestsBase
    {
        [Fact]
        public void CheckCollectionVendor()
        {
            #region Arrange
            var fakedContext = new XrmFakedContext();

            ServiceProvider = ServiceProviderMock.Object;
            Incident incident = new Incident().Fake();
            incident.ipg_CollectionVendor = new OptionSetValue((int)Incident_ipg_CollectionVendor.IPG);
            incident.ipg_BillToPatient = new OptionSetValue((int)ipg_TwoOptions.Yes);

            ipg_statementgenerationeventconfiguration configuration = new ipg_statementgenerationeventconfiguration().Fake();
            configuration.ipg_name = "S1";

            ipg_statementgenerationtask task = new ipg_statementgenerationtask().Fake().WithCaseReference(incident);
            task.ipg_EndDate = DateTime.Today.AddDays(-1);
            task.ipg_eventid = configuration.ToEntityReference();
            #endregion

            #region Act
            fakedContext.Initialize(new List<Entity> { configuration, task, incident });

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeActionsUpdateCollectionVendorValueToSCG",
                Stage = PipelineStages.PostOperation,
                InitiatingUserId = Guid.NewGuid()
            };

            fakedContext.ExecutePluginWith<UpdateCollectionVendorValueToSCG>(pluginContext);

            #endregion

            #region Assert
            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);
            Incident incidentFaked = fakedService.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(nameof(Incident.ipg_CollectionVendor).ToLower())).ToEntity<Incident>();
            Assert.Equal(incidentFaked.ipg_CollectionVendor.Value, (int)Incident_ipg_CollectionVendor.SCG);
            #endregion
        }

    }
}
