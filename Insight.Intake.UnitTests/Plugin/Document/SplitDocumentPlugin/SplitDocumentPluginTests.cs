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
    public class SplitDocumentPluginTests : PluginTestsBase
    {
        private static readonly string CurrentPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\Plugin\Document\SplitDocumentPlugin";
        private readonly SplitDocumentPlugin _plugin = new SplitDocumentPlugin();

        [Fact]
        public void Splits_into_2_ranges()
        {
            //ARRANGE

            Guid caseId = Guid.Parse("f4c7d6a2-90eb-467f-89f2-22c02c1f7a92");
            const string compoundPdfDocName = "doc name1";
            const string compoundPdfFileName = "compound.pdf";
            var mdtDocType = new ipg_documenttype().Fake("Generic").Generate();
            const ipg_DocumentSourceCode compoundPdfDocSource = ipg_DocumentSourceCode.User;

            var inputTarget = new EntityReference(ipg_document.EntityLogicalName, Guid.Parse("fb97b9f9-47f1-45dc-9da7-ec2173a0d193"));
            var ranges = new List<string>
            {
                "1",
                "2-3"
            };
            var fakeDocumentTypes = new List<ipg_documenttype> {
                new ipg_documenttype().Fake("a document type name 1").Generate(),
                new ipg_documenttype().Fake("a document type name 2").Generate(),
            };
            var descriptions = new List<string>
            {
                null,
                "test description 1"
            };
            var newDocIds = new List<Guid>
            {
                Guid.NewGuid(), Guid.NewGuid()
            };
            var remainingDocId = Guid.NewGuid();

            var inputParameters = new ParameterCollection()
            {
                { SplitDocumentPlugin.TargetInputParameterName, inputTarget },
                { SplitDocumentPlugin.CaseIdParameterName, new EntityReference(Incident.EntityLogicalName, caseId) }
            };
            AddRangesParameters(inputParameters, ranges, fakeDocumentTypes, descriptions);
            PluginExecutionContextMock.Setup(pec => pec.InputParameters).Returns(inputParameters);

            var fakeNotes = new EntityCollection(new List<Annotation>{ new Annotation().Fake().WithDocument(compoundPdfFileName, Convert.ToBase64String(File.ReadAllBytes(CurrentPath + @"\" + compoundPdfFileName))).Generate() }.Cast<Entity>().ToList());
            OrganizationServiceMock
                .Setup(x => x.RetrieveMultiple(It.Is<QueryExpression>(ce =>
                    ce.EntityName == Annotation.EntityLogicalName)))
                .Returns(fakeNotes);

            var fakeNote = fakeNotes.Entities.First();
            OrganizationServiceMock
                .Setup(x => x.Retrieve(Annotation.EntityLogicalName, fakeNote.Id, It.Is<ColumnSet>(cs =>
                    cs.AllColumns)))
                .Returns(fakeNote);

            var fakeDocument = new ipg_document().Fake(inputTarget.Id)
                .WithName(compoundPdfDocName)
                .WithSource(compoundPdfDocSource)
                .WithDocumentTypeReference(mdtDocType)
                .Generate();
            OrganizationServiceMock
                .Setup(x => x.Retrieve(ipg_document.EntityLogicalName, inputTarget.Id, It.IsAny<ColumnSet>()))
                .Returns(fakeDocument);

            var fakeCase = new Incident().Fake().Generate();
            OrganizationServiceMock
                .Setup(x => x.Retrieve(Incident.EntityLogicalName, caseId, It.Is<ColumnSet>(cs =>
                    ContainsColumns(cs, nameof(Incident.Title).ToLower()))))
                .Returns(fakeCase);

            
            OrganizationServiceMock
                .Setup(x => x.Retrieve(ipg_documenttype.EntityLogicalName, fakeDocumentTypes[0].Id, It.Is<ColumnSet>(cs =>
                    ContainsColumns(cs, nameof(ipg_documenttype.ipg_name).ToLower()))))
                .Returns(fakeDocumentTypes[0]);
            OrganizationServiceMock
                .Setup(x => x.Retrieve(ipg_documenttype.EntityLogicalName, fakeDocumentTypes[1].Id, It.Is<ColumnSet>(cs =>
                    ContainsColumns(cs, nameof(ipg_documenttype.ipg_name).ToLower()))))
                .Returns(fakeDocumentTypes[1]);
            OrganizationServiceMock
                .Setup(x => x.Retrieve(ipg_documenttype.EntityLogicalName, mdtDocType.Id, It.Is<ColumnSet>(cs =>
                    ContainsColumns(cs, nameof(ipg_documenttype.ipg_name).ToLower()))))
                .Returns(mdtDocType);

            OrganizationServiceMock
                .Setup(x => x.Create(It.Is<ipg_document>(d => d.ipg_DocumentTypeId.Id == fakeDocumentTypes[0].Id)))
                .Returns(newDocIds[0]);
            OrganizationServiceMock
                .Setup(x => x.Create(It.Is<ipg_document>(d => d.ipg_DocumentTypeId.Id == fakeDocumentTypes[1].Id)))
                .Returns(newDocIds[1]);
            OrganizationServiceMock
                .Setup(x => x.Create(It.Is<ipg_document>(d => d.ipg_DocumentTypeId.Id == mdtDocType.Id)))
                .Returns(remainingDocId);

            var outputParameters = new ParameterCollection();
            PluginExecutionContextMock.Setup(pec => pec.OutputParameters).Returns(outputParameters);

            ServiceProvider = ServiceProviderMock.Object;


            //ACT

            _plugin.Execute(ServiceProvider);


            //ASSERT


            var pluginExecutionContext = (IPluginExecutionContext)ServiceProvider.GetService(typeof(IPluginExecutionContext));
            OrganizationServiceMock.Verify(os => os.Create(It.Is<Entity>(e => e.LogicalName == ipg_document.EntityLogicalName
                && e.ToEntity<ipg_document>().ipg_CaseId != null && e.ToEntity<ipg_document>().ipg_CaseId.Id == caseId
                && e.ToEntity<ipg_document>().ipg_Source.Value == (int)compoundPdfDocSource
                && e.ToEntity<ipg_document>().ipg_DocumentTypeId.Id == fakeDocumentTypes[0].Id
                && e.ToEntity<ipg_document>().ipg_Description == descriptions[0]
                && e.ToEntity<ipg_document>().ipg_OriginalDocumentId.Id == inputTarget.Id)));
            OrganizationServiceMock.Verify(os => os.Create(It.Is<Entity>(e => e.LogicalName == Annotation.EntityLogicalName
                && e.ToEntity<Annotation>().ObjectId.Id == newDocIds[0]
                && e.ToEntity<Annotation>().FileName == compoundPdfFileName
                && GetPdfPageText(e.ToEntity<Annotation>(), 1).Trim() == "Page number 1")));

            OrganizationServiceMock.Verify(os => os.Create(It.Is<Entity>(e => e.LogicalName == ipg_document.EntityLogicalName
                && e.ToEntity<ipg_document>().ipg_CaseId != null && e.ToEntity<ipg_document>().ipg_CaseId.Id == caseId
                && e.ToEntity<ipg_document>().ipg_Source.Value == (int)compoundPdfDocSource
                && e.ToEntity<ipg_document>().ipg_DocumentTypeId.Id == fakeDocumentTypes[1].Id
                && e.ToEntity<ipg_document>().ipg_Description == descriptions[1]
                && e.ToEntity<ipg_document>().ipg_OriginalDocumentId.Id == inputTarget.Id)));
            OrganizationServiceMock.Verify(os => os.Create(It.Is<Entity>(e => e.LogicalName == Annotation.EntityLogicalName
                && e.ToEntity<Annotation>().ObjectId.Id == newDocIds[1]
                && e.ToEntity<Annotation>().FileName == compoundPdfFileName
                && GetPdfPageText(e.ToEntity<Annotation>(), 1).Trim() == "Page number 2"
                && GetPdfPageText(e.ToEntity<Annotation>(), 2).Trim() == "Page number 3")));

            OrganizationServiceMock.Verify(os => os.Create(It.Is<Entity>(e => e.LogicalName == ipg_document.EntityLogicalName
                && e.ToEntity<ipg_document>().ipg_CaseId == null
                && e.ToEntity<ipg_document>().ipg_Source.Value == fakeDocument.ipg_Source.Value
                && e.ToEntity<ipg_document>().ipg_DocumentTypeId.Id == mdtDocType.Id
                && e.ToEntity<ipg_document>().ipg_Description == fakeDocument.ipg_Description
                && e.ToEntity<ipg_document>().ipg_OriginalDocumentId.Id == inputTarget.Id)));
            OrganizationServiceMock.Verify(os => os.Create(It.Is<Entity>(e => e.LogicalName == Annotation.EntityLogicalName
                && e.ToEntity<Annotation>().ObjectId.Id == remainingDocId
                && e.ToEntity<Annotation>().FileName == compoundPdfFileName
                && GetPdfPageText(e.ToEntity<Annotation>(), 1).Trim() == "Page number 4")));

            OrganizationServiceMock.Verify();

            Assert.Equal(pluginExecutionContext.OutputParameters[SplitDocumentPlugin.NewDocumentIdOutputParameterName], remainingDocId);
        }

        private ParameterCollection AddRangesParameters(ParameterCollection parameters, IList<string> ranges, IList<ipg_documenttype> docTypes, IList<string> descriptions)
        {
            for (int i = 0; i < ranges.Count; i++)
            {
                parameters.Add(SplitDocumentPlugin.RangeInputParameterNamePrefix + i, ranges[i]);
                parameters.Add(SplitDocumentPlugin.DocTypeInputParameterNamePrefix + i, new EntityReference(ipg_documenttype.EntityLogicalName, docTypes[i].Id));
                parameters.Add(SplitDocumentPlugin.DescriptionInputParameterNamePrefix + i, descriptions[i]);
            }

            return parameters;
        }

        private bool ContainsColumns(ColumnSet columnSet, params string[] columns)
        {
            return Enumerable.Any(columns, c => columnSet.Columns.Contains(c));
        }

        private string GetPdfPageText(Annotation note, int pageNumber)
        {
            byte[] bytes = Convert.FromBase64String(note.DocumentBody);
            using (var pdfMemoryStream = new MemoryStream(bytes))
            {
                using (PdfDocument pdf = PdfReader.Open(pdfMemoryStream, PdfDocumentOpenMode.Import))
                {
                    return pdf.Pages[pageNumber-1].ExtractTextAsString();
                }
            }
        }
    }
}
