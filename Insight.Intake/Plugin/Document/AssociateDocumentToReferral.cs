using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Document
{
    public class AssociateDocumentToReferral : PluginBase
    {
        private readonly List<string> multipleAssociationAbbreviations = new List<string>()
        {
            "EML",
            "PTC",
            "ATC",
            "CRC",
            "FCC",
            "RFP",
            "RFC",
            "MFG INV"
        };
        public AssociateDocumentToReferral() : base(typeof(AssociateDocumentToReferral))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, ipg_document.EntityLogicalName, UpdateDocumentFieldsFromReferral);

            RegisterEvent(PipelineStages.PostOperation, MessageNames.Associate, ipg_document.Relationships.ipg_ipg_document_ipg_referral, PostOperationAssociateAction);
        }

        private void PostOperationAssociateAction(LocalPluginContext context)
        {
            var executionContext = context.PluginExecutionContext;

            if (executionContext.InputParameters.Contains("Relationship")
               && executionContext.InputParameters["Relationship"].ToString() != ipg_document.Relationships.ipg_ipg_document_ipg_referral + ".Referencing"
               && executionContext.InputParameters["Relationship"].ToString() != ipg_document.Relationships.ipg_ipg_document_ipg_referral + ".")
            {
                return;
            }

            var service = context.OrganizationService;
            var documentRef = context.TargetRef();
            var document = service.Retrieve(ipg_document.EntityLogicalName, documentRef.Id, new ColumnSet(ipg_document.Fields.ipg_DocumentTypeId)).ToEntity<ipg_document>();
            
            QueryExpression query = new QueryExpression(ipg_referral.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                LinkEntities =
                {
                    new LinkEntity
                    {
                        LinkFromEntityName = ipg_referral.EntityLogicalName,
                        LinkToEntityName = ipg_document.Relationships.ipg_ipg_document_ipg_referral,
                        LinkFromAttributeName = ipg_referral.Fields.Id,
                        LinkToAttributeName = ipg_referral.Fields.Id,
                        LinkEntities =
                        {
                            new LinkEntity
                            {
                                LinkFromEntityName = ipg_document.Relationships.ipg_ipg_document_ipg_referral,
                                LinkToEntityName = ipg_document.EntityLogicalName,
                                LinkFromAttributeName = ipg_document.Fields.Id,
                                LinkToAttributeName = ipg_document.Fields.Id,
                                LinkCriteria = new FilterExpression
                                {
                                    Conditions =
                                    {
                                        new ConditionExpression(ipg_document.Fields.Id, ConditionOperator.Equal, document.Id)
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var result = service.RetrieveMultiple(query);

            var referralsCount = result.Entities.Count;

            var documentTypeAbbr = service.Retrieve(ipg_documenttype.EntityLogicalName, document.ipg_DocumentTypeId.Id, new ColumnSet(ipg_documenttype.Fields.ipg_DocumentTypeAbbreviation))
                .ToEntity<ipg_documenttype>().ipg_DocumentTypeAbbreviation;

            if (!multipleAssociationAbbreviations.Contains(documentTypeAbbr) && referralsCount > 1)
                throw new ArgumentException("Only one referral can be associate to the document of " + documentTypeAbbr + " type!");
        }

        private void UpdateDocumentFieldsFromReferral(LocalPluginContext context)
        {
            var document = context.Target<ipg_document>();

            if (HasDocumentAssociatedReferral(document))
            {
                AssignDocumentToReferralOwner(context, document);
            }
        }

        private void AssignDocumentToReferralOwner(LocalPluginContext context, ipg_document document)
        {
            var referralOwner = new OrganizationServiceContext(context.OrganizationService).CreateQuery<ipg_referral>().
                FirstOrDefault(referral => referral.Id == document.ipg_ReferralId.Id).OwnerId;
            document.OwnerId = referralOwner;
        }
        private bool HasDocumentAssociatedReferral(ipg_document ipg_document) => ipg_document.ipg_ReferralId != null;
    }
}
