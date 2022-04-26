using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckCasePartDetails: PluginBase
    {
        public CheckCasePartDetails() : base(typeof(CheckCasePartDetails))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingCheckCasePartDetails", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var targetRef = localPluginContext.TargetRef();
            var gateManager = new GateManager(localPluginContext.OrganizationService, localPluginContext.TracingService, targetRef);
            var result = gateManager.CheckCasePartDetails();
            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = result.Succeeded;
            localPluginContext.PluginExecutionContext.OutputParameters["PortalNote"] = result.PortalNote;
            localPluginContext.PluginExecutionContext.OutputParameters["CaseNote"] = result.CaseNote;
        }
    }
}
