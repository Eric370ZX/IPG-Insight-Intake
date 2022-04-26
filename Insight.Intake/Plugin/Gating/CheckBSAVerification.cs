namespace Insight.Intake.Plugin.Gating
{
    public class CheckBSAVerification : PluginBase
    {
        public CheckBSAVerification() : base(typeof(CheckBSAVerification))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingCheckBSAVerification", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var targetRef = localPluginContext.TargetRef();
            var gateManager = new GateManager(localPluginContext.OrganizationService, localPluginContext.TracingService, targetRef);
            var result = gateManager.ValidateBSAVerification();
            context.OutputParameters["Succeeded"] = result.Succeeded;
            context.OutputParameters["CodeOutput"] = result.CodeOutput;
            context.OutputParameters["PortalNote"] = string.Empty;
            context.OutputParameters["CaseNote"] = string.Empty;
        }
    }
}
