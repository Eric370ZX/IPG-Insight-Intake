using System;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Contact
{
    public class RequestNewFacilityUserNotificationPlugin : PluginBase
    {
        private const string EmailTemplate = "New Facility User Notification template";
        private const string NoReplyQueueName = "noreplymySurgPro"; // TODO: set PRD noReply queue name
        private const string FacilityUserRequestsQueueName = "<Mykheilo Veshchuk>"; // TODO: set PRD queue name ATL-FacilityUserRequests

        public RequestNewFacilityUserNotificationPlugin() : base(typeof(RequestNewFacilityUserNotificationPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Intake.Contact.EntityLogicalName, PostOperationCreateHandler);
        }

        private void PostOperationCreateHandler(LocalPluginContext localContext)
        {
            var contact = localContext.Target<Intake.Contact>();
            var service = localContext.OrganizationService;

            if (string.IsNullOrEmpty(contact.adx_identity_securitystamp) 
                || contact.ipg_facility_user_status_typecodeEnum != ipg_facility_user_request_status_type.Pending)
            {
                return;
            }

            var emailTemplate = RetrieveEmailTemplateByName(EmailTemplate, service);
            if (emailTemplate == null)
            {
                throw new InvalidPluginExecutionException(OperationStatus.Failed, $"Email template with name '{emailTemplate}' does not exist!");
            }

            InstantiateTemplateResponse templateResponse = service.Execute(new InstantiateTemplateRequest()
            {
                ObjectId = contact.Id,
                ObjectType = contact.LogicalName,
                TemplateId = emailTemplate.Id
            }) as InstantiateTemplateResponse;

            var contactFacilityId = contact.ipg_facilities?.Split(',')?.FirstOrDefault();
            if (!Guid.TryParse(contactFacilityId, out Guid contactFacilityGuid))
            {
                throw new InvalidPluginExecutionException(OperationStatus.Failed, "Portal contact should have at least one related Facility.");
            }

            var contactFacility = service.Retrieve(Intake.Account.EntityLogicalName, contactFacilityGuid, new ColumnSet(Intake.Account.Fields.Name))?.ToEntity<Intake.Account>()
                ?? throw new InvalidPluginExecutionException(OperationStatus.Failed, $"Facility with Id '{contactFacilityId}' does not exist.");

            var noReplyQueue = RetrieveQueueByName(NoReplyQueueName, service)
                ?? throw new InvalidPluginExecutionException(OperationStatus.Failed, $"Queue with name '{NoReplyQueueName}' does not exist.");

            var facilityUserRequestsQueue = RetrieveQueueByName(FacilityUserRequestsQueueName, service)
                ?? throw new InvalidPluginExecutionException(OperationStatus.Failed, $"Queue with name '{FacilityUserRequestsQueueName}' does not exist.");

            var emailEntity = templateResponse.EntityCollection[0].ToEntity<Email>();
            emailEntity.From = new List<ActivityParty>() { new ActivityParty() { PartyId = noReplyQueue.ToEntityReference() } };
            emailEntity.To = new List<ActivityParty>() { new ActivityParty() { PartyId = facilityUserRequestsQueue.ToEntityReference() } };
            emailEntity.RegardingObjectId = contact.ToEntityReference();
            emailEntity.Subject = emailEntity.Subject.Replace("{Facility Name}", contactFacility.Name);
            emailEntity.Id = service.Create(emailEntity);

            SendEmailRequest sendEmailReq = new SendEmailRequest
            {
                EmailId = emailEntity.Id,
                TrackingToken = "",
                IssueSend = true
            };

            service.Execute(sendEmailReq);
        }

        private Queue RetrieveQueueByName(string queueName, IOrganizationService service)
        {
            var queue = service.RetrieveMultiple(new QueryExpression(Queue.EntityLogicalName)
            {
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(Queue.Fields.Name, ConditionOperator.Equal, queueName)
                    }
                }
            }).Entities.FirstOrDefault()?.ToEntity<Queue>();

            return queue;
        }

        private Template RetrieveEmailTemplateByName(string templateName, IOrganizationService service)
        {
            var emailTemplate = service.RetrieveMultiple(new QueryExpression(Template.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(Template.Fields.SubjectSafeHtml, Template.Fields.Body),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(Template.Fields.Title, ConditionOperator.Equal, templateName)
                    }
                }
            }).Entities?.FirstOrDefault()?.ToEntity<Template>();

            return emailTemplate;
        }
    }
}
