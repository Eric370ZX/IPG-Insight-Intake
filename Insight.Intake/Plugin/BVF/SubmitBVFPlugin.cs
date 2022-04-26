using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using Insight.Intake.Plugin.Common.Interfaces;
using Insight.Intake.Plugin.Common.Benefits;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.BVF
{
    public class SubmitBVFPlugin : PluginBase
    {
        const string REASON = "BVF Completed successfully";

        private ICaseBenefitSwitcher _caseBenefitSwitcher;

        private List<KeyValuePair<string, string>> _BVFToIncidentMapping;

        public SubmitBVFPlugin() : base(typeof(SubmitBVFPlugin))
        {
            _BVFToIncidentMapping = GetBVFToIncidentMapping();

            RegisterEvents();
        }

        /// <summary>
        /// For unit tests
        /// </summary>
        /// <param name="caseBenefitSwitcher"></param>
        public SubmitBVFPlugin(ICaseBenefitSwitcher caseBenefitSwitcher) : base(typeof(SubmitBVFPlugin))
        {
            _caseBenefitSwitcher = caseBenefitSwitcher;
            _BVFToIncidentMapping = GetBVFToIncidentMapping();

            RegisterEvents();
        }

        private void RegisterEvents()
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_benefitsverificationform.EntityLogicalName, PostOperationHandler);
        }

        private void InitializeServices(LocalPluginContext context)
        {
            if (_caseBenefitSwitcher == null)
            {
                _caseBenefitSwitcher = new CaseBenefitSwitcher(context.OrganizationService, context.TracingService);
            }
        }

        private void PostOperationHandler(LocalPluginContext context)
        {
            InitializeServices(context);

            var target = context.Target<ipg_benefitsverificationform>();
            var tracingService = context.TracingService;

            tracingService.Trace($"SubmitBVFPlugin stareted");

            tracingService.Trace($"Check if BVF:{target.Id} has Parent Case");
            if (target.ipg_parentcaseid != null)
            {
                var crmService = context.OrganizationService;

                tracingService.Trace($"Recreate Benefits from BVF");
                RecreateBenefits(crmService, target);

                tracingService.Trace($"Copy Benefit information from BVF to Case");
                CopyDataFromBVFToCase(crmService, target);

                tracingService.Trace($"Update Case InOutNetwork");
                _caseBenefitSwitcher.UpdateInOutNetwork(target.ipg_parentcaseid.Id, target.ipg_CarrierId.Id);

                tracingService.Trace($"Close Tasks with Reason: {REASON}");
                ProcessTask(crmService, target.ipg_parentcaseid, context.PluginExecutionContext.InitiatingUserId, ipg_TaskType1.ManualBVF);
                ProcessTask(crmService, target.ipg_parentcaseid, context.PluginExecutionContext.InitiatingUserId, ipg_TaskType1.ManualEBVcheckneeded);
                ProcessTask(crmService, target.ipg_parentcaseid, context.PluginExecutionContext.InitiatingUserId, ipg_TaskType1.ManualBenefitsVerificationRequired, "Manual Benefits Verification Required");
            }

            tracingService.Trace($"SubmitBVFPlugin finished");
        }

        private void CopyDataFromBVFToCase(IOrganizationService crmService, ipg_benefitsverificationform bvf)
        {
            var caseUpdate = new Incident();
            var caseFields = typeof(Incident.Fields).GetFields().Select(x => x.GetValue(x.Name).ToString()).ToList();

            foreach (var field in _BVFToIncidentMapping)
            {
                if (caseFields.Contains(field.Value) && bvf.Contains(field.Key)
                   && bvf[field.Key] != null && bvf[field.Key].ToString() != string.Empty && !string.IsNullOrWhiteSpace(bvf[field.Key].ToString()))
                    caseUpdate[field.Value] = bvf[field.Key];
            }

            if (bvf.ipg_parentcaseid != null)
                caseUpdate.Id = bvf.ipg_parentcaseid.Id;
            if (bvf.GetAttributeValue<OptionSetValue>("ipg_benefittypecode") != null)
                caseUpdate.ipg_BenefitTypeCode = bvf.GetAttributeValue<OptionSetValue>("ipg_benefittypecode");
            if (bvf.ipg_inn_or_oon_codeEnum.HasValue)
                caseUpdate.ipg_inoutnetwork = bvf.ipg_inn_or_oon_codeEnum.Value == ipg_inn_or_oon.INN;
            if (bvf.GetAttributeValue<OptionSetValue>("ipg_autobenefitsexhausted") != null)
                caseUpdate.ipg_medicalbenefitsexhausted = bvf.GetAttributeValue<OptionSetValue>("ipg_autobenefitsexhausted").Value == 1;
            if (bvf.GetAttributeValue<OptionSetValue>("ipg_facilityautoexhaustletteronfile") != null)
                caseUpdate.ipg_facilityexhaustletteronfile = bvf.GetAttributeValue<OptionSetValue>("ipg_facilityautoexhaustletteronfile").Value == 1;
            if (bvf.ipg_primarycarrierplantypeEnum != ipg_CarrierPlanTypes1.Auto && bvf.ipg_primarycarrierplantypeEnum != ipg_CarrierPlanTypes1.WorkersComp)
            {
                caseUpdate.ipg_CoverageLevelDeductible = bvf.ipg_deductibletypecode;
                caseUpdate.ipg_CoverageLevelOOP = bvf.ipg_oopmaxtypecode;
            }

            crmService.Update(caseUpdate);
        }

        private void ProcessTask(IOrganizationService crmService, EntityReference caseRef, Guid? userId, ipg_TaskType1? taskType, string title = null)
        {
            Action<EntityReference> CompleteTask = (taskRef) =>
            crmService.Update(new Task()
            {
                Id = taskRef.Id,
                StatusCodeEnum = Task_StatusCode.Resolved,
                StateCode = TaskState.Completed,
                ipg_closurenote = REASON
            });

            var crmContext = new OrganizationServiceContext(crmService);

            var taskTemp = new Task()
            {
                OwnerId = new EntityReference(SystemUser.EntityLogicalName, userId.Value),
                ipg_tasktypecodeEnum = taskType,
                RegardingObjectId = caseRef
            };
            if (!string.IsNullOrEmpty(title))
                taskTemp.Subject = title;

            var BVF_Task = (from task in crmContext.CreateQuery<Task>()
                            where task.RegardingObjectId.Equals(caseRef)
                                && task.ipg_tasktypecodeEnum == taskType
                            select new Task() { Id = task.Id }).FirstOrDefault();

            if (BVF_Task != null)
            {
                CompleteTask(BVF_Task.ToEntityReference());
            }
            else
            {
                taskTemp.Id = crmService.Create(taskTemp);

                CompleteTask(taskTemp.ToEntityReference());
            }
        }

        private List<KeyValuePair<string, string>> GetBVFToIncidentMapping()
        {
            return new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string> ("ipg_policyholdername", "ipg_policyid"),
                new KeyValuePair<string, string> ("ipg_relationtoinsuredcode", "ipg_relationtoinsured"),
                new KeyValuePair<string, string> ("ipg_homeplancarrierid", "ipg_homeplancarrierid"),
                new KeyValuePair<string, string> ("ipg_coverageeffectivedate", "ipg_primarycarriereffectivedate"),
                new KeyValuePair<string, string> ("ipg_coverageexpirationdate", "ipg_primarycarrierexpirationdate"),
                new KeyValuePair<string, string> ("ipg_preauthrequired", "ipg_ispreauthrequired"),
                new KeyValuePair<string, string> ("ipg_facilityauth", "ipg_facilityauthnumber"),
                new KeyValuePair<string, string> ("ipg_ipgauth", "ipg_ipgauthnumber"),
                new KeyValuePair<string, string> ("ipg_csrname", "ipg_authcsrname"),
                new KeyValuePair<string, string> ("ipg_csrphone", "ipg_authcsrphone"),
                new KeyValuePair<string, string> ("ipg_callreference", "ipg_authcallreference"),
                //new KeyValuePair<string, string> ("ipg_autocarrierid", "ipg_autocarrierid"),
                new KeyValuePair<string, string> ("ipg_claim", "ipg_autoclaimnumber"),
                new KeyValuePair<string, string> ("ipg_adjustername", "ipg_autoadjustername"),
                new KeyValuePair<string, string> ("ipg_plantypecode", "ipg_benefitplantypecode"),
                new KeyValuePair<string, string> ("ipg_plandescription", "ipg_plandescription"),
                new KeyValuePair<string, string> ("ipg_plansponsor", "ipg_plansponsor"),
                new KeyValuePair<string, string> ("ipg_claimsmailingaddress", "ipg_carrierclaimmailingaddress"),
                new KeyValuePair<string, string> ("ipg_carriercoinsurance", "ipg_payercoinsurance"),
                new KeyValuePair<string, string> ("modifiedby", "ipg_benefitsupdatedby"),
                new KeyValuePair<string, string> ("modifiedon", "ipg_benefitsupdatedon"),
                new KeyValuePair<string, string> ("ipg_memberidnumber", "ipg_memberidnumber"),
                new KeyValuePair<string, string> ("ipg_procedurename", "ipg_procedurename"),
                new KeyValuePair<string, string> ("ipg_cptcode1", "ipg_cptcode1"),
                new KeyValuePair<string, string> ("ipg_cptcode2", "ipg_cptcode2"),
                new KeyValuePair<string, string> ("ipg_cptcode3", "ipg_cptcode3"),
                new KeyValuePair<string, string> ("ipg_cptcode4", "ipg_cptcode4"),
                new KeyValuePair<string, string> ("ipg_cptcode5", "ipg_cptcode5"),
                new KeyValuePair<string, string> ("ipg_cptcode6", "ipg_cptcode6"),
                //new KeyValuePair<string, string> ("ipg_carrierid", "ipg_carrierid"),
                new KeyValuePair<string, string> ("ipg_primarycarriergroupidnumber", "ipg_primarycarriergroupidnumber"),
                new KeyValuePair<string, string> ("ipg_primarycarrierplantype", "ipg_primarycarrierplantype"),
                new KeyValuePair<string, string> ("ipg_autheffectivedate", "ipg_autheffectivedate"),
                new KeyValuePair<string, string> ("ipg_authexpirationdate", "ipg_authexpirationdate"),
                new KeyValuePair<string, string> ("ipg_authorizationnotes", "ipg_authorizationnotes"),
                new KeyValuePair<string, string> ("ipg_adjusterphone", "ipg_adjusterphone"),
                new KeyValuePair<string, string> ("ipg_billingfax", "ipg_billingfax"),
                new KeyValuePair<string, string> ("ipg_jurisdictionstateid", "ipg_jurisdictionstate"),
                new KeyValuePair<string, string> ("ipg_deductible", "ipg_deductible"),
                new KeyValuePair<string, string> ("ipg_deductiblemet", "ipg_deductiblemet"),
                new KeyValuePair<string, string> ("ipg_oopmax", "ipg_oopmax"),
                new KeyValuePair<string, string> ("ipg_oopmaxmet", "ipg_oopmet"),
                new KeyValuePair<string, string> ("ipg_csrname", "ipg_csrname"),
                new KeyValuePair<string, string> ("ipg_csrphone", "ipg_csrphone"),
                new KeyValuePair<string, string> ("ipg_callreference", "ipg_callreference"),
                new KeyValuePair<string, string> ("ipg_benefitnotesmultiplelines", "ipg_benefitsnotes"),
                new KeyValuePair<string, string> ("ipg_patientcoinsurance", "ipg_patientcoinsurance"),
                new KeyValuePair<string, string> ("ipg_nursecasemgrname", "ipg_nursecasemgrname"),
                new KeyValuePair<string, string> ("ipg_nursecasemgrphone", "ipg_nursecasemgrphone")
            };
        }

        private void RecreateBenefits(IOrganizationService organizationService, ipg_benefitsverificationform bvf)
        {
            ipg_BenefitType benefitType;
            if (bvf.ipg_formtypeEnum.HasValue)
            {
                switch (bvf.ipg_formtypeEnum.Value)
                {
                    case ipg_benefitsverificationform_ipg_formtype.Auto:
                        benefitType = ipg_BenefitType.Auto;
                        break;
                    case ipg_benefitsverificationform_ipg_formtype.GeneralHealth:
                        if (bvf.ipg_BenefitTypeCodeEnum.HasValue == false)
                        {
                            throw new Exception($"BVF {nameof(bvf.ipg_BenefitTypeCode)} is required");
                        }

                        benefitType = bvf.ipg_BenefitTypeCodeEnum.Value;
                        break;
                    case ipg_benefitsverificationform_ipg_formtype.WC:
                        benefitType = ipg_BenefitType.WorkersComp;
                        break;
                    case ipg_benefitsverificationform_ipg_formtype.DME:
                        benefitType = ipg_BenefitType.DurableMedicalEquipment_DME;
                        break;
                    default:
                        throw new Exception("Unexpected form type: " + bvf.ipg_formtypeEnum);
                }
            }
            else
            {
                throw new Exception($"BVF {nameof(bvf.ipg_formtype)} is required");
            }

            bool inOutNetwork;
            if(benefitType == ipg_BenefitType.Auto
                || benefitType == ipg_BenefitType.WorkersComp)
            {
                inOutNetwork = true;
            }
            else
            {
                if (bvf.ipg_inn_or_oon_codeEnum.HasValue)
                {
                    switch (bvf.ipg_inn_or_oon_codeEnum.Value)
                    {
                        case ipg_inn_or_oon.INN:
                            inOutNetwork = true;
                            break;
                        case ipg_inn_or_oon.OON:
                            inOutNetwork = false;
                            break;
                        default:
                            throw new Exception("Unexpected network value: " + bvf.ipg_inn_or_oon_codeEnum);
                    }
                }
                else
                {
                    throw new Exception("Network value is required");
                }
            }

            DeleteBenefits(organizationService, bvf.ipg_parentcaseid, bvf.ipg_CarrierId, bvf.ipg_MemberIdNumber, benefitType, inOutNetwork);
            
            if(bvf.ipg_deductibletypecodeEnum == ipg_BenefitCoverageLevels.Individual
                || bvf.ipg_oopmaxtypecodeEnum == ipg_BenefitCoverageLevels.Individual)
            {
                CreateBenefit(organizationService, bvf.ipg_parentcaseid, bvf.ipg_CarrierId, bvf.ipg_MemberIdNumber, benefitType, inOutNetwork, ipg_BenefitCoverageLevels.Individual,
                    bvf);
            }
            if(bvf.ipg_deductibletypecodeEnum == ipg_BenefitCoverageLevels.Family
                || bvf.ipg_oopmaxtypecodeEnum == ipg_BenefitCoverageLevels.Family)
            {
                CreateBenefit(organizationService, bvf.ipg_parentcaseid, bvf.ipg_CarrierId, bvf.ipg_MemberIdNumber, benefitType, inOutNetwork, ipg_BenefitCoverageLevels.Family,
                    bvf);
            }
        }

        private void DeleteBenefits(IOrganizationService organizationService, EntityReference incidentReference, EntityReference carrierReference, string memberId, ipg_BenefitType benefitType, bool inOutNetwork)
        {
            var existingBenefits = organizationService.RetrieveMultiple(new QueryExpression(ipg_benefit.EntityLogicalName)
            {
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_benefit.Fields.ipg_CaseId, ConditionOperator.Equal, incidentReference.Id),
                        new ConditionExpression(ipg_benefit.Fields.ipg_CarrierId, ConditionOperator.Equal, carrierReference.Id),
                        new ConditionExpression(ipg_benefit.Fields.ipg_MemberID, ConditionOperator.Equal, memberId),
                        new ConditionExpression(ipg_benefit.Fields.ipg_BenefitType, ConditionOperator.Equal, (int)benefitType),
                        new ConditionExpression(ipg_benefit.Fields.ipg_InOutNetwork, ConditionOperator.Equal, inOutNetwork),
                        new ConditionExpression(ipg_benefit.Fields.StateCode, ConditionOperator.Equal, (int)ipg_benefitState.Active)
                    }
                }
            });
            foreach (var existingBenefit in existingBenefits.Entities.ToList())
            {
                organizationService.Delete(ipg_benefit.EntityLogicalName, existingBenefit.Id);
            }
        }

        private void CreateBenefit(IOrganizationService organizationService, EntityReference incidentReference, EntityReference carrierReference, string memberId, 
            ipg_BenefitType benefitType, bool inOutNetwork, ipg_BenefitCoverageLevels coverageLevel, ipg_benefitsverificationform bvf)
        {
            var newBenefit = new ipg_benefit()
            {
                ipg_CaseId = incidentReference,
                ipg_CarrierId = carrierReference,
                ipg_MemberID = memberId,
                ipg_BenefitTypeEnum = benefitType,
                ipg_InOutNetwork = inOutNetwork,
                ipg_CoverageLevelEnum = coverageLevel,

                ipg_GroupID = bvf.ipg_primarycarriergroupidnumber,
                ipg_insurancetype = bvf.ipg_primarycarrierplantype,
                ipg_PlanSponsor = bvf.ipg_plansponsor,
                ipg_EligibilityStartDate = bvf.ipg_coverageeffectivedate,
                ipg_EligibilityEndDate = bvf.ipg_coverageexpirationdate,
                ipg_CoinsuranceBenefitSourceEnum = ipg_BenefitSources.BVF,
                ipg_CarrierCoinsurance = (double?)bvf.ipg_carriercoinsurance,
                ipg_MemberCoinsurance = (double?)bvf.ipg_patientcoinsurance,
                ipg_BenefitSourceEnum = ipg_BenefitSources.BVF,
                ipg_ObtainedBy = bvf.CreatedBy
            };

            if(bvf.ipg_deductibletypecodeEnum == coverageLevel)
            {
                newBenefit.ipg_Deductible = bvf.ipg_deductible;
                newBenefit.ipg_DeductibleMet = bvf.ipg_deductiblemet;
            }
            if(bvf.ipg_oopmaxtypecodeEnum == coverageLevel)
            {
                newBenefit.ipg_MemberOOPMax = bvf.ipg_oopmax;
                newBenefit.ipg_MemberOOPMet = bvf.ipg_oopmaxmet;
            }

            organizationService.Create(newBenefit);
        }
    }
}