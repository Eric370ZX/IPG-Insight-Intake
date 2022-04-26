using System;
using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk;

namespace Insight.Intake.Plugin.Referral
{
    public class PreventCreationNotFromInitiateReferral : PluginBase
    {
        public PreventCreationNotFromInitiateReferral() : base(typeof(PreventCreationNotFromInitiateReferral))
        {
            RegisterEvent(
                PipelineStages.PreOperation,
                "Create",
                ipg_referral.EntityLogicalName,
                OnCreate);
        }

        private void OnCreate(LocalPluginContext pluginContext)
        {
            ipg_referral referral = pluginContext.PluginExecutionContext.GetTarget<ipg_referral>();

            if (referral.ipg_isinitiatereferral == null || !referral.ipg_isinitiatereferral.Value)
            {
                throw new InvalidPluginExecutionException("Referral can be created only using Initiate Referral button (Document Processing)");
            }

            referral.ipg_isinitiatereferral = null;
        }
    }
}
