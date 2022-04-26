using FakeXrmEasy;
using Insight.Intake.Plugin.Note;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Note
{
    public class CreateDocumentTest : PluginTestsBase
    {
        [Fact]
        public void Returns_true_when_case_has_one_related_document_with_populated_fields()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();
            var nowDate = DateTime.Now;
            var fileName = "test.pdf";
            var documentBody = "test body";

            //Create a fake Annotation, SalesOrder, Document Type and Case. Bind the Salesorder with the Case, and Annotation with SalesOrder.
            ipg_documenttype DocumentType = new ipg_documenttype().Fake("Implant Charge Sheet", "ICS");
            Incident incident = new Incident().Fake();
            Task task = new Task().Fake().WithSubCategory("ICS").WithCase(incident.ToEntityReference());
            Annotation createdNote = new Annotation().Fake()
                .WithDocument(fileName, documentBody)
                .WithObjectReference(task);

            var listForInit = new List<Entity> { DocumentType, incident, task, createdNote };

            fakedContext.Initialize(listForInit);

            #endregion

            #region Setup execution context

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Annotation.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", createdNote } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin

            fakedContext.ExecutePluginWith<CreateDocumentFromNote>(pluginContext);

            #endregion

            #region Asserts

            var fakedService = fakedContext.GetOrganizationService();

            var query = new QueryExpression(ipg_document.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression( ipg_document.Fields.ipg_CaseId, ConditionOperator.Equal, incident.ToEntityReference())
                    }
                }
            };
            //retrieve all related documents to Referral
            var createdDocument = fakedService.RetrieveMultiple(query).Entities.FirstOrDefault()?.ToEntity<ipg_document>();

            Assert.Equal(documentBody, createdDocument.ipg_documentbody);
            Assert.Equal(fileName, createdDocument.ipg_FileName);
            Assert.Equal(DocumentType.ToEntityReference(), createdDocument.ipg_DocumentTypeId);
            Assert.Equal(1, createdDocument.ipg_Revision);
            #endregion
        }
    }
}
