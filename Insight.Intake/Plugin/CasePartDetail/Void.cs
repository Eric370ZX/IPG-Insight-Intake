using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.Plugin.CasePartDetail
{
    public class Void : PluginBase
    {
        public Void() : base(typeof(Void))
        {
            RegisterEvent(
                PipelineStages.PostOperation,
                "ipg_CasePartDetailActionVoid",
                ipg_casepartdetail.EntityLogicalName,
                OnExecute);
        }

        private void OnExecute(LocalPluginContext pluginContext)
        {
            pluginContext.PluginExecutionContext.OutputParameters["IsSuccess"] = false;
            pluginContext.PluginExecutionContext.OutputParameters["Message"] = "Error processing a remove part request";

            EntityReference targetRef = pluginContext.TargetRef();

            try
            {
                new CasePartDetailManager(pluginContext.OrganizationService, pluginContext.TracingService)
                    .VoidCasePartFromCase(targetRef.Id);

                pluginContext.PluginExecutionContext.OutputParameters["IsSuccess"] = true;
                pluginContext.PluginExecutionContext.OutputParameters["Message"] = "";
            }
            catch (ArgumentException e)
            {
                pluginContext.PluginExecutionContext.OutputParameters["IsSuccess"] = false;
                pluginContext.PluginExecutionContext.OutputParameters["Message"] = e.Message;
            }
        }
    }
}
