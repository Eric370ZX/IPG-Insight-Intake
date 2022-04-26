using System.Collections.Generic;
using FakeXrmEasy;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Xunit;
using Microsoft.Xrm.Sdk.Query;
using Insight.Intake.Plugin.Case;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class DetermineCMtoAssignTests : PluginTestsBase
    {
        [Theory]
        [InlineData("ipg_IPGCaseActionsDetermineCMtoAssign")]
        public void CheckSetCaseMngr(string messageName)
        {
            // Arrange
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();


            SystemUser fakedUser1 = new SystemUser().Fake();
            fakedUser1.Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad90");

            Intake.Account fakedFacility = new Intake.Account()
            {
                Id = new System.Guid("33B0AE3E-E213-EC11-B6E6-000D3A3B75A0"),
                ipg_FacilityCaseMgrId = fakedUser1.ToEntityReference()

            };

            Incident fakedCase = new Incident().Fake();

            var listForInit = new List<Entity>() { fakedCase, fakedUser1, fakedFacility };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", fakedCase.ToEntityReference() }, { "FacilityRef", fakedFacility.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "AssignToUserId", null }, { "AssignToTeamId", null } };
            var fakedPluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = messageName,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters
            };

            // Act
            fakedContext.ExecutePluginWith<DetermineCMtoAssign>(fakedPluginContext);
            var incident = fakedService.Retrieve(Incident.EntityLogicalName, fakedCase.Id, new ColumnSet(Incident.Fields.OwnerId))?.ToEntity<Incident>();

            // Assert
            Assert.Equal(fakedPluginContext.OutputParameters["AssignToUserId"], fakedUser1.Id.ToString());
            Assert.Equal(incident?.OwnerId?.Id, fakedUser1.Id);
        }

        [Theory]
        [InlineData("ipg_IPGCaseActionsDetermineCMtoAssign")]
        public void CheckNullMngr(string messageName)
        {
            // Arrange
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            Intake.Account fakedFacility = new Intake.Account()
            {
                Id = new System.Guid("33B0AE3E-E213-EC11-B6E6-000D3A3B75A0"),
                ipg_FacilityCaseMgrId = null

            };

            Incident fakedCase = new Incident().Fake();

            var listForInit = new List<Entity>() { fakedCase, fakedFacility };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", fakedCase.ToEntityReference() }, { "FacilityRef", fakedFacility.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "AssignToUserId", null }, { "AssignToTeamId", null } };
            var fakedPluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = messageName,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters
            };

            // Act
            fakedContext.ExecutePluginWith<DetermineCMtoAssign>(fakedPluginContext);
            var incident = fakedService.Retrieve(Incident.EntityLogicalName, fakedCase.Id, new ColumnSet(Incident.Fields.ipg_assignedtoteamid))?.ToEntity<Incident>();

            // Assert
            Assert.Equal(fakedPluginContext.OutputParameters["AssignToUserId"], null);
            Assert.Equal(incident?.ipg_assignedtoteamid?.Id, new System.Guid("EB250319-B41D-E911-A979-000D3A370E23"));
        }

        [Theory]
        [InlineData("ipg_IPGCaseActionsDetermineCMtoAssign")]
        public void CheckNullFacility(string messageName)
        {
            // Arrange
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();


            SystemUser fakedUser1 = new SystemUser().Fake();
            fakedUser1.Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad90");

            Incident fakedCase = new Incident().Fake();

            var listForInit = new List<Entity>() { fakedCase, fakedUser1 };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", fakedCase.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "AssignToUserId", null }, { "AssignToTeamId", null } };
            var fakedPluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = messageName,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters
            };

            // Act
            fakedContext.ExecutePluginWith<DetermineCMtoAssign>(fakedPluginContext);
            var incident = fakedService.Retrieve(Incident.EntityLogicalName, fakedCase.Id, new ColumnSet(Incident.Fields.ipg_assignedtoteamid))?.ToEntity<Incident>();

            // Assert
            Assert.Equal(fakedPluginContext.OutputParameters["AssignToUserId"], null);
            Assert.Equal(incident?.ipg_assignedtoteamid?.Id, new System.Guid("EB250319-B41D-E911-A979-000D3A370E23"));
        }
    }
}
