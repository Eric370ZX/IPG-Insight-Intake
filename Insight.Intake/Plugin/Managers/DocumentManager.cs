using Insight.Intake.Extensions;
using Insight.Intake.Helpers;
using Insight.Intake.Repositories;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Managers
{
    public class DocumentManager
    {
        private IOrganizationService _crmService;
        private ITracingService _tracingService;

        private DocumentTypeRepository _documentTypeRepository;

        public DocumentManager(IOrganizationService organizationService, ITracingService tracingService)
        {
            this._crmService = organizationService;
            this._tracingService = tracingService;

            _documentTypeRepository = new DocumentTypeRepository(organizationService);
        }

        public void ReassignDocumentToCase(EntityReference documentRef, EntityReference caseRef)
        {
            if (documentRef != null && caseRef != null)
            {
                _crmService.Update(new ipg_document()
                {
                    Id = documentRef.Id,
                    ipg_CaseId = caseRef
                });
            }
        }

        public void ReassignDocumentToReferral(EntityReference documentRef, EntityReference referralRef)
        {
            if (documentRef != null && referralRef != null)
            {
                _crmService.Update(new ipg_document()
                {
                    Id = documentRef.Id,
                    ipg_ReferralId = referralRef
                });
            }
        }

        public void HandleOriginalTaskForReassignedDocument(EntityReference documentRef, EntityReference originalCaseRef, EntityReference caseRef)
        {
            ColumnSet documentColumns = new ColumnSet("ipg_originatingtaskid", "ipg_documenttypeid", "ownerid");

            ipg_document document = _crmService.Retrieve<ipg_document>(documentRef.LogicalName, documentRef.Id, documentColumns);
            
            if (originalCaseRef != null) 
            {
                if (document.ipg_originatingtaskid != null)
                {
                    _crmService.Update(new Task()
                    {
                        Id = document.ipg_originatingtaskid.Id,
                        RegardingObjectId = caseRef,
                        ipg_caseid = caseRef
                    });
                }
                else
                {
                    Task newTask = new Task()
                    {
                        RegardingObjectId = originalCaseRef,
                        ipg_caseid = originalCaseRef,
                        ipg_DocumentType = document.ipg_DocumentTypeId,
                        ipg_taskcategorycode = ipg_Taskcategory1.User.ToOptionSetValue(),
                        OwnerId = document.OwnerId
                    };

                    newTask.Id = _crmService.Create(newTask);

                    _crmService.Update(new Task()
                    {
                        Id = newTask.Id,
                        ipg_caseid = caseRef,
                        RegardingObjectId = caseRef,
                        StateCode = TaskState.Completed,
                        StatusCode = Task_StatusCode.Resolved.ToOptionSetValue()
                    });
                }
            }
        }

        public void HandleOriginalTaskForReassignedDocumentForReferral(EntityReference documentRef, EntityReference originalReferralRef, EntityReference referralRef)
        {
            ColumnSet documentColumns = new ColumnSet("ipg_originatingtaskid", "ipg_documenttypeid", "ownerid");

            ipg_document document = _crmService.Retrieve<ipg_document>(documentRef.LogicalName, documentRef.Id, documentColumns);

            if (originalReferralRef != null)
            {
                if (document.ipg_originatingtaskid != null)
                {
                    _crmService.Update(new Task()
                    {
                        Id = document.ipg_originatingtaskid.Id,
                        RegardingObjectId = referralRef
                    });
                }
                else
                {
                    Task newTask = new Task()
                    {
                        RegardingObjectId = originalReferralRef,
                        ipg_DocumentType = document.ipg_DocumentTypeId,
                        ipg_taskcategorycode = ipg_Taskcategory1.User.ToOptionSetValue(),
                        OwnerId = document.OwnerId
                    };

                    newTask.Id = _crmService.Create(newTask);

                    _crmService.Update(new Task()
                    {
                        Id = newTask.Id,
                        RegardingObjectId = referralRef,
                        StateCode = TaskState.Completed,
                        StatusCode = Task_StatusCode.Resolved.ToOptionSetValue()
                    });
                }
            }
        }

        public Guid CreateSystemGenereatedPortalPif(ipg_referral referral)
        {
            if (referral != null)
            {
                ipg_documenttype pifType = _documentTypeRepository.GetByAbbreviation(Constants.DocumentTypeAbbreviations.PIF);

                if (pifType == null)
                {
                    throw new InvalidPluginExecutionException("PIF document type hasn't found in the system");
                }

                return _crmService.Create(new ipg_document()
                {
                    ipg_ReferralId = referral.ToEntityReference(),
                    ipg_Source = ipg_DocumentSourceCode.Portal.ToOptionSetValue(),
                    ipg_DocumentTypeId = pifType.ToEntityReference(),
                    ipg_InitiatedReferralOn = referral.CreatedOn
                });
            }

            throw new InvalidPluginExecutionException("Can't generate a document without a referral");
        }

        /// <summary>
        /// Looking for oldest Original Doc Date value from documents which are passed as a param 
        /// </summary>
        /// <param name="documentIds"></param>
        /// <returns>Oldest value of Original Doc Date field from document passed as a param. If no documents found - DateTime.UtcNow</returns>
        internal DateTime GetOldestOriginalDocDate(Guid[] documentIds)
        {
            QueryExpression query = new QueryExpression(ipg_document.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(ipg_document.Fields.ipg_originaldocdate),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_document.Fields.ipg_documentId, ConditionOperator.In, documentIds),
                        new ConditionExpression(ipg_document.Fields.ipg_originaldocdate, ConditionOperator.NotNull)
                    }
                },
                Orders =
                {
                    new OrderExpression(ipg_document.Fields.ipg_originaldocdate, OrderType.Ascending)
                }
            };

            Entity document = _crmService.RetrieveMultiple(query)?.Entities?.FirstOrDefault();
            if (document != null)
            {
                return document.ToEntity<ipg_document>().ipg_originaldocdate.Value;
            }

            return DateTime.UtcNow;
        }

        internal IEnumerable<ipg_document> GetDocsByAccountOfType(EntityReference acntRef, string documentTypeAbbr)
        {
            if (acntRef == null||string.IsNullOrEmpty(documentTypeAbbr))
            {
                return null;
            }
            var fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                  <entity name='ipg_document'>
                    <all-attributes/>
                    <order attribute='ipg_name' descending='false' />
                    <filter type='and'>
                      <condition attribute='ipg_facilityid' operator='eq' value='{acntRef.Id}' />
                      <condition attribute='statecode' operator='eq' value='0' />
                    </filter>
                    <link-entity name='ipg_documenttype' from='ipg_documenttypeid' to='ipg_documenttypeid' link-type='inner' alias='aa'>
                      <filter type='and'>
                        <condition attribute='ipg_documenttypeabbreviation' operator='eq' value='{documentTypeAbbr}' />
                      </filter>
                    </link-entity>
                  </entity>
                </fetch>";
            return _crmService
                .RetrieveMultiple(new FetchExpression(fetch))
                .Entities
                .Select(p => p.ToEntity<ipg_document>());
        }
    }
}