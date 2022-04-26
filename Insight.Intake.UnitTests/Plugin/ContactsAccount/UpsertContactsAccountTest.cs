using FakeXrmEasy;
using Insight.Intake.Plugin.ContactAccount;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.ContactsAccount
{
    public class UpsertContactsAccountTest: PluginTestsBase
    {
        [Fact]
        public void IncertContactTest()
        {

            var fakedContext = new XrmFakedContext();
            Intake.Account account = new Intake.Account().Fake();

            ipg_contactsaccounts contAcc = new ipg_contactsaccounts().Fake(account: account);
            contAcc.ipg_mainphone = "11111";
            contAcc.ipg_otherphone = "111123";
            contAcc.ipg_email = "asd@ll.ww";
            contAcc.ipg_fax = "123321";
            contAcc.ipg_contactname = "blabla Ohoh";

            var listForInit = new List<Entity> { account, contAcc };
            fakedContext.Initialize(listForInit);
            var inputParameters = new ParameterCollection { { "Target", contAcc } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 40,
                PrimaryEntityName = ipg_contactsaccounts.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<UpsertContactAccount>(pluginContext);
            var updatedContactAcc = new OrganizationServiceContext(fakedContext.GetOrganizationService()).CreateQuery<Contact>().FirstOrDefault();
            Assert.NotNull(updatedContactAcc);
        }
        [Fact]
        public void UpdateContactTest()
        {

            var fakedContext = new XrmFakedContext();
            Intake.Account account = new Intake.Account().Fake();
            Contact contact = new Contact().Fake();
            ipg_contactsaccounts contAcc = new ipg_contactsaccounts().Fake(account: account, contact: contact);

            contAcc.ipg_mainphone = "11111";
            contAcc.ipg_otherphone = "111123";
            contAcc.ipg_email = "asd@ll.ww";
            contAcc.ipg_fax = "123321";
            contAcc.ipg_contactname = "blabla Ohoh";

            var listForInit = new List<Entity> { account, contact, contAcc };
            fakedContext.Initialize(listForInit);
            var inputParameters = new ParameterCollection { { "Target", contAcc } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 40,
                PrimaryEntityName = ipg_contactsaccounts.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<UpsertContactAccount>(pluginContext);

            var updatedContactAcc = new OrganizationServiceContext(fakedContext.GetOrganizationService()).CreateQuery<Contact>().FirstOrDefault();
            Assert.NotNull(updatedContactAcc.Telephone1);
            Assert.NotNull(updatedContactAcc.FirstName);
            Assert.NotNull(updatedContactAcc.LastName);
        }
    }
}
