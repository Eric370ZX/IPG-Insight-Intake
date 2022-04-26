using Insight.Intake.Extensions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Gating.Common
{
    public class TaskManager
    {
        List<Task> _existingTasks;
        private EntityReference _caseRef;
        private EntityReference _referralRef;
        IOrganizationService _service;
        OrganizationServiceContext _crmContext;

        public TaskManager(EntityReference caseRef, EntityReference referralRef, IOrganizationService service)
        {
            this._caseRef = caseRef;
            this._referralRef = referralRef;
            this._service = service;
            _crmContext = new OrganizationServiceContext(_service);

        }
        private bool TaskAlreadyExistV3(ipg_taskconfiguration taskConfiguration, EntityReference caseRef)
        {
            if(taskConfiguration == null || taskConfiguration.ipg_tasktypeid == null)
            {
                return false;
            }
            if (_existingTasks == null)
            {
                if (caseRef != null)
                {
                    _existingTasks = (from task in _crmContext.CreateQuery<Task>()
                                      where task.RegardingObjectId.Id == caseRef.Id
                                      && task.StateCode == (int)TaskState.Open
                                      select task).ToList();
                }
                else
                {
                    _existingTasks = new List<Task>();
                }
            }
            return _existingTasks
                .Any(task => task.ipg_tasktypeid != null && task.ipg_tasktypeid.Id == taskConfiguration?.ipg_tasktypeid?.Id);
        }

        public EntityCollection GetActiveTaskConfiguration(Guid gateConfigurationDetailId)
        {
            var query = new QueryExpression()
            {
                EntityName = ipg_taskconfiguration.EntityLogicalName,
                ColumnSet = new ColumnSet(ipg_taskconfiguration.Fields.ipg_subject,
                                          ipg_taskconfiguration.Fields.ipg_duedate,
                                          ipg_taskconfiguration.Fields.ipg_startdate,
                                          ipg_taskconfiguration.Fields.ipg_priority,
                                          ipg_taskconfiguration.Fields.ipg_reassigntoteamid,
                                          ipg_taskconfiguration.Fields.ipg_createif,
                                          ipg_taskconfiguration.Fields.ipg_tasktypecode,
                                          ipg_taskconfiguration.Fields.ipg_gatingoutcomeid,
                                          ipg_taskconfiguration.Fields.ipg_taskcategoryid,
                                          ipg_taskconfiguration.Fields.ipg_tasktypeid)
            };
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("ipg_gateconfigurationdetailid", ConditionOperator.Equal, gateConfigurationDetailId);

            return _service.RetrieveMultiple(query);
        }

        public List<Task> GetOpenTasks(Entity taskConfiguration, EntityReference caseRef)
        {
            List<Task> openTasks = new List<Task>();
            if (taskConfiguration.Contains("ipg_tasktypecode") && (caseRef != null))
            {
                openTasks = (from task in _crmContext.CreateQuery<Task>()
                             where task.RegardingObjectId.Id == caseRef.Id &&
                                 task.StateCode == (int)TaskState.Open &&
                                 task.ipg_tasktypecode != null &&
                                 task.ipg_tasktypecode.Value == taskConfiguration.GetAttributeValue<OptionSetValue>("ipg_tasktypecode").Value
                             select task).ToList();
            }
            return openTasks;
        }

        public IEnumerable<Task> GetOpenTasksV3(EntityReference taskTypeRef, EntityReference caseRef, EntityReference workflowtaskRef)
        {
            if (taskTypeRef != null && caseRef != null && workflowtaskRef != null)
            {
                return (from task in _crmContext.CreateQuery<Task>()
                        where task.RegardingObjectId.Id == caseRef.Id
                            && task.StateCode == (int)TaskState.Open
                            && task.ipg_WorkflowTaskId != null
                            && task.ipg_WorkflowTaskId.Id == workflowtaskRef.Id
                            && task.ipg_tasktypeid != null
                            && task.ipg_tasktypeid.Id == taskTypeRef.Id
                        select task);
            }
            return Enumerable.Empty<Task>();
        }

        private DateTime? GetCaseDos(EntityReference targetRef)
        {
            return _service.Retrieve(targetRef.LogicalName, targetRef.Id,
                        new ColumnSet("ipg_actualdos", "ipg_surgerydate")).GetCaseDos();
        }

        private string GetInactiveHcpcsCode(DateTime? caseDos)
        {
            var actualPartsQuery = new QueryExpression(ipg_casepartdetail.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(ipg_casepartdetail.Fields.ipg_hcpcscode),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression("ipg_caseid", ConditionOperator.Equal, _caseRef.Id)
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(ipg_casepartdetail.EntityLogicalName, ipg_masterhcpcs.EntityLogicalName,
                    "ipg_hcpcscode", "ipg_masterhcpcsid", JoinOperator.Inner)
                    {
                        EntityAlias = "m",
                        LinkCriteria =
                        {
                            FilterOperator = LogicalOperator.Or,
                            Conditions =
                            {
                                new ConditionExpression("ipg_effectivedate", ConditionOperator.GreaterThan, caseDos.Value),
                                new ConditionExpression("ipg_expirationdate", ConditionOperator.LessThan, caseDos.Value)
                            }
                        }
                    }
                }
            };
            var actualPart = _service.RetrieveMultiple(actualPartsQuery).Entities.FirstOrDefault();
            if (actualPart != null && actualPart[ipg_casepartdetail.Fields.ipg_hcpcscode] != null)
            {
                return ((EntityReference)actualPart[ipg_casepartdetail.Fields.ipg_hcpcscode]).Name
                    ?? _service.Retrieve(ipg_masterhcpcs.EntityLogicalName,
                                         ((EntityReference)actualPart[ipg_casepartdetail.Fields.ipg_hcpcscode]).Id,
                                         new ColumnSet(ipg_masterhcpcs.Fields.ipg_name))
                                .ToEntity<ipg_masterhcpcs>()
                                .ipg_name;
            }
            return string.Empty;
        }

        private string GetInvalidCptCode(EntityReference targetRef, DateTime? caseDos)
        {
            var targetCptCodes = GetCptCodeIds(targetRef);

            var query = new QueryExpression()
            {
                EntityName = ipg_cptcode.EntityLogicalName,
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Filters = {
                    new FilterExpression {
                        Conditions = {
                            new ConditionExpression("ipg_cptcodeid", ConditionOperator.In, targetCptCodes)
                        }
                    },
                    new FilterExpression{
                        FilterOperator = LogicalOperator.Or,
                        Conditions = {
                            new ConditionExpression("ipg_expirationdate", ConditionOperator.LessThan, caseDos),
                            new ConditionExpression("ipg_effectivedate", ConditionOperator.GreaterThan, caseDos),
                        }
                    }
                }
                }
            };

            var invalidCptCode = _service.RetrieveMultiple(query).Entities.FirstOrDefault().ToEntity<ipg_cptcode>().ipg_name;

            return invalidCptCode;
        }

        private List<Guid> GetCptCodeIds(EntityReference targetRef)
        {
            var cptCodes = new List<Guid>();
            var cptAttributesNames = new List<string>() { "ipg_cptcodeid1", "ipg_cptcodeid2", "ipg_cptcodeid3", "ipg_cptcodeid4", "ipg_cptcodeid5", "ipg_cptcodeid6" };
            var incident = _service.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet(cptAttributesNames.ToArray()));
            foreach (var attributeName in cptAttributesNames)
            {
                if (incident.Contains(attributeName) && incident.GetAttributeValue<EntityReference>(attributeName) != null)
                {
                    cptCodes.Add(incident.GetAttributeValue<EntityReference>(attributeName).Id);
                }
            }

            return cptCodes;
        }

        private void GetCasePhysicianAndFacility(EntityReference targetRef, out string physicianName, out string facilityName)
        {
            physicianName = "";
            facilityName = "";
            var incident = _service.Retrieve(targetRef.LogicalName,
                                            targetRef.Id,
                                            new ColumnSet(Incident.Fields.ipg_PhysicianId,
                                                        Incident.Fields.ipg_FacilityId));
            if (incident.GetAttributeValue<EntityReference>(Incident.Fields.ipg_PhysicianId) != null)
            {
                physicianName = incident.GetAttributeValue<EntityReference>(Incident.Fields.ipg_PhysicianId).Name ??
                    _service.Retrieve(Intake.Contact.EntityLogicalName,
                    incident.GetAttributeValue<EntityReference>(Incident.Fields.ipg_PhysicianId).Id,
                    new ColumnSet(Intake.Contact.Fields.FullName))
                    .ToEntity<Intake.Contact>()
                    .FullName;
            }
            if (incident.GetAttributeValue<EntityReference>(Incident.Fields.ipg_FacilityId) != null)
            {
                facilityName = incident.GetAttributeValue<EntityReference>(Incident.Fields.ipg_FacilityId).Name ??
                    _service.Retrieve(Intake.Account.EntityLogicalName,
                    incident.GetAttributeValue<EntityReference>(Incident.Fields.ipg_FacilityId).Id,
                    new ColumnSet(Intake.Account.Fields.Name))
                    .ToEntity<Intake.Account>()
                    .Name;
            }
        }

        private List<string> GetInvalidDxCodes(EntityReference targetRef)
        {
            var dxCodeAttributes = new List<string>() { Incident.Fields.ipg_DxCodeId1, Incident.Fields.ipg_DxCodeId2, Incident.Fields.ipg_DxCodeId3,
                                                        Incident.Fields.ipg_DxCodeId4, Incident.Fields.ipg_DxCodeId5, Incident.Fields.ipg_DxCodeId6};
            var caseColumns = targetRef.LogicalName == Incident.EntityLogicalName ? new List<string>() { Incident.Fields.ipg_PatientGender } :
                                                                                    new List<string>() { ipg_referral.Fields.ipg_gender };
            caseColumns.AddRange(dxCodeAttributes);
            var incident = _service.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet(caseColumns.ToArray()));
            var patientGender = targetRef.LogicalName == Incident.EntityLogicalName ? incident.ToEntity<Incident>().ipg_PatientGender.Value :
                                                                                      incident.ToEntity<ipg_referral>().ipg_gender.Value;
            var dxCodeIds = new List<Guid>();
            foreach (var attributeName in dxCodeAttributes)
            {
                if (incident.Contains(attributeName) && incident.GetAttributeValue<EntityReference>(attributeName) != null)
                {
                    dxCodeIds.Add(incident.GetAttributeValue<EntityReference>(attributeName).Id);
                }
            }
            var dxCodes = _service.RetrieveMultiple(new QueryExpression(ipg_dxcode.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(ipg_dxcode.Fields.ipg_name, ipg_dxcode.Fields.ipg_gender),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_dxcode.Fields.Id, ConditionOperator.In, dxCodeIds)
                    }
                }
            });
            return dxCodes.Entities
                .Where(dx => dx.ToEntity<ipg_dxcode>().ipg_gender.Value != patientGender)
                .Select(dx => dx.GetAttributeValue<string>(ipg_dxcode.Fields.ipg_name))
                .ToList();
        }

        public void CreateGatingTasksV3(EntityCollection tasksToCreate, bool succeeded, EntityReference workflowTaskRef, GatingResponse gatingResponse, ITracingService _tracingService = null)
        {
            if (workflowTaskRef == null)
            {
                return;
            }

            EntityReference taskConfigurationRef = null;
            if (gatingResponse == null || gatingResponse.CodeOutput == 0)
            {
                taskConfigurationRef = (from wtoc in _crmContext.CreateQuery<ipg_workflowtaskoutputconfig>()
                                         join wt in _crmContext.CreateQuery<ipg_workflowtask>() on wtoc.ipg_WorkflowTaskId.Id equals wt.ipg_workflowtaskId
                                         where wt.ipg_workflowtaskId == workflowTaskRef.Id
                                         && wtoc.ipg_OutcomeType == succeeded
                                         select wtoc.ipg_TaskConfigurationId).FirstOrDefault();
            }
            else
            {
                taskConfigurationRef = (from wtoc in _crmContext.CreateQuery<ipg_workflowtaskoutputconfig>()
                                         join wt in _crmContext.CreateQuery<ipg_workflowtask>() on wtoc.ipg_WorkflowTaskId.Id equals wt.ipg_workflowtaskId
                                         where wt.ipg_workflowtaskId == workflowTaskRef.Id
                                         && wtoc.ipg_CodeOutput == gatingResponse.CodeOutput
                                         && wtoc.ipg_OutcomeType == succeeded
                                         select wtoc.ipg_TaskConfigurationId).FirstOrDefault();
            }

            if (taskConfigurationRef == null)
            {
                return;
            }

            var taskConfiguration = _service.Retrieve(taskConfigurationRef.LogicalName, taskConfigurationRef.Id, new ColumnSet(true)).ToEntity<ipg_taskconfiguration>();

            var workflowTask = _service.Retrieve(workflowTaskRef.LogicalName, workflowTaskRef.Id, new ColumnSet(ipg_workflowtask.Fields.ipg_WFTaskGroupId)).ToEntity<ipg_workflowtask>();
            var wfTaskGroupRef = workflowTask.ipg_WFTaskGroupId;
            var reference = _caseRef == null ? _referralRef : _caseRef;
            if (wfTaskGroupRef != null)
            {
                var wfTaskGroup = _service.Retrieve(wfTaskGroupRef.LogicalName, wfTaskGroupRef.Id, new ColumnSet(true));
                var taskWithGroup = tasksToCreate.Entities.Where(x => ((Task)x).ipg_wftaskgroupid != null && ((Task)x).ipg_wftaskgroupid.Id == wfTaskGroupRef.Id).FirstOrDefault()?.ToEntity<Task>();
                if (taskWithGroup != null)
                {
                    if(taskConfiguration.ipg_tasktypeid != null)
                    {
                        var taskType = _service.Retrieve(taskConfiguration.ipg_tasktypeid.LogicalName, taskConfiguration.ipg_tasktypeid.Id, new ColumnSet(ipg_tasktype.Fields.ipg_description)).ToEntity<ipg_tasktype>();
                        taskWithGroup.Description += (taskType.ipg_description ?? string.Empty) + "\r\n";
                    }
                }
                else
                {
                    if (!TaskAlreadyExistV3(taskConfiguration, reference))
                    {
                        tasksToCreate.Entities.Add(CreateTaskV3(reference, taskConfiguration, gatingResponse, _tracingService, null, workflowTaskRef, wfTaskGroup));
                    }
                }
            }
            else
            {
                if (!TaskAlreadyExistV3(taskConfiguration, reference))
                {
                    tasksToCreate.Entities.Add(CreateTaskV3(reference, taskConfiguration, gatingResponse, _tracingService, null, workflowTaskRef));
                }
            }
        }

        public void CreateGatingTasksV3(EntityCollection tasksToCreate, bool succeeded, ipg_gateconfigurationdetail gcd, GatingResponse gatingResponse, ITracingService _tracingService = null)
        {
            if (gcd == null)
            {
                return;
            }

            EntityReference taskConfiguration = null;
            if (gatingResponse == null || gatingResponse.CodeOutput == 0)
            {
                taskConfiguration = (from wtoc in _crmContext.CreateQuery<ipg_workflowtaskoutputconfig>()
                                     where wtoc.ipg_WorkflowTaskId.Id == gcd.ipg_WorkflowTaskId.Id
                                     && wtoc.ipg_OutcomeType == succeeded
                                     select wtoc.ipg_TaskConfigurationId).FirstOrDefault();
            }
            else
            {
                taskConfiguration = (from wtoc in _crmContext.CreateQuery<ipg_workflowtaskoutputconfig>()
                                     where wtoc.ipg_WorkflowTaskId.Id == gcd.ipg_WorkflowTaskId.Id
                                     && wtoc.ipg_CodeOutput == gatingResponse.CodeOutput
                                     && wtoc.ipg_OutcomeType == succeeded
                                     select wtoc.ipg_TaskConfigurationId).FirstOrDefault();
            }

            if (taskConfiguration == null)
            {
                return;
            }

            var taskConfigurationEntity = _service.Retrieve(taskConfiguration.LogicalName, taskConfiguration.Id, new ColumnSet(true)).ToEntity<ipg_taskconfiguration>();
            var reference = _caseRef == null ? _referralRef : _caseRef;
            if (!TaskAlreadyExistV3(taskConfigurationEntity, reference))
            {
                tasksToCreate.Entities.Add(CreateTaskV3(reference, taskConfigurationEntity, gatingResponse, _tracingService, null, gcd.ipg_WorkflowTaskId));
            }
        }

        private Entity CreateTaskV3(EntityReference targetRef, Entity taskConfiguration, GatingResponse gatingResponse, ITracingService _tracingService, ipg_caseworkflowtask cwt, EntityReference workflowTaskRef = null, Entity wfTaskGroup = null)
        {
            if (targetRef == null)
            {
                targetRef = _referralRef;
            }

            var task = new Task();
            task.RegardingObjectId = targetRef;
            if (taskConfiguration != null && taskConfiguration.Contains(ipg_taskconfiguration.Fields.ipg_taskcategoryid))
            {
                task[Task.Fields.ipg_taskcategoryid] = taskConfiguration.GetAttributeValue<EntityReference>(ipg_taskconfiguration.Fields.ipg_taskcategoryid);
            }
            else
            {
                task[Task.Fields.ipg_taskcategoryid] = new EntityReference(ipg_taskcategory.EntityLogicalName, Helpers.Constants.TaskCategory.Administrative);
            }
            if (taskConfiguration != null && taskConfiguration.Contains(ipg_taskconfiguration.Fields.ipg_tasktypeid))
            {
                var taskTypeRef = taskConfiguration.GetAttributeValue<EntityReference>(ipg_taskconfiguration.Fields.ipg_tasktypeid);
                task.ipg_tasktypeid = taskTypeRef;
                var taskType = _service.Retrieve(ipg_tasktype.EntityLogicalName, taskTypeRef.Id, new ColumnSet(ipg_tasktype.Fields.ipg_name, ipg_tasktype.Fields.ipg_description, ipg_tasktype.Fields.ipg_priority)).ToEntity<ipg_tasktype>();
                if (!string.IsNullOrEmpty(gatingResponse?.TaskDescripton))
                {
                    task.Description = gatingResponse?.TaskDescripton;
                }
                else
                {
                    task.Description = taskType.ipg_description;
                }
                UpdateTaskDescription(taskType, task, targetRef);
                task.ipg_priority = taskType.ipg_priority;
            }
            else
            {
                task[Task.Fields.Description] = gatingResponse?.TaskDescripton;
                if (taskConfiguration != null && taskConfiguration.Contains("ipg_subject"))
                {
                    task.Subject = string.IsNullOrEmpty(gatingResponse?.TaskSubject)
                        ? taskConfiguration?.GetAttributeValue<string>("ipg_subject")
                        : gatingResponse?.TaskSubject;
                }
            }
            if (taskConfiguration != null && taskConfiguration.Contains("ipg_tasktypecode"))
            {
                task.ipg_tasktypecode= taskConfiguration.GetAttributeValue<OptionSetValue>("ipg_tasktypecode");
            }
            if (taskConfiguration?.Contains("ipg_gatingoutcomeid") ?? false)
            {
                task.ipg_gatingoutcomeid = taskConfiguration.GetAttributeValue<EntityReference>("ipg_gatingoutcomeid");
            }

            DateTime now = DateTime.UtcNow;
            task[Task.Fields.ScheduledStart] = now.AddDays(Convert.ToDouble(taskConfiguration.GetAttributeValue<int?>("ipg_startdate") ?? 0));

            if (taskConfiguration != null && taskConfiguration.Contains("ipg_duedate"))
            {
                task.ScheduledEnd= now.AddDays(Convert.ToDouble(taskConfiguration.GetAttributeValue<int>("ipg_duedate")));
            }
            if (taskConfiguration != null && taskConfiguration.GetAttributeValue<EntityReference>("ipg_reassigntoteamid") != null)
            {
                task.OwnerId = taskConfiguration.GetAttributeValue<EntityReference>("ipg_reassigntoteamid");
            }

            task.ipg_taskcategorycode = new OptionSetValue((int)ipg_Taskcategory1.System);

            task[Task.Fields.ipg_generatedbycode] = new OptionSetValue((int)ipg_GeneratedByCode.System);

            if (gatingResponse != null && !string.IsNullOrWhiteSpace(gatingResponse.CrmReason))
            {
                task.Description = (string.IsNullOrWhiteSpace(task.Description) ? string.Empty : task.Description + Environment.NewLine) + gatingResponse.CrmReason;
            }
            task.ipg_WorkflowTaskId = (cwt == null ? (workflowTaskRef == null ? null : workflowTaskRef) : cwt.ipg_WorkflowTaskId);
            task.ipg_wftaskgroupid = wfTaskGroup == null ? null : wfTaskGroup.ToEntityReference();

            return task;
        }

        private string GetOpenHighPriorityTasksNames(EntityReference targetRef)
        {
            var query = new QueryExpression
            {
                EntityName = Task.EntityLogicalName,
                ColumnSet = new ColumnSet(Task.Fields.Subject),
                Criteria = {
                    Conditions = {
                        new ConditionExpression(Task.Fields.RegardingObjectId, ConditionOperator.Equal, targetRef.Id),
                        new ConditionExpression(Task.Fields.ipg_priority, ConditionOperator.Equal, (int)ipg_Priority.High),
                        new ConditionExpression(Task.Fields.StateCode, ConditionOperator.Equal, (int)TaskState.Open),
                    }
                }
            };
            return string.Join("\n", _service.RetrieveMultiple(query).Entities.Select(x => x.ToEntity<Task>().Subject));
        }

        private void UpdateTaskDescription(ipg_tasktype taskType, Task task, EntityReference targetRef)
        {
            var gateManager = new GateManager(_service, null, targetRef);

            if (taskType.ipg_name == "Inactive HCPCS on Case")
            {
                var caseDos = GetCaseDos(targetRef);
                var hcpcsCode = GetInactiveHcpcsCode(caseDos);
                task.Description = string.Format(taskType.ipg_description, hcpcsCode, caseDos);
            }
            else if (taskType.ipg_name.Contains("Inactive CPT on Case"))
            {
                var caseDos = GetCaseDos(targetRef);
                var cptCode = GetInvalidCptCode(targetRef, caseDos);
                task.Description = string.Format(taskType.ipg_description, cptCode, caseDos);
            }
            else if (taskType.ipg_name == "New Physician Request")
            {
                string physicianName;
                string facilityName;
                GetCasePhysicianAndFacility(targetRef, out physicianName, out facilityName);
                task.Description = string.Format(taskType.ipg_description, physicianName, facilityName);
            }
            else if (taskType.ipg_name == "DX Code Issues")
            {
                var dxCodes = GetInvalidDxCodes(targetRef);
                task[Task.Fields.Description] = string.Format(taskType.ipg_description, string.Join(", ", dxCodes));
            }
            else if (taskType.ipg_name.Contains("Open High Priority Task(s)"))
            {
                var openHighPriorityTasksNames = GetOpenHighPriorityTasksNames(targetRef);
                if (!string.IsNullOrEmpty(openHighPriorityTasksNames))
                {
                    task.Description = string.Format(taskType.ipg_description + "\n", openHighPriorityTasksNames);
                }
            }
            else if (taskType.ipg_name.Contains("Claim Generation Errors"))
            {
                var carrier = gateManager.GetCarrier();
                task.Description = string.Format(taskType.ipg_description, carrier.Name);
            }
            else if (taskType.ipg_name.Contains("Missing Facility Carrier Contract"))
            {
                var caseDos = GetCaseDos(targetRef);
                string physicianName;
                string facilityName;
                GetCasePhysicianAndFacility(targetRef, out physicianName, out facilityName);
                var carrier = gateManager.GetCarrier();
                task.Description = string.Format(taskType.ipg_description, facilityName, carrier.Name, caseDos);
            }
            else if (taskType.ipg_name.Contains("Facility BSA Not on File and Facility Not Exempt"))
            {
                string physicianName;
                string facilityName;
                GetCasePhysicianAndFacility(targetRef, out physicianName, out facilityName);
                task.Description = string.Format(taskType.ipg_description, facilityName);
            }
            else if (taskType.ipg_name.Contains("Calc Rev Update and Save Required"))
            {
                var incident = _service.Retrieve(targetRef.LogicalName, targetRef.Id
                    , new ColumnSet(Incident.Fields.ipg_changedcriticalfield, Incident.Fields.Title))
                    .ToEntity<Incident>();
                var criticalField = incident?.ipg_changedcriticalfield;

                task.Description = string.Format(taskType.ipg_description, criticalField, incident.Title);
            }
            else if (taskType.ipg_name.Contains("Check for Carrier Balance"))
            {
                var incident = _service.Retrieve(targetRef.LogicalName, targetRef.Id
                    , new ColumnSet(Incident.Fields.ipg_changedcriticalfield, Incident.Fields.ipg_RemainingCarrierBalance))
                    .ToEntity<Incident>();
                task.Description = string.Format(taskType.ipg_description, incident?.ipg_RemainingCarrierBalance?.Value);
            }
        }

        public EntityReference GetWorkflowTaskTaskType(EntityReference workflowTaskRef, bool succeeded, int codeOutput)
        {
            if (workflowTaskRef == null)
            {
                return null;
            }

            EntityReference taskConfigurationRef = null;
            if (codeOutput == 0)
            {
                taskConfigurationRef = (from wtoc in _crmContext.CreateQuery<ipg_workflowtaskoutputconfig>()
                                        join wt in _crmContext.CreateQuery<ipg_workflowtask>() on wtoc.ipg_WorkflowTaskId.Id equals wt.ipg_workflowtaskId
                                        where wt.ipg_workflowtaskId == workflowTaskRef.Id
                                        && wtoc.ipg_OutcomeType == succeeded
                                        select wtoc.ipg_TaskConfigurationId).FirstOrDefault();
            }
            else
            {
                taskConfigurationRef = (from wtoc in _crmContext.CreateQuery<ipg_workflowtaskoutputconfig>()
                                        join wt in _crmContext.CreateQuery<ipg_workflowtask>() on wtoc.ipg_WorkflowTaskId.Id equals wt.ipg_workflowtaskId
                                        where wt.ipg_workflowtaskId == workflowTaskRef.Id
                                        && wtoc.ipg_CodeOutput == codeOutput
                                        && wtoc.ipg_OutcomeType == succeeded
                                        select wtoc.ipg_TaskConfigurationId).FirstOrDefault();
            }

            if (taskConfigurationRef == null)
            {
                return null;
            }

            var taskConfiguration = _service.Retrieve(taskConfigurationRef.LogicalName, taskConfigurationRef.Id, new ColumnSet(ipg_taskconfiguration.Fields.ipg_tasktypeid)).ToEntity<ipg_taskconfiguration>();
            return taskConfiguration.ipg_tasktypeid;
        }

        public void CancelTask(EntityReference taskRef)
        {
            var setStateRequest = new SetStateRequest()
            {
                EntityMoniker = taskRef,
                State = new OptionSetValue((int)TaskState.Completed),
                Status = new OptionSetValue((int)Task_StatusCode.Cancelled)
            };
            _service.Execute(setStateRequest);
        }
    }
}