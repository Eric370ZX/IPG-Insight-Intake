using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.TaskEntity
{
    public class PopulateCustomCaseField : PluginBase
    {
        public PopulateCustomCaseField() : base(typeof(PopulateCustomCaseField))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, Task.EntityLogicalName, PreOperationHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, Task.EntityLogicalName, PreOperationHandler);
        }

        private void PreOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;

            var target = ((Entity)context.InputParameters["Target"]).ToEntity<Task>();

            Task preImage = null;
            if (context.PreEntityImages != null && context.PreEntityImages.Contains("Image")) {
                preImage = ((Entity)context.PreEntityImages["Image"])?.ToEntity<Task>();
            }

            var regardingId = target.Contains(nameof(Task.RegardingObjectId).ToLower()) ? target.RegardingObjectId : preImage?.RegardingObjectId;


            if (regardingId?.LogicalName == Incident.EntityLogicalName) {
                target.ipg_caseid = regardingId;
                target.ipg_facilityid = service.Retrieve(target.ipg_caseid.LogicalName, target.ipg_caseid.Id
                    , new ColumnSet(nameof(Incident.ipg_FacilityId).ToLower())).ToEntity<Incident>().ipg_FacilityId;
            }
            else if (preImage?.RegardingObjectId?.LogicalName == Incident.EntityLogicalName
                        && regardingId?.LogicalName != Incident.EntityLogicalName)
            {
                target.ipg_caseid = target.ipg_facilityid = null;
            }
        }
    }
}
