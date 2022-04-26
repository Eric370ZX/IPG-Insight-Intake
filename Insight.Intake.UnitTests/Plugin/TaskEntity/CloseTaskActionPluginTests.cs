using FakeXrmEasy;
using Insight.Intake.Helpers;
using Insight.Intake.Plugin.TaskEntity;
using Insight.Intake.Repositories;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;


namespace Insight.Intake.UnitTests.Plugin.TaskEntity
{
    public class CloseTaskActionPluginTests
    {
        [Fact]
        public void CloseTaskActionPluginTests_TaskClosed_noCaseNoteCreated()
        {
            //arrange
            var fakedContext = new XrmFakedContext();

            Incident caseEntity = new Incident().Fake();
            caseEntity.Id = new Guid("{F84258AB-0D80-471F-8BF6-14984F46C13A}");
            caseEntity.StateCode = IncidentState.Active;

            Insight.Intake.Task taskEntity = new  Insight.Intake.Task().Fake();
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
                { "ProduceTaskNote", false }
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

            Insight.Intake.Task taskEntity = new  Insight.Intake.Task().Fake();
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
            var fakedService = fakedContext.GetOrganizationService();

            Team team = new Team().Fake("Patient Outreach");
            ipg_statementgenerationeventconfiguration statementConfig = new ipg_statementgenerationeventconfiguration().Fake();
            Incident incident = new Incident().Fake().WithLastStatementType(statementConfig);
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithStartDate(1).WithDueDate(3).WithAssignToteam(team.ToEntityReference());
            ipg_taskreason taskReason = new ipg_taskreason().Fake();
            ipg_taskreasondetails taskreasondetails = new ipg_taskreasondetails().Fake().WithTaskType(taskType);
            Task task = new Task().Fake().WithTypeRef(taskType.ToEntityReference()).WithCase(incident.ToEntityReference()).WithRegarding(incident.ToEntityReference()).WithLevel(1);

            var listForInit = new List<Entity>() { team, taskType, task, statementConfig, incident, taskType, taskReason, taskreasondetails };
            fakedContext.Initialize(listForInit);

            fakedContext.Fake_ipg_ipg_taskreasondetails_ipg_statementgene_RelationShip(statementConfig, taskreasondetails);
            fakedContext.Fake_ipg_ipg_taskreasondetails_ipg_taskreason_RelationShip(taskReason, taskreasondetails);

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
            var fakedService = fakedContext.GetOrganizationService();

            Team team = new Team().Fake("Carrier Collection");
            Incident incident = new Incident().Fake().WithPatientCarrierBalance(0,2);
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithStartDate(1).WithDueDate(3).WithAssignToteam(team.ToEntityReference());
            ipg_taskreason taskReason = new ipg_taskreason().Fake().WithTaskType(taskType);
            ipg_taskreasondetails taskReasonDetails = new ipg_taskreasondetails().Fake().WithTaskType(taskType).WithCarrierPatientRules(ipg_Conditions.GreaterThan, 1, ipg_Conditions.Equal, 0);
            Task task = new Task().Fake().WithTypeRef(taskType.ToEntityReference()).WithCase(incident.ToEntityReference()).WithRegarding(incident.ToEntityReference()).WithLevel(1);

            var listForInit = new List<Entity>() { team, taskType, task, incident, taskType, taskReason, taskReasonDetails};
            fakedContext.Initialize(listForInit);

            fakedContext.Fake_ipg_ipg_taskreasondetails_ipg_taskreason_RelationShip(taskReason, taskReasonDetails);

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
        public void TestThatNoTaskCreatedOnFailWithNoStatementsTaskReasonRules()
        {
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            Team team = new Team().Fake("Carrier Collection");
            ipg_statementgenerationeventconfiguration statementConfig = new ipg_statementgenerationeventconfiguration().Fake(PSEvents.A2Generated);
            Incident incident = new Incident().Fake().WithPatientCarrierBalance(0, 2).WithLastStatementType(statementConfig);
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithStartDate(1).WithDueDate(3).WithAssignToteam(team.ToEntityReference());
            ipg_taskreason taskReason = new ipg_taskreason().Fake().WithTaskType(taskType);
            ipg_taskreasondetails taskReasonDetails = new ipg_taskreasondetails().Fake().WithTaskType(taskType).WithNoPatientStatementGenerated();
            Task task = new Task().Fake().WithTypeRef(taskType.ToEntityReference()).WithCase(incident.ToEntityReference()).WithRegarding(incident.ToEntityReference());

            var listForInit = new List<Entity>() { team, taskType, task, incident, taskType, taskReason, taskReasonDetails, statementConfig };
            fakedContext.Initialize(listForInit);

            fakedContext.Fake_ipg_ipg_taskreasondetails_ipg_taskreason_RelationShip(taskReason, taskReasonDetails);

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
            Assert.True(!nextTask.Any());
        }

        [Fact]
        public void TestThatTaskCreatedDueDateStartDateOverridedFromTaskReasonRules()
        {
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            Team team = new Team().Fake("Carrier Collection");
            ipg_statementgenerationeventconfiguration statementConfig = new ipg_statementgenerationeventconfiguration().Fake(PSEvents.A2Generated);
            Incident incident = new Incident().Fake().WithPatientCarrierBalance(0, 2).WithLastStatementType(statementConfig);
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithStartDate(1).WithDueDate(3).WithAssignToteam(team.ToEntityReference());
            ipg_taskreason taskReason = new ipg_taskreason().Fake().WithTaskType(taskType);
            ipg_taskreasondetails taskReasonDetails = new ipg_taskreasondetails().Fake().WithTaskType(taskType).WithStartDueDate(2,1);
            Task task = new Task().Fake().WithTypeRef(taskType.ToEntityReference()).WithCase(incident.ToEntityReference()).WithRegarding(incident.ToEntityReference()).WithLevel(1);

            var listForInit = new List<Entity>() { team, taskType, task, incident, taskType, taskReason, taskReasonDetails, statementConfig };
            fakedContext.Initialize(listForInit);

            fakedContext.Fake_ipg_ipg_taskreasondetails_ipg_taskreason_RelationShip(taskReason, taskReasonDetails);

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
            Assert.Equal(BusinessDayHelper.AddBusinessDays(DateTime.Now,2, null).Date,nextTask.First().ScheduledStart?.Date);
            Assert.Equal(BusinessDayHelper.AddBusinessDays(DateTime.Now, 3, null).Date, nextTask.First().ScheduledEnd?.Date);
        }

        [Fact]
        public void TestThatTaskCreatedOnFailWithNoStatementsTaskReasonRules()
        {
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            Team team = new Team().Fake("Carrier Collection");
            ipg_statementgenerationeventconfiguration statementConfig = new ipg_statementgenerationeventconfiguration().Fake();
            Incident incident = new Incident().Fake().WithPatientCarrierBalance(0, 2);
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithStartDate(1).WithDueDate(3).WithAssignToteam(team.ToEntityReference());
            ipg_taskreason taskReason = new ipg_taskreason().Fake().WithTaskType(taskType);
            ipg_taskreasondetails taskReasonDetails = new ipg_taskreasondetails().Fake().WithTaskType(taskType).WithCarrierPatientRules(ipg_Conditions.GreaterThan, 1, ipg_Conditions.Equal, 0);
            Task task = new Task().Fake().WithTypeRef(taskType.ToEntityReference()).WithCase(incident.ToEntityReference()).WithRegarding(incident.ToEntityReference()).WithLevel(1);

            var listForInit = new List<Entity>() { team, taskType, task, incident, taskType, taskReason, taskReasonDetails };
            fakedContext.Initialize(listForInit);

            fakedContext.Fake_ipg_ipg_taskreasondetails_ipg_taskreason_RelationShip(taskReason, taskReasonDetails);

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
        public void TestThatTaskNotCreatedOnFailTaskReasonRules()
        {
            var fakedContext = new XrmFakedContext();

            Team team = new Team().Fake("Carrier Collection");
            Incident incident = new Incident().Fake().WithPatientCarrierBalance(0, 1);
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithStartDate(1).WithDueDate(3).WithAssignToteam(team.ToEntityReference());
            ipg_taskreason taskReason = new ipg_taskreason().Fake().WithTaskType(taskType);
            ipg_taskreasondetails taskReasonDetails = new ipg_taskreasondetails().Fake().WithTaskType(taskType);
            Task task = new Task().Fake().WithTypeRef(taskType.ToEntityReference()).WithCase(incident.ToEntityReference()).WithRegarding(incident.ToEntityReference());

            var listForInit = new List<Entity>() { team, taskType, task, incident, taskType, taskReason, taskReasonDetails };
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

        [Fact]
        public void TestLevelLogic()
        {
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            Team team = new Team().Fake("Carrier Collection");
            ipg_statementgenerationeventconfiguration statementConfig = new ipg_statementgenerationeventconfiguration().Fake();
            Incident incident = new Incident().Fake().WithPatientCarrierBalance(0, 2);
            ipg_tasktype outgoingCalltaskType = new ipg_tasktype().Fake().WithStartDate(1).WithDueDate(3).WithAssignToteam(team.ToEntityReference());
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithStartDate(1).WithDueDate(3).WithAssignToteam(team.ToEntityReference());
            ipg_taskreason taskReason = new ipg_taskreason().Fake();
            ipg_taskreasondetails taskReasonDetails = new ipg_taskreasondetails().Fake().WithTaskType(outgoingCalltaskType);
            Task task = new Task().Fake().WithTypeRef(taskType.ToEntityReference()).WithCase(incident.ToEntityReference()).WithRegarding(incident.ToEntityReference()).WithLevel(1);
            Task outgoingCallTask = new Task().Fake().WithTypeRef(outgoingCalltaskType.ToEntityReference()).WithCase(incident.ToEntityReference()).WithRegarding(incident.ToEntityReference()).WithLevel(2);

            var listForInit = new List<Entity>() { team, outgoingCallTask, taskType, task, incident, taskReason, taskReasonDetails };
            fakedContext.Initialize(listForInit);

            fakedContext.Fake_ipg_ipg_taskreasondetails_ipg_taskreason_RelationShip(taskReason, taskReasonDetails);

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
                            where t.ipg_tasktypeid != null && t.ipg_tasktypeid.Id == outgoingCalltaskType.Id &&
                             t.RegardingObjectId != null && t.RegardingObjectId.Id == task.RegardingObjectId.Id
                             && t.ipg_level == 3
                            select t).ToList();

            //Assert
            Assert.True(nextTask.Any());
        }
    }
}
