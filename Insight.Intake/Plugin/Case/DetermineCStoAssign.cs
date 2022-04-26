using System;
using System.Linq;
using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Case
{
    public class DetermineCStoAssign : PluginBase
    {
        public DetermineCStoAssign() : base(typeof(DetermineCStoAssign))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGCaseActionsDetermineCStoAssign", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            localPluginContext.PluginExecutionContext.OutputParameters["AssignToTeamId"] = Constants.TeamGuids.CarrierServices.ToString();
            var service = localPluginContext.OrganizationService;
            var context = localPluginContext.PluginExecutionContext;

            var caseRef = context.InputParameters.Contains("Target")
                ? localPluginContext.GetInput<EntityReference>("Target")
                : null;

            var carrierRef = context.InputParameters.Contains("PrimaryCarrier")
                ? localPluginContext.GetInput<EntityReference>("PrimaryCarrier")
                : context.InputParameters.Contains("SecondaryCarrier")
                    ? localPluginContext.GetInput<EntityReference>("SecondaryCarrier")
                    : null;


            if (caseRef == null) return;

            var caseUpd = new Entity(caseRef.LogicalName, caseRef.Id)
            {
                [Incident.Fields.ipg_assignedtoteamid] = new EntityReference(Team.EntityLogicalName, Constants.TeamGuids.CarrierServices)
            }.ToEntity<Intake.Incident>();


            if (carrierRef != null)
            {
                var assignToUser = GetUserToAssignTo(service, carrierRef);

                if (assignToUser != null)
                {
                    caseUpd.OwnerId = assignToUser;
                    caseUpd.ipg_assignedtoteamid = null;
                    localPluginContext.PluginExecutionContext.OutputParameters["AssignToUserId"] = assignToUser.Id.ToString();
                }
            }

            service.Update(caseUpd);
        }

        private EntityReference GetUserToAssignTo(IOrganizationService service, EntityReference carrierRef)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = "ipg_assigntocsrule",
                ColumnSet = new ColumnSet("ownerid"),
                Criteria = new FilterExpression()
                {
                    FilterOperator = LogicalOperator.And,
                    Filters =
                    {
                        new FilterExpression()
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions =
                            {
                                new ConditionExpression("statecode", ConditionOperator.Equal, 0)
                            }
                        },
                        new FilterExpression()
                        {
                            FilterOperator = LogicalOperator.Or,
                            Conditions =
                            {
                                new ConditionExpression("ipg_carrierid", ConditionOperator.Equal, carrierRef.Id)
                            }
                        }
                    }
                }
            };

            var result = service.RetrieveMultiple(query).Entities.Select(p => p.ToEntity<Intake.ipg_assigntocsrule>());
            return result.Count() == 1
                ? result.FirstOrDefault().OwnerId
                : null;
        }
    }
}