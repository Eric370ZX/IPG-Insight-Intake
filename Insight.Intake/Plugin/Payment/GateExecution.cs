using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Insight.Intake.Extensions;
using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Payment
{
    public class GateExecution : PluginBase
    {
        public GateExecution() : base(typeof(GateExecution))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_payment.EntityLogicalName, PostOperationCreateHandler);
        }

        private void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var payment = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_payment>();

            if (payment.ipg_CaseId == null)
            {
                return;
            }

            if (((payment.ipg_TotalInsurancePaid ?? 0) != 0) || ((payment.ipg_MemberPaid_new == null ? 0 : payment.ipg_MemberPaid_new.Value) != 0))
            {
                var lifecycleStepId = service.GetGlobalSettingValueByKey("Lifecyclestep.Collections");
                var incident = service.Retrieve(payment.ipg_CaseId.LogicalName, payment.ipg_CaseId.Id, new ColumnSet(Incident.Fields.ipg_lifecyclestepid)).ToEntity<Incident>();
                if(string.Equals(incident.ipg_lifecyclestepid.Id.ToString(), lifecycleStepId))
                {
                    var actionParams = new Dictionary<string, object>() { { "Target", payment.ipg_CaseId } };
                    service.ExecuteAction(Constants.ActionNames.GatingStartGateProcessing, actionParams);
                }
            }
        }
    }
}
