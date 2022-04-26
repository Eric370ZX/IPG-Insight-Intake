using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Contact
{
    public class CreateTaskForPhysicianPlugin : PluginBase
    {
        IOrganizationService CrmService { get; set; }
        ITracingService TracingService { get; set; }
        private const string MAINTENANCE_ADMIN_ID = "fc81028a-9330-ea11-a810-000d3a33f42d";
        public CreateTaskForPhysicianPlugin() : base(typeof(CreateTaskForPhysicianPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Intake.Contact.EntityLogicalName, PostOperationCreateHandler);
        }
        private void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            var contact = localPluginContext.Target<Intake.Contact>();
            CrmService = localPluginContext.OrganizationService;
            TracingService = localPluginContext.TracingService;
            var taskManager = new TaskManager(CrmService, TracingService);
            if (IsContactPhysician(contact.ipg_ContactTypeId) && contact.ipg_physicianrequestedbyid != null && !(contact.ipg_approved.HasValue && contact.ipg_approved.Value))
            {
                CreateTask(contact, taskManager);
            }
        }

        private void CreateTask(Intake.Contact contact, TaskManager taskManager)
        {
            string facilityName = "";
            if (contact.ipg_currentfacilityid != null)
            {
                facilityName = contact.ipg_currentfacilityid?.Name;
            }
            var taskType = taskManager.GetTaskTypeById(TaskManager.TaskTypeIds.NEW_PHYSICIAN_REQUEST);
            var task = new Task();
            var descriptionParams = new[] { contact?.FullName, facilityName };
            taskManager.FillTaskByTaskType(task, taskType, descriptionParams);
            CrmService.Create(task);
        }

        private Incident RetrieveCaseByContact(Intake.Contact contact, ColumnSet columns)
        {
            var query = new QueryExpression
            {
                EntityName = Incident.EntityLogicalName,
                ColumnSet = columns,
                TopCount = 1,
                Criteria = {
                    Conditions = {
                        new ConditionExpression(Incident.Fields.ipg_PhysicianId, ConditionOperator.Equal, contact.Id)
                    }
                },
                Orders = {
                    new OrderExpression(Incident.Fields.CreatedOn, OrderType.Descending),
                }
            };
            var incidentsList = CrmService.RetrieveMultiple(query).Entities;
            return incidentsList.FirstOrDefault()?.ToEntity<Incident>();
        }

        private bool IsContactPhysician(EntityReference contactTypeRef)
        {
            var physitianTypeName = "Physician";
            if (contactTypeRef == null)
            {
                return false;
            }
            var query = new QueryExpression
            {
                EntityName = ipg_contacttype.EntityLogicalName,
                ColumnSet = new ColumnSet(ipg_contacttype.PrimaryIdAttribute),
                Criteria = {
                    Conditions = {
                        new ConditionExpression(ipg_contacttype.Fields.ipg_contacttypeId, ConditionOperator.Equal, contactTypeRef.Id),
                        new ConditionExpression(ipg_contacttype.Fields.StateCode, ConditionOperator.Equal, (int)ipg_contacttypeState.Active),
                        new ConditionExpression(ipg_contacttype.Fields.ipg_name, ConditionOperator.Equal, physitianTypeName),
                    }
                }
            };
            return CrmService.RetrieveMultiple(query).Entities.Any();
        }

        private Intake.Contact RetrievePhysicianById(EntityReference physitianRef)
        {
            if (physitianRef == null)
            {
                return null;
            }
            var contact = CrmService.Retrieve(Intake.Contact.EntityLogicalName, physitianRef.Id, new ColumnSet(true)).ToEntity<Intake.Contact>();
            return contact;
        }
    }
}