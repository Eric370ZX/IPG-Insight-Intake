using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Insight.Intake.Helpers.Constants;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckPatientBalance : PluginBase
    {
        public CheckPatientBalance() : base(typeof(CheckPatientBalance))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingCheckPatientBalance", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var targetRef = (EntityReference)localPluginContext.PluginExecutionContext.InputParameters["Target"];
            if (targetRef == null)
            {
                throw new InvalidPluginExecutionException("Target case is null");
            }
            var gateManager = new GateManager(localPluginContext.OrganizationService, localPluginContext.TracingService, targetRef);
           
            var result = gateManager.CheckPatientBalance();
            context.OutputParameters["Succeeded"] = result.IsSuccess;

            if (result.IsSuccess)
            {
                var taskManager = new TaskManager(service, localPluginContext.TracingService, null);
                taskManager.CloseCategoryTasks(targetRef.Id, TaskCategoryNames.PatientOutreach);
            }

        }
    }
}
