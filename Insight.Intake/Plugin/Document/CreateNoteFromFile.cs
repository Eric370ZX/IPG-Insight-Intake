using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Document
{
    public class CreateNoteFromFile: PluginBase
    {
        public CreateNoteFromFile() : base(typeof(CreateNoteFromFile))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_document.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_document.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;

            var document = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_document>();

            if (document != null && !String.IsNullOrEmpty(document.ipg_documentbody)) {
                var crmContext = new OrganizationServiceContext(service);
                List<Annotation> annotations = (from item in crmContext.CreateQuery<Annotation>()
                                                where (item.ObjectId == document.ToEntityReference())
                                                && (item.IsDocument ?? false)
                                                select item).ToList();
                if (annotations.Count == 0)
                {
                    var annotation = new Annotation();
                    annotation.ObjectId = document.ToEntityReference();
                    annotation.Subject = document.ipg_name;
                    annotation.DocumentBody = document.ipg_documentbody;
                    annotation.MimeType = document.ipg_mimetype;
                    annotation.FileName = document.ipg_FileName;
                    annotation.IsDocument = true;
                    service.Create(annotation);
                }

                var updDocument = new ipg_document()
                {
                    Id = document.Id,
                    ipg_mimetype = string.Empty,
                    ipg_documentbody = string.Empty
                };
                service.Update(updDocument);
            }
        }
    }
}
