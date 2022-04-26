using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating.PostProcess
{
    public class Gate4 : PluginBase
    {
        public Gate4() : base(typeof(Gate4))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingPostProcessGate4", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var targetRef = localPluginContext.TargetRef();
            try
            {
                var postProcessManager = new PostProcessManager(localPluginContext.OrganizationService, localPluginContext.TracingService, targetRef);
                var minimumSeverityLevel = localPluginContext.PluginExecutionContext.InputParameters.Contains("MinimumSeverityLevel") ? (localPluginContext.PluginExecutionContext.InputParameters["MinimumSeverityLevel"] as int?) : -1;
                if (minimumSeverityLevel == (int)ipg_SeverityLevel.Info || minimumSeverityLevel == (int)ipg_SeverityLevel.Warning)
                {
                    var sourceIncident = localPluginContext.OrganizationService
                        .Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet("ownerid", "ipg_actualdos", "ipg_surgerydate"))
                        .ToEntity<Incident>();
                    var surgeryDate = sourceIncident.GetCaseDos();
                    if (surgeryDate != null && surgeryDate < DateTime.Now)
                    {
                        var tissueRequestTasks = postProcessManager.GetTasks(ipg_TaskType1.TissueRequestForm);
                        postProcessManager.CloseOutstandingUserTasks(tissueRequestTasks.Where(p => p.StateCode == TaskState.Open), "Case DOS is met");
                    }
                }
                localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = true;
                localPluginContext.PluginExecutionContext.OutputParameters["Output"] = "Success";
            }
            catch (Exception ex)
            {
                localPluginContext.PluginExecutionContext.OutputParameters["Succeeded"] = false;
                localPluginContext.PluginExecutionContext.OutputParameters["Output"] = ex.Message;
            }
        }
    }
}
