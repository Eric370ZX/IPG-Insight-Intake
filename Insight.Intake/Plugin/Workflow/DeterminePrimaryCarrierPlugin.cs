using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Insight.Intake.Data;
using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Workflow
{
    public class DeterminePrimaryCarrierPlugin : IPlugin
    {
        private IOrganizationService _organizationService;
        
        public void Execute(IServiceProvider serviceProvider)
        {
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            
            try
            {
                var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                var organizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                _organizationService = organizationServiceFactory.CreateOrganizationService(context.UserId);

                if (context.MessageName != Constants.ActionNames.CaseActionsDeterminePrimaryCarrier)
                {
                    return;
                }

                if (!context.InputParameters.Contains("CarrierRef") || !context.InputParameters.Contains("FacilityRef") || !context.InputParameters.Contains("ScheduledDOS"))
                {
                    return;
                }

                var carrierRef = (EntityReference)context.InputParameters["CarrierRef"];

                var facilityRef = (EntityReference)context.InputParameters["FacilityRef"];

                var scheduledDos = (DateTime)context.InputParameters["ScheduledDOS"];

                var actualDos = (DateTime)context.InputParameters["ActualDOS"];
                
                var carrier = (Intake.Account)_organizationService.Retrieve(carrierRef.LogicalName, carrierRef.Id, new ColumnSet(true));

                context.OutputParameters["NoHomePlanNetwork"] = false;
                
                context.OutputParameters["CorrectPrimaryCarrier"] = false;

                context.OutputParameters["NoActiveContracts"] = false;

                context.OutputParameters["IncorrectPrimaryCarrier"] = false;
                
                context.OutputParameters["NewPrimaryCarrierId"] = carrierRef;
                
                context.OutputParameters["MultipleCarriers"] = false;
                
                if (carrier.ipg_HomePlanNetworkOptionSet == null || carrier.ipg_HomePlanNetworkOptionSet.Value != CarrierHomePlanNetworkOptionSet.Bcbs)
                {
                    context.OutputParameters["NoHomePlanNetwork"] = true;
                    return;
                }
                
                var carrierFacilityRelationship = RetrieveCarrierFacilityRelationship(carrierRef, facilityRef, scheduledDos, actualDos);

                if (carrierFacilityRelationship.Any())
                {
                    context.OutputParameters["CorrectPrimaryCarrier"] = true;
                    return;
                }

                var carrierFacilityRelationships = RetrieveCarrierFacilityRelationships(facilityRef, scheduledDos, actualDos).ToList();

                switch (carrierFacilityRelationships.Count)
                {
                    case 0:
                        context.OutputParameters["NoActiveContracts"] = true;
                        break;
                    case 1:
                        var record = (Entitlement) carrierFacilityRelationships.First();
                    
                        context.OutputParameters["IncorrectPrimaryCarrier"] = true;

                        context.OutputParameters["NewPrimaryCarrierId"] = record.ipg_CarrierId;
                        break;
                    default:
                        context.OutputParameters["MultipleCarriers"] = true;
                        break;
                }
            }
            catch (FaultException<OrganizationServiceFault> faultException)
            {
                throw new InvalidPluginExecutionException($"An error occurred in {nameof(DeterminePrimaryCarrierPlugin)}.", faultException);
            }
            catch (Exception exception)
            {
                tracingService.Trace("{0}: {1}", nameof(DeterminePrimaryCarrierPlugin), exception.ToString());
                throw;
            }
        }

        private QueryExpression PrepareExpression(EntityReference facilityRef, DateTime scheduledDos, DateTime actualDos)
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
            
            carrierFacilityRelationshipQuery.Criteria.AddCondition("ipg_facilityid", ConditionOperator.Equal, facilityRef.Id);
            carrierFacilityRelationshipQuery.Criteria.AddCondition("ipg_carrierunsupported", ConditionOperator.Equal, false);
            carrierFacilityRelationshipQuery.Criteria.AddCondition(nameof(Entitlement.ipg_EntitlementType).ToLower(), 
                                                                   ConditionOperator.Equal, (int)ipg_EntitlementTypes.FacilityCarrier);

            var effectiveDateFilter = new FilterExpression(LogicalOperator.Or);
            var dos = actualDos > DateTime.MinValue ? actualDos : scheduledDos;
            var dosFilter = new FilterExpression(LogicalOperator.And);

            dosFilter.AddCondition("startdate", ConditionOperator.LessEqual, dos);
            dosFilter.AddCondition("enddate", ConditionOperator.GreaterEqual, dos);
            effectiveDateFilter.AddFilter(dosFilter);
            carrierFacilityRelationshipQuery.Criteria.AddFilter(effectiveDateFilter);

            return carrierFacilityRelationshipQuery;
        }

        private IEnumerable<Entity> RetrieveCarrierFacilityRelationship(EntityReference carrierRef, EntityReference facilityRef, DateTime scheduledDos, DateTime actualDos)
        {
            var carrierFacilityRelationshipQuery = PrepareExpression(facilityRef, scheduledDos, actualDos);          
            carrierFacilityRelationshipQuery.Criteria.AddCondition("ipg_carrierid", ConditionOperator.Equal, carrierRef.Id);
            var carrierFacilityRelationship = _organizationService.RetrieveMultiple(carrierFacilityRelationshipQuery);
            return carrierFacilityRelationship.Entities;
        }

        private IEnumerable<Entity> RetrieveCarrierFacilityRelationships(EntityReference facilityRef, DateTime scheduledDos, DateTime actualDos)
        {
            var carrierFacilityRelationshipQuery = PrepareExpression(facilityRef, scheduledDos, actualDos);            
            var carrierFacilityRelationship = _organizationService.RetrieveMultiple(carrierFacilityRelationshipQuery);
            var result = new List<Entity>();
            foreach (var record in carrierFacilityRelationship.Entities.Cast<Entitlement>())
            {
                var carrier = (Intake.Account) _organizationService.Retrieve(record.ipg_CarrierId.LogicalName, record.ipg_CarrierId.Id, new ColumnSet(true));
                if (carrier.ipg_HomePlanNetworkOptionSet != null && carrier.ipg_HomePlanNetworkOptionSet.Value == CarrierHomePlanNetworkOptionSet.Bcbs)
                {
                    result.Add(record);
                }
            }
            return result;
        }
    }
}