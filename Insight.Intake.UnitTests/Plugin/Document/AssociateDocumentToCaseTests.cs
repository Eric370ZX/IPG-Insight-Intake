using FakeXrmEasy;
using Insight.Intake.Extensions;
using Insight.Intake.Plugin.Document;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk.Query;
using Xunit;
using FakeXrmEasy.FakeMessageExecutors;
using Microsoft.Xrm.Sdk.Messages;

namespace Insight.Intake.UnitTests.Plugin.Document
{
    public class AssociateDocumentToCaseTests : PluginTestsBase
    {
        [Fact]
        public void CheckAssociateInvoiceDocumentToCasePreCreate_CaseHasNotParts_returnException()
        {
            var fakedContext = new XrmFakedContext();

            ipg_documenttype documentType = new ipg_documenttype().Fake("IPG Invoice");
            Incident caseEntity = new Incident().Fake();
            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithCaseReference(caseEntity);


            var listForInit = new List<Entity> { documentType, document, caseEntity };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", document } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 20,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            Exception ex = Assert.Throws<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<AssociateDocumentToCase>(pluginContext));
            Assert.Equal("Error: Case does not have associated parts!", ex.Message);
        }

        [Fact]
        public void CheckAssociateInvoiceDocumentToCasePreCreate_CaseHasPart()
        {
            var fakedContext = new XrmFakedContext();

            ipg_documenttype documentType = new ipg_documenttype().Fake("IPG Invoice");
            Incident caseEntity = new Incident().Fake();

            ipg_casepartdetail part = new ipg_casepartdetail().Fake()
                .WithCaseReference(caseEntity);

            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithCaseReference(caseEntity);

            var listForInit = new List<Entity> { documentType, document, caseEntity, part };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", document } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 20,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<AssociateDocumentToCase>(pluginContext);
        }

        [Fact]
        public void CheckAssociateInvoiceDocumentToCasePreUpdate_CaseHasNotParts_returnException()
        {
            var fakedContext = new XrmFakedContext();

            ipg_documenttype documentType = new ipg_documenttype().Fake("IPG Invoice");

            Incident caseEntity = new Incident().Fake();

            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithCaseReference(caseEntity);


            var listForInit = new List<Entity> { documentType, document, caseEntity };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", new ipg_document { Id = document.Id, ipg_CaseId = document.ipg_CaseId } } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 20,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = new EntityImageCollection { { "PreImage", document } }
            };

            Exception ex = Assert.Throws<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<AssociateDocumentToCase>(pluginContext));
            Assert.Equal("Error: Case does not have associated parts!", ex.Message);
        }

        [Fact]
        public void CheckThatDocBindedToPatient()
        {
            var fakedContext = new XrmFakedContext();

            ipg_documenttype documentType = new ipg_documenttype().Fake("IPG Invoice");
            Contact patient = new Contact().Fake();
            Incident caseEntity = new Incident().Fake().WithPatientReference(patient);

            ipg_casepartdetail part = new ipg_casepartdetail().Fake()
                .WithCaseReference(caseEntity);

            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithCaseReference(caseEntity);

            var listForInit = new List<Entity> { documentType, document, caseEntity, part, patient };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", document } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 20,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<AssociateDocumentToCase>(pluginContext);

            Assert.Equal(document.ipg_patientid, patient.ToEntityReference());
        }

        [Fact]
        public void Completes_Open_DocumentRequired_Tasks()
        {
            //Arrange

            ipg_documenttype documentType = new ipg_documenttype().Fake("Medical Records");
            Incident caseEntity = new Incident().Fake();
            ipg_casepartdetail part = new ipg_casepartdetail().Fake()
                .WithCaseReference(caseEntity);
            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithCaseReference(caseEntity);
            Task openTask1 = new Task().Fake()
                .WithDocumentType(documentType.ToEntityReference())
                .WithRegarding(caseEntity.ToEntityReference())
                .WithCase(caseEntity.ToEntityReference())
                .WithState(TaskState.Open)
                .WithStatus(Task_StatusCode.NotStarted);
            Task openTask2 = new Task().Fake()
                .WithDocumentType(documentType.ToEntityReference())
                .WithRegarding(caseEntity.ToEntityReference())
                .WithCase(caseEntity.ToEntityReference())
                .WithState(TaskState.Open)
                .WithStatus(Task_StatusCode.NotStarted);

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(
                new List<Entity> { documentType, document, caseEntity, part, openTask1, openTask2 }
            );
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = new ParameterCollection { { "Target", document } },
                PreEntityImages = new EntityImageCollection() { { "PreImage", document } }
            };


            //Act

            fakedContext.ExecutePluginWith<AssociateDocumentToCase>(pluginContext);


            //Assert

            var organizationService = fakedContext.GetOrganizationService();
            var orgServiceContext = new OrganizationServiceContext(organizationService);

            var updatedTasks = (from task in orgServiceContext.CreateQuery<Task>()
                                where task.RegardingObjectId.Id == caseEntity.Id
                                 && task.ipg_DocumentType.Id == documentType.Id
                                select task).ToList();

            var updatedTask1 = updatedTasks.FirstOrDefault(t => t.Id == openTask1.Id);
            Assert.Equal(TaskState.Completed, updatedTask1.StateCode);
            Assert.Equal((int)Task_StatusCode.Resolved, updatedTask1.StatusCode.Value);

            var updatedTask2 = updatedTasks.FirstOrDefault(t => t.Id == openTask2.Id);
            Assert.Equal(TaskState.Completed, updatedTask2.StateCode);
            Assert.Equal((int)Task_StatusCode.Resolved, updatedTask2.StatusCode.Value);

            var openTasks = updatedTasks.Where(t => t.StateCode == TaskState.Open);

            Assert.Empty(openTasks);
        }

        [Fact]
        public void Creates_DocumentRequired_Task_If_No_Open_Tasks()
        {
            //Arrange

            ipg_documenttype documentType = new ipg_documenttype().Fake("Medical Records");
            Incident caseEntity = new Incident().Fake();
            ipg_casepartdetail part = new ipg_casepartdetail().Fake()
                .WithCaseReference(caseEntity);
            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithCaseReference(caseEntity);

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(
                new List<Entity> { documentType, document, caseEntity, part }
            );

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = new ParameterCollection { { "Target", document } },
                PreEntityImages = new EntityImageCollection() { { "PreImage", document } }
            };


            //Act

            fakedContext.ExecutePluginWith<AssociateDocumentToCase>(pluginContext);


            //Assert

            var organizationService = fakedContext.GetOrganizationService();
            var orgServiceContext = new OrganizationServiceContext(organizationService);

            var tasks = (from task in orgServiceContext.CreateQuery<Task>()
                         where task.RegardingObjectId.Id == caseEntity.Id
                          && task.ipg_caseid.Id == caseEntity.Id
                          && task.ipg_DocumentType.Id == documentType.Id
                          && task.Subject == documentType.ipg_name + " Document Required"
                          && task.Subcategory == documentType.ipg_name
                          && task.ScheduledStart == DateTime.Today
                          && task.ScheduledEnd == DateTime.Today
                          && task.StateCode == TaskState.Completed
                         select task).ToList();

            Assert.Single(tasks);
        }

        [Fact]
        public void Does_not_process_DocumentRequired_tasks_of_generic_documents()
        {
            //Arrange

            ipg_documenttype documentType = new ipg_documenttype().Fake("Portal Generic Document");
            Incident caseEntity = new Incident().Fake();
            ipg_casepartdetail part = new ipg_casepartdetail().Fake()
                .WithCaseReference(caseEntity);
            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithCaseReference(caseEntity);

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(
                new List<Entity> { documentType, document, caseEntity, part }
            );

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = new ParameterCollection { { "Target", document } },
                PreEntityImages = new EntityImageCollection() { { "PreImage", document } }
            };


            //Act

            fakedContext.ExecutePluginWith<AssociateDocumentToCase>(pluginContext);


            //Assert

            var organizationService = fakedContext.GetOrganizationService();
            var orgServiceContext = new OrganizationServiceContext(organizationService);

            var tasks = (from task in orgServiceContext.CreateQuery<Task>()
                         select task).ToList();

            Assert.Empty(tasks);
        }

        [Fact]
        public void OldCaseReferenceStoredInNxNRelationShipOnReassign()
        {
            //Arrange

            ipg_documenttype documentType = new ipg_documenttype().Fake("MFG INV");
            Incident caseEntity = new Incident().Fake();
            Incident case2Entity = new Incident().Fake();
            ipg_casepartdetail part = new ipg_casepartdetail().Fake()
                .WithCaseReference(caseEntity);
            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithCaseReference(caseEntity);

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(
                new List<Entity> { documentType, document, caseEntity, part, case2Entity }
            );
            fakedContext.AddRelationship(ipg_incident_ipg_document.EntityLogicalName, new XrmFakedRelationship
            {
                IntersectEntity = ipg_incident_ipg_document.EntityLogicalName,
                Entity1LogicalName = ipg_document.EntityLogicalName,
                Entity1Attribute = ipg_document.PrimaryIdAttribute,
                Entity2LogicalName = Incident.EntityLogicalName,
                Entity2Attribute = Incident.PrimaryIdAttribute
            });

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", new ipg_document() { Id = document.Id, ipg_CaseId = case2Entity.ToEntityReference() } } },
                PreEntityImages = new EntityImageCollection() { { "PreImage", document } }
            };


            //Act

            fakedContext.ExecutePluginWith<AssociateDocumentToCase>(pluginContext);


            //Assert

            var organizationService = fakedContext.GetOrganizationService();
            var orgServiceContext = new OrganizationServiceContext(organizationService);

            var doc = (from incidentdoc in orgServiceContext.CreateQuery<ipg_incident_ipg_document>()
                       select incidentdoc).FirstOrDefault();

            Assert.NotNull(doc);
        }

        [Fact]
        public void DocumentWithTypeThatAllowsMultipleAssociation_success()
        {
            //Arrange
            ipg_documenttype documentType = new ipg_documenttype().Fake("Manufacturer Invoice", "MFG INV");
            Incident caseEntity = new Incident().Fake();
            Incident caseEntity2 = new Incident().Fake();
            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType);

            var fakedContext = new XrmFakedContext();

            fakedContext.AddRelationship(ipg_incident_ipg_document.EntityLogicalName, new XrmFakedRelationship
            {
                IntersectEntity = ipg_incident_ipg_document.EntityLogicalName,
                Entity1LogicalName = ipg_document.EntityLogicalName,
                Entity1Attribute = ipg_document.PrimaryIdAttribute,
                Entity2LogicalName = Incident.EntityLogicalName,
                Entity2Attribute = Incident.PrimaryIdAttribute
            });

            fakedContext.Initialize(new List<Entity> { documentType, document, caseEntity, caseEntity2 });

            var organizationService = fakedContext.GetOrganizationService();

            AssociateRequest assosiateRequest = new AssociateRequest()
            {
                RelatedEntities = new EntityReferenceCollection()
                {
                    caseEntity.ToEntityReference(),
                    caseEntity2.ToEntityReference()
                },
                Relationship = new Relationship(ipg_incident_ipg_document.EntityLogicalName),
                Target = document.ToEntityReference()
            };

            organizationService.Execute(assosiateRequest);

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Associate,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = new ParameterCollection() {
                    { "RelatedEntities", new EntityReferenceCollection(){ (new ipg_document() { Id = document.Id }).ToEntityReference()}},
                    { "Relationship", new Relationship(ipg_document.Relationships.ipg_incident_ipg_document) }
                }
            };

            //Act
            fakedContext.ExecutePluginWith<AssociateDocumentToCase>(pluginContext);

            //Assert
            QueryExpression query = new QueryExpression(Incident.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                LinkEntities =
                {
                    new LinkEntity
                    {
                        LinkFromEntityName = Incident.EntityLogicalName,
                        LinkToEntityName = ipg_document.Relationships.ipg_incident_ipg_document,
                        LinkFromAttributeName = Incident.Fields.Id,
                        LinkToAttributeName = Incident.Fields.Id,
                        LinkEntities =
                        {
                            new LinkEntity
                            {
                                LinkFromEntityName = ipg_document.Relationships.ipg_incident_ipg_document,
                                LinkToEntityName = ipg_document.EntityLogicalName,
                                LinkFromAttributeName = ipg_document.Fields.Id,
                                LinkToAttributeName = ipg_document.Fields.Id,
                                LinkCriteria = new FilterExpression
                                {
                                    Conditions =
                                    {
                                        new ConditionExpression(ipg_document.Fields.Id, ConditionOperator.Equal, document.Id)
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var casesCount = organizationService.RetrieveMultiple(query).Entities.Count;
            Assert.Equal(2, casesCount);
        }
        [Fact]
        public void DocumentWithTypeThatDontAllowMultipleAssociation_throwsException()
        {
            //Arrange
            ipg_documenttype documentType = new ipg_documenttype().Fake("Benefits Verification Form", "BVF");
            Incident caseEntity = new Incident().Fake();
            Incident caseEntity2 = new Incident().Fake();
            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType);

            var fakedContext = new XrmFakedContext();

            fakedContext.AddRelationship(ipg_incident_ipg_document.EntityLogicalName, new XrmFakedRelationship
            {
                IntersectEntity = ipg_incident_ipg_document.EntityLogicalName,
                Entity1LogicalName = ipg_document.EntityLogicalName,
                Entity1Attribute = ipg_document.PrimaryIdAttribute,
                Entity2LogicalName = Incident.EntityLogicalName,
                Entity2Attribute = Incident.PrimaryIdAttribute
            });

            fakedContext.Initialize(new List<Entity> { documentType, document, caseEntity, caseEntity2 });

            var organizationService = fakedContext.GetOrganizationService();

            AssociateRequest assosiateRequest = new AssociateRequest()
            {
                RelatedEntities = new EntityReferenceCollection()
                {
                    caseEntity.ToEntityReference(),
                    caseEntity2.ToEntityReference()
                },
                Relationship = new Relationship(ipg_incident_ipg_document.EntityLogicalName),
                Target = document.ToEntityReference()
            };

            organizationService.Execute(assosiateRequest);

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Associate,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = new ParameterCollection() {
                    { "RelatedEntities", new EntityReferenceCollection(){ (new ipg_document() { Id = document.Id }).ToEntityReference()}},
                    { "Relationship", new Relationship(ipg_document.Relationships.ipg_incident_ipg_document) }
                }
            };

            //Act & Assert
            Assert.ThrowsAny<Exception>(() => fakedContext.ExecutePluginWith<AssociateDocumentToCase>(pluginContext));
        }
    }
}
