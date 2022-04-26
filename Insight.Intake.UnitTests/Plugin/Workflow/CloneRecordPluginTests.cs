using System;
using System.Collections.Generic;
using System.Linq;
using FakeXrmEasy;
using Insight.Intake.Plugin.Workflow;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Workflow
{
    public class CloneRecordPluginTests : PluginTestsBase
    {
        [Fact]
        public void CheckThatDocumentClonedCorrectly()
        {
            var fakedContext = new XrmFakedContext();

            ipg_referral referal = new ipg_referral().Fake();
            ipg_document document = new ipg_document().Fake()
                .WithReferralReference(referal);
            var numberOfAttachments = new Random().Next(1, 5);

            IList<Annotation> attachments = new Annotation().Fake()
                .WithDocument()
                .WithObjectReference(document)
                .Generate(numberOfAttachments);

            var listForInit = new List<Entity>();
            listForInit.Add(document);
            listForInit.AddRange(attachments);

            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Record", document.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "ClonedRecord", null } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGIntakeActionsCloneRecord",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CloneRecordPlugin>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            //checking that output parameter is returned
            Assert.True(pluginContext.OutputParameters.Contains("ClonedRecord")
                        && pluginContext.OutputParameters["ClonedRecord"] is EntityReference);

            //checking that dicument's field is cloned correctly
            var clonedRecordRef = (EntityReference)pluginContext.OutputParameters["ClonedRecord"];
            var clonedRecord = fakedService.Retrieve(clonedRecordRef.LogicalName, clonedRecordRef.Id, new ColumnSet(true)).ToEntity<ipg_document>();
           
            Assert.True(document.ipg_ReferralId.Id == clonedRecord.ipg_ReferralId.Id);

            //checking that dicument's attachments are cloned correctly
            var clonedAttachments = (from note in crmContext.CreateQuery<Annotation>()
                               where note.ObjectId.Id == clonedRecord.Id
                               select note).ToList();

            Assert.True(attachments.Count == clonedAttachments.Count);
        }
    }
}