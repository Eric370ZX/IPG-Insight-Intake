using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.TaskEntity
{
    public class RequestToCloseIsResolved : PluginBase
    {
        public RequestToCloseIsResolved() : base(typeof(RequestToCloseIsResolved))
        {
            RegisterEvent(
                PipelineStages.PostOperation,
                "Update",
                Intake.Task.EntityLogicalName,
                PluginAction);
        }
        private void PluginAction(LocalPluginContext pluginContext)
        {
            var target = pluginContext.Target<Intake.Task>();
            if (target.StatusCodeEnum != Task_StatusCode.Resolved)
            {
                return;
            }

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

            pluginContext.OrganizationService.Update(new Incident()
            {
                Id = postImage.RegardingObjectId.Id,
                ipg_casehold = false,
            });
        }
    }
}
