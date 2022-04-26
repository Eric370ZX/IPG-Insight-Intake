using Insight.Intake.Plugin.Common.Benefits;
using Microsoft.Xrm.Sdk;

namespace Insight.Intake.Plugin.Case
{
    public class VerifyBenefits : PluginBase
    {
        private IOrganizationService service;
        public VerifyBenefits() : base(typeof(VerifyBenefits))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGCaseActionsVerifyBenefits", Incident.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            localPluginContext.Trace($"Starting execution");

            var context = localPluginContext.PluginExecutionContext;
            service = localPluginContext.OrganizationService;

            EntityReference targetRef = context.InputParameters["Target"] as EntityReference;

            if (targetRef != null) 
            {
                localPluginContext.Trace($"targetRef is not null");

                bool isUserGenerated = (bool)context.InputParameters["IsUserGenerated"];
                localPluginContext.Trace($"IsUserGenerated: " + isUserGenerated);

                var carrierNumber = (CarrierNumbers)context.InputParameters["CarrierNumber"];
                localPluginContext.Trace($"CarrierNumber: " + carrierNumber);

                var benefitVerifier = new BenefitVerifier(context, localPluginContext.OrganizationService, localPluginContext.TracingService);
                bool isBenefitVerified = benefitVerifier.Verify(targetRef, isUserGenerated, carrierNumber);

                if(!isBenefitVerified)
                {
                    benefitVerifier.CreateTaskManualBenefitVerificationRequired(targetRef, carrierNumber);
                }
            }

            localPluginContext.Trace($"Finished execution");
        }
    }
}