using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using Microsoft.Xrm.Sdk.Query;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Microsoft.Crm.Sdk.Messages;

namespace Insight.Intake.Plugin.Document
{
    public class MergeDocumentsPlugin : IPlugin
    {
        private static class InputParamentersNames
        {
            public static readonly string CaseIdParameterName = "CaseId";
            public static readonly string PackageNameInputParameterName = "PackageName";
            public static readonly string DocIdsInputParameterName = "DocIds";
            public static readonly string DocTypeIdInputParameterName = "TypeId";
            public static readonly string DocDescriptionInputParameterName = "Description";
        }

        public static readonly int MaxNumberOfRanges = 10;
        public static readonly int MaxDocNameLength = 100;
        private static readonly string LogisticsPackageDocumentTypeAbbreviation = "PPP";
        private static readonly int CreatePostProcedurePacketTaskTypeOSV = 427880029;

        public void Execute(IServiceProvider serviceProvider)
        {
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            tracingService.Trace($"{this.GetType()} Execute method started");

            tracingService.Trace("Getting plugin execution context");
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            try
            {
                tracingService.Trace("Getting Organization service");
                var organizationService = serviceProvider.GetOrganizationService(context.UserId);

                tracingService.Trace("Getting CaseId argument");
                EntityReference caseEntityReference = (EntityReference)context.InputParameters[InputParamentersNames.CaseIdParameterName];
                if (caseEntityReference == null)
                {
                    throw new Exception("Case ID is required");
                }

                tracingService.Trace("Getting Package name argument");
                string packageName = (string)context.InputParameters[InputParamentersNames.PackageNameInputParameterName];

                tracingService.Trace("Parsing Document IDs argument");
                Guid[] docIds = ParseDocIds((string)context.InputParameters[InputParamentersNames.DocIdsInputParameterName]);

                Guid.TryParse(context.InputParameters[InputParamentersNames.DocTypeIdInputParameterName].ToString(), out var docTypeId);
                string description = context.InputParameters[InputParamentersNames.DocDescriptionInputParameterName].ToString();

                tracingService.Trace("Assembling a package");
                Assemble(caseEntityReference.Id, packageName, docIds, docTypeId, description, organizationService, tracingService);

                tracingService.Trace($"{this.GetType()} Execute method finished");
            }
            catch (FaultException<OrganizationServiceFault> faultException)
            {
                throw new InvalidPluginExecutionException($"An error occurred in {this.GetType().Name}.", faultException);
            }
            catch (Exception exception)
            {
                tracingService.Trace($"{this.GetType().Name}: {exception}");
                throw;
            }
        }

        private Guid[] ParseDocIds(string docIdsString)
        {
            if (string.IsNullOrWhiteSpace(docIdsString) == false)
            {
                string[] docIdsStringArray = docIdsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (docIdsStringArray.Length > 0)
                {
                    var guids = new List<Guid>();

                    foreach (string docIdString in docIdsStringArray)
                    {
                        guids.Add(Guid.Parse(docIdString));
                    }

                    return guids.ToArray();
                }
            }

            return new Guid[] { };
        }

        private void Assemble(Guid caseId, string packageName, Guid[] docIds, Guid docTypeId, string description, IOrganizationService organizationService, ITracingService tracingService)
        {
            using (PdfDocument outputDocument = new PdfDocument())
            {
                tracingService.Trace("Setting PDF document properties");
                outputDocument.Info.Title = "Logistics Package";

                tracingService.Trace("Enumerating doc IDs");
                foreach (Guid docId in docIds)
                {
                    Annotation pdfDocAnnotation = GetPdfDocument(organizationService, docId, tracingService);

                    tracingService.Trace("Converting base64 to bytes");
                    byte[] pdfDocBytes = Convert.FromBase64String(pdfDocAnnotation.DocumentBody);
                    using (var pdfMemoryStream = new MemoryStream(pdfDocBytes))
                    {
                        tracingService.Trace("Opening PDF document");
                        using (PdfDocument inputDocument = PdfReader.Open(pdfMemoryStream, PdfDocumentOpenMode.Import))
                        {
                            for (int i = 0; i < inputDocument.PageCount; i++)
                            {
                                tracingService.Trace("Adding a PDF page");
                                outputDocument.AddPage(inputDocument.Pages[i]);
                            }
                        }
                    }
                }

                DateTime originalDocDate = DateTime.Now;

                SavePdfDocument(organizationService, tracingService, caseId, packageName, outputDocument, originalDocDate, docTypeId, description);
            }
        }

        private Annotation GetPdfDocument(IOrganizationService organizationService, Guid docId, ITracingService tracingService)
        {
            var queryExpression = new QueryExpression(Annotation.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(Annotation.AnnotationId).ToLower(), nameof(Annotation.FileName).ToLower()),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(Annotation.ObjectId).ToLower(), ConditionOperator.Equal, docId),
                            new ConditionExpression(nameof(Annotation.FileName).ToLower(), ConditionOperator.EndsWith, ".pdf") //this is case-insensitive
                        }
                },
                TopCount = 1
            };

            tracingService.Trace("Retrieving Annotations for docId=" + docId);
            EntityCollection notes = organizationService.RetrieveMultiple(queryExpression);
            Entity noteWithPdf = notes.Entities.FirstOrDefault();
            if (noteWithPdf == null)
            {
                throw new Exception("Could not find a PDF file");
            }

            //we need to retrieve DocumentBody using Retrieve method
            tracingService.Trace("Retrieving Annotation with id=" + noteWithPdf.Id);
            noteWithPdf = organizationService.Retrieve(Annotation.EntityLogicalName, noteWithPdf.Id, new ColumnSet(true));
            if (noteWithPdf == null)
            {
                throw new Exception("Could not retrieve the the PDF file");
            }

            return noteWithPdf.ToEntity<Annotation>();
        }

        private void SavePdfDocument(
            IOrganizationService organizationService, ITracingService tracingService,
            Guid caseId, string packageName, PdfDocument pdfDocument,
            DateTime originalDocDate, Guid docTypeId, string description = null)
        {
            string outputBase64;

            tracingService.Trace("Creating base64 content for the document assembly");
            using (var outputMemoryStream = new MemoryStream())
            {
                pdfDocument.Save(outputMemoryStream, false);
                outputBase64 = Convert.ToBase64String(
                    outputMemoryStream.GetBuffer(), 0, (int)outputMemoryStream.Length);
                outputMemoryStream.Close();
                pdfDocument.Close();
            }

            tracingService.Trace("Creating base64 content for the document assembly");
            ipg_documenttype assemblyDocType = GetDocumentTypeByAbbreviation(organizationService, LogisticsPackageDocumentTypeAbbreviation);
            if (assemblyDocType == null)
            {
                throw new Exception($"Could not find 'Post Procedure Packet' (abbreviation={LogisticsPackageDocumentTypeAbbreviation}) document type");
            }

            if (!packageName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                packageName += ".pdf";
            }

            tracingService.Trace("Retrieving existing document of a same type");
            var PPPdocument = GetCaseDocumentByType(organizationService, caseId, assemblyDocType.Id);
            if (PPPdocument != null)
            {
                tracingService.Trace("Deleting  document with id: " + PPPdocument.Id);
                organizationService.Delete(PPPdocument.LogicalName, PPPdocument.Id);
            }

            tracingService.Trace("Creating a new Document entity");
            var newDocument = new ipg_document()
            {
                ipg_CaseId = new EntityReference(Incident.EntityLogicalName, caseId),
                //ipg_name is generated by RenameDocumentWhenAttachedToCase
                ipg_FileName = packageName,
                ipg_DocumentTypeId = docTypeId != Guid.Empty ? new EntityReference(ipg_documenttype.EntityLogicalName, docTypeId) : new EntityReference(ipg_documenttype.EntityLogicalName, assemblyDocType.Id),
                ipg_Revision = 1,
                ipg_originaldocdate = originalDocDate,
                ipg_numberofpages = pdfDocument.Pages.Count,
                ipg_Description = description
            };
            Guid newDocumentId = organizationService.Create(newDocument);

            tracingService.Trace("Creating a new Annotation entity");
            var newNote = new Annotation
            {
                ObjectId = new EntityReference(ipg_document.EntityLogicalName, newDocumentId),
                FileName = packageName,
                DocumentBody = outputBase64,
                MimeType = "application/pdf",
                IsDocument = true
            };

            organizationService.Create(newNote);

            tracingService.Trace("Retrieving PPPtask");
            var PPPtask = GetCaseOpenTaskByType(organizationService, caseId, CreatePostProcedurePacketTaskTypeOSV);
            if (PPPtask != null)
            {
                tracingService.Trace("Completing PPPtask with id: " + PPPtask.Id);
                SetStateRequest setStateRequest = new SetStateRequest();
                setStateRequest.EntityMoniker = new EntityReference(PPPtask.LogicalName, PPPtask.Id);
                // Set the State and Status OptionSet Values to Cancelled.
                setStateRequest.State = new OptionSetValue(1);
                setStateRequest.Status = new OptionSetValue(5);
                organizationService.Execute(setStateRequest);
            }
        }
        private ipg_document GetCaseDocumentByType(IOrganizationService organizationService, Guid CaseId, Guid DocTypeId)
        {
            var queryExpression = new QueryExpression(ipg_document.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(ipg_document.ipg_name).ToLower()),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(ipg_document.ipg_DocumentTypeId).ToLower(), ConditionOperator.Equal, DocTypeId),
                            new ConditionExpression(nameof(ipg_document.ipg_CaseId).ToLower(), ConditionOperator.Equal, CaseId)
                        }
                },
                TopCount = 1
            };

            EntityCollection entities = organizationService.RetrieveMultiple(queryExpression);
            Entity entity = entities.Entities.FirstOrDefault();
            if (entity != null)
            {
                return entity.ToEntity<ipg_document>();
            }

            return null;
        }

        private Task GetCaseOpenTaskByType(IOrganizationService organizationService, Guid CaseId, int taskType)
        {
            var queryExpression = new QueryExpression(Task.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(Task.Subject).ToLower()),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(Task.StateCode).ToLower(), ConditionOperator.Equal, 0),
                            new ConditionExpression(nameof(Task.ipg_tasktypecode).ToLower(), ConditionOperator.Equal, taskType),
                            new ConditionExpression(nameof(Task.RegardingObjectId).ToLower(), ConditionOperator.Equal, CaseId)
                        }
                },
                TopCount = 1
            };

            EntityCollection entities = organizationService.RetrieveMultiple(queryExpression);
            Entity entity = entities.Entities.FirstOrDefault();
            if (entity != null)
            {
                return entity.ToEntity<Task>();
            }

            return null;
        }

        private ipg_documenttype GetDocumentTypeByAbbreviation(IOrganizationService organizationService, string abbreviation)
        {
            var queryExpression = new QueryExpression(ipg_documenttype.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(ipg_documenttype.ipg_name).ToLower()),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(ipg_documenttype.ipg_DocumentTypeAbbreviation).ToLower(), ConditionOperator.Equal, abbreviation),
                        }
                },
                TopCount = 1
            };

            EntityCollection entities = organizationService.RetrieveMultiple(queryExpression);
            Entity entity = entities.Entities.FirstOrDefault();
            if (entity != null)
            {
                return entity.ToEntity<ipg_documenttype>();
            }

            return null;
        }

    }
}