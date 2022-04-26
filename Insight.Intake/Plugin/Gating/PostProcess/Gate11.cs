using Insight.Intake.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating.PostProcess
{
    public class Gate11 : PluginBase
    {
        public Gate11() : base(typeof(Gate12))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingPostProcessGate11", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var targetRef = localPluginContext.TargetRef();
            var postProcessManager = new PostProcessManager(localPluginContext.OrganizationService, localPluginContext.TracingService, targetRef);
            var minimumSeverityLevel = localPluginContext.PluginExecutionContext.InputParameters.Contains("MinimumSeverityLevel") ? (localPluginContext.PluginExecutionContext.InputParameters["MinimumSeverityLevel"] as int?) :-1;
            if (minimumSeverityLevel == (int)ipg_SeverityLevel.Warning)
            {
                postProcessManager.CreateCarrierPaymentStatementGenerationTask(targetRef);
            }
            else if (minimumSeverityLevel == (int)ipg_SeverityLevel.Error)
            {
                postProcessManager.UpdateStatementTasksStartAndDueDate(targetRef);
            }
            else if (minimumSeverityLevel == (int)ipg_SeverityLevel.Info)
            {
                var note = "Case Closed by System for $0.00 Balance.";
                postProcessManager.CreateNote(note, note);
            }
            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = true;
            localPluginContext.PluginExecutionContext.OutputParameters["Output"] = "";
        }
    }
}
