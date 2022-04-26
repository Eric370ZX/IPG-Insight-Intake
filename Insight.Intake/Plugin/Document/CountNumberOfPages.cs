using Microsoft.Xrm.Sdk;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.IO;

namespace Insight.Intake.Plugin.Document
{
    public class CountNumberOfPages : PluginBase
    {
        public CountNumberOfPages() : base(typeof(CountNumberOfPages))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, ipg_document.EntityLogicalName, PreOperationCreateOrUpdateHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, ipg_document.EntityLogicalName, PreOperationCreateOrUpdateHandler);
        }

        private void PreOperationCreateOrUpdateHandler(LocalPluginContext localPluginContext)
        {
            var targetDoc = (localPluginContext.PluginExecutionContext.InputParameters["Target"] as Entity).ToEntity<ipg_document>();
            if (targetDoc.Contains(ipg_document.Fields.ipg_documentbody) && !string.IsNullOrEmpty(targetDoc.ipg_documentbody))
            {
                if (targetDoc.ipg_FileName.EndsWith(".pdf"))
                {
                    var pdfDocBytes = Convert.FromBase64String(targetDoc.ipg_documentbody);
                    using (var pdfMemoryStream = new MemoryStream(pdfDocBytes))
                    {
                        using (PdfDocument inputDocument = PdfReader.Open(pdfMemoryStream, PdfDocumentOpenMode.ReadOnly))
                        {
                            targetDoc.ipg_numberofpages = inputDocument.PageCount;
                            inputDocument.Close();
                        }
                    }
                }
            }
        }
    }
}
