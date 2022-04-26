using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Insight.Intake.Extensions;
using System.Linq;

namespace Insight.Intake.Plugin.FacilityCarrierCPTRule
{
    public class ValidateOverlappingCPTRules : PluginBase
    {
        public ValidateOverlappingCPTRules() : base(typeof(ValidateOverlappingCPTRules))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_facilitycarriercptrule.EntityLogicalName, PostOperationCreateAndUpdateHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_facilitycarriercptrule.EntityLogicalName, PostOperationCreateAndUpdateHandler);
        }

        void PostOperationCreateAndUpdateHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;
            tracingService.Trace($"{typeof(ValidateOverlappingCPTRules)} plugin started");

            if (((Entity)context.InputParameters["Target"]).LogicalName != ipg_facilitycarriercptrule.EntityLogicalName) 
            {
                return;
            }
            var facilitycarriercptruleref = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_facilitycarriercptrule>();

            var facilityCarrierCPTRule = service.Retrieve(ipg_facilitycarriercptrule.EntityLogicalName,
                                     facilitycarriercptruleref.Id, new ColumnSet(true)).ToEntity<ipg_facilitycarriercptrule>();


            ValidateOverlappingRules(service, facilityCarrierCPTRule);
            ValidateRuleDataIsWithinEntitlementDate(service, facilityCarrierCPTRule);
        }

        private void ValidateRuleDataIsWithinEntitlementDate(IOrganizationService service, ipg_facilitycarriercptrule facilityCarrierCPTRule)
        {
            var facilitycarriercptrulecontract = service.Retrieve(ipg_facilitycarriercptrulecontract.EntityLogicalName, facilityCarrierCPTRule.ipg_FacilityCarrierCPTRuleContractId.Id,
                                               new ColumnSet(LogicalNameof<ipg_facilitycarriercptrulecontract>.Property(x => x.ipg_EffectiveDate),
                                                             LogicalNameof<ipg_facilitycarriercptrulecontract>.Property(x => x.ipg_ExpirationDate)))
                                               .ToEntity<ipg_facilitycarriercptrulecontract>();
            if (facilityCarrierCPTRule.ipg_EffectiveDate.Value.Date < facilitycarriercptrulecontract.ipg_EffectiveDate.Value.Date ||
                facilityCarrierCPTRule.ipg_ExpirationDate.Value.Date > facilitycarriercptrulecontract.ipg_ExpirationDate.Value.Date)
            {
                throw new System.Exception($"The rule date range {facilityCarrierCPTRule.ipg_EffectiveDate.Value.ToShortDateString()} - {facilityCarrierCPTRule.ipg_ExpirationDate.Value.ToShortDateString()} " +
                    $"is outside parent Entitlements's date range {facilitycarriercptrulecontract.ipg_EffectiveDate.Value.ToShortDateString()} - {facilitycarriercptrulecontract.ipg_ExpirationDate.Value.ToShortDateString()}.");
            }
        }

        private void ValidateOverlappingRules(IOrganizationService service, ipg_facilitycarriercptrule facilityCarrierCPTRule)
        {
            var queryExpression = new QueryExpression(ipg_facilitycarriercptrule.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(LogicalNameof<ipg_facilitycarriercptrule>.Property(x => x.ipg_FacilityCarrierCPTRuleContractId),
                                          LogicalNameof<ipg_facilitycarriercptrule>.Property(x => x.ipg_name)),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Filters = {
                        new FilterExpression {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                            {
                                new ConditionExpression(LogicalNameof<ipg_facilitycarriercptrule>.Property(e => e.ipg_FacilityCarrierCPTRuleContractId), ConditionOperator.Equal, facilityCarrierCPTRule.ipg_FacilityCarrierCPTRuleContractId.Id),
                                new ConditionExpression(LogicalNameof<ipg_facilitycarriercptrule>.Property(e => e.ipg_facilitycarriercptruleId), ConditionOperator.NotEqual, facilityCarrierCPTRule.ipg_facilitycarriercptruleId),
                                new ConditionExpression(LogicalNameof<ipg_facilitycarriercptrule>.Property(e => e.ipg_CptId), ConditionOperator.Equal, facilityCarrierCPTRule.ipg_CptId.Id)

                            }
                        },
                        new FilterExpression {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                            {
                                new ConditionExpression(LogicalNameof<ipg_facilitycarriercptrule>.Property(x => x.ipg_EffectiveDate), ConditionOperator.LessEqual, facilityCarrierCPTRule.ipg_ExpirationDate),
                                new ConditionExpression(LogicalNameof<ipg_facilitycarriercptrule>.Property(x => x.ipg_ExpirationDate), ConditionOperator.GreaterEqual, facilityCarrierCPTRule.ipg_EffectiveDate)

                            }
                        }
                    }

                }
            };

            var overlappingRules = service.RetrieveMultiple(queryExpression);

            if (overlappingRules.Entities.Any())
            {
                throw new System.Exception($"There is a overlapping rule name " +
                    $"{overlappingRules.Entities.FirstOrDefault().ToEntity<ipg_facilitycarriercptrule>().ipg_name} " +
                    $"in this entitlement.");
            }
        }
    }
}
