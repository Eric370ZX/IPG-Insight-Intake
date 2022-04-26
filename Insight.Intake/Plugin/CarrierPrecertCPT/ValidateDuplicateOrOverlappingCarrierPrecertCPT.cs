using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.CarrierPrecertCPT
{
    public class ValidateDuplicateOrOverlappingCarrierPrecertCPT : PluginBase
    {

        public ValidateDuplicateOrOverlappingCarrierPrecertCPT() : base(typeof(ValidateDuplicateOrOverlappingCarrierPrecertCPT))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_carrierprecertcpt.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_carrierprecertcpt.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext context)
        {
            var service = context.OrganizationService;

            var target = context.Target<ipg_carrierprecertcpt>();
            var tracintService = context.TracingService;

            var carrierPrecert = service.Retrieve(ipg_carrierprecertcpt.EntityLogicalName, target.Id, new ColumnSet(true)).ToEntity<ipg_carrierprecertcpt>();

            tracintService.Trace("ValidateDuplicateOrOverlappingCarrierPrecertCPT started.");

            var query = new QueryExpression
            {
                EntityName = ipg_carrierprecertcpt.EntityLogicalName,
                ColumnSet = new ColumnSet(ipg_carrierprecertcpt.Fields.Id),
                Criteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Filters =
                                    {
                                        new FilterExpression
                                        {
                                            FilterOperator = LogicalOperator.Or,
                                            Conditions =
                                            {
                                                new ConditionExpression(ipg_carrierprecertcpt.Fields.ipg_EffectiveEndDate, ConditionOperator.Null),
                                                new ConditionExpression(ipg_carrierprecertcpt.Fields.ipg_EffectiveEndDate, ConditionOperator.GreaterEqual, carrierPrecert.ipg_EffectiveStartDate)
                                            }
                                        },
                                        new FilterExpression
                                        {
                                            FilterOperator = LogicalOperator.And,
                                            Conditions =
                                            {
                                                new ConditionExpression(ipg_carrierprecertcpt.Fields.ipg_EffectiveStartDate, ConditionOperator.LessEqual, carrierPrecert.ipg_EffectiveEndDate),
                                                new ConditionExpression(ipg_carrierprecertcpt.Fields.ipg_CPTId, ConditionOperator.Equal, carrierPrecert.ipg_CPTId.Id),
                                                new ConditionExpression(ipg_carrierprecertcpt.Fields.ipg_CarrierId, ConditionOperator.Equal, carrierPrecert.ipg_CarrierId.Id),
                                                new ConditionExpression(ipg_carrierprecertcpt.Fields.ipg_carrierprecertcptId, ConditionOperator.NotEqual, carrierPrecert.ipg_carrierprecertcptId)
                                            }
                                        },
                                    }
                }
            };

            var duplicateOrOverlappingRecord = service.RetrieveMultiple(query);

            if(duplicateOrOverlappingRecord.Entities.Any())
            {
                throw new Exception($"Duplicate record with overlapping date already exists: {duplicateOrOverlappingRecord.Entities.FirstOrDefault().Id}.");
            }

        }
    }
}
