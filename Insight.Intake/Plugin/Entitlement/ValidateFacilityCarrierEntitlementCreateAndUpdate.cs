using Microsoft.Xrm.Sdk;
using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.EntitlementNamespace
{
    public class ValidateFacilityCarrierEntitlementCreateAndUpdate : PluginBase
    {
        public ValidateFacilityCarrierEntitlementCreateAndUpdate() : base(typeof(ValidateFacilityCarrierEntitlementCreateAndUpdate))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, Entitlement.EntityLogicalName, PostOperationCreateHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Entitlement.EntityLogicalName, PostOperationUpdateHandler);
        }

        private void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                var entitlementRef = ((Entity)context.InputParameters["Target"]).ToEntity<Entitlement>();
                var entitlement = new Entitlement();
                entitlement = service.Retrieve(Entitlement.EntityLogicalName,
                              entitlementRef.Id, new ColumnSet(
                                  LogicalNameof<Entitlement>.Property(x => x.StatusCode),
                                  LogicalNameof<Entitlement>.Property(x => x.ipg_EntitlementType),
                                  LogicalNameof<Entitlement>.Property(x => x.ipg_FacilityId),
                                  LogicalNameof<Entitlement>.Property(x => x.ipg_CarrierId),
                                  LogicalNameof<Entitlement>.Property(x => x.StartDate),
                                  LogicalNameof<Entitlement>.Property(x => x.EndDate))).ToEntity<Entitlement>();

                if (entitlement.ipg_EntitlementType.Value == (int)ipg_EntitlementTypes.FacilityCarrier)
                {
                    ValidateOverlappingEntitlementBasedOnFacilityCarrierAndAcceptanceDate(service, entitlement);
                }
            }
        }

        //This routine will be called when Entitlement is updated. Now when user activate a Entitlement,
        // we check if its a facility carrier entitl
        private void PostOperationUpdateHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                var entitlementRef = ((Entity)context.InputParameters["Target"]).ToEntity<Entitlement>();
                var entitlement = new Entitlement();
                entitlement = service.Retrieve(Entitlement.EntityLogicalName,
                              entitlementRef.Id, new ColumnSet(
                                  LogicalNameof<Entitlement>.Property(x => x.StatusCode),
                                  LogicalNameof<Entitlement>.Property(x => x.ipg_EntitlementType),
                                  LogicalNameof<Entitlement>.Property(x => x.ipg_FacilityId),
                                  LogicalNameof<Entitlement>.Property(x => x.ipg_CarrierId),
                                  LogicalNameof<Entitlement>.Property(x => x.StartDate),
                                  LogicalNameof<Entitlement>.Property(x => x.EndDate))).ToEntity<Entitlement>();

                if (entitlement.ipg_EntitlementType.Value == (int)ipg_EntitlementTypes.FacilityCarrier)
                {
                    if (entitlementRef.Contains(entitlement.GetAttributeLogicalName(x => x.StatusCode)))
                    {
                        throw new Exception("Cant activate/deactivate facility carrier contract");
                    }

                    ValidateOverlappingEntitlementBasedOnFacilityCarrierAndAcceptanceDate(service, entitlement);
                }
            }

        }

        private static void ValidateOverlappingEntitlementBasedOnFacilityCarrierAndAcceptanceDate(IOrganizationService service, Entitlement entitlement)
        {
            var queryExpression = new QueryExpression(Entitlement.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(LogicalNameof<Entitlement>.Property(x => x.EntitlementId),
                                          LogicalNameof<Entitlement>.Property(x => x.Name)),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Filters = {
                                new FilterExpression {
                                FilterOperator = LogicalOperator.And,
                                Conditions =
                                    {
                                       new ConditionExpression(LogicalNameof<Entitlement>.Property(e => e.EntitlementId), ConditionOperator.NotEqual, entitlement.EntitlementId),
                                       new ConditionExpression(LogicalNameof<Entitlement>.Property(e => e.ipg_FacilityId), ConditionOperator.Equal, entitlement.ipg_FacilityId.Id),
                                       new ConditionExpression(LogicalNameof<Entitlement>.Property(e => e.ipg_CarrierId), ConditionOperator.Equal, entitlement.ipg_CarrierId.Id),
                                       new ConditionExpression(LogicalNameof<Entitlement>.Property(e => e.ipg_EntitlementType), ConditionOperator.Equal, (int)entitlement.ipg_EntitlementType.Value),
                                       new ConditionExpression(LogicalNameof<Entitlement>.Property(x => x.StartDate), ConditionOperator.LessEqual, entitlement.EndDate),
                                       new ConditionExpression(LogicalNameof<Entitlement>.Property(x => x.EndDate), ConditionOperator.GreaterEqual, entitlement.StartDate)
                                    }
                                }
                            }

                }
            };


            var overlappingEntitlement = service.RetrieveMultiple(queryExpression);

            if (overlappingEntitlement.Entities.Any())
            {
                throw new Exception($"There is a overlapping entitlement name " +
                    $"{overlappingEntitlement.Entities.FirstOrDefault().ToEntity<Entitlement>().Name} " +
                    $"in this entitlement.");
            }
        }
    }
}
