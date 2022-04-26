using Insight.Intake.Extensions;
using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace Insight.Intake.Plugin.Contact
{
    public class PotentialDuplicatePatient : PluginBase
    {
        const string PATIENT_CONTACT_TYPE = "Patient";
        public PotentialDuplicatePatient() : base(typeof(CreateTaskForFacilityUserPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Intake.Contact.EntityLogicalName, PostOperationUpdateHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Intake.Contact.EntityLogicalName, PostOperationUpdateHandler);
        }

        private void PostOperationUpdateHandler(LocalPluginContext localContext)
        {
            var contactEntity = localContext.Target<Intake.Contact>();

            if (localContext.PluginExecutionContext.MessageName == MessageNames.Update)
            {
                contactEntity = localContext.OrganizationService.Retrieve(Intake.Contact.EntityLogicalName, contactEntity.Id, new ColumnSet(
                    Intake.Contact.Fields.FirstName,
                    Intake.Contact.Fields.LastName,
                    Intake.Contact.Fields.FullName,
                    Intake.Contact.Fields.BirthDate,
                    Intake.Contact.Fields.ipg_ContactTypeId
                    )).ToEntity<Intake.Contact>();
            }

            if (contactEntity.ipg_ContactTypeId == null) return;

            var contactType = localContext.
              OrganizationService.
              Retrieve(ipg_contacttype.EntityLogicalName,
                       contactEntity.ipg_ContactTypeId.Id,
                       new ColumnSet(ipg_contacttype.Fields.ipg_name)).ToEntity<ipg_contacttype>();

            if (contactType.ipg_name != PATIENT_CONTACT_TYPE) return;

            var queryExpression = new QueryExpression(Intake.Contact.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(
                                            Intake.Contact.Fields.FirstName,
                                            Intake.Contact.Fields.LastName,
                                            Intake.Contact.Fields.FullName,
                                            Intake.Contact.Fields.BirthDate),
                LinkEntities = { new LinkEntity(Intake.Contact.EntityLogicalName,ipg_contacttype.EntityLogicalName
                                            , Intake.Contact.Fields.ipg_ContactTypeId, ipg_contacttype.Fields.ipg_contacttypeId, JoinOperator.Inner) },
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(Intake.Contact.EntityLogicalName,Intake.Contact.Fields.BirthDate, ConditionOperator.Equal, contactEntity.BirthDate),
                            new ConditionExpression(Intake.Contact.EntityLogicalName,Intake.Contact.Fields.ContactId, ConditionOperator.NotEqual, contactEntity.Id),
                            new ConditionExpression(ipg_contacttype.EntityLogicalName, ipg_contacttype.Fields.ipg_name, ConditionOperator.Equal, PATIENT_CONTACT_TYPE)
                        }
                }
            };

            EntityCollection patientWithSameDOB = localContext.OrganizationService.RetrieveMultiple(queryExpression);

            EntityCollection possibleDuplicatePatient = new EntityCollection();

            foreach (var patient in patientWithSameDOB.Entities)
            {
                var item = patient.ToEntity<Intake.Contact>();

                string sourceFirstName = item.FirstName.ToLower();
                string destinationFirstName = contactEntity.FirstName?.ToLower();

                string sourceLastName = item.LastName.ToLower();
                string destinationLastName = contactEntity.LastName?.ToLower();

                //Levenstein comparison for this patient name and any other patient name in the system with same DOB.
                if (sourceFirstName.ComputeLevenshteinDistance(destinationFirstName) < 3 && sourceLastName.ComputeLevenshteinDistance(destinationLastName) < 3)
                {
                    possibleDuplicatePatient.Entities.Add(item);
                }
            }

            if (possibleDuplicatePatient.Entities.Any())
            {
                var taskManager = new TaskManager(localContext.OrganizationService, localContext.TracingService, null, localContext.PluginExecutionContext.UserId);

                var relatedTask = taskManager.GetRelatedTask(contactEntity.ToEntityReference(), Constants.TaskTypeNames.PotentialDuplicatePatient);

                                //Fetch Potential duplicate patient task type.
                var taskType = taskManager.GetTaskTypeById(TaskManager.TaskTypeIds.DUPLICATE_PATIENT);
                if (taskType == null)
                    throw new InvalidPluginExecutionException($"Unable to find a task type with name {Constants.TaskTypeNames.PotentialDuplicatePatient}");

                string commaseparatedDupPatient = string.Join(",", possibleDuplicatePatient.Entities.Select(x => x.ToEntity<Intake.Contact>().FullName + "(" + x.Id + ")"));

                //Check if the task has already been created.
                if (relatedTask == null)
                {
                    var task = new Task();
                    task.Description = $"Patient {contactEntity.FullName} may be a duplicate of {commaseparatedDupPatient}. " +
                        "Please review the Patient list to determine if these are true duplicates. Document your findings in the Task Note.";
                    localContext.TracingService.Trace("Creating task");
                    taskManager.CreateTask(contactEntity.ToEntityReference(), TaskManager.TaskTypeIds.DUPLICATE_PATIENT, task);
                    localContext.TracingService.Trace("Task Created");
                }
             }
        }
    }
}