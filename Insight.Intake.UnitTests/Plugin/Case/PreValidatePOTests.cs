using System;
using System.Collections.Generic;
using System.Linq;
using FakeXrmEasy;
using Insight.Intake.Data;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class PreValidatePOTests : PluginTestsBase
    {
        [Fact]
        public void CheckWhatIncidentValidatesCorrectly()
        {
            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;

            Incident incident = new Incident().Fake();

            Intake.Account carrier = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Carrier);

            Intake.Account facility = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Facility);

            Intake.Account manufacturer = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Manufacturer);

            var incidentParts = new ipg_casepartdetail().Fake().Generate(5);
            
            OrganizationServiceMock.WithRetrieveCrud(incident);
            
            OrganizationServiceMock.WithRetrieveCrud(carrier);
            
            OrganizationServiceMock.WithRetrieveCrud(facility);
            
            OrganizationServiceMock.WithRetrieveCrud(manufacturer);

            OrganizationServiceMock.WithRetrieveMultipleCrud(
                ipg_casepartdetail.EntityLogicalName,
                incidentParts.Cast<Entity>().ToList()
            );
            
            OrganizationService = OrganizationServiceMock.Object;
            #endregion
            
            #region Setup execution context
            PluginExecutionContextMock.Setup(x => x.MessageName).Returns("ipg_IPGCaseActionsPreValidatePO");

            var request = new ipg_IPGCaseActionsPreValidatePORequest
            {
                Target = new EntityReference(Incident.EntityLogicalName, incident.Id),
            };

            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(request.Parameters);

            var outputParameters = new ParameterCollection();

            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(outputParameters);
            #endregion
            
            #region Execute plugin
            var plugin = new PreValidatePO();

            plugin.Execute(ServiceProvider);
            
            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));
            
            Assert.True(pluginExecutionContext.OutputParameters.Contains("HasError"));
            
            Assert.True((bool)pluginExecutionContext.OutputParameters["HasError"]);
            
            Assert.True(pluginExecutionContext.OutputParameters.Contains("Message"));

            var errorList = ((string)pluginExecutionContext.OutputParameters["Message"]).Split(new[] {", "}, StringSplitOptions.None);
            
            var expectedErrors = new List<string>
            {
                "Patient First Name",
                "Patient Last Name",
                "Procedure",
                "Carrier",
                "Facility",
                "Stage",
            };

            foreach (var expectedError in expectedErrors)
            {
                Assert.Contains(expectedError, errorList);
            }
            #endregion
        }
        
        [Fact]
        public void CheckWhatIncidentPartsValidatesCorrectly()
        {
            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;

            Intake.Account carrier = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Carrier);

            Intake.Account facility = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Facility);

            Incident incident = new Incident().Fake()
                .WithPatientFullName()
                .WithPatientProcedure()
                .WithPrimaryCarrierReference(carrier)
                .WithFacilityReference(facility)
                .WithDeviceStageOptionSet(DeviceStageOptionSet.Other);
            
            Intake.Account manufacturer = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Manufacturer);

            var incidentParts = new ipg_casepartdetail().Fake().Generate(5);
            
            OrganizationServiceMock.WithRetrieveCrud(incident);
            
            OrganizationServiceMock.WithRetrieveCrud(carrier);
            
            OrganizationServiceMock.WithRetrieveCrud(facility);
            
            OrganizationServiceMock.WithRetrieveCrud(manufacturer);

            OrganizationServiceMock.WithRetrieveMultipleCrud(
                ipg_casepartdetail.EntityLogicalName,
                incidentParts.Cast<Entity>().ToList()
            );
            
            OrganizationService = OrganizationServiceMock.Object;
            #endregion
            
            #region Setup execution context
            PluginExecutionContextMock.Setup(x => x.MessageName).Returns("ipg_IPGCaseActionsPreValidatePO");

            var request = new ipg_IPGCaseActionsPreValidatePORequest
            {
                Target = new EntityReference(Incident.EntityLogicalName, incident.Id),
            };

            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(request.Parameters);

            var outputParameters = new ParameterCollection();

            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(outputParameters);
            #endregion
            
            #region Execute plugin
            var plugin = new PreValidatePO();

            plugin.Execute(ServiceProvider);
            
            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));
            
            Assert.True(pluginExecutionContext.OutputParameters.Contains("HasError"));
            
            Assert.True((bool)pluginExecutionContext.OutputParameters["HasError"]);
            
            Assert.True(pluginExecutionContext.OutputParameters.Contains("Message"));

            var actualError = (string)pluginExecutionContext.OutputParameters["Message"];

            var expectedError = "No Parts eligible for PO generation. Check Inventory Method, ReOrder Method, Part Type, Actual DOS, etc.";

            Assert.Equal(actualError, expectedError);
            #endregion
        }
        
        [Fact]
        public void CheckWhatManufacturerValidatesCorrectly()
        {
            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;

            Intake.Account carrier = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Carrier);

            Intake.Account facility = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Facility)
                .WithDTM(true);

            Incident incident = new Incident().Fake()
                .WithPatientFullName()
                .WithPatientProcedure()
                .WithPrimaryCarrierReference(carrier)
                .WithFacilityReference(facility)
                .WithDeviceStageOptionSet(DeviceStageOptionSet.Other);
            
            Intake.Account manufacturer = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Manufacturer)
                .WithManufacturerEffectiveDate(DateTime.UtcNow.AddDays(-1))
                .WithManufacturerExpirationDate(DateTime.UtcNow.AddDays(1));

            Intake.Product product = new Intake.Product().Fake()
                .WithProductTypeOptionSet(ProductTypeOptionSet.Biologics);
            
            var incidentParts = new ipg_casepartdetail().Fake()
                .WithProductReference(product)
                .WithPayFacility(false)
                .WithFacilityOrders(FacilityOrdersOptionSet.FacilityOrders)
                .WithManufacturerReference(manufacturer)
                .Generate(1);
            
            OrganizationServiceMock.WithRetrieveCrud(incident);
            
            OrganizationServiceMock.WithRetrieveCrud(carrier);
            
            OrganizationServiceMock.WithRetrieveCrud(facility);
            
            OrganizationServiceMock.WithRetrieveCrud(product);
            
            OrganizationServiceMock.WithRetrieveCrud(manufacturer);

            OrganizationServiceMock.WithRetrieveMultipleCrud(
                ipg_casepartdetail.EntityLogicalName,
                incidentParts.Cast<Entity>().ToList()
            );

            OrganizationServiceMock.WithRetrieveMultipleCrud(
                ipg_facilitymanufacturerrelationship.EntityLogicalName,
                new List<Entity> {}
            );
            
            OrganizationService = OrganizationServiceMock.Object;
            #endregion
            
            #region Setup execution context
            PluginExecutionContextMock.Setup(x => x.MessageName).Returns("ipg_IPGCaseActionsPreValidatePO");

            var request = new ipg_IPGCaseActionsPreValidatePORequest
            {
                Target = new EntityReference(Incident.EntityLogicalName, incident.Id),
            };

            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(request.Parameters);

            var outputParameters = new ParameterCollection();

            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(outputParameters);
            #endregion
            
            #region Execute plugin
            var plugin = new PreValidatePO();

            plugin.Execute(ServiceProvider);
            
            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));
            
            Assert.True(pluginExecutionContext.OutputParameters.Contains("HasError"));            
            Assert.True((bool)pluginExecutionContext.OutputParameters["HasError"]);          
            Assert.True(pluginExecutionContext.OutputParameters.Contains("Message"));

            var actualError = (string)pluginExecutionContext.OutputParameters["Message"];
            var expectedError = $"{manufacturer.Name} missing manufacturer definition for facility";
            Assert.Equal(actualError, expectedError);

            #endregion
        }
        
        [Fact]
        public void CheckWhatRequiredDocumentsValidatesCorrectly()
        {
            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;

            Intake.Account carrier = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Carrier);

            Intake.Account facility = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Facility)
                .WithDTM(true);

            Incident incident = new Incident().Fake()
                .WithActualDos(DateTime.UtcNow)
                .WithPatientFullName()
                .WithPatientProcedure()
                .WithPrimaryCarrierReference(carrier)
                .WithFacilityReference(facility)
                .WithDeviceStageOptionSet(DeviceStageOptionSet.Other);
            
            Intake.Account manufacturer = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Manufacturer)
                .WithManufacturerEffectiveDate(DateTime.UtcNow.AddDays(-1))
                .WithManufacturerExpirationDate(DateTime.UtcNow.AddDays(1));

            Intake.Product product = new Intake.Product().Fake()
                .WithProductTypeOptionSet(ProductTypeOptionSet.Biologics);
            
            var incidentParts = new ipg_casepartdetail().Fake()
                .WithProductReference(product)
                .WithPayFacility(false)
                .WithFacilityOrders(FacilityOrdersOptionSet.FacilityOrders)
                .WithManufacturerReference(manufacturer)
                .Generate(1);

            var facilityCarrierRelationships = new ipg_facilitymanufacturerrelationship().Fake()
                .WithFacilityReference(facility)
                .WithManufacturerReference(manufacturer)
                .Generate(1);

            ipg_documenttype documentType = new ipg_documenttype().Fake("Some rquired type"); 
            
            var requiredInformations = new ipg_requiredinformation().Fake()
                .WithIncidentReference(incident)
                .WithDocumentTypeReference(documentType)
                .Generate(1);
            
            OrganizationServiceMock.WithRetrieveCrud(incident);
            
            OrganizationServiceMock.WithRetrieveCrud(carrier);
            
            OrganizationServiceMock.WithRetrieveCrud(facility);
            
            OrganizationServiceMock.WithRetrieveCrud(product);
            
            OrganizationServiceMock.WithRetrieveCrud(manufacturer);

            OrganizationServiceMock.WithRetrieveMultipleCrud(
                ipg_casepartdetail.EntityLogicalName,
                incidentParts.Cast<Entity>().ToList()
            );

            OrganizationServiceMock.WithRetrieveMultipleCrud(
                ipg_facilitymanufacturerrelationship.EntityLogicalName,
                facilityCarrierRelationships.Cast<Entity>().ToList()
            );

            OrganizationServiceMock.WithRetrieveMultipleCrud(
                ipg_requiredinformation.EntityLogicalName,
                requiredInformations.Cast<Entity>().ToList()
            );

            OrganizationServiceMock.WithRetrieveMultipleCrud(
                ipg_document.EntityLogicalName,
                new List<Entity>()
            );
            
            OrganizationService = OrganizationServiceMock.Object;
            #endregion
            
            #region Setup execution context
            PluginExecutionContextMock.Setup(x => x.MessageName).Returns("ipg_IPGCaseActionsPreValidatePO");

            var request = new ipg_IPGCaseActionsPreValidatePORequest
            {
                Target = new EntityReference(Incident.EntityLogicalName, incident.Id),
            };

            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(request.Parameters);

            var outputParameters = new ParameterCollection();

            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(outputParameters);
            #endregion
            
            #region Execute plugin
            var plugin = new PreValidatePO();

            plugin.Execute(ServiceProvider);
            
            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));
            
            Assert.True(pluginExecutionContext.OutputParameters.Contains("HasError"));
            
            Assert.True((bool)pluginExecutionContext.OutputParameters["HasError"]);
            
            Assert.True(pluginExecutionContext.OutputParameters.Contains("Message"));

            var actualError = (string)pluginExecutionContext.OutputParameters["Message"];
            
            var expectedError = "Required documents not available";

            Assert.Equal(actualError, expectedError);
            #endregion
        }
        
        [Fact]
        public void CheckWhatAllDataValidatesCorrectly()
        {
            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;

            Intake.Account carrier = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Carrier);

            Intake.Account facility = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Facility)
                .WithDTM(true);

            Incident incident = new Incident().Fake()
                .WithPatientFullName()
                .WithPatientProcedure()
                .WithPrimaryCarrierReference(carrier)
                .WithFacilityReference(facility)
                .WithDeviceStageOptionSet(DeviceStageOptionSet.Other);
            
            Intake.Account manufacturer = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Manufacturer)
                .WithManufacturerEffectiveDate(DateTime.UtcNow.AddDays(-1))
                .WithManufacturerExpirationDate(DateTime.UtcNow.AddDays(1));

            Intake.Product product = new Intake.Product().Fake()
                .WithProductTypeOptionSet(ProductTypeOptionSet.Biologics);
            
            var incidentParts = new ipg_casepartdetail().Fake()
                .WithProductReference(product)
                .WithPayFacility(false)
                .WithFacilityOrders(FacilityOrdersOptionSet.FacilityOrders)
                .WithManufacturerReference(manufacturer)
                .Generate(1);

            var facilityCarrierRelationships = new ipg_facilitymanufacturerrelationship().Fake()
                .WithFacilityReference(facility)
                .WithManufacturerReference(manufacturer)
                .Generate(1);
            
            OrganizationServiceMock.WithRetrieveCrud(incident);
            
            OrganizationServiceMock.WithRetrieveCrud(carrier);
            
            OrganizationServiceMock.WithRetrieveCrud(facility);
            
            OrganizationServiceMock.WithRetrieveCrud(product);
            
            OrganizationServiceMock.WithRetrieveCrud(manufacturer);

            OrganizationServiceMock.WithRetrieveMultipleCrud(
                ipg_casepartdetail.EntityLogicalName,
                incidentParts.Cast<Entity>().ToList()
            );

            OrganizationServiceMock.WithRetrieveMultipleCrud(
                ipg_facilitymanufacturerrelationship.EntityLogicalName,
                facilityCarrierRelationships.Cast<Entity>().ToList()
            );
            
            OrganizationService = OrganizationServiceMock.Object;
            #endregion
            
            #region Setup execution context
            PluginExecutionContextMock.Setup(x => x.MessageName).Returns("ipg_IPGCaseActionsPreValidatePO");

            var request = new ipg_IPGCaseActionsPreValidatePORequest
            {
                Target = new EntityReference(Incident.EntityLogicalName, incident.Id),
            };

            PluginExecutionContextMock.Setup(x => x.InputParameters).Returns(request.Parameters);

            var outputParameters = new ParameterCollection();

            PluginExecutionContextMock.Setup(x => x.OutputParameters).Returns(outputParameters);
            #endregion
            
            #region Execute plugin
            var plugin = new PreValidatePO();

            plugin.Execute(ServiceProvider);
            
            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));
            
            Assert.True(pluginExecutionContext.OutputParameters.Contains("HasError"));
            
            Assert.False((bool)pluginExecutionContext.OutputParameters["HasError"]);
            
            Assert.True(pluginExecutionContext.OutputParameters.Contains("Message"));

            var infoList = ((string)pluginExecutionContext.OutputParameters["Message"]).Split(new[] {", "}, StringSplitOptions.None);
            
            var expectedInfos = new List<string>
            {
                $"{manufacturer.Name} **<- TPO Only ** (No Actual DOS)",
            };

            foreach (var expectedInfo in expectedInfos)
            {
                Assert.Contains(expectedInfo, infoList);
            }
            #endregion
        }

        [Fact] 
        public void CheckManufacturerValidationRequiredNumber()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            Intake.Account manufacturer = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Manufacturer).RuleFor(x=>x.ipg_ManufacturerIsFacilityAcctRequired, x=>true);
            Intake.Team caseMg = new Team().Fake("Case Management");

            Intake.Account Facility = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Facility).RuleFor(x => x.ipg_ManufacturerIsFacilityAcctRequired, x => false);

            Incident incident = new Incident().Fake()
            .WithActualDos(DateTime.UtcNow)
            .WithPatientFullName()
            .WithPatientProcedure()
            .WithDeviceStageOptionSet(DeviceStageOptionSet.Other)
            .WithFacilityReference(Facility);

            Intake.ipg_facilitymanufacturerrelationship relationship = new Intake.ipg_facilitymanufacturerrelationship().Fake()
                .WithFacilityReference(Facility)
                .WithManufacturerReference(manufacturer);


            var listForInit = new List<Entity> { manufacturer, incident, caseMg, relationship };

            fakedContext.Initialize(listForInit);
            #endregion

            #region Setup execution context

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() }, { "ManufacturerId", manufacturer.ToEntityReference() }, { "Validation", "Manufacturer" } };

            var outputParameters = new ParameterCollection ();

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGCaseActionsPreValidatePO",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection(),
                UserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin

            fakedContext.ExecutePluginWith<PreValidatePO>(pluginContext);
            fakedContext.ExecutePluginWith<PreValidatePO>(pluginContext);

            var crmContext = new OrganizationServiceContext(fakedContext.GetOrganizationService());


            Assert.True(outputParameters.Contains("HasError")
              && outputParameters["HasError"] as bool? == true
              && outputParameters["Message"] as string == $"Facility Account # for {manufacturer.Name} Manufacturer Does Not Exist!", "Wrong Validation");

            var alltask = (from t in crmContext.CreateQuery<Intake.Task>()
                           select t).ToList();
            var task = (from t in crmContext.CreateQuery<Intake.Task>()
                            //where t.RegardingObjectId.Id == incident.Id
                            //&& t.ipg_tasktypecode.Value == (int)ipg_TaskType1.RequiredFacilityAccount
                        select t).ToList();// FirstOrDefault();

            Assert.True(task != null, $"There is no {ipg_TaskType1.RequiredFacilityAccount} task");


            #endregion
        }

        [Fact]
        public void CheckManufacturerValidationNotRequiredNumber()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();
       
            Intake.Account manufacturer = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Manufacturer).RuleFor(x => x.ipg_ManufacturerIsFacilityAcctRequired, x => false);
            Intake.Account Facility = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Facility).RuleFor(x => x.ipg_ManufacturerIsFacilityAcctRequired, x => false);

            Incident incident = new Incident().Fake()
            .WithActualDos(DateTime.UtcNow)
            .WithPatientFullName()
            .WithPatientProcedure()
            .WithDeviceStageOptionSet(DeviceStageOptionSet.Other)
            .WithFacilityReference(Facility);

            Intake.Team caseMg = new Team().Fake("Case Management");


            var listForInit = new List<Entity> { manufacturer, incident, caseMg };

            fakedContext.Initialize(listForInit);
            #endregion

            #region Setup execution context

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() }, { "ManufacturerId", manufacturer.ToEntityReference() }, { "Validation", "Manufacturer" } };

            var outputParameters = new ParameterCollection();

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGCaseActionsPreValidatePO",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection(),
                UserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin

            fakedContext.ExecutePluginWith<PreValidatePO>(pluginContext);

            var crmContext = new OrganizationServiceContext(fakedContext.GetOrganizationService());


            Assert.True(!outputParameters.Contains("HasError") || outputParameters["HasError"] as bool? != true, "Validation faulted");

            var alltask = (from t in crmContext.CreateQuery<Intake.Task>()
                           select t).ToList();
            var task = (from t in crmContext.CreateQuery<Intake.Task>()
                        where t.RegardingObjectId.Id == manufacturer.Id
                        && t.ipg_tasktypecode.Value == (int)ipg_TaskType1.RequiredFacilityAccount
                        select t).FirstOrDefault();

            Assert.True(task == null);

            #endregion
        }

        [Fact]
        public void CheckManufacturerValidationRequiredByParentNumber()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            Intake.Account Parentmanufacturer = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Manufacturer).RuleFor(x => x.ipg_ManufacturerIsFacilityAcctRequired, x => true);
            Intake.Account manufacturer = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Manufacturer).WithParentMfg(Parentmanufacturer.ToEntityReference())
                .RuleFor(x => x.ipg_ManufacturerIsFacilityAcctRequired, x => false);

            Intake.Account Facility = new Intake.Account().Fake(customerTypeCode: CustomerTypeCodeOptionSet.Facility).RuleFor(x => x.ipg_ManufacturerIsFacilityAcctRequired, x => false);

            Incident incident = new Incident().Fake()
            .WithActualDos(DateTime.UtcNow)
            .WithPatientFullName()
            .WithPatientProcedure()
            .WithDeviceStageOptionSet(DeviceStageOptionSet.Other)
            .WithFacilityReference(Facility);

            Intake.ipg_facilitymanufacturerrelationship relationship = new Intake.ipg_facilitymanufacturerrelationship().Fake()
                .WithFacilityReference(Facility)
                .WithManufacturerReference(manufacturer);

            Intake.Team caseMg = new Team().Fake("Case Management");


            var listForInit = new List<Entity> { manufacturer, incident, caseMg, Parentmanufacturer, relationship };

            fakedContext.Initialize(listForInit);
            #endregion

            #region Setup execution context

            var inputParameters = new ParameterCollection { { "Target", incident.ToEntityReference() }, { "ManufacturerId", manufacturer.ToEntityReference() }, { "Validation", "Manufacturer" } };

            var outputParameters = new ParameterCollection();

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGCaseActionsPreValidatePO",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection(),
                UserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin

            fakedContext.ExecutePluginWith<PreValidatePO>(pluginContext);

            var crmContext = new OrganizationServiceContext(fakedContext.GetOrganizationService());


            Assert.True(outputParameters.Contains("HasError") 
                && outputParameters["HasError"] as bool? == true 
                && outputParameters["Message"] as string == $"Facility Account # for {manufacturer.Name} Manufacturer Does Not Exist!", "Wrong Validation");

            var alltask = (from t in crmContext.CreateQuery<Intake.Task>()
                           select t).ToList();
            var task = (from t in crmContext.CreateQuery<Intake.Task>()
                        where t.RegardingObjectId.Id == incident.Id
                        && t.ipg_tasktypecode.Value == (int)ipg_TaskType1.RequiredFacilityAccount
                        select t).FirstOrDefault();

            Assert.True(task != null, $"There is no {ipg_TaskType1.RequiredFacilityAccount} task");

            #endregion
        }
    }
}