using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Note
{
    public class CreateDocumentFromNote : PluginBase
    {
        private const string MissingInformationTaskCategoryName = "Missing Information";
        public CreateDocumentFromNote() : base(typeof(CreateDocumentFromNote))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Annotation.EntityLogicalName, PostOperationCreateHandler);
        }
        private void PostOperationCreateHandler(LocalPluginContext localContext)
        {
            var service = localContext.OrganizationService;
            var note = localContext.Target<Annotation>();

            CreateDocumentIfTask(note, service);
        }
        private void CreateDocumentIfTask(Annotation note, IOrganizationService service)
        {
            if (note != null &&
                note.Contains(Annotation.Fields.ObjectId) &&
                note.ObjectTypeCode == Task.EntityLogicalName &&
                note.IsDocument.HasValue && 
                note.IsDocument.Value)
            {
                CreateDocumentForTask(note, service);
                CompleteTask(service, note.ObjectId.Id);
            }
        }

        private void CompleteTask(IOrganizationService service, Guid taskId)
        {
            Task task = new Task()
            {
                Id = taskId,
                ipg_addeddocument = true,
                StateCode = TaskState.Completed,
                StatusCode = new OptionSetValue((int)Task_StatusCode.Resolved)
            };
            service.Update(task);
        }
        private static void CreateDocumentForTask(Annotation note, IOrganizationService service)
        {
            var task = service.Retrieve(Task.EntityLogicalName, note.ObjectId.Id,
                    new ColumnSet(
                     Task.Fields.ipg_caseid,
                     Task.Fields.ipg_DocumentType,
                     Task.Fields.Subcategory,
                     Task.Fields.ipg_CaseInfo,
                     Task.Fields.ipg_taskcategoryid))
                    .ToEntity<Task>();

            if (!string.IsNullOrEmpty(task.Subcategory))
            {
                var docType = service.RetrieveMultiple(new QueryExpression(ipg_documenttype.EntityLogicalName)
                {
                    TopCount = 1,
                    ColumnSet = new ColumnSet(ipg_documenttype.Fields.ipg_DocumentCategoryTypeId),
                    Criteria = new FilterExpression()
                    {
                        Conditions =
                    {
                        new ConditionExpression(ipg_documenttype.Fields.ipg_DocumentTypeAbbreviation, ConditionOperator.Equal,  task.Subcategory)
                    }
                    }
                }).Entities.FirstOrDefault()?.ToEntity<ipg_documenttype>();

                if (task.ipg_caseid != null && note.DocumentBody != null && docType != null)
                {
                    
                    ipg_document document = new ipg_document()
                    {

                        ipg_CaseId = task.ipg_caseid,
                        ipg_name = note.FileName,
                        ipg_documentbody = note.DocumentBody,
                        ipg_documenttypecategoryid = docType?.ipg_DocumentCategoryTypeId,
                        ipg_DocumentTypeId = docType?.ToEntityReference(),
                        ipg_Revision = 1,
                        CreatedOn = task.CreatedOn,
                        ipg_ReviewStatus = new OptionSetValue((int)ipg_document_ipg_ReviewStatus.PendingReview),
                        ipg_FileName = note.FileName,
                        ipg_originatingtaskid = task.ToEntityReference()
                    };
                    if (!string.IsNullOrEmpty(note.NoteText) && note.NoteText.StartsWith("*WEB*"))
                    {
                        document.ipg_Source = new OptionSetValue((int)ipg_DocumentSourceCode.Portal);
                    }

                    service.Create(document);
                }
            }
        }
    }
}
