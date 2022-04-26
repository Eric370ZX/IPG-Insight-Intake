using FakeXrmEasy;
using Insight.Intake.Plugin.Contact;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Microsoft.Crm.Sdk.Messages;

namespace Insight.Intake.UnitTests.Plugin.ContactEntity
{
    public class RequestNewFacilityUserNotificationPluginTests : PluginTestsBase
    {
        private const string EmailTemplate = "New Facility User Notification template";
        private const string NoReplyQueueName = "noreplymySurgPro";
        private const string FacilityUserRequestsQueueName = "<Mykheilo Veshchuk>";

        [Fact]
        public void CreatePortalContactWithPendingFacilityStatus_ShouldSendProperEmailNotification()
        {
            // Arrange
            var fakedContext = new XrmFakedContext();
            fakedContext.AddExecutionMock<InstantiateTemplateRequest>(InstantiateTemplateRequestMock);

            var fakedService = fakedContext.GetOrganizationService();

            Intake.Account fakedFacility1 = new Intake.Account().Fake();
            fakedFacility1.Name = "Lacy Erdman";

            Intake.Account fakedFacility2 = new Intake.Account().Fake();
            fakedFacility2.Name = "Tom Harley";

            Contact fakedContact = new Contact().FakeWithFacilityUserStatus(ipg_facility_user_request_status_type.Pending);
            fakedContact.adx_identity_securitystamp = "Top_Secret_Stamp";
            fakedContact.ipg_facilities = ($"{fakedFacility1.Id},{fakedFacility2.Id}");

            Queue fakedNoReplyQueue = new Queue().Fake();
            fakedNoReplyQueue.Name = NoReplyQueueName;
            fakedNoReplyQueue.EMailAddress = "test@mail.com";
            Queue fakedFacilityUserRequestsQueue = new Queue().Fake();
            fakedFacilityUserRequestsQueue.Name = FacilityUserRequestsQueueName;

            var fakedEmailTemplate = new Template();
            fakedEmailTemplate.Id = Guid.NewGuid();
            fakedEmailTemplate.Title = EmailTemplate;
            fakedEmailTemplate.Subject = "New Facility User Request from {Facility Name}";
            fakedEmailTemplate.Body = "This facility has requested the addition of a new Portal user...";

            var fakedNotNotificationEmail = new Email();
            fakedNotNotificationEmail.Id = Guid.NewGuid();

            var listForInit = new List<Entity>() { fakedFacility1, fakedFacility2, fakedContact, fakedEmailTemplate, fakedNotNotificationEmail, fakedNoReplyQueue, fakedFacilityUserRequestsQueue };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection() { { "Target", fakedContact } };

            var fakedPluginExecutionContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = fakedContact.LogicalName,
                InputParameters = inputParameters,
            };

            // Act
            fakedContext.ExecutePluginWith<RequestNewFacilityUserNotificationPlugin>(fakedPluginExecutionContext);

            var query = new QueryByAttribute()
            {
                EntityName = Email.EntityLogicalName,
                ColumnSet = new ColumnSet(true)
            };

            var resultEmailCollection = fakedService.RetrieveMultiple(query);

            var expectedEmail = new Email();
            expectedEmail.Subject = $"New Facility User Request from Lacy Erdman";
            expectedEmail.Description = "This facility has requested the addition of a new Portal user...";
            expectedEmail.StateCode = EmailState.Completed;
            expectedEmail.From = new List<ActivityParty>() { new ActivityParty() { PartyId = fakedNoReplyQueue.ToEntityReference() } };
            expectedEmail.To = new List<ActivityParty>() { new ActivityParty() { PartyId = fakedFacilityUserRequestsQueue.ToEntityReference() } };

            // Assert
            Assert.NotNull(resultEmailCollection?.Entities);
            Assert.True(resultEmailCollection.Entities.Count == 2);
            Assert.Contains(resultEmailCollection.Entities
                .Select(x => x.ToEntity<Email>()), email => 
                    email.Subject == expectedEmail.Subject 
                    && email.Description == expectedEmail.Description
                    && email.StateCode == expectedEmail.StateCode
                    && email.From.ToList()[0].PartyId.Id == expectedEmail.From.ToList()[0].PartyId.Id
                    && email.To.ToList()[0].PartyId.Id == expectedEmail.To.ToList()[0].PartyId.Id);

        }

        private OrganizationResponse InstantiateTemplateRequestMock(OrganizationRequest request)
        {
            var email = new Email()
            {
                Subject = "New Facility User Request from {Facility Name}",
                Description = "This facility has requested the addition of a new Portal user..."
            };
            var response = new InstantiateTemplateResponse();
            response.Results["EntityCollection"] = new EntityCollection(new List<Entity> { email });

            return response;
        }

        [Fact]
        public void CreatePortalContactWithNotPendingFacilityStatus_ShoudNotSendEmailNotification()
        {
            // Arrange
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            Intake.Account fakedFacility1 = new Intake.Account().Fake();
            Intake.Account fakedFacility2 = new Intake.Account().Fake();

            Contact fakedContact = new Contact().FakeWithFacilityUserStatus(ipg_facility_user_request_status_type.Rejected);
            fakedContact.adx_identity_securitystamp = "Top_Secret_Stamp";
            fakedContact.ipg_facilities = ($"{fakedFacility1.Id},{fakedFacility2.Id}");

            var fakedNotNotificationEmail = new Email();
            fakedNotNotificationEmail.Id = Guid.NewGuid();

            var listForInit = new List<Entity>() { fakedFacility1, fakedFacility2, fakedContact, fakedNotNotificationEmail };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection() { { "Target", fakedContact } };
            
            var fakedPluginExecutionContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = fakedContact.LogicalName,
                InputParameters = inputParameters,
            };

            // Act
            fakedContext.ExecutePluginWith<RequestNewFacilityUserNotificationPlugin>(fakedPluginExecutionContext);

            var query = new QueryByAttribute()
            {
                EntityName = Email.EntityLogicalName,
                ColumnSet = new ColumnSet(true)
            };

            var resultEmailCollection = fakedService.RetrieveMultiple(query);

            var expectedEmail = new Email();
            expectedEmail.Id = new Guid();

            // Assert
            Assert.NotNull(resultEmailCollection?.Entities);
            Assert.True(resultEmailCollection.Entities.Count == 1);
            Assert.Contains(resultEmailCollection.Entities, email => email.Id == fakedNotNotificationEmail.Id);
            Assert.DoesNotContain(resultEmailCollection.Entities, email => email.Id == expectedEmail.Id);
        }

        [Fact]
        public void CreateCrmContact_ShoudNotSendEmailNotification()
        {
            // Arrange
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            Intake.Account fakedFacility1 = new Intake.Account().Fake();
            Intake.Account fakedFacility2 = new Intake.Account().Fake();

            Contact fakedContact = new Contact().FakeWithFacilityUserStatus(ipg_facility_user_request_status_type.Pending);
            fakedContact.adx_identity_securitystamp = null;
            fakedContact.ipg_facilities = ($"{fakedFacility1.Id},{fakedFacility2.Id}");

            var fakedNotNotificationEmail = new Email();
            fakedNotNotificationEmail.Id = Guid.NewGuid();

            var listForInit = new List<Entity>() { fakedFacility1, fakedFacility2, fakedContact, fakedNotNotificationEmail };
            fakedContext.Initialize(listForInit);

            var inputParameters = new ParameterCollection() { { "Target", fakedContact } };

            var fakedPluginExecutionContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = fakedContact.LogicalName,
                InputParameters = inputParameters,
            };

            // Act
            fakedContext.ExecutePluginWith<RequestNewFacilityUserNotificationPlugin>(fakedPluginExecutionContext);

            var query = new QueryByAttribute()
            {
                EntityName = Email.EntityLogicalName,
                ColumnSet = new ColumnSet(true)
            };

            var resultEmailCollection = fakedService.RetrieveMultiple(query);

            var expectedEmail = new Email();
            expectedEmail.Id = new Guid();

            // Assert
            Assert.NotNull(resultEmailCollection?.Entities);
            Assert.True(resultEmailCollection.Entities.Count == 1);
            Assert.Contains(resultEmailCollection.Entities, email => email.Id == fakedNotNotificationEmail.Id);
            Assert.DoesNotContain(resultEmailCollection.Entities, email => email.Id == expectedEmail.Id);
        }
    }
}
