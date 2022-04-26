using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.SLA
{
    public class DecisionCase : PluginBase
    {
        public DecisionCase() : base(typeof(DecisionCase))
        {
            RegisterEvent(
                    PipelineStages.PostOperation,
                    MessageNames.Create,
                    ipg_referral.EntityLogicalName,
                    OnCreate);

            RegisterEvent(
                    PipelineStages.PostOperation,
                    MessageNames.Update,
                    ipg_referral.EntityLogicalName,
                    OnUpdateReferral);
        }

        private void OnCreate(LocalPluginContext pluginContext)
        {
            pluginContext.TracingService.Trace("Start OnCreate");

            ipg_referral target = pluginContext.Target<ipg_referral>();

            CreateSlaTask(pluginContext, target);
        }

        private void OnUpdateReferral(LocalPluginContext pluginContext)
        {
            pluginContext.TracingService.Trace("Start OnUpdateReferral");

            ipg_referral preImage = pluginContext.PreImage<ipg_referral>();
            ipg_referral postImage = pluginContext.PostImage<ipg_referral>();

            //TODO Not sure what means Referral Closed. Its when is Inactive or just case status changed from gating to closed

            if (preImage != null && postImage != null
                && preImage.StateCode == ipg_referralState.Inactive 
                && preImage.ipg_OutcomeCodeEnum == ipg_OutcomeCodes.Rejected //TODO rejected status may be moved to a different field later when statuses are reworked
                && postImage.StateCode == ipg_referralState.Active
                || 
                preImage.ipg_casestatusEnum == ipg_CaseStatus.Closed
                && preImage.ipg_OutcomeCodeEnum == ipg_OutcomeCodes.Rejected
                && postImage.ipg_casestatusEnum == ipg_CaseStatus.Open
                //Referral Type set on Update
                || preImage.ipg_referraltypeEnum != postImage.ipg_referraltypeEnum)
            {
                DeleteSLATask(pluginContext, postImage.ToEntityReference());
                CreateSlaTask(pluginContext, postImage);
            }
        }

        private void CreateSlaTask(LocalPluginContext pluginContext, ipg_referral referral)
        {
            pluginContext.TracingService.Trace("Start CreateSlaTask");

            if (referral.Contains(ipg_referral.Fields.ipg_referraltype) && referral.ipg_referraltype != null)
            {
                TaskManager taskManager = new TaskManager(
                pluginContext.SystemOrganizationService,
                pluginContext.TracingService,
                null,
                pluginContext.PluginExecutionContext.InitiatingUserId);

                pluginContext.TracingService.Trace("Getting task subject");

                var taskType = taskManager.GetSlaTaskTypeRefByReferralType(referral.ipg_referraltypeEnum);

                taskManager.CreateTask(referral.ToEntityReference(), taskType, new Task() { ActualStart = DateTime.Now });
            }
        }

        private void DeleteSLATask(LocalPluginContext pluginContext, EntityReference refRef)
        {
            pluginContext.TracingService.Trace("Start CancelSlaTasks");
            using (CrmServiceContext context = new CrmServiceContext(pluginContext.SystemOrganizationService))
            {
                pluginContext.TracingService.Trace("Requesting existing tasks");

                var slaTasks = TaskManager.GetOpenDecisionSLATasks(refRef, pluginContext.OrganizationService, pluginContext.TracingService);

                pluginContext.TracingService.Trace($"Deletion {slaTasks.Count()} SLA tasks");
                
                foreach (var slaTask in slaTasks)
                {
                    context.Attach(slaTask);
                    context.DeleteObject(slaTask);
                }

                context.SaveChanges();
            }
        }
    }
}
