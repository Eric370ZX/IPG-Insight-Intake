using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Insight.Intake.Extensions;

namespace Insight.Intake.Plugin.Document
{
    public class ReassingDocumentToCase : PluginBase
    {
        public ReassingDocumentToCase() : base(typeof(ReassingDocumentToCase))
        {
            RegisterEvent(
                PipelineStages.PostOperation,
                "ipg_IPGDocumentActionsReassignToCase",
                ipg_document.EntityLogicalName,
                OnExecute);
        }

        private void OnExecute(LocalPluginContext pluginContext)
        {
            EntityReference targetRef = pluginContext.PluginExecutionContext.InputParameters["Target"] as EntityReference;
            EntityReference caseRef = pluginContext.PluginExecutionContext.InputParameters["CaseRef"] as EntityReference;

            DocumentManager documentManager = new DocumentManager(pluginContext.OrganizationService, pluginContext.TracingService);

            ipg_document document = pluginContext.OrganizationService.Retrieve<ipg_document>(targetRef.LogicalName, targetRef.Id, new ColumnSet("ipg_caseid"));

            documentManager.ReassignDocumentToCase(targetRef, caseRef);
            documentManager.HandleOriginalTaskForReassignedDocument(targetRef, document.ipg_CaseId, caseRef);

            pluginContext.PluginExecutionContext.OutputParameters["IsSuccess"] = true;
            pluginContext.PluginExecutionContext.OutputParameters["Message"] = "";
        }
    }
}
