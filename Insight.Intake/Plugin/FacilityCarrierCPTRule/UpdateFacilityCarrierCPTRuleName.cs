using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.FacilityCarrierCPTRule
{
    public class UpdateFacilityCarrierCPTRuleName : PluginBase
    {
        public UpdateFacilityCarrierCPTRuleName() : base(typeof(UpdateFacilityCarrierCPTRuleName))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, ipg_facilitycarriercptrule.EntityLogicalName, PreOperationCRUDHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, ipg_facilitycarriercptrule.EntityLogicalName, PreOperationCRUDHandler);
        }

        void PreOperationCRUDHandler(LocalPluginContext localPluginContext)
        {
            

            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;
            tracingService.Trace($"{typeof(UpdateFacilityCarrierCPTRuleName)} plugin started");

            var facilitycarriercptrule = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_facilitycarriercptrule>();


            if (facilitycarriercptrule.LogicalName != ipg_facilitycarriercptrule.EntityLogicalName)
            {
                return;
            }

            //Update rule name with CPT Code.
            if (facilitycarriercptrule.Attributes.Contains(ipg_facilitycarriercptrule.Fields.ipg_CptId))
            {
                ipg_cptcode cptcode = service.Retrieve(ipg_cptcode.EntityLogicalName,
                                                                  facilitycarriercptrule.ipg_CptId.Id,
                                                                  new ColumnSet(ipg_cptcode.Fields.ipg_cptcode1))
                                                        .ToEntity<ipg_cptcode>();
                facilitycarriercptrule.ipg_name = (cptcode.ipg_cptcode1);
            }

        }

    }
}
