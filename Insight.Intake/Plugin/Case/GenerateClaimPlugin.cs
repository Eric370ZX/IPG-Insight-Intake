using Microsoft.Xrm.Sdk;
using System;
using Insight.Intake.Plugin.Managers;

namespace Insight.Intake.Plugin.Case
{
    public class GenerateClaimPlugin : PluginBase
    {
        public GenerateClaimPlugin() : base(typeof(GenerateClaimPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGCaseActionsCreateClaim", Incident.EntityLogicalName, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;


            try
            {
                var claimRequest = new ipg_IPGCaseActionsCreateClaimRequest()
                {
                    Target = (EntityReference)context.InputParameters["Target"],
                    IsPrimaryOrSecondaryClaim = (bool)context.InputParameters["IsPrimaryOrSecondaryClaim"],
                    GenerateClaimFlag = (bool)context.InputParameters["GenerateClaimFlag"],
                    GeneratePdfFlag = (bool)context.InputParameters["GeneratePdfFlag"],
                    IsReplacementClaim = (bool)context.InputParameters["IsReplacementClaim"],
                    Icn = (string)context.InputParameters["Icn"],
                    Box32 = (string)context.InputParameters["Box32"],
                    Reason = (string)context.InputParameters["Reason"],
                    ManualClaim = context.InputParameters.Contains("ManualClaim") ? (bool)context.InputParameters["ManualClaim"] : false
                };

                var claimGenerationManager = new ClaimGenerationManager(service, tracingService, claimRequest, localPluginContext.PluginExecutionContext.InitiatingUserId);

                claimGenerationManager.ProcessGeneration(context.OutputParameters);
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}