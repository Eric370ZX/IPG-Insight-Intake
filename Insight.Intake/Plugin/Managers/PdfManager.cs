using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using PdfSharp.Pdf.AcroForms;

namespace Insight.Intake.Plugin.Managers
{
    public class PdfManager
    {
        private readonly IOrganizationService crmService;
        private readonly ITracingService traceService;

        public PdfManager(IOrganizationService crmService, ITracingService traceService)
        {
            this.crmService = crmService;
            this.traceService = traceService;
        }

        public byte[] GetTemplate(string settingName)
        {
            var query = new QueryExpression(ipg_globalsetting.EntityLogicalName)
            {
                ColumnSet = new ColumnSet("ipg_name", "ipg_globalsettingid"),
                Criteria = new FilterExpression(LogicalOperator.And)
            };

            query.Criteria.AddCondition("ipg_name", ConditionOperator.Equal, settingName);
            var settings = crmService.RetrieveMultiple(query);

            if (settings.Entities.Count == 0)
            {
                throw new Exception($"Global Setting {settingName} not defined. Please enter setting and add note with Pdf template to it.");
            }
            var settingId = settings.Entities[0].ToEntity<ipg_globalsetting>().Id;

            query = new QueryExpression(Annotation.EntityLogicalName)
            {
                Criteria = new FilterExpression(LogicalOperator.And),
                ColumnSet = new ColumnSet("filename", "filesize", "objecttypecode", "objectid", "mimetype", "documentbody", "annotationid")
            };

            query.Criteria.AddCondition("objectid", ConditionOperator.Equal, settingId);

            var result = crmService.RetrieveMultiple(query);
            if (result.Entities.Count == 0)
            {
                throw new Exception($"Note with Pdf template file doesn't exist: {settingName}");
            }
            var pdfTemplateNote = result.Entities[0].ToEntity<Annotation>();
            var bytes = Convert.FromBase64String(pdfTemplateNote.DocumentBody);
            return bytes;
        }

        public string Merge(byte[] template, Entity targetBVF, IEnumerable<BvfPdfMap> mapFields)
        {
            var base64 = string.Empty;
            using (var ms = new MemoryStream(template))
            {
                using (var pdfDoc = PdfSharp.Pdf.IO.PdfReader.Open(ms, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Modify))
                {
                    var pdfFields = pdfDoc.AcroForm.Fields;
                    var prefix = string.Empty;
                    MapFieldValues(pdfFields, prefix, mapFields);

                    pdfDoc.AcroForm.Elements["/NeedAppearances"] = new PdfSharp.Pdf.PdfBoolean(true);
                    //Save the PDF to the output destination
                    var ms2 = new MemoryStream();
                    pdfDoc.Flatten();
                    pdfDoc.Save(ms2, false);
                    base64 = Convert.ToBase64String(ms2.GetBuffer(), 0, (int)ms2.Length);
                    //context.OutputParameters["FileContent"] = base64;
                    ms2.Close();
                    pdfDoc.Close();
                }
            }
            return base64;
        }

        public void CreateDocument(string fileContent, EntityReference caseRef, string fileName, string documentType)
        {
            using (var trgFile = File.Create(fileName))
            {
                var targContent = Convert.FromBase64String(fileContent); //(ms2.GetBuffer(), 0, (int)ms2.Length);
                trgFile.Write(targContent, 0, targContent.Length);

            }
            var claimDocumentTypeRef = GetDocumentTypeId(documentType);
            var existingDocumentId = GetExistingDocumentId(caseRef.Id, claimDocumentTypeRef.Id);
            if (existingDocumentId != null)
            {
                crmService.Delete(ipg_document.EntityLogicalName, existingDocumentId.Value);
            }

            var document = new ipg_document()
            {
                ipg_CaseId = caseRef,
                ipg_FileName = fileName,
                ipg_name = fileName,
                //ipg_Source = new OptionSetValue((int)ipg_DocumentSourceCode.User),
                ipg_DocumentTypeId = claimDocumentTypeRef
            };

            var documentId = crmService.Create(document);

            var annotation = new Annotation()
            {
                FileName = fileName,
                ObjectId = new EntityReference(ipg_document.EntityLogicalName, documentId),
                DocumentBody = fileContent
            };
            annotation.Id = crmService.Create(annotation);
        }

        private Guid? GetExistingDocumentId(Guid caseId, Guid documentTypeId)
        {
            var query = new QueryExpression(ipg_document.EntityLogicalName)
            {
                ColumnSet = new ColumnSet("ipg_documentid"),
                Criteria = new FilterExpression(LogicalOperator.And),
                TopCount = 1
            };

            query.Criteria.AddCondition("ipg_caseid", ConditionOperator.Equal, caseId);
            query.Criteria.AddCondition("ipg_documenttypeid", ConditionOperator.Equal, documentTypeId);

            var result = crmService.RetrieveMultiple(query);
            if (result.Entities.Count > 0)
            {
                return result.Entities[0].Id;
            }
            else
            {
                return null;
            }
        }

        private EntityReference GetDocumentTypeId(string documentType)
        {
            var query = new QueryExpression(ipg_documenttype.EntityLogicalName)
            {
                ColumnSet = new ColumnSet("ipg_documenttypeid"),
                Criteria = new FilterExpression(LogicalOperator.And),
                TopCount = 1
            };

            query.Criteria.AddCondition("ipg_documenttypeabbreviation", ConditionOperator.Equal, documentType);

            var result = crmService.RetrieveMultiple(query);
            if (result.Entities.Count > 0)
            {
                return new EntityReference(ipg_documenttype.EntityLogicalName, result.Entities[0].Id);
            }
            else
            {
                throw new InvalidPluginExecutionException($"Document Type with name '{documentType}' doesn't exist.");
            }
        }
        private void MapFieldValues(PdfAcroField.PdfAcroFieldCollection pdfFields, string prefix, IEnumerable<BvfPdfMap> pdfData)
        {
            for (int i = 0; i < pdfFields.Count; i++)
            {
                try
                {
                    var pdfField = pdfFields[i];
                    if (pdfField.HasKids)
                    {
                        MapFieldValues(pdfField.Fields, prefix + pdfField.Name + ".", pdfData);
                        continue;
                    }

                    var flag = pdfField.ReadOnly;
                    if (pdfField.ReadOnly)
                    {
                        pdfField.ReadOnly = false;
                    }

                    var fieldValue = pdfData.FirstOrDefault(p => p.PdfField == prefix + pdfField.Name)?.Value;
                    if (fieldValue != null)
                    {
                        if (pdfField is PdfCheckBoxField)
                        {
                            ((PdfCheckBoxField)pdfField).Checked = fieldValue.Equals("Yes");
                        }
                        else if (pdfField is PdfTextField)
                        {
                            pdfField.Value = new PdfSharp.Pdf.PdfString(fieldValue);
                        }
                    }

                    pdfField.ReadOnly = true;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public string MergeFiles(IEnumerable<byte[]> contents)
        {
            var base64 = string.Empty;
            if (contents.Count() > 0)
            {
                var outPdf = new PdfSharp.Pdf.PdfDocument();
                var outMs = new MemoryStream();
                foreach (var content in contents)
                {
                    using (var ms = new MemoryStream(content))
                    {
                        using (var pdfDoc = PdfSharp.Pdf.IO.PdfReader.Open(ms, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import))
                        {
                            for (int i = 0; i < pdfDoc.PageCount; i++)
                            {
                                outPdf.AddPage(pdfDoc.Pages[i]);
                            }
                            pdfDoc.Close();
                        }
                    }
                }
                outPdf.Save(outMs, false);
                base64 = Convert.ToBase64String(outMs.GetBuffer(), 0, (int)outMs.Length);
                outMs.Close();
                outPdf.Close();
            }
            return base64;
        }
    }
}
