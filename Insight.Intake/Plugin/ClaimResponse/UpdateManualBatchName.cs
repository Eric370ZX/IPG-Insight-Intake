using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Insight.Intake.Helpers;

namespace Insight.Intake.Plugin.ClaimResponse
{
    public class UpdateManualBatchName : PluginBase
    {

        public UpdateManualBatchName() : base(typeof(UpdateManualBatchName))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, ipg_claimresponsebatch.EntityLogicalName, PreOperationCreateHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, ipg_claimresponsebatch.EntityLogicalName, PreOperationCreateHandler);

        }

        private void PreOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                var claimResponseBatch = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_claimresponsebatch>();

                if (!string.IsNullOrWhiteSpace(claimResponseBatch.ipg_name))
                {
                    return;
                }

                if (claimResponseBatch.ipg_PaymentSource != null && claimResponseBatch.ipg_PaymentDate != null)
                {
                    if (claimResponseBatch.ipg_PaymentDate.Value.Date > DateTime.Now.Date)
                    {
                        throw new Exception("Bank date can't be in the future.");
                    }

                    claimResponseBatch.ipg_name = $"{D365Helpers.GetOptionSetValueLabel(ipg_claimresponsebatch.EntityLogicalName, nameof(ipg_claimresponsebatch.ipg_PaymentSource).ToLower(), claimResponseBatch.ipg_PaymentSource.Value, service)}" +
                    $"_{claimResponseBatch.ipg_PaymentDate?.ToString("yyyyMMdd")}_{new Random().Next(1000000).ToString("D6")}";
                }
                else if ((claimResponseBatch.ipg_Type ?? false) && (claimResponseBatch.ipg_PaymentDate != null))
                {
                    claimResponseBatch.ipg_name = $"Refund" +
                    $"_{claimResponseBatch.ipg_PaymentDate?.ToString("yyyyMMdd")}_{new Random().Next(1000000).ToString("D6")}";
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}