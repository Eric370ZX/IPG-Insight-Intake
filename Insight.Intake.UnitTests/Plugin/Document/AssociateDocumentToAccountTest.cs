using FakeXrmEasy;
using Insight.Intake.Plugin.Document;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Document
{
    public class AssociateDocumentToAccountTest : PluginTestsBase
    {        
        [Fact]
        public void DocumentWithTypeThatAllowsMultipleAssociation_success()
        {
            //Arrange
            ipg_documenttype documentType = new ipg_documenttype().Fake("Business Associate Agreement", "BAA");
            Intake.Account accountEntity = new Intake.Account().Fake();
            Intake.Account accountEntity2 = new Intake.Account().Fake();
            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType);

            var fakedContext = new XrmFakedContext();

            fakedContext.AddRelationship(ipg_ipg_document_account.EntityLogicalName, new XrmFakedRelationship
            {
                IntersectEntity = ipg_ipg_document_account.EntityLogicalName,
                Entity1LogicalName = ipg_document.EntityLogicalName,
                Entity1Attribute = ipg_document.PrimaryIdAttribute,
                Entity2LogicalName = Intake.Account.EntityLogicalName,
                Entity2Attribute = Intake.Account.PrimaryIdAttribute
            });

            fakedContext.Initialize(new List<Entity> { documentType, document, accountEntity, accountEntity2 });

            var organizationService = fakedContext.GetOrganizationService();

            AssociateRequest assosiateRequest = new AssociateRequest()
            {
                RelatedEntities = new EntityReferenceCollection()
                {
                    accountEntity.ToEntityReference(),
                    accountEntity2.ToEntityReference()
                },
                Relationship = new Relationship(ipg_ipg_document_account.EntityLogicalName),
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
                    { "Relationship", new Relationship(ipg_document.Relationships.ipg_ipg_document_account) }
                }
            };

            //Act
            fakedContext.ExecutePluginWith<AssociateDocumentToAccount>(pluginContext);

            //Assert
            QueryExpression query = new QueryExpression(Intake.Account.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                LinkEntities =
                {
                    new LinkEntity
                    {
                        LinkFromEntityName = Intake.Account.EntityLogicalName,
                        LinkToEntityName = ipg_document.Relationships.ipg_ipg_document_account,
                        LinkFromAttributeName = Intake.Account.Fields.Id,
                        LinkToAttributeName = Intake.Account.Fields.Id,
                        LinkEntities =
                        {
                            new LinkEntity
                            {
                                LinkFromEntityName = ipg_document.Relationships.ipg_ipg_document_account,
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

            var accountsCount = organizationService.RetrieveMultiple(query).Entities.Count;
            Assert.Equal(2, accountsCount);
        }
        [Fact]
        public void DocumentWithTypeThatDontAllowMultipleAssociation_throwsException()
        {
            //Arrange
            ipg_documenttype documentType = new ipg_documenttype().Fake("Data Integration Form", "DIF");
            Intake.Account accountEntity = new Intake.Account().Fake();
            Intake.Account accountEntity2 = new Intake.Account().Fake();
            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType);

            var fakedContext = new XrmFakedContext();

            fakedContext.AddRelationship(ipg_ipg_document_account.EntityLogicalName, new XrmFakedRelationship
            {
                IntersectEntity = ipg_ipg_document_account.EntityLogicalName,
                Entity1LogicalName = ipg_document.EntityLogicalName,
                Entity1Attribute = ipg_document.PrimaryIdAttribute,
                Entity2LogicalName = Intake.Account.EntityLogicalName,
                Entity2Attribute = Intake.Account.PrimaryIdAttribute
            });

            fakedContext.Initialize(new List<Entity> { documentType, document, accountEntity, accountEntity2 });

            var organizationService = fakedContext.GetOrganizationService();

            AssociateRequest assosiateRequest = new AssociateRequest()
            {
                RelatedEntities = new EntityReferenceCollection()
                {
                    accountEntity.ToEntityReference(),
                    accountEntity2.ToEntityReference()
                },
                Relationship = new Relationship(ipg_ipg_document_account.EntityLogicalName),
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
                    { "Relationship", new Relationship(ipg_document.Relationships.ipg_ipg_document_account) }
                }
            };

            //Act & Assert
            Assert.ThrowsAny<Exception>(() => fakedContext.ExecutePluginWith<AssociateDocumentToAccount>(pluginContext));
        }

    }
}
