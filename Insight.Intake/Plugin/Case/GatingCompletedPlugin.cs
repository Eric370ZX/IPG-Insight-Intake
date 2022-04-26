using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Case
{
    public class GatingCompletedPlugin : PluginBase
    {
        public GatingCompletedPlugin() : base(typeof(GatingCompletedPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, "Update", Incident.EntityLogicalName, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var targetCase = localPluginContext.Target<Incident>();
            var postCase = localPluginContext.PostImage<Incident>();
            
            if (targetCase.ipg_gateoutcome == null || 
                !(postCase.ipg_ehrupdatestatus?.Value == (int)Incident_ipg_ehrupdatestatus.EHRclosedcasererun || postCase.ipg_ehrupdatestatus?.Value == (int)Incident_ipg_ehrupdatestatus.EHRopencasererun))
            {
                return;
            }

            if (postCase.ipg_ehrupdatestatus?.Value == (int)Incident_ipg_ehrupdatestatus.EHRclosedcasererun
                && (targetCase.ipg_gateoutcomeEnum == ipg_SeverityLevel.Error || targetCase.ipg_gateoutcomeEnum == ipg_SeverityLevel.Critical))
            {
                var updCase = new Incident();
                updCase.Id = targetCase.Id;
                updCase.ipg_ehrupdatestatus = null;
                updCase.StatusCodeEnum = Incident_StatusCode.Canceled;
                updCase.StateCode = IncidentState.Canceled;
                updCase.ipg_CaseStatusEnum = ipg_CaseStatus.Closed;
                localPluginContext.SystemOrganizationService.Update(updCase);
            }
        }
    }
}
