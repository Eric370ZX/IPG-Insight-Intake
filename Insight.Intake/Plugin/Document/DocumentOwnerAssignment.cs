using Insight.Intake.Helpers;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Insight.Intake.Plugin.Document
{
    public class DocumentOwnerAssignment : PluginBase
    {
        public DocumentOwnerAssignment() : base(typeof(DocumentOwnerAssignment))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_document.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;
            var document = localPluginContext.Target<ipg_document>();

            if (document != null)
            {
                var documentAdminTeam = GetTeamByName(Constants.TeamNames.DocumentAdmin, service);
                EntityReference assignee = null;

                if ((document.Attributes.Contains(ipg_document.Fields.ipg_Source) && document.ipg_Source.Value == (int)ipg_DocumentSourceCode.Portal)
                    || document.ipg_DocumentTypeId?.Id == Constants.DocumentTypeGuids.PortalGenericDocument)
                {
                    assignee = GetFacilityCaseManager(document, service) ?? documentAdminTeam;
                }

                if (document.Attributes.Contains(ipg_document.Fields.ipg_DocumentTypeId) && document.ipg_DocumentTypeId != null)
                {
                    var documentType = service.Retrieve(ipg_documenttype.EntityLogicalName, document.ipg_DocumentTypeId.Id, new ColumnSet(ipg_documenttype.Fields.ipg_name))?.ToEntity<ipg_documenttype>();
                    switch (documentType?.ipg_name.ToLower())
                    {
                        case "fax":
                        case "email":
                            assignee = documentAdminTeam;
                            break;
                        default:
                            break;
                    }
                }

                if (assignee != null)
                {
                    try
                    {
                        var assignRequest = new AssignRequest()
                        {
                            Assignee = assignee,
                            Target = document.ToEntityReference()
                        };
                        service.Execute(assignRequest);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidPluginExecutionException(OperationStatus.Failed, $"An error occured while trying to execute the assign request: {ex.Message}");
                    }
                }
            }
            else
            {
                throw new InvalidPluginExecutionException(OperationStatus.Failed, $"Target entity '{ipg_document.EntityLogicalName}' can not be null.");
            }
        }

        private EntityReference GetFacilityCaseManager(ipg_document document, IOrganizationService service)
        {
            EntityReference facilityCaseMng = null;
            EntityReference facilityId = null;
            if (document.Attributes.Contains(ipg_document.Fields.ipg_FacilityId))
            {
                facilityId = document.ipg_FacilityId;
            }
            else if (document.Attributes.Contains(ipg_document.Fields.ipg_ReferralId))
            {
                var referral = service.Retrieve(ipg_referral.EntityLogicalName, document.ipg_ReferralId.Id, new ColumnSet(ipg_referral.Fields.ipg_FacilityId))?.ToEntity<ipg_referral>();
                facilityId = referral?.ipg_FacilityId;
            }
            else if (document.Attributes.Contains(ipg_document.Fields.ipg_CaseId))
            {
                var caseEntity = service.Retrieve(Incident.EntityLogicalName, document.ipg_CaseId.Id, new ColumnSet(Incident.Fields.ipg_FacilityId))?.ToEntity<Incident>();
                facilityId = caseEntity?.ipg_FacilityId;
            }

            if (facilityId != null)
            {
                var facility = service.Retrieve(
                    Intake.Account.EntityLogicalName, facilityId.Id, new ColumnSet(Intake.Account.Fields.ipg_FacilityCaseMgrId))?.ToEntity<Intake.Account>();

                facilityCaseMng = facility?.ipg_FacilityCaseMgrId;
            }
            return facilityCaseMng;
        }

        private EntityReference GetTeamByName(string teamName, IOrganizationService service)
        {
            var query = new QueryExpression(Team.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(Team.Fields.Id),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(Team.Fields.Name, ConditionOperator.Equal, teamName)
                    }
                }
            };
            var result = service.RetrieveMultiple(query);

            return result.Entities.Count > 0
                ? result.Entities[0].ToEntityReference()
                : throw new InvalidPluginExecutionException(OperationStatus.Failed, $"An error occurred while getting '{teamName}' Team.");
        }
    }
}