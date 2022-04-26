using FakeXrmEasy;
using Insight.Intake.Plugin.Account;
using Insight.Intake.Plugin.Document;
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

namespace Insight.Intake.UnitTests.Plugin.Account
{
    public class SetDocumentAssociationDatePlugInTest : PluginTestsBase
    {
        [Fact]
        public void SetDocumentAssociationDateTest()
        {
            var fakedContext = new XrmFakedContext();

            ipg_document document = new ipg_document().Fake().WithDateAddedToFacility(DateTime.Now.Date);
            Intake.Account facilityEntity = new Intake.Account().Fake();

            var listForInit = new List<Entity> { document, facilityEntity };

            fakedContext.Initialize(listForInit);

            EntityReferenceCollection collection = new EntityReferenceCollection();
            collection.Add(document.ToEntityReference());

            var inputParameters = new ParameterCollection { { "Target", facilityEntity }, { "Relationship", "ipg_ipg_document_account.Referenced" }, { "RelatedEntities", collection } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Associate",
                Stage = 10,
                PrimaryEntityName = ipg_document.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<SetDocumentAssociationDate>(pluginContext);
            Assert.Equal(DateTime.Now.Date, document.ipg_DateAddedToFacility);
        }
    }
}
