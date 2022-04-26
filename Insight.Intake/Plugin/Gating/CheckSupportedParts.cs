using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckSupportedParts : PluginBase
    {
        public CheckSupportedParts() : base(typeof(CheckSupportedParts))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingCheckSupportedParts", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var targetRef = (EntityReference)localPluginContext.PluginExecutionContext.InputParameters["Target"];
            if (targetRef == null)
            {
                throw new InvalidPluginExecutionException("Target case is null");
            }
            var gateManager = new GateManager(localPluginContext.OrganizationService, localPluginContext.TracingService, targetRef);
            var result = gateManager.CheckSupportedParts();
            context.OutputParameters["Succeeded"] = result;
        }
    }
}
