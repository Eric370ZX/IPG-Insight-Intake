using FakeXrmEasy;
using Insight.Intake.Plugin.TaskEntity;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.TaskEntity
{
    public class CloseByRegardingTests
    {
        [Fact]
        public void CloseByRegardingTests_TaskClosed()
        {
            /// Arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            caseEntity.Id = Guid.NewGuid();

            Task task1Entity = new Task().Fake();
            task1Entity.Id = Guid.NewGuid();
            task1Entity.RegardingObjectId = caseEntity.ToEntityReference();
            task1Entity.StateCode = TaskState.Open;

            Task task2Entity = new Task().Fake();
            task2Entity.Id = Guid.NewGuid();
            task2Entity.RegardingObjectId = caseEntity.ToEntityReference();
            task2Entity.StateCode = TaskState.Open;

            var listForInit = new List<Entity>() { task1Entity, task2Entity };
            fakedContext.Initialize(listForInit);

            var closeReason = (int)Task_StatusCode.Cancelled;
            var closeNote = "Facility Error";
            var inputParameters = new ParameterCollection {
                { "Regarding", caseEntity.ToEntityReference() },
                { "CloseReason", closeReason },
                { "CloseNote", closeNote },
                { "ProduceTaskNote", false }
            };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGTaskCloseByRegarding",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };

            /// Act
            fakedContext.ExecutePluginWith<CloseByRegarding>(pluginContext);
            var tasks = fakedContext.CreateQuery<Task>()
                .Where(task => task.StateCode == TaskState.Completed)
                .Where(task => task.StatusCode.Value == closeReason)
                .Where(task => task.ipg_closurenote == closeNote)
                .Where(task => task.RegardingObjectId.Id == caseEntity.Id)
                .ToList();

            /// Assert
            Assert.True(tasks.Any());
        }
    }
}