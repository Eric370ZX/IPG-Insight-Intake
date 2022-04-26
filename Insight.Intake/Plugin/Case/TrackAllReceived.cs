using Microsoft.Xrm.Sdk.Query;
using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.Plugin.Case
{
    //this plugin is deprecated according to CPI-22045
    [Obsolete]
    public class TrackAllReceived : PluginBase
    {
        public TrackAllReceived() : base(typeof(TrackAllReceived))
        {
            RegisterEvent(
                PipelineStages.PreOperation,
                "Update",
                Incident.EntityLogicalName,
                OnUpdate);
        }

        private void OnUpdate(LocalPluginContext pluginContext)
        {
            ColumnSet columns = new ColumnSet("ipg_isallreceived", "ipg_isallreceiveddate", "ipg_allreceivedby");
            Entity target = pluginContext.PluginExecutionContext.InputParameters["Target"] as Entity;

            Incident incident = pluginContext.OrganizationService.Retrieve<Incident>(target.Id, columns);

            if (target.Contains("ipg_isallreceived") && incident.ipg_isallreceiveddate == null)
            {
                target["ipg_isallreceiveddate"] = DateTime.UtcNow;
                target["ipg_allreceivedby"] = new EntityReference(SystemUser.EntityLogicalName, pluginContext.PluginExecutionContext.InitiatingUserId);
            }
        }
    }
}
