using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.Plugin.BVF
{
    public class PopulatePrimaryAttributePlugin : PluginBase
    {
        public PopulatePrimaryAttributePlugin() : base(typeof(PopulatePrimaryAttributePlugin))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, ipg_benefitsverificationform.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;

            var target = localPluginContext.Target<ipg_benefitsverificationform>();

            target.ipg_name = BuildBVFName();

            context.InputParameters["Target"] = target.ToEntity<Entity>();
        }

        private string BuildBVFName()
        {
            return $"BVF - {DateTime.Now.ToString("MMddyyyy")}";
        }
    }
}
