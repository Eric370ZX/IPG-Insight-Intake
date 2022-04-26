using Insight.Intake.Plugin.Managers;
using Insight.Intake.Repositories;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using Insight.Intake.Extensions;

namespace Insight.Intake.Plugin.TaskEntity
{
    public class PopulateSoonestAuthTaskOnCase : PluginBase
    {
        public PopulateSoonestAuthTaskOnCase() : base(typeof(PopulateSoonestAuthTaskOnCase))
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

            bool isOwnerFromAuth = new UserManager(pluginContext.OrganizationService)
                    .IsAuth(preImage.OwnerId);

            if (isOwnerFromAuth)
            {
                ProcessTask(pluginContext, preImage.ipg_caseid.Id);
            }
        }

        private void OnUpdateAsync(LocalPluginContext pluginContext)
        {
            Task target = (pluginContext.PluginExecutionContext.InputParameters["Target"] as Entity).ToEntity<Task>();
            Task preImage = pluginContext.PluginExecutionContext.PreEntityImages["PreImage"].ToEntity<Task>();
            Task task = pluginContext.OrganizationService.Retrieve<Task>(target.Id, new ColumnSet(true));

            bool isPrevOwnerFromAuth = new UserManager(pluginContext.OrganizationService).IsAuth(preImage.OwnerId);
            bool isOwnerFromAuth = new UserManager(pluginContext.OrganizationService).IsAuth(task.OwnerId);

            if (isOwnerFromAuth && target.ipg_caseid != null)
            {
                ProcessTask(pluginContext, target.ipg_caseid.Id);
            }

            if (isPrevOwnerFromAuth && preImage.ipg_caseid != null && preImage.ipg_caseid.Id != target.ipg_caseid?.Id)
            {
                ProcessTask(pluginContext, preImage.ipg_caseid.Id);
            }
        }

        private void OnCreateAsync(LocalPluginContext pluginContext)
        {
            Task target = (pluginContext.PluginExecutionContext.InputParameters["Target"] as Entity).ToEntity<Task>();

            if (target.ipg_caseid != null && target.OwnerId != null)
            {
                bool isOwnerFromAuth = new UserManager(pluginContext.OrganizationService)
                    .IsAuth(target.OwnerId);

                if (isOwnerFromAuth)
                {
                    ProcessTask(pluginContext, target.ipg_caseid.Id);
                }
            }
        }

        private void ProcessTask(LocalPluginContext pluginContext, Guid caseId)
        {
            ColumnSet taskColumns = new ColumnSet(false);

            Entity soonestTask = new TaskRepository(pluginContext.OrganizationService)
                .GetOpenAuthTasksByCase(caseId, taskColumns, 1)
                .FirstOrDefault();

            pluginContext.OrganizationService.Update(new Incident()
            {
                Id = caseId,
                ipg_soonestauthtaskid = soonestTask != null ? soonestTask.ToEntityReference() : null
            });
        }
    }
}