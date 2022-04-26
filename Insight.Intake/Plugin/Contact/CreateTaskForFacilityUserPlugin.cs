using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace Insight.Intake.Plugin.Contact
{
    public class CreateTaskForFacilityUserPlugin : PluginBase
    {
        public CreateTaskForFacilityUserPlugin() : base(typeof(CreateTaskForFacilityUserPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Intake.Contact.EntityLogicalName, PostOperationCreateHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Intake.Contact.EntityLogicalName, PostOperationUpdateHandler);
        }

        private void PostOperationUpdateHandler(LocalPluginContext localContext)
        {
            var contact = localContext.Target<Intake.Contact>();
            var taskManager  = new TaskManager(localContext.OrganizationService, localContext.TracingService);

            var task = taskManager.GetRelatedTask(contact.ToEntityReference(), Constants.TaskTypeNames.NewPortalUserRequest);

            if (contact.ipg_facility_user_status_typecodeEnum == ipg_facility_user_request_status_type.Rejected)
            {
                SendEmailAboutRejection(contact, localContext.OrganizationService, localContext.TracingService);
            }

            if (task != null)
            {
                if (contact.ipg_facility_user_status_typecodeEnum == ipg_facility_user_request_status_type.Rejected || contact.ipg_facility_user_status_typecodeEnum == ipg_facility_user_request_status_type.Approved)
                {
                    localContext.OrganizationService.Update(new Task()
                    {
                        Id = task.Id,
                        StateCode = TaskState.Completed,
                        StatusCodeEnum = Task_StatusCode.Resolved,
                        ipg_reviewstatuscodeEnum = contact.ipg_facility_user_status_typecodeEnum == ipg_facility_user_request_status_type.Rejected ? Task_ipg_reviewstatuscode.Rejected : Task_ipg_reviewstatuscode.Approved
                    });
                }
            }
        }

        private void SendEmailAboutRejection(Intake.Contact contact, IOrganizationService organizationService, ITracingService tracingService)
        {
            organizationService.Execute(new ipg_IPGFacilityUserSendEmailAboutRejectionRequest() { Target = contact.ToEntityReference() });
        }

        private void PostOperationCreateHandler(LocalPluginContext localContext)
        {
            var contact = localContext.Target<Intake.Contact>();

            if (IsContactFacilityUser(contact, localContext.OrganizationService, localContext.TracingService))
            {
                CreateNewPortalUserRequest(contact, localContext.OrganizationService, localContext.TracingService);
            }
        }

        private void CreateNewPortalUserRequest(Intake.Contact contact, IOrganizationService organizationService, ITracingService tracingService)
        {
            new TaskManager(organizationService, tracingService).CreateTask(contact.ToEntityReference(), Constants.TaskTypeNames.NewPortalUserRequest, new Task() {ipg_reviewstatuscodeEnum = Task_ipg_reviewstatuscode.PendingReview });
        }

        private bool IsContactFacilityUser(Intake.Contact contact, IOrganizationService organizationService, ITracingService tracingService)
        {
            var facilityRelationShip = organizationService.RetrieveMultiple(new QueryExpression(ipg_contactsaccounts.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(false),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_contactsaccounts.Fields.ipg_contactid, ConditionOperator.Equal, contact.Id)
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(ipg_contactsaccounts.EntityLogicalName, Intake.Account.EntityLogicalName, ipg_contactsaccounts.Fields.ipg_accountid, Intake.Account.Fields.AccountId, JoinOperator.Inner)
                    {
                        Columns = new ColumnSet(false),
                        LinkCriteria = new FilterExpression()
                        {
                            Conditions =
                            {
                                new ConditionExpression(Intake.Account.Fields.CustomerTypeCode, ConditionOperator.Equal, (int)Account_CustomerTypeCode.Facility)
                            }
                        }
                    }
                }
            }).Entities;

            return facilityRelationShip.Any();
        }
    }
}