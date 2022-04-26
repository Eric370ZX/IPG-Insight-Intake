using System.Collections.Generic;
using FakeXrmEasy;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Xunit;
using Microsoft.Xrm.Sdk.Query;
using Insight.Intake.Plugin.Case;
using Insight.Intake.Helpers;
using System;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class SetOwnerByStateLifecycleTests : PluginTestsBase
    {
        [Fact]
        public void SetAssigntToTeam_Success()
        {
            // Arrange
            var caseState = ipg_CaseStateCodes.Authorization;
            var lifecicleStepId = Constants.LifecycleStepGuids.IntakeStep2LifecycleStepId;
            var initCaseOwner = new SystemUser() { Id = Guid.NewGuid() };
            var assignToTeam = new Team() { Id = Guid.NewGuid() };

            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            Incident caseEntity = new Incident().Fake()
                .RuleFor(p => p.ipg_StateCode, p => new OptionSetValue((int)caseState))
                .RuleFor(p => p.ipg_lifecyclestepid, p => new EntityReference(ipg_lifecyclestep.EntityLogicalName, lifecicleStepId))
                .RuleFor(p => p.OwnerId, p => initCaseOwner.ToEntityReference());

            ipg_assigntorule assigntorule = new ipg_assigntorule().Fake()
                .RuleFor(p => p.StatusCode, p => new OptionSetValue((int)ipg_assigntorule_StatusCode.Active))
                .RuleFor(p => p.ipg_casestate, p => new OptionSetValue((int)caseState))
                .RuleFor(p => p.ipg_lifecyclestepid, p => new EntityReference(ipg_lifecyclestep.EntityLogicalName, lifecicleStepId))
                .RuleFor(p => p.ipg_assigntoteamid, p => assignToTeam.ToEntityReference());


            var listForInit = new List<Entity>() { caseEntity, assigntorule };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity }};
            var fakedPluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = new EntityImageCollection() { { "PostImage", caseEntity } },
            };

            // Act
            fakedContext.ExecutePluginWith<SetOwnerByStateLifecycle>(fakedPluginContext);
            var resultCase = fakedService.Retrieve(Incident.EntityLogicalName, caseEntity.Id, new ColumnSet(Incident.Fields.ipg_assignedtoteamid))?.ToEntity<Incident>();

            // Assert
            Assert.Equal(resultCase?.ipg_assignedtoteamid?.Id, assignToTeam.Id);
        }

        [Fact]
        public void SetAssigntToTeam_NoConfig_Fail()
        {
            // Arrange
            var caseState = ipg_CaseStateCodes.Authorization;
            var lifecicleStepId = Constants.LifecycleStepGuids.IntakeStep2LifecycleStepId;
            var initCaseOwner = new SystemUser() { Id = Guid.NewGuid() };
            var assignToTeam = new Team() { Id = Guid.NewGuid() };

            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            Incident caseEntity = new Incident().Fake()
                .RuleFor(p => p.ipg_StateCode, p => new OptionSetValue((int)caseState))
                .RuleFor(p => p.ipg_lifecyclestepid, p => new EntityReference(ipg_lifecyclestep.EntityLogicalName, lifecicleStepId))
                .RuleFor(p => p.OwnerId, p => initCaseOwner.ToEntityReference());

            ipg_assigntorule assigntorule = new ipg_assigntorule().Fake()
                .RuleFor(p => p.StatusCode, p => new OptionSetValue((int)ipg_assigntorule_StatusCode.Active))
                .RuleFor(p => p.ipg_casestate, p => new OptionSetValue((int)caseState))
                .RuleFor(p => p.ipg_lifecyclestepid, p => null)
                .RuleFor(p => p.ipg_assigntoteamid, p => assignToTeam.ToEntityReference());


            var listForInit = new List<Entity>() { caseEntity, assigntorule };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", caseEntity } };
            var fakedPluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = new EntityImageCollection() { { "PostImage", caseEntity } },
            };

            // Act
            fakedContext.ExecutePluginWith<SetOwnerByStateLifecycle>(fakedPluginContext);
            var resultCase = fakedService.Retrieve(Incident.EntityLogicalName, caseEntity.Id, new ColumnSet(Incident.Fields.OwnerId))?.ToEntity<Incident>();

            // Assert
            Assert.NotEqual(resultCase?.OwnerId?.Id, assignToTeam.Id);
        }
    }
}
