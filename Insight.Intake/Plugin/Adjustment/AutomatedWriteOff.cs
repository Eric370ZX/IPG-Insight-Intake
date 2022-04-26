using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Adjustment
{
    public class AutomatedWriteOff : PluginBase
    {
        public AutomatedWriteOff() : base(typeof(AutomatedWriteOff))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGIntakeActionsAutomatedWriteOff", null, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                EntityCollection cases = localPluginContext.GetInput<EntityCollection>("Cases");

                var queryExpression = new QueryExpression(Incident.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(Incident.PrimaryIdAttribute, Incident.Fields.ipg_RemainingPatientBalance.ToLower()),
                    Criteria = new FilterExpression(LogicalOperator.Or)
                };
                foreach (Entity incident in cases.Entities)
                {
                    queryExpression.Criteria.Conditions.Add(new ConditionExpression(Incident.PrimaryIdAttribute, ConditionOperator.Equal, incident.Id));
                }

                var patientBalances = service.RetrieveMultiple(queryExpression).Entities.Select(i => i.ToEntity<Incident>());

                foreach (var incident in patientBalances)
                {
                    ipg_adjustment adjustment = new ipg_adjustment()
                    {
                        ipg_AdjustmentTypeEnum = ipg_AdjustmentTypes.WriteOff,
                        ipg_AmountType = false,
                        ipg_ApplyToEnum = ipg_PayerType.Patient,
                        ipg_CaseId = incident.ToEntityReference(),
                        ipg_Percent = 100,
                        ipg_ReasonEnum = ipg_AdjustmentReasons.WOSmallBalance,
                        ipg_Amount = incident.ipg_RemainingPatientBalance,
                        ipg_AmountToApply = incident.ipg_RemainingPatientBalance
                    };

                    service.Create(adjustment);
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }

    }
}
