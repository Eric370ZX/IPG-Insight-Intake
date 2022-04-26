using FakeXrmEasy;
using Insight.Intake.Plugin.Referral;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk.Query;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Referral
{
    public class AssociateDocumentsToReferralPluginTests : PluginTestsBase
    {
        [Fact]
        public void Returns_true_when_referral_has_one_related_document()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            //Create a fake PIF Document and referral. Bind the referral with the document.
            ipg_document pifDocument = new ipg_document().Fake();
            ipg_referral createdReferral = new ipg_referral().Fake().WithSourceDocument(pifDocument);

            var listForInit = new List<Entity> { pifDocument, createdReferral };

            fakedContext.Initialize(listForInit);

            #endregion

            #region Setup execution context

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_referral.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", createdReferral } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin

            fakedContext.ExecutePluginWith<AssociateDocumentsToReferralPlugin>(pluginContext);

            #endregion

            #region Asserts

            var fakedService = fakedContext.GetOrganizationService();

            var query = new QueryExpression(ipg_document.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression( ipg_document.Fields.ipg_ReferralId, ConditionOperator.Equal, createdReferral.ToEntityReference())
                    }
                }
            };
            //retrieve all related documents to Referral
            var relatedDocuments = fakedService.RetrieveMultiple(query).Entities;

            Assert.True(relatedDocuments.Count == 1);

            #endregion
        }

        [Fact]
        public void Returns_true_when_referral_has_multiple_related_documents()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            //Create a fake PIF Document, some other documents and referral. Bind the referral and other documents with the PIF Document.
            ipg_document pifDocument = new ipg_document().Fake();
            List<ipg_document> relatedToPifDocs = new List<ipg_document>(new ipg_document[]
            {
                new ipg_document().Fake().WithMainDocument(pifDocument),
                new ipg_document().Fake().WithMainDocument(pifDocument),
                new ipg_document().Fake().WithMainDocument(pifDocument)
            });
            ipg_referral createdReferral = new ipg_referral().Fake().WithSourceDocument(pifDocument);

            var listForInit = new List<Entity> { pifDocument, createdReferral }.Concat(relatedToPifDocs);

            fakedContext.Initialize(listForInit);

            #endregion

            #region Setup execution context

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_referral.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", createdReferral } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin

            fakedContext.ExecutePluginWith<AssociateDocumentsToReferralPlugin>(pluginContext);

            #endregion

            #region Asserts

            var fakedService = fakedContext.GetOrganizationService();

            var query = new QueryExpression(ipg_document.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression( ipg_document.Fields.ipg_ReferralId, ConditionOperator.Equal, createdReferral.ToEntityReference())
                    }
                }
            };
            //retrieve all related documents to Referral
            var relatedDocuments = fakedService.RetrieveMultiple(query).Entities;

            Assert.True(relatedDocuments.Count == relatedToPifDocs.Count + 1);

            #endregion
        }

        [Fact]
        public void Fails_when_referral_has_no_source_document()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            //Create a fake referral without source document.
            ipg_referral createdReferral = new ipg_referral().Fake();

            var listForInit = new List<Entity> { createdReferral };

            fakedContext.Initialize(listForInit);

            #endregion

            #region Arrange

            var inputParameters = new ParameterCollection() { { "Target", createdReferral } };
            PluginExecutionContextMock.Setup(pec => pec.MessageName).Returns(MessageNames.Create);
            PluginExecutionContextMock.Setup(pec => pec.Stage).Returns(PipelineStages.PostOperation);
            PluginExecutionContextMock.Setup(pec => pec.PrimaryEntityName).Returns(ipg_referral.EntityLogicalName);
            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);
            ServiceProvider = ServiceProviderMock.Object;

            #endregion

            #region Asserts

            var plugin = new AssociateDocumentsToReferralPlugin();

            Assert.ThrowsAny<InvalidPluginExecutionException>(() => plugin.Execute(ServiceProvider));

            #endregion
        }
        [Fact]
        public void Executes_without_any_exception_when_has_no_portal_user()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            ipg_document pifDocument = new ipg_document().Fake();
            ipg_referral createdReferral = new ipg_referral().Fake().WithSourceDocument(pifDocument).WithPortalUser();

            var listForInit = new List<Entity> { pifDocument, createdReferral };

            fakedContext.Initialize(listForInit);

            #endregion

            #region Setup execution context

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_referral.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", createdReferral } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin

            fakedContext.ExecutePluginWith<AssociateDocumentsToReferralPlugin>(pluginContext);

            #endregion

            #region Assert
            // No asserts, just executes and return nothing
            #endregion
        }

        [Fact]
        public void Create_important_event_log()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            var referral = new ipg_referral().Fake().Generate();
            var pifDoc = new ipg_document().Fake().Generate();
            referral.ipg_SourceDocumentId = pifDoc.ToEntityReference();

            var fakedImportantEventConfig = new ipg_importanteventconfig()
            {
                Id = new Guid(),
                ipg_name = "ET1",
                ipg_eventdescription = "Referral documents received",
                ipg_eventtype = "Referral Documents Received"
            };

            var fakedActivityType = new ipg_activitytype()
            {
                Id = new Guid(),
                ipg_name = "Referral Documents Received"
            };

            fakedService.Create(fakedImportantEventConfig);
            fakedService.Create(fakedActivityType);

            var listForInit = new List<Entity>() { pifDoc, referral };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", referral } };
            #endregion

            #region Setup execution context
            var fakedPluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_referral.EntityLogicalName,
                InputParameters = inputParameters,
                PreEntityImages = null,
                PostEntityImages = null
            };
            #endregion

            #region Act
            fakedContext.ExecutePluginWith<AssociateDocumentsToReferralPlugin>(fakedPluginContext);
            var query = new QueryExpression(ipg_importanteventslog.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(false)
            };
            var resultLog = fakedService.RetrieveMultiple(query).Entities.SingleOrDefault();
            #endregion

            #region Act
            Assert.NotNull(resultLog);
            #endregion
        }
    }
}
