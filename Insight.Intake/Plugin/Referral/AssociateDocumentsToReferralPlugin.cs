using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using Microsoft.Xrm.Sdk.Query;
using Insight.Intake.Plugin.Managers;
using Insight.Intake.Helpers;
using Insight.Intake.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Insight.Intake.Plugin.Referral
{
    public class AssociateDocumentsToReferralPlugin : PluginBase
    {
        private IOrganizationService service;
        public AssociateDocumentsToReferralPlugin() : base(typeof(AssociateDocumentsToReferralPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_referral.EntityLogicalName, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            service = localPluginContext.OrganizationService;
            var referral = localPluginContext.Target<ipg_referral>();

            if (IsRequestedFromPortal(referral))
            {
                //skip association if portal request
                return;
            }
            if (referral.ipg_OriginEnum == Incident_CaseOriginCode.EHR)
            {
                //skip association if this is EHR
                return;
            }
            if (referral.ipg_SourceDocumentId == null)
            {
                throw new InvalidPluginExecutionException("Source document field is empty.");
            }
            var pifDoc = service.Retrieve(
                    ipg_document.EntityLogicalName,
                    referral.ipg_SourceDocumentId.Id,
                    new ColumnSet(false)).ToEntity<ipg_document>();

            var referralRef = referral.ToEntityReference();
            pifDoc.Id = referral.ipg_SourceDocumentId.Id;
            pifDoc.ipg_ReferralId = referralRef;
            pifDoc.ipg_ReviewStatus = ipg_document_ipg_ReviewStatus.Approved.ToOptionSetValue();
            pifDoc.ipg_InitiatedReferralOn = DateTime.UtcNow;

            var relatedDocs = RetrieveRelatedToPifDocuments(pifDoc.Id);
            relatedDocs.Add(pifDoc);
            if (relatedDocs.Count > 0)
            {
                foreach (var doc in relatedDocs)
                {
                    var document = new ipg_document();
                    document.Id = doc.Id;
                    document.ipg_ReferralId = referralRef;
                    document.ipg_InitiatedReferralOn = DateTime.UtcNow;
                    document.ipg_ReviewStatus = ipg_document_ipg_ReviewStatus.Approved.ToOptionSetValue();
                    document.ipg_ipg_document_ipg_referral = new List<ipg_referral>() { referral };
                    service.Update(document);
                }
            }
           

            CreateE1Log(referral, localPluginContext);
        }

        private DataCollection<Entity> RetrieveRelatedToPifDocuments(Guid pifId)
        {
            var query = new QueryExpression(ipg_document.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression( ipg_document.Fields.ipg_maindocument, ConditionOperator.Equal, pifId)
                    }
                }
            };
            return service.RetrieveMultiple(query).Entities;
        }
        private void CreateE1Log(ipg_referral referral,LocalPluginContext localPluginContext)
        {
            if (localPluginContext.PluginExecutionContext.Stage == PipelineStages.PostOperation && localPluginContext.PluginExecutionContext.MessageName == MessageNames.Create)
            {
                var importantEventManager = new ImportantEventManager(service);
                importantEventManager.CreateImportantEventLog(referral, localPluginContext.PluginExecutionContext.InitiatingUserId, Constants.EventIds.ET1);
                importantEventManager.SetCaseOrReferralPortalHeader(referral, Constants.EventIds.ET1);
            }
        }
        private bool IsRequestedFromPortal(ipg_referral referral) => referral.ipg_portaluserid != null;
    }
}
