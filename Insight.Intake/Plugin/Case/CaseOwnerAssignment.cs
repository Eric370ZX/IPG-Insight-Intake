using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.Case
{
    public class CaseOwnerAssignment : PluginBase
    {
        public CaseOwnerAssignment() : base(typeof(CaseOwnerAssignment))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Incident.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Incident.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;
            var executionContext = localPluginContext.PluginExecutionContext;
            var caseEntity = executionContext.MessageName == MessageNames.Update ? localPluginContext.PostImage<Incident>() : localPluginContext.Target<Incident>();
            if (caseEntity == null)
            {
                throw new InvalidPluginExecutionException($"{(executionContext.MessageName == MessageNames.Update ? "PostImage" : "Target")} can not be null");
            }

            if (caseEntity.ipg_StateCodeEnum.HasValue == false
                || !Enum.IsDefined(typeof(ipg_CaseStateCodes), caseEntity.ipg_StateCodeEnum) 
                || !Enum.Equals(caseEntity.ipg_CaseStatusEnum, ipg_CaseStatus.Open))
            {
                return;
            }

            var fetchXml = $@"
                <fetch top='1' >
                  <entity name='{ipg_caseassignmentconfig.EntityLogicalName}' >
                    <attribute name='{ipg_caseassignmentconfig.Fields.ipg_AssignToUser}' />
                    <attribute name='{ipg_caseassignmentconfig.Fields.ipg_AssignToTeam}' />
                    <filter>
                      <condition attribute='{ipg_caseassignmentconfig.Fields.ipg_CaseState}' operator='eq' value='{caseEntity.ipg_StateCode.Value}' />
                    </filter>
                  </entity>
                </fetch>";

            var config = service.RetrieveMultiple(new FetchExpression(fetchXml))?.Entities
            .Select(e => e.ToEntity<ipg_caseassignmentconfig>()).ToList().FirstOrDefault();


            if (config != null)
            {
                EntityReference assignee = null;

                if (Enum.Equals(caseEntity.ipg_StateCodeEnum, ipg_CaseStateCodes.CaseManagement))
                {
                    var facilityCaseMng = GetFacilityCaseManager(caseEntity, service);
                    assignee = facilityCaseMng != null ? facilityCaseMng : (config?.ipg_AssignToUser ?? config?.ipg_AssignToTeam);
                }
                else
                {
                    assignee = config?.ipg_AssignToUser ?? config?.ipg_AssignToTeam;
                }

                if (assignee == null)
                {
                    throw new InvalidPluginExecutionException("Failed getting assignee from Case Assignment Configuration records.");
                }

                var assignRequest = new AssignRequest()
                {
                    Assignee = assignee,
                    Target = caseEntity.ToEntityReference()
                };
                service.Execute(assignRequest);

            }
            else
            {
                throw new InvalidPluginExecutionException("An error occurred while getting Case Assignment Configuration records.");
            }
        }

        private EntityReference GetFacilityCaseManager(Incident caseEntity, IOrganizationService service)
        {
            EntityReference facilityCaseMng = null;
            if (caseEntity.Contains(Incident.Fields.ipg_FacilityId))
            {
                var facility = service.Retrieve(
                    Intake.Account.EntityLogicalName, caseEntity.ipg_FacilityId.Id, new ColumnSet(Intake.Account.Fields.ipg_FacilityCaseMgrId))?.ToEntity<Intake.Account>();

                facilityCaseMng = facility?.ipg_FacilityCaseMgrId;
            }
            return facilityCaseMng;
        }
    }
}