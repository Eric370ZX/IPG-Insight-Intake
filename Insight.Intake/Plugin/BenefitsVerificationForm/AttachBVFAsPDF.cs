using Insight.Intake.Extensions;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.BenefitsVerificationForm
{
    public class AttachBVFAsPDF : PluginBase
    {
        public AttachBVFAsPDF() : base(typeof(AttachBVFAsPDF))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Insight.Intake.ipg_benefitsverificationform.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Insight.Intake.ipg_benefitsverificationform.EntityLogicalName, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var target = localPluginContext.PostImage<ipg_benefitsverificationform>();
            if (target.GetAttributeValue<string>("ipg_templatename") == null)
            {
                return;
            }
            var traceService = localPluginContext.TracingService;
            var crmService = localPluginContext.OrganizationService;
            traceService.Trace($"Start");
            //var bvfRef = initBvfRef.Get(context);
            var docType = target.GetAttributeValue<string>("ipg_templatename");
            var pdfManager = new PdfManager(crmService, traceService);
            var bvfManager = new BvfManager(crmService, traceService);
            var template = pdfManager.GetTemplate("BVF_Form");
            traceService.Trace($"Primary entity: {target.LogicalName}, {target.Id}");
            var targetBVF = crmService.Retrieve(target.LogicalName, target.Id, new ColumnSet(true));
            var utcNowDateTime = DateTime.UtcNow;
            var localDateTime = crmService.ConvertToLocalTime(localPluginContext.PluginExecutionContext.InitiatingUserId, utcNowDateTime);
            var mapFields = bvfManager.GetMappedFields(targetBVF, localDateTime, (localDateTime - utcNowDateTime));
            traceService.Trace($"Fields were mapped");
            var mergedDoc = pdfManager.Merge(template, targetBVF, mapFields);
            traceService.Trace($"Document was merged");
            pdfManager.CreateDocument(mergedDoc, targetBVF.GetAttributeValue<EntityReference>("ipg_parentcaseid"), $"Manual {docType} {localDateTime.ToShortDateString()}.pdf", docType);
            traceService.Trace($"Document was created");
        }
    }
}
