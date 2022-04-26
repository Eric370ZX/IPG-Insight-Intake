using Insight.Intake.Extensions;
using Insight.Intake.Helpers;
using Insight.Intake.Models;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace Insight.Intake.Plugin.Case
{
    public class TriggerEHRUpdateFeedProcess : PluginBase
    {
        public TriggerEHRUpdateFeedProcess() : base(typeof(TriggerEHRUpdateFeedProcess))
        {
            RegisterEvent(PipelineStages.PostOperation, "Update", Incident.EntityLogicalName, PostOperationHandler);
        }
        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var targetCase = localPluginContext.Target<Incident>();
            var postCase = localPluginContext.PostImage<Incident>();
            Guid lcId;
            if (!Validate(postCase, targetCase, out lcId))
            {
                return;
            }
            SetupCaseValues(localPluginContext, targetCase, postCase, lcId);
            var actionParams = new Dictionary<string, object>() { { "Target", targetCase.ToEntityReference() } };
            localPluginContext.SystemOrganizationService.ExecuteAction(Constants.ActionNames.GatingStartGateProcessing, actionParams);
        }

        private static void SetupCaseValues(LocalPluginContext localPluginContext, Incident targetCase, Incident postCase, Guid lcId)
        {
            var setToGate6 = false;
            if (lcId == LifecycleStepsConstants.CalculateRevenueGate7 || lcId == LifecycleStepsConstants.GeneratePOGate8 || lcId == LifecycleStepsConstants.SubmitClaimGate9
                || lcId == LifecycleStepsConstants.ZirmedResponseGate10 || lcId == LifecycleStepsConstants.PatientPaymentGate11)
            {
                setToGate6 = true;
            }

            var updCase = new Incident()
            {
                Id = targetCase.Id,
            };
            updCase.ipg_ehrupdatestatus = new OptionSetValue((int)Incident_ipg_ehrupdatestatus.EHRopencasererun);
            if (setToGate6)
            {
                //SVV CPI-26103 Error fix
                //updCase.ipg_lifecyclestepid = new EntityReference(ipg_lifecyclestep.EntityLogicalName, LifecycleStepsConstants.AddPartsGate6);
            }
            if (postCase.ipg_CaseStatusEnum == ipg_CaseStatus.Closed)
            {
                updCase.ipg_CaseStatusEnum = ipg_CaseStatus.Open;
                updCase.ipg_ehrupdatestatus = new OptionSetValue((int)Incident_ipg_ehrupdatestatus.EHRclosedcasererun);

            }
            if (postCase.StatusCodeEnum == Incident_StatusCode.Canceled)
            {
            updCase.ipg_ehrupdatestatus = new OptionSetValue((int)Incident_ipg_ehrupdatestatus.EHRclosedcasererun);
                updCase.StatusCodeEnum = Incident_StatusCode.InProgress;
                updCase.StateCode = IncidentState.Active;
            }
            localPluginContext.OrganizationService.Update(updCase);
        }

        private bool Validate(Incident postCase, Incident targetCase, out Guid lcId)
        {
            lcId = Guid.Empty;
            if (targetCase.ipg_EHRUpdate != true)
            {
                return false;
            }
            if (postCase.ipg_lifecyclestepid == null)
            {
                return false;
            }
            lcId = postCase.ipg_lifecyclestepid.Id;
            if (lcId == LifecycleStepsConstants.IntakeStep1Gate1 || lcId == LifecycleStepsConstants.IntakeStep2Gate2 || lcId == LifecycleStepsConstants.AuthorizationGate3
                || lcId == LifecycleStepsConstants.DOSTransitionGate4 || lcId == LifecycleStepsConstants.EHRRefferalGateEHR)
            {
                return false;
            }
            return true;
        }
    }
}