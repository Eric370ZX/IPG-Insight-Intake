using Insight.Intake.Data;
using Insight.Intake.Extensions;
using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    public class DeterminePrimaryCarrierPlugin : GatingPluginBase
    {
        public DeterminePrimaryCarrierPlugin() : base("ipg_IPGCaseActionsDeterminePrimaryCarrier") { }
        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx)
        {
            try
            {
                var caseRequiredFields = new[] { Incident.Fields.ipg_CarrierId, Incident.Fields.ipg_FacilityId, Incident.Fields.ipg_SurgeryDate, Incident.Fields.ipg_ActualDOS, Incident.Fields.CreatedBy, Incident.Fields.Title, Incident.Fields.ipg_carriernetwork };

                ColumnSet columns = targetRef.LogicalName == Incident.EntityLogicalName ?
                    new ColumnSet(caseRequiredFields) :
                    new ColumnSet(true);
                var targetEntity = crmService.Retrieve(targetRef.LogicalName, targetRef.Id, columns);

                if (!targetEntity.Contains(Incident.Fields.ipg_CarrierId) || !targetEntity.Contains(Incident.Fields.ipg_FacilityId) || !targetEntity.Contains(Incident.Fields.ipg_SurgeryDate))
                {
                    return new GatingResponse(false, "One or several required fields are empty");
                }
                var carrier = (Intake.Account)crmService.Retrieve(targetEntity.GetAttributeValue<EntityReference>(Incident.Fields.ipg_CarrierId).LogicalName, targetEntity.GetAttributeValue<EntityReference>(Incident.Fields.ipg_CarrierId).Id, new ColumnSet(true));
               
                Action<Guid, EntityReference> updateCaseCarrier = (id, carrierId) =>
                {
                    var caseWithUpdatedAccount = new Incident
                    {
                        ipg_SecondaryCarrierId = carrierId,
                        Id = id,
                    };
                    crmService.Update(caseWithUpdatedAccount);
                };
                CopyDefaultValues(targetEntity);
                if (carrier.ipg_HomePlanNetworkOptionSet == null || carrier.ipg_HomePlanNetworkOptionSet.Value != CarrierHomePlanNetworkOptionSet.Bcbs)
                {
                    return new GatingResponse()
                    {
                        Succeeded = true,
                        CodeOutput = (int)DeterminePrimaryCarrierOutcomes.NoPrimaryCarrierDeterminationRequired,

                    };
                }
                var carrierFacilityRelationship = RetrieveCarrierFacilityRelationship(targetEntity.GetAttributeValue<EntityReference>(Incident.Fields.ipg_CarrierId), targetEntity.GetAttributeValue<EntityReference>(Incident.Fields.ipg_FacilityId), targetEntity.GetAttributeValue<DateTime>("ipg_surgerydate"), targetEntity.GetCaseDos());
                if (carrierFacilityRelationship.Any())
                {
                    //createPost($"{carrier.Name} is the correct contracted carrier for this case.");
                    return new GatingResponse()
                    {
                        Succeeded = true,
                        CodeOutput = (int)DeterminePrimaryCarrierOutcomes.СorrectContractedCarrierForThisCase
                    };
                }
                var carrierFacilityRelationships = RetrieveCarrierFacilityRelationships(targetEntity.GetAttributeValue<EntityReference>(Incident.Fields.ipg_FacilityId), targetEntity.GetAttributeValue<DateTime>("ipg_surgerydate"), targetEntity.GetCaseDos()).ToList();
                switch (carrierFacilityRelationships.Count)
                {
                    case 0:
                        //createPost($"There are no active carrier contracts for {carrier.Name} for the Home Plan Network BCBS.");
                        return new GatingResponse()
                        {
                            Succeeded = false,
                            CodeOutput = (int)DeterminePrimaryCarrierOutcomes.NoActiveCarrierContractsForCarrierforBCBS
                        };
                    case 1:
                        var record = (Entitlement)carrierFacilityRelationships.First();
                        var carrierFacility = (Intake.Account)crmService.Retrieve(record.ipg_CarrierId.LogicalName, record.ipg_CarrierId.Id, new ColumnSet(Intake.Account.Fields.Name));
                        //createPost($"{carrier.Name} is part of Home Plan Network BCBS and the carrier for this case has been assigned as {carrierFacility.Name}.");
                        if (targetEntity.LogicalName == Incident.EntityLogicalName)
                        {
                            updateCaseCarrier(targetEntity.Id, carrierFacility.ToEntityReference());
                        }
                        return new GatingResponse()
                        {
                            Succeeded = true,
                            CodeOutput = (int)DeterminePrimaryCarrierOutcomes.PrimaryCarrierDeterminedUpdated
                        };
                    default:
                        return new GatingResponse()
                        {
                            Succeeded = false,
                            CodeOutput = (int)DeterminePrimaryCarrierOutcomes.MultiplePrimaryCarriersPossible
                        };
                }
            }
            catch (FaultException<OrganizationServiceFault> faultException)
            {
                throw new InvalidPluginExecutionException($"An error occurred in {nameof(DeterminePrimaryCarrierPlugin)}.", faultException);
            }
            catch (Exception exception)
            {
                tracingService.Trace($"{nameof(DeterminePrimaryCarrierPlugin)}: {exception.ToString()}");
            }
            return new GatingResponse()
            {
                Succeeded = true,
                CodeOutput = (int)DeterminePrimaryCarrierOutcomes.NoPrimaryCarrierDeterminationRequired,

            };
        }

        private void CopyDefaultValues(Entity targetEntity)
        {
            Action<Guid, EntityReference> updateCasePrimaryCarrier = (id, carrierid) =>
            {
                var caseWithUpdatedPCarrier = new Incident
                {
                    ipg_CarrierId = carrierid,
                    Id = id,
                };
                crmService.Update(caseWithUpdatedPCarrier);
            };

            Action<EntityReference> updateCaseNetwork = (carrierNetworkId) =>
            {
                var caseWithUpdatedCarrierNetwork = new Incident
                {
                    ipg_carriernetwork = carrierNetworkId,
                    Id = targetEntity.Id,
                };
                crmService.Update(caseWithUpdatedCarrierNetwork);
            };
            if (targetEntity.GetAttributeValue<EntityReference>(Incident.Fields.ipg_FacilityId) != null && IsAccountInState("FL", targetEntity.GetAttributeValue<EntityReference>(Incident.Fields.ipg_FacilityId)) && IsAetnaAccount(targetEntity.GetAttributeValue<EntityReference>(Incident.Fields.ipg_CarrierId), "Aetna"))
            {
                var aetnaTieredAccount = RetrieveAccountByName("Aetna-Tiered Carrier");
                if (aetnaTieredAccount != null)
                {
                    if (targetEntity.LogicalName == Incident.EntityLogicalName)
                    {
                        updateCasePrimaryCarrier(targetEntity.Id, aetnaTieredAccount.ToEntityReference());
                    }
                    targetEntity[Incident.Fields.ipg_CarrierId] = aetnaTieredAccount.ToEntityReference();
                }
            }
            if (targetEntity.GetAttributeValue<EntityReference>(Incident.Fields.ipg_CarrierId) != null)
            {
                var carrierNetwork = (Intake.Account)crmService.Retrieve(targetEntity.GetAttributeValue<EntityReference>(Incident.Fields.ipg_CarrierId).LogicalName, targetEntity.GetAttributeValue<EntityReference>(Incident.Fields.ipg_CarrierId).Id, new ColumnSet("ipg_network"));
                if (carrierNetwork.ipg_Network != null)
                {
                    if (targetEntity.LogicalName == Incident.EntityLogicalName)
                    {
                        updateCaseNetwork(carrierNetwork.ipg_Network);
                    }
                }
            }
        }

        private QueryExpression PrepareExpression(EntityReference facilityRef, DateTime scheduledDos, DateTime? actualDos)
        {
            var carrierFacilityRelationshipQuery = new QueryExpression
            {
                EntityName = Entitlement.EntityLogicalName,
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression(LogicalOperator.And),
                PageInfo = new PagingInfo
                {
                    ReturnTotalRecordCount = true,
                },
            };

            carrierFacilityRelationshipQuery.Criteria.AddCondition(Incident.Fields.ipg_FacilityId, ConditionOperator.Equal, facilityRef.Id);
            carrierFacilityRelationshipQuery.Criteria.AddCondition(Entitlement.Fields.ipg_CarrierUnsupported, ConditionOperator.Equal, false);
            carrierFacilityRelationshipQuery.Criteria.AddCondition(Entitlement.Fields.ipg_EntitlementType,
                                                                   ConditionOperator.Equal, (int)ipg_EntitlementTypes.FacilityCarrier);

            var effectiveDateFilter = new FilterExpression(LogicalOperator.Or);
            var dos = actualDos > DateTime.MinValue && actualDos != null ? actualDos.Value : scheduledDos;
            var dosFilter = new FilterExpression(LogicalOperator.And);

            dosFilter.AddCondition(Entitlement.Fields.StartDate, ConditionOperator.LessEqual, dos);
            dosFilter.AddCondition(Entitlement.Fields.EndDate, ConditionOperator.GreaterEqual, dos);
            effectiveDateFilter.AddFilter(dosFilter);
            carrierFacilityRelationshipQuery.Criteria.AddFilter(effectiveDateFilter);

            return carrierFacilityRelationshipQuery;
        }

        private IEnumerable<Entity> RetrieveCarrierFacilityRelationship(EntityReference carrierRef, EntityReference facilityRef, DateTime scheduledDos, DateTime? actualDos)
        {
            var carrierFacilityRelationshipQuery = PrepareExpression(facilityRef, scheduledDos, actualDos);
            carrierFacilityRelationshipQuery.Criteria.AddCondition(Incident.Fields.ipg_CarrierId, ConditionOperator.Equal, carrierRef.Id);
            var carrierFacilityRelationship = crmService.RetrieveMultiple(carrierFacilityRelationshipQuery);
            return carrierFacilityRelationship.Entities;
        }

        private IEnumerable<Entity> RetrieveCarrierFacilityRelationships(EntityReference facilityRef, DateTime scheduledDos, DateTime? actualDos)
        {
            var carrierFacilityRelationshipQuery = PrepareExpression(facilityRef, scheduledDos, actualDos);
            var carrierFacilityRelationship = crmService.RetrieveMultiple(carrierFacilityRelationshipQuery);
            var result = new List<Entity>();
            foreach (var record in carrierFacilityRelationship.Entities.Cast<Entitlement>())
            {
                var carrier = (Intake.Account)crmService.Retrieve(record.ipg_CarrierId.LogicalName, record.ipg_CarrierId.Id, new ColumnSet(true));
                if (carrier.ipg_HomePlanNetworkOptionSet != null && carrier.ipg_HomePlanNetworkOptionSet.Value == CarrierHomePlanNetworkOptionSet.Bcbs)
                {
                    result.Add(record);
                }
            }
            return result;
        }
        private bool IsAccountInState(string stateName, EntityReference accountRef)
        {
            var query = new QueryExpression
            {
                EntityName = accountRef.LogicalName,
                ColumnSet = new ColumnSet(Intake.Account.Fields.AccountId),
                Criteria = {
                    Conditions = {
                        new ConditionExpression(Intake.Account.Fields.AccountId, ConditionOperator.Equal, accountRef.Id),
                        new ConditionExpression(Intake.Account.Fields.Address1_StateOrProvince, ConditionOperator.Equal, stateName),
                    }
                }
            };
            return crmService.RetrieveMultiple(query).Entities.Any();
        }

        private bool IsAetnaAccount(EntityReference accountRef, string name)
        {
            var query = new QueryExpression
            {
                EntityName = accountRef.LogicalName,
                ColumnSet = new ColumnSet(Intake.Account.Fields.AccountId),
                Criteria = {
                    Conditions = {
                        new ConditionExpression(Intake.Account.Fields.AccountId, ConditionOperator.Equal, accountRef.Id),
                        new ConditionExpression(Intake.Account.Fields.Name, ConditionOperator.Equal, name),
                    }
                }
            };
            return crmService.RetrieveMultiple(query).Entities.Any();
        }

        private Intake.Account RetrieveAccountByName(string name)
        {
            var query = new QueryExpression
            {
                EntityName = Intake.Account.EntityLogicalName,
                ColumnSet = new ColumnSet(Intake.Account.Fields.AccountId),
                Criteria = {
                    Conditions = {
                        new ConditionExpression(Intake.Account.Fields.Name, ConditionOperator.Equal, name),
                    }
                }
            };
            var entities = crmService.RetrieveMultiple(query).Entities;
            return entities.Any()
                ? entities.FirstOrDefault().ToEntity<Intake.Account>()
                : null;
        }
    }

    public enum DeterminePrimaryCarrierOutcomes
    {
        NoPrimaryCarrierDeterminationRequired = 1,
        СorrectContractedCarrierForThisCase = 2,
        NoActiveCarrierContractsForCarrierforBCBS = 3,
        PrimaryCarrierDeterminedUpdated = 4,
        MultiplePrimaryCarriersPossible = 5
    }
}
