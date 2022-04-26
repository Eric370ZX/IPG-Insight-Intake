using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.TaskEntity
{
    public class VerifyAllMissingInformationForPoolTask : PluginBase
    {
        public VerifyAllMissingInformationForPoolTask() : base(typeof(VerifyAllMissingInformationForPoolTask))
        {
            RegisterEvent(PipelineStages.PreValidation, MessageNames.Create, Task.EntityLogicalName, PreValidationHandler);
        }

        private void PreValidationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;

            var target = localPluginContext.Target<Task>();
            var caseRef = target.Contains(Task.Fields.RegardingObjectId)
                && target.RegardingObjectId != null
                && target.RegardingObjectId.LogicalName == Incident.EntityLogicalName 
                ? target.RegardingObjectId : null;
            if (caseRef != null && CheckTaskCategoryAndTaskType(target.ipg_taskcategoryid, target.ipg_tasktypeid, service))
            {
                var validationResponse = new List<ValidationResponse>();
                validationResponse.Add(CheckCaseOpenMissingInformationTaks(caseRef, service));
                validationResponse.Add(CheckCaseDocumentsReviewStatus(caseRef, service));
                validationResponse.Add(CheckPostProcedurePacketDocument(caseRef, service));
                validationResponse.Add(CheckCaseActualProcedureDate(caseRef, service));
                if (validationResponse.Any(r => !r.Succeeded))
                {
                    var errorMessage = "Before place Case into 'The Pool' please resolve the following issues:\n" + string.Join("\n", validationResponse.Where(r => !r.Succeeded).Select(r => r.Message));
                    throw new InvalidPluginExecutionException(errorMessage);
                }
            }
        }

        private bool CheckTaskCategoryAndTaskType(EntityReference taskCategoryRef, EntityReference taskTypeRef, IOrganizationService service)
        {
            if (taskCategoryRef != null && taskTypeRef != null)
            {
                var taskCategoryName = taskCategoryRef.Name ?? service.Retrieve(ipg_taskcategory.EntityLogicalName,
                                                                                taskCategoryRef.Id,
                                                                                new ColumnSet(ipg_taskcategory.Fields.ipg_name))
                                                                                .ToEntity<ipg_taskcategory>()
                                                                                .ipg_name;
                var taskTypeName = taskTypeRef.Name ?? service.Retrieve(ipg_tasktype.EntityLogicalName,
                                                                        taskTypeRef.Id,
                                                                        new ColumnSet(ipg_tasktype.Fields.ipg_name))
                                                                        .ToEntity<ipg_tasktype>()
                                                                        .ipg_name;
                return taskCategoryName == Constants.TaskCategoryNames.CaseProcessing
                    && taskTypeName == Constants.TaskTypeNames.RequestToCompleteCaseMgmtWork;
            }
            return false;
        }

        private ValidationResponse CheckCaseOpenMissingInformationTaks(EntityReference caseRef, IOrganizationService service)
        {
            var missingInformationCategory = service.RetrieveMultiple(new QueryExpression(ipg_taskcategory.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(ipg_taskcategory.Fields.Id, ipg_taskcategory.Fields.ipg_name),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_taskcategory.Fields.ipg_name, ConditionOperator.Equal, Constants.TaskCategoryNames.MissingInformation)
                    }
                }
            }).Entities.FirstOrDefault()?.ToEntity<ipg_taskcategory>();

            var openTasks = service.RetrieveMultiple(new QueryExpression(Task.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(Task.Fields.ipg_taskcategoryid, ConditionOperator.Equal, missingInformationCategory.Id),
                        new ConditionExpression(Task.Fields.RegardingObjectId, ConditionOperator.Equal, caseRef.Id),
                        new ConditionExpression(Task.Fields.StateCode, ConditionOperator.Equal, (int)TaskState.Open)
                    }
                }
            }).Entities;

            if (openTasks.Count > 0)
            {
                return new ValidationResponse(false, "There are some open Missing Information tasks in case. ");
            }
            else
            {
                return new ValidationResponse(true);
            }
        }

        private ValidationResponse CheckCaseDocumentsReviewStatus(EntityReference caseRef, IOrganizationService service)
        {
            var caseDocuments = service.RetrieveMultiple(new QueryExpression(ipg_document.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(ipg_document.Fields.ipg_ReviewStatus),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_document.Fields.ipg_CaseId, ConditionOperator.Equal, caseRef.Id)
                    }
                }
            }).Entities;

            if (caseDocuments.All(d => d.ToEntity<ipg_document>().ipg_ReviewStatus.Value == (int)ipg_document_ipg_ReviewStatus.Approved))
            {
                return new ValidationResponse(true);
            }
            else
            {
                return new ValidationResponse(false, "Not all documents have Review Status 'Approved'. ");
            }
        }

        private ValidationResponse CheckPostProcedurePacketDocument(EntityReference caseRef, IOrganizationService service)
        {
            var pppType = service.RetrieveMultiple(new QueryExpression(ipg_documenttype.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(false),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_documenttype.Fields.ipg_DocumentTypeAbbreviation, ConditionOperator.Equal, Constants.DocumentTypeAbbreviations.PostProcedurePacket)
                    }
                }
            }).Entities.FirstOrDefault()?.ToEntity<ipg_documenttype>();

            var pppDocument = service.RetrieveMultiple(new QueryExpression(ipg_document.EntityLogicalName)
            {
                TopCount = 1,
                ColumnSet = new ColumnSet(false),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_document.Fields.ipg_CaseId, ConditionOperator.Equal, caseRef.Id),
                        new ConditionExpression(ipg_document.Fields.ipg_DocumentTypeId, ConditionOperator.Equal, pppType.Id)
                    }
                }
            }).Entities.FirstOrDefault();

            if (pppDocument != null)
            {
                return new ValidationResponse(true);
            }
            else
            {
                return new ValidationResponse(false, "The Post-Procedure Packet document is not generated or attached to case. ");
            }
        }

        private ValidationResponse CheckCaseActualProcedureDate(EntityReference caseRef, IOrganizationService service)
        {
            var incident = service.Retrieve(Incident.EntityLogicalName, caseRef.Id, new ColumnSet(Incident.Fields.ipg_ActualDOS)).ToEntity<Incident>();
            if (incident.ipg_ActualDOS != null)
            {
                return new ValidationResponse(true);
            }
            else
            {
                return new ValidationResponse(false, "Actual Procedure Date should be populated.");
            }
        }
    }
}
