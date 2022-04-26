using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.Plugin.PortalComment
{
    public class SetDefaultPortalFields : PluginBase
    {
        public static readonly Guid CaseManagementTeam = new Guid("eb250319-b41d-e911-a979-000d3a370e23");
        public SetDefaultPortalFields() : base(typeof(SetDefaultPortalFields))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, adx_portalcomment.EntityLogicalName, PreOperationHandler);
        }

        private void PreOperationHandler(LocalPluginContext localPluginContext)
        {
            var comment = localPluginContext.Target<adx_portalcomment>();
            comment.ipg_assignedtoteamid = new EntityReference(Team.EntityLogicalName, CaseManagementTeam);
            if (comment.adx_PortalCommentDirectionCode.Value == (int)adx_portalcomment_adx_PortalCommentDirectionCode.Incoming)
            {
                if (comment.ipg_owningportaluserid != null && comment.ipg_owningportaluserid.LogicalName == "contact")
                {
                    comment.ipg_portalcreatedby = comment.ipg_owningportaluserid.Name;
                }
                comment.StatusCode = new OptionSetValue((int)adx_portalcomment_StatusCode.Unread);
                comment.ipg_tofrom = "To IPG";
                comment.ipg_from = "Facility";
            }
            else
            {
                comment.ipg_portalcreatedby = "IPG";
                comment.ipg_tofrom = "From IPG";
                comment.ipg_from = "IPG";
            }
        }
    }
}
