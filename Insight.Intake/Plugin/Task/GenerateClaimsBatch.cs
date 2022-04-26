using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.TaskEntity
{
    public class GenerateClaimsBatch : PluginBase
    {
        public GenerateClaimsBatch() : base(typeof(RunClaimGenerationJob))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGIntakeActionsGenerateClaimsBatch", null, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var crmContext = new OrganizationServiceContext(service);
            var tracingService = localPluginContext.TracingService;

            var task = (EntityReference)context.InputParameters["Target"];
            var taskTypeName = (string)context.InputParameters["TaskType"];
            int taskType = -1;
            if (taskTypeName == Constants.TaskTypeNames.InstitutionalClaimsReadyToPrint)
            {
                taskType = (int)ipg_claim_type.CMS1500;
            }
            else if (taskTypeName == Constants.TaskTypeNames.ProfessionalClaimsReadyToPrint)
            {
                taskType = (int)ipg_claim_type.UB04;
            }

            if (taskType > -1)
            {
                var claimDocumentTypeRef = GetClaimDocumentTypeId(Constants.DocumentTypeNames.CMS1500_UB04, crmContext);

                var documents = (from claim in crmContext.CreateQuery<Invoice>()
                                    join incident in crmContext.CreateQuery<Incident>()
                                    on claim.ipg_caseid.Id equals incident.IncidentId
                                    join document in crmContext.CreateQuery<ipg_document>()
                                    on incident.IncidentId equals document.ipg_CaseId.Id
                                    join annotation in crmContext.CreateQuery<Annotation>()
                                    on document.ipg_documentId equals annotation.ObjectId.Id
                                    where claim.ipg_claimtypecode.Value == taskType
                                        && claim.ipg_WasBatchGenerated == false
                                        && claim.StateCode == InvoiceState.Active
                                        && document.ipg_DocumentTypeId.Id == claimDocumentTypeRef.Id
                                    select annotation.DocumentBody).ToList();

                tracingService.Trace($"Found {documents.Count.ToString()} claims");
                if (documents.Count > 0) {
                    var list = new List<byte[]>();
                    foreach (var document in documents)
                    {
                        list.Add(Convert.FromBase64String(document));
                    }
                    var pdfManager = new PdfManager(service, tracingService);
                    var fileContent = pdfManager.MergeFiles(list);
                    var annotation = new Annotation()
                    {
                        FileName = "Batch.pdf",
                        ObjectId = task,
                        DocumentBody = fileContent,
                        MimeType = "application/pdf"
                    };
                    service.Create(annotation);

                    var claims = (from claim in crmContext.CreateQuery<Invoice>()
                                     join incident in crmContext.CreateQuery<Incident>()
                                     on claim.ipg_caseid.Id equals incident.IncidentId
                                     join document in crmContext.CreateQuery<ipg_document>()
                                     on incident.IncidentId equals document.ipg_CaseId.Id
                                     where claim.ipg_claimtypecode.Value == taskType
                                         && claim.ipg_WasBatchGenerated == false
                                         && claim.StateCode == InvoiceState.Active
                                         && document.ipg_DocumentTypeId.Id == claimDocumentTypeRef.Id
                                     select claim).ToList();
                    foreach(var claim in claims)
                    {
                        var invoice = new Invoice()
                        {
                            Id = claim.Id,
                            ipg_WasBatchGenerated = true
                        };
                        service.Update(invoice);
                    }

                    var request = new SetStateRequest()
                    {
                        EntityMoniker = task,
                        State = new OptionSetValue((int)TaskState.Completed),
                        Status = new OptionSetValue((int)Task_StatusCode.Resolved)
                    };
                    service.Execute(request);

                    context.OutputParameters["PdfFileBase64"] = fileContent;
                }
            }
        }

        private EntityReference GetClaimDocumentTypeId(string claimDocumentTypeName, OrganizationServiceContext crmContext)
        {
            var documentTypes = (from documentType in crmContext.CreateQuery<ipg_documenttype>()
                                where documentType.ipg_name == claimDocumentTypeName
                                select documentType).ToList();
            if (documentTypes.Count > 0)
            {
                return documentTypes.FirstOrDefault().ToEntityReference();
            }
            else
            {
                throw new InvalidPluginExecutionException($"Document Type with name '{claimDocumentTypeName}' doesn't exist.");
            }
        }
    }
}