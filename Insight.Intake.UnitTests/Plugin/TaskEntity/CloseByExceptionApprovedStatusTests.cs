using FakeXrmEasy;
using Insight.Intake.Plugin.TaskEntity;
using Insight.Intake.Repositories;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;


namespace Insight.Intake.UnitTests.Plugin.TaskEntity
{
    public class CloseByExceptionApprovedStatusTests
    {
        [Fact]
        public void CloseByExceptionApprovedStatusTests_TaskIsClosed()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident incident = new Incident().Fake();
            ipg_activitytype activityType = new ipg_activitytype()
                .Fake()
                .RuleFor(p => p.ipg_name, p => "Exception Approved");

            Task taskEntity = new Task()
                .Fake()
                .RuleFor(p=>p.ipg_is_exception_approved,p=>true)
                .WithCase(incident.ToEntityReference());

            var listForInit = new List<Entity>() { taskEntity, incident, activityType };
            fakedContext.Initialize(listForInit);

            int reason = (int)Task_StatusCode.Resolved;
            var inputParameters = new ParameterCollection {
                { "Target", taskEntity }
                };

            var postImageParameters = new EntityImageCollection {
                { "PostImage", taskEntity }
             };
            //var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                //OutputParameters = outputParameters,
                PostEntityImages = postImageParameters,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CloseByExceptionApprovedStatus>(pluginContext);

            var tasks = (from t in fakedContext.CreateQuery<Task>()
                         where (t.StatusCode.Value == reason)
                         select t).ToList();
            var notes = (from n in fakedContext.CreateQuery<Annotation>()
                         where (n.ObjectId.Id == taskEntity.RegardingObjectId.Id
                        )
                         select n).ToList();
            //Assert
            Assert.True(tasks.Any());
            Assert.False(notes.Any());
        }
        [Fact]
        public void CloseTaskActionPluginTests_TaskClosed_CaseNoteCreated()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            caseEntity.Id = new Guid("{F84258AB-0D80-471F-8BF6-14984F46C13A}");
            caseEntity.StateCode = IncidentState.Active;

            Insight.Intake.Task taskEntity = new Insight.Intake.Task().Fake();
            taskEntity.Id = new Guid("{F84258AB-0D80-471F-8BF6-14984F46C13B}");
            taskEntity.RegardingObjectId = caseEntity.ToEntityReference();

            var listForInit = new List<Entity>() { taskEntity };
            fakedContext.Initialize(listForInit);

            var closeNote = "Close Note";
            int reason = (int)Task_StatusCode.Cancelled;
            var inputParameters = new ParameterCollection {
                { "Target", taskEntity.ToEntityReference() },
                { "CloseReason", reason },
                { "CloseNote", closeNote },
                { "ProduceTaskNote", true }
                };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGTaskActionsCloseTask",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CloseTaskActionPlugin>(pluginContext);
            var tasks = (from t in fakedContext.CreateQuery<Task>()
                         where (t.ipg_closurenote == closeNote &&
                         t.StatusCode.Value == reason)
                         select t).ToList();
            var notes = (from n in fakedContext.CreateQuery<Annotation>()
                         where (n.ObjectId.Id == taskEntity.RegardingObjectId.Id &&
                         n.NoteText == closeNote)
                         select n).ToList();
            //Assert
            Assert.True(tasks.Any());
            Assert.True(notes.Any());
        }

        [Fact]
        public void CloseTaskActionPluginTests_NextCarrierCollectionTaskCreated()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            ipg_taskcategory category = new ipg_taskcategory().Fake().WithName("Carrier Collection");
            ipg_tasktype taskType = new ipg_tasktype().Fake();
            ipg_taskreason taskReason = new ipg_taskreason().Fake().WithTaskType(taskType);
            ipg_taskreasondetails taskreasondetail = new ipg_taskreasondetails().Fake().WithTaskType(taskType);
            Incident caseEntity = new Incident().Fake();
            caseEntity.Id = new Guid("{F84258AB-0D80-471F-8BF6-14984F46C13A}");
            caseEntity.StateCode = IncidentState.Active;

            Insight.Intake.Task taskEntity = new Insight.Intake.Task().Fake();
            taskEntity.Id = new Guid("{F84258AB-0D80-471F-8BF6-14984F46C13B}");
            taskEntity.RegardingObjectId = caseEntity.ToEntityReference();

            var listForInit = new List<Entity>() { taskEntity, taskType, taskReason, category, caseEntity, taskreasondetail };
            fakedContext.Initialize(listForInit);

            fakedContext.Fake_ipg_ipg_taskreasondetails_ipg_taskreason_RelationShip(taskReason, taskreasondetail);

            var closeNote = "Close Note";
            int reason = (int)Task_StatusCode.Cancelled;
            var inputParameters = new ParameterCollection {
                { "Target", taskEntity.ToEntityReference() },
                { "CloseReason", reason },
                { "CloseNote", closeNote },
                { "ProduceTaskNote", true },
                {"TaskReason", taskReason.ToEntityReference() }
                };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGTaskActionsCloseTask",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CloseTaskActionPlugin>(pluginContext);

            var tasks = (from t in fakedContext.CreateQuery<Task>() select t).ToList();

            var nextTask = (from t in fakedContext.CreateQuery<Task>()
                            where t.ipg_tasktypeid != null && t.ipg_tasktypeid.Id == taskType.Id &&
                             t.RegardingObjectId != null && t.RegardingObjectId.Id == taskEntity.RegardingObjectId.Id
                            select t).ToList();

            //Assert
            Assert.True(nextTask.Any());
        }

        [Fact]
        public void TestThatNeededTaskCreatedByStatementConfig()
        {
            var fakedContext = new XrmFakedContext();

            Team team = new Team().Fake("Patient Outreach");
            ipg_statementgenerationeventconfiguration statementConfig = new ipg_statementgenerationeventconfiguration().Fake(PSEvents.CarrierPayment);
            Incident incident = new Incident().Fake().WithLastStatementType(statementConfig);
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithStartDate(1).WithDueDate(3).WithAssignToteam(team.ToEntityReference());
            ipg_taskreason taskReason = new ipg_taskreason().Fake();
            ipg_taskreasondetails taskreasondetail = new ipg_taskreasondetails().Fake().WithTaskType(taskType).WithNoPatientStatementGenerated();
            Task task = new Task().Fake().WithTypeRef(taskType.ToEntityReference()).WithCase(incident.ToEntityReference()).WithRegarding(incident.ToEntityReference()).WithLevel(1);

            var listForInit = new List<Entity>() { team, taskType, task, statementConfig, incident, taskReason, taskreasondetail };
            fakedContext.Initialize(listForInit);

            fakedContext.Fake_ipg_ipg_taskreasondetails_ipg_statementgene_RelationShip(statementConfig, taskreasondetail);
            fakedContext.Fake_ipg_ipg_taskreasondetails_ipg_taskreason_RelationShip(taskReason, taskreasondetail);

            var closeNote = "Close Note";
            int reason = (int)Task_StatusCode.Cancelled;
            var inputParameters = new ParameterCollection {
                { "Target", task.ToEntityReference() },
                { "CloseReason", reason },
                { "CloseNote", closeNote },
                { "ProduceTaskNote", true },
                { "TaskReason", taskReason.ToEntityReference() }
                };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGTaskActionsCloseTask",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CloseTaskActionPlugin>(pluginContext);

            var tasks = (from t in fakedContext.CreateQuery<Task>() select t).ToList();

            var nextTask = (from t in fakedContext.CreateQuery<Task>()
                            where t.ipg_tasktypeid != null && t.ipg_tasktypeid.Id == taskType.Id &&
                             t.RegardingObjectId != null && t.RegardingObjectId.Id == task.RegardingObjectId.Id
                             && t.ipg_level == 2
                            select t).ToList();

            //Assert
            Assert.True(nextTask.Any());
        }

        [Fact]
        public void TestThatTaskCreatedOnSuccessTaskReasonRules()
        {
            var fakedContext = new XrmFakedContext();

            Team team = new Team().Fake("Carrier Collection");
            Incident incident = new Incident().Fake().WithPatientCarrierBalance(0, 2);
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithStartDate(1).WithDueDate(3).WithAssignToteam(team.ToEntityReference());
            ipg_taskreason taskReason = new ipg_taskreason().Fake().WithTaskType(taskType);
            ipg_taskreasondetails taskreasondetail = new ipg_taskreasondetails().Fake().WithTaskType(taskType).WithCarrierPatientRules(ipg_Conditions.GreaterThan, 1, ipg_Conditions.Equal, 0);
            Task task = new Task().Fake().WithTypeRef(taskType.ToEntityReference()).WithCase(incident.ToEntityReference()).WithRegarding(incident.ToEntityReference()).WithLevel(1);

            var listForInit = new List<Entity>() { team, taskType, task, incident, taskType, taskReason, taskreasondetail };
            fakedContext.Initialize(listForInit);

            fakedContext.Fake_ipg_ipg_taskreasondetails_ipg_taskreason_RelationShip(taskReason, taskreasondetail);

            var closeNote = "Close Note";
            int reason = (int)Task_StatusCode.Cancelled;

            var closeTaskRequest = new ipg_IPGTaskActionsCloseTaskRequest()
            {
                Target = task.ToEntityReference(),
                CloseReason = reason,
                CloseNote = closeNote,
                ProduceTaskNote = true,
                TaskReason = taskReason.ToEntityReference()
            };

            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = closeTaskRequest.RequestName,
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = closeTaskRequest.Parameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CloseTaskActionPlugin>(pluginContext);


            fakedContext.AddRelationship(ipg_ipg_taskreasondetails_ipg_statementgene.EntityLogicalName, new XrmFakedRelationship
            {
                IntersectEntity = ipg_ipg_taskreasondetails_ipg_statementgene.EntityLogicalName,
                Entity1LogicalName = ipg_taskreasondetails.EntityLogicalName,
                Entity1Attribute = ipg_taskreasondetails.PrimaryIdAttribute,
                Entity2LogicalName = ipg_statementgenerationeventconfiguration.EntityLogicalName,
                Entity2Attribute = ipg_statementgenerationeventconfiguration.PrimaryIdAttribute
            });

            var tasks = (from t in fakedContext.CreateQuery<Task>() select t).ToList();

            var nextTask = (from t in fakedContext.CreateQuery<Task>()
                            where t.ipg_tasktypeid != null && t.ipg_tasktypeid.Id == taskType.Id &&
                             t.RegardingObjectId != null && t.RegardingObjectId.Id == task.RegardingObjectId.Id
                             && t.ipg_level == 2
                            select t).ToList();

            //Assert
            Assert.True(nextTask.Any());
        }

        [Fact]
        public void TestThatTaskNotCreatedOnFailTaskReasonRules()
        {
            var fakedContext = new XrmFakedContext();

            Team team = new Team().Fake("Carrier Collection");
            Incident incident = new Incident().Fake().WithPatientCarrierBalance(0, 1);
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithStartDate(1).WithDueDate(3).WithAssignToteam(team.ToEntityReference());
            ipg_taskreason taskReason = new ipg_taskreason().Fake().WithTaskType(taskType).WithRules("Patient Balance = 0; Carrier Balance > 1");
            Task task = new Task().Fake().WithTypeRef(taskType.ToEntityReference()).WithCase(incident.ToEntityReference()).WithRegarding(incident.ToEntityReference());

            var listForInit = new List<Entity>() { team, taskType, task, incident, taskType, taskReason };
            fakedContext.Initialize(listForInit);


            var closeNote = "Close Note";
            int reason = (int)Task_StatusCode.Cancelled;
            var inputParameters = new ParameterCollection {
                { "Target", task.ToEntityReference() },
                { "CloseReason", reason },
                { "CloseNote", closeNote },
                { "ProduceTaskNote", true },
                { "TaskReason", taskReason.ToEntityReference() }
                };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGTaskActionsCloseTask",
                Stage = 40,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PostEntityImages = null,
                PreEntityImages = null
            };
            //ACT
            fakedContext.ExecutePluginWith<CloseTaskActionPlugin>(pluginContext);

            var tasks = (from t in fakedContext.CreateQuery<Task>() select t).ToList();

            var nextTask = (from t in fakedContext.CreateQuery<Task>()
                            where t.ipg_tasktypeid != null && t.ipg_tasktypeid.Id == taskType.Id &&
                             t.RegardingObjectId != null && t.RegardingObjectId.Id == task.RegardingObjectId.Id
                             && t.ipg_level == 2
                            select t).ToList();

            //Assert
            Assert.False(nextTask.Any());
        }

        [Fact]
        public void TestThatImportantEventLogCreated()
        {
            //ARRANGE
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            var incident = new Incident().Fake().Generate();
            var task = new Task().Fake().WithCase(incident.ToEntityReference()).Generate();

            var fakedImportantEventConfig = new ipg_importanteventconfig()
            {
                Id = new Guid(),
                ipg_name = "ET4",
                ipg_eventdescription = "Revenue approval obtained",
                ipg_eventtype = "Revenue Approval Obtained"
            };

            var fakedActivityType = new ipg_activitytype()
            {
                Id = new Guid(),
                ipg_name = "Revenue Approval Obtained"
            };

            fakedService.Create(fakedImportantEventConfig);
            fakedService.Create(fakedActivityType);

            var listForInit = new List<Entity>() { incident, task };
            fakedContext.Initialize(listForInit);

            var closeNote = "Close Note";
            int reason = (int)Task_StatusCode.Cancelled;
            var inputParameters = new ParameterCollection {
                { "Target", task.ToEntityReference() },
                { "CloseReason", reason },
                { "CloseNote", closeNote },
                { "ProduceTaskNote", true }
                };
            var outputParameters = new ParameterCollection { { "Succeeded", false } };

            var fakedPluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = "ipg_IPGTaskActionsCloseTask",
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = "task",
                InputParameters = inputParameters,
                OutputParameters = outputParameters,
                PreEntityImages = null,
                PostEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<CloseTaskActionPlugin>(fakedPluginContext);
            var query = new QueryExpression(ipg_importanteventslog.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(false)
            };
            var resultLog = fakedService.RetrieveMultiple(query).Entities.SingleOrDefault();

            //ASSERT
            Assert.NotNull(resultLog);
        }
    }
}
