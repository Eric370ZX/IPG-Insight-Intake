using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class ValidateRequiredDocumentTest : PluginTestsBase
    {
        [Fact]
        public void ValidateRequiredDocumentsAreFetchedFromAccount1()
        {
            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;

            ipg_documenttype fakeDocumentType1 = new ipg_documenttype().Fake("DocType 1");
            ipg_documenttype fakeDocumentType2 = new ipg_documenttype().Fake("DocType 2");

            Intake.Account fakeCarrier = new Intake.Account().Fake();

            Incident fakeIncident = new Incident().Fake().
                                        WithCarrierReference(fakeCarrier);

            OrganizationServiceMock.WithRetrieveCrud(fakeIncident);
            OrganizationServiceMock.WithRetrieveCrud(fakeCarrier);

            OrganizationServiceMock.WithRetrieveMultipleCrud(ipg_documenttype.EntityLogicalName,
                new List<Entity>() { fakeDocumentType1, fakeDocumentType2 });

            OrganizationServiceMock.WithRetrieveMultipleCrud(ipg_document.EntityLogicalName,
                                                             new List<Entity>());
            OrganizationServiceMock.WithRetrieveMultipleCrud(ipg_requiredinformation.EntityLogicalName,
                                                             new List<Entity>());
            OrganizationServiceMock.WithRetrieveMultipleCrud(Task.EntityLogicalName,
                                                             new List<Entity>());

            OrganizationService = OrganizationServiceMock.Object;
            #endregion

            #region Setup execution context

            var inputTarget = new EntityReference(Incident.EntityLogicalName, fakeIncident.Id);
            var inputParameters = new ParameterCollection
            {
                {"Target", inputTarget}
            };

            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            List<Task> tasksCreated = new List<Task>();

            OrganizationServiceMock.Setup(t =>
                t.Create(It.Is<Entity>(e => e.LogicalName.ToUpper() == Task.EntityLogicalName.ToUpper()))) //only match an entity with a logical name of "task"
                .Returns(Guid.NewGuid()) //can return any guid here
                .Callback<Task>(s => tasksCreated.Add(s)); //store the Create method invocation parameter for inspection later

            List<ipg_requiredinformation> requiredInformations = new List<ipg_requiredinformation>();

            OrganizationServiceMock.Setup(t =>
                t.Create(It.Is<Entity>(e => e.LogicalName.ToUpper() == ipg_requiredinformation.EntityLogicalName.ToUpper()))) //only match an entity with a logical name of "task"
                .Returns(Guid.NewGuid()) //can return any guid here
                .Callback<ipg_requiredinformation>(s => requiredInformations.Add(s)); //store the Create method invocation parameter for inspection later
            #endregion

            #region Execute Plugin

            var plugin = new ValidateRequiredDocuments();

            plugin.Execute(ServiceProvider);

            #endregion

            #region Assert

            Assert.Contains(tasksCreated, t => t.Subject.Contains("DocType 1"));
            Assert.Contains(tasksCreated, t => t.Subject.Contains("DocType 2"));
            Assert.All(tasksCreated, t => Assert.Equal(t.RegardingObjectId.Id, fakeIncident.Id));
            Assert.All(requiredInformations, r => Assert.Equal(r.ipg_CaseId.Id, fakeIncident.Id));
            Assert.Contains(requiredInformations, ri => ri.ipg_DocumentTypeId.Id == fakeDocumentType1.Id);
            Assert.Contains(requiredInformations, ri => ri.ipg_DocumentTypeId.Id == fakeDocumentType2.Id);
            #endregion
        }


        [Fact]
        public void ValidateRequiredDocumentsAreFetchedFromAccount2()
        {
            #region Setup services
            ServiceProvider = ServiceProviderMock.Object;

            ipg_documenttype fakeDocumentType1 = new ipg_documenttype().Fake("DocType 1");
            ipg_documenttype fakeDocumentType2 = new ipg_documenttype().Fake("DocType 2");

            ipg_document fakeDocument1 = new ipg_document().Fake().WithDocumentTypeReference(fakeDocumentType1);

            Intake.Account fakeCarrier = new Intake.Account().Fake();

            Incident fakeIncident = new Incident().Fake().
                                        WithCarrierReference(fakeCarrier);

            OrganizationServiceMock.WithRetrieveCrud(fakeIncident);
            OrganizationServiceMock.WithRetrieveCrud(fakeCarrier);

            OrganizationServiceMock.WithRetrieveMultipleCrud(ipg_documenttype.EntityLogicalName,
                new List<Entity>() { fakeDocumentType1, fakeDocumentType2 });

            OrganizationServiceMock.WithRetrieveMultipleCrud(ipg_document.EntityLogicalName,
                                                             new List<Entity>() { fakeDocument1 });
            OrganizationServiceMock.WithRetrieveMultipleCrud(ipg_requiredinformation.EntityLogicalName,
                                                             new List<Entity>());
            OrganizationServiceMock.WithRetrieveMultipleCrud(Task.EntityLogicalName,
                                                             new List<Entity>());

            OrganizationService = OrganizationServiceMock.Object;
            #endregion

            #region Setup execution context

            var inputTarget = new EntityReference(Incident.EntityLogicalName, fakeIncident.Id);
            var inputParameters = new ParameterCollection
            {
                {"Target", inputTarget}
            };

            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            List<Task> tasksCreated = new List<Task>();

            OrganizationServiceMock.Setup(t =>
                t.Create(It.Is<Entity>(e => e.LogicalName.ToUpper() == Task.EntityLogicalName.ToUpper()))) //only match an entity with a logical name of "task"
                .Returns(Guid.NewGuid()) //can return any guid here
                .Callback<Task>(s => tasksCreated.Add(s)); //store the Create method invocation parameter for inspection later

            List<ipg_requiredinformation> requiredInformations = new List<ipg_requiredinformation>();

            OrganizationServiceMock.Setup(t =>
                t.Create(It.Is<Entity>(e => e.LogicalName.ToUpper() == ipg_requiredinformation.EntityLogicalName.ToUpper()))) //only match an entity with a logical name of "task"
                .Returns(Guid.NewGuid()) //can return any guid here
                .Callback<ipg_requiredinformation>(s => requiredInformations.Add(s)); //store the Create method invocation parameter for inspection later
            #endregion

            #region Execute Plugin

            var plugin = new ValidateRequiredDocuments();

            plugin.Execute(ServiceProvider);

            #endregion

            #region Assert

            Assert.DoesNotContain(tasksCreated, t => t.Subject.Contains("DocType 1"));
            Assert.Contains(tasksCreated, t => t.Subject.Contains("DocType 2"));
            Assert.All(tasksCreated, t => Assert.Equal(t.RegardingObjectId.Id, fakeIncident.Id));
            Assert.All(requiredInformations, r => Assert.Equal(r.ipg_CaseId.Id, fakeIncident.Id));
            Assert.DoesNotContain(requiredInformations, ri => ri.ipg_DocumentTypeId.Id == fakeDocumentType1.Id);
            Assert.Contains(requiredInformations, ri => ri.ipg_DocumentTypeId.Id == fakeDocumentType2.Id);
            #endregion
        }
    }
}
