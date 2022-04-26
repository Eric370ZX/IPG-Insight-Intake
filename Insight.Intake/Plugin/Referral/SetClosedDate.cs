using System;

namespace Insight.Intake.Plugin.Referral
{
    public class SetClosedDate : PluginBase
    {
        public SetClosedDate() : base(typeof(SetClosedDate))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, ipg_referral.EntityLogicalName, PreOperationHandler);
        }
        private void PreOperationHandler(LocalPluginContext localPluginContext)
        {
            var referral = localPluginContext.Target<ipg_referral>();
            var preImage = localPluginContext.PreImage<ipg_referral>();
            if (referral.ipg_casestatus?.Value == (int)ipg_CaseStatus.Closed && referral.ipg_casestatus?.Value != preImage.ipg_casestatus?.Value)
            {
                referral.ipg_closeddate = DateTime.Now;
            }
        }
    }
}
