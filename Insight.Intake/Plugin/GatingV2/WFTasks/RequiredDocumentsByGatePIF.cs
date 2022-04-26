using Insight.Intake.Helpers;
using Insight.Intake.Plugin.GatingV2.CommonWfTask;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.GatingV2.WFTasks
{
    public class RequiredDocumentsByGatePIF : WFTaskBase
    {
        public override WFTaskResult Run(WFTaskContext ctx)
        {
            var service = ctx.CrmService;
            var tracingService = ctx.TraceService;

            var gatingResponse = new WFTaskResult(false);

            if (ctx.dbContext.Referral != null)
            {
                var referral = ctx.dbContext.Referral;
                //923720001 - Fax
                if (!referral.ipg_OriginEnum.HasValue || referral.ipg_OriginEnum.Value != Incident_CaseOriginCode.Fax)
                {
                    gatingResponse.Succeeded = true;
                }
                else if (referral.ipg_SourceDocumentId_Entity?.ipg_DocumentTypeId_Entity?.ipg_DocumentTypeAbbreviation == Constants.DocumentTypeAbbreviations.PIF)
                {
                    gatingResponse.Succeeded = true;
                }
            }
            else if (ctx.dbContext.Case != null)
            {
                var incident = ctx.dbContext.Case;
                if (!incident.CaseOriginCodeEnum.HasValue || incident.CaseOriginCodeEnum.Value != Incident_CaseOriginCode.Fax)
                {
                    return new WFTaskResult(true);
                }
                var pifDocuments = GetPIFDocuments(service, incident.ToEntityReference());
                gatingResponse.Succeeded = pifDocuments.Entities.Count != 1;
            }
            return gatingResponse;
        }

        private EntityCollection GetPIFDocuments(IOrganizationService service, EntityReference targetRef)
        {
            QueryExpression query = new QueryExpression("ipg_document")
            {
                ColumnSet = new ColumnSet(false)
            };
            query.Criteria.AddCondition("ipg_caseid", ConditionOperator.Equal, targetRef.Id);

            LinkEntity linkEntity = new LinkEntity("ipg_document", "ipg_documenttype", "ipg_documenttypeid", "ipg_documenttypeid", JoinOperator.Inner);
            linkEntity.LinkCriteria.AddCondition("ipg_documenttypeabbreviation", ConditionOperator.Equal, Constants.DocumentTypeAbbreviations.PIF);

            query.LinkEntities.Add(linkEntity);

            return service.RetrieveMultiple(query);
        }
    }
}