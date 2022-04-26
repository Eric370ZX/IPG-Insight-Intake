using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Insight.Intake.Extensions;

namespace Insight.Intake.Plugin.AssociatedCpt
{
    public class CheckCarrierCPTOverlappingDates : PluginBase
    {
        public CheckCarrierCPTOverlappingDates() : base(typeof(CheckCarrierCPTOverlappingDates))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_associatedcpt.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_associatedcpt.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {

            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            if (((Entity)context.InputParameters["Target"]).LogicalName != ipg_associatedcpt.EntityLogicalName) return;

            var targetEntity = ((Entity)context.InputParameters["Target"]);

            if (localPluginContext.PluginExecutionContext.MessageName == MessageNames.Create ||
                localPluginContext.PluginExecutionContext.MessageName == MessageNames.Update)
            {
                var associatedCPT = service.Retrieve(ipg_associatedcpt.EntityLogicalName,
                                                         targetEntity.Id, new ColumnSet(
                                                             LogicalNameof<ipg_associatedcpt>.Property(x => x.ipg_EffectiveDate),
                                                             LogicalNameof<ipg_associatedcpt>.Property(x => x.ipg_ExpirationDate),
                                                             LogicalNameof<ipg_associatedcpt>.Property(x => x.ipg_ExpirationDate),
                                                             LogicalNameof<ipg_associatedcpt>.Property(x => x.ipg_CPTCodeId),
                                                             LogicalNameof<ipg_associatedcpt>.Property(x => x.ipg_CarrierId))
                                                         )
                                                         .ToEntity<ipg_associatedcpt>();

                var associatedCptQueryExpression = new QueryExpression
                {
                    EntityName = ipg_associatedcpt.EntityLogicalName,
                    ColumnSet = new ColumnSet(LogicalNameof<ipg_associatedcpt>.Property(x => x.ipg_EffectiveDate),
                                                             LogicalNameof<ipg_associatedcpt>.Property(x => x.ipg_ExpirationDate),
                                                             LogicalNameof<ipg_associatedcpt>.Property(x => x.ipg_ExpirationDate)),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Filters = {
                        new FilterExpression {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                            {
                                new ConditionExpression(LogicalNameof<ipg_associatedcpt>.Property(e => e.ipg_CPTCodeId), ConditionOperator.Equal, associatedCPT.ipg_CPTCodeId.Id),
                                new ConditionExpression(LogicalNameof<ipg_associatedcpt>.Property(e => e.ipg_CarrierId), ConditionOperator.Equal, associatedCPT.ipg_CarrierId.Id),
                                new ConditionExpression(LogicalNameof<ipg_associatedcpt>.Property(e => e.ipg_associatedcptId), ConditionOperator.NotEqual, associatedCPT.ipg_associatedcptId)

                            }
                        },
                        new FilterExpression {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                            {
                                new ConditionExpression(LogicalNameof<ipg_associatedcpt>.Property(x => x.ipg_EffectiveDate), ConditionOperator.LessEqual, associatedCPT.ipg_ExpirationDate),
                                new ConditionExpression(LogicalNameof<ipg_associatedcpt>.Property(x => x.ipg_ExpirationDate), ConditionOperator.GreaterEqual, associatedCPT.ipg_EffectiveDate)
                            }
                        }
                    }

                    },
                };
                var overlappingAssociatedCPT = service.RetrieveMultiple(associatedCptQueryExpression).Entities;
                if (overlappingAssociatedCPT.Count > 0)
                {
                    throw new InvalidPluginExecutionException("Dates are overlapping with other dates.");
                }
            }
        }
    }
}
