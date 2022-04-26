using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Insight.Intake.Plugin.Contact;
using Insight.Intake.UnitTests.Fakes;

namespace Insight.Intake.UnitTests.Plugin.ContactEntity
{
    public class NewPhysicianRequestPluginTests : PluginTestsBase
    {
        [Fact]
        public void NewPhysicianRequestPluginTests_ContactTypeIsPhysician_SetParentCustomerId()
        {
            var fakedContext = new XrmFakedContext();

            ipg_contacttype physicianContactType = new ipg_contacttype().Fake("Physician");
            Intake.Account facilityEntity = new Intake.Account().Fake();
            facilityEntity.CustomerTypeCodeEnum = Account_CustomerTypeCode.Facility;
            Contact contactEntity = new Contact().Fake();
            contactEntity.ipg_ContactTypeId = physicianContactType.ToEntityReference();
            contactEntity.ipg_currentfacilityid = facilityEntity.ToEntityReference();

            var listForInit = new List<Entity>() { physicianContactType, facilityEntity, contactEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", contactEntity } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 40,
                PrimaryEntityName = Contact.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };
            fakedContext.ExecutePluginWith<NewPhysicianRequestPlugin>(pluginContext);

            var resultContact = contactEntity;
            //Assert
            Assert.Equal(facilityEntity.Id, resultContact.ParentCustomerId.Id);
        }

        [Fact]
        public void NewPhysicianRequestPluginTests_ContactTypeIsPatient_DontSetParentCustomerId()
        {
            var fakedContext = new XrmFakedContext();

            ipg_contacttype physicianContactType = new ipg_contacttype().Fake("Patient");
            Intake.Account facilityEntity = new Intake.Account().Fake();
            facilityEntity.CustomerTypeCodeEnum = Account_CustomerTypeCode.Facility;
            Contact contactEntity = new Contact().Fake();
            contactEntity.ipg_ContactTypeId = physicianContactType.ToEntityReference();
            contactEntity.ipg_currentfacilityid = facilityEntity.ToEntityReference();

            var listForInit = new List<Entity>() { physicianContactType, facilityEntity, contactEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", contactEntity } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 40,
                PrimaryEntityName = Contact.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };
            fakedContext.ExecutePluginWith<NewPhysicianRequestPlugin>(pluginContext);

            var resultContact = contactEntity;
            //Assert
            Assert.Null(resultContact.ParentCustomerId);
        }
    }
}
