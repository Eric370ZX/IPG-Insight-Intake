using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Insight.Intake.Helpers.Constants;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckCarrierBalance : PluginBase
    {
        public CheckCarrierBalance() : base(typeof(CheckCarrierBalance))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingCheckCarrierBalance", null, PostOperationHandler);
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

            var result = gateManager.CheckCarrierBalance();

            var balancesValidation = gateManager.CheckPayerPatientBalance();
            var patientBalanceValidation = gateManager.CheckPatientBalance();
            context.OutputParameters["Succeeded"] = balancesValidation.Succeeded && (result.IsSuccess || (!result.IsSuccess && result.BalanceValidation == GateManager.BalanceValidation.Negative && patientBalanceValidation.BalanceValidation == GateManager.BalanceValidation.Positive));
            context.OutputParameters["PortalNote"] = balancesValidation.PortalNote;
            context.OutputParameters["CaseNote"] = balancesValidation.CaseNote;
            if ((bool)context.OutputParameters["Succeeded"])
            {
                context.OutputParameters["CodeOutput"] = !result.IsSuccess ? (int)CheckCarrierBalance_Output.PostivePatientBalance : (int)CheckCarrierBalance_Output.ZeroBalance;
            }

            var taskManager = new TaskManager(service, localPluginContext.TracingService, null);

            if (!result.IsSuccess || !patientBalanceValidation.IsSuccess)
            {
                var incident = service.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet(nameof(Incident.ipg_CarrierId).ToLower())).ToEntity<Incident>();
                var taskTitle = (!result.IsSuccess ? "Carrier Credit Balance with No Patient Balance" : "Open Patient Credit Balance");
                var task = taskManager.GetRelatedTask(targetRef, taskTitle);
                if(task == null)
                {
                    if (!result.IsSuccess)
                    {
                        var description = "The most recent payment or adjustment activity has created a credit balance for Carrier " + incident.ipg_CarrierId.Name + ". There is no Patient Balance or Credit Balance. Review to determine if a refund is appropriate.";
                        taskManager.CreateTask(targetRef, taskTitle, new Task() { Description = description });
                    }
                    else
                    {
                        taskManager.CreateTask(targetRef, taskTitle);
                    }
                }
            }

            if ((bool)context.OutputParameters["Succeeded"])
            {
                taskManager.CloseCategoryTasks(targetRef.Id, TaskCategoryNames.CarrierOutreach);
            }
        }

        enum CheckCarrierBalance_Output
        {
            ZeroBalance = 1,
            PostivePatientBalance = 2,
        }
    }
}
