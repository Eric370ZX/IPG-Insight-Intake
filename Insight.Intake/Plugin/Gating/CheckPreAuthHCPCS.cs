using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckPreAuthHCPCS : PluginBase
    {
        public CheckPreAuthHCPCS() : base(typeof(CheckPreAuthHCPCS))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingCheckPreAuthHCPCS", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var targetRef = localPluginContext.TargetRef();
            var gateManager = new GateManager(localPluginContext.OrganizationService, localPluginContext.TracingService, targetRef);
            var result = gateManager.ValidatePreAuthHCPCS();
            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = result.Succeeded;
            localPluginContext.PluginExecutionContext.OutputParameters["PortalNote"] = result.PortalNote;
            localPluginContext.PluginExecutionContext.OutputParameters["CaseNote"] = result.CaseNote;
            localPluginContext.PluginExecutionContext.OutputParameters["TaskDescripton"] = result.TaskDescripton;
        }
    }
}
