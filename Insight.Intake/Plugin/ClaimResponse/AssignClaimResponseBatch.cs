using System;
using Insight.Intake.Helpers;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.ClaimResponse
{
    public class AssignClaimResponseBatch : PluginBase
    {
        public AssignClaimResponseBatch() : base(typeof(AssignClaimResponseBatch))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGIntakeActionsAssignClaimResponseBatch", ipg_claimresponsebatch.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                EntityReference batchRef = (EntityReference)context.InputParameters["Target"];
                AssignBatch(service, batchRef);
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }

        private void AssignBatch(IOrganizationService service, EntityReference batchRef)
        {
            ipg_claimresponsebatch batch = service.Retrieve(batchRef.LogicalName, batchRef.Id, new ColumnSet(nameof(ipg_claimresponsebatch.ipg_PaymentSource).ToLower())).ToEntity<ipg_claimresponsebatch>();
            if ((batch.ipg_PaymentSource != null) && (batch.ipg_PaymentSource.Value == (int)ipg_PaymentSource.BNY))
            {

                QueryExpression queryExpression = new QueryExpression(Team.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(nameof(Team.TeamId).ToLower()),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(nameof(Team.Name).ToLower(), ConditionOperator.Equal, Constants.TeamNames.PatientServices)
                        }
                    }
                };
                EntityCollection teams = service.RetrieveMultiple(queryExpression);
                if (teams.Entities.Count > 0)
                {
                    var assign = new AssignRequest
                    {
                        Assignee = new EntityReference(Team.EntityLogicalName, teams.Entities[0].GetAttributeValue<Guid>(nameof(Team.TeamId).ToLower())),
                        Target = batchRef
                    };
                    service.Execute(assign);
                }
            }
        }
    }
}