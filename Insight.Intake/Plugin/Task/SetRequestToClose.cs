using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.TaskEntity
{
    public class SetRequestToClose : PluginBase
    {
        public SetRequestToClose() : base(typeof(SetRequestToClose))
        {
            RegisterEvent(
                PipelineStages.PostOperation,
                "Create",
                Intake.Task.EntityLogicalName,
                PluginAction);

            RegisterEvent(
                PipelineStages.PostOperation,
                "Update",
                Intake.Task.EntityLogicalName,
                PluginAction);
        }

        private void PluginAction(LocalPluginContext pluginContext)
        {
            var postImage = pluginContext.PostImage<Intake.Task>();
            if (postImage.ipg_tasktypeid == null || postImage.RegardingObjectId == null || postImage.RegardingObjectId.LogicalName != Incident.EntityLogicalName)
            {
                return;
            }
            var taskType = pluginContext.SystemOrganizationService.Retrieve(postImage.ipg_tasktypeid.LogicalName, postImage.ipg_tasktypeid.Id, new ColumnSet(ipg_tasktype.Fields.ipg_typeid))
                .ToEntity<ipg_tasktype>();

            if (taskType.ipg_typeid != 99022)//Request to Close Case
            {
                return;
            }
            var carrierServiceTeam = GetCarrierServicesTeam(pluginContext.SystemOrganizationService);
            var updIncident = new Incident()
            {
                Id = postImage.RegardingObjectId.Id,
                ipg_casehold = true,
                ipg_caseholdreasonEnum = ipg_Caseholdreason.ManagerReview,
                ipg_assignedtoteamid = carrierServiceTeam.ToEntityReference(),
            };
            if (carrierServiceTeam != null)
                updIncident.OwnerId = carrierServiceTeam.ToEntityReference();

            pluginContext.OrganizationService.Update(updIncident);
        }
        private Intake.Team GetCarrierServicesTeam(IOrganizationService orgService)
        {
            using (var context = new OrganizationServiceContext(orgService))
            {
                var teams = from team in context.CreateQuery<Intake.Team>()
                            where team.Name == "Carrier Services"
                            select team;
                return teams.FirstOrDefault();
            }
        }
    }
}
