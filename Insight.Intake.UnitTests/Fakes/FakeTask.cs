using Bogus;
using Insight.Intake.Extensions;
using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class FakeTask
    {
        public static Task FakeTaskForDerivedHomePlan(this Task task, string subject, string description, Guid id)
        {
            task.Id = new Guid("c3e33475-b767-477a-a7cb-3ea636c8b83b");
            task.Subject = subject;
            task.Description = description;
            task.RegardingObjectId = new EntityReference(Incident.EntityLogicalName, id);
            return task;
        }

        public static Faker<Task> FakeTissueRequestForm(this Task self, Guid caseId, EntityReference taskCategoryRef, EntityReference taskTypeRef)
        {
            string subject = "Missing Information: Tissue Request Form";

            return new Faker<Task>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.Subject, x => subject)
                .RuleFor(x => x.RegardingObjectId, x => new EntityReference(Incident.EntityLogicalName, caseId))
                .RuleFor(x => x.ipg_caseid, x => new EntityReference(Incident.EntityLogicalName, caseId))
                .RuleFor(x => x.ipg_taskcategoryid, x => taskCategoryRef)
                .RuleFor(x => x.ipg_tasktypeid, x => taskTypeRef)
                .RuleFor(x => x.ipg_taskcategorycode, x => ipg_Taskcategory1.User.ToOptionSetValue())
                .RuleFor(x => x.ipg_systemtasktypecode, x => ipg_SystemTaskType.WorkflowTask_InfoorWarning.ToOptionSetValue())
                .RuleFor(x => x.ipg_tasktypecode, ipg_TaskType1.MissinginformationTissueRequestForm.ToOptionSetValue())
                .RuleFor(x => x.StateCode, x => TaskState.Open)
                .RuleFor(x => x.StatusCode, x => Task_StatusCode.NotStarted.ToOptionSetValue());
        }

        public static Faker<Task> Fake(this Task self)
        {
            return new Faker<Task>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.StateCode, x => TaskState.Open);
        }

        public static Faker<Task> Fake(this Task self, Guid id)
        {
            return new Faker<Task>()
                .RuleFor(x => x.Id, x => id);
        }

        public static Faker<Task> WithSubCategory(this Faker<Task> self, string subcategory)
        {
            return self.RuleFor(x => x.Subcategory, x => subcategory);
        }

        public static Faker<Task> FakeSubmitGenerateClaim(this Task self, Guid caseId)
        {
            return new Faker<Task>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.Subject, x => TaskHelper.GenerateSubmitClaimSubject)
                .RuleFor(x => x.RegardingObjectId, x => new EntityReference(Incident.EntityLogicalName, caseId))
                .RuleFor(x => x.ipg_caseid, x => new EntityReference(Incident.EntityLogicalName, caseId))
                .RuleFor(x => x.ipg_taskcategorycode, x => ipg_Taskcategory1.System.ToOptionSetValue())
                .RuleFor(x => x.ipg_systemtasktypecode, x => ipg_SystemTaskType.WorkflowTask_InfoorWarning.ToOptionSetValue())
                .RuleFor(x => x.ipg_tasktypecode, ipg_TaskType1.GenerateSubmitClaim.ToOptionSetValue())
                .RuleFor(x => x.StateCode, x => TaskState.Open)
                .RuleFor(x => x.StatusCode, x => Task_StatusCode.NotStarted.ToOptionSetValue());
        }

        public static Faker<Task> FakeOpenTask(this Task self)
        {
            return new Faker<Task>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_portalstatus, x => Task_ipg_portalstatus.Open.ToOptionSetValue())
                .RuleFor(x=>x.ipg_isvisibleonportal, x=> true);
        }

        public static Faker<Task> WithPortalStatus(this Faker<Task> self, Task_ipg_portalstatus portalStatus)
        {
            return self.RuleFor(x => x.ipg_portalstatus, x => new OptionSetValue((int)portalStatus));
        }

        public static Faker<Task> WithType(this Faker<Task> self, ipg_TaskType1 taskType)
        {
            return self.RuleFor(x => x.ipg_tasktypecode, x => new OptionSetValue((int)taskType)); 
        }

        public static Faker<Task> WithTypeRef(this Faker<Task> self,EntityReference typeRef)
        {
            return self.RuleFor(x => x.ipg_tasktypeid, x => typeRef);
        }

        public static Faker<Task> WithRegarding(this Faker<Task> self, EntityReference regardingRef)
        {
            return self.RuleFor(x => x.RegardingObjectId, x => regardingRef);
        }

        public static Faker<Task> WithTaskCategoryRef(this Faker<Task> self, EntityReference taskcategory)
        {
            return self.RuleFor(x => x.ipg_taskcategoryid, x => taskcategory);
        }

        public static Faker<Task> WithDuplicatePatient(this Faker<Task> self, Contact contact)
        {
            return self.RuleFor(x => x.ipg_DuplicatePatientId, x => contact.ToEntityReference());
        }

        public static Faker<Task> WithCase(this Faker<Task> self, EntityReference caseRef)
        {
            return self.RuleFor(x => x.ipg_caseid, x => caseRef);
        }

        public static Faker<Task> WithTaskCategory(this Faker<Task> self, ipg_Taskcategory1 taskCategory = ipg_Taskcategory1.User)
        {
            return self.RuleFor(x => x.ipg_taskcategorycodeEnum, x => taskCategory);
        }

        public static Faker<Task> WithAssignedToTeam(this Faker<Task> self, Team team)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (team == null) throw new ArgumentNullException(nameof(team));

            self.RuleFor(x => x.ipg_assignedtoteamid, x => team.ToEntityReference());
            return self;
        }

        public static Faker<Task> WithReferral(this Faker<Task> self, EntityReference referralRef)
        {
            return self.RuleFor(x => x.RegardingObjectId, x => referralRef);
        }

        public static Faker<Task> WithSlaType(this Faker<Task> self, ipg_SLATaskType slaType)
        {
            return self.RuleFor(x => x.ipg_slatypecode, x => slaType.ToOptionSetValue());
        }

        public static Faker<Task> WithStatus(this Faker<Task> self, Task_StatusCode status)
        {
            return self.RuleFor(x => x.StatusCode, x => status.ToOptionSetValue());
        }

        public static Faker<Task> WithState(this Faker<Task> self, TaskState state)
        {
            return self.RuleFor(x => x.StateCode, x => state);
        }

        public static Faker<Task> WithScheduledStart(this Faker<Task> self, DateTime scheduledStart)
        {
            return self.RuleFor(x => x.ScheduledStart, x => scheduledStart);
        }

        public static Faker<Task> WithScheduledEnd(this Faker<Task> self, DateTime scheduledEnd)
        {
            return self.RuleFor(x => x.ScheduledEnd, x => scheduledEnd);
        }

        public static Faker<Task> WithTaskCategory(this Faker<Task> self, EntityReference taskCategoryRef)
        {
            return self.RuleFor(x => x.ipg_taskcategoryid, x => taskCategoryRef);
        }

        public static Faker<Task> WithDocumentType(this Faker<Task> self, EntityReference docTypeRef)
        {
            return self.RuleFor(x => x.ipg_DocumentType, x => docTypeRef);
        }

        public static Faker<Task> WithCarrier(this Faker<Task> self, EntityReference carrierRef)
        {
            return self.RuleFor(x => x.ipg_carrierid, x => carrierRef);
        }

        public static Faker<Task> WithSubject(this Faker<Task> self, string subject)
        {
            return self.RuleFor(x => x.Subject, x => subject);
        }

        public static Faker<Task> WithOwner(this Faker<Task> self, EntityReference ownerRef)
        {
            return self.RuleFor(x => x.OwnerId, x => ownerRef);
        }
        public static Faker<Task> WithLevel(this Faker<Task> self, int level)
        {
            return self.RuleFor(x => x.ipg_level, x => level);
        }

        public static Faker<Task> WithMetaTag(this Faker<Task> self, string metaTag)
        {
            return self.RuleFor(x => x.ipg_metatag, x => metaTag);
        }

        public static Faker<Task> WithDescription(this Faker<Task> self, string description)
        {
            return self.RuleFor(x => x.Description, x => description);
        }
    }
}
