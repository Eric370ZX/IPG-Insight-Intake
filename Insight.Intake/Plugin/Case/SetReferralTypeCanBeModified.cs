using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Case
{
    public class SetReferralTypeCanBeModified : PluginBase
    {
        public SetReferralTypeCanBeModified() : base(typeof(SetReferralTypeCanBeModified))
        {
            RegisterEvent(PipelineStages.PostOperation, "Update", Incident.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;
            tracingService.Trace($"{typeof(SetReferralTypeCanBeModified)} plugin started");


            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Incident incident = ((Entity)context.InputParameters["Target"]).ToEntity<Incident>();

                if (incident.LogicalName != Incident.EntityLogicalName)
                {
                    return;
                }

                if (!CheckIfDOSHasBeenModified(incident, service))
                {

                    if (incident.ipg_SurgeryDate.HasValue)
                    {
                        SetDOSHasBeenModified(incident, service, tracingService);

                        incident.ipg_referraltypecanbeaccessed = true;
                        service.Update(incident);
                    }
                }

                else
                {
                    if (incident.ipg_ReferralType != null)
                    {

                        if (!CheckIfReferralTypeHasBeenModified(incident, service))
                        {
                            incident.ipg_referraltypehasbeenmodified = true;
                            incident.ipg_referraltypecanbeaccessed = false;
                            service.Update(incident);
                        }
                    }
                }
            }
        }


        private void SetDOSHasBeenModified(Incident incident, IOrganizationService service, ITracingService tracingService)
        {

            if (!CheckIfDOSHasBeenModified(incident, service))
            {
                Incident GateConfigurationIncident = service.Retrieve(Incident.EntityLogicalName, incident.Id, new ColumnSet("ipg_gateconfigurationid")).ToEntity<Incident>();

                if (GateConfigurationIncident.ipg_gateconfigurationid != null)
                {

                    if (GateConfigurationIncident.ipg_gateconfigurationid.Name == Constants.GateNames.Gate1 ||
                        GateConfigurationIncident.ipg_gateconfigurationid.Name == Constants.GateNames.Gate2 ||
                        GateConfigurationIncident.ipg_gateconfigurationid.Name == Constants.GateNames.Gate3)
                    {
                        incident.ipg_dosHasBeenModified = true;
                        service.Update(incident);
                    }
                }
            }
        }

        private bool CheckIfDOSHasBeenModified(Incident incident, IOrganizationService service)
        {
            Incident DOSHasBeenModifiedIncident = service.Retrieve(Incident.EntityLogicalName, incident.Id, new ColumnSet("ipg_doshasbeenmodified")).ToEntity<Incident>();

            bool DOSHasBeenModified = DOSHasBeenModifiedIncident.ipg_dosHasBeenModified ?? false;

            return DOSHasBeenModified;
        }

        private bool CheckIfReferralTypeHasBeenModified(Incident incident, IOrganizationService service)
        {
            Incident ReferralTypeHasBeenModifiedIncident = service.Retrieve(Incident.EntityLogicalName, incident.Id, new ColumnSet("ipg_referraltypehasbeenmodified")).ToEntity<Incident>(); ;

            bool ReferralTypeHasBeenModified = ReferralTypeHasBeenModifiedIncident.ipg_referraltypehasbeenmodified ?? false;

            return ReferralTypeHasBeenModified;
        }

    }
}