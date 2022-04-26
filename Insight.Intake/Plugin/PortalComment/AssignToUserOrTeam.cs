using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

namespace Insight.Intake.Plugin.PortalComment
{
    public class AssignToUserOrTeam : PluginBase
    {
        private IOrganizationService service;
        public AssignToUserOrTeam() : base(typeof(AssignToUserOrTeam))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, adx_portalcomment.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            service = localPluginContext.OrganizationService;
            var comment = localPluginContext.Target<adx_portalcomment>();
            if (comment.OwnerId != null && comment.OwnerId.LogicalName == SystemUser.EntityLogicalName)
            {
                var commentToUpdate = new adx_portalcomment
                {
                    Id = comment.Id,
                    To = new List<ActivityParty>() { new ActivityParty { PartyId = comment.OwnerId } }
                };

                service.Update(commentToUpdate);
            }
        }
    }
}
