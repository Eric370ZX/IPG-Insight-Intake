using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;
using System.Collections.Generic;
using Insight.Intake.Plugin.Managers;
using static Insight.Intake.Plugin.Managers.TaskManager;
using System.Text.RegularExpressions;

namespace Insight.Intake.Plugin.Case
{
    /// <summary>
    /// Purpose of this plugin is to validate missing information on the case.
    /// </summary>
    public class ValidateMissingInformation : PluginBase
    {
        private readonly string[] incidentColumnSet =
        {
            Incident.Fields.ipg_PatientLastName,
            Incident.Fields.ipg_PatientFirstName,
            Incident.Fields.ipg_PatientDateofBirth,
            Incident.Fields.ipg_PatientAddress,
            Incident.Fields.ipg_CasePatientZipCodeId,  
            Incident.Fields.ipg_DxCodeId1,
            Incident.Fields.ipg_CPTCodeId1,
            Incident.Fields.ipg_PhysicianId,
            Incident.Fields.ipg_SurgeryDate
        };


        public ValidateMissingInformation():base(typeof(ValidateMissingInformation))
        {
            RegisterEvent(PipelineStages.PreValidation, new ipg_IPGIntakeActionValidateMissingInformationRequest().RequestName, Incident.EntityLogicalName, preValidationHandler);
        }

        private void preValidationHandler(LocalPluginContext obj)
        {
            ITracingService tracing = obj.TracingService;
            IOrganizationService service = obj.OrganizationService;
            EntityReference targetRef = obj.TargetRef();
            TaskManager taskmgr = new TaskManager(service, tracing);
            
            tracing.Trace($"Target:{targetRef?.Id}");

            var incident = service.Retrieve(targetRef.LogicalName, targetRef.Id,
                                                    new ColumnSet(incidentColumnSet.Concat(new string[] { Incident.Fields.OwnerId }).ToArray())).ToEntity<Incident>();
            tracing.Trace("Get Missing Information Fields from metadata");

            List<string> missingInfoFields = GetMissingInformationFields(service, tracing, incident);

            tracing.Trace("Cancel Open Task Patient Information");
            taskmgr.CancelTasks(incident.ToEntityReference(), "", TaskTypeIds.REQUEST_PATIENT_INFORMATION);
            tracing.Trace("DeleteAllRequiredInformationForFieldsOnCase");
            DeleteAllRequiredInformationForFieldsOnCase(service, targetRef);

            if (missingInfoFields.Any())
            {
                tracing.Trace("CreateTaskForMissingInformation");
                CreateTaskForMissingInformation(taskmgr, missingInfoFields, incident);
                tracing.Trace("CreateRequiredInformationOnCase");
                foreach (string field in missingInfoFields)
                {
                    CreateRequiredInformationOnCase(service, tracing, targetRef, field);
                }
            }
        }

        //Remove previously created fields
        internal void DeleteAllRequiredInformationForFieldsOnCase(IOrganizationService service, EntityReference incident)
        {
            var requiredInformationQuery = new QueryExpression
            {
                EntityName = ipg_requiredinformation.EntityLogicalName,
                ColumnSet = new ColumnSet(false),
                Criteria = new FilterExpression(LogicalOperator.And),
                PageInfo = new PagingInfo
                {
                    ReturnTotalRecordCount = true
                },
            };

            requiredInformationQuery.Criteria.AddCondition(ipg_requiredinformation.Fields.ipg_RequiredFieldName, ConditionOperator.NotNull);
            requiredInformationQuery.Criteria.AddCondition(ipg_requiredinformation.Fields.ipg_CaseId, ConditionOperator.Equal, incident.Id);
            
            var currentRequiredDocuments = service.RetrieveMultiple(requiredInformationQuery);
            if (currentRequiredDocuments != null)
            {
                foreach (var entity in currentRequiredDocuments?.Entities)
                {
                    service.Delete(ipg_requiredinformation.EntityLogicalName, entity.Id);
                }
            }
        }

        /// <summary>
        /// If passed the names of fields which are missing from the case, this method will create a task and assign it to the user
        /// to update case and provide missing information.
        /// </summary>
        /// <param name="missingInfoFields"></param>
        private void CreateTaskForMissingInformation(TaskManager taskmgr, List<string> missingInfoFields, Incident incident)
        {
            var taskType = taskmgr.GetTaskTypeById(TaskTypeIds.REQUEST_PATIENT_INFORMATION);
            var description = Regex.Replace(taskType.ipg_description ?? "<>", "<.*?>", string.Join(",", missingInfoFields));

            taskmgr.CreateTask(incident.ToEntityReference(), new Task()
            {
                ipg_tasktypeid = taskType.ToEntityReference(),
                Description = description
            });
        }

        /// <summary>
        /// Create Required information record for case based on the missing field
        /// </summary>
        /// <param name="incident"></param>
        /// <param name="fieldName"></param>
        private void CreateRequiredInformationOnCase(IOrganizationService service, ITracingService tracing, EntityReference incident, string fieldName)
        {
            // Create a task activity to follow up with the account customer in 7 days. 
            ipg_requiredinformation requiredinformation = new ipg_requiredinformation();
            requiredinformation.ipg_name = fieldName;
            requiredinformation.ipg_CaseId = new EntityReference(Incident.EntityLogicalName, incident.Id);
            requiredinformation.ipg_RequiredFieldName = fieldName;

            // Create the task in Microsoft Dynamics CRM.
            tracing.Trace(string.Format("ValidateMissingInformation: Creating Required Information {0} on Case {1}.", fieldName, incident.Name));
            service.Create(requiredinformation);
        }

        /// <summary>
        /// Retrieve missing information fields on a case.
        /// </summary>
        /// <returns></returns>
        private List<string> GetMissingInformationFields(IOrganizationService service, ITracingService trace, Incident incident)
        {
            trace.Trace("Get Metadata");
            var incidentMetaData = service.GetEntityMetadata(Incident.EntityLogicalName);

            List<string> missingInfoFields = new List<string>();
            foreach (var columnName in incidentColumnSet)
            {
                trace.Trace($"Get {columnName} from Case");
                var value = incident.GetAttributeValue<object>(columnName);
                trace.Trace($"Value == null {value == null}");

                if (value == null)
                {
                    var columnMetaData = incidentMetaData.Attributes.Where(x => x.LogicalName == columnName).FirstOrDefault();
                    trace.Trace($"columnMetaData == null {columnMetaData == null}");
                    var displayName = columnMetaData?.DisplayName?.UserLocalizedLabel?.Label ?? columnName;
                    trace.Trace($"displayName {displayName}");
                    missingInfoFields.Add(displayName);
                }
            }

            return missingInfoFields;
        }
    }
}