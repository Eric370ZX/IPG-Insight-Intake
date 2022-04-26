using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class CaseOwnerAssignmentTests : PluginTestsBase
    {
        [Theory]
        [InlineData(MessageNames.Create, (int)ipg_CaseStateCodes.Intake)]
        [InlineData(MessageNames.Update, (int)ipg_CaseStateCodes.Intake)]
        [InlineData(MessageNames.Create, (int)ipg_CaseStateCodes.Authorization)]
        [InlineData(MessageNames.Update, (int)ipg_CaseStateCodes.Authorization)]
        [InlineData(MessageNames.Create, (int)ipg_CaseStateCodes.CaseManagement)]
        [InlineData(MessageNames.Update, (int)ipg_CaseStateCodes.CaseManagement)]
        [InlineData(MessageNames.Create, (int)ipg_CaseStateCodes.Billing)]
        [InlineData(MessageNames.Update, (int)ipg_CaseStateCodes.Billing)]
        [InlineData(MessageNames.Create, (int)ipg_CaseStateCodes.CarrierServices)]
        [InlineData(MessageNames.Update, (int)ipg_CaseStateCodes.CarrierServices)]
        [InlineData(MessageNames.Create, (int)ipg_CaseStateCodes.PatientServices)]
        [InlineData(MessageNames.Update, (int)ipg_CaseStateCodes.PatientServices)]
        public void CreateOrUpdateCase_ShouldAssignCaseOwner(string messageName, int ipg_CaseStateCode)
        {
            // Arrange
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            SystemUser fakedCaseMng = new SystemUser().Fake();
            fakedCaseMng.Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad98");

            Intake.Account fakedAccount = new Intake.Account().Fake();
            fakedAccount.Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad67");
            fakedAccount.ipg_FacilityCaseMgrId = fakedCaseMng.ToEntityReference();

            Incident fakedCase = new Incident().Fake();
            fakedCase.ipg_StateCode = new OptionSetValue(ipg_CaseStateCode);
            fakedCase.ipg_CaseStatusEnum = ipg_CaseStatus.Open;

            SystemUser fakedUser = new SystemUser().Fake();
            fakedUser.Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad73");

            Team fakedTeam = new Team();
            fakedTeam.Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad71");

            ipg_caseassignmentconfig fakedConfig1 = new ipg_caseassignmentconfig()
            {
                Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad75"),
                ipg_CaseState = ipg_CaseStateCodes.Intake,
                ipg_AssignToUser = fakedUser.ToEntityReference(),
                ipg_AssignToTeam = null
            };
            ipg_caseassignmentconfig fakedConfig2 = new ipg_caseassignmentconfig()
            {
                Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad71"),
                ipg_CaseState = ipg_CaseStateCodes.Authorization,
                ipg_AssignToUser = fakedUser.ToEntityReference(),
                ipg_AssignToTeam = null
            };
            ipg_caseassignmentconfig fakedConfig3 = new ipg_caseassignmentconfig()
            {
                Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad76"),
                ipg_CaseState = ipg_CaseStateCodes.CaseManagement,
                ipg_AssignToUser = fakedCaseMng.ToEntityReference(),
                ipg_AssignToTeam = null
            };
            ipg_caseassignmentconfig fakedConfig4 = new ipg_caseassignmentconfig()
            {
                Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad23"),
                ipg_CaseState = ipg_CaseStateCodes.Billing,
                ipg_AssignToUser = null,
                ipg_AssignToTeam = fakedTeam.ToEntityReference()
            };
            ipg_caseassignmentconfig fakedConfig5 = new ipg_caseassignmentconfig()
            {
                Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad90"),
                ipg_CaseState = ipg_CaseStateCodes.CarrierServices,
                ipg_AssignToUser = fakedUser.ToEntityReference(),
                ipg_AssignToTeam = null
            };
            ipg_caseassignmentconfig fakedConfig6 = new ipg_caseassignmentconfig()
            {
                Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad54"),
                ipg_CaseState = ipg_CaseStateCodes.PatientServices,
                ipg_AssignToUser = fakedUser.ToEntityReference(),
                ipg_AssignToTeam = null
            };

            var listForInit = new List<Entity>() { fakedCase, fakedUser, fakedTeam, fakedConfig1, fakedConfig2, fakedConfig3, fakedConfig4, fakedConfig5, fakedConfig6, fakedCaseMng, fakedAccount };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", fakedCase } };

            var fakedPluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = messageName,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = new EntityImageCollection { { "PostImage", fakedCase } },
            };

            // Act
            fakedContext.ExecutePluginWith<CaseOwnerAssignment>(fakedPluginContext);
            var incident = fakedService.Retrieve(Incident.EntityLogicalName, fakedCase.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(Incident.Fields.OwnerId))?.ToEntity<Incident>();

            // Assert
            switch (fakedCase.ipg_StateCodeEnum)
            {
                case ipg_CaseStateCodes.Intake:
                case ipg_CaseStateCodes.Authorization:
                case ipg_CaseStateCodes.CarrierServices:
                case ipg_CaseStateCodes.PatientServices:
                    Assert.Equal(fakedUser.Id, incident.OwnerId.Id);
                    break;
                case ipg_CaseStateCodes.CaseManagement:
                    Assert.Equal(fakedCaseMng.Id, incident.OwnerId.Id);
                    break;
                case ipg_CaseStateCodes.Billing:
                    Assert.Equal(fakedTeam.Id, incident.OwnerId.Id);
                    break;
                default:
                    break;
            }
        }
    }
}
