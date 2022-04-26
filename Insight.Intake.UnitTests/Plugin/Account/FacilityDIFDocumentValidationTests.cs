using System;
using System.Collections.Generic;
using System.Linq;
using FakeXrmEasy;
using Insight.Intake.Data;
using Insight.Intake.Plugin.Account;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Account
{
    public class FacilityDIFDocumentValidationTests : PluginTestsBase
    {
        [Fact]
        public void CheckDIFDocumentsTests_hasDocument_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Intake.Account facilityEntity = new Intake.Account().Fake();
            facilityEntity.CustomerTypeCodeEnum = Account_CustomerTypeCode.Facility;
            facilityEntity.ipg_EHREffectiveDate = DateTime.Now;

            Intake.ipg_documenttype DIFDocType = new Intake.ipg_documenttype().Fake();
            DIFDocType.ipg_DocumentTypeAbbreviation = "DIF";

            Intake.ipg_document DIFDoc = new Intake.ipg_document().Fake()
                .WithDocumentTypeReference(DIFDocType)
                .WithFacilityReference(facilityEntity);


            var listForInit = new List<Entity>() { facilityEntity, DIFDocType, DIFDoc };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", facilityEntity } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var postImage = new EntityImageCollection();
            postImage.Add("PostImage", facilityEntity);
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = postImage,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<FacilityDIFDocumentValidation>(pluginContext);

            //Assert no exception is thrown
            Assert.True(true);
        }

        [Fact]
        public void CheckDIFDocumentsTests_NoDocumentsCreatesTask_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Intake.Account facilityEntity = new Intake.Account().Fake();
            facilityEntity.CustomerTypeCodeEnum = Account_CustomerTypeCode.Facility;
            facilityEntity.ipg_EHREffectiveDate = DateTime.Now;

            Intake.ipg_documenttype DIFDocType = new Intake.ipg_documenttype().Fake();
            DIFDocType.ipg_DocumentTypeAbbreviation = "FAA";

            Intake.ipg_document DIFDoc = new Intake.ipg_document().Fake()
                .WithDocumentTypeReference(DIFDocType)
                .WithFacilityReference(facilityEntity);


            var listForInit = new List<Entity>() { facilityEntity, DIFDocType, DIFDoc };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", facilityEntity } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var postImage = new EntityImageCollection();
            postImage.Add("PostImage", facilityEntity);
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = postImage,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<FacilityDIFDocumentValidation>(pluginContext);
            var tasks = fakedContext.CreateQuery("task");
            //Assert no exception is thrown
            Assert.True(tasks.Any(), "User task should be created");
        }
    }
}
