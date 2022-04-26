using FakeXrmEasy;
using Insight.Intake.Plugin.Contact;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.ContactEntity
{
    public class ImpersonationTests : PluginTestsBase
    {
        [Fact]
        public void CheckImpersonation()
        {
            var fakedContext = new XrmFakedContext();
            SystemUser sysUser = new SystemUser().Fake().WithWmail("test@ipg.com");
            Contact contact = new Contact().FakePortalContact().WithEmail(sysUser.InternalEMailAddress);
            Contact fromContact = new Contact().FakePortalContact().WithEmail("from@ipg.com");
            Role role = new Role().Fake();
            Adx_webrole oldWebRole = new Adx_webrole().Fake("Dev Admin");
            Adx_webrole newWebRole = new Adx_webrole().Fake("Manager");
            Intake.Account oldfacility = new Intake.Account().Fake(Account_CustomerTypeCode.Facility);
            Intake.Account newfacility = new Intake.Account().Fake(Account_CustomerTypeCode.Facility);

            ipg_contactsaccounts contactAccount = new ipg_contactsaccounts().Fake(fromContact, newfacility);
            ipg_contactsaccounts contactAccount2 = new ipg_contactsaccounts().Fake(contact, oldfacility);

            fakedContext.AddRelationship("systemuserroles_association", new XrmFakedRelationship
            {
                IntersectEntity = SystemUserRoles.EntityLogicalName,
                Entity1LogicalName = SystemUser.EntityLogicalName,
                Entity1Attribute = SystemUser.PrimaryIdAttribute,
                Entity2LogicalName = Role.EntityLogicalName,
                Entity2Attribute = Role.PrimaryIdAttribute
            });

            fakedContext.AddRelationship(adx_webrole_contact.EntityLogicalName, new XrmFakedRelationship
            {
                IntersectEntity = adx_webrole_contact.EntityLogicalName,
                Entity1LogicalName = Adx_webrole.EntityLogicalName,
                Entity1Attribute = Adx_webrole.PrimaryIdAttribute,
                Entity2LogicalName = Contact.EntityLogicalName,
                Entity2Attribute = Contact.PrimaryIdAttribute
            });

        

            fakedContext.Initialize(new List<Entity>() { oldWebRole, newWebRole, sysUser, contact, fromContact, role, oldfacility, newfacility, contactAccount, contactAccount2 });
            var crmService = fakedContext.GetOrganizationService();

            var request = new AssociateRequest()
            {
                Target = sysUser.ToEntityReference(),
                RelatedEntities = new EntityReferenceCollection()
                    {
                        role.ToEntityReference()
                    },
                Relationship = new Relationship("systemuserroles_association")
            };

            crmService.Execute(request);


            request = new AssociateRequest()
            {
                Target = contact.ToEntityReference(),
                RelatedEntities = new EntityReferenceCollection()
                    {
                        oldWebRole.ToEntityReference()
                    },
                Relationship = new Relationship(adx_webrole_contact.EntityLogicalName)
            };

            crmService.Execute(request);


            request = new AssociateRequest()
            {
                Target = fromContact.ToEntityReference(),
                RelatedEntities = new EntityReferenceCollection()
                    {
                        newWebRole.ToEntityReference()
                    },
                Relationship = new Relationship(adx_webrole_contact.EntityLogicalName)
            };

            crmService.Execute(request);

            var ImpersonateRequest = new ipg_IPGIntakeContactActionsImpersonateRequest() { Target = fromContact.ToEntityReference() };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                InitiatingUserId = sysUser.Id,
                MessageName = ImpersonateRequest.RequestName,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = fromContact.LogicalName,
                InputParameters = ImpersonateRequest.Parameters,
            };

            fakedContext.ExecutePluginWith<Impersonation>(pluginContext);


            var serviceContext = new CrmServiceContext(fakedContext.GetOrganizationService());
            var updatedContact = serviceContext.ContactSet.Where(x => x.ContactId == contact.ContactId).FirstOrDefault();
            var contactAccounts = serviceContext.ipg_contactsaccountsSet.Where(x => x.ipg_contactid.Id == contact.ContactId).Select(x => x.ipg_accountid);
            var webRoles = serviceContext.adx_webrole_contactSet.Where(x => x.contactid == contact.ContactId).Select(x => new EntityReference(Adx_webrole.EntityLogicalName, x.adx_webroleid.Value));

            Assert.Contains(newfacility.ToEntityReference(), contactAccounts);
            Assert.DoesNotContain(oldfacility.ToEntityReference(), contactAccounts);

            Assert.Contains(newWebRole.ToEntityReference(), webRoles);
            Assert.DoesNotContain(oldWebRole.ToEntityReference(), webRoles);
            Assert.NotNull(updatedContact?.ipg_backupjson);
        }
        [Fact]
        public void CheckImpersonationErrorIfNoContactForSysAdmin()
        {
            var fakedContext = new XrmFakedContext();
            SystemUser sysUser = new SystemUser().Fake().WithWmail("test@ipg.com");
            Contact contact = new Contact().FakePortalContact().WithEmail(sysUser.InternalEMailAddress);
            Contact fromContact = new Contact().FakePortalContact().WithEmail("from@ipg.com");
            Role role = new Role().Fake();
            Intake.Account oldfacility = new Intake.Account().Fake(Account_CustomerTypeCode.Facility);
            Intake.Account newfacility = new Intake.Account().Fake(Account_CustomerTypeCode.Facility);

            ipg_contactsaccounts contactAccount = new ipg_contactsaccounts().Fake(fromContact, newfacility);
            ipg_contactsaccounts contactAccount2 = new ipg_contactsaccounts().Fake(contact, oldfacility);

            fakedContext.Initialize(new List<Entity>() { sysUser, contact, fromContact, role, oldfacility, newfacility, contactAccount, contactAccount2 });

            var request = new ipg_IPGIntakeContactActionsImpersonateRequest() { Target = fromContact.ToEntityReference() };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                InitiatingUserId = sysUser.Id,
                MessageName = request.RequestName,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = fromContact.LogicalName,
                InputParameters = request.Parameters,
            };

            Exception ex = Assert.Throws<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<Impersonation>(pluginContext));
            Assert.Equal("Error: You don't have Portal Contact with the same email as your User!", ex.Message);
        }

        [Fact]
        public void RevertBackTest()
        {
            var fakedContext = new XrmFakedContext();
            SystemUser sysUser = new SystemUser().Fake().WithWmail("test@ipg.com");
            Contact contact = new Contact().FakePortalContact().WithEmail(sysUser.InternalEMailAddress);
            Contact fromContact = new Contact().FakePortalContact().WithEmail("from@ipg.com");
            Role role = new Role().Fake();
            Adx_webrole oldWebRole = new Adx_webrole().Fake("Dev Admin");
            Adx_webrole newWebRole = new Adx_webrole().Fake("Manager");
            Intake.Account oldfacility = new Intake.Account().Fake(Account_CustomerTypeCode.Facility);
            Intake.Account newfacility = new Intake.Account().Fake(Account_CustomerTypeCode.Facility);

            ipg_contactsaccounts contactAccount = new ipg_contactsaccounts().Fake(fromContact, newfacility);
            ipg_contactsaccounts contactAccount2 = new ipg_contactsaccounts().Fake(contact, oldfacility);

            fakedContext.AddRelationship("systemuserroles_association", new XrmFakedRelationship
            {
                IntersectEntity = SystemUserRoles.EntityLogicalName,
                Entity1LogicalName = SystemUser.EntityLogicalName,
                Entity1Attribute = SystemUser.PrimaryIdAttribute,
                Entity2LogicalName = Role.EntityLogicalName,
                Entity2Attribute = Role.PrimaryIdAttribute
            });

            fakedContext.AddRelationship(adx_webrole_contact.EntityLogicalName, new XrmFakedRelationship
            {
                IntersectEntity = adx_webrole_contact.EntityLogicalName,
                Entity1LogicalName = Adx_webrole.EntityLogicalName,
                Entity1Attribute = Adx_webrole.PrimaryIdAttribute,
                Entity2LogicalName = Contact.EntityLogicalName,
                Entity2Attribute = Contact.PrimaryIdAttribute
            });



            fakedContext.Initialize(new List<Entity>() { oldWebRole, newWebRole, sysUser, contact, fromContact, role, oldfacility, newfacility, contactAccount, contactAccount2 });
            var crmService = fakedContext.GetOrganizationService();

            var request = new AssociateRequest()
            {
                Target = sysUser.ToEntityReference(),
                RelatedEntities = new EntityReferenceCollection()
                    {
                        role.ToEntityReference()
                    },
                Relationship = new Relationship("systemuserroles_association")
            };

            crmService.Execute(request);


            request = new AssociateRequest()
            {
                Target = contact.ToEntityReference(),
                RelatedEntities = new EntityReferenceCollection()
                    {
                        oldWebRole.ToEntityReference()
                    },
                Relationship = new Relationship(adx_webrole_contact.EntityLogicalName)
            };

            crmService.Execute(request);


            request = new AssociateRequest()
            {
                Target = fromContact.ToEntityReference(),
                RelatedEntities = new EntityReferenceCollection()
                    {
                        newWebRole.ToEntityReference()
                    },
                Relationship = new Relationship(adx_webrole_contact.EntityLogicalName)
            };

            crmService.Execute(request);

            var ImpersonateRequest = new ipg_IPGIntakeContactActionsImpersonateRequest() { Target = fromContact.ToEntityReference() };
            var RevertBackRequest = new ipg_IPGIntakeContactActionsRevertBackImpersonationRequest() { Target = contact.ToEntityReference() };

            var impersonatePluginContext = new XrmFakedPluginExecutionContext()
            {
                InitiatingUserId = sysUser.Id,
                MessageName = ImpersonateRequest.RequestName,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = fromContact.LogicalName,
                InputParameters = ImpersonateRequest.Parameters,
            };

            var revertBackContext = new XrmFakedPluginExecutionContext()
            {
                InitiatingUserId = sysUser.Id,
                MessageName = RevertBackRequest.RequestName,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = contact.LogicalName,
                InputParameters = RevertBackRequest.Parameters,
            };

            fakedContext.ExecutePluginWith<Impersonation>(impersonatePluginContext);
            fakedContext.ExecutePluginWith<Impersonation>(revertBackContext);


            var serviceContext = new CrmServiceContext(fakedContext.GetOrganizationService());
            var updatedContact = serviceContext.ContactSet.Where(x => x.ContactId == contact.ContactId).FirstOrDefault();
            var contactAccounts = serviceContext.ipg_contactsaccountsSet.Where(x => x.ipg_contactid.Id == contact.ContactId).Select(x => x.ipg_accountid);
            var webRoles = serviceContext.adx_webrole_contactSet.Where(x => x.contactid == contact.ContactId).Select(x => new EntityReference(Adx_webrole.EntityLogicalName, x.adx_webroleid.Value));

            Assert.DoesNotContain(newfacility.ToEntityReference(), contactAccounts);
            Assert.Contains(oldfacility.ToEntityReference(), contactAccounts);

            Assert.DoesNotContain(newWebRole.ToEntityReference(), webRoles);
            Assert.Contains(oldWebRole.ToEntityReference(), webRoles);
            Assert.Null(updatedContact?.ipg_backupjson);
        }

    }
}
