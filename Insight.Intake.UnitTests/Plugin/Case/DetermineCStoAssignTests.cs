using System.Collections.Generic;
using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Xunit;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class DetermineCStoAssignTests : PluginTestsBase
    {
        [Theory]
        [InlineData("ipg_IPGCaseActionsDetermineCStoAssign")]
        public void DetermineOwnerByCarrier(string messageName)
        {
            // Arrange
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();


            Incident fakedCase = new Incident().Fake();
            fakedCase.Id = new System.Guid("33B0AE3E-E213-EC11-B6E6-000D3A3B75A8");
            var fakedCaseRef = fakedCase.ToEntityReference();

            ipg_carriernetwork fakedCarrierNetwork = new ipg_carriernetwork().Fake();
            fakedCarrierNetwork.Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad96");

            Intake.Account fakedCarrier = new Intake.Account()
            {
                Id = new System.Guid("e9717a81-ba0a-e411-b681-6c3be5a8ad91"),
                ipg_carriernetworkid = fakedCarrierNetwork.ToEntityReference()
            };

            Intake.Account fakedSecondaryCarrier = new Intake.Account().Fake();
            fakedSecondaryCarrier.Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad91");

            SystemUser fakedUser1 = new SystemUser().Fake();
            fakedUser1.Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad90");

            SystemUser fakedUser2 = new SystemUser().Fake();
            fakedUser1.Id = new System.Guid("e9718a91-ba0a-e411-b681-6c3be5a8ad90");

            SystemUser fakedUser3 = new SystemUser().Fake();
            fakedUser1.Id = new System.Guid("e9719a91-ba0a-e411-b681-6c3be5a8ad90");

            ipg_assigntocsrule rule1 = new ipg_assigntocsrule()
            {
                Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad76"),
                ipg_carrierid = fakedCarrier.ToEntityReference(),
                ipg_carriernetworkid = new EntityReference(),
                OwnerId = fakedUser1.ToEntityReference(),
            };

            ipg_assigntocsrule rule2 = new ipg_assigntocsrule()
            {
                Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad77"),
                ipg_carrierid = new EntityReference(),
                ipg_carriernetworkid = new EntityReference(),
                OwnerId = fakedUser2.ToEntityReference(),
            };

            ipg_assigntocsrule rule3 = new ipg_assigntocsrule()
            {
                Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad78"),
                ipg_carrierid = new EntityReference(),
                ipg_carriernetworkid = new EntityReference(),
                OwnerId = fakedUser3.ToEntityReference(),
            };

            var listForInit = new List<Entity>() { fakedCase, rule1, rule2, rule3, fakedUser1, fakedUser2, fakedUser3, fakedCarrier, fakedCarrierNetwork };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", fakedCase.ToEntityReference() }, { "PrimaryCarrier", fakedCarrier.ToEntityReference() }, { "SecondaryCarrier", fakedSecondaryCarrier.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "AssignToUserId", null } };
            var fakedPluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = messageName,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters
            };

            // Act
            fakedContext.ExecutePluginWith<DetermineCStoAssign>(fakedPluginContext);
            var incident = fakedService.Retrieve(Incident.EntityLogicalName, fakedCase.Id, new ColumnSet(Incident.Fields.OwnerId))?.ToEntity<Incident>();

            // Assert
            Assert.Equal(fakedPluginContext.OutputParameters["AssignToUserId"], fakedUser1.Id.ToString());
            Assert.Equal(incident?.OwnerId?.Id, fakedUser1.Id);
        }

        [Theory]
        [InlineData("ipg_IPGCaseActionsDetermineCStoAssign")]
        public void NoMatchingRule(string messageName)
        {
            // Arrange
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();


            Incident fakedCase = new Incident().Fake();
            fakedCase.Id = new System.Guid("33B0AE3E-E213-EC11-B6E6-000D3A3B75A8");
            var fakedCaseRef = fakedCase.ToEntityReference();

            ipg_carriernetwork fakedCarrierNetwork = new ipg_carriernetwork().Fake();
            fakedCarrierNetwork.Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad96");

            Intake.Account fakedCarrier = new Intake.Account()
            {
                Id = new System.Guid("e9717a81-ba0a-e411-b681-6c3be1a8ad91"),
                ipg_carriernetworkid = fakedCarrierNetwork.ToEntityReference()
            };

            Intake.Account fakedSecondaryCarrier = new Intake.Account().Fake();
            fakedSecondaryCarrier.Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad91");

            SystemUser fakedUser1 = new SystemUser().Fake();
            fakedUser1.Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad90");

            SystemUser fakedUser2 = new SystemUser().Fake();
            fakedUser1.Id = new System.Guid("e9718a91-ba0a-e411-b681-6c3be5a8ad90");

            SystemUser fakedUser3 = new SystemUser().Fake();
            fakedUser1.Id = new System.Guid("e9719a91-ba0a-e411-b681-6c3be5a8ad90");

            ipg_assigntocsrule rule1 = new ipg_assigntocsrule()
            {
                Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad76"),
                ipg_carrierid = new EntityReference(),
                ipg_carriernetworkid = new EntityReference(),
                OwnerId = fakedUser1.ToEntityReference(),
            };

            ipg_assigntocsrule rule2 = new ipg_assigntocsrule()
            {
                Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad77"),
                ipg_carrierid = new EntityReference(),
                ipg_carriernetworkid = fakedCarrierNetwork.ToEntityReference(),
                OwnerId = fakedUser2.ToEntityReference(),
            };

            ipg_assigntocsrule rule3 = new ipg_assigntocsrule()
            {
                Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad78"),
                ipg_carrierid = new EntityReference(),
                ipg_carriernetworkid = new EntityReference(),
                OwnerId = fakedUser3.ToEntityReference(),
            };

            var listForInit = new List<Entity>() { fakedCase, rule1, rule2, rule3, fakedUser1, fakedUser2, fakedUser3, fakedCarrier, fakedCarrierNetwork };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", fakedCase.ToEntityReference() }, { "PrimaryCarrier", fakedCarrier.ToEntityReference() }, { "SecondaryCarrier", fakedSecondaryCarrier.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "AssignToUserId", null } };
            var fakedPluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = messageName,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters
            };

            // Act
            fakedContext.ExecutePluginWith<DetermineCStoAssign>(fakedPluginContext);
            var incident = fakedService.Retrieve(Incident.EntityLogicalName, fakedCase.Id, new ColumnSet(Incident.Fields.ipg_assignedtoteamid))?.ToEntity<Incident>();

            // Assert
            Assert.Equal(fakedPluginContext.OutputParameters["AssignToUserId"], null);
            Assert.Equal(incident?.ipg_assignedtoteamid.Id, new System.Guid("6FE6456E-8EF9-E911-A813-000D3A33F7CA"));
        }

        [Theory]
        [InlineData("ipg_IPGCaseActionsDetermineCStoAssign")]
        public void OwnerNotFound(string messageName)
        {
            // Arrange
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            SystemUser fakedUser1 = new SystemUser().Fake();
            fakedUser1.Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad90");

            SystemUser fakedUser2 = new SystemUser().Fake();
            fakedUser1.Id = new System.Guid("e9718a91-ba0a-e411-b681-6c3be5a8ad90");

            SystemUser fakedUser3 = new SystemUser().Fake();
            fakedUser1.Id = new System.Guid("e9719a91-ba0a-e411-b681-6c3be5a8ad90");

            Incident fakedCase = new Incident().Fake();
            fakedCase.Id = new System.Guid("33B0AE3E-E213-EC11-B6E6-000D3A3B75A8");
            fakedCase.OwnerId = fakedUser3.ToEntityReference();
            var fakedCaseRef = fakedCase.ToEntityReference();

            ipg_carriernetwork fakedCarrierNetwork = new ipg_carriernetwork().Fake();
            fakedCarrierNetwork.Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad96");

            Intake.Account fakedCarrier = new Intake.Account()
            {
                Id = new System.Guid("e9717a81-ba0a-e411-b681-6c3be5a8ad91"),
                ipg_carriernetworkid = fakedCarrierNetwork.ToEntityReference()
            };

            Intake.Account fakedSecondaryCarrier = new Intake.Account().Fake();
            fakedSecondaryCarrier.Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad91");

            ipg_assigntocsrule rule1 = new ipg_assigntocsrule()
            {
                Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad76"),
                ipg_carrierid = new EntityReference(),
                ipg_carriernetworkid = new EntityReference(),
                OwnerId = fakedUser1.ToEntityReference(),
            };

            ipg_assigntocsrule rule2 = new ipg_assigntocsrule()
            {
                Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad77"),
                ipg_carrierid = new EntityReference(),
                ipg_carriernetworkid = new EntityReference(),
                OwnerId = fakedUser2.ToEntityReference(),
            };

            ipg_assigntocsrule rule3 = new ipg_assigntocsrule()
            {
                Id = new System.Guid("e9717a91-ba0a-e411-b681-6c3be5a8ad78"),
                ipg_carrierid = new EntityReference(),
                ipg_carriernetworkid = new EntityReference(),
                OwnerId = new EntityReference(),
            };

            var listForInit = new List<Entity>() { fakedCase, rule1, rule2, rule3, fakedUser1, fakedUser2, fakedUser3, fakedCarrier, fakedCarrierNetwork };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", fakedCase.ToEntityReference() }, { "PrimaryCarrier", fakedCarrier.ToEntityReference() }, { "SecondaryCarrier", fakedSecondaryCarrier.ToEntityReference() } };
            var outputParameters = new ParameterCollection { { "AssignToUserId", null } };
            var fakedPluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = messageName,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = outputParameters
            };

            // Act
            fakedContext.ExecutePluginWith<DetermineCStoAssign>(fakedPluginContext);
            var incident = fakedService.Retrieve(Incident.EntityLogicalName, fakedCase.Id, new ColumnSet(Incident.Fields.ipg_assignedtoteamid))?.ToEntity<Incident>();

            // Assert
            Assert.Equal(fakedPluginContext.OutputParameters["AssignToUserId"], null);
            Assert.Equal(incident?.ipg_assignedtoteamid.Id, new System.Guid("6FE6456E-8EF9-E911-A813-000D3A33F7CA"));
        }
    }
}
