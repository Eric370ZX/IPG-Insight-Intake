using Insight.Intake.Plugin.Managers;
//using static Insight.Intake.Plugin.Managers.TaskManager;

namespace Insight.Intake.Plugin.Gating.PostProcess
{
    public class Gate8 : PluginBase
    {
        public Gate8() : base(typeof(Gate8))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingPostProcessGate8", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var targetRef = localPluginContext.TargetRef();
            var minimumSeverityLevel = localPluginContext.PluginExecutionContext.InputParameters.Contains("MinimumSeverityLevel") ? (localPluginContext.PluginExecutionContext.InputParameters["MinimumSeverityLevel"] as int?) : -1;
            var postProcessManager = new PostProcessManager(localPluginContext.OrganizationService, localPluginContext.TracingService, targetRef);
            
            postProcessManager.SetBillingDate();
            if (minimumSeverityLevel == (int)ipg_SeverityLevel.Info || minimumSeverityLevel == (int)ipg_SeverityLevel.Warning)
            {
                postProcessManager.UpdateLifecycleStepAndCaseState();
            }
            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = true;
            localPluginContext.PluginExecutionContext.OutputParameters["Output"] = "";
        }
    }
}
