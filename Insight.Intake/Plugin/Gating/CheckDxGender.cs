using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckDxGender : PluginBase
    {
        public CheckDxGender() : base(typeof(CheckDxGender))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingCheckDxGender", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var targetRef = localPluginContext.TargetRef();
            var gateManager = new GateManager(localPluginContext.OrganizationService, localPluginContext.TracingService, targetRef);
            var result = gateManager.CheckDxGender();
            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = result.Succeeded;
            localPluginContext.PluginExecutionContext.OutputParameters["PortalNote"] = result.PortalNote;
            localPluginContext.PluginExecutionContext.OutputParameters["CaseNote"] = result.CaseNote;
            localPluginContext.PluginExecutionContext.OutputParameters["GatingOutcome"] = result.GatingOutcome;
        }
    }
}
