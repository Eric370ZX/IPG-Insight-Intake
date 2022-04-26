using Insight.Intake.Helpers;
using Insight.Intake.Repositories;
namespace Insight.Intake.Plugin.Gating.PostProcess
{
    public class Gate9 : PluginBase
    {
        public Gate9() : base(typeof(Gate9))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingPostProcessGate9", null, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var targetRef = localPluginContext.TargetRef();
            var postProcessManager = new PostProcessManager(localPluginContext.OrganizationService, localPluginContext.TracingService, targetRef);

            var primaryCarrier = postProcessManager.GetCarrier();
            int followUpTask = 30;
            if (primaryCarrier.Name?.ToUpper().Contains("BCBSNC") == true)
            {
                followUpTask = 21;
            }
            var subject = $"Follow up regarding a claim payment";
            var body = $"Claim has not yet adjudicated. Follow up with {primaryCarrier.Name}";
            var teamName = localPluginContext.OrganizationService.GetGlobalSettingValueByKey("Carrier Services team");
            var userTeam = postProcessManager.GetTeam(teamName);
            postProcessManager.CreateUserTask(subject, body, followUpTask, userTeam.ToEntityReference(), targetRef);

            var minimumSeverityLevel = localPluginContext.PluginExecutionContext.InputParameters.Contains("MinimumSeverityLevel") ? (localPluginContext.PluginExecutionContext.InputParameters["MinimumSeverityLevel"] as int?) : -1;
            if (minimumSeverityLevel == (int)ipg_SeverityLevel.Info
                || minimumSeverityLevel == (int)ipg_SeverityLevel.Warning)
            {
                postProcessManager.CancelStatementTasks(targetRef);
                postProcessManager.DeactivateStatementRecordsExceptP1(targetRef);

                localPluginContext.Trace($"Create a new { PSEvents.PromotedToCollection1} statement generation task if not exists");
                postProcessManager.CreateStatementGenerationTaskIfNotExists(targetRef, PSEvents.PromotedToCollection1);
                localPluginContext.Trace($"Create a new { PSEvents.PromotedToCollection2} statement generation task if not exists");
                postProcessManager.CreateStatementGenerationTaskIfNotExists(targetRef, PSEvents.PromotedToCollection2);
            }

            postProcessManager.SetCollectionDate();
            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = true;
            localPluginContext.PluginExecutionContext.OutputParameters["Output"] = "";
        }
    }
}
