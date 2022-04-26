using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating.PreProcess
{
    public class Gate2 : PluginBase
    {
        public Gate2() : base(typeof(Gate2))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingPreProcessGate2", null, GatingPreProcessHandler);
        }
        private void GatingPreProcessHandler(LocalPluginContext localPluginContext)
        {
            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = true;
            localPluginContext.PluginExecutionContext.OutputParameters["Output"] = "";
        }

        private void RunDeriveHomePlan(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var targetRef = localPluginContext.TargetRef();
            if (targetRef?.LogicalName == ipg_referral.EntityLogicalName)
            {
                context.OutputParameters["Succeeded"] = true;
                return;
            }
            context.OutputParameters["Succeeded"] = false;

            if (targetRef != null)
            {
                string[] columns = {
                    Incident.Fields.ipg_CarrierId,
                    Incident.Fields.ipg_SurgeryDate,
                    Incident.Fields.ipg_SecondaryCarrierId,
                    Incident.Fields.ipg_MemberIdNumber,
                    Incident.Fields.ipg_SecondaryMemberIdNumber
                };
                var caseRecord = service.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet(columns));

                var preProcessManager = new PreProcessManager(service, targetRef);
                preProcessManager.DeleteAllDeriveHomePlanOnCase(caseRecord.ToEntityReference());
                var result = preProcessManager.DeriveHomePlan(caseRecord, null);
                context.OutputParameters["Succeeded"] = result;
            }
        }
    }
}
