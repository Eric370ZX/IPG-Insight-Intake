
using FakeXrmEasy;
using Insight.Intake.Plugin.Document;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Document
{

    public class AssociateDocumentTest : PluginTestsBase
    {
        [Fact]
        public void AssosiatedWithCaseReferenceTest_shouldReturnTrue()
        {
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            ipg_document document = new ipg_document().Fake()
                .WithCaseReference(caseEntity);


            var listForInit = new List<Entity> { document, caseEntity };

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

            fakedContext.ExecutePluginWith<AssociateDocument>(pluginContext);

            Assert.True(document.ipg_ReviewStatus.Value == ((int)ipg_document_ipg_ReviewStatus.Approved));
        }
        [Fact]
        public void AssosiatedWithReferralReferenceTest_shouldReturnTrue()
        {
            var fakedContext = new XrmFakedContext();

            ipg_referral referralEntity = new ipg_referral().Fake();
            ipg_document document = new ipg_document().Fake()
                .WithReferralReference(referralEntity);


            var listForInit = new List<Entity> { document, referralEntity };

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

            fakedContext.ExecutePluginWith<AssociateDocument>(pluginContext);

            Assert.True(document.ipg_ReviewStatus.Value == ((int)ipg_document_ipg_ReviewStatus.Approved));
        }
        [Fact]
        public void NotAssosiatedDocumentlTest_shouldReturnTrue()
        {
            var fakedContext = new XrmFakedContext();

            ipg_document document = new ipg_document().Fake();

            fakedContext.Initialize(document);

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

            fakedContext.ExecutePluginWith<AssociateDocument>(pluginContext);

            Assert.True(document?.ipg_ReviewStatus == null);
        }
    }
}
