using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Insight.Intake.Extensions;
using System.Linq;

namespace Insight.Intake.Plugin.FacilityCarrierCPTRuleContract
{
    public class ValidateOverlapFacilityCarrierCPTRuleContract : PluginBase
    {
        public ValidateOverlapFacilityCarrierCPTRuleContract() : base(typeof(ValidateOverlapFacilityCarrierCPTRuleContract))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_facilitycarriercptrulecontract.EntityLogicalName, PostOperationCreateAndUpdateHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_facilitycarriercptrulecontract.EntityLogicalName, PostOperationCreateAndUpdateHandler);
        }

        void PostOperationCreateAndUpdateHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;
            var facilitycarriercptrulecontractRef = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_facilitycarriercptrulecontract>();

            tracingService.Trace($"{typeof(ValidateOverlapFacilityCarrierCPTRuleContract)} plugin started");

            if (((Entity)context.InputParameters["Target"]).LogicalName != ipg_facilitycarriercptrulecontract.EntityLogicalName)
            {
                return;
            }

            if(facilitycarriercptrulecontractRef.ipg_EntitlementId == null)
            {
                return;
            }

            var facilitycarriercptrulecontract = service.Retrieve(ipg_facilitycarriercptrulecontract.EntityLogicalName,
                                     facilitycarriercptrulecontractRef.Id, new ColumnSet(true)).ToEntity<ipg_facilitycarriercptrulecontract>();


            ValidateOverlappingRules(service, facilitycarriercptrulecontract);
            ValidateRuleDataIsWithinEntitlementDate(service, facilitycarriercptrulecontract);
        }

        private void ValidateRuleDataIsWithinEntitlementDate(IOrganizationService service, ipg_facilitycarriercptrulecontract facilityCarrierCPTRuleContract)
        {
            var entitlement = service.Retrieve(Entitlement.EntityLogicalName, facilityCarrierCPTRuleContract.ipg_EntitlementId.Id,
                                               new ColumnSet(LogicalNameof<Entitlement>.Property(x => x.StartDate),
                                                             LogicalNameof<Entitlement>.Property(x => x.EndDate)))
                                               .ToEntity<Entitlement>();
            if (facilityCarrierCPTRuleContract.ipg_EffectiveDate.Value.Date < entitlement.StartDate.Value.Date ||
                facilityCarrierCPTRuleContract.ipg_ExpirationDate.Value.Date > entitlement.EndDate.Value.Date)
            {
                throw new System.Exception($"The rule date range {facilityCarrierCPTRuleContract.ipg_EffectiveDate.Value.ToShortDateString()} - {facilityCarrierCPTRuleContract.ipg_ExpirationDate.Value.ToShortDateString()} " +
                    $"is outside parent Entitlements's date range {entitlement.StartDate.Value.ToShortDateString()} - {entitlement.EndDate.Value.ToShortDateString()}.");
            }
        }

        private void ValidateOverlappingRules(IOrganizationService service, ipg_facilitycarriercptrulecontract facilitycarriercptrulecontract)
        {
            var queryExpression = new QueryExpression(ipg_facilitycarriercptrulecontract.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(LogicalNameof<ipg_facilitycarriercptrulecontract>.Property(x => x.ipg_EntitlementId),
                                          LogicalNameof<ipg_facilitycarriercptrulecontract>.Property(x => x.ipg_name)),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Filters = {
                        new FilterExpression {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                            {
                                new ConditionExpression(LogicalNameof<ipg_facilitycarriercptrulecontract>.Property(e => e.ipg_EntitlementId), ConditionOperator.Equal, facilitycarriercptrulecontract.ipg_EntitlementId.Id),
                                new ConditionExpression(LogicalNameof<ipg_facilitycarriercptrulecontract>.Property(e => e.ipg_facilitycarriercptrulecontractId), ConditionOperator.NotEqual, facilitycarriercptrulecontract.ipg_facilitycarriercptrulecontractId)
                            }
                        },
                        new FilterExpression {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                            {
                                new ConditionExpression(LogicalNameof<ipg_facilitycarriercptrule>.Property(x => x.ipg_EffectiveDate), ConditionOperator.LessEqual, facilitycarriercptrulecontract.ipg_ExpirationDate),
                                new ConditionExpression(LogicalNameof<ipg_facilitycarriercptrule>.Property(x => x.ipg_ExpirationDate), ConditionOperator.GreaterEqual, facilitycarriercptrulecontract.ipg_EffectiveDate)

                            }
                        }
                    }

                }
            };

            var overlappingRules = service.RetrieveMultiple(queryExpression);

            if (overlappingRules.Entities.Any())
            {
                throw new System.Exception($"There is a overlapping rule name " +
                    $"{overlappingRules.Entities.FirstOrDefault().ToEntity<ipg_facilitycarriercptrulecontract>().ipg_name} " +
                    $"in this entitlement.");
            }
        }
    }
}
