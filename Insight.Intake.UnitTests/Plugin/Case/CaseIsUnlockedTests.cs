using System.Collections.Generic;
using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.Plugin.Gating.Constants;
using Insight.Intake.Plugin.Managers;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Xunit;
using static Insight.Intake.Helpers.Constants;
using static Insight.Intake.Plugin.Managers.TaskManager;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class CaseIsUnlockedTests : PluginTestsBase
    {
        [Fact]
        public void CaseIsUnlocked_Success()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            ipg_taskcategory taskCategory = new ipg_taskcategory().Fake().WithName("Patient Outreach");
            ipg_taskcategory adminCategory = new ipg_taskcategory().Fake().WithName("Administrative");
            ipg_taskcategory billinfCategory = new ipg_taskcategory().Fake().WithName("Billing");
            ipg_taskreason taskReason = new ipg_taskreason().Fake().RuleFor(x => x.ipg_name, x => TaskStatusReasonNames.CaseUnlocked);
            ipg_tasktype ClaimGenerationType = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.GENERATE_SUBMIT_CLAIM);
            ipg_tasktype UnlockCaseType = new ipg_tasktype().Fake().WithTypeId(TaskTypeIds.REQUEST_TO_UNLOCK_CASE);

            Incident caseEntity = new Incident().Fake();
            Task generateClaimTask = new Task().Fake().WithTypeRef(ClaimGenerationType.ToEntityReference()).WithRegarding(caseEntity.ToEntityReference());
            Task unlockCaseTask = new Task().Fake().WithTypeRef(UnlockCaseType.ToEntityReference()).WithRegarding(caseEntity.ToEntityReference());

            caseEntity.ipg_islocked = false;

            ipg_gateconfiguration gate6 = new ipg_gateconfiguration().Fake("Gate 6");
            ipg_lifecyclestep lfGate6step = new ipg_lifecyclestep(){Id = LifeCycleStep.ADD_PARTS_GATE6};
            ipg_casestatusdisplayed statusMock = new ipg_casestatusdisplayed().Fake().WithName("Mock");
            ipg_gateprocessingrule gateprocessingRule = new ipg_gateprocessingrule().Fake(lfGate6step, lfGate6step, "processRule6_6")
                                                                                    .WithCaseStatusDisplayed(statusMock.ToEntityReference())
                                                                                    .WithCaseStateEnum(ipg_CaseStateCodes.Intake)
                                                                                    .WithCaseStatusEnum(ipg_CaseStatus.Closed);

            Task relTask = new Task().Fake()
                                     .WithRegarding(caseEntity.ToEntityReference())
                                     .WithState(TaskState.Open)
                                     .WithTaskCategoryRef(taskCategory.ToEntityReference());
            ipg_statementgenerationtask psEventsTask = new ipg_statementgenerationtask().Fake().WithCaseReference(caseEntity);

            var listForInit = new List<Entity>() { caseEntity, billinfCategory, adminCategory,  taskCategory, relTask, psEventsTask, taskReason
                , gateprocessingRule, ClaimGenerationType, UnlockCaseType, generateClaimTask, unlockCaseTask };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection {
                { "Target", caseEntity },
            };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "Update",
                Stage = 40,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            //act
            fakedContext.ExecutePluginWith<CaseIsUnlocked>(pluginContext);
            var service = fakedContext.GetOrganizationService();
            var resultTask = service
                .Retrieve(Task.EntityLogicalName, relTask.Id, new ColumnSet("statecode", Task.Fields.ipg_taskreason, Task.Fields.StatusCode))
                .ToEntity<Task>();

            var psresultTask = service
             .Retrieve(ipg_statementgenerationtask.EntityLogicalName, psEventsTask.Id, new ColumnSet(ipg_statementgenerationtask.Fields.StatusCode, ipg_statementgenerationtask.Fields.StateCode))
             .ToEntity<ipg_statementgenerationtask>();

            generateClaimTask = service.Retrieve(Task.EntityLogicalName, generateClaimTask.Id, new ColumnSet(true))
             .ToEntity<Task>();

            unlockCaseTask = service.Retrieve(Task.EntityLogicalName, unlockCaseTask.Id, new ColumnSet(true))
             .ToEntity<Task>();

            //Assert
            Assert.Equal(ipg_statementgenerationtask_StatusCode.Canceled, psresultTask.StatusCodeEnum);
            Assert.Equal(ipg_statementgenerationtaskState.Inactive, psresultTask.StateCode);

            Assert.Equal(TaskState.Completed, resultTask.StateCode);
            Assert.Equal(taskReason.ToEntityReference(), resultTask.ipg_taskreason);
            Assert.Equal(Task_StatusCode.Cancelled, resultTask.StatusCodeEnum);

            Assert.Equal(taskReason.ToEntityReference(), generateClaimTask.ipg_taskreason);
            Assert.Equal(Task_StatusCode.Cancelled, generateClaimTask.StatusCodeEnum);

            Assert.Null(unlockCaseTask.ipg_taskreason);
            Assert.Equal(Task_StatusCode.Resolved, unlockCaseTask.StatusCodeEnum);
        }
    }
}
