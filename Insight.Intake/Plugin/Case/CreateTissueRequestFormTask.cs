using Insight.Intake.Extensions;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Insight.Intake.Plugin.Case
{
    public class CreateTissueRequestFormTask : PluginBase
    {
        private static readonly string[] _incidentColumns = new string[]
        {
            Incident.Fields.ipg_CPTCodeId1, Incident.Fields.ipg_CPTCodeId2, Incident.Fields.ipg_CPTCodeId3,
            Incident.Fields.ipg_CPTCodeId4, Incident.Fields.ipg_CPTCodeId5, Incident.Fields.ipg_CPTCodeId6
        };

        public CreateTissueRequestFormTask() : base(typeof(CreateTissueRequestFormTask))
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

        private void OnUpdateAsync(LocalPluginContext pluginContext)
        {
            Incident target = pluginContext.PluginExecutionContext.GetTarget<Incident>();
            Incident incident = pluginContext.OrganizationService.Retrieve<Incident>(target.Id, new ColumnSet(_incidentColumns));

            ProcessIncident(pluginContext, incident);
        }

        private void OnCreateAsync(LocalPluginContext pluginContext)
        {
            Incident target = pluginContext.PluginExecutionContext.GetTarget<Incident>();

            ProcessIncident(pluginContext, target);
        }

        private void ProcessIncident(LocalPluginContext pluginContext, Incident incident)
        {
            bool isTrfRequired = new CaseManager(pluginContext.OrganizationService, pluginContext.TracingService, incident.ToEntityReference())
                .IsCaseRequiresTissueRequestForm(incident);

            TaskManager taskManager = new TaskManager(
                pluginContext.OrganizationService, pluginContext.TracingService, 
                null, pluginContext.PluginExecutionContext.InitiatingUserId);

            if (isTrfRequired)
            {
                taskManager.UpsertMissingTissueRequestFormTask(incident.Id);
            }
            else
            {
                taskManager.CancelOpenMissingTissueRequestFormTasks(incident.Id);
            }
        }
    }
}
