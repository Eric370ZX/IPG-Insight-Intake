using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.IO;

namespace Insight.Intake.Plugin.Note
{
    public class SetNoOfPagesOnPDFUpload : PluginBase
    {
        public SetNoOfPagesOnPDFUpload() : base(typeof(SetNoOfPagesOnPDFUpload))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Annotation.EntityLogicalName, PostOperationCreateHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Annotation.EntityLogicalName, PostOperationCreateHandler);
        }

        private void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;

            var target = ((Entity)context.InputParameters["Target"]).ToEntity<Annotation>();
            if (!string.IsNullOrWhiteSpace(target.FileName) && target.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(target.DocumentBody))
            {
                var annotation = service.Retrieve(target.LogicalName, target.Id, new ColumnSet(nameof(Annotation.ObjectTypeCode).ToLower(), nameof(Annotation.ObjectId).ToLower())).ToEntity<Annotation>();
                if (annotation.ObjectTypeCode == ipg_document.EntityLogicalName)
                {
                    try
                    {
                        var document = new ipg_document()
                        {
                            Id = annotation.ObjectId.Id,
                            ipg_numberofpages = GetNoOfPages(target)
                        };
                        service.Update(document);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
        }

        private int GetNoOfPages(Annotation pdfDocAnnotation)
        {
            int NoOfPages = 0;
            byte[] pdfDocBytes = Convert.FromBase64String(pdfDocAnnotation.DocumentBody);

            using (var pdfMemoryStream = new MemoryStream(pdfDocBytes))
            {
                using (PdfDocument inputDocument = PdfReader.Open(pdfMemoryStream, PdfDocumentOpenMode.Import))
                {
                    if (inputDocument != null && inputDocument.Pages.Count != 0)
                    {
                        NoOfPages = inputDocument.PageCount;
                    }
                    inputDocument.Close();
                }
            }
            return NoOfPages;
        }

    }

}
