using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Document
{
    public class AssociateDocumentToAccount : PluginBase
    {
        private readonly List<string> multipleAssociationAbbreviations = new List<string>()
        {
            "BAA",
            "BSA",
            "FOT",
            "FPL"
        };
        public AssociateDocumentToAccount() : base(typeof(AssociateDocumentToAccount))
        {          
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Associate, ipg_document.Relationships.ipg_ipg_document_account, PostOperationAssociateAction);
        }

        private void PostOperationAssociateAction(LocalPluginContext context)
        {
            var executionContext = context.PluginExecutionContext;

            if (executionContext.InputParameters.Contains("Relationship")
               && executionContext.InputParameters["Relationship"].ToString() != ipg_document.Relationships.ipg_ipg_document_account + ".Referenced"
               && executionContext.InputParameters["Relationship"].ToString() != ipg_document.Relationships.ipg_ipg_document_account + ".")
            {
                return;
            }

            var service = context.OrganizationService;
            var documentRef = context.GetNullAbleInput<EntityReferenceCollection>("RelatedEntities")[0];
            var document = service.Retrieve(ipg_document.EntityLogicalName, documentRef.Id, new ColumnSet(ipg_document.Fields.ipg_DocumentTypeId)).ToEntity<ipg_document>();

            QueryExpression query = new QueryExpression(Intake.Account.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                LinkEntities =
                {
                    new LinkEntity
                    {
                        LinkFromEntityName = Intake.Account.EntityLogicalName,
                        LinkToEntityName = ipg_document.Relationships.ipg_ipg_document_account,
                        LinkFromAttributeName = Intake.Account.Fields.Id,
                        LinkToAttributeName = Intake.Account.Fields.Id,
                        LinkEntities =
                        {
                            new LinkEntity
                            {
                                LinkFromEntityName = ipg_document.Relationships.ipg_ipg_document_account,
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

            var accountsCount = result.Entities.Count;

            var documentTypeAbbr = service.Retrieve(ipg_documenttype.EntityLogicalName, document.ipg_DocumentTypeId.Id, new ColumnSet(ipg_documenttype.Fields.ipg_DocumentTypeAbbreviation))
                .ToEntity<ipg_documenttype>().ipg_DocumentTypeAbbreviation;

            if (!multipleAssociationAbbreviations.Contains(documentTypeAbbr) && accountsCount > 1)
                throw new ArgumentException("Only one account can be associate to the document of " + documentTypeAbbr + " type!");
        }
                
    }

}