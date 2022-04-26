using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Insight.Intake.Plugin.Document
{
    public class ValidateMfgPriceList : PluginBase
    {
        public static readonly string TargetInputParameterName = "Target";

        public ValidateMfgPriceList() : base(typeof(RenameDocumentWhenPropertiesChange))
        {
            RegisterEvent(PipelineStages.PreValidation, MessageNames.Create, ipg_document.EntityLogicalName, PreValidationCreateOrUpdateHandler);

            /*On Update of these fields:
            -ipg_documenttypeid
            -ipg_reviewstatus
            */
            RegisterEvent(PipelineStages.PreValidation, MessageNames.Update, ipg_document.EntityLogicalName, PreValidationCreateOrUpdateHandler);
        }

        private void PreValidationCreateOrUpdateHandler(LocalPluginContext localPluginContext)
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

            
            if (document.Attributes.Contains(nameof(ipg_document.ipg_ReviewStatus).ToLower()) == false
                && document.Attributes.Contains(nameof(ipg_document.ipg_DocumentTypeId).ToLower()) == false)
            {
                tracingService.Trace("Review status or doc type did not change. Return");
                return;
            }
            if(document.Attributes.Contains(nameof(ipg_document.ipg_ReviewStatus).ToLower())
                && ((OptionSetValue)document.Attributes[nameof(ipg_document.ipg_ReviewStatus).ToLower()])?.Value == (int?)ipg_document_ipg_ReviewStatus.PendingReview)
            {
                tracingService.Trace("Review status is Pending Review. Return");
                return;
            }

            ipg_document documentFromDb = null;
            if (context.MessageName == MessageNames.Update)
            {
                tracingService.Trace("Retrieving the document from the DB");
                documentFromDb = service.Retrieve(ipg_document.EntityLogicalName,
                                               document.Id,
                                               new ColumnSet(
                                                   nameof(ipg_document.ipg_DocumentTypeId).ToLower()
                                                )
                            ).ToEntity<ipg_document>();
                if (documentFromDb == null)
                {
                    throw new InvalidPluginExecutionException("Could not find the requested document");
                }
            }

            Guid? docTypeId = document.ipg_DocumentTypeId?.Id ?? documentFromDb?.ipg_DocumentTypeId?.Id;
            if(docTypeId.HasValue == false)
            {
                tracingService.Trace("No doc type set. Return");
                return;
            }

            tracingService.Trace("Retrieving Doc Type with id = " + docTypeId);
            var docType = service.Retrieve(ipg_documenttype.EntityLogicalName,
                                           docTypeId.Value,
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


            var approvingTeam = MfgPriceListHelper.GetMfgPriceListApproverTeamOrThrow(service, tracingService);

            tracingService.Trace("Checking whether the current user is a system admin");
            if (HavingAdminRole(context.InitiatingUserId, service))
            {
                tracingService.Trace("System admin. Return");
                return;
            }

            tracingService.Trace("Checking whether the current user is a member of the authorized team");
            if (IsTeamMember(approvingTeam.Id, context.InitiatingUserId, service))
            {
                tracingService.Trace("Member of the authorized team. Return");
                return;
            }

            throw new InvalidPluginExecutionException("You are authorized to set only 'Draft' Review Status for 'Mfg Price List' documents");
        }

        private bool HavingAdminRole(Guid systemUserId, IOrganizationService organizationService)
        {
            var query = new QueryExpression("role");
            query.Criteria.AddCondition("roletemplateid", ConditionOperator.Equal, Constants.RoleGuids.AdminRole);
            var link = query.AddLink("systemuserroles", "roleid", "roleid");
            link.LinkCriteria.AddCondition("systemuserid", ConditionOperator.Equal, systemUserId);

            return organizationService.RetrieveMultiple(query).Entities.Count > 0;
        }

        private bool IsTeamMember(Guid teamID, Guid userID, IOrganizationService service)
        {
            var query = new QueryExpression(Team.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(Team.TeamId).ToLower()),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                            new ConditionExpression(nameof(Team.TeamId).ToLower(), ConditionOperator.Equal, teamID)
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(
                        Team.EntityLogicalName,
                        TeamMembership.EntityLogicalName,
                        nameof(Team.TeamId).ToLower(),
                        nameof(TeamMembership.TeamId).ToLower(),
                        JoinOperator.Inner)
                    {
                        Columns = new ColumnSet(nameof(TeamMembership.TeamMembershipId).ToLower()),
                        LinkCriteria = new FilterExpression()
                        {
                            Conditions =
                            {
                                new ConditionExpression(nameof(TeamMembership.SystemUserId).ToLower(), ConditionOperator.Equal, userID)
                            }
                        }
                    }
                }
            };

            var results = service.RetrieveMultiple(query);
            return results.Entities.Count > 0;
        }
    }
}
