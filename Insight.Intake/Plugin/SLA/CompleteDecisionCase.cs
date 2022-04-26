using Insight.Intake.Extensions;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Insight.Intake.Plugin.SLA
{
    public class CompleteDecisionCase : PluginBase
    {
        public CompleteDecisionCase() : base(typeof(CompleteDecisionCase))
        {
            RegisterEvent(
                PipelineStages.PostOperation,
                MessageNames.Update,
                ipg_referral.EntityLogicalName,
                OnUpdateReferral);

            RegisterEvent(
                PipelineStages.PostOperation,
                MessageNames.Update,
                Incident.EntityLogicalName,
                OncaseUpdate);
        }

        private void OnUpdateReferral(LocalPluginContext pluginContext)
        {
            var service = pluginContext.OrganizationService;
            var tracing = pluginContext.TracingService;
            
            var referral = pluginContext.Target<ipg_referral>();

            if (referral.Contains(ipg_referral.Fields.StateCode) 
                && referral.StateCode != null
                && (int)referral.StateCode == (int)ipg_referralState.Inactive
                || referral.Contains(ipg_referral.Fields.ipg_casestatus)
                && referral.ipg_casestatusEnum == ipg_CaseStatus.Closed
                && referral.Contains(ipg_referral.Fields.ipg_OutcomeCode)
                && referral.ipg_OutcomeCodeEnum == ipg_OutcomeCodes.Rejected)
            {
                var tasks = TaskManager.GetOpenDecisionSLATasks(referral.ToEntityReference(), service, tracing);

                foreach (var task in tasks)
                {
                    CloseTask(service, task);
                }
            }
        }

        private void OncaseUpdate(LocalPluginContext pluginContext)
        {
            var service = pluginContext.OrganizationService;
            var tracing = pluginContext.TracingService;

            var incident = pluginContext.Target<Incident>();
            if ((incident.Contains(Incident.Fields.ipg_CaseStatus) 
                && incident.ipg_CaseStatus != null 
                && incident.ipg_CaseStatus.Value == (int)ipg_CaseStatus.Closed)
                || (incident.Contains(Incident.Fields.ipg_StateCode) 
                && incident.ipg_StateCode != null
                && incident.ipg_StateCode.Value == (int)ipg_CaseStateCodes.CaseManagement))
            {
                CloseTasks(incident.ToEntityReference(), service, tracing);

                var referralRef = service.Retrieve(incident.LogicalName, incident.Id, new ColumnSet(Incident.Fields.ipg_ReferralId))?.ToEntity<Incident>().ipg_ReferralId;

                if(referralRef != null)
                {
                    CloseTasks(referralRef, service, tracing);
                }
            }
        }

        private void CloseTask(IOrganizationService service, Entity task)
        {
            if (task != null)
            {
                service.Update(new Task()
                {
                    Id = task.Id,
                    Subcategory = DateTime.Now <= task.ToEntity<Task>().ScheduledEnd ? "SLA Met" : "SLA Not Met",
                    StateCode = TaskState.Completed,
                    StatusCode = Task_StatusCode.Resolved.ToOptionSetValue()
                });
            }
        }

        private void CloseTasks(EntityReference regardingRef, IOrganizationService service, ITracingService tracing)
        {
            var tasks = TaskManager.GetOpenDecisionSLATasks(regardingRef, service, tracing);

            foreach (var task in tasks)
            {
                CloseTask(service, task);
            }
        }
    }
}
