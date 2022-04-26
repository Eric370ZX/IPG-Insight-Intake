using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckValidatedAcceptedClaim : PluginBase
    {
        public CheckValidatedAcceptedClaim() : base(typeof(CheckValidatedAcceptedClaim))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingCheckValidatedAcceptedClaim", null, PostOperationHandler);
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
           
            var result = gateManager.CheckValidatedAcceptedClaim();
            context.OutputParameters["Succeeded"] = result;
        }
    }
}
