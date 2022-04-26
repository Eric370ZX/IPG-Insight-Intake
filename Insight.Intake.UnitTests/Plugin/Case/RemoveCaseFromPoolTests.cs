using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class RemoveCaseFromPoolTests : PluginTestsBase
    {
        [Fact]
        public void Returns_true_when_case_removed_from_pool()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            //Create case and 'Pool' task, bind them
            SystemUser originalCaseManager = new SystemUser()
                .Fake(); 

            Incident incident = new Incident()
                .Fake()
                .WithCMAssigned(originalCaseManager.ToEntityReference());

            ipg_taskcategory poolTaskCategory = new ipg_taskcategory()
                .Fake()
                .WithName("Case Processing");
            ipg_tasktype poolTaskType = new ipg_tasktype()
                .Fake()
                .WithName("Request to Complete Case Mgmt. Work (Pool)");
            var taskId = Guid.NewGuid();
            Task preImage = new Task()
                .Fake(taskId)
                .WithTaskCategoryRef(poolTaskCategory.ToEntityReference())
                .WithTypeRef(poolTaskType.ToEntityReference())
                .WithRegarding(incident.ToEntityReference());
            Task closedPoolTask = new Task()
                .Fake(taskId)
                .WithState(TaskState.Completed);

            var listForInit = new List<Entity> { originalCaseManager, incident, poolTaskCategory, poolTaskType, preImage, closedPoolTask };
            fakedContext.Initialize(listForInit);
            #endregion

            #region Setup execution context
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", closedPoolTask } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection() { { "PreImage", preImage } },
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin
            fakedContext.ExecutePluginWith<RemoveCaseFromPool>(pluginContext);
            #endregion

            #region Asserts
            var fakedService = fakedContext.GetOrganizationService();

            var updatedIncident = fakedService.Retrieve(Incident.EntityLogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();

            Assert.NotNull(updatedIncident);
            Assert.Equal(new OptionSetValue((int)ipg_CaseReasons.ProcedureCompletePendingInformation),
                updatedIncident.ipg_Reasons);
            #endregion
        }
    }
}
