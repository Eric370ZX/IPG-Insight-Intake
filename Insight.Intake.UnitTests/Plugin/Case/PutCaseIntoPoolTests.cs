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
    public class PutCaseIntoPoolTests : PluginTestsBase
    {
        [Fact]
        public void Returns_true_when_case_put_to_pool()
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
            Task createdPoolTask = new Task()
                .Fake()
                .WithTaskCategoryRef(poolTaskCategory.ToEntityReference())
                .WithTypeRef(poolTaskType.ToEntityReference())
                .WithRegarding(incident.ToEntityReference());

            var listForInit = new List<Entity> { incident, poolTaskCategory, poolTaskType, createdPoolTask };
            fakedContext.Initialize(listForInit);
            #endregion

            #region Setup execution context
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = new ParameterCollection() { { "Target", createdPoolTask } },
                OutputParameters = new ParameterCollection(),
                PostEntityImages = new EntityImageCollection(),
                PreEntityImages = new EntityImageCollection(),
                InitiatingUserId = Guid.NewGuid()
            };
            #endregion

            #region Execute plugin
            fakedContext.ExecutePluginWith<PutCaseIntoPool>(pluginContext);
            #endregion

            #region Asserts
            var fakedService = fakedContext.GetOrganizationService();

            var poolTask = fakedService.Retrieve(Task.EntityLogicalName, createdPoolTask.Id, new ColumnSet(false));
            var updatedIncident = fakedService.Retrieve(Incident.EntityLogicalName, incident.Id, new ColumnSet(true)).ToEntity<Incident>();

            Assert.NotNull(poolTask);
            Assert.Equal(new OptionSetValue((int)ipg_CaseReasons.ProcedureCompleteQueuedforBilling),
                updatedIncident.ipg_Reasons);
            #endregion
        }
    }
}
