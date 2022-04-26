using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.Plugin.Referral
{
    public class GeneratePifDocumentFromPortal : PluginBase
    {
        public GeneratePifDocumentFromPortal() : base(typeof(GeneratePifDocumentFromPortal))
        {
            RegisterEvent(
                PipelineStages.PostOperation,
                "Create",
                ipg_referral.EntityLogicalName,
                OnCreate);
        }

        private void OnCreate(LocalPluginContext pluginContext)
        {
            ipg_referral target = (pluginContext.PluginExecutionContext.InputParameters["Target"] as Entity).ToEntity<ipg_referral>();

            if (target.ipg_Origin != null && target.ipg_Origin.Value == (int)Incident_CaseOriginCode.Portal)
            {
                Guid documentId = new DocumentManager(pluginContext.OrganizationService, pluginContext.TracingService)
                    .CreateSystemGenereatedPortalPif(target);
            }
        }
    }
}
