using Insight.Intake.Plugin.GatingV2.CommonWfTask;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.GatingV2
{
    public class TaskManager_2
    {
        List<Task> _existingTasks;
        private EntityReference _caseRef;
        IOrganizationService _service;
        OrganizationServiceContext _crmContext;

        public TaskManager_2(EntityReference caseRef, IOrganizationService service)
        {
            this._caseRef = caseRef;
            this._service = service;
            _crmContext = new OrganizationServiceContext(_service);

        }
        public EntityCollection CreateGatingTasks(ipg_gateconfiguration gateConfiguration, EntityCollection tasksToCreate, Entity gateConfigurationDetail, bool succeeded, WFTaskResult gatingResponse = null)
        {
            EntityCollection taskConfigurations = GetActiveTaskConfiguration(gateConfigurationDetail.Id);

            foreach (Entity taskConfiguration in taskConfigurations.Entities)
            {
                if (!TaskAlreadyExist(taskConfiguration, _caseRef))
                {
                    var taskConfigurationEntity = taskConfiguration.ToEntity<ipg_taskconfiguration>();
                    Guid.TryParse(gatingResponse?.GatingOutcome, out var gatingOutcome);

                    if ((taskConfigurationEntity.ipg_gatingoutcomeid == null
                        && ((taskConfigurationEntity.ipg_createif?.Value == (int)ipg_CreateIf.Succeeded && succeeded)
                        || (taskConfigurationEntity.ipg_createif?.Value == (int)ipg_CreateIf.Failed && !succeeded)
                        || (taskConfigurationEntity.ipg_createif?.Value == (int)ipg_CreateIf.Both)))
                        || (taskConfigurationEntity.ipg_gatingoutcomeid?.Id == gatingOutcome))
                    {
                        tasksToCreate.Entities.Add(CreateTask(_caseRef, taskConfiguration, gateConfiguration, gateConfigurationDetail, gatingResponse));
                    }
                }
            }
            return tasksToCreate;
        }

        private bool TaskAlreadyExist(Entity taskConfiguration, EntityReference caseRef)
        {

            if (_existingTasks == null)
            {
                if (caseRef != null)
                {
                    _existingTasks = (from task in _crmContext.CreateQuery<Task>()
                                      where task.RegardingObjectId.Id == caseRef.Id
                                      select task).ToList();
                }
                else
                {
                    _existingTasks = new List<Task>();
                }
            }

            if (taskConfiguration.Contains("ipg_tasktypecode"))
            {
                return _existingTasks.Where(x => x.ipg_tasktypecode != null && x.ipg_tasktypecode.Value == taskConfiguration.GetAttributeValue<OptionSetValue>("ipg_tasktypecode")?.Value).Count() > 0;
            }

            return false;
        }
        public EntityCollection GetActiveTaskConfiguration(Guid gateConfigurationDetailId)
        {
            var query = new QueryExpression()
            {
                EntityName = ipg_taskconfiguration.EntityLogicalName,
                ColumnSet = new ColumnSet("ipg_subject", "ipg_duedate", "ipg_priority", "ipg_reassigntoteamid", "ipg_createif", "ipg_tasktypecode", "ipg_gatingoutcomeid")
            };
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("ipg_gateconfigurationdetailid", ConditionOperator.Equal, gateConfigurationDetailId);

            return _service.RetrieveMultiple(query);
        }
        private Entity CreateTask(EntityReference targetRef, Entity taskConfiguration, Entity gateConfiguration, Entity gateConfigurationDetail, WFTaskResult gatingResponse)
        {
            Entity task = new Entity(Task.EntityLogicalName);
            task["regardingobjectid"] = targetRef;
            task[Task.Fields.Description] = gatingResponse?.TaskDescripton;
            if (taskConfiguration != null && taskConfiguration.Contains("ipg_subject"))
            {
                task["subject"] = string.IsNullOrEmpty(gatingResponse?.TaskSubject)
                    ? taskConfiguration?.GetAttributeValue<string>("ipg_subject")
                    : gatingResponse?.TaskSubject;
            }
            if (taskConfiguration != null && taskConfiguration.Contains("ipg_tasktypecode"))
            {
                task["ipg_tasktypecode"] = taskConfiguration.GetAttributeValue<OptionSetValue>("ipg_tasktypecode");
            }
            if (taskConfiguration != null && taskConfiguration.Contains("ipg_duedate"))
            {
                DateTime now = DateTime.UtcNow;
                task["scheduledend"] = now.AddDays(Convert.ToDouble(taskConfiguration.GetAttributeValue<int>("ipg_duedate")));
            }
            if (taskConfiguration != null && taskConfiguration.Contains("ipg_priority"))
            {
                task["ipg_priority"] = taskConfiguration.GetAttributeValue<OptionSetValue>("ipg_priority");
            }
            if (taskConfiguration != null && taskConfiguration.Contains("ipg_reassigntoteamid"))
            {
                task["ownerid"] = taskConfiguration.GetAttributeValue<EntityReference>("ipg_reassigntoteamid");
            }
            else
            {
                task["ownerid"] = gateConfiguration.GetAttributeValue<EntityReference>("ipg_assigntoteamid");
            }
            if (gateConfigurationDetail != null && gateConfigurationDetail.GetAttributeValue<OptionSetValue>("ipg_severitylevel") != null)
            {
                var severity = gateConfigurationDetail.GetAttributeValue<OptionSetValue>("ipg_severitylevel").Value;
                if (severity == (int)ipg_SeverityLevel.Critical || severity == (int)ipg_SeverityLevel.Error)
                {
                    task["ipg_systemtasktypecode"] = new OptionSetValue((int)ipg_SystemTaskType.WorkflowTask_ErrororCritical);
                }
                else if (severity == (int)ipg_SeverityLevel.Warning || severity == (int)ipg_SeverityLevel.Info)
                {
                    task["ipg_systemtasktypecode"] = new OptionSetValue((int)ipg_SystemTaskType.WorkflowTask_InfoorWarning);

                }

            }
            task["ipg_gateconfigdetailid"] = gateConfigurationDetail?.ToEntityReference();

            task["ipg_taskcategorycode"] = new OptionSetValue((int)ipg_Taskcategory1.System);

            return task;
        }
        private void ReOpenTask(Entity taskConfiguration)
        {
            var existingTask = _existingTasks.Where(x => x.ipg_tasktypecode != null && x.ipg_tasktypecode.Value == taskConfiguration.GetAttributeValue<OptionSetValue>("ipg_tasktypecode")?.Value).FirstOrDefault(null);
            if (existingTask != null)
            {
                var task = new Task();
                task.Id = existingTask.Id;
                task.StateCode = TaskState.Open;
                task.StatusCode = new OptionSetValue((int)Task_StatusCode.NotStarted);
                _service.Update(task);
            }
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
    }
}
