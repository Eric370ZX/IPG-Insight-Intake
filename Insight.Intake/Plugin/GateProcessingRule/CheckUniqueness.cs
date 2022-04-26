using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.GateProcessingRule
{
    public class CheckUniqueness : PluginBase
    {
        public CheckUniqueness() : base(typeof(CheckUniqueness))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_gateprocessingrule.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_gateprocessingrule.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var tracing = localPluginContext.TracingService;

            ipg_gateprocessingrule gateProcessingRule = service.Retrieve(context.PrimaryEntityName, context.PrimaryEntityId, new ColumnSet(
                ipg_gateprocessingrule.Fields.ipg_gateconfigurationid
                , ipg_gateprocessingrule.Fields.ipg_severitylevel
                , ipg_gateprocessingrule.Fields.ipg_actioncode
                , ipg_gateprocessingrule.Fields.ipg_lifecyclestepid)).ToEntity<ipg_gateprocessingrule>();

            tracing.Trace($"Action {gateProcessingRule.ipg_actioncodeEnum?.ToString()}");

            if (gateProcessingRule.ipg_actioncodeEnum != ipg_gateprocessingrule_ipg_actioncode.ReOpen && (gateProcessingRule.ipg_gateconfigurationid == null || gateProcessingRule.ipg_severitylevel == null))
            {
                throw new InvalidPluginExecutionException("Pleasae fill all required fields.");
            }


            if (FindDuplicates(service, gateProcessingRule, tracing))
            {
                throw new InvalidPluginExecutionException("Gate Processing Rule with the same set of GateConfiguration and SeverityLevel already exists.");
            }
        }

        private bool FindDuplicates(IOrganizationService service, ipg_gateprocessingrule gateProcessingRule, ITracingService tracing)
        {
            QueryExpression query = new QueryExpression(ipg_gateprocessingrule.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false)
            };

            query.Criteria.AddCondition(ipg_gateprocessingrule.Fields.StateCode, ConditionOperator.Equal, 0);
            query.Criteria.AddCondition(ipg_gateprocessingrule.Fields.ipg_gateprocessingruleId, ConditionOperator.NotEqual, gateProcessingRule.Id);

            if (gateProcessingRule.ipg_gateconfigurationid != null)
            {
                query.Criteria.AddCondition(ipg_gateprocessingrule.Fields.ipg_gateconfigurationid, ConditionOperator.Equal, gateProcessingRule.ipg_gateconfigurationid.Id);
            }
            else
            {
                tracing.Trace($"{ipg_gateprocessingrule.Fields.ipg_gateconfigurationid} is null");

                if (gateProcessingRule.ipg_lifecyclestepid != null)
                {
                    tracing.Trace($"Check for LF addedc {gateProcessingRule.ipg_lifecyclestepid.Id}");

                    query.Criteria.AddCondition(ipg_gateprocessingrule.Fields.ipg_lifecyclestepid, ConditionOperator.Equal, gateProcessingRule.ipg_lifecyclestepid.Id);
                }

                query.Criteria.AddCondition(ipg_gateprocessingrule.Fields.ipg_gateconfigurationid, ConditionOperator.Null);
            }

            if (gateProcessingRule.ipg_severitylevel != null)
            {
                query.Criteria.AddCondition(ipg_gateprocessingrule.Fields.ipg_severitylevel, ConditionOperator.Equal, gateProcessingRule.ipg_severitylevel.Value);
            }
            else
            {
                tracing.Trace($"{ipg_gateprocessingrule.Fields.ipg_severitylevel} is null");
                query.Criteria.AddCondition(ipg_gateprocessingrule.Fields.ipg_severitylevel, ConditionOperator.Null);
            }

            if (gateProcessingRule.ipg_actioncode != null)
            {
                query.Criteria.AddCondition(ipg_gateprocessingrule.Fields.ipg_actioncode, ConditionOperator.Equal, gateProcessingRule.ipg_actioncode.Value);
            }
            else
            {
                tracing.Trace($"{ipg_gateprocessingrule.Fields.ipg_actioncode} is null");
                query.Criteria.AddCondition(ipg_gateprocessingrule.Fields.ipg_actioncode, ConditionOperator.Equal, null);
            }


            var result = service.RetrieveMultiple(query).Entities;

            tracing.Trace($"Duplicate for Gate Processing Rule id: {result.FirstOrDefault()?.Id}");

            return result.Any();
        }
    }
}