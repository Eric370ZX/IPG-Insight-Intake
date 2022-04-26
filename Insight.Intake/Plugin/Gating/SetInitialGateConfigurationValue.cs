using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace Insight.Intake.Plugin.Gating
{
    public class SetInitialGateConfigurationValue : PluginBase
    {
        public SetInitialGateConfigurationValue() : base(typeof(SetInitialGateConfigurationValue))
        {
            RegisterEvent(PipelineStages.PreValidation, MessageNames.Create, ipg_referral.EntityLogicalName, PreValidationCreateHandler);
        }

        private void PreValidationCreateHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;

            // The InputParameters collection contains all the data passed in the message request. 
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parmameters.
                var targetEntity = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_referral>();
                var lifecycleStep = GetInitialLifeCycleStep(service, targetEntity);
                if (lifecycleStep != null)
                {
                    targetEntity.ipg_lifecyclestepid = lifecycleStep.ToEntityReference();
                    targetEntity.ipg_gateconfigurationid = lifecycleStep.ToEntity<ipg_lifecyclestep>().ipg_gateconfigurationid;
                }
            }
        }

        private Entity GetInitialLifeCycleStep(IOrganizationService service, ipg_referral initReferral)
        {
            QueryExpression query = new QueryExpression(ipg_lifecyclestep.EntityLogicalName)
            {
                ColumnSet = new ColumnSet("ipg_gateconfigurationid"),
                TopCount = 1
            };
            if (initReferral.ipg_OriginEnum == Incident_CaseOriginCode.EHR)
            {
                query.Criteria.AddCondition("ipg_starttype", ConditionOperator.Equal, (int)ipg_Gatingstarttype.EHR);//EHR
            }
            query.AddOrder("ipg_executionorder", OrderType.Ascending);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            EntityCollection coll = service.RetrieveMultiple(query);

            return coll.Entities.Count > 0 ? coll.Entities.First() : null;
        }

        private EntityReference GetInitialGateConfiguration(IOrganizationService service)
        {
            QueryExpression query = new QueryExpression(ipg_gateconfiguration.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false),
                TopCount = 1
            };
            query.AddOrder("ipg_executionorder", OrderType.Ascending);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            EntityCollection coll = service.RetrieveMultiple(query);

            return coll.Entities.Count > 0 ? coll.Entities.First().ToEntityReference() : null;
        }
    }
}