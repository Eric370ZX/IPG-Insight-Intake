using FakeXrmEasy;
using Insight.Intake;
using Insight.Intake.Plugin.Document;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Document
{
    public class RenameDocumentWhenPropertiesChangeTest : PluginTestsBase
    {
        const string dateFormat = "MMddyyy";

        [Fact]
        public void RenameDocumentWhenAttachedToCaseTest()
        {
            var fakedContext = new XrmFakedContext();

            ipg_documenttype documentType = new ipg_documenttype().Fake("DocType 1", "DT1");
            Incident caseEntity = new Incident().Fake();
            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithCaseReference(caseEntity)
                .WithRevision(3);

            document.ipg_name = "Document Test Name";

            var listForInit = new List<Entity> { documentType, document, caseEntity };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", document } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<RenameDocumentWhenPropertiesChange>(pluginContext);

            var expectedDocName = $"{caseEntity.Title}_{documentType.ipg_DocumentTypeAbbreviation}_{document.CreatedOn.Value.ToString(dateFormat)}.{document.ipg_Revision}";
            var actualDocName = ((Entity)pluginContext.InputParameters["Target"]).ToEntity<ipg_document>().ipg_name;

            Assert.Equal(expectedDocName, actualDocName);
        }

        [Fact]
        public void RenameDocumentWhenAttachedToCarrierTest()
        {
            var fakedContext = new XrmFakedContext();

            ipg_documentcategorytype documentCategory = new ipg_documentcategorytype().Fake("Carrier");
            ipg_documenttype documentType = new ipg_documenttype().Fake("DocType 1", "DT1").WithDocumentCategory(documentCategory);

            Intake.Account accountEnt = new Intake.Account().Fake();
            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithCarrierReference(accountEnt)
                .WithRevision(3);



            var listForInit = new List<Entity> { documentType, document, accountEnt, documentCategory };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", document } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<RenameDocumentWhenPropertiesChange>(pluginContext);

            var expectedDocName = $"{accountEnt.Name}_{documentType.ipg_DocumentTypeAbbreviation}_{document.CreatedOn.Value.ToString(dateFormat)}.{document.ipg_Revision}";
            var actualDocName = ((Entity)pluginContext.InputParameters["Target"]).ToEntity<ipg_document>().ipg_name;

            Assert.Equal(expectedDocName, actualDocName);
        }

        [Fact]
        public void RenameDocumentWhenAttachedToFacilityTest()
        {
            var fakedContext = new XrmFakedContext();

            ipg_documentcategorytype documentCategory = new ipg_documentcategorytype().Fake("Facility");
            ipg_documenttype documentType = new ipg_documenttype().Fake("DocType 1", "DT1").WithDocumentCategory(documentCategory);

            Intake.Account accountEnt = new Intake.Account().Fake();
            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithFacilityReference(accountEnt)
                .WithRevision(3);

            //document.ipg_name = "Document Test Name";

            var listForInit = new List<Entity> { documentType, document, accountEnt, documentCategory };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", document } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<RenameDocumentWhenPropertiesChange>(pluginContext);


            var expectedDocName = $"{accountEnt.Name}_{documentType.ipg_DocumentTypeAbbreviation}_{document.CreatedOn.Value.ToString(dateFormat)}.{document.ipg_Revision}";
            var actualDocName = ((Entity)pluginContext.InputParameters["Target"]).ToEntity<ipg_document>().ipg_name;

            Assert.Equal(expectedDocName, actualDocName);
        }

        [Fact]
        public void AddToFileNameSourceTestOnCreate()
        {
            var fakedContext = new XrmFakedContext();

            var fileName = "Test.pdf";
            var dataSource = ipg_DocumentSourceCode.Portal;
            ipg_documentcategorytype documentCategory = new ipg_documentcategorytype().Fake("Facility");
            ipg_documenttype documentType = new ipg_documenttype().Fake("DocType 1", "DT1").WithDocumentCategory(documentCategory);

            Intake.Account accountEnt = new Intake.Account().Fake();
            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithFacilityReference(accountEnt)
                .WithRevision(3)
                .WithFileName(fileName)
                .WithSource(dataSource);

            document.ipg_name = "Document Test Name";

            var listForInit = new List<Entity> { documentType, document, accountEnt, documentCategory };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", document } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<RenameDocumentWhenPropertiesChange>(pluginContext);

            var expectedDocName = $"{accountEnt.Name}_{documentType.ipg_DocumentTypeAbbreviation}_{document.CreatedOn.Value.ToString(dateFormat)}.{document.ipg_Revision}";
            var actualDocName = ((Entity)pluginContext.InputParameters["Target"]).ToEntity<ipg_document>().ipg_name;

            Assert.Equal(expectedDocName, actualDocName);
        }

        [Fact]
        public void TestThatRevisionIncreasedIfThereAlreadyDocWithSameType()
        {
            var fakedContext = new XrmFakedContext();

            var fileName = "Test.pdf";
            ipg_documentcategorytype documentCategory = new ipg_documentcategorytype().Fake("Facility");
            ipg_documenttype documentType = new ipg_documenttype().Fake("DocType 1", "DT1").WithDocumentCategory(documentCategory);

            Intake.Account accountEnt = new Intake.Account().Fake();
            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithFacilityReference(accountEnt)
                .WithRevision(3)
                .WithFileName(fileName);

            document.ipg_name = "Document Test Name";

            ipg_document nextdocument = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithFacilityReference(accountEnt)
                .WithRevision(1)
                .WithFileName("NextTestDoc.pdf");

            document.ipg_name = "Document Test Name";



            var listForInit = new List<Entity> { documentType, document, accountEnt, documentCategory };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", nextdocument } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<RenameDocumentWhenPropertiesChange>(pluginContext);

            var RevOfTheNextDoc = ((Entity)pluginContext.InputParameters["Target"]).ToEntity<ipg_document>().ipg_Revision;

            Assert.Equal(4, RevOfTheNextDoc);
        }

        [Fact]
        public void TestThatRevision1WhenDocumentCreatedWithoutNoParent()
        {
            var fakedContext = new XrmFakedContext();

            var fileName = "Test.pdf";
            ipg_documentcategorytype documentCategory = new ipg_documentcategorytype().Fake("Facility");
            ipg_documenttype documentType = new ipg_documenttype().Fake("DocType 1", "DT1").WithDocumentCategory(documentCategory);

            Intake.Account accountEnt = new Intake.Account().Fake();
            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithFacilityReference(accountEnt)
                .WithRevision(3)
                .WithFileName(fileName);

            document.ipg_name = "Document Test Name";

            ipg_document nextdocument = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithFileName("NextTestDoc.pdf");

            document.ipg_name = "Document Test Name";



            var listForInit = new List<Entity> { documentType, document, accountEnt, documentCategory };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", nextdocument } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<RenameDocumentWhenPropertiesChange>(pluginContext);

            var RevOfTheNextDoc = ((Entity)pluginContext.InputParameters["Target"]).ToEntity<ipg_document>().ipg_Revision;

            Assert.Equal(1, RevOfTheNextDoc);
        }
    }
}
