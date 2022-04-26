using Insight.Intake.Models;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Authorization
{
    public class ResetAuthWfTasks : PluginBase
    {
        public ResetAuthWfTasks() : base(typeof(ResetAuthWfTasks))
        {
            RegisterEvent(
                PipelineStages.PostOperation,
                MessageNames.Create,
                ipg_authorization.EntityLogicalName,
                OnCreatePostOperation);
        }
        private void OnCreatePostOperation(LocalPluginContext pluginContext)
        {
            var postImage = pluginContext.PostImage<ipg_authorization>();
            var caseId = postImage.ipg_incidentid;
            if (caseId == null)
            {
                return;
            }
            var authWFCache = GetAuthCache(pluginContext, caseId.Id);
            foreach (var iCache in authWFCache)
            {
                pluginContext.SystemOrganizationService.Delete(iCache.LogicalName, iCache.Id);
            }

            var authTasks = GetAuthTasks(pluginContext, caseId.Id);
            foreach (var iTask in authTasks)
            {
                pluginContext.OrganizationService.Create(new ipg_caseworkflowtask()
                {
                    ipg_CaseId = postImage.ipg_incidentid,
                    ipg_WorkflowTaskId = iTask.ToEntityReference(),
                    ipg_Passed = true
                });
            }

        }
        private IEnumerable<ipg_workflowtask> GetAuthTasks(LocalPluginContext pluginContext, Guid caseId)
        {
            var query = new QueryExpression(ipg_workflowtask.EntityLogicalName)
            {
                Criteria = new FilterExpression()
                {
                    Conditions = {
                    new ConditionExpression(ipg_workflowtask.Fields.ipg_WFTaskGroupId, ConditionOperator.Equal, WorkflowTaskGroupsConstants.ReviewAuthorizationRequirements)
                    }
                }
            };

            var result = pluginContext.OrganizationService.RetrieveMultiple(query)
                .Entities;
            result.Add(new ipg_workflowtask() { Id = WorkflowTasksConstants.PreauthorizationHCPCS });

            return result.Select(p => p.ToEntity<ipg_workflowtask>());
        }
        private IEnumerable<ipg_caseworkflowtask> GetAuthCache(LocalPluginContext pluginContext, Guid caseId)
        {
            var fetchHCPCS = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                          <entity name='ipg_caseworkflowtask'>
                            <attribute name='ipg_caseworkflowtaskid' />
                            <attribute name='ipg_name' />
                            <order attribute='ipg_name' descending='false' />
                            <filter type='and'>
                              <condition attribute='ipg_caseid' operator='eq' value='{caseId}' />
                              <condition attribute='ipg_workflowtaskid' operator='eq' value='{WorkflowTasksConstants.PreauthorizationHCPCS}' />
                            </filter>
                          </entity>
                        </fetch>";
            var resultsHCPCS = pluginContext.OrganizationService.RetrieveMultiple(new FetchExpression(fetchHCPCS))
                .Entities
                .Select(p => p.ToEntity<ipg_caseworkflowtask>());

            var fetchAuthGroup = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='ipg_caseworkflowtask'>
                                    <attribute name='ipg_caseworkflowtaskid' />
                                    <attribute name='ipg_name' />
                                    <order attribute='ipg_name' descending='false' />
                                    <filter type='and'>
                                      <condition attribute='ipg_caseid' operator='eq' value='{caseId}' />
                                    </filter>
                                    <link-entity name='ipg_workflowtask' from='ipg_workflowtaskid' to='ipg_workflowtaskid' link-type='inner' alias='aa'>
                                      <filter type='and'>
                                        <condition attribute='ipg_wftaskgroupid' operator='eq' value='{WorkflowTaskGroupsConstants.ReviewAuthorizationRequirements}' />
                                      </filter>
                                    </link-entity>
                                  </entity>
                                </fetch>";
            var resultsAuthGroup = pluginContext.OrganizationService.RetrieveMultiple(new FetchExpression(fetchAuthGroup))
                .Entities
                .Select(p => p.ToEntity<ipg_caseworkflowtask>())
                .ToList();
            resultsAuthGroup.AddRange(resultsHCPCS);
            return resultsAuthGroup;
        }
    }
}
