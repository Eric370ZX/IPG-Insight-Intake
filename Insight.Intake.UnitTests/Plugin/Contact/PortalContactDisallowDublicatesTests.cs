using FakeXrmEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Insight.Intake.Plugin.Contact;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;

namespace Insight.Intake.UnitTests.Plugin.ContactEntity
{
    public class PortalContactDisallowDublicatesTests : PluginTestsBase
    {
        [Fact]
        public void PortalContactHasDublicates_returnException()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Intake.Contact contactEntity = new Intake.Contact().Fake();
            contactEntity.adx_identity_username = "uniqueusername@email.com";

            Intake.Contact existingEntity = new Intake.Contact().Fake();
            existingEntity.adx_identity_username = "uniqueusername@email.com";

            var listForInit = new List<Entity>() { contactEntity, existingEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", contactEntity },
            };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 20,
                PrimaryEntityName = Intake.Contact.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            //Act & Assert
            Exception ex = Assert.Throws<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<PortalContactDisallowDuplicates>(pluginContext));
            Assert.Equal("Error: Contact with such 'Username' already exists", ex.Message);
        }
    }
}
