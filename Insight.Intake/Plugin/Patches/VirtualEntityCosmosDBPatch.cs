using Microsoft.Xrm.Sdk;

namespace Insight.Intake.Plugin.Patches
{
    public class VirtualEntityCosmosDBPatch : PluginBase
    {
        public VirtualEntityCosmosDBPatch() : base(typeof(VirtualEntityCosmosDBPatch))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.RetrieveMultiple, "ipg_ebvinquiry", PostOperationRetrieveMultipleHandlerInquiry); //todo: use the entity name from the context
            RegisterEvent(PipelineStages.PostOperation, MessageNames.RetrieveMultiple, "ipg_ebvresponse", PostOperationRetrieveMultipleHandlerResponse); //todo: use the entity name from the context
            RegisterEvent(PipelineStages.PostOperation, MessageNames.RetrieveMultiple, "ipg_ebvbenefit", PostOperationRetrieveMultipleHandlerBenefit); //todo: use the entity name from the context
            RegisterEvent(PipelineStages.PostOperation, MessageNames.RetrieveMultiple, "ipg_ebventity", PostOperationRetrieveMultipleHandlerEntity); //todo: use the entity name from the context
            RegisterEvent(PipelineStages.PostOperation, MessageNames.RetrieveMultiple, "ipg_ebvsubscriber", PostOperationRetrieveMultipleHandlerSubscriber); //todo: use the entity name from the context
            RegisterEvent(PipelineStages.PostOperation, MessageNames.RetrieveMultiple, "ipg_ehrstaging", PostOperationRetrieveMultipleHandlerEHRStaging); //todo: use the entity name from the context
        }

        private void PostOperationRetrieveMultipleHandlerInquiry(LocalPluginContext localPluginContext)
        {
            PostOperationRetrieveMultipleHandler(localPluginContext, "ipg_ebvinquiry"); //todo: use the entity name from the context
        }
        private void PostOperationRetrieveMultipleHandlerResponse(LocalPluginContext localPluginContext)
        {
            PostOperationRetrieveMultipleHandler(localPluginContext, "ipg_ebvresponse"); //todo: use the entity name from the context
        }
        private void PostOperationRetrieveMultipleHandlerBenefit(LocalPluginContext localPluginContext)
        {
            PostOperationRetrieveMultipleHandler(localPluginContext, "ipg_ebvbenefit"); //todo: use the entity name from the context
        }
        private void PostOperationRetrieveMultipleHandlerEntity(LocalPluginContext localPluginContext)
        {
            PostOperationRetrieveMultipleHandler(localPluginContext, "ipg_ebventity"); //todo: use the entity name from the context
        }
        private void PostOperationRetrieveMultipleHandlerSubscriber(LocalPluginContext localPluginContext)
        {
            PostOperationRetrieveMultipleHandler(localPluginContext, "ipg_ebvsubscriber"); //todo: use the entity name from the context
        }
        private void PostOperationRetrieveMultipleHandlerEHRStaging(LocalPluginContext localPluginContext)
        {
            PostOperationRetrieveMultipleHandler(localPluginContext, "ipg_ehrstaging"); //todo: use the entity name from the context
        }

        private void PostOperationRetrieveMultipleHandler(LocalPluginContext localPluginContext, string entityLogicalName)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;

            if (context.OutputParameters.Contains("BusinessEntityCollection") && context.OutputParameters["BusinessEntityCollection"] is EntityCollection)
            {
                var collection = (EntityCollection)context.OutputParameters["BusinessEntityCollection"];
                collection.EntityName = entityLogicalName;
            }
        }
    }
}
