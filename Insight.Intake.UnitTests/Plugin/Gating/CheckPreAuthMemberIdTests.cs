using FakeXrmEasy;
using Insight.Intake.Plugin.Gating;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Gating
{
    public class CheckPreAuthMemberIdTestsTests : PluginTestsBase
    {
        [Fact]
        public void CheckPreAuthMemberIdTests_NoMemberId_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();

            var listForInit = new List<Entity>() { caseEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckPreAuthMemberId",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckPreAuthMemberId>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }
        [Fact]
        public void CheckPreAuthMemberIdTests_SimpleMemberId_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            caseEntity.ipg_MemberIdNumber = "1234";
            var listForInit = new List<Entity>() { caseEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckPreAuthMemberId",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckPreAuthMemberId>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }
        [Fact]
        public void CheckPreAuthMemberIdTests_AuthMemberId_returnError()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            caseEntity.ipg_MemberIdNumber = "PBB1234";
            var listForInit = new List<Entity>() { caseEntity };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckPreAuthMemberId",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckPreAuthMemberId>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == false);
        }
        [Fact]
        public void CheckPreAuthMemberIdTests_AuthMemberIdHasClosedTask_returnSuccess()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            caseEntity.ipg_MemberIdNumber = "PBB1234";
            Task relTask = new Task().Fake().WithRegarding(caseEntity.ToEntityReference());
            relTask.StateCode = TaskState.Completed;
            relTask.ipg_tasktypecodeEnum = ipg_TaskType1.CaserequiresPublixGroupAuthorization;
            var listForInit = new List<Entity>() { caseEntity, relTask };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGGatingCheckPreAuthMemberId",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CheckPreAuthMemberId>(pluginContext);

            //Assert
            Assert.True(pluginContext.OutputParameters.Contains("Succeeded") && pluginContext.OutputParameters["Succeeded"] is bool);
            Assert.True(pluginContext.OutputParameters["Succeeded"] as bool? == true);
        }
    }
}
