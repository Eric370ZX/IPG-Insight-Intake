using FakeXrmEasy;
using Insight.Intake.Plugin.User;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.User
{
    public class RemoveCaseManagerRelationshipOnDeactivationTests: PluginTestsBase
    {
        [Fact]
        public void RemoveCaseManagerRelationship_ThereIsCaseManager_returnRemoveCaseManagerRelationshipIsRemoved()
        {
            var fakedContext = new XrmFakedContext();

            Intake.Account account = new Intake.Account().Fake();
            var user = new SystemUser() { Id = Guid.NewGuid(), IsDisabled = true };
            var accountUserTest = new ipg_accountuser()
            {
                Id = Guid.NewGuid(),
                ipg_accountid = account.ToEntityReference(),
                ipg_userid = user.ToEntityReference(),
                ipg_isprimary = true,
                ipg_rolecode = new OptionSetValue((int)ipg_accountuser_ipg_rolecode.CaseManager)
            };

            fakedContext.Initialize(new List<Entity> { account, user, accountUserTest });

            var inputParameters = new ParameterCollection { { "Target", user } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 40,
                PrimaryEntityName = SystemUser.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };

            fakedContext.ExecutePluginWith<RemoveCaseManagerRelationshipOnDeactivation>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var crmContext = new OrganizationServiceContext(fakedService);

            var accountUsers = (from accountUser in crmContext.CreateQuery<ipg_accountuser>()
                                where accountUser.ipg_rolecode.Value == (int)ipg_accountuser_ipg_rolecode.CaseManager
                                && accountUser.ipg_userid.Id == user.Id
                                select accountUser).ToList();

            Assert.True(accountUsers.Count == 0);
        }
    }
}
