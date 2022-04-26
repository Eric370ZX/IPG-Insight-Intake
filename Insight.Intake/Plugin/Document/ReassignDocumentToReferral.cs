using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Insight.Intake.Extensions;

namespace Insight.Intake.Plugin.Document
{
    public class ReassignDocumentToReferral: PluginBase
    {
        public ReassignDocumentToReferral() : base(typeof(ReassignDocumentToReferral))
        {
            RegisterEvent(
                PipelineStages.PostOperation,
                "ipg_IPGDocumentActionsReassignToReferral",
                ipg_document.EntityLogicalName,
                OnExecute);
        }

        private void OnExecute(LocalPluginContext pluginContext)
        {
            EntityReference targetRef = pluginContext.PluginExecutionContext.InputParameters["Target"] as EntityReference;
            EntityReference referralRef = pluginContext.PluginExecutionContext.InputParameters["ReferralRef"] as EntityReference;

            DocumentManager documentManager = new DocumentManager(pluginContext.OrganizationService, pluginContext.TracingService);

            ipg_document document = pluginContext.OrganizationService.Retrieve<ipg_document>(targetRef.LogicalName, targetRef.Id, new ColumnSet(ipg_document.Fields.ipg_ReferralId));

            documentManager.ReassignDocumentToReferral(targetRef, referralRef);
            documentManager.HandleOriginalTaskForReassignedDocumentForReferral(targetRef, document.ipg_ReferralId, referralRef);

            pluginContext.PluginExecutionContext.OutputParameters["IsSuccess"] = true;
            pluginContext.PluginExecutionContext.OutputParameters["Message"] = "";
        }
    }


}

