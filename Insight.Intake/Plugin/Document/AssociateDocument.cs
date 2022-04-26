using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.Document
{
    public class AssociateDocument : PluginBase
    {
        public AssociateDocument() : base(typeof(AssociateDocument))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, ipg_document.EntityLogicalName, UpdateAssosiatedDocuments);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, ipg_document.EntityLogicalName, UpdateAssosiatedDocuments);
        }

        private void UpdateAssosiatedDocuments(LocalPluginContext context)
        {
            var document = context.Target<ipg_document>();
            if (HasDocumentAssociatedCase(document) || HasDocumentAssociatedReferral(document))
            {
                SetReviewStatus(document);
                SetFacilityFromRelatedCaseOrReferral(document, context.OrganizationService);
            }

        }
        private void SetFacilityFromRelatedCaseOrReferral(ipg_document document, IOrganizationService crmService)
        {
            var relatedFacilityId = GetRelatedFacilityId(document, crmService);

            if (relatedFacilityId != null)
                document.ipg_FacilityId = new EntityReference(Intake.Account.EntityLogicalName, relatedFacilityId);
        }
        private Guid GetRelatedFacilityId(ipg_document document, IOrganizationService crmService)
        {
            var context = new OrganizationServiceContext(crmService);

            return HasDocumentAssociatedCase(document)
                ? context.CreateQuery<Incident>().
                                          Where(incident => incident.Id == document.ipg_CaseId.Id && incident.ipg_FacilityId != null).
                                          Select(incident => incident.ipg_FacilityId.Id).
                                          FirstOrDefault()
                : context.CreateQuery<ipg_referral>().
                                          Where(referral => referral.Id == document.ipg_ReferralId.Id && referral.ipg_FacilityId != null).
                                          Select(referral => referral.ipg_FacilityId.Id).
                                          FirstOrDefault();

        }
        private void SetReviewStatus(ipg_document document) => 
            document.ipg_ReviewStatus = document.ipg_Source?.Value == (int)ipg_DocumentSourceCode.Portal 
            ? new OptionSetValue((int)ipg_document_ipg_ReviewStatus.PendingReview) 
            : new OptionSetValue((int)ipg_document_ipg_ReviewStatus.Approved);
        private bool HasDocumentAssociatedCase(ipg_document ipg_document) => ipg_document.ipg_CaseId != null;
        private bool HasDocumentAssociatedReferral(ipg_document ipg_document) => ipg_document.ipg_ReferralId != null;
    }
}
