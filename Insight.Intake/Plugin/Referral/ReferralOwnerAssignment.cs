using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.Referral
{
    public class ReferralOwnerAssignment : PluginBase
    {
        public ReferralOwnerAssignment() : base(typeof(ReferralOwnerAssignment))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_referral.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_referral.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var executionContext = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var referral = executionContext.MessageName == MessageNames.Update ? localPluginContext.PostImage<ipg_referral>() : localPluginContext.Target<ipg_referral>();
            if (referral == null)
            {
                throw new InvalidPluginExecutionException($"{(executionContext.MessageName == MessageNames.Update ? "PostImage" : "Target")} can not be null");
            }

            if (!Enum.Equals(referral.ipg_statecodeEnum, ipg_CaseStateCodes.Intake))
            {
                return;
            }

            var fetchXml = $@"
                <fetch top='1' >
                  <entity name='{ipg_caseassignmentconfig.EntityLogicalName}' >
                    <attribute name='{ipg_caseassignmentconfig.Fields.ipg_AssignToUser}' />
                    <attribute name='{ipg_caseassignmentconfig.Fields.ipg_AssignToTeam}' />
                    <filter>
                      <condition attribute='{ipg_caseassignmentconfig.Fields.ipg_CaseState}' operator='eq' value='{referral.ipg_statecode.Value}' />
                    </filter>
                  </entity>
                </fetch>";

            var config = service.RetrieveMultiple(new FetchExpression(fetchXml))?.Entities
            .Select(e => e.ToEntity<ipg_caseassignmentconfig>()).ToList().FirstOrDefault();

            if (config != null)
            {
                var assignee = config?.ipg_AssignToUser ?? config?.ipg_AssignToTeam;
                if (assignee == null)
                {
                    throw new InvalidPluginExecutionException("Failed getting assignee from Case Assignment Configuration records.");
                }
                
                var assignRequest = new AssignRequest()
                {
                    Assignee = assignee,
                    Target = referral.ToEntityReference()
                };
                service.Execute(assignRequest);
            }
            else
            {
                throw new InvalidPluginExecutionException($"An error occurred while getting Case Assignment Configuration records.");
            }
        }
    }
}
