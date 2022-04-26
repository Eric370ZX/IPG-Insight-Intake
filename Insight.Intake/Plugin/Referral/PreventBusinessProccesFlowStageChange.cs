using Microsoft.Xrm.Sdk;

namespace Insight.Intake.Plugin.Referral
{
    public class PreventBusinessProccesFlowStageChange : PluginBase
    {
        public PreventBusinessProccesFlowStageChange() : base(typeof(PreventBusinessProccesFlowStageChange))
        {
            RegisterEvent(PipelineStages.PreValidation, MessageNames.Update, ipg_ipgreferralbpfmainflow.EntityLogicalName, PreValidationHandler);
        }

        private void PreValidationHandler(LocalPluginContext localPluginContext)
        {
            var bpf = localPluginContext.Target<ipg_ipgreferralbpfmainflow>();
            var preImage = localPluginContext.PreImage<ipg_ipgreferralbpfmainflow>();
            if (bpf.Contains(ipg_ipgreferralbpfmainflow.Fields.ActiveStageId) && bpf.ActiveStageId != preImage.ActiveStageId)
            {
                throw new InvalidPluginExecutionException("You are not able to change stage.");
            }
        }
    }
}
