using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;
using Insight.Intake.Extensions;

namespace Insight.Intake.Plugin.TaskEntity
{
    public class PopulateSoonestTasksOnCase : PluginBase
    {
        public PopulateSoonestTasksOnCase() : base(typeof(PopulateSoonestTasksOnCase))
        {
            RegisterEvent(
                PipelineStages.PostOperation,
                "Create",
                Task.EntityLogicalName,
                OnCreateAsync);

            RegisterEvent(
                PipelineStages.PostOperation,
                "Update",
                Task.EntityLogicalName,
                OnUpdateAsync);

            RegisterEvent(
                PipelineStages.PostOperation,
                "Delete",
                Task.EntityLogicalName,
                OnDeleteAsync);
        }

        private void OnDeleteAsync(LocalPluginContext pluginContext)
        {
            Task preImage = pluginContext.PluginExecutionContext.PreEntityImages["PreImage"].ToEntity<Task>();

            ProcessTask(preImage, pluginContext);
        }

        private void OnUpdateAsync(LocalPluginContext pluginContext)
        {
            Task target = (pluginContext.PluginExecutionContext.InputParameters["Target"] as Entity).ToEntity<Task>();
            Task preImage = (pluginContext.PluginExecutionContext.PreEntityImages["PreImage"]).ToEntity<Task>();
            Task task = pluginContext.OrganizationService.Retrieve<Task>(target.Id, new ColumnSet(true));

            if (target.ipg_caseid != null || target.StateCode != null)
            {
                ProcessTask(task, pluginContext);
            }

            if (preImage.ipg_caseid != null && task.ipg_caseid != null && preImage.ipg_caseid.Id != task.ipg_caseid.Id)
            {
                ProcessTask(preImage, pluginContext);
            }
        }

        private void OnCreateAsync(LocalPluginContext pluginContext)
        {
            Task target = (pluginContext.PluginExecutionContext.InputParameters["Target"] as Entity).ToEntity<Task>();
            
            if (target.ipg_caseid != null)
            {
                ProcessTask(target, pluginContext);
            }
        }

        private void ProcessTask(Task task, LocalPluginContext pluginContext)
        {
            TaskManager taskManager = new TaskManager(
                    pluginContext.OrganizationService,
                    pluginContext.TracingService,
                    task.ToEntityReference(),
                    pluginContext.PluginExecutionContext.InitiatingUserId);

            var fields = taskManager.GetCaseFieldsBySoonestTasksMapping(task);
            if (fields.Any())
            {
                Incident updateIncident = new Incident()
                {
                    Id = task.ipg_caseid.Id,
                    Attributes = new AttributeCollection()
                };

                foreach (var field in fields)
                {
                    updateIncident[field] = taskManager.GetSoonestDueTasksByCaseField(field, task.ipg_caseid.Id);
                }

                pluginContext.OrganizationService.Update(updateIncident);
            }
        }
    }
}
