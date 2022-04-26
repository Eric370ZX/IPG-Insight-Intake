using Insight.Intake.Extensions;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.CodeActivities
{
    public class AttachBVFasPDFDoc : WorkFlowActivityBase
    {
        public AttachBVFasPDFDoc() : base(typeof(AttachBVFasPDFDoc)) { }
        //[Input("BVF")]
        //[ReferenceTarget("ipg_benefitsverificationform")]
        //[RequiredArgument]
        //public InArgument<EntityReference> initBvfRef { get; set; }

        [Input("Document type")]
        [RequiredArgument]
        public InArgument<string> initDcoumentTypeAbbr { get; set; }
        protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
        {
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            //IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService crmService = localContext.OrganizationService; //serviceFactory.CreateOrganizationService(Guid.Empty);
            var traceService = context.GetExtension<ITracingService>();
            traceService.Trace($"Start"); 
            //var bvfRef = initBvfRef.Get(context);
            var docType = initDcoumentTypeAbbr.Get(context);
            var pdfManager = new PdfManager(crmService, traceService);
            var bvfManager = new BvfManager(crmService, traceService);
            var template=pdfManager.GetTemplate("BVF_Form");
            traceService.Trace($"Primary entity: {workflowContext.PrimaryEntityName}, {workflowContext.PrimaryEntityId}");
            var targetBVF = crmService.Retrieve(workflowContext.PrimaryEntityName, workflowContext.PrimaryEntityId, new ColumnSet(true));
            var utcNowDateTime = DateTime.UtcNow;
            var localDateTime = crmService.ConvertToLocalTime(workflowContext.InitiatingUserId, utcNowDateTime);
            var mapFields = bvfManager.GetMappedFields(targetBVF, localDateTime, (localDateTime - utcNowDateTime));
            traceService.Trace($"Fields were mapped"); 
            var mergedDoc = pdfManager.Merge(template, targetBVF, mapFields);
            traceService.Trace($"Document was merged");
            pdfManager.CreateDocument(mergedDoc, targetBVF.GetAttributeValue<EntityReference>("ipg_parentcaseid"), $"{docType}.pdf", docType);
            traceService.Trace($"Document was created");
        }
    }
}
