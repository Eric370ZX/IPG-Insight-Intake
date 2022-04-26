using FakeXrmEasy;
using Insight.Intake.Plugin.Document;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Document
{
    public class ValidateFileExtensionTest : PluginTestsBase
    {
        [Fact]
        public void Fails_when_attached_file_has_not_pdf_extension_with_no_related_case()
        {
            var fakedContext = new XrmFakedContext();

            ipg_documenttype documentType = new ipg_documenttype().Fake("IPG Invoice");
            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithFileName("test.test");


            var listForInit = new List<Entity> { documentType, document, documentType };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", document } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = PipelineStages.PreValidation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            Exception ex = Assert.Throws<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<ValidateFileExtension>(pluginContext));
            Assert.Equal("Error: Only pdf documents can be attached to the document.", ex.Message);
        }

        [Fact]
        public void CheckValidationDoesNotTriggerForPatientStatement()
        {
            var fakedContext = new XrmFakedContext();

            ipg_documenttype documentType = new ipg_documenttype().Fake("Patient Statement", "PST");
            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithFileName("test.test");


            var listForInit = new List<Entity> { documentType, document, documentType };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", document } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = PipelineStages.PreValidation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<ValidateFileExtension>(pluginContext);
        }

        [Fact]
        public void Fails_when_attached_file_has_not_pdf_extension_with_related_open_case()
        {
            var fakedContext = new XrmFakedContext();

            ipg_documenttype documentType = new ipg_documenttype().Fake("IPG Invoice");
            Incident fakeCase = new Incident().Fake().WithCaseStatus((int)ipg_CaseStatus.Open);
            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithFileName("test.test")
                .WithCaseReference(fakeCase);


            var listForInit = new List<Entity> { documentType, document, fakeCase };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", document } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = PipelineStages.PreValidation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            Exception ex = Assert.Throws<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<ValidateFileExtension>(pluginContext));
            Assert.Equal("Error: Only pdf documents can be attached to the Open Case.", ex.Message);
        }

        [Fact]
        public void Not_fails_when_attached_file_has_not_pdf_extension_with_related_closed_case()
        {
            var fakedContext = new XrmFakedContext();

            ipg_documenttype documentType = new ipg_documenttype().Fake("IPG Invoice");
            Incident fakeCase = new Incident().Fake().WithCaseStatus((int)ipg_CaseStatus.Closed);
            ipg_document document = new ipg_document().Fake()
                .WithDocumentTypeReference(documentType)
                .WithFileName("test.test")
                .WithCaseReference(fakeCase);


            var listForInit = new List<Entity> { documentType, document, fakeCase };

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", document } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = PipelineStages.PreValidation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<ValidateFileExtension>(pluginContext);
        }

    }
}
