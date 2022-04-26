using FakeXrmEasy;
using Insight.Intake.Plugin.AccountUser;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.AccountUser
{
    public class CheckAccountUserOnDuplicatePluginTests: PluginTestsBase
    {
        [Fact]
        public void CheckWhenDuplicationExists()
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
                ipg_rolecode = new OptionSetValue((int)ipg_accountuser_ipg_rolecode.CaseManager),
                StateCode = ipg_accountuserState.Active
            };

            var duplicate = accountUser.Clone();
            duplicate["ipg_accountuserid"] = Guid.NewGuid();
            duplicate["ipg_isprimary"] = false;

            duplicate.ToEntity<ipg_accountuser>().StateCode = ipg_accountuserState.Active;

            fakedContext.Initialize(new List<Entity> { account, user, duplicate });


            var inputParameters = new ParameterCollection { { "Target", accountUser } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = ipg_accountuser.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = new EntityImageCollection()
            };

            var error = Assert.Throws<InvalidPluginExecutionException>(()=>fakedContext.ExecutePluginWith<CheckAccountUserOnDuplicatePlugin>(pluginContext));
            Assert.Contains("This relationship already exist!", error.Message);
        }

        [Fact]
        public void CheckWhenInactiveDuplicationExists()
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
                ipg_rolecode = new OptionSetValue((int)ipg_accountuser_ipg_rolecode.CaseManager),
                StateCode = ipg_accountuserState.Active,
        };

            var duplicate = accountUser.Clone().ToEntity<ipg_accountuser>();
            duplicate.Id= Guid.NewGuid();
            duplicate.ToEntity<ipg_accountuser>().StateCode = ipg_accountuserState.Inactive;

            fakedContext.Initialize(new List<Entity> { account, user, duplicate });

            var inputParameters = new ParameterCollection { { "Target", accountUser } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = ipg_accountuser.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = new EntityImageCollection()
            };

            fakedContext.ExecutePluginWith<CheckAccountUserOnDuplicatePlugin>(pluginContext);
        }

        [Fact]
        public void CheckWhenDuplicationNotExists()
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
                ipg_rolecode = new OptionSetValue((int)ipg_accountuser_ipg_rolecode.CaseManager),
                StateCode = ipg_accountuserState.Active

            };

            fakedContext.Initialize(new List<Entity> { account, user });

            var inputParameters = new ParameterCollection { { "Target", accountUser } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Create",
                Stage = PipelineStages.PreOperation,
                PrimaryEntityName = ipg_accountuser.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = new EntityImageCollection()
            };

            fakedContext.ExecutePluginWith<CheckAccountUserOnDuplicatePlugin>(pluginContext);
        }
    }
}
