using FakeXrmEasy;
using Insight.Intake.Plugin.TaskEntity;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.TaskEntity
{
    public class UpdateAlertFieldOnCaseTests : PluginTestsBase
    {
        [Fact]
        public void Test_UpdateAlertFieldOnCaseTests_Create()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            Incident incident = new Incident().Fake();
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithName("Missing information Tissue Request Form");
            Entity taskEntity = new Entity(Task.EntityLogicalName, new Guid("0B3D3006-5CE1-4C3F-A4AA-3C18EE00F207"));
            taskEntity[Task.Fields.ipg_portalstatus] = new OptionSetValue((int)Task_ipg_portalstatus.Open);
            taskEntity[Task.Fields.ipg_isvisibleonportal] = true;
            taskEntity[Task.Fields.ipg_tasktypeid] = taskType.ToEntityReference();
            taskEntity[Task.Fields.ipg_caseid] = incident.ToEntityReference();
            #endregion

            #region Setup execution context

            var entities = new List<Entity>() { incident, taskType, taskEntity };
            fakedContext.Initialize(entities);
            var inputParameters = new ParameterCollection { { "Target", taskEntity } };
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };
            #endregion

            #region Execute plugin
            fakedContext.ExecutePluginWith<UpdateAlertFieldOnCase>(pluginContext);

            var query = new QueryExpression(Incident.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(Incident.Fields.ipg_portalalerts)
            };
            var updatedCase = fakedService.RetrieveMultiple(query).Entities.SingleOrDefault();
            #endregion
            Assert.NotNull(updatedCase);
            Assert.Equal("Missing Information", updatedCase[Incident.Fields.ipg_portalalerts]);
        }

        [Fact]
        public void Test_UpdateAlertFieldOnCaseTests_Update()
        {
            #region Setup services
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            Incident incident = new Incident().Fake();
            incident.ipg_portalalerts = "PO Issued";
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithName("Missing information Tissue Request Form");
            Task targetTask = new Task().Fake();
            Entity postTask = new Entity(Task.EntityLogicalName, new Guid("0B3D3006-5CE1-4C3F-A4AA-3C18EE00F207"));
            postTask[Task.Fields.ipg_portalstatus] = new OptionSetValue((int)Task_ipg_portalstatus.Open);
            postTask[Task.Fields.ipg_isvisibleonportal] = true;
            postTask[Task.Fields.ipg_tasktypeid] = taskType.ToEntityReference();
            postTask[Task.Fields.ipg_caseid] = incident.ToEntityReference();
            #endregion

            #region Setup execution context

            var entities = new List<Entity>() { incident, postTask, taskType, targetTask };
            fakedContext.Initialize(entities);
            var inputParameters = new ParameterCollection { { "Target", targetTask } };
            
            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Task.EntityLogicalName,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = new EntityImageCollection() { { "PostImage", postTask } },
                PreEntityImages = null
            };
            #endregion

            #region Execute plugin
            fakedContext.ExecutePluginWith<UpdateAlertFieldOnCase>(pluginContext);

            var query = new QueryExpression(Incident.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(Incident.Fields.ipg_portalalerts)
            };
            var updatedCase = fakedService.RetrieveMultiple(query).Entities.SingleOrDefault();
            #endregion
            Assert.NotNull(updatedCase);
            Assert.NotEqual(incident.ipg_portalalerts, updatedCase[Incident.Fields.ipg_portalalerts]);
            Assert.Equal("Missing Information", updatedCase[Incident.Fields.ipg_portalalerts]);
        }
    }
}
