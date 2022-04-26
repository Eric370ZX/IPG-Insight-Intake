using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Managers;
using Microsoft.Xrm.Sdk;

namespace Insight.Intake.Plugin.Referral
{
    public class ReopenReferralPlugin : PluginBase
    {
        public ReopenReferralPlugin() : base(typeof(ReopenReferralPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGReferralReopenReferral", null, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var pluginExecutionContext = localPluginContext.PluginExecutionContext;
            var referralEntity = (Entity)localPluginContext.PluginExecutionContext.InputParameters["Referral"];
            if (referralEntity != null)
            {
                var referral = referralEntity.ToEntity<ipg_referral>();
                referral.StateCode = ipg_referralState.Active;
                referral.ipg_casestatus = new OptionSetValue((int)ipg_CaseStatus.Open);
                referral.ipg_casestatusdisplayedid = new EntityReference(ipg_casestatusdisplayed.EntityLogicalName, Constants.CaseStatusDisplayedGuids.CaseStatusDisplayedId);

                var referralInfo = localPluginContext.OrganizationService.Retrieve(ipg_referral.EntityLogicalName, referralEntity.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(
                    ipg_referral.Fields.ipg_Origin)).ToEntity<ipg_referral>();
                if(referralInfo.ipg_OriginEnum == Incident_CaseOriginCode.EHR)
                {
                    referral.ipg_lifecyclestepid = new EntityReference(ipg_lifecyclestep.EntityLogicalName, Constants.LifecycleStepGuids.EhrLifecycleStepId);
                    referral.ipg_gateconfigurationid = new EntityReference(ipg_gateconfiguration.EntityLogicalName, Constants.GateConfigurationGuids.EhrGateId);
                }
                else
                {
                    referral.ipg_lifecyclestepid = new EntityReference(ipg_lifecyclestep.EntityLogicalName, Constants.LifecycleStepGuids.IntakeStep2LifecycleStepId);
                    referral.ipg_gateconfigurationid = new EntityReference(ipg_gateconfiguration.EntityLogicalName, Constants.GateConfigurationGuids.Gate2Config);
                }
                
                localPluginContext.OrganizationService.Update(referral);

                ImportantEventManager eventManager = new ImportantEventManager(localPluginContext.OrganizationService);
                eventManager.CreateImportantEventLog(referral, pluginExecutionContext.UserId, Constants.EventIds.ET26);
                eventManager.SetCaseOrReferralPortalHeader(referral, Constants.EventIds.ET26);
            }
        }
    }
}