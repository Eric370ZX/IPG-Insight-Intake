using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;
using Microsoft.Crm.Sdk.Messages;
using Insight.Intake.Helpers;

namespace Insight.Intake.Plugin.TaskEntity
{
    public class CreateDocumentFromTaskNote : PluginBase
    {

        public CreateDocumentFromTaskNote() : base(typeof(CreateDocumentFromTaskNote))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Task.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;               

                var taskEntityTarget = localPluginContext.Target<Task>();
                var taskEntity = localPluginContext.PostImage<Task>();

                if (taskEntity.ipg_reviewstatuscode != null && taskEntity.ipg_reviewstatuscode.Value == (int)Task_ipg_reviewstatuscode.Rejected)
                { 
                   var rejnoteEntity = GetLastTaskNote(service, taskEntity.ToEntityReference());
                    if (rejnoteEntity != null)
                    {
                        var lastAnnotation = rejnoteEntity.ToEntity<Annotation>();
                        var updatedNote =  new Annotation()
                        {
                            Id = lastAnnotation.Id,
                            Attributes = new AttributeCollection()
                        };
                        updatedNote.Subject = "Rejected - " + lastAnnotation.Subject;
                        updatedNote.NoteText = "*WEB*" + "Rejected - " + lastAnnotation.NoteText.Replace("*WEB*","");;
                        service.Update(updatedNote);
                    }
                }

                if (taskEntity.ipg_reviewstatuscode != null && taskEntity.ipg_reviewstatuscode.Value == (int)Task_ipg_reviewstatuscode.Approved)
                {
                    var noteEntity = GetLastTaskNote(service, taskEntity.ToEntityReference());
                    if (noteEntity != null)
                    {
                        var lastAnnotation = noteEntity.ToEntity<Annotation>();
                        var document = new ipg_document();
                        document.ipg_Description = lastAnnotation.NoteText.Replace("*WEB*","");
                        document.ipg_name = lastAnnotation.FileName;
                        document.ipg_FileName = lastAnnotation.FileName;
                        document.ipg_DocumentTypeId = GetPortalGenericDocumentReference(service);

                        if (taskEntity.RegardingObjectId != null && taskEntity.RegardingObjectId.LogicalName == Incident.EntityLogicalName )
                        {
                            document.ipg_CaseId = taskEntity.RegardingObjectId;
                        }

                        Guid documentId = service.Create(document);

                        var documentAnnotation = new Annotation();
                        documentAnnotation.Subject = lastAnnotation.Subject;
                        documentAnnotation.FileName = lastAnnotation.FileName;
                        documentAnnotation.DocumentBody = lastAnnotation.DocumentBody;
                        documentAnnotation.ObjectId = new EntityReference(document.LogicalName, documentId);
                        documentAnnotation.NoteText = lastAnnotation.NoteText;
                        service.Create(documentAnnotation);

                        var setStateRequest = new SetStateRequest()
                        {
                            EntityMoniker = taskEntity.ToEntityReference(),
                            State = new OptionSetValue((int)TaskState.Completed),
                            Status = new OptionSetValue((int)Task_StatusCode.Resolved)
                        };
                        var closeResponse = service.Execute(setStateRequest);
                    }
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }

        private Entity GetLastTaskNote(IOrganizationService service, EntityReference noteER)
        {            
            var QEannotation = new QueryExpression(Annotation.EntityLogicalName);
            QEannotation.TopCount = 1;
            
            QEannotation.ColumnSet.AllColumns = true;
            QEannotation.AddOrder(Annotation.Fields.CreatedOn, OrderType.Descending);

            // Define filter QEannotation.Criteria
            QEannotation.Criteria.AddCondition(Annotation.Fields.ObjectId, ConditionOperator.Equal, noteER.Id.ToString());
            QEannotation.Criteria.AddCondition(Annotation.Fields.IsDocument, ConditionOperator.Equal, true);

            EntityCollection lastTaskNoteEC = service.RetrieveMultiple(QEannotation);
            return lastTaskNoteEC.Entities.FirstOrDefault<Entity>();
        }
        private EntityReference GetPortalGenericDocumentReference(IOrganizationService service)
        {
            QueryExpression ipgGlobalSettingQuery = new QueryExpression(ipg_globalsetting.EntityLogicalName);
            ipgGlobalSettingQuery.TopCount = 1;
            ipgGlobalSettingQuery.ColumnSet.AddColumns(ipg_globalsetting.Fields.ipg_name, ipg_globalsetting.Fields.ipg_value, ipg_globalsetting.Fields.ipg_Type);
            ipgGlobalSettingQuery.Criteria.AddCondition(ipg_globalsetting.Fields.ipg_name, ConditionOperator.Equal, Constants.Settings.PortalGenericDocumentSettings);
            EntityCollection configs = service.RetrieveMultiple(ipgGlobalSettingQuery);

            Entity config = configs.Entities.FirstOrDefault();

            return config == null ?
                null : new EntityReference(config.GetAttributeValue<string>(ipg_globalsetting.Fields.ipg_Type), Guid.Parse(config.GetAttributeValue<string>(ipg_globalsetting.Fields.ipg_value)));
        }
    }
}