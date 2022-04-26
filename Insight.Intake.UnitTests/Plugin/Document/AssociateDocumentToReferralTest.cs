
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

    public class AssociateDocumentToReferralTest : PluginTestsBase
    {
        [Fact]
        public void UpdateOwnerFromAssosiatedReferral_shouldReturnTrue()
        {
            var fakedContext = new XrmFakedContext();

            SystemUser user = new SystemUser().Fake("Test");
            ipg_referral referralEntity = new ipg_referral().Fake().WithOwner(user);
            
            ipg_document document = new ipg_document().Fake()
                .WithReferralReference(referralEntity);


            var listForInit = new List<Entity> {user, document, referralEntity };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", document } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 20,
                Depth = 1,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<AssociateDocumentToReferral>(pluginContext);

            Assert.True(document.OwnerId.Id == user.Id);
        }

        [Fact]
        public void DocumentWithTypeThatAllowsMultipleAssociation_success()
        {
            //Arrange
            ipg_documenttype documentType = new ipg_documenttype().Fake("Manufacturer Invoice", "MFG INV");
            ipg_referral referralEntity = new ipg_referral().Fake();
            ipg_referral referralEntity2 = new ipg_referral().Fake();
            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType);

            var fakedContext = new XrmFakedContext();

            fakedContext.AddRelationship(ipg_ipg_document_ipg_referral.EntityLogicalName, new XrmFakedRelationship
            {
                IntersectEntity = ipg_ipg_document_ipg_referral.EntityLogicalName,
                Entity1LogicalName = ipg_document.EntityLogicalName,
                Entity1Attribute = ipg_document.PrimaryIdAttribute,
                Entity2LogicalName = ipg_referral.EntityLogicalName,
                Entity2Attribute = ipg_referral.PrimaryIdAttribute
            });

            fakedContext.Initialize(new List<Entity> { documentType, document, referralEntity, referralEntity2 });

            var organizationService = fakedContext.GetOrganizationService();

            AssociateRequest assosiateRequest = new AssociateRequest()
            {
                RelatedEntities = new EntityReferenceCollection()
                {
                    referralEntity.ToEntityReference(),
                    referralEntity2.ToEntityReference()
                },
                Relationship = new Relationship(ipg_ipg_document_ipg_referral.EntityLogicalName),
                Target = document.ToEntityReference()
            };

            organizationService.Execute(assosiateRequest);

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Associate,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = new ParameterCollection() {
                    { "Target", document.ToEntityReference() },
                    { "Relationship", new Relationship(ipg_document.Relationships.ipg_ipg_document_ipg_referral) }
                }
            };

            //Act
            fakedContext.ExecutePluginWith<AssociateDocumentToReferral>(pluginContext);

            //Assert
            QueryExpression query = new QueryExpression(ipg_referral.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                LinkEntities =
                {
                    new LinkEntity
                    {
                        LinkFromEntityName = ipg_referral.EntityLogicalName,
                        LinkToEntityName = ipg_document.Relationships.ipg_ipg_document_ipg_referral,
                        LinkFromAttributeName = ipg_referral.Fields.Id,
                        LinkToAttributeName = ipg_referral.Fields.Id,
                        LinkEntities =
                        {
                            new LinkEntity
                            {
                                LinkFromEntityName = ipg_document.Relationships.ipg_ipg_document_ipg_referral,
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

            var referralsCount = organizationService.RetrieveMultiple(query).Entities.Count;
            Assert.Equal(2, referralsCount);
        }
        [Fact]
        public void DocumentWithTypeThatDontAllowMultipleAssociation_throwsException()
        {
            //Arrange
            ipg_documenttype documentType = new ipg_documenttype().Fake("Benefits Verification Form", "BVF");
            ipg_referral referralEntity = new ipg_referral().Fake();
            ipg_referral referralEntity2 = new ipg_referral().Fake();
            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType);

            var fakedContext = new XrmFakedContext();

            fakedContext.AddRelationship(ipg_ipg_document_ipg_referral.EntityLogicalName, new XrmFakedRelationship
            {
                IntersectEntity = ipg_ipg_document_ipg_referral.EntityLogicalName,
                Entity1LogicalName = ipg_document.EntityLogicalName,
                Entity1Attribute = ipg_document.PrimaryIdAttribute,
                Entity2LogicalName = ipg_referral.EntityLogicalName,
                Entity2Attribute = ipg_referral.PrimaryIdAttribute
            });

            fakedContext.Initialize(new List<Entity> { documentType, document, referralEntity, referralEntity2 });

            var organizationService = fakedContext.GetOrganizationService();

            AssociateRequest assosiateRequest = new AssociateRequest()
            {
                RelatedEntities = new EntityReferenceCollection()
                {
                    referralEntity.ToEntityReference(),
                    referralEntity2.ToEntityReference()
                },
                Relationship = new Relationship(ipg_ipg_document_ipg_referral.EntityLogicalName),
                Target = document.ToEntityReference()
            };

            organizationService.Execute(assosiateRequest);

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Associate,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = new ParameterCollection() {
                    { "Target", document.ToEntityReference() },
                    { "Relationship", new Relationship(ipg_document.Relationships.ipg_ipg_document_ipg_referral) }
                }
            };

            //Act & Assert
            Assert.ThrowsAny<Exception>(() => fakedContext.ExecutePluginWith<AssociateDocumentToReferral>(pluginContext));
        }
    }
}
