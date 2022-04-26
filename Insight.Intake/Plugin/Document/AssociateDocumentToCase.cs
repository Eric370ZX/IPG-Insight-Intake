using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Document
{
    public class AssociateDocumentToCase : PluginBase
    {
        private readonly List<string> multipleAssociationAbbreviations = new List<string>()
        {
            "MFG INV"
        };

        public AssociateDocumentToCase() : base(typeof(AssociateDocumentToCase))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, ipg_document.EntityLogicalName, PreOperationCreateHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, ipg_document.EntityLogicalName, PreOperationUpdateHandler);

            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_document.EntityLogicalName, PostOperationCreateOrUpdateHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_document.EntityLogicalName, PostOperationCreateOrUpdateHandler);

            RegisterEvent(PipelineStages.PostOperation, MessageNames.Associate, ipg_document.Relationships.ipg_incident_ipg_document, PostOperationAssociateAction);
        }

        private void PostOperationAssociateAction(LocalPluginContext context)
        {
            var executionContext = context.PluginExecutionContext;

            if (executionContext.InputParameters.Contains("Relationship")
                && executionContext.InputParameters["Relationship"].ToString() != ipg_document.Relationships.ipg_incident_ipg_document + ".Referencing"
                && executionContext.InputParameters["Relationship"].ToString() != ipg_document.Relationships.ipg_incident_ipg_document + ".")
            {
                return;
            }

            var service = context.OrganizationService;
            var documentRef = context.GetNullAbleInput<EntityReferenceCollection>("RelatedEntities")[0];
            if (documentRef.LogicalName == ipg_document.EntityLogicalName)
            {
                var document = service.Retrieve(documentRef.LogicalName, documentRef.Id, new ColumnSet(ipg_document.Fields.ipg_DocumentTypeId)).ToEntity<ipg_document>();
                
                QueryExpression query = new QueryExpression(Incident.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(false),
                    LinkEntities =
                {
                    new LinkEntity
                    {
                        LinkFromEntityName = Incident.EntityLogicalName,
                        LinkToEntityName = ipg_document.Relationships.ipg_incident_ipg_document,
                        LinkFromAttributeName = Incident.Fields.Id,
                        LinkToAttributeName = Incident.Fields.Id,
                        LinkEntities =
                        {
                            new LinkEntity
                            {
                                LinkFromEntityName = ipg_document.Relationships.ipg_incident_ipg_document,
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

                var casesCount = result.Entities.Count;

                var documentTypeAbbr = service.Retrieve(document.ipg_DocumentTypeId.LogicalName, document.ipg_DocumentTypeId.Id, new ColumnSet(ipg_documenttype.Fields.ipg_DocumentTypeAbbreviation))
                    .ToEntity<ipg_documenttype>().ipg_DocumentTypeAbbreviation;

                if (!multipleAssociationAbbreviations.Contains(documentTypeAbbr) && casesCount > 1)
                    throw new ArgumentException("Only one case can be associate to the document of " + documentTypeAbbr + " type!");
            }
        }

        private void PreOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;

            var document = localPluginContext.Target<ipg_document>();

            localPluginContext.Trace($"Case Id is {document.ipg_CaseId}");

            if (document.Contains(ipg_document.Fields.ipg_DocumentTypeId) && document.Contains(ipg_document.Fields.ipg_CaseId))
            {
                var documentType = service.Retrieve(document.ipg_DocumentTypeId.LogicalName, document.ipg_DocumentTypeId.Id, new ColumnSet(ipg_documenttype.Fields.ipg_name)).ToEntity<ipg_documenttype>();
                if (documentType.ipg_name.Contains("Invoice") && !CaseHasAssociatedParts(document.ipg_CaseId, service))
                {
                    throw new InvalidPluginExecutionException("Case does not have associated parts!");
                }
            }

            BindDocToPatient(document, document.ipg_CaseId, service, localPluginContext.TracingService);
        }
      
        private void BindDocToPatient(ipg_document doc, EntityReference caseRef, IOrganizationService service, ITracingService tracingService)
        {
            if (caseRef != null)
            {
                var patientId = service.Retrieve(caseRef.LogicalName, caseRef.Id, new ColumnSet(Incident.Fields.ipg_PatientId)).ToEntity<Incident>().ipg_PatientId;
                tracingService.Trace($"patientColumn {Incident.Fields.ipg_PatientId}; id : {patientId?.Id}");
                if (patientId != null)
                {
                    doc.ipg_patientid = patientId;
                }
            }
        }

        private void PreOperationUpdateHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;

            var targetDocument = localPluginContext.Target<ipg_document>();
            var preImageDocument = localPluginContext.PreImage<ipg_document>();

            var documentTypeRef = targetDocument.Contains(ipg_document.Fields.ipg_DocumentTypeId) ? targetDocument.ipg_DocumentTypeId : preImageDocument.ipg_DocumentTypeId;
            var caseRef = targetDocument.Contains(ipg_document.Fields.ipg_CaseId) ? targetDocument.ipg_CaseId : preImageDocument.ipg_CaseId;

            if (documentTypeRef != null && caseRef != null)
            {
                var documentType = service.Retrieve(documentTypeRef.LogicalName, documentTypeRef.Id, new ColumnSet(ipg_documenttype.Fields.ipg_name)).ToEntity<ipg_documenttype>();
                if (documentType.ipg_name.Contains("Invoice") && !CaseHasAssociatedParts(caseRef, service))
                {
                    throw new Exception("Case does not have associated parts!");
                }
            }

            BindDocToPatient(targetDocument, targetDocument.ipg_CaseId, service, localPluginContext.TracingService);
        }

        private bool CaseHasAssociatedParts(EntityReference caseRef, IOrganizationService service)
        {
            var crmContext = new OrganizationServiceContext(service);
            var parts = (from part in crmContext.CreateQuery<ipg_casepartdetail>()
                         where part.ipg_caseid.Id == caseRef.Id
                         select part).ToList();

            if (parts.Count > 0)
            {
                return true;
            }
            return false;
        }

        private void PostOperationCreateOrUpdateHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;

            var document = localPluginContext.Target<ipg_document>();
            var preDocument = localPluginContext.PreImage<ipg_document>();

            localPluginContext.Trace("Testing if Document type and Case are present to process DocumentRequired tasks");
            if (document.ipg_DocumentTypeId != null && document.ipg_CaseId != null)
            {
                var documentType = service.Retrieve(
                    ipg_documenttype.EntityLogicalName,
                    document.ipg_DocumentTypeId.Id,
                    new ColumnSet(ipg_documenttype.PrimaryNameAttribute)
                    )
                    .ToEntity<ipg_documenttype>();

                localPluginContext.Trace("Processing tasks for required case documents");
                ProcessTasks(service, document.ipg_CaseId, documentType, localPluginContext.TracingService);

                var reviewStatus = (document.ipg_SourceEnum == ipg_DocumentSourceCode.Portal
                        ? ipg_document_ipg_ReviewStatus.PendingReview
                        : ipg_document_ipg_ReviewStatus.Approved)
                    .ToOptionSetValue();

               SetDocumentReviewStatus(service, document.Id, reviewStatus);
            }

            localPluginContext.Trace("Keep Reference to old Case");

            if (context.MessageName == MessageNames.Update &&
                document.Contains(ipg_document.Fields.ipg_CaseId) &&
                preDocument?.ipg_CaseId != null &&
                preDocument.ipg_CaseId.Id != document.ipg_CaseId?.Id)
            {
                service.Associate(document.LogicalName,
                    document.Id,
                    new Relationship(ipg_incident_ipg_document.EntitySchemaName),
                    new EntityReferenceCollection() { preDocument.ipg_CaseId });
            }
        }
        private void SetDocumentReviewStatus(IOrganizationService orgService, Guid documentId, OptionSetValue reviewStatus)
        {
            orgService.Update(new ipg_document()
            {
                Id = documentId,
                ipg_ReviewStatus = reviewStatus
            });
        }
        private void ProcessTasks(IOrganizationService organizationService, EntityReference caseReference, ipg_documenttype documentType, ITracingService tracingService)
        {
            tracingService.Trace("Check if the document type is generic");
            if ((documentType.ipg_name ?? "").ToLower().Contains("generic"))
            {
                return; //do not process tasks of generic documents
            }

            var orgServiceContext = new OrganizationServiceContext(organizationService);
            var openTasks = (from task in orgServiceContext.CreateQuery<Task>()
                             where task.ipg_DocumentType.Id == documentType.Id
                                && task.StateCode == TaskState.Open
                                && task.RegardingObjectId.Id == caseReference.Id
                             select task).ToList();
            if (openTasks.Any())
            {
                tracingService.Trace("Completing open DocumentRequired tasks");
                foreach (var openTask in openTasks)
                {
                    organizationService.Update(new Task
                    {
                        Id = openTask.Id,
                        ScheduledEnd = DateTime.Today,
                        OwnerId = openTask.CreatedBy,
                        StateCode = TaskState.Completed,
                        StatusCode = Task_StatusCode.Resolved.ToOptionSetValue()
                    });
                }
            }
            else
            {
                tracingService.Trace("Creating a new DocumentRequired task");
                var newTask = new Task()
                {
                    //todo: ask if need a category here
                    RegardingObjectId = caseReference,
                    ipg_caseid = caseReference,
                    ipg_DocumentType = documentType.ToEntityReference(),
                    Subject = documentType.ipg_name + " Document Required",
                    Subcategory = documentType.ipg_name,
                    ScheduledStart = DateTime.Today,
                    ScheduledEnd = DateTime.Today
                };
                var newTaskId = organizationService.Create(newTask);

                organizationService.Update(new Task()
                {
                    Id = newTaskId,
                    StateCode = TaskState.Completed,
                    StatusCode = Task_StatusCode.Resolved.ToOptionSetValue()
                });
            }
        }
    }
}
