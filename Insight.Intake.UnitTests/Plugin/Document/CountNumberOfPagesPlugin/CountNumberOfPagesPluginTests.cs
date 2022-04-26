using FakeXrmEasy;
using Insight.Intake.Plugin.Document;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Document.CountNumberOfPagesPlugin
{
    public class CountNumberOfPagesPluginTests : PluginTestsBase
    {
        private static readonly string CurrentPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\Plugin\Document\CountNumberOfPagesPlugin";

        [Fact]
        public void Returns_true_if_number_of_pages_set_to_3()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();
            var fileName = "MultiDoc.pdf";

            //Create a fake PIF Document and referral. Bind the referral with the document.
            ipg_document document = new ipg_document().Fake()
                .WithFileName("multiDoc.pdf")
                .WithDocumentBody(Convert.ToBase64String(File.ReadAllBytes(CurrentPath + @"\" + fileName)));

            var listForInit = new List<Entity> { document };

            fakedContext.Initialize(listForInit);

            #endregion

            #region Setup execution context

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", document } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin
            
            fakedContext.ExecutePluginWith<CountNumberOfPages>(pluginContext);

            #endregion

            #region Asserts

            Assert.True(document.ipg_numberofpages == 3);

            #endregion
        }

        [Fact]
        public void Returns_true_if_number_of_pages_not_set()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();
            var fileName = "ErrorDetails.txt";

            //Create a fake PIF Document and referral. Bind the referral with the document.
            ipg_document document = new ipg_document().Fake()
                .WithFileName("ErrorDetails.txt")
                .WithDocumentBody(Convert.ToBase64String(File.ReadAllBytes(CurrentPath + @"\" + fileName)));

            var listForInit = new List<Entity> { document };

            fakedContext.Initialize(listForInit);

            #endregion

            #region Setup execution context

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", document } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin

            fakedContext.ExecutePluginWith<CountNumberOfPages>(pluginContext);

            #endregion

            #region Asserts

            Assert.True(document.ipg_numberofpages == null);

            #endregion
        }
    }
}
