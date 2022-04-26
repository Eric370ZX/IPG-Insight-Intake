extern alias PdfSharpUnitTests;

using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Insight.Intake.UnitTests.Fakes;
using Moq;
using Microsoft.Xrm.Sdk.Query;
using Insight.Intake.Plugin.Document;
using System.IO;
using PdfSharpUnitTests::PdfSharp.Pdf;
using PdfSharpUnitTests::PdfSharp.Pdf.IO;
using Insight.Intake.UnitTests.Helpers;
using Microsoft.Crm.Sdk.Messages;

namespace Insight.Intake.UnitTests.Plugin.Document
{
    public class MergeDocumentPluginTests : PluginTestsBase
    {
        private static readonly string CurrentPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\Plugin\Document\MergeDocumentsPlugin";
        private readonly MergeDocumentsPlugin _plugin = new MergeDocumentsPlugin();
        private static class InputParamentersNames
        {
            public static readonly string CaseIdParameterName = "CaseId";
            public static readonly string PackageNameInputParameterName = "PackageName";
            public static readonly string DocIdsInputParameterName = "DocIds";
            public static readonly string DocTypeIdInputParameterName = "TypeId";
            public static readonly string DocDescriptionInputParameterName = "Description";
        }

        [Fact]
        public void Merges_2_docs()
        {
            //ARRANGE

            var caseReference = new EntityReference(Incident.EntityLogicalName, Guid.Parse("fb97b9f9-47f1-45dc-9da7-ec2173a0d193"));
            ipg_documenttype docType = new ipg_documenttype().Fake();

            const string packageName = "packageName1";
            const string pdfFile1Name = "Doc1.pdf";
            const string pdfFile2Name = "Doc2.pdf";

            Guid[] docIds = new[]
            {
                Guid.NewGuid(),
                Guid.NewGuid()
            };

            //todo if needed
            //var ranges = new List<string>
            //{
            //    "1",
            //    "2-3"
            //};

            var inputParameters = new ParameterCollection()
            {
                { InputParamentersNames.CaseIdParameterName, caseReference },
                { InputParamentersNames.PackageNameInputParameterName, packageName },
                { InputParamentersNames.DocIdsInputParameterName, string.Join(",", docIds.Select(g => g.ToString()))},
                { InputParamentersNames.DocTypeIdInputParameterName, docType.Id},
                { InputParamentersNames.DocDescriptionInputParameterName, "Test description"}
            };
            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            DateTime originalDocDate1 = new DateTime(2020, 09, 01, 12, 00, 00);
            DateTime originalDocDate2 = new DateTime(2020, 01, 01, 12, 00, 00);

            var doc1 = SetupDocument(pdfFile1Name, docIds[0], originalDocDate1);
            var doc2 = SetupDocument(pdfFile2Name, docIds[1], originalDocDate2);

            OrganizationServiceMock
                .Setup(x => x.RetrieveMultiple(It.Is<QueryExpression>(qe =>
                    qe.EntityName == ipg_document.EntityLogicalName
                    && qe.TopCount == 1
                    && qe.Criteria.Conditions.Any(c => 
                        c.AttributeName == ipg_document.Fields.ipg_documentId
                        && c.Operator == ConditionOperator.In
                        && (c.Values.Select(y => (Guid)y).Intersect(docIds).Count() == docIds.Length)
                    )
                    && qe.Orders.Count > 0
                        && qe.Orders[0].AttributeName == ipg_document.Fields.ipg_originaldocdate
                        && qe.Orders[0].OrderType == OrderType.Ascending
                    )))
                .Returns(new EntityCollection(new List<ipg_document> { doc2 }.Cast<Entity>().ToList()));

            var assemblyDocType = new ipg_documenttype().Fake("Logistics Package", "PPP").Generate();
            OrganizationServiceMock
                .Setup(x => x.RetrieveMultiple(It.Is<QueryExpression>(ann =>
                    ann.EntityName == ipg_documenttype.EntityLogicalName
                    && ann.Criteria.Conditions.Any(c => c.AttributeName == nameof(ipg_documenttype.ipg_DocumentTypeAbbreviation).ToLower()
                        && c.Operator == ConditionOperator.Equal
                        && c.Values.Count == 1
                        && string.Equals((string)c.Values[0], assemblyDocType.ipg_DocumentTypeAbbreviation, StringComparison.OrdinalIgnoreCase)))))
                .Returns(new EntityCollection(new List<ipg_documenttype> { assemblyDocType }.Cast<Entity>().ToList()));

            //Precreate PPP document
            var existingPPPDoc = new ipg_document().Fake(Guid.Parse("ab97b9f9-47f1-45dc-9da7-ec2173a0d193")).Generate();           
            //Preset RetrieveMultiple result
            OrganizationServiceMock
                .Setup(x => x.RetrieveMultiple(It.Is<QueryExpression>(ann =>
                    ann.EntityName == ipg_document.EntityLogicalName
                    && ann.Criteria.Conditions.Any(c => c.AttributeName == nameof(ipg_document.ipg_CaseId).ToLower()
                        && c.Operator == ConditionOperator.Equal
                        && c.Values.Count == 1
                        && (Guid)c.Values[0] == caseReference.Id )
                    && ann.Criteria.Conditions.Any(c => c.AttributeName == nameof(ipg_document.ipg_DocumentTypeId).ToLower()
                        && c.Operator == ConditionOperator.Equal
                        && c.Values.Count == 1
                        && (Guid)c.Values[0] == assemblyDocType.Id )                        
                        )))
                .Returns(new EntityCollection(new List<ipg_document> { existingPPPDoc }.Cast<Entity>().ToList()));
            
            //Precreate PPP task
            var existingPPPTask = new Task().Fake().Generate();
            //Preset RetrieveMultiple result
            OrganizationServiceMock
                .Setup(x => x.RetrieveMultiple(It.Is<QueryExpression>(ann =>
                    ann.EntityName == Task.EntityLogicalName
                    && ann.Criteria.Conditions.Any(c => c.AttributeName == nameof(Task.RegardingObjectId).ToLower()
                        && c.Operator == ConditionOperator.Equal
                        && c.Values.Count == 1
                        && (Guid)c.Values[0] == caseReference.Id )
                    && ann.Criteria.Conditions.Any(c => c.AttributeName == nameof(Task.StateCode).ToLower()
                        && c.Operator == ConditionOperator.Equal
                        && c.Values.Count == 1
                        && (int)c.Values[0] == 0 )
                    && ann.Criteria.Conditions.Any(c => c.AttributeName == nameof(Task.ipg_tasktypecode).ToLower()
                        && c.Operator == ConditionOperator.Equal
                        && c.Values.Count == 1
                        && (int)c.Values[0] == 427880029 )
                        )))
                .Returns(new EntityCollection(new List<Task> { existingPPPTask }.Cast<Entity>().ToList()));

            Guid newDocId = Guid.NewGuid();
            OrganizationServiceMock
                .Setup(x => x.Create(It.Is<ipg_document>(d => d.ipg_DocumentTypeId.Id == assemblyDocType.Id)))
                .Returns(newDocId);

            var outputParameters = new ParameterCollection();
            PluginExecutionContextMock.Setup(pec => pec.OutputParameters).Returns(outputParameters);

            ServiceProvider = ServiceProviderMock.Object;


            //ACT

            _plugin.Execute(ServiceProvider);


            //ASSERT

            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));
            OrganizationServiceMock.Verify(os => os.Create(It.Is<Entity>(
                e => e.LogicalName == ipg_document.EntityLogicalName
                && e.ToEntity<ipg_document>().ipg_CaseId.Id == caseReference.Id)));
            //Check if document is deleted
            OrganizationServiceMock.Verify(os => os.Delete(existingPPPDoc.LogicalName,existingPPPDoc.Id));
            //Check if PPP task is completed
            OrganizationServiceMock.Verify(os => os.Execute(It.Is<SetStateRequest>(e => 
                e.EntityMoniker.Id ==  existingPPPTask.Id &&
                e.EntityMoniker.LogicalName ==  existingPPPTask.LogicalName &&
                e.State.Value == 1 &&
                e.Status.Value == 5
                )));
        }

        private ipg_document SetupDocument(string pdfFileName, Guid docId, DateTime originalDocDate)
        {
            var doc = new ipg_document().Fake().WithOriginalDocDate(originalDocDate).Generate();
            var docNote = new Annotation().Fake().WithObjectReference(doc).WithDocument(pdfFileName, Convert.ToBase64String(File.ReadAllBytes(CurrentPath + @"\" + pdfFileName))).Generate();

            OrganizationServiceMock
                .Setup(x => x.RetrieveMultiple(It.Is<QueryExpression>(qe =>
                    qe.EntityName == Annotation.EntityLogicalName
                    && qe.Criteria.Conditions.Any(c => c.AttributeName == nameof(Annotation.ObjectId).ToLower()
                        && c.Operator == ConditionOperator.Equal
                        && c.Values.Count == 1
                        && (Guid)c.Values[0] == docId
                        ))))
                .Returns(new EntityCollection(new List<Annotation> { docNote }.Cast<Entity>().ToList()));

            OrganizationServiceMock
                .Setup(x => x.Retrieve(Annotation.EntityLogicalName, docNote.Id, It.Is<ColumnSet>(cs =>
                    cs.AllColumns)))
                .Returns(docNote);

            return doc;
        }

        private string GetPdfPageText(Annotation note, int pageNumber)
        {
            byte[] bytes = Convert.FromBase64String(note.DocumentBody);
            using (var pdfMemoryStream = new MemoryStream(bytes))
            {
                using (PdfDocument pdf = PdfReader.Open(pdfMemoryStream, PdfDocumentOpenMode.Import))
                {
                    return pdf.Pages[pageNumber - 1].ExtractTextAsString();
                }
            }
        }
    }
}
