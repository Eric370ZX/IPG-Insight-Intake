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
    public class CheckThereIsOnlyOnePrimaryCaseManagerTests: PluginTestsBase
    {
        [Fact]
        public void CheckThereIsOnlyOnePrimaryCaseManager_ThereIsNoPrimaryCaseManager_returnPrimaryCaseManagerIsCreated()
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

            fakedContext.Initialize(new List<Entity> { account, user });

            var inputParameters = new ParameterCollection { { "Target", accountUser } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 20,
                PrimaryEntityName = ipg_accountuser.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CheckThereIsOnlyOnePrimaryCaseManager>(pluginContext);

        }

        [Fact]
        public void CheckThereIsOnlyOnePrimaryCaseManager_ThereIsNoPrimaryCaseManagerAndTargetIsNotPrimary_returnPrimaryCaseManagerIsCreated()
        {
            var fakedContext = new XrmFakedContext();

            Intake.Account account = new Intake.Account().Fake();
            var user = new SystemUser() { Id = Guid.NewGuid() };
            var accountUser = new ipg_accountuser()
            {
                Id = Guid.NewGuid(),
                ipg_accountid = account.ToEntityReference(),
                ipg_userid = user.ToEntityReference(),
                ipg_isprimary = false,
                ipg_rolecode = new OptionSetValue((int)ipg_accountuser_ipg_rolecode.CaseManager)
            };

            fakedContext.Initialize(new List<Entity> { account, user });

            var inputParameters = new ParameterCollection { { "Target", accountUser } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 20,
                PrimaryEntityName = ipg_accountuser.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<CheckThereIsOnlyOnePrimaryCaseManager>(pluginContext);

            Assert.True(accountUser.ipg_isprimary == true);
        }

        [Fact]
        public void CheckThereIsOnlyOnePrimaryCaseManager_ThereIsPrimaryCaseManager_returnError()
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
                ipg_isprimary = true,
                ipg_rolecode = new OptionSetValue((int)ipg_accountuser_ipg_rolecode.CaseManager)
            };

            fakedContext.Initialize(new List<Entity> { account, user, accountUser2 });

            var inputParameters = new ParameterCollection { { "Target", accountUser } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = 20,
                PrimaryEntityName = ipg_accountuser.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            Exception ex = Assert.Throws<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<CheckThereIsOnlyOnePrimaryCaseManager>(pluginContext));
            Assert.Equal("Error: Primary case manager already exists!", ex.Message);
        }
    }
}
