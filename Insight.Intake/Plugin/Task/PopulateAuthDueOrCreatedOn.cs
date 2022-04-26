using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.TaskEntity
{
    public class PopulateAuthDueOrCreatedOn : PluginBase
    {
        public PopulateAuthDueOrCreatedOn() : base(typeof(PopulateAuthDueOrCreatedOn))
        {
            RegisterEvent(
                PipelineStages.PostOperation,
                "Create",
                Incident.EntityLogicalName,
                OnCreateAsync);

            RegisterEvent(
                PipelineStages.PostOperation,
                "Update",
                Incident.EntityLogicalName,
                OnUpdateAsync);
        }

        private void OnCreateAsync(LocalPluginContext pluginContext)
        {
            Incident target = pluginContext.PluginExecutionContext.GetTarget<Incident>();

            DateTime? authDueOrCreatedOn = target.CreatedOn;

            if (target.ipg_soonestauthtaskid != null)
            {
                ColumnSet columns = new ColumnSet(Task.Fields.ScheduledEnd);
                Task authTask = pluginContext.OrganizationService.Retrieve<Task>(target.ipg_soonestauthtaskid.Id, columns);

                if (authTask.ScheduledEnd != null && authTask.ScheduledEnd.HasValue && authTask.ScheduledEnd < authDueOrCreatedOn)
                    authDueOrCreatedOn = authTask.ScheduledEnd;
            }

            pluginContext.OrganizationService.Update(new Incident()
            {
                Id = target.Id,
                ipg_authdueorcreatedon = authDueOrCreatedOn
            });
        }

        private void OnUpdateAsync(LocalPluginContext pluginContext)
        {
            Incident target = pluginContext.PluginExecutionContext.GetTarget<Incident>();
            Incident incident = pluginContext.OrganizationService.Retrieve<Incident>(target.Id, new ColumnSet(Incident.Fields.CreatedOn));

            DateTime? authDueOrCreatedOn = incident.CreatedOn;

            if (target.ipg_soonestauthtaskid != null)
            {
                ColumnSet columns = new ColumnSet(Task.Fields.ScheduledEnd);
                Task authTask = pluginContext.OrganizationService.Retrieve<Task>(target.ipg_soonestauthtaskid.Id, columns);

                if (authTask.ScheduledEnd != null && authTask.ScheduledEnd.HasValue && authTask.ScheduledEnd < authDueOrCreatedOn)
                    authDueOrCreatedOn = authTask.ScheduledEnd;
            }

            pluginContext.OrganizationService.Update(new Incident()
            {
                Id = target.Id,
                ipg_authdueorcreatedon = authDueOrCreatedOn
            });
        }
    }
}