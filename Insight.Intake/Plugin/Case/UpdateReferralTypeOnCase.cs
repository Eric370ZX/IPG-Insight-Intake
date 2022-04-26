using Insight.Intake.Extensions;
using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.Plugin.Case
{
    public class UpdateReferralTypeOnCase : PluginBase
    {
        public UpdateReferralTypeOnCase() : base(typeof(UpdateReferralTypeOnCase))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, Incident.EntityLogicalName, PreOperationCreateHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, Incident.EntityLogicalName, PostOperationHandler);
        }

        void PreOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            //test
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;
            tracingService.Trace($"{typeof(UpdateReferralTypeOnCase)} plugin started");

            var incident = ((Entity)context.InputParameters["Target"]).ToEntity<Incident>();

            var nullableDos = incident.GetCaseDos();
            if (incident.LogicalName != Incident.EntityLogicalName || nullableDos == null)
            {
                return;
            }

            var dos = (DateTime)nullableDos;

            UpdateReferralType(ref incident, tracingService, dos);

            context.InputParameters["Target"] = incident.ToEntity<Entity>();
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;
            tracingService.Trace($"{typeof(UpdateReferralTypeOnCase)} plugin started");

            var incident = ((Entity)context.InputParameters["Target"]).ToEntity<Incident>();

            var nullableDos = incident.GetCaseDos();
            if (incident.LogicalName != Incident.EntityLogicalName || nullableDos == null)
            {
                return;
            }

            var dos = (DateTime)nullableDos;

            UpdateReferralType(ref incident, tracingService, dos);

            service.Update(incident);
        }


        private void UpdateReferralType(ref Incident incident, ITracingService tracingService, DateTime dos)
        {           
            tracingService.Trace($"{typeof(UpdateReferralTypeOnCase)}: Date of surgery {dos.ToLongDateString()}");

            incident.ipg_ReferralTypeEnum = ReferralHelper.CalculateReferralType(dos, DateTime.Now);

            var nextBusinessDay = DateTime.Now;
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                nextBusinessDay = nextBusinessDay.AddDays(2);
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                nextBusinessDay = nextBusinessDay.AddDays(1);
            }
            if (dos < nextBusinessDay)
            {
                incident.ipg_ActualDOS = dos;
                incident.ipg_SurgeryDate = dos;
            }
        }
    }
}
