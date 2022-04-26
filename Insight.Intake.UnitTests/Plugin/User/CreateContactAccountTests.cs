using FakeXrmEasy;
using Insight.Intake.Plugin.User;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.User
{
    public class CreateContactAccountTests : PluginTestsBase
    {
        [Fact]
        public void ContactAccountsCreatedonSysAdminRole()
        {
            var fakedContext = new XrmFakedContext();
            SystemUser sysUser = new SystemUser().Fake().WithWmail("test@ipg.com");
            Contact contact = new Contact().FakePortalContact().WithEmail(sysUser.InternalEMailAddress);
            Role role = new Role().Fake();
            Intake.Account facility1 = new Intake.Account().Fake(Account_CustomerTypeCode.Facility);
            Intake.Account facility2 = new Intake.Account().Fake(Account_CustomerTypeCode.Facility);

            fakedContext.Initialize(new List<Entity>() { sysUser, contact, role, facility1, facility2 });
            var inputParameters = new ParameterCollection {
                { "Relationship", new Relationship(){SchemaName ="systemuserroles_association"} },
                {"Target", sysUser.ToEntityReference()},
                {"RelatedEntities",new EntityReferenceCollection(){ role.ToEntityReference() } }};

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Associate,
                Stage = PipelineStages.PostOperation,
                InputParameters = inputParameters,
                OutputParameters = new ParameterCollection(),
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection()
            };

            fakedContext.ExecutePluginWith<CreateContactAccount>(pluginContext);

            var service = fakedContext.GetOrganizationService();
            var cmrContext = new CrmServiceContext(service);

            var contactAccounts = cmrContext.ipg_contactsaccountsSet.Where(c => c.ipg_contactid.Id == contact.Id).ToList();

            Assert.Equal(2, contactAccounts.Count);
        }
        [Fact]
        public void ContactAccountsCreatedForSysAdminRoleOnActiveFacilityCreate()
        {
            var fakedContext = new XrmFakedContext();
            SystemUser sysUser = new SystemUser().Fake().WithWmail("test@ipg.com");
            Contact contact = new Contact().FakePortalContact().WithEmail(sysUser.InternalEMailAddress);
            Role role = new Role().Fake();
            Intake.Account facility1 = new Intake.Account().Fake(Account_CustomerTypeCode.Facility);

            fakedContext.AddRelationship("systemuserroles_association", new XrmFakedRelationship
            {
                IntersectEntity = SystemUserRoles.EntityLogicalName,
                Entity1LogicalName = SystemUser.EntityLogicalName,
                Entity1Attribute = SystemUser.PrimaryIdAttribute,
                Entity2LogicalName = Role.EntityLogicalName,
                Entity2Attribute = Role.PrimaryIdAttribute
            });

            fakedContext.Initialize(new List<Entity>() { sysUser, contact, role});

            var request = new AssociateRequest()
            {
                Target = sysUser.ToEntityReference(),
                RelatedEntities = new EntityReferenceCollection()
                    {
                        role.ToEntityReference()
                    },
                Relationship = new Relationship("systemuserroles_association")
            };

            fakedContext.GetOrganizationService().Execute(request);

            var inputParameters = new ParameterCollection {
                    {"Target",facility1} };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                InputParameters = inputParameters,
                OutputParameters = new ParameterCollection(),
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection()
            };

            fakedContext.ExecutePluginWith<CreateContactAccount>(pluginContext);

            var service = fakedContext.GetOrganizationService();
            var cmrContext = new CrmServiceContext(service);

            var contactAccount = cmrContext.ipg_contactsaccountsSet.Where(
                c => c.ipg_contactid.Id == contact.Id).FirstOrDefault();

            Assert.NotNull(contactAccount);
        }

        [Fact(Skip = "Relevant for integation tests only")]
        //[Fact]
        public void ContactAccountsCreatedonSysAdminRoleOnRealDataTest()
        {
            var stapwatcher = new Stopwatch();
            //Use to catch specific error
            var fakedContext = new XrmRealContext();
            var service = fakedContext.GetOrganizationService();
            var facility = service.Retrieve(Intake.Account.EntityLogicalName, new Guid("{D1F94AF5-2284-EC11-8D21-0022480A16BB}"),new ColumnSet(true));
            //var rolRef = new EntityReference(Role.EntityLogicalName, new Guid("{781B2F1F-2F48-E911-A974-000D3A37FE2A}"));

            var inputParameters = new ParameterCollection {
                    {"Target",facility} };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                InputParameters = inputParameters,
                OutputParameters = new ParameterCollection(),
                PreEntityImages = new EntityImageCollection(),
                PostEntityImages = new EntityImageCollection()
            };

            fakedContext.ExecutePluginWith<CreateContactAccount>(pluginContext);

        }
    }
}
