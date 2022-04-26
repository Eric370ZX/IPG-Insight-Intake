using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Insight.Intake.Plugin.Document
{
    public class CreateApproveMfgPriceListTask : PluginBase
    {
        public static readonly string TargetInputParameterName = "Target";
        private static readonly int ApproveMfgPriceListTaskTypeValue = 427880053;

        public CreateApproveMfgPriceListTask() : base(typeof(RenameDocumentWhenPropertiesChange))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_document.EntityLogicalName, PostOperationCreateOrUpdateHandler);

            /*On Update of these fields:
            -ipg_documenttypeid
            */
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_document.EntityLogicalName, PostOperationCreateOrUpdateHandler);
        }

        private void PostOperationCreateOrUpdateHandler(LocalPluginContext localPluginContext)
        {
            localPluginContext.TracingService.Trace($"{typeof(CreateApproveMfgPriceListTask)} plugin started");

            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;

            tracingService.Trace("Getting Target input parameter (Document entity reference)");
            var document = ((Entity)context.InputParameters[TargetInputParameterName]).ToEntity<ipg_document>();
            if (document == null || document.LogicalName != ipg_document.EntityLogicalName)
            {
                tracingService.Trace("Target is null or not a document. Return");
                return;
            }

            if(document.ipg_DocumentTypeId == null)
            {
                tracingService.Trace("Doc type is not set. Return");
                return;
            }

            tracingService.Trace("Retrieving Doc Type with id = " + document.ipg_DocumentTypeId.Id);
            var docType = service.Retrieve(ipg_documenttype.EntityLogicalName,
                                           document.ipg_DocumentTypeId.Id,
                                           new ColumnSet(nameof(ipg_documenttype.ipg_DocumentTypeAbbreviation).ToLower()
                                        ))
                                           .ToEntity<ipg_documenttype>();
            if(docType == null)
            {
                throw new InvalidPluginExecutionException("Could not find the requested document with id=" + document.Id);
            }
            if(string.Equals(docType.ipg_DocumentTypeAbbreviation, Constants.DocumentTypeAbbreviations.MfgPriceListDocType, StringComparison.OrdinalIgnoreCase) == false)
            {
                tracingService.Trace("Doc type is not a price list. Return");
                return;
            }

            if (document.ipg_ReviewStatus?.Value != (int)ipg_document_ipg_ReviewStatus.Approved)
            {
                tracingService.Trace("Retrieving existing Approve tasks");
                if (DoesApproveTaskAlreadyExist(service, document.Id))
                {
                    tracingService.Trace("At least one Approve task already exists. Return");
                    return;
                }

                var approvingTeam = MfgPriceListHelper.GetMfgPriceListApproverTeamOrThrow(service, tracingService);

                tracingService.Trace("Create a new Approve Mfg. Price List task");
                Guid newTaskId = service.Create(new Task()
                {
                    RegardingObjectId = new EntityReference(ipg_document.EntityLogicalName, document.Id),
                    ipg_tasktypecode = new OptionSetValue(ApproveMfgPriceListTaskTypeValue),
                    Subject = "Approve uploaded Mfg. Price List",
                    OwnerId = new EntityReference(Team.EntityLogicalName, approvingTeam.Id)
                });
                tracingService.Trace("Created a new Task with id=" + newTaskId);
            }
            
            tracingService.Trace($"{typeof(CreateApproveMfgPriceListTask)} plugin finished");
        }

        private bool DoesApproveTaskAlreadyExist(IOrganizationService organizationService, Guid documentId)
        {
            var existingTasks = organizationService.RetrieveMultiple(new QueryExpression(Task.EntityLogicalName)
            {
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(Task.RegardingObjectId).ToLower(), ConditionOperator.Equal, documentId),
                            new ConditionExpression(nameof(Task.ipg_tasktypecode).ToLower(), ConditionOperator.Equal, ApproveMfgPriceListTaskTypeValue)
                        }
                }
            });
            if (existingTasks.Entities.Count > 0)
            {
                return true;
            }

            return false;
        }
    }
}