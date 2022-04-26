using FakeXrmEasy;
using Insight.Intake.Plugin.AccountUser;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.AccountUser
{
    public class CreateTaskWhenThereIsNoCaseManagerForAccountTests: PluginTestsBase
    {
        [Fact]
        public void CreateTaskWhenThereIsNoCaseManagerForAccount_ThereIsNoCaseManager_returnTaskIsCreated()
        {
            var fakedContext = new XrmFakedContext();

            Intake.Account account = new Intake.Account().Fake();
            var user = new SystemUser() { Id = Guid.NewGuid() };
            var accountUser = new ipg_accountuser()
            {
                Id = Guid.NewGuid(),
                ipg_accountid = account.ToEntityReference(),
                ipg_userid = user.ToEntityReference(),
                ipg_isprimary = true,
                ipg_rolecode = new OptionSetValue((int)ipg_accountuser_ipg_rolecode.CaseManager)
            };

            fakedContext.Initialize(new List<Entity> { account, user, accountUser });

            var inputParameters = new ParameterCollection { { "Target", accountUser } };
            var entityImageCollection = new EntityImageCollection() { { "PreImage", accountUser } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Delete",
                Stage = 20,
                PrimaryEntityName = ipg_accountuser.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = entityImageCollection
            };

            fakedContext.ExecutePluginWith<CreateTaskWhenThereIsNoCaseManagerForAccount>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var tasks = (from taskRecord in crmContext.CreateQuery<Task>()
                         where taskRecord.RegardingObjectId.Id == account.Id
                         select taskRecord).ToList();

            Assert.True(tasks.Count == 1);
        }

        [Fact]
        public void CreateTaskWhenThereIsNoCaseManagerForAccount_ThereIsCaseManager_returnTaskIsNotCreated()
        {
            var fakedContext = new XrmFakedContext();

            Intake.Account account = new Intake.Account().Fake();
            var user = new SystemUser() { Id = Guid.NewGuid() };
            var accountUser = new ipg_accountuser()
            {
                Id = Guid.NewGuid(),
                ipg_accountid = account.ToEntityReference(),
                ipg_userid = user.ToEntityReference(),
                ipg_isprimary = true,
                ipg_rolecode = new OptionSetValue((int)ipg_accountuser_ipg_rolecode.CaseManager)
            };

            var accountUser2 = new ipg_accountuser()
            {
                Id = Guid.NewGuid(),
                ipg_accountid = account.ToEntityReference(),
                ipg_userid = new SystemUser() { Id = Guid.NewGuid() }.ToEntityReference(),
                ipg_isprimary = false,
                ipg_rolecode = new OptionSetValue((int)ipg_accountuser_ipg_rolecode.CaseManager)
            };

            fakedContext.Initialize(new List<Entity> { account, user, accountUser2, accountUser });

            var inputParameters = new ParameterCollection { { "Target", accountUser } };
            var entityImageCollection = new EntityImageCollection() { { "PreImage", accountUser } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Delete",
                Stage = 20,
                PrimaryEntityName = ipg_accountuser.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = entityImageCollection
            };

            fakedContext.ExecutePluginWith<CreateTaskWhenThereIsNoCaseManagerForAccount>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var tasks = (from taskRecord in crmContext.CreateQuery<Task>()
                         where taskRecord.RegardingObjectId.Id == account.Id
                         select taskRecord).ToList();

            Assert.True(tasks.Count == 0);
        }
    }
}
