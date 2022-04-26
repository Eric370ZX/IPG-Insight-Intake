using Insight.Intake.Extensions;
using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.Plugin.Referral
{
    public class UpdateReferralType : PluginBase
    {
        public UpdateReferralType() : base(typeof(UpdateReferralType))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, ipg_referral.EntityLogicalName, PreOperationCreateHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_referral.EntityLogicalName, PostOperationHandler); //ipg_actualdos, ipg_SurgeryDate
        }

        void PreOperationCreateHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var tracingService = localPluginContext.TracingService;
            tracingService.Trace($"{typeof(UpdateReferralType)} plugin started");

            var referral = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_referral>();

            var dos = referral.ipg_actualdos ?? referral.ipg_SurgeryDate;
            if (dos.HasValue == false)
            {
                tracingService.Trace("No DOS. Exit.");
                return;
            }

            UpdateReferralTypeField(tracingService, referral, dos.Value, DateTime.Now);

            tracingService.Trace("Setting Target Referral");
            context.InputParameters["Target"] = referral.ToEntity<Entity>();
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            var tracingService = localPluginContext.TracingService;
            tracingService.Trace($"{typeof(UpdateReferralType)} plugin started");

            var referral = ((Entity)context.InputParameters["Target"]).ToEntity<ipg_referral>();
            var referralPreImage = context.PreEntityImages["PreImage"].ToEntity<ipg_referral>();

            DateTime? dos = (referral.ipg_actualdos ?? referralPreImage.ipg_actualdos) 
                ?? (referral.ipg_SurgeryDate ?? referralPreImage.ipg_SurgeryDate);
            if (dos.HasValue == false)
            {
                tracingService.Trace("No DOS. Exit.");
                return;
            }

            DateTime? createdOn = referral.CreatedOn ?? referralPreImage?.CreatedOn;
            if(createdOn.HasValue == false)
            {
                tracingService.Trace("No CreatedOn. Exit.");
                return;
            }

            UpdateReferralTypeField(tracingService, referral, dos.Value, createdOn.Value);

            if(referralPreImage.ipg_referraltypeEnum != referral.ipg_referraltypeEnum)
            {
                tracingService.Trace("Updating Referral");
                service.Update(referral);
            }
        }

        private void UpdateReferralTypeField(ITracingService tracingService, ipg_referral referral, DateTime dos, DateTime createdOn)
        {
            tracingService .Trace("Setting Referral Type");
            referral.ipg_referraltypeEnum = ReferralHelper.CalculateReferralType(dos, createdOn);
            tracingService.Trace("New Referral Type = " + referral.ipg_referraltypeEnum);
        }
    }
}
