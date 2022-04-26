using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Gating.PostProcess
{
    public class Gate6 : PluginBase
    {
        public Gate6() : base(typeof(Gate6))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingPostProcessGate6", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var targetRef = localPluginContext.TargetRef();
            //All received logic is deprecated according to CPI-22045 
            


            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = true;
            localPluginContext.PluginExecutionContext.OutputParameters["Output"] = "";
        }
    }
}