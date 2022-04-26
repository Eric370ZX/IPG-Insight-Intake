using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using PdfSharp.Pdf.AcroForms;
using System.Globalization;
using PdfSharp.Pdf.IO;
using System.Linq;
using Microsoft.Xrm.Sdk.Client;
using Insight.Intake.Helpers;

namespace Insight.Intake.Plugin.Note
{   //TODO:Remove from D365 and assembly(the logic moved to d365)
    public class GeneratePDFPatientStatement : PluginBase
    {
        private ITracingService tracingService;

        public GeneratePDFPatientStatement() : base(typeof(GeneratePDFPatientStatement))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Annotation.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                tracingService = localPluginContext.TracingService;
                var target = ((Entity)context.InputParameters["Target"]).ToEntity<Annotation>();
                if(target.ObjectId == null || target.DocumentBody == null || target.ObjectId.LogicalName != ipg_document.EntityLogicalName)
                {
                    return;
                }
                var document = service.Retrieve(ipg_document.EntityLogicalName, target.ObjectId.Id, new ColumnSet(nameof(ipg_document.ipg_DocumentTypeId).ToLower())).ToEntity<ipg_document>();
                if(document.ipg_DocumentTypeId == null)
                {
                    return;
                }
                var documentType = service.Retrieve(ipg_documenttype.EntityLogicalName, document.ipg_DocumentTypeId.Id, new ColumnSet(nameof(ipg_documenttype.ipg_DocumentTypeAbbreviation).ToLower())).ToEntity<ipg_documenttype>();
                if(!String.Equals(documentType.ipg_DocumentTypeAbbreviation ?? "", Constants.DocumentTypeAbbreviations.PatientStatementDocType, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(DecodeStringFromBase64(target.DocumentBody));
                XmlNamespaceManager namespaces = new XmlNamespaceManager(xmlDoc.NameTable);
                namespaces.AddNamespace("ns", "A");
                var node = xmlDoc.SelectSingleNode("/PatientStatementModelBase/ns:Documents", namespaces);
                if (node != null)
                {
                    var templateName = node["TemplateName"].InnerText;
                    var caseNumber = node.SelectSingleNode("ns:Procedure/ns:ProcedureNumber", namespaces).InnerText;
                    var pdfTemplateNote = GetPdfTemplateAnnotation(service, templateName);

                    GenerateStatementForCase(caseNumber, templateName, pdfTemplateNote, node, namespaces, service, tracingService);
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }

        private string GeneratePdf(XmlNode node, XmlNamespaceManager namespaces, string templateName, Annotation pdfTemplateNote, Incident incident, IOrganizationService service)
        {
            var pdfData = new Dictionary<string, string>();
            var pdfBase64 = string.Empty;
            
            GetDataForStatements(node, namespaces, templateName, incident, pdfData, service);
            
            pdfBase64 = WriteToBase64Pdf(pdfData, service, pdfTemplateNote);
            return pdfBase64;
        }

        private void GetDataForStatements(XmlNode node, XmlNamespaceManager namespaces, string templateName, Incident incident, Dictionary<string, string> pdfData, IOrganizationService service)
        {
            if (!string.IsNullOrEmpty(incident.ipg_PatientAddress))
            {
                pdfData.Add("Patient Address 1", incident.ipg_PatientAddress);
                pdfData.Add("Patient Address 2", incident.ipg_PatientAddress);
            }

            var CityAddressState = $"{incident.ipg_PatientCity}/{incident.ipg_PatientState}/{incident.ipg_PatientZipCodeId?.Name}";

            pdfData.Add("City/State/Address 1", CityAddressState);
            pdfData.Add("City/State/Address 2", CityAddressState);

            var patientName = $"{incident.ipg_PatientLastName}, {incident.ipg_PatientFirstName}";
            pdfData.Add("Patient Name 1", patientName.ToUpper());
            pdfData.Add("Patient Name 2", pdfData["Patient Name 1"]);
            pdfData.Add("Patient Name 3", pdfData["Patient Name 1"]);

            pdfData.Add("Procedure Date 1", (incident.ipg_ActualDOS ?? incident.ipg_SurgeryDate)?.ToString("MM.dd.yyyy"));
            pdfData.Add("Procedure Date 2", pdfData["Procedure Date 1"]);

            pdfData.Add("Procedure Name 1", incident.ipg_ProcedureName.ToUpper());
            pdfData.Add("Procedure Name 2", pdfData["Procedure Name 1"]);

            pdfData.Add("Doctor", incident.ipg_PhysicianId?.Name.ToUpper());
            pdfData.Add("Facility", incident.ipg_FacilityId?.Name.ToUpper());

            pdfData.Add("Due Date 1", DateTime.ParseExact(GetValue(node, "ns:DueDate", namespaces), "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString("MM.dd.yyyy"));
            pdfData.Add("Due Date 2", pdfData["Due Date 1"]);

            pdfData.Add("Statement Date 1", GetValue(node, "ns:StatementDate", namespaces).ToUpper());
            pdfData.Add("Statement Date 2", pdfData["Statement Date 1"]);

            pdfData.Add("IPG Account No 1", GetValue(node, "ns:Procedure/ns:ProcedureNumber", namespaces).ToUpper());
            pdfData.Add("IPG Account No 2", pdfData["IPG Account No 1"]);

            pdfData.Add("Total Charges", ToDecimalNullSafe(incident.ipg_BilledCharges?.Value).ToString("C", CultureInfo.CreateSpecificCulture("en-US")));
            pdfData.Add("Ins Adjustments", ToDecimalNullSafe(((decimal?)(incident.ipg_TotalCarrierRespAdjustments?.Value + incident.ipg_TotalCarrierWriteoff?.Value + incident.ipg_TotalSecondaryCarrierRespAdjustments?.Value + incident.ipg_TotalSecondaryCarrierWriteoff?.Value))).ToString("C", CultureInfo.CreateSpecificCulture("en-US")));
            pdfData.Add("Ins Payments", ToDecimalNullSafe(((decimal?)(incident.ipg_TotalReceivedfromCarrier?.Value + incident.ipg_TotalReceivedfromSecondaryCarrier?.Value))).ToString("C", CultureInfo.CreateSpecificCulture("en-US")));
            pdfData.Add("Patient Adjustments", ToDecimalNullSafe(((decimal?)(incident.ipg_TotalPatientRespAdjustments?.Value + incident.ipg_TotalPatientWriteoff?.Value))).ToString("C", CultureInfo.CreateSpecificCulture("en-US")));
            pdfData.Add("Patient Payments", ToDecimalNullSafe(incident.ipg_TotalReceivedFromPatient?.Value).ToString("C", CultureInfo.CreateSpecificCulture("en-US")));

            pdfData.Add("Prior Statement Balance 1", Convert.ToDecimal(GetValue(node, "ns:Procedure/ns:AmountDue", namespaces)).ToString("C", CultureInfo.CreateSpecificCulture("en-US")));
            pdfData.Add("Amount Due 1", Convert.ToDecimal(GetValue(node, "ns:Procedure/ns:AmountDue", namespaces)).ToString("C", CultureInfo.CreateSpecificCulture("en-US")));
            pdfData.Add("Amount Due 2", pdfData["Amount Due 1"]);
            pdfData.Add("Amount Due 3", pdfData["Amount Due 1"]);

            switch (templateName)
            {
                case "A2":
                    {
                        pdfData.Add("0-30 Days", pdfData["Amount Due 1"]);

                        
                        break;
                    }
                case "A3":
                    {
                        pdfData.Add("31-60 Days", pdfData["Amount Due 1"]);

                        break;
                    }
                case "A5":
                    {
                        pdfData.Add("60+ Days", pdfData["Amount Due 1"]);
                        break;
                    }
                case "D1":
                    {
                        pdfData.Add("Est Amount Due 1", (incident.ipg_ActualMemberResponsibility?.Value ?? 0).ToString("C", CultureInfo.CreateSpecificCulture("en-US")));
                        pdfData.Add("Est Amount Due 2", pdfData["Est Amount Due 1"]);
                        pdfData.Add("Est Amount Due 3", pdfData["Est Amount Due 1"]);
                        break;
                    }
            }

            var parts = "";
            var crmContext = new OrganizationServiceContext(service);
            var partNames = (from cpd in crmContext.CreateQuery<ipg_casepartdetail>()
                             join p in crmContext.CreateQuery<Intake.Product>() on cpd.ipg_productid.Id equals p.ProductId
                             where cpd.ipg_caseid.Id == incident.Id && cpd.StateCode == (int)ipg_casepartdetailState.Active
                             select p.Name).ToList();

            foreach (var part in partNames)
            {
                parts += (String.IsNullOrWhiteSpace(parts) ? "" : "\n") + part;
            }

            pdfData.Add("Parts", parts.ToUpper());
        }

        private decimal ToDecimalNullSafe(decimal? value)
        {
            return (value == null ? 0 : (decimal)value);
        }

        private string GetValue(XmlNode node, string xpath, XmlNamespaceManager namespaces)
        {
            return node.SelectSingleNode(xpath, namespaces).InnerText;
        }

        private string WriteToBase64Pdf(Dictionary<string, string> pdfData, IOrganizationService service, Annotation pdfTemplateNote)
        {
            var bytes = Convert.FromBase64String(pdfTemplateNote.DocumentBody);
            var base64 = string.Empty;
            using (var ms = new MemoryStream(bytes))
            {
                using (var pdfDoc = PdfReader.Open(ms, PdfDocumentOpenMode.Modify))
                {
                    var pdfFields = pdfDoc.AcroForm.Fields;
                    var prefix = string.Empty;

                    MapFieldValues(pdfFields, prefix, pdfData);


                    //Save the PDF to the output destination
                    var ms2 = new MemoryStream();
                    pdfDoc.Flatten();
                    pdfDoc.Save(ms2, false);
                    base64 = Convert.ToBase64String(ms2.GetBuffer(), 0, (int)ms2.Length);
                    ms2.Close();
                    pdfDoc.Close();
                }
            }
            return base64;
        }

        private void MapFieldValues(PdfAcroField.PdfAcroFieldCollection pdfFields, string prefix, Dictionary<string, string> pdfData)
        {
            for (int i = 0; i < pdfFields.Count; i++)
            {
                try
                {
                    var pdfField = pdfFields[i];
                    if (pdfField.HasKids)
                    {
                        MapFieldValues(pdfField.Fields, prefix + pdfField.Name, pdfData);
                        continue;
                    }

                    var flag = pdfField.ReadOnly;
                    if (pdfField.ReadOnly)
                    {
                        pdfField.ReadOnly = false;
                    }

                    var fieldValue = pdfData.ContainsKey(prefix + pdfField.Name) ? pdfData[prefix + pdfField.Name] : null;
                    if (fieldValue != null)
                    {
                        if (pdfField is PdfCheckBoxField)
                        {
                            ((PdfCheckBoxField)pdfField).Checked = fieldValue.Equals("Yes");
                        }
                        else if (pdfField is PdfTextField)
                        {
                            pdfField.Value = new PdfSharp.Pdf.PdfString(fieldValue);
                            if (pdfField.Elements.ContainsKey("/NeedAppearances")) {
                                pdfField.Elements["/NeedAppearances"] = new PdfSharp.Pdf.PdfBoolean(true);
                            }
                            else
                            {
                                pdfField.Elements.Add("/NeedAppearances", new PdfSharp.Pdf.PdfBoolean(true));
                            }
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

        private Annotation GetPdfTemplateAnnotation(IOrganizationService service, string templateName)
        {
            var name = templateName + "_PDF_TEMPLATE";
            var query = new QueryExpression(ipg_globalsetting.EntityLogicalName)
            {
                ColumnSet = new ColumnSet("ipg_name", "ipg_globalsettingid"),
                Criteria = new FilterExpression(LogicalOperator.And)
            };

            query.Criteria.AddCondition("ipg_name", ConditionOperator.Equal, name);
            var settings = service.RetrieveMultiple(query);

            if (settings.Entities.Count == 0)
            {
                throw new Exception($"Global Setting {name}. Please enter setting and add note with Pdf template to it.");
            }
            var settingId = settings.Entities[0].ToEntity<ipg_globalsetting>().Id;

            query = new QueryExpression(Annotation.EntityLogicalName)
            {
                Criteria = new FilterExpression(LogicalOperator.And),
                ColumnSet = new ColumnSet("filename", "filesize", "objecttypecode", "objectid", "mimetype", "documentbody", "annotationid")
            };

            query.Criteria.AddCondition("objectid", ConditionOperator.Equal, settingId);

            var result = service.RetrieveMultiple(query);
            if (result.Entities.Count == 0)
            {

                throw new Exception($"Note with Pdf template file doesn't exist: {name}");
            }
            return result.Entities[0].ToEntity<Annotation>();
        }

        private void CreateCaseAnnotation(Guid caseId, string fileContent, string fileName, IOrganizationService service)
        {
            //var claimDocumentTypeRef = GetClaimDocumentTypeId(_claimDocumentTypeName, service);

            var document = new ipg_document()
            {
                ipg_CaseId = new EntityReference(Incident.EntityLogicalName, caseId),
                ipg_FileName = fileName,
                ipg_name = fileName,
                ipg_Source = new OptionSetValue((int)ipg_DocumentSourceCode.User),
                //ipg_DocumentTypeId = claimDocumentTypeRef
            };

            var documentId = service.Create(document);

            var annotation = new Annotation()
            {
                FileName = fileName,
                ObjectId = new EntityReference(ipg_document.EntityLogicalName, documentId),
                DocumentBody = fileContent,
                MimeType = "application/pdf"
            };
            service.Create(annotation);
        }

        private string DecodeStringFromBase64(string stringToDecode)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(stringToDecode));
        }

        private void GenerateStatementForCase(string caseNumber, string templateName, Annotation pdfTemplateNote, XmlNode node, XmlNamespaceManager namespaces, IOrganizationService service, ITracingService tracingService)
        {
            if(pdfTemplateNote == null)
            {
                tracingService.Trace($"Pdf Template not found for {templateName} Statement.");
                return;
            }

            Incident incident = null;

            if (!string.IsNullOrEmpty(caseNumber))
            {
                var query = new QueryExpression(Incident.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(true),
                    TopCount = 1,
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                                {
                                    new ConditionExpression(nameof(Incident.Title).ToLower(), ConditionOperator.Equal, caseNumber)
                                }
                    }
                };

                incident = service.RetrieveMultiple(query).Entities.FirstOrDefault()?.ToEntity<Incident>();
            }

            if (incident == null)
            {
                tracingService.Trace($"Incident with number {caseNumber} Not Found.");
                return;
            }

            var base64 = GeneratePdf(node, namespaces, templateName, pdfTemplateNote, incident, service);

            CreateCaseAnnotation(incident.Id, base64, templateName + ".pdf", service);
        }
    }
}