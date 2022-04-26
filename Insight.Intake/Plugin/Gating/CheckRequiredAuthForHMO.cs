using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckRequiredAuthForHMO : PluginBase
    {
        public CheckRequiredAuthForHMO() : base(typeof(CheckRequiredAuthForHMO))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingCheckRequiredAuthForHMO", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var targetRef = localPluginContext.TargetRef();
            var gateManager = new GateManager(localPluginContext.OrganizationService, localPluginContext.TracingService, targetRef);
            var result = gateManager.CheckRequiredAuthForHMO();
            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = result.Succeeded;
            localPluginContext.PluginExecutionContext.OutputParameters["PortalNote"] = result.PortalNote;
            localPluginContext.PluginExecutionContext.OutputParameters["CaseNote"] = result.CaseNote;
        }
    }
}
