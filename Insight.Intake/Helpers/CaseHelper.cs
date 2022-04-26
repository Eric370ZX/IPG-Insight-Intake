using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Helpers
{
    internal static class CaseHelper
    {
        public static string DetermineNetworkName(Incident incident, IOrganizationService orgService, ITracingService tracingService)
        {
            //todo: this method can be replaced with a Network field on Case

            tracingService.Trace("Determine Network Name");

            if (incident.ipg_CarrierId == null)
            {
                throw new Exception("Case (Incident) must have a Carrier reference");
            }
            if (incident.ipg_FacilityId == null)
            {
                throw new Exception("Case (Incident) must have a Facility reference");
            }

            tracingService.Trace("Retrieving the related Carrier");
            Intake.Account carrier = orgService.Retrieve(Intake.Account.EntityLogicalName, incident.ipg_CarrierId.Id,
                new ColumnSet(
                    Intake.Account.Fields.ipg_contract,
                    Intake.Account.Fields.ipg_CarrierType,
                    Intake.Account.Fields.ipg_carriernetworkid
                )).ToEntity<Intake.Account>();
            if (carrier == null)
            {
                throw new Exception($"Could not find the requested Carrier (Account.id={incident.ipg_CarrierId.Id})");
            }

            var networksRetrievalResult = GetCarrierNetworks(incident, carrier, orgService, tracingService);
            IEnumerable<ipg_carriernetwork> networks = networksRetrievalResult.Entities.ToList().Select(e => e.ToEntity<ipg_carriernetwork>());

            tracingService.Trace("Selecting a Carrier Network");
            foreach (var network in networks)
            {
                if (network.ipg_CarrierAssignmentOnly == true)
                {
                    if (carrier.ipg_carriernetworkid != null)
                    {
                        var carrierNetwork = networks.FirstOrDefault(n => n.Id == carrier.ipg_carriernetworkid.Id);
                        if (carrierNetwork != null)
                        {
                            return carrierNetwork.ipg_AbbreviatedNameforGP;
                        }
                    }
                }
                else
                {
                    if (network.ipg_ContractedPayorsOnly == true)
                    {
                        if (carrier.ipg_contract == true)
                        {
                            return network.ipg_AbbreviatedNameforGP;
                        }
                    }
                    else
                    {
                        return network.ipg_AbbreviatedNameforGP;
                    }
                }
            }

            return null;
        }

        private static EntityCollection GetCarrierNetworks(Incident incident, Intake.Account carrier, IOrganizationService orgService, ITracingService tracingService)
        {
            if (carrier.ipg_CarrierType == null)
            {
                throw new Exception("Carrier SupportedPlanTypes cannot be null");
            }

            tracingService.Trace("Retrieving the related Facility");
            Intake.Account facility = orgService.Retrieve(Intake.Account.EntityLogicalName, incident.ipg_FacilityId.Id,
                new ColumnSet(
                    nameof(facility.ipg_StateId).ToLower()
                )).ToEntity<Intake.Account>();
            if (facility == null)
            {
                throw new Exception($"Could not find the requested Facility (Account.id={incident.ipg_FacilityId.Id})");
            }
            if (facility.ipg_StateId == null)
            {
                throw new Exception("Facility must have a State reference");
            }

            tracingService.Trace("Building QueryExpression to retrieve Carrier Networks");
            var queryExpression = new QueryExpression(ipg_carriernetwork.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression(
                            nameof(ipg_carriernetwork.StateCode).ToLower(),
                            ConditionOperator.Equal, 0) //0 means active
                    }
                },
                LinkEntities =
                {
                    new LinkEntity(
                        ipg_carriernetwork.EntityLogicalName,
                        ipg_ipg_carriernetwork_ipg_state.EntityLogicalName,
                        nameof(ipg_carriernetwork.ipg_carriernetworkId).ToLower(),
                        nameof(ipg_ipg_carriernetwork_ipg_state.ipg_carriernetworkid).ToLower(),
                        JoinOperator.Inner)
                    {
                        Columns = new ColumnSet(nameof(ipg_ipg_carriernetwork_ipg_state.ipg_ipg_carriernetwork_ipg_stateId).ToLower()),
                        LinkCriteria = new FilterExpression()
                        {
                            Conditions =
                            {
                                new ConditionExpression(
                                    ipg_ipg_carriernetwork_ipg_state.Fields.ipg_stateid,
                                    ConditionOperator.Equal, facility.ipg_StateId.Id)
                            }
                        }
                    }
                }
            };

            //add plan type filters
            queryExpression.Criteria.Conditions.Add(new ConditionExpression(
               ipg_carriernetwork.Fields.ipg_plantype,
                ConditionOperator.ContainValues,
                carrier.ipg_CarrierType.Value));

            tracingService.Trace("Retrieving Carrier Networks");
            return orgService.RetrieveMultiple(queryExpression);
        }
    }
}
