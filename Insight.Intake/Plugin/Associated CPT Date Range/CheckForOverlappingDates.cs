using Insight.Intake.Extensions;
using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Insight.Intake.Plugin.AssociatedCPTDateRange
{
    public class CheckForOverlappingDates : PluginBase
    {
        public CheckForOverlappingDates() : base(typeof(CheckForOverlappingDates))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_associatedcptdaterange.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_associatedcptdaterange.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            if (((Entity)context.InputParameters["Target"]).LogicalName != ipg_associatedcptdaterange.EntityLogicalName) return;

            var targetEntity = ((Entity)context.InputParameters["Target"]);

            if (localPluginContext.PluginExecutionContext.MessageName == MessageNames.Create ||
                localPluginContext.PluginExecutionContext.MessageName == MessageNames.Update)
            {
                var associatedCPTDateRange = service.Retrieve(ipg_associatedcptdaterange.EntityLogicalName,
                                                         targetEntity.Id, new ColumnSet(true))
                                                         .ToEntity<ipg_associatedcptdaterange>();
                //https://eti-ipg.atlassian.net/browse/CPI-11252
                //At the time of deleting parent associated cpt, this record is getting updated
                //since the parent is already deleted, the query below was throwing an exception.
                if (associatedCPTDateRange.ipg_AssociatedCPT == null) return;

                var associatedCptDateRangeQueryExpression = new QueryExpression
                {
                    EntityName = ipg_associatedcptdaterange.EntityLogicalName,
                    ColumnSet = new ColumnSet("ipg_expirationdate", "ipg_effectivedate", "ipg_associatedcptdaterangeid"),
                    Criteria = new FilterExpression(LogicalOperator.And)
                    {
                        Filters = {
                        new FilterExpression {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                            {
                                new ConditionExpression(LogicalNameof<ipg_associatedcptdaterange>.Property(e => e.ipg_AssociatedCPT), ConditionOperator.Equal, associatedCPTDateRange.ipg_AssociatedCPT.Id),
                                new ConditionExpression(LogicalNameof<ipg_associatedcptdaterange>.Property(e => e.ipg_associatedcptdaterangeId), ConditionOperator.NotEqual, associatedCPTDateRange.ipg_associatedcptdaterangeId)

                            }
                        },
                        new FilterExpression {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                            {
                                new ConditionExpression(LogicalNameof<ipg_associatedcptdaterange>.Property(x => x.ipg_EffectiveDate), ConditionOperator.LessEqual, associatedCPTDateRange.ipg_ExpirationDate),
                                new ConditionExpression(LogicalNameof<ipg_associatedcptdaterange>.Property(x => x.ipg_ExpirationDate), ConditionOperator.GreaterEqual, associatedCPTDateRange.ipg_EffectiveDate)
                            }
                        }
                    }

                    },
                };
                var overlappingAssociatedCPT = service.RetrieveMultiple(associatedCptDateRangeQueryExpression).Entities;
                if (overlappingAssociatedCPT.Count > 0)
                {
                    throw new InvalidPluginExecutionException("Dates are overlapping with other dates.");
                }
            }
        }
    }
}
