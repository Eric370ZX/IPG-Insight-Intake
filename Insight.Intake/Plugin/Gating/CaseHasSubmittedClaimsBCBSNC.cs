using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    //Joined with validation CaseHasSubmittedClaims per https://eti-ipg.atlassian.net/browse/CPI-21926
    [Obsolete]
    public class CaseHasSubmittedClaimsBCBSNC : PluginBase
    {
        public CaseHasSubmittedClaimsBCBSNC() : base(typeof(CaseHasSubmittedClaimsBCBSNC))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingCaseHasSubmittedClaimsBCBSNC", null, PostOperationHandler);
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
            var primaryCarrier = gateManager.GetCarrier();
            if (primaryCarrier.Name?.ToUpper().Contains("BCBSNC") == false)
            {
                context.OutputParameters["Succeeded"] = true;
                return;
            }
            var result = gateManager.HasSubmittedClaims();
            context.OutputParameters["Succeeded"] = result;
        }
    }
}
