using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Insight.Intake.Plugin.Document
{
    public class CreateApprovePriceListUserTask : PluginBase
    {
        public static readonly string TargetInputParameterName = "Target";
        private static readonly int ApproveMfgPriceListTaskTypeValue = 427880053;

        public CreateApprovePriceListUserTask() : base(typeof(RenameDocumentWhenPropertiesChange))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_document.EntityLogicalName, PostOperationCreateOrUpdateHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_document.EntityLogicalName, PostOperationCreateOrUpdateHandler);
        }

        private void PostOperationCreateOrUpdateHandler(LocalPluginContext localPluginContext)
        {
            localPluginContext.TracingService.Trace($"{typeof(CreateApprovePriceListUserTask)} plugin started");

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

            tracingService.Trace("Retrieving existing Approve tasks");
            if (DoesApproveTaskAlreadyExist(service, document.Id))
            {
                tracingService.Trace("At least one Approve task already exists. Return");
                return;
            }

            var team = GetMfgPriceListApproverTeam(service, tracingService);
            if(team == null)
            {
                throw new InvalidPluginExecutionException("Could not find the team approving Mfg Price List");
            }

            tracingService.Trace("Create a new Approve Mfg. Price List task");
            Guid newTaskId = service.Create(new Task()
            {
                RegardingObjectId = new EntityReference(ipg_document.EntityLogicalName, document.Id),
                ipg_tasktypecode = new OptionSetValue(ApproveMfgPriceListTaskTypeValue),
                Subject = "Approve uploaded Mfg. Price List",
                OwnerId = new EntityReference(Team.EntityLogicalName, team.Id)
            });
            tracingService.Trace("Created a new Task with id=" + newTaskId);

            tracingService.Trace($"{typeof(CreateApprovePriceListUserTask)} plugin finished");
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

        private Team GetMfgPriceListApproverTeam(IOrganizationService organizationService, ITracingService tracingService)
        {
            tracingService.Trace($"Retrieving {Constants.Settings.MfgPriceListApprover} global setting");
            var globalSettings = organizationService.RetrieveMultiple(new QueryExpression(ipg_globalsetting.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(ipg_globalsetting.ipg_value)),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(ipg_globalsetting.ipg_name).ToLower(), ConditionOperator.Equal, Constants.Settings.MfgPriceListApprover)
                        }
                }
            });
            if (globalSettings.Entities.Count == 0)
            {
                throw new InvalidPluginExecutionException($"Could not find the global setting '{Constants.Settings.MfgPriceListApprover}'");
            }
            var globalSetting = globalSettings.Entities[0].ToEntity<ipg_globalsetting>();
            if (string.IsNullOrWhiteSpace(globalSetting.ipg_value))
            {
                throw new InvalidPluginExecutionException($"'{Constants.Settings.MfgPriceListApprover}' global setting value is empty");
            }

            tracingService.Trace($"Retrieving '{globalSetting.ipg_value}' team");
            var teams = organizationService.RetrieveMultiple(new QueryExpression(Team.EntityLogicalName)
            {
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(Team.Name).ToLower(), ConditionOperator.Equal, globalSetting.ipg_value)
                        }
                }
            });
            if (teams.Entities.Count == 0)
            {
                return null;
            }

            return teams.Entities[0].ToEntity<Team>();
        }
    }
}