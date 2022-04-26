using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using Insight.Intake.Extensions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using Microsoft.Xrm.Sdk.Query;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Insight.Intake.Plugin.Document
{
    public class SplitDocumentPlugin : IPlugin
    {
        public static readonly int MaxNumberOfRanges = 10;
        public static readonly string TargetInputParameterName = "Target";
        public static readonly string ReferralIdParameterName = "ReferralId";
        public static readonly string CaseIdParameterName = "CaseId";
        public static readonly string RangeInputParameterNamePrefix = "Range";
        public static readonly string DocTypeInputParameterNamePrefix = "DocTypeId";
        public static readonly string DescriptionInputParameterNamePrefix = "Description";
        public static readonly string NewDocumentIdOutputParameterName = "NewDocumentId";

        private ITracingService _tracingService;
        private IOrganizationService _organizationService;


        public void Execute(IServiceProvider serviceProvider)
        {
            _tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            _tracingService.Trace($"{this.GetType()} Execute method started");

            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            try
            {
                _tracingService.Trace($"Validating {TargetInputParameterName} input parameter");
                if (context.InputParameters.Contains(TargetInputParameterName) == false
                    || !(context.InputParameters[TargetInputParameterName] is EntityReference targetEntityReference)
                    || targetEntityReference.LogicalName != Intake.ipg_document.EntityLogicalName)
                {
                    throw new Exception($"{TargetInputParameterName} entity ipg_document is required");
                }

                _tracingService.Trace("Getting Organization service");
                _organizationService = serviceProvider.GetOrganizationService(context.UserId);

                EntityReference referralEntityReference;
                EntityReference caseEntityReference;
                IList<SplitRange> splitRanges;
                ReadInputParameters(context.InputParameters, out splitRanges,
                    out referralEntityReference, out caseEntityReference);

                _tracingService.Trace("Validating ranges");
                ValidateAndCleanRanges(splitRanges);

                _tracingService.Trace("Getting PDF document");
                Annotation pdfDocAnnotation = GetPdfDocument(targetEntityReference);
                if (pdfDocAnnotation == null)
                {
                    throw new Exception("Could not find the PDF document");
                }

                _tracingService.Trace("Getting Document entity");
                ipg_document documentEntity = _organizationService.Retrieve(ipg_document.EntityLogicalName, targetEntityReference.Id,
                        new ColumnSet(true)).ToEntity<ipg_document>();
                if (documentEntity == null)
                {
                    throw new Exception("Could not find the document entity");
                }

                _tracingService.Trace("Splitting the PDF file");


                Guid? remainingDocumentId = SplitDocument(documentEntity, pdfDocAnnotation, splitRanges, referralEntityReference, caseEntityReference);
                context.OutputParameters[NewDocumentIdOutputParameterName] = remainingDocumentId;

                _tracingService.Trace("Deactivating the original document");
                UpdateOriginalDocReviewStatus(documentEntity, ipg_document_ipg_ReviewStatus.Used);

                _tracingService.Trace("SplitDocumentPlugin finished");
            }
            catch (FaultException<OrganizationServiceFault> faultException)
            {
                _tracingService.Trace($"Error occured {faultException?.Message}");
                throw new InvalidPluginExecutionException($"An error occurred in {this.GetType().Name}.", faultException);
            }
            catch (Exception exception)
            {
                _tracingService.Trace($"{this.GetType().Name}: {exception}");
                throw;
            }
        }

        private void ReadInputParameters(ParameterCollection inputParameters, out IList<SplitRange> splitRanges,
            out EntityReference referralEntityReference, out EntityReference caseEntityReference)
        {
            inputParameters.TryGetValue(ReferralIdParameterName, out var referralReference);
            referralEntityReference = (EntityReference)referralReference ?? null;
            _tracingService.Trace($"Getting Referral reference: {referralEntityReference?.Id}");

            inputParameters.TryGetValue(CaseIdParameterName, out var caseReference);
            caseEntityReference = (EntityReference)caseReference ?? null;
            _tracingService.Trace($"Getting Case reference {caseEntityReference?.Id}");

            _tracingService.Trace("Parsing ranges");
            splitRanges = ParseRanges(inputParameters);
        }

        private IList<SplitRange> ParseRanges(ParameterCollection inputParameters)
        {
            var ranges = new List<SplitRange>();
            for (int i = 0; i < MaxNumberOfRanges; i++)
            {
                string rangeParamKey = RangeInputParameterNamePrefix + i;
                if (inputParameters.ContainsKey(rangeParamKey))
                {
                    string rangeI = (string)inputParameters[rangeParamKey];
                    if (string.IsNullOrWhiteSpace(rangeI) == false)
                    {
                        int startPage;
                        int endpage;
                        SplitRange.ParseRange(rangeI, out startPage, out endpage);

                        EntityReference docTypeReference = (EntityReference)inputParameters[DocTypeInputParameterNamePrefix + i];
                        if (docTypeReference == null)
                        {
                            throw new Exception("Doc type ID is required for " + rangeParamKey);
                        }

                        string description = (string)inputParameters[DescriptionInputParameterNamePrefix + i];

                        ranges.Add(new SplitRange(startPage, endpage, docTypeReference.Id, description));
                    }
                }
            }

            return ranges;
        }

        private void ValidateAndCleanRanges(IList<SplitRange> splitRanges)
        {
            foreach (var splitRange in splitRanges)
            {
                if (splitRange.DocumentTypeId.HasValue || splitRange.StartPage.HasValue || splitRange.EndPage.HasValue)
                {
                    if (splitRange.StartPage.HasValue == false)
                    {
                        throw new Exception("Start page number is required");
                    }
                    if (splitRange.StartPage < 1)
                    {
                        throw new Exception("Start page number cannot be less than 1");
                    }
                    if (splitRange.EndPage.HasValue && splitRange.EndPage < splitRange.StartPage)
                    {
                        throw new Exception("End page number cannot be less than start page number");
                    }

                    if (splitRange.DocumentTypeId.HasValue == false)
                    {
                        throw new Exception("Document type is required ");
                    }
                }
                else
                {
                    splitRanges.Remove(splitRange);
                }
            }

            if (splitRanges.Count == 0)
            {
                throw new Exception("At least 1 range is required");
            }
        }

        private Annotation GetPdfDocument(EntityReference documentEntityReference)
        {
            var queryExpression = new QueryExpression(Annotation.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(Annotation.AnnotationId).ToLower(), nameof(Annotation.FileName).ToLower()),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(Annotation.ObjectId).ToLower(), ConditionOperator.Equal, documentEntityReference.Id),
                            new ConditionExpression(nameof(Annotation.FileName).ToLower(), ConditionOperator.EndsWith, ".pdf") //this is case-insensitive
                        }
                },
                TopCount = 1
            };
            EntityCollection notes = _organizationService.RetrieveMultiple(queryExpression);
            Entity noteWithPdf = notes.Entities.FirstOrDefault();
            if (noteWithPdf == null)
            {
                throw new Exception("Could not find a PDF file");
            }

            //we need to retrieve DocumentBody using Retrieve method
            noteWithPdf = _organizationService.Retrieve(Annotation.EntityLogicalName, noteWithPdf.Id, new ColumnSet(true));
            if (noteWithPdf == null)
            {
                throw new Exception("Could not retrieve the the PDF file");
            }

            return noteWithPdf.ToEntity<Annotation>();
        }

        private Guid? SplitDocument(ipg_document documentEntity, Annotation pdfDocAnnotation, IEnumerable<SplitRange> splitRanges,
            EntityReference referralEntityReference, EntityReference incidentEntityReference)
        {
            byte[] pdfDocBytes = Convert.FromBase64String(pdfDocAnnotation.DocumentBody);
            using (var pdfMemoryStream = new MemoryStream(pdfDocBytes))
            {
                _tracingService.Trace("Opening PDF document");
                using (PdfDocument inputDocument = PdfReader.Open(pdfMemoryStream, PdfDocumentOpenMode.Import))
                {
                    CreateSplitDocuments(documentEntity, pdfDocAnnotation, splitRanges, referralEntityReference, incidentEntityReference, inputDocument);

                    Guid? newRemainingDocumentId = CreateRemainingDocument(documentEntity, pdfDocAnnotation, splitRanges, inputDocument);

                    inputDocument.Close();

                    return newRemainingDocumentId;
                }
            }
        }

        private void CreateSplitDocuments(ipg_document documentEntity, Annotation pdfDocAnnotation,
            IEnumerable<SplitRange> splitRanges, EntityReference referralEntityReference, EntityReference incidentEntityReference,
            PdfDocument inputDocument)
        {
            _tracingService.Trace("Creating split PDF documents");

            foreach (var splitRange in splitRanges)
            {
                using (PdfDocument outputDocument = new PdfDocument())
                {
                    _tracingService.Trace("Copying PDF document properties");

                    outputDocument.Info.Title = inputDocument.Info.Title;
                    if (string.IsNullOrWhiteSpace(outputDocument.Info.Title) == false)
                    {
                        outputDocument.Info.Title += ". ";
                    }
                    outputDocument.Info.Title += $"Page range: {splitRange.StartPage}-{splitRange.EndPage}";
                    outputDocument.Info.Creator = inputDocument.Info.Creator;

                    _tracingService.Trace("Getting PDF pages");
                    for (int i = splitRange.StartPage.Value; i <= (splitRange.EndPage ?? splitRange.StartPage.Value) && i <= inputDocument.PageCount; i++)
                    {
                        outputDocument.AddPage(inputDocument.Pages[i - 1]);
                    }

                    SavePdfDocument(pdfDocAnnotation.FileName, outputDocument, documentEntity,
                        splitRange.DocumentTypeId.Value, splitRange.Description, referralEntityReference,
                        incidentEntityReference);
                }
            }
        }

        private Guid? CreateRemainingDocument(ipg_document documentEntity, Annotation pdfDocAnnotation, IEnumerable<SplitRange> splitRanges, PdfDocument inputDocument)
        {
            _tracingService.Trace("Creating a new PDF with remaining pages");

            var remainingPages = new List<int>();
            for (int i = 0; i < inputDocument.Pages.Count; i++)
            {
                int pageNumber = i + 1;
                if (splitRanges.Any(r => pageNumber >= r.StartPage.Value && (r.EndPage.HasValue == false || pageNumber <= r.EndPage.Value)) == false)
                {
                    remainingPages.Add(pageNumber);
                }
            }
            if (remainingPages.Count == 0)
            {
                return null;
            }
            using (PdfDocument outputDocument = new PdfDocument())
            {
                _tracingService.Trace("Copying PDF document properties");
                outputDocument.Info.Title = inputDocument.Info.Title;
                outputDocument.Info.Creator = inputDocument.Info.Creator;

                _tracingService.Trace("Getting PDF pages");
                foreach (var remainingPageNumber in remainingPages)
                {
                    outputDocument.AddPage(inputDocument.Pages[remainingPageNumber - 1]);
                }

                return SavePdfDocument(pdfDocAnnotation.FileName, outputDocument, documentEntity, documentEntity.ipg_DocumentTypeId?.Id,
                    documentEntity.ipg_Description, documentEntity.ipg_ReferralId, documentEntity.ipg_CaseId);
            }
        }

        private Guid SavePdfDocument(string fileName, PdfDocument pdfDocument, ipg_document originalDocumentEntity,
            Guid? documentTypeId, string description, EntityReference referralEntityReference,
            EntityReference incidentEntityReference)
        {
            string base64;
            using (var outputMemoryStream = new MemoryStream())
            {
                pdfDocument.Save(outputMemoryStream, false);
                base64 = Convert.ToBase64String(
                    outputMemoryStream.GetBuffer(), 0, (int)outputMemoryStream.Length);
                outputMemoryStream.Close();
                pdfDocument.Close();
            }

            _tracingService.Trace("Creating a new Document entity");
            var newDocument = new ipg_document();
            CopyDocumentProperties(originalDocumentEntity, newDocument);

            newDocument.ipg_name = fileName; //it should be updated by RenameDocumentWhenAttachedToCase
            newDocument.ipg_FileName = fileName;
            newDocument.ipg_Revision = 1;
            newDocument.ipg_OriginalDocumentId = new EntityReference(ipg_document.EntityLogicalName, originalDocumentEntity.Id);
            newDocument.ipg_originaldocdate = originalDocumentEntity.ipg_originaldocdate;
            newDocument.ipg_Description = description;
            //Set no of pages for the new document
            newDocument.ipg_numberofpages = pdfDocument.Pages.Count;

            if (documentTypeId.HasValue)
            {
                newDocument.ipg_DocumentTypeId = new EntityReference(ipg_documenttype.EntityLogicalName, documentTypeId.Value);
            }
            if (referralEntityReference != null)
            {
                newDocument.ipg_ReferralId = referralEntityReference;
            }
            else if (incidentEntityReference != null)
            {
                newDocument.ipg_CaseId = new EntityReference(Incident.EntityLogicalName, incidentEntityReference.Id);
            }
            SetReviewStatus(newDocument, originalDocumentEntity.ipg_DocumentTypeId);


            Guid newDocumentId = _organizationService.Create(newDocument);

            _tracingService.Trace("Creating a new Annotation entity");
            var newNote = new Annotation
            {
                MimeType = "application/pdf",
                ObjectId = new EntityReference(ipg_document.EntityLogicalName, newDocumentId),
                FileName = fileName,
                DocumentBody = base64,
                IsDocument = true
            };
            _organizationService.Create(newNote);

            return newDocumentId;
        }
        private void SetReviewStatus(ipg_document document, EntityReference originalDocType)
        {
            if (IsDocumentGeneric(document))
            {
                document.ipg_ReviewStatus = new OptionSetValue((int)ipg_document_ipg_ReviewStatus.PendingReview);
                document.ipg_DocumentTypeId = originalDocType;
            }
            else
            {
                document.ipg_ReviewStatus = new OptionSetValue((int)ipg_document_ipg_ReviewStatus.Approved);
            }
        }

        private bool IsDocumentGeneric(ipg_document document) =>
            document.ipg_DocumentTypeId?.Name != null
                ? document.ipg_DocumentTypeId.Name.Contains("Generic")
                : _organizationService.Retrieve(ipg_documenttype.EntityLogicalName, document.ipg_DocumentTypeId.Id, new ColumnSet(ipg_documenttype.Fields.ipg_name)).ToEntity<ipg_documenttype>()
                       .ipg_name.Contains("Generic");


        private void UpdateOriginalDocReviewStatus(ipg_document document, ipg_document_ipg_ReviewStatus status)
        {
            _organizationService.Update(new ipg_document()
            {
                Id = document.Id,
                ipg_ReviewStatus = status.ToOptionSetValue()
            });
        }
        private void CopyDocumentProperties(ipg_document originalDocument, ipg_document newDocument)
        {
            newDocument.ipg_DocumentTypeId = originalDocument.ipg_DocumentTypeId;
            newDocument.ipg_Source = originalDocument.ipg_Source;
            newDocument.ipg_CaseId = originalDocument.ipg_CaseId;
            newDocument.ipg_ReferralId = originalDocument.ipg_ReferralId;
        }

        public class SplitRange
        {
            public SplitRange(int startPage, int endPage, Guid? documentTypeId, string description)
            {
                this.StartPage = startPage;
                this.EndPage = endPage;
                this.DocumentTypeId = documentTypeId;
                this.Description = description;
            }

            public int? StartPage { get; set; }
            public int? EndPage { get; set; }
            public Guid? DocumentTypeId { get; set; }
            public string Description { get; set; }

            public static void ParseRange(string range, out int startPage, out int endPage)
            {
                startPage = endPage = 0;

                range = range.Trim();
                if (string.IsNullOrWhiteSpace(range))
                {
                    return;
                }

                if (range.Contains("-"))
                {
                    string[] numberStrings = range.Split(new[] { '-' });
                    if (numberStrings.Length > 2)
                    {
                        throw new Exception("Page range cannot consist of more than 2 parts");
                    }

                    startPage = int.Parse(numberStrings[0].Trim());

                    if (numberStrings.Length > 1)
                    {
                        endPage = int.Parse(numberStrings[1].Trim());
                    }
                }
                else
                {
                    startPage = endPage = int.Parse(range.Trim());
                }
            }
        }
    }
}