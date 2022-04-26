using Insight.Intake.Extensions;
using Insight.Intake.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Authorization
{
    public class CompleteTasks : PluginBase
    {
        private const int AUTHORIZATION_REQUIRED_TASK_TYPE_ID = 1123;

        public CompleteTasks() : base(typeof(CaseRecentAuthorization))
        {
            RegisterEvent(
                PipelineStages.PostOperation,
                MessageNames.Create,
                ipg_authorization.EntityLogicalName,
                OnCreatePostOperation);
        }

        private void OnCreatePostOperation(LocalPluginContext pluginContext)
        {
            pluginContext.TracingService.Trace("Get Target");
            ipg_authorization target = pluginContext.PluginExecutionContext.GetTarget<ipg_authorization>();

            if (target.ipg_incidentid != null && target.ipg_carrierid != null)
            {
                pluginContext.TracingService.Trace("Retrieving open tasks");
                var openTasks = GetAuthorizationTasks(pluginContext.SystemOrganizationService, target.ipg_incidentid.Id, target.ipg_carrierid.Id);

                pluginContext.TracingService.Trace($"Completing {openTasks.Count} tasks");
                foreach (var openTask in openTasks)
                {
                    pluginContext.OrganizationService.Update(new Task()
                    {
                        Id = openTask.Id,
                        StateCode = TaskState.Completed,
                        StatusCodeEnum = Task_StatusCode.Resolved
                    });
                }
            }
        }

        private List<Task> GetAuthorizationTasks(IOrganizationService orgService, Guid caseId, Guid carrierId)
        {
            var tasks = new List<Task>();
            
            var fetchHCPCS = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='task'>
                                <attribute name='subject' />
                                <attribute name='activityid' />
                                <order attribute='subject' descending='false' />
                                <filter type='and'>
                                  <condition attribute='ipg_workflowtaskid' operator='eq' value='{WorkflowTasksConstants.PreauthorizationHCPCS}' />
                                  <condition attribute='statecode' operator='eq' value='0' />
                                  <condition attribute='regardingobjectid' operator='eq' value='{caseId}' />
                                </filter>
                              </entity>
                            </fetch>";
            var resultsHCPCS = orgService.RetrieveMultiple(new FetchExpression(fetchHCPCS))
                .Entities
                .Select(p => p.ToEntity<Task>());
            tasks.AddRange(resultsHCPCS);

            var fetchAuthGroup = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='task'>
                                <attribute name='subject' />
                                <attribute name='activityid' />
                                <order attribute='subject' descending='false' />
                                <filter type='and'>
                                  <condition attribute='statecode' operator='eq' value='0' />
                                  <condition attribute='regardingobjectid' operator='eq' value='{caseId}' />
                                </filter>
                                <link-entity name='ipg_workflowtask' from='ipg_workflowtaskid' to='ipg_workflowtaskid' link-type='inner' alias='aa'>
                                  <filter type='and'>
                                    <condition attribute='ipg_wftaskgroupid' operator='eq' value='{WorkflowTaskGroupsConstants.ReviewAuthorizationRequirements}' />
                                  </filter>
                                </link-entity>
                              </entity>
                            </fetch>";
            var resultsAuthGroup = orgService.RetrieveMultiple(new FetchExpression(fetchAuthGroup))
                .Entities
                .Select(p => p.ToEntity<Task>())
                .ToList();
            tasks.AddRange(resultsAuthGroup);

            using (CrmServiceContext context = new CrmServiceContext(orgService))
            {
                var authRequiredTasks = (from task in context.TaskSet
                                 join taskType in context.ipg_tasktypeSet on task.ipg_tasktypeid.Id equals taskType.Id
                                 where taskType.ipg_typeid == AUTHORIZATION_REQUIRED_TASK_TYPE_ID
                                     && task.RegardingObjectId != null && task.RegardingObjectId.Id == caseId
                                     && task.ipg_carrierid != null && task.ipg_carrierid.Id == carrierId
                                     && task.StateCode == TaskState.Open
                                 select task).ToList();
                tasks.AddRange(authRequiredTasks);
            }
            

            return tasks;
        }
    }
}