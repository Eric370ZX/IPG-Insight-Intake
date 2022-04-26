using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.Plugin.Contact;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.ContactEntity
{
    public class CreateTaskForFacilityUserPluginTests: PluginTestsBase
    {
        const string TASK_NAME = "New Portal User Request";

        [Fact]
        public void TaskCreatedOnFacilityUserCreationTest()
        {
            var fakedContext = new XrmFakedContext();

            Contact contact = new Contact().Fake();
            Intake.Account facility = new Intake.Account().Fake((int)Account_CustomerTypeCode.Facility);
            ipg_contactsaccounts Contactsaccounts = new ipg_contactsaccounts().Fake(contact, facility);
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithName(TASK_NAME);


            var listForInit = new List<Entity>() { contact, facility, Contactsaccounts, taskType };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", contact } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };
            fakedContext.ExecutePluginWith<CreateTaskForFacilityUserPlugin>(pluginContext);


            var service = fakedContext.GetOrganizationService();

            var createdTask = service.RetrieveMultiple(new QueryByAttribute(Task.EntityLogicalName) { Attributes = { Task.Fields.RegardingObjectId }, Values = { contact.Id } });

            Assert.True(createdTask.Entities.Any(), "There Task has not been created!");
        }

        [Fact]
        public void  TaskNotCreatedOnContactCreationTest()
        {
            var fakedContext = new XrmFakedContext();

            Contact contact = new Contact().Fake();
            Intake.Account facility = new Intake.Account().Fake((int)Account_CustomerTypeCode.Facility);
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithName(TASK_NAME);


            var listForInit = new List<Entity>() { contact, facility, taskType };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", contact } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };
            fakedContext.ExecutePluginWith<CreateTaskForFacilityUserPlugin>(pluginContext);


            var service = fakedContext.GetOrganizationService();

            var createdTask = service.RetrieveMultiple(new QueryByAttribute(Task.EntityLogicalName) { Attributes = { Task.Fields.RegardingObjectId }, Values = { contact.Id } });

            Assert.True(!createdTask.Entities.Any(), "There Task has been created!");
        }

        [Fact]
        public void  TaskClosedAndapprovedOnContactApprovedTest()
        {
            var fakedContext = new XrmFakedContext();

            Contact contact = new Contact().FakeWithFacilityUserStatus(ipg_facility_user_request_status_type.Approved);
            Intake.Account facility = new Intake.Account().Fake((int)Account_CustomerTypeCode.Facility);
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithName(TASK_NAME);
            Task task = new Task().Fake().WithTypeRef(taskType.ToEntityReference()).WithRegarding(contact.ToEntityReference());


            var listForInit = new List<Entity>() { contact, facility, taskType, task };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", contact } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };
            fakedContext.ExecutePluginWith<CreateTaskForFacilityUserPlugin>(pluginContext);


            var service = fakedContext.GetOrganizationService();

            var Task = service.Retrieve(task.LogicalName, task.Id, new ColumnSet(true)).ToEntity<Task>();

            Assert.True(Task.ipg_reviewstatuscodeEnum == Task_ipg_reviewstatuscode.Approved && Task.StateCode == TaskState.Completed && Task.StatusCodeEnum == Task_StatusCode.Resolved, "There Task has not  been Approved!");
        }

        [Fact]
        public void TaskClosedRejecteddOnContactRejectedTest()
        {
            var fakedContext = new XrmFakedContext();

            fakedContext.AddExecutionMock<ipg_IPGFacilityUserSendEmailAboutRejectionRequest>(ipg_IPGFacilityUserSendEmailAboutRejectionRequestMock);
            Contact contact = new Contact().FakeWithFacilityUserStatus(ipg_facility_user_request_status_type.Rejected);
            Intake.Account facility = new Intake.Account().Fake((int)Account_CustomerTypeCode.Facility);
            ipg_tasktype taskType = new ipg_tasktype().Fake().WithName(TASK_NAME);
            Task task = new Task().Fake().WithTypeRef(taskType.ToEntityReference()).WithRegarding(contact.ToEntityReference());


            var listForInit = new List<Entity>() { contact, facility, taskType, task };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection { { "Target", contact } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Update,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = null,
                InputParameters = inputParameters,
                OutputParameters = null,
                PostEntityImages = null,
                PreEntityImages = null
            };
                        
            fakedContext.ExecutePluginWith<CreateTaskForFacilityUserPlugin>(pluginContext);

            var service = fakedContext.GetOrganizationService();

            var Task = service.Retrieve(task.LogicalName, task.Id, new ColumnSet(true)).ToEntity<Task>();

            Assert.True(Task.ipg_reviewstatuscodeEnum == Task_ipg_reviewstatuscode.Rejected && Task.StateCode == TaskState.Completed && Task.StatusCodeEnum == Task_StatusCode.Resolved, "There Task has not  been Rejected!");
        }

        public OrganizationResponse ipg_IPGFacilityUserSendEmailAboutRejectionRequestMock(OrganizationRequest req)
        {
            return new OrganizationResponse { ResponseName = "Successful" };
        }
    }
}
