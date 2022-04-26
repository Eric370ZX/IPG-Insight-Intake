using Microsoft.Xrm.Sdk.Query;
using Insight.Intake.Helpers;
using static Insight.Intake.Helpers.Constants;

namespace Insight.Intake.Plugin.Adjustment
{
    public class AdjustmentGateExecution : PluginBase
    {
        public AdjustmentGateExecution() : base(typeof(AdjustmentGateExecution))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_adjustment.EntityLogicalName, PostOperationCreateHandler);
        }

        private void PostOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;
            var adjustment = localPluginContext.Target<ipg_adjustment>();

            if (adjustment.ipg_CaseId != null && (adjustment.ipg_AmountToApply?.Value ?? 0) != 0 && adjustment.ipg_SkipGatingExecution != true)
            {
                var LFCollectionId = service.GetGlobalSettingValueByKey(Settings.Collection_LF_Step_Id)?.ToLower();

                if (LFCollectionId == null)
                {
                    localPluginContext.TracingService.Trace("There is no configuration");
                    return;
                }

                var incident = service.Retrieve(adjustment.ipg_CaseId.LogicalName, adjustment.ipg_CaseId.Id, new ColumnSet(Incident.Fields.ipg_lifecyclestepid)).ToEntity<Incident>();
                if (incident.ipg_lifecyclestepid.Id.ToString().Equals(LFCollectionId, System.StringComparison.OrdinalIgnoreCase))
                {
                    service.Execute(new ipg_IPGGatingStartGateProcessingRequest() { Target = incident.ToEntityReference() });
                }

                //Executes Gate on Case from what balance have been transfered
                if (adjustment.ipg_AdjustmentTypeEnum == ipg_AdjustmentTypes.TransferofPayment)
                {
                    incident = service.Retrieve(adjustment.ipg_FromCase.LogicalName, adjustment.ipg_FromCase.Id, new ColumnSet(Incident.Fields.ipg_lifecyclestepid)).ToEntity<Incident>();
                    if (incident.ipg_lifecyclestepid.Id.ToString().Equals(LFCollectionId))
                    {
                        service.Execute(new ipg_IPGGatingStartGateProcessingRequest() { Target = incident.ToEntityReference() });
                    }
                }
            }
        }
    }
}