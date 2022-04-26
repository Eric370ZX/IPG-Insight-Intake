using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating.PostProcess
{
    public class Gate10 : PluginBase
    {
        public Gate10() : base(typeof(Gate10))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingPostProcessGate10", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var targetRef = localPluginContext.TargetRef();
            var postProcessManager = new PostProcessManager(localPluginContext.OrganizationService, localPluginContext.TracingService, targetRef);
            var result = postProcessManager.SetCollectionDate();
            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = result.Succeeded;
            localPluginContext.PluginExecutionContext.OutputParameters["Output"] = result.Succeeded;
        }
    }
}
