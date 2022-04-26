using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.Plugin.Gating
{
    public class CaseHasSubmittedClaims : PluginBase
    {
        public CaseHasSubmittedClaims() : base(typeof(CaseHasSubmittedClaims))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingCaseHasSubmittedClaims", null, PostOperationHandler);
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
            var result = gateManager.HasSubmittedClaims();
            context.OutputParameters["Succeeded"] = result;
        }
    }
}
