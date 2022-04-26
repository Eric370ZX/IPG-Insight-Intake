using Insight.Intake.Extensions;
using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.SLA
{
    public class GeneratePo : PluginBase
    {
        private static readonly string[] _statusesToProcess = new[]
        {
            "Procedure Complete - Queued for Billing"
        };

        public GeneratePo() : base(typeof(GeneratePo))
        {
            RegisterEvent(
                PipelineStages.PostOperation,
                "Update",
                Incident.EntityLogicalName,
                OnUpdateAsync);
        }

        private void OnUpdateAsync(LocalPluginContext pluginContext)
        {
            Incident target = (pluginContext.PluginExecutionContext.InputParameters["Target"] as Entity).ToEntity<Incident>();
            Incident incident = pluginContext.SystemOrganizationService.Retrieve<Incident>(target.Id, new ColumnSet("ipg_isallreceiveddate", "ipg_casestatusdisplayedid"));

            string statusName = "";

            using (CrmServiceContext context = new CrmServiceContext(pluginContext.SystemOrganizationService))
            {
                if (target.ipg_casestatusdisplayedid is EntityReference caseStatusRef)
                {
                    statusName = pluginContext.SystemOrganizationService
                        .Retrieve<ipg_casestatusdisplayed>(caseStatusRef.Id, new ColumnSet("ipg_name"))
                        .ipg_name;
                }

                var taskManager = new TaskManager(pluginContext.OrganizationService,
                    pluginContext.TracingService,
                    null,
                    pluginContext.PluginExecutionContext.InitiatingUserId);

                var taskType = taskManager.RetrieveTaskTypeByName(Constants.TaskTypeNames.SLAGeneratePO);

                var existingTask = context.TaskSet
                    .FirstOrDefault(t =>
                        t.RegardingObjectId != null && t.RegardingObjectId.Id == incident.Id &&
                        t.ipg_tasktypeid != null && t.ipg_tasktypeid == taskType.ToEntityReference());

                if (existingTask == null && _statusesToProcess.Contains(statusName) && incident.ipg_isallreceiveddate != null)
                {
                    var caseRef = incident.ToEntityReference();

                    context.AddObject(new Task()
                    {
                        Subject = Constants.TaskTypeNames.SLAGeneratePO,
                        ScheduledStart = incident.ipg_isallreceiveddate,
                        ActualStart = DateTime.Now,
                        RegardingObjectId = caseRef,
                        ipg_caseid = caseRef,
                        ipg_taskcategoryid = taskType.ipg_taskcategoryid,
                        ipg_tasktypeid = taskType.ToEntityReference()
                    });

                    context.SaveChanges();
                }
            }
        }
    }
}
