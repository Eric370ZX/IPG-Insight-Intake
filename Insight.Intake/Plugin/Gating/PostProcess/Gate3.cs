using Insight.Intake.Repositories;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Insight.Intake.Plugin.Gating.PostProcess
{
    public class Gate3 : PluginBase
    {
        public Gate3() : base(typeof(Gate3))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingPostProcessGate3", null, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var targetRef = localPluginContext.TargetRef(); 
            var postProcessManager = new PostProcessManager(localPluginContext.OrganizationService, localPluginContext.TracingService, targetRef);
            var minimumSeverityLevel = localPluginContext.PluginExecutionContext.InputParameters.Contains("MinimumSeverityLevel") ? (localPluginContext.PluginExecutionContext.InputParameters["MinimumSeverityLevel"] as int?) : -1;
            if (minimumSeverityLevel == (int)ipg_SeverityLevel.Info
                || minimumSeverityLevel == (int)ipg_SeverityLevel.Warning)
            {
                localPluginContext.Trace($"Create a new { PSEvents.CaseApproved} statement generation task if not exists");

                postProcessManager.CreateStatementGenerationTaskIfNotExists(targetRef, PSEvents.CaseApproved);

                if (targetRef.LogicalName == Incident.EntityLogicalName)
                {
                    var incident = localPluginContext.OrganizationService
                        .Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet(Incident.Fields.ipg_ActualDOS))
                        .ToEntity<Incident>();
                    if (incident.ipg_ActualDOS < DateTime.Now)
                    {
                        postProcessManager.RunGating(targetRef);
                    }
                }
            }

            localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = true;
            localPluginContext.PluginExecutionContext.OutputParameters["Output"] = "";
        }
    }
}
