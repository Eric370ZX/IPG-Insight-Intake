using Insight.Intake.Extensions;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.Gating.DetermineFacilityCarrierStatus
{
    public class NoContract : PluginBase
    {
        public NoContract() : base(typeof(NoContract))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingDetermineFacilityCarrierStatusNoContract", null, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var taskManager = new TaskManager(service, localPluginContext.TracingService);

            EntityReference targetRef = context.InputParameters["Target"] as EntityReference;
            context.OutputParameters["Succeeded"] = false;

            if (targetRef != null)
            {
                Entity target = service.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet("ipg_carrierid", "ipg_facilityid", "ipg_actualdos", "ipg_surgerydate", "ipg_inoutnetwork"));
                var dos = target.GetCaseDos();

                var carrier = target.GetAttributeValue<EntityReference>("ipg_carrierid");
                var facility = target.GetAttributeValue<EntityReference>("ipg_facilityid");

                if (carrier == null || facility == null)
                {
                    context.OutputParameters["TaskDescripton"] = $"Carrier or facility is empty"; ;
                    context.OutputParameters["CaseNote"] = context.OutputParameters["TaskDescripton"];
                    return;
                }

                EntityCollection facilityCarrierContracts = GetActiveFacilityCarrierContracts(service, carrier.Id, facility.Id);

                var caseNote = string.Empty;
                if (facilityCarrierContracts.Entities.Count == 0)
                {
                    context.OutputParameters["TaskDescripton"] = string.Format(@"No contract exists for {0} at {1}", carrier?.Name, facility?.Name);
                    context.OutputParameters["CaseNote"] = context.OutputParameters["TaskDescripton"];
                    return;
                }
                else
                {
                    var activeFacilityCarrierContracts = facilityCarrierContracts.Entities.Where(e => (e.ToEntity<Entitlement>()).StartDate >= dos && (e.ToEntity<Entitlement>()).EndDate <= dos);
                    if (!activeFacilityCarrierContracts.Any())
                    {
                        context.OutputParameters["TaskDescripton"] = string.Format(@"No active carrier contracts exist for {0} for {1}", facility?.Name, ((DateTime)dos).ToString("yyyy-MM-dd"));
                        context.OutputParameters["CaseNote"] = context.OutputParameters["TaskDescripton"];
                        return;
                    }
                    else if(activeFacilityCarrierContracts.Count() > 1)
                    {
                        context.OutputParameters["TaskDescripton"] = string.Format(@"{0} has more than one active contract for {1}", carrier?.Name, facility?.Name);
                        context.OutputParameters["CaseNote"] = context.OutputParameters["TaskDescripton"];
                        return;
                    }
                    else if(activeFacilityCarrierContracts.First().ToEntity<Entitlement>().ipg_NetworkStatus == null)
                    {
                        context.OutputParameters["TaskDescripton"] = string.Format(@"No network status is listed for {0} for {1}", carrier?.Name, facility?.Name);
                        context.OutputParameters["CaseNote"] = context.OutputParameters["TaskDescripton"];
                        return;
                    }
                }
                context.OutputParameters["Succeeded"] = true;
            }
        }

        private EntityCollection GetActiveFacilityCarrierContracts(IOrganizationService service, Guid carrierId, Guid facilityId)
        {
            QueryExpression query = new QueryExpression
            {
                EntityName = Entitlement.EntityLogicalName,
                ColumnSet = new ColumnSet(Entitlement.Fields.StartDate, Entitlement.Fields.EndDate, Entitlement.Fields.ipg_NetworkStatus)
            };
            query.Criteria.AddCondition(Entitlement.Fields.ipg_FacilityId, ConditionOperator.Equal, facilityId);
            query.Criteria.AddCondition(Entitlement.Fields.ipg_CarrierId, ConditionOperator.Equal, carrierId);
            query.Criteria.AddCondition(Entitlement.Fields.ipg_EntitlementType, ConditionOperator.Equal, (int)ipg_EntitlementTypes.FacilityCarrier);
            //query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 1);

            return service.RetrieveMultiple(query);
        }
    }
}
