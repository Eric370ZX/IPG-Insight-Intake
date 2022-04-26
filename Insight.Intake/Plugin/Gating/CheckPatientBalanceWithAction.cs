using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckPatientBalanceWithAction : PluginBase
    {
        public CheckPatientBalanceWithAction() : base(typeof(CheckPatientBalanceWithAction))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingCheckPatientBalanceWithAction", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var targetRef = (EntityReference)localPluginContext.PluginExecutionContext.InputParameters["Target"];
            if (targetRef == null)
            {
                throw new InvalidPluginExecutionException("Target case is null");
            }
            var gateManager = new GateManager(localPluginContext.OrganizationService, localPluginContext.TracingService, targetRef);
           
            var result = gateManager.CheckPatientBalance();
            context.OutputParameters["Succeeded"] = result.IsSuccess;

            //if validation was succesfull, close outstanding patient tasks
            if (result.IsSuccess)
            {
                gateManager.CloseOutstandingTasks();
            }
            //if validation wasn't succesfull, close outstanding patient tasks
            else if (!result.IsSuccess && result.BalanceValidation == GateManager.BalanceValidation.Negative)
            {
                gateManager.CloseOutstandingTasks();
                if (!gateManager.HasUserTask_CreditBalance())
                    gateManager.CreateUserTask_CreditBalance();
            }
        }
    }
}
