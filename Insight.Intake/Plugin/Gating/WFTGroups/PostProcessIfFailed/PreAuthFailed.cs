using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating.WFTGroups.PostProcessIfFailed
{
    public class PreAuthFailed : PluginBase
    {
        public PreAuthFailed() : base(typeof(PreAuthFailed))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingGroupPreAuthFailed", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var targetRef = localPluginContext.TargetRef();
            if (targetRef.LogicalName == Incident.EntityLogicalName) {
                localPluginContext.OrganizationService.Update(new Incident()
                {
                    Id = targetRef.Id,
                    ipg_is_authorization_required = true
                });
            }
        }
    }
}
