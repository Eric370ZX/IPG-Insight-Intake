using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Gating.PostProcess
{
    public class Gate2 : PluginBase
    {
        public Gate2() : base(typeof(Gate2)) =>
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingPostProcessGate2", null, PostOperationHandler);

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var targetRef = localPluginContext.TargetRef();
            
            if(targetRef.LogicalName == Incident.EntityLogicalName)
            {
                var target = service
                .Retrieve<Incident>(targetRef.LogicalName, targetRef.Id, new ColumnSet(Incident.Fields.ipg_EBVResult, Incident.Fields.ipg_MemberIdNumber));
                if (target.ipg_MemberIdNumber?.ToLower().StartsWith("jqu") != true)
                {
                    if (target.ipg_EBVResultEnum != ipg_EBVResults.FAILED)
                    {
                        localPluginContext.Trace($"{nameof(Gate2)} - start benefit verification execution");
                        service.ExecuteWorkflow(Insight.Intake.Helpers.Constants.Workflows.VerifyBenefitsAsyncId, targetRef.Id);
                    }
                }
            }
            
            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = true;
            localPluginContext.PluginExecutionContext.OutputParameters["Output"] = "";
        }
    }
}