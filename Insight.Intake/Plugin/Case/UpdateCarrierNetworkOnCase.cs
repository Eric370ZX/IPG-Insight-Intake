using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Insight.Intake.Plugin.Case
{
    public class UpdateCarrierNetworkOnCase : PluginBase
    {
        public UpdateCarrierNetworkOnCase() : base(typeof(UpdateCarrierNetworkOnCase))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, Incident.EntityLogicalName, PreOperationCreateHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, Incident.EntityLogicalName, PreOperationCreateHandler);
        }

        void PreOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;
            tracingService.Trace($"{typeof(UpdateCarrierNetworkOnCase)} plugin started");

            var incident = ((Entity)context.InputParameters["Target"]).ToEntity<Incident>();

            if (incident.LogicalName != Incident.EntityLogicalName)
            {
                return;
            }

            if (incident.ipg_CarrierId != null)
            {
                EntityReference carrierRef = incident.ipg_CarrierId;

                var carrier = service.Retrieve(carrierRef.LogicalName, carrierRef.Id, new ColumnSet("ipg_carriernetworkid")).ToEntity<Intake.Account>();

                if (carrier != null)
                {
                    if (carrier.ipg_carriernetworkid != null)
                    {
                        incident.ipg_carriernetwork = carrier.ipg_carriernetworkid;
                        tracingService.Trace($"{typeof(UpdateCarrierNetworkOnCase)} Updated incident carrier network to: " + incident.ipg_carriernetwork.Name);
                    }
                }
            }

            context.InputParameters["Target"] = incident.ToEntity<Entity>();
        }

    }
}
