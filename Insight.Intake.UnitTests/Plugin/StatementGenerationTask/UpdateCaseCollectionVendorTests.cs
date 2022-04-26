using FakeXrmEasy;
using Insight.Intake.Plugin.StatementGenerationTask;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.StatementGenerationTask
{
    public class UpdateCaseCollectionVendorTests : PluginTestsBase
    {
        [Fact]
        public void UpdateVendorToSCG()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident incident = new Incident().Fake();
            incident.ipg_CollectionVendor = new OptionSetValue((int)Incident_ipg_CollectionVendor.IPG);

            ipg_statementgenerationeventconfiguration eventConfiguration = new ipg_statementgenerationeventconfiguration().Fake();
            eventConfiguration.ipg_name = "S1 Generated";

            ipg_documentcategorytype docCategory = new ipg_documentcategorytype()
            {
                Id = new Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad70"),
                ipg_name = "Patient Statement"
            };

            ipg_document document = new ipg_document().Fake();
            document.ipg_documenttypecategoryid = docCategory.ToEntityReference();

            ipg_statementgenerationtask task = new ipg_statementgenerationtask().Fake();
            task.ipg_eventid = eventConfiguration.ToEntityReference();
            task.ipg_caseid = incident.ToEntityReference();
            task.ipg_DocumentId = document.ToEntityReference();

            var listForInit = new List<Entity>() { incident, eventConfiguration, docCategory, document, task };
            fakedContext.Initialize(listForInit);

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", task } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection() { { "PostImage", task } },
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };

            //ACT
            fakedContext.ExecutePluginWith<UpdateCaseCollectionVendor>(pluginContext);

            //Assert
            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var incidentUpd = fakedService.Retrieve(Incident.EntityLogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();

            Assert.Equal(incidentUpd.ipg_CollectionVendor?.Value, (int)Incident_ipg_CollectionVendor.SCG);
        }


        [Fact]
        public void UpdateVendorToIPG()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident incident = new Incident().Fake();
            incident.ipg_CollectionVendor = new OptionSetValue((int)Incident_ipg_CollectionVendor.SCG);

            ipg_statementgenerationeventconfiguration eventConfiguration = new ipg_statementgenerationeventconfiguration().Fake();
            eventConfiguration.ipg_name = "A2 Generated";

            ipg_documentcategorytype docCategory = new ipg_documentcategorytype()
            {
                Id = new Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad70"),
                ipg_name = "Patient Statement"
            };

            ipg_document document = new ipg_document().Fake();
            document.ipg_documenttypecategoryid = docCategory.ToEntityReference();

            ipg_statementgenerationtask task = new ipg_statementgenerationtask().Fake();
            task.ipg_eventid = eventConfiguration.ToEntityReference();
            task.ipg_caseid = incident.ToEntityReference();
            task.ipg_DocumentId = document.ToEntityReference();

            var listForInit = new List<Entity>() { incident, eventConfiguration, docCategory, document, task };
            fakedContext.Initialize(listForInit);

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", task } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection() { { "PostImage", task } },
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };

            //ACT
            fakedContext.ExecutePluginWith<UpdateCaseCollectionVendor>(pluginContext);

            //Assert
            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var incidentUpd = fakedService.Retrieve(Incident.EntityLogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();

            Assert.Equal(incidentUpd.ipg_CollectionVendor?.Value, (int)Incident_ipg_CollectionVendor.IPG);
        }


        [Fact]
        public void IgnoreUpdate()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident incident = new Incident().Fake();
            incident.ipg_CollectionVendor = new OptionSetValue((int)Incident_ipg_CollectionVendor.SCG);

            ipg_statementgenerationeventconfiguration eventConfiguration = new ipg_statementgenerationeventconfiguration().Fake();
            eventConfiguration.ipg_name = "A1";

            ipg_documentcategorytype docCategory = new ipg_documentcategorytype()
            {
                Id = new Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad70"),
                ipg_name = "Other Category"
            };

            ipg_document document = new ipg_document().Fake();
            document.ipg_documenttypecategoryid = docCategory.ToEntityReference();

            ipg_statementgenerationtask task = new ipg_statementgenerationtask().Fake();
            task.ipg_eventid = eventConfiguration.ToEntityReference();
            task.ipg_caseid = incident.ToEntityReference();
            task.ipg_DocumentId = document.ToEntityReference();

            var listForInit = new List<Entity>() { incident, eventConfiguration, docCategory, document, task };
            fakedContext.Initialize(listForInit);

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", task } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection() { { "PostImage", task } },
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };

            //ACT
            fakedContext.ExecutePluginWith<UpdateCaseCollectionVendor>(pluginContext);

            //Assert
            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var incidentUpd = fakedService.Retrieve(Incident.EntityLogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();

            Assert.Equal(incidentUpd.ipg_CollectionVendor?.Value, (int)Incident_ipg_CollectionVendor.SCG);
        }
    }
}
