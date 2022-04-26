using Microsoft.Xrm.Sdk;

namespace Insight.Intake.Plugin.PortalComment
{
    public class SetMessageStatus : PluginBase
    {
        public SetMessageStatus() : base(typeof(SetDefaultPortalFields))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, adx_portalcomment.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, adx_portalcomment.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var comment = localPluginContext.Target<adx_portalcomment>();
            if (comment.StateCode.HasValue && comment.StateCode == adx_portalcommentState.Open)
            {
                comment.StatusCode = new OptionSetValue((int)adx_portalcomment_StatusCode.Unread);
            }
            else if(comment.StateCode.HasValue && 
                    comment.StateCode == adx_portalcommentState.Completed)
            {
                comment.StatusCode = new OptionSetValue((int)adx_portalcomment_StatusCode.Read);
            }
            localPluginContext.OrganizationService.Update(comment);
        }
    }
}

