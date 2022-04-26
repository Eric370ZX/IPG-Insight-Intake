using FakeXrmEasy;
using Insight.Intake.UnitTests;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.Plugin.Referral
{
    public class ReferralOwnerAssignmentTests : PluginTestsBase
    {

        [Fact]
        public void ReferralOwnerAssignmentPluginTest()
        {
            //Arrange
            var fakedContext = new XrmFakedContext();
            ipg_referral referral = new ipg_referral().Fake();
            referral.ipg_statecode = new OptionSetValue((int)ipg_CaseStateCodes.Intake);
            SystemUser user = new SystemUser().Fake();
            ipg_caseassignmentconfig caseAssignmentConfig = new ipg_caseassignmentconfig()
            {
                Id = new System.Guid("F84258AB-0D80-471F-8BF6-14984F46C131"),
                ipg_CaseState = ipg_CaseStateCodes.Intake,
                ipg_AssignToUser = user.ToEntityReference()
            };

            var listForInit = new List<Entity> { referral, user, caseAssignmentConfig };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", referral } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_referral.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection { { "PostImage", referral } }
            };

            //Act
            fakedContext.ExecutePluginWith<ReferralOwnerAssignment>(pluginContext);

            var fakedService = fakedContext.GetOrganizationService();
            var query = new QueryExpression(ipg_referral.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(ipg_referral.Fields.OwnerId)
            };
            var updatedReferral = fakedService.RetrieveMultiple(query).Entities.FirstOrDefault()?.ToEntity<ipg_referral>();

            ///Assert
            Assert.Equal(user.Id, updatedReferral.OwnerId.Id);
            Assert.NotEqual(referral.OwnerId.Id, updatedReferral.OwnerId.Id);
        }

        [Fact]
        public void ReferralOwnerAssignmentWithoutUserAndTeamTest()
        {
            //Arrange
            var fakedContext = new XrmFakedContext();
            ipg_referral referral = new ipg_referral().Fake();
            referral.ipg_statecode = new OptionSetValue((int)ipg_CaseStateCodes.Intake);
            ipg_caseassignmentconfig caseAssignmentConfig = new ipg_caseassignmentconfig()
            {
                Id = new System.Guid("F84258AB-0D80-471F-8BF6-14984F46C131"),
                ipg_CaseState = ipg_CaseStateCodes.Intake,
            };

            var listForInit = new List<Entity> { referral, caseAssignmentConfig };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", referral } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_referral.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection { { "PostImage", referral } }
            };

            //Act
            Exception ex = Assert.Throws<InvalidPluginExecutionException>(() => fakedContext.ExecutePluginWith<ReferralOwnerAssignment>(pluginContext));

            //Assert
            Assert.Equal("Error: Failed getting assignee from Case Assignment Configuration records.", ex.Message);
        }
    }
}