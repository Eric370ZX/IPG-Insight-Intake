using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating.PostProcess
{
    public class Gate12 : PluginBase
    {
        public Gate12() : base(typeof(Gate12))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingPostProcessGate12", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var targetRef = localPluginContext.TargetRef();
            var postProcessManager = new PostProcessManager(localPluginContext.OrganizationService, localPluginContext.TracingService, targetRef);
            var minimumSeverityLevel= localPluginContext.PluginExecutionContext.InputParameters.Contains("MinimumSeverityLevel") ? (localPluginContext.PluginExecutionContext.InputParameters["MinimumSeverityLevel"] as int?) :-1;
            if (minimumSeverityLevel == (int)ipg_SeverityLevel.Info)
            {
                //  var note = "Case Closed by System for $0.00 Balance.";
                var note = "";
                postProcessManager.CreateNote(note, note);
            }
            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = true;
            localPluginContext.PluginExecutionContext.OutputParameters["Output"] = "";
        }
    }
}
