using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Payment
{
    public class CreateAdjustment : PluginBase
    {
        public CreateAdjustment() : base(typeof(GateExecution))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_payment.EntityLogicalName, PostOperationCreateHandler);
        }

        private void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            try
            {
                var context = localPluginContext.PluginExecutionContext;
                var service = localPluginContext.OrganizationService;
                var payment = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_payment>();

                if (payment.ipg_ApplyAdjustment != true)
                {
                    return;
                }

                ipg_adjustment adjustment = new ipg_adjustment();
                string[] array = { nameof(ipg_adjustment.ipg_CaseId).ToLower(),
                                nameof(ipg_adjustment.ipg_AdjustmentType).ToLower(),
                                nameof(ipg_adjustment.ipg_ApplyTo).ToLower(),
                                nameof(ipg_adjustment.ipg_From).ToLower(),
                                nameof(ipg_adjustment.ipg_To).ToLower(),
                                nameof(ipg_adjustment.ipg_Reason).ToLower(),
                                nameof(ipg_adjustment.ipg_Note).ToLower(),
                                nameof(ipg_adjustment.ipg_AmountType).ToLower(),
                                nameof(ipg_adjustment.ipg_Amount).ToLower(),
                                nameof(ipg_adjustment.ipg_AmountToApply).ToLower()};
                foreach (string field in array)
                {
                    if (payment.Attributes.Contains(field))
                    {
                        adjustment[field] = payment[field];
                    }
                }
                string percentField = nameof(ipg_adjustment.ipg_Percent).ToLower();
                if (payment.Attributes.Contains(percentField))
                {
                    adjustment[percentField] = (decimal)((int)payment[percentField]);
                }
                adjustment.ipg_SkipGatingExecution = true;
                service.Create(adjustment);
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}
