using Insight.Intake.Helpers;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace Insight.Intake.Plugin.Referral
{
    public class CreateAnnotationFromReferralNote : PluginBase
    {
        public CreateAnnotationFromReferralNote() : base(typeof(CreateAnnotationFromReferralNote))
        {
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Create, ipg_referral.EntityLogicalName, PreOperationHandler);
            RegisterEvent(PipelineStages.PreOperation, MessageNames.Update, ipg_referral.EntityLogicalName, PreOperationHandler);
        }
        private void PreOperationHandler(LocalPluginContext localPluginContext)
        {
            var referral = localPluginContext.Target<ipg_referral>();

            if(!string.IsNullOrEmpty(referral.ipg_referralnote))
            {
                var note = new Annotation()
                {
                    Subject = Constants.AnnotationSubjects.ReferralNote,
                    NoteText = referral.ipg_referralnote,
                    ObjectId = referral.ToEntityReference()
                };
                localPluginContext.OrganizationService.Create(note);
            }
        }
    }
}
