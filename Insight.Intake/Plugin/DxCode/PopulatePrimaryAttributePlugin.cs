using Microsoft.Xrm.Sdk;

namespace Insight.Intake.Plugin.DxCode
{
    public class PopulatePrimaryAttributePlugin : PluginBase
    {
        public PopulatePrimaryAttributePlugin() : base(typeof(PopulatePrimaryAttributePlugin))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, ipg_dxcode.EntityLogicalName, PreOperationCreateOrUpdateHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_dxcode.EntityLogicalName, PreOperationCreateOrUpdateHandler);
        }

        private void PreOperationCreateOrUpdateHandler(LocalPluginContext pluginContext)
        {
            var target = pluginContext.Target<ipg_dxcode>();

            if (pluginContext.PluginExecutionContext.MessageName == MessageNames.Create)
            {
                target.ipg_name = $"{target.ipg_DxCode} - {target.ipg_dxname}";
            }
            else if (pluginContext.PluginExecutionContext.MessageName == MessageNames.Update)
            {
                if ((target.Contains(ipg_dxcode.Fields.ipg_DxCode) && target.ipg_DxCode != null)
                    || (target.Contains(ipg_dxcode.Fields.ipg_dxname) && target.ipg_dxname != null))
                {
                    var dxCode = pluginContext.PostImage<ipg_dxcode>();
                    pluginContext.OrganizationService.Update(new ipg_dxcode()
                    {
                        Id = target.Id,
                        ipg_name = $"{dxCode.ipg_DxCode} - {dxCode.ipg_dxname}"
                    });
                }
            }
            else
            {
                throw new InvalidPluginExecutionException("Unexpected message name: " + pluginContext.PluginExecutionContext.MessageName);
            }
        }
    }
}