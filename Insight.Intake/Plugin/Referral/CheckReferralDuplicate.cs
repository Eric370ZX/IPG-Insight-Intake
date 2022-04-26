using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace Insight.Intake.Plugin.Referral
{
    public class CheckReferralDuplicate : PluginBase
    {
        public CheckReferralDuplicate() : base(typeof(CheckReferralDuplicate))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_referral.EntityLogicalName, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var target = localPluginContext.Target<ipg_referral>();

            if (string.IsNullOrEmpty(target.ipg_PatientFirstName) || string.IsNullOrEmpty(target.ipg_PatientLastName) ||
                target.ipg_PatientDateofBirth == null || target.ipg_SurgeryDate == null)
            {
                return;
            }
            var originQuery = new QueryExpression(ipg_referral.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression( ipg_referral.PrimaryIdAttribute, ConditionOperator.NotEqual, target.Id),
                        new ConditionExpression(ipg_referral.Fields.ipg_PatientFirstName, ConditionOperator.Equal, target.ipg_PatientFirstName),
                        new ConditionExpression(ipg_referral.Fields.ipg_PatientLastName, ConditionOperator.Equal, target.ipg_PatientLastName),
                        new ConditionExpression(ipg_referral.Fields.ipg_PatientDateofBirth, ConditionOperator.On, target.ipg_PatientDateofBirth.Value.Date),
                        new ConditionExpression(ipg_referral.Fields.ipg_SurgeryDate, ConditionOperator.On, target.ipg_SurgeryDate.Value.Date),
                        new ConditionExpression(ipg_referral.Fields.StateCode, ConditionOperator.Equal, (int)ipg_referralState.Active)
                    }
                }
            };
            var origin = localPluginContext.OrganizationService.RetrieveMultiple(originQuery).Entities.FirstOrDefault()?.ToEntity<ipg_referral>();
            if (origin == null)
            {
                return;
            }
            localPluginContext.OrganizationService.Update(new ipg_referral()
            {
                Id = target.Id,
                ipg_casestatusEnum = ipg_CaseStatus.Closed,
                ipg_ReasonsEnum = ipg_CaseReasons.DuplicateReferralORDuplicateCase
            });
            if (origin.ipg_casestatusEnum == ipg_CaseStatus.Closed)
            {
                var originUpdate = new ipg_referral()
                {
                    Id = origin.Id,
                    ipg_casestatusEnum = ipg_CaseStatus.Open
                };
                foreach (var attr in target.Attributes)
                {
                    if (attr.Value != null && attr.Key != ipg_referral.PrimaryIdAttribute)
                    {
                        originUpdate[attr.Key] = attr.Value;
                    }
                }                
                localPluginContext.OrganizationService.Update(originUpdate);
            }
        }
    }
}
