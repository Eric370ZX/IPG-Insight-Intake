using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckRequiredDocumentByGate : PluginBase
    {

        public CheckRequiredDocumentByGate() : base(typeof(CheckRequiredDocumentByGate))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingCheckRequiredDocumentByGate", null, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;
            var targetRef = context.InputParameters["Target"] as EntityReference;
            var documentTypeRef = context.InputParameters["DocumentType"] as EntityReference;

            context.OutputParameters["CaseNote"] = string.Empty;
            context.OutputParameters["PortalNote"] = string.Empty;

            var query = new QueryExpression(ipg_document.EntityLogicalName);
            query.ColumnSet = new ColumnSet(false);
            var parentReferenceField = targetRef.LogicalName == Incident.EntityLogicalName ?
                nameof(ipg_document.ipg_CaseId).ToLower()
                : nameof(ipg_document.ipg_ReferralId).ToLower();
            query.Criteria.AddCondition(parentReferenceField, ConditionOperator.Equal, targetRef.Id);
            query.Criteria.AddCondition(nameof(ipg_document.ipg_DocumentTypeId).ToLower(), ConditionOperator.Equal, documentTypeRef.Id);
            query.Criteria.AddCondition(nameof(ipg_document.StateCode).ToLower(), ConditionOperator.Equal, (int)ipg_documentState.Active);
            query.Criteria.AddCondition(nameof(ipg_document.ipg_ReviewStatus).ToLower(), ConditionOperator.Equal, (int)ipg_document_ipg_ReviewStatus.Approved);
            var result = service.RetrieveMultiple(query);
            context.OutputParameters["Succeeded"] = result.Entities.Any();
            var abbreviation = (service.Retrieve(documentTypeRef.LogicalName, documentTypeRef.Id, new ColumnSet(ipg_documenttype.Fields.ipg_DocumentTypeAbbreviation)).ToEntity<ipg_documenttype>()).ipg_DocumentTypeAbbreviation.Replace(" ", "");
            context.OutputParameters["CodeOutput"] = (int)Enum.Parse(typeof(DocumentTypes), abbreviation);
        }
    }

    public enum DocumentTypes
    {
        ICS = 1,
        MFGINV = 2,
        PPP = 3,
        OPR = 4,
        MRC = 5,
        LOMN = 6,
        TRF = 7
    }
}
