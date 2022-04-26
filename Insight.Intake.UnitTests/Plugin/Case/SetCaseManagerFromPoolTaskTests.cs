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
    public class SetCaseManagerFromPoolTaskTests : PluginTestsBase
    {
        [Fact]
        public void Returns_true_when_case_manager_changed_to_task_owner()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();

            //Create case and 'Pool' task, bind them
            Incident incident = new Incident()
                .Fake();

            ipg_taskcategory poolTaskCategory = new ipg_taskcategory()
                .Fake()
                .WithName("Case Processing");
            ipg_tasktype poolTaskType = new ipg_tasktype()
                .Fake()
                .WithName("Request to Complete Case Mgmt. Work (Pool)");
            var taskId = Guid.NewGuid();
            Task poolTaskPreImage = new Task()
                .Fake(taskId)
                .WithTaskCategoryRef(poolTaskCategory.ToEntityReference())
                .WithTypeRef(poolTaskType.ToEntityReference())
                .WithRegarding(incident.ToEntityReference());
            SystemUser owner = new SystemUser()
                .Fake();
            Task updatedPoolTask = new Task()
                .Fake(taskId)
                .WithOwner(owner.ToEntityReference());

            var listForInit = new List<Entity> { incident, poolTaskCategory, poolTaskType, poolTaskPreImage, owner, updatedPoolTask };
            fakedContext.Initialize(listForInit);
            #endregion

            #region Setup execution context
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", updatedPoolTask } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection() { { "PreImage", poolTaskPreImage } },
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin
            fakedContext.ExecutePluginWith<SetCaseManagerFromPoolTask>(pluginContext);
            #endregion

            #region Asserts
            var fakedService = fakedContext.GetOrganizationService();

            var updatedIncident = fakedService.Retrieve(Incident.EntityLogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();

            Assert.NotNull(updatedIncident.ipg_CaseManagerId);
            Assert.Equal(owner.ToEntityReference(),
                updatedIncident.ipg_CaseManagerId);
            #endregion
        }
    }
}
