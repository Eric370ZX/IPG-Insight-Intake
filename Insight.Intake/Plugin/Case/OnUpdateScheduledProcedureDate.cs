using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.Plugin.Case
{
    public class OnUpdateScheduledProcedureDate : PluginBase
    {
        public const string AuthorizationLifecycleStepId = "20a244cb-d3c1-e911-a983-000d3a37043b";

        public OnUpdateScheduledProcedureDate() : base(typeof(OnUpdateScheduledProcedureDate))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, Incident.EntityLogicalName, PreOperationHandler);
        }

        void PreOperationHandler(LocalPluginContext localPluginContext)
        {
            var preIncident = localPluginContext.PreImage<Incident>();
            var incident = localPluginContext.Target<Incident>();

            if (incident.ipg_isportalrequest.HasValue && incident.ipg_isportalrequest.Value
                && preIncident.ipg_SurgeryDate != incident.ipg_SurgeryDate)
            {
                incident.ipg_StateCode = new OptionSetValue((int)ipg_CaseStateCodes.Authorization);
                incident.ipg_lifecyclestepid = new EntityReference(ipg_lifecyclestep.EntityLogicalName, new Guid(AuthorizationLifecycleStepId));
            }
            incident.ipg_isportalrequest = false;
        }
    }
}
