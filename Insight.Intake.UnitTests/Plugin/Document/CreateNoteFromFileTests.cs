using FakeXrmEasy;
using Insight.Intake.Plugin.Document;
using Insight.Intake.Plugin.Workflow;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Document
{
    public class CreateNoteFromFileTests : PluginTestsBase
    {
        [Fact]
        public void CheckCreateNoteFromFile_DocumentBodyIsFilledIn_returnCreatedNotesAndFildsAreEmpty()
        {
            var fakedContext = new XrmFakedContext();

            ipg_document document = new ipg_document().Fake();
            document.ipg_documentbody = "testbody";
            document.ipg_mimetype = "testmimetype";

            fakedContext.Initialize(document);

            var inputParameters = new ParameterCollection { { "Target", document } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CreateNoteFromFile>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var attachments = (from note in crmContext.CreateQuery<Annotation>()
                                     where note.ObjectId.Id == document.Id
                                     select note).ToList();

            Assert.True(attachments.Count == 1);

            var updDocument = fakedService.Retrieve(document.LogicalName, document.Id, new ColumnSet(true)).ToEntity<ipg_document>();

            Assert.True(updDocument.ipg_mimetype == string.Empty);
            Assert.True(updDocument.ipg_documentbody == string.Empty);
        }

        [Fact]
        public void CheckCreateNoteFromFile_DocumentBodyIsNotFilledIn_returnNoCreatedNotes()
        {
            var fakedContext = new XrmFakedContext();

            ipg_document document = new ipg_document().Fake();
            document.ipg_documentbody = "";

            fakedContext.Initialize(document);

            var inputParameters = new ParameterCollection { { "Target", document } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CreateNoteFromFile>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var attachments = (from note in crmContext.CreateQuery<Annotation>()
                               where note.ObjectId.Id == document.Id
                               select note).ToList();

            Assert.True(attachments.Count == 0);
        }
    }
}
