using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Gating.PostProcess
{
    public class GateEHR : PluginBase
    {
        public GateEHR() : base(typeof(GateEHR)) =>
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingPostProcessGateEHR", null, PostOperationHandler);

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var targetRef = localPluginContext.TargetRef();
            var target = service
                .Retrieve<Incident>(targetRef.LogicalName, targetRef.Id, new ColumnSet(Incident.Fields.ipg_EBVResult));

            if (target.ipg_EBVResultEnum != ipg_EBVResults.FAILED)
            {
                localPluginContext.Trace($"{nameof(GateEHR)} - start benefit verification execution");
                service.ExecuteWorkflow(Insight.Intake.Helpers.Constants.Workflows.VerifyBenefitsAsyncId, targetRef.Id);
            }

            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = true;
            localPluginContext.PluginExecutionContext.OutputParameters["Output"] = "";
        }
    }
}