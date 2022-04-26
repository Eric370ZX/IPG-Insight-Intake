using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;


namespace Insight.Intake.Plugin.FacilityCPT
{
    public class OnFacilityCPTCreateAndUpdate : PluginBase
    {
        //This plugin is to run duplicate validation and other stuff when creating/updating facility cpt exclusion rules.
        public OnFacilityCPTCreateAndUpdate() : base(typeof(OnFacilityCPTCreateAndUpdate))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_facilitycpt.EntityLogicalName, PostOperationCreateAndUpdateHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_facilitycpt.EntityLogicalName, PostOperationCreateAndUpdateHandler);
        }

        void PostOperationCreateAndUpdateHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;
            tracingService.Trace($"{typeof(OnFacilityCPTCreateAndUpdate)} plugin started");

            //If the incoming(Target) entity is not ipg_facilitycpt, return.
            if (((Entity)context.InputParameters["Target"]).LogicalName != ipg_facilitycpt.EntityLogicalName) return;

            var facilityCPTRef = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_facilitycpt>();

            //Retrieve the saved record.
            var facilityCPT = service.Retrieve(ipg_facilitycpt.EntityLogicalName,
                                     facilityCPTRef.Id, new ColumnSet(true)).ToEntity<ipg_facilitycpt>();

            var validator = new FaciltyCPTValidator(service);
            //Validate is there is another CPT which overlaps with this record. 
            //Overlapping criteria includes effective and expiration dates.
            validator.ValidateForOverlappingFacilityCPT(facilityCPT);
        }
    }
}
