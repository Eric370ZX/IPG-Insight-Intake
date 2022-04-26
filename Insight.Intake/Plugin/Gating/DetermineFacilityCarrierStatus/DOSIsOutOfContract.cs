using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Insight.Intake.Plugin.Gating.DetermineFacilityCarrierStatus
{
    public class DOSIsOutOfContract : PluginBase
    {
        public DOSIsOutOfContract() : base(typeof(DOSIsOutOfContract))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingDetermineFacilityCarrierStatusDOSIsOutOfContract", null, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;

            EntityReference targetRef = context.InputParameters["Target"] as EntityReference;
            context.OutputParameters["Succeeded"] = false;

            if (targetRef != null)
            {
                Entity target = service.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet(Incident.Fields.ipg_CarrierId,
                    Incident.Fields.ipg_FacilityId,
                    Incident.Fields.ipg_ActualDOS, 
                    Incident.Fields.ipg_SurgeryDate, 
                    Incident.Fields.ipg_inoutnetwork));

                var dos = target.GetCaseDos();
                if (!target.Contains(Incident.Fields.ipg_CarrierId) || !target.Contains(Incident.Fields.ipg_FacilityId) || (dos == null))
                    return;

                //var carrierRef = target.GetAttributeValue<EntityReference>("ipg_carrierid");
                //var carrier = service.Retrieve(carrierRef.LogicalName, carrierRef.Id, new ColumnSet("name", "ipg_carriernetworkid"));
                EntityCollection facilityCarrierContracts = GetActiveFacilityCarrierContracts(service, target, (DateTime)dos);
                var facilityCarrierContractsNoDate = GetActiveFacilityCarrierContracts(service, target);

                var caseNote = string.Empty;
                if (facilityCarrierContracts.Entities.Count.Equals(1))
                {
                    context.OutputParameters["Succeeded"] = true;
                    var entitlement = facilityCarrierContracts.Entities[0].ToEntity<Entitlement>();
                    caseNote = string.Format(@"{0} has been validated and confirmed active for DOS {1} at {2} with a Network Status of {3}.
                                            The contract dates are {4} through {5}.",
                                            target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_CarrierId)?.Name,
                                            dos,
                                            target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_FacilityId)?.Name,
                                            target.GetAttributeValue<bool>(Incident.Fields.ipg_inoutnetwork) ? "IN" : "OUT",
                                            entitlement.StartDate.HasValue ? entitlement.StartDate.Value.Date.ToShortDateString() : string.Empty,
                                            entitlement.EndDate.HasValue ? entitlement.EndDate.Value.Date.ToShortDateString() : string.Empty);
                }
                else if (facilityCarrierContractsNoDate.Entities.Count.Equals(0))
                {
                    context.OutputParameters["Succeeded"] = true;
                }
                else
                {
                    caseNote = string.Format(@"{0} contract for {1} is invalid due to DOS is outside active contract range. Please contact {1} to confirm the network status.",
                        target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_CarrierId)?.Name,
                        target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_FacilityId)?.Name);
                    context.OutputParameters["TaskSubject"] = "Resolve Facility-Carrier Contract Issues";
                    context.OutputParameters["TaskDescripton"] = $"Review and resolve the following facility-carrier contract issue(s): {caseNote}"; ;
                }
                context.OutputParameters["CaseNote"] = caseNote;
            }
        }

        private EntityCollection GetActiveFacilityCarrierContracts(IOrganizationService service, Entity target, DateTime dos)
        {
            QueryExpression query = new QueryExpression
            {
                EntityName = Entitlement.EntityLogicalName,
                ColumnSet = new ColumnSet("startdate", "enddate")
            };

            query.Criteria.AddCondition(Entitlement.Fields.ipg_FacilityId, ConditionOperator.Equal, target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_FacilityId).Id);
            query.Criteria.AddCondition(Entitlement.Fields.ipg_CarrierId, ConditionOperator.Equal, target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_CarrierId).Id);
            query.Criteria.AddCondition(Entitlement.Fields.ipg_EntitlementType, ConditionOperator.Equal, 923720001);
            //query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 1);

            query.Criteria.AddFilter(new FilterExpression
            {
                FilterOperator = LogicalOperator.And,
                Conditions =
                {
                    new ConditionExpression(Entitlement.Fields.StartDate, ConditionOperator.LessThan, dos),
                    new ConditionExpression(Entitlement.Fields.EndDate, ConditionOperator.GreaterThan, dos)
                }
            });

            return service.RetrieveMultiple(query);
        }

        private EntityCollection GetActiveFacilityCarrierContracts(IOrganizationService service, Entity target)
        {
            QueryExpression query = new QueryExpression
            {
                EntityName = Entitlement.EntityLogicalName,
                ColumnSet = new ColumnSet(false)
            };

            query.Criteria.AddCondition(Entitlement.Fields.ipg_FacilityId, ConditionOperator.Equal, target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_FacilityId).Id);
            query.Criteria.AddCondition(Entitlement.Fields.ipg_CarrierId, ConditionOperator.Equal, target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_CarrierId).Id);
            query.Criteria.AddCondition(Entitlement.Fields.ipg_EntitlementType, ConditionOperator.Equal, (int)ipg_EntitlementTypes.FacilityCarrier);

            return service.RetrieveMultiple(query);
        }
    }
}