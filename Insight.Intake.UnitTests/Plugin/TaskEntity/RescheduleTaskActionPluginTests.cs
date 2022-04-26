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
    public class RescheduleTaskActionPluginTests
    {
        [Fact]
        public void CloseTaskActionPluginTests_TaskResheduled_noCaseNoteCreated()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            SystemUser user = new SystemUser().Fake();
            user.Id = new Guid("{FF4258AB-0D80-471F-8BF6-14984F46C13A}");

            Incident caseEntity = new Incident().Fake();
            caseEntity.Id = new Guid("{F84258AB-0D80-471F-8BF6-14984F46C13A}");
            caseEntity.StateCode = IncidentState.Active;

            Task taskEntity = new Task().Fake();
            taskEntity.Id = new Guid("{F84258AB-0D80-471F-8BF6-14984F46C13B}");
            taskEntity.RegardingObjectId = caseEntity.ToEntityReference();

            Incident additionalTaskCaseEntity = new Incident().Fake();
            additionalTaskCaseEntity.Id = new Guid("{F84258AB-0D80-471F-8BF6-14984F46C13E}");
            additionalTaskCaseEntity.StateCode = IncidentState.Active;

            Task additionalTask = new Task().Fake();
            additionalTask.Id = new Guid("{F84258AB-0D80-471F-8BF6-14984F46C13F}");
            additionalTask.RegardingObjectId = additionalTaskCaseEntity.ToEntityReference();

            DateTime newDate = new DateTime(2018, 02, 02);
            var listForInit = new List<Entity>() { taskEntity, user, caseEntity , additionalTask, additionalTaskCaseEntity };
            fakedContext.Initialize(listForInit);

            var rescheduleNote = "Reschedule Note";

            var inputParameters = new ParameterCollection
            {
                { "Target", taskEntity.ToEntityReference() },
                { "AdditionalTasks", additionalTask.Id.ToString()},
                { "NewStartDate", newDate },
                { "RescheduleNote", rescheduleNote },
                { "ProduceTaskNote", false }
            };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGTaskActionsRescheduleTask",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = user.Id
            };
            //ACT
            fakedContext.ExecutePluginWith<RescheduleTaskActionPlugin>(pluginContext);
            var tasks = (from t in fakedContext.CreateQuery<Task>()
                         select t).ToList();
            var notes = (from n in fakedContext.CreateQuery<Annotation>()
                         where n.ObjectId.Id == taskEntity.RegardingObjectId.Id
                         select n).ToList();
            //Assert
            Assert.True(tasks.Any());
            Assert.False(notes.Any());
        }
        [Fact]
        public void CloseTaskActionPluginTests_TaskResheduled_CaseNoteCreated()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            SystemUser user = new SystemUser().Fake();
            user.Id = new Guid("{FF4258AB-0D80-471F-8BF6-14984F46C13A}");

            Incident caseEntity = new Incident().Fake();
            caseEntity.Id = new Guid("{F84258AB-0D80-471F-8BF6-14984F46C13A}");
            caseEntity.StateCode = IncidentState.Active;
            DateTime newDate = new DateTime(2018, 02, 02);
            Task taskEntity = new Task().Fake();
            taskEntity.Id = new Guid("{F84258AB-0D80-471F-8BF6-14984F46C13B}");
            taskEntity.RegardingObjectId = caseEntity.ToEntityReference();

            Incident additionalTaskCaseEntity = new Incident().Fake();
            additionalTaskCaseEntity.Id = new Guid("{F84258AB-0D80-471F-8BF6-14984F46C13E}");
            additionalTaskCaseEntity.StateCode = IncidentState.Active;

            Task additionalTask = new Task().Fake();
            additionalTask.Id = new Guid("{F84258AB-0D80-471F-8BF6-14984F46C13F}");
            additionalTask.RegardingObjectId = additionalTaskCaseEntity.ToEntityReference();

            var listForInit = new List<Entity>() { taskEntity, user, caseEntity , additionalTaskCaseEntity, additionalTask };
            fakedContext.Initialize(listForInit);

            var rescheduleNote = "Reschedule Note";

            var inputParameters = new ParameterCollection
            {
                { "Target", taskEntity.ToEntityReference() },
                { "AdditionalTasks", additionalTask.Id.ToString()},
                { "NewStartDate", newDate },
                { "RescheduleNote", rescheduleNote },
                { "ProduceTaskNote", true }
            };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGTaskActionsRescheduleTask",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null,
                InitiatingUserId = user.Id
            };
            //ACT
            fakedContext.ExecutePluginWith<RescheduleTaskActionPlugin>(pluginContext);
            var tasks = (from t in fakedContext.CreateQuery<Task>()
                         select t).ToList();
            var notes = (from n in fakedContext.CreateQuery<Annotation>()
                             //     where n.ObjectId.Id == taskEntity.RegardingObjectId.Id
                         select n).ToList();
            //Assert
            Assert.True(tasks.Any());
            Assert.True(notes.Any());
        }
    }
}
