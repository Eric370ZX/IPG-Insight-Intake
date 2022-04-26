using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;

namespace Insight.Intake.Plugin.Case
{
    public class CheckIfCaseCanBeClosed : PluginBase
    {
        public CheckIfCaseCanBeClosed() : base(typeof(CheckIfCaseCanBeClosed))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGCaseCheckIfCaseCanBeClosed", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var caseRef = localPluginContext.TargetRef();
            var gateManager = new CaseManager(localPluginContext.OrganizationService, localPluginContext.TracingService, caseRef);
            var result=gateManager.CheckIfCaseCanBeClosed();           
            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = result.Succeeded;
            localPluginContext.PluginExecutionContext.OutputParameters["SeverityLevel"] =new OptionSetValue((int)result.SeverityLevel);
            localPluginContext.PluginExecutionContext.OutputParameters["Output"] = result.Output;
        }
    }
}
