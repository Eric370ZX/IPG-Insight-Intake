using Insight.Intake.Extensions;
using Insight.Intake.Helpers;
using Insight.Intake.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Referral
{
    public class EhrCptOverride : PluginBase
    {
        public EhrCptOverride() : base(typeof(EhrCptOverride))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGReferralActionsEhrCptOverride", null, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var tracingService = localPluginContext.TracingService;
            var target = (EntityReference)localPluginContext.PluginExecutionContext.InputParameters["Target"];
            if(target == null)
            {
                tracingService.Trace("No Target Referral. Exit");
                return;
            }

            var referral = localPluginContext.OrganizationService.Retrieve(ipg_referral.EntityLogicalName, target.Id, new ColumnSet(true)).ToEntity<ipg_referral>();
            
            if(ValidateReferral(referral) == false)
            {
                tracingService.Trace("Referral is not valid for Ehr Cpt Override");
                localPluginContext.PluginExecutionContext.OutputParameters["IsSuccess"] = false;
                return;
            }

            tracingService.Trace("Removing CPT codes");
            RemoveCptCodes(localPluginContext.OrganizationService, referral);

            tracingService.Trace("Setting default procedure name");
            SetDefaultProcedureName(localPluginContext.OrganizationService, referral);

            tracingService.Trace("Reset lifecycle step and case status");
            referral.ipg_lifecyclestepid = new EntityReference(ipg_lifecyclestep.EntityLogicalName, LifecycleStepsConstants.EHRRefferalGateEHR);
            referral.ipg_casestatusEnum = ipg_CaseStatus.Open;

            tracingService.Trace("Updating the referral");
            localPluginContext.OrganizationService.Update(referral);

            localPluginContext.PluginExecutionContext.OutputParameters["IsSuccess"] = true;
        }

        private bool ValidateReferral(ipg_referral referral)
        {
            return referral.ipg_casestatusEnum == ipg_CaseStatus.Closed
                && referral.ipg_AssociatedCaseId == null
                && referral.ipg_caseoutcomeEnum == ipg_CaseOutcomeCodes.GateEHRFail;
        }

        private void RemoveCptCodes(IOrganizationService orgService, ipg_referral referral)
        {
            var removedCpts = new List<string>();

            if(referral.ipg_CPTCodeId1 != null)
            {
                removedCpts.Add(referral.ipg_CPTCodeId1.Name);
                referral.ipg_CPTCodeId1 = null;
            }
            if (referral.ipg_CPTCodeId2 != null)
            {
                removedCpts.Add(referral.ipg_CPTCodeId2.Name);
                referral.ipg_CPTCodeId2 = null;
            }
            if (referral.ipg_CPTCodeId3 != null)
            {
                removedCpts.Add(referral.ipg_CPTCodeId3.Name);
                referral.ipg_CPTCodeId3 = null;
            }
            if (referral.ipg_CPTCodeId4 != null)
            {
                removedCpts.Add(referral.ipg_CPTCodeId4.Name);
                referral.ipg_CPTCodeId4 = null;
            }
            if (referral.ipg_CPTCodeId5 != null)
            {
                removedCpts.Add(referral.ipg_CPTCodeId5.Name);
                referral.ipg_CPTCodeId5 = null;
            }
            if (referral.ipg_CPTCodeId6 != null)
            {
                removedCpts.Add(referral.ipg_CPTCodeId6.Name);
                referral.ipg_CPTCodeId6 = null;
            }

            if(removedCpts.Any())
            {
                Guid noteId = orgService.Create(new Annotation
                {
                    ObjectId = referral.ToEntityReference(),
                    Subject = "Original CPT codes in EHR",
                    NoteText = "CPT values originally provided in the EHR file: " + string.Join(", ", removedCpts)
                });

                var noteIds = referral.GetNotesToCopyToCase();
                noteIds.Add(noteId);
                referral.SetNotesToCopyToCase(noteIds);
            }
        }

        private void SetDefaultProcedureName(IOrganizationService orgService, ipg_referral referral)
        {
            referral.ipg_EHRProcedureName = referral.ipg_ProcedureNameId?.Name;
            
            string defaultProcedureIdString = D365Helpers.GetGlobalSettingValueByKey(orgService, GlobalSettingConstants.DefaultProcedureIdSettingName);
            if(Guid.TryParse(defaultProcedureIdString, out Guid defaultProcedureId) == false)
            {
                throw new Exception($"A valid global setting '{GlobalSettingConstants.DefaultProcedureIdSettingName}' is required");
            }

            referral.ipg_ProcedureNameId = new EntityReference(ipg_procedurename.EntityLogicalName, defaultProcedureId);
        }
    }
}