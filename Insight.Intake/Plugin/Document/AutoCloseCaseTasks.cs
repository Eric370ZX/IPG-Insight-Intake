using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Document
{
    public class AutoCloseCaseTasks : PluginBase
    {
        private  IOrganizationService OrganizationService { get; set; }
        private static readonly Guid MissingInformation = new Guid("7647bc90-4dfe-ea11-a815-000d3a3156c1");

        public AutoCloseCaseTasks() : base(typeof(AutoCloseCaseTasks))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_document.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_document.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext context)
        {
            OrganizationService = context.OrganizationService;

            var document = context.PostImage<ipg_document>();

            if (document.ipg_CaseId != null && document.ipg_DocumentTypeId != null)
            {
                CloseMissingTasksOnCase(document.ipg_CaseId.Id, document.ipg_DocumentTypeId.Id);
            }
        }
        private void CloseMissingTasksOnCase(Guid caseId, Guid documentTypeId)
        {
            var tasks = RetrieveCaseOpenMissingTasks(caseId, documentTypeId);
            if (tasks != null && tasks.Count > 0)
            {
                foreach (var task in tasks)
                {
                    task.ToEntity<Task>().StateCode = TaskState.Completed;
                    task.ToEntity<Task>().StatusCode = new OptionSetValue((int)Task_StatusCode.Resolved);
                    OrganizationService.Update(task);
                }
            }
        }

        private DataCollection<Entity> RetrieveCaseOpenMissingTasks(Guid caseId, Guid documentTypeId)
        {
            var queryExpression = new QueryExpression(Task.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(Task.Fields.ipg_DocumentType),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression(Task.Fields.ipg_caseid, ConditionOperator.Equal, caseId),
                        new ConditionExpression(Task.Fields.ipg_taskcategoryid, ConditionOperator.Equal, MissingInformation),
                        new ConditionExpression(Task.Fields.StateCode, ConditionOperator.Equal, (int)TaskState.Open),
                        new ConditionExpression(Task.Fields.ipg_DocumentType, ConditionOperator.Equal, documentTypeId)
                    }
                }
            };

            return OrganizationService.RetrieveMultiple(queryExpression)?.Entities;
        }
    }
}
