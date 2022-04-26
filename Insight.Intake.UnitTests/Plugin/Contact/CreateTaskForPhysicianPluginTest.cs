using FakeXrmEasy;
using Insight.Intake.Plugin.Contact;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.ContactEntity
{
    public class CreateTaskForPhysicianPluginTest : PluginTestsBase
    {
        [Fact]
        public void CreateContactExtensionTask()
        {
            var fakedContext = new XrmFakedContext();
            Contact contactEntity = new Contact().Fake();
            contactEntity.ipg_ContactTypeId = new EntityReference(ipg_contacttype.EntityLogicalName, new Guid());
            contactEntity.ipg_physicianrequestedbyid = new EntityReference(Contact.EntityLogicalName, new Guid());
            contactEntity.ipg_approved = true;

            var listForInit = new List<Entity>() { contactEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", contactEntity } };

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
            fakedContext.ExecutePluginWith<CreateTaskForPhysicianPlugin>(pluginContext);

            var service = fakedContext.GetOrganizationService();
            var query = new QueryByAttribute {
                EntityName = Task.EntityLogicalName,
                ColumnSet = new ColumnSet(true)
            };

            var resultTasksCollection = service.RetrieveMultiple(query);
            //Assert
            Assert.False(resultTasksCollection.Entities.Any());
        }
    }
}
