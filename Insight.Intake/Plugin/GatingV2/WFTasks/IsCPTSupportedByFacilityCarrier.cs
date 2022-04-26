using Insight.Intake.Plugin.GatingV2.CommonWfTask;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Insight.Intake.Extensions;
using Microsoft.Xrm.Sdk.Client;

namespace Insight.Intake.Plugin.GatingV2.WFTasks
{
    public class IsCPTSupportedByFacilityCarrier : WFTaskBase
    {

        public override WFTaskResult Run(WFTaskContext ctx)
        {
            var service = ctx.CrmService;
            var crmContext = new OrganizationServiceContext(service);
            var tracingService = ctx.TraceService;
            var target = ctx.dbContext.Referral != null ? ctx.dbContext.Referral.ToIncident() : ctx.dbContext.Case;

            //Pick actual date of surgery, if actual dos is null then pick scheduled dos.
            var dos = target.GetCaseDos();
            var gatingResponse = new WFTaskResult(false);
            if (target.ipg_FacilityId != null && target.ipg_CarrierId != null && dos != null)
            {
                //Look for facility carrier rule applicable on DOS.
                var rulesWithContract = (from rule in crmContext.CreateQuery<ipg_facilitycarriercptrule>()
                                         join contract in crmContext.CreateQuery<ipg_facilitycarriercptrulecontract>()
                                         on rule.ipg_FacilityCarrierCPTRuleContractId.Id equals contract.Id
                                         join entitlement in crmContext.CreateQuery<Entitlement>()
                                         on contract.ipg_EntitlementId.Id equals entitlement.Id
                                         where entitlement.ipg_CarrierId.Id == target.ipg_CarrierId.Id
                                         && entitlement.ipg_FacilityId.Id == target.ipg_FacilityId.Id
                                         && entitlement.StartDate <= dos
                                         && entitlement.EndDate >= dos
                                         && contract.ipg_EffectiveDate <= dos
                                         && contract.ipg_ExpirationDate >= dos
                                         && rule.ipg_EffectiveDate <= dos
                                         && rule.ipg_ExpirationDate >= dos
                                         select new { rule, contract, entitlement }).ToList();

                var rules = rulesWithContract?.Select(x => x.rule);
                var facilityCarrierContract = rulesWithContract?.FirstOrDefault()?.entitlement;
                var ipg_facilitycarriercptrulecontract = rulesWithContract?.FirstOrDefault()?.contract;
                var cptIds = GetCptCodeIds(target);

                if (ipg_facilitycarriercptrulecontract != null &&
                    ipg_facilitycarriercptrulecontract.ipg_CPTInclusionType == ipg_cptinclusiontyperule.Excluded)
                {
                    if (cptIds.Any())
                    {
                        foreach (Guid cptId in cptIds)
                        {
                            //Here we are looking for one good CPT code which is not in the exclusion list.
                            if (!rules.Any(x => x.ipg_CptId.Id.Equals(cptId)))
                            {
                                gatingResponse.Succeeded = true;
                                break;
                            }
                        }
                    }//For referral CPT code is not required.
                    else if (ctx.dbContext.Referral != null && target.ipg_CPTCodeId1 == null)
                    {
                        gatingResponse.Succeeded = true;
                    }

                    if (!gatingResponse.Succeeded)
                    {
                        gatingResponse = GetGatingResponseForCPTNotSupportedByFacilityCarrier(service, target, dos, facilityCarrierContract);
                    }
                }
                else if (ipg_facilitycarriercptrulecontract != null &&
                         ipg_facilitycarriercptrulecontract.ipg_CPTInclusionType == ipg_cptinclusiontyperule.Included)
                {
                    if (cptIds.Any())
                    {
                        foreach (Guid cptId in cptIds)
                        {
                            //Here we are looking for one good CPT code which is in the inclusion list.
                            if (rules.Any(x => x.ipg_CptId.Id.Equals(cptId)))
                            {
                                gatingResponse.Succeeded = true;
                                break;
                            }
                        }
                    }//For referral CPT code is not required.
                    else if (ctx.dbContext.Referral != null && target.ipg_CPTCodeId1 == null)
                    {
                        gatingResponse.Succeeded = true;
                    }

                    if (!gatingResponse.Succeeded)
                    {
                        gatingResponse = GetGatingResponseForCPTNotSupportedByFacilityCarrier(service, target, dos, facilityCarrierContract);
                    }

                }
                else if (facilityCarrierContract == null)
                {

                    var rulesForFacilityContract = (from rule in crmContext.CreateQuery<ipg_facilitycarriercptrule>()
                                                    join contract in crmContext.CreateQuery<ipg_facilitycarriercptrulecontract>()
                                                    on rule.ipg_FacilityCarrierCPTRuleContractId.Id equals contract.Id
                                                    join entitlement in crmContext.CreateQuery<Entitlement>()
                                                    on contract.ipg_EntitlementId.Id equals entitlement.Id
                                                    where entitlement.ipg_CarrierId.Id == target.ipg_CarrierId.Id
                                                    && entitlement.ipg_FacilityId.Id == target.ipg_FacilityId.Id
                                                    where entitlement.ipg_CarrierId.Id == target.ipg_CarrierId.Id
                                                    && entitlement.ipg_FacilityId.Id == target.ipg_FacilityId.Id
                                                    select new { rule, contract, entitlement }).ToList();
                    //If a rule exists for carrier facility contract irrespective of the effective date, create a case note
                    //for the user to notify them that the carrier facility rule of not configured for DOS.
                    if (rulesForFacilityContract.Any())
                    {

                        gatingResponse.Succeeded = true;
                        gatingResponse.CaseNote = $"The facility carrier contract for " +
                                                  $"{rulesForFacilityContract?.FirstOrDefault()?.entitlement?.Name} " +
                                                  $"is not set up for the DOS {dos.Value.ToShortDateString()}.";
                    }
                    else
                    {
                        gatingResponse.Succeeded = true;
                    }
                }

            }
            return gatingResponse;
        }


        private WFTaskResult GetGatingResponseForCPTNotSupportedByFacilityCarrier(IOrganizationService service, Incident target, DateTime? dos, Entitlement entitlement)
        {
            var facility = target.ipg_FacilityId_Entity;
            string facilityCIM = facility.ipg_FacilityCimId != null ? facility.ipg_FacilityCimId.Name : "CIM";
            var cptCodes = RetrieveCptCodes(service, target);
            var response = new WFTaskResult(false);
            response.CaseNote = $"None of the provided CPT code(s) {string.Join(",", cptCodes.Select(x => x.ipg_cptcode1))} " +
                                $"are supported by this {entitlement.Name} relationship for this DOS {dos.Value.ToShortDateString()}. " +
                                $"Case rejected and facility notified.";
            response.PortalNote = $"IPG has rejected this case due to none of the provided CPT " +
                                  $"{string.Join(",", cptCodes.Select(x => x.ipg_cptcode1))} being supported by " +
                                  $"{target.ipg_CarrierId.Name} for this facility, {target.ipg_FacilityId.Name} for DOS {dos.Value.ToShortDateString()}. " +
                                  $"If the physician uses a supported implant, please notify your CIM, {facilityCIM} for reconsideration of this case.";
            return response;
        }

        private List<ipg_cptcode> RetrieveCptCodes(IOrganizationService service, Incident target)
        {
            var cptCodes = new List<ipg_cptcode>();

            if (target.ipg_CPTCodeId1_Entity != null)
            {
                cptCodes.Add(target.ipg_CPTCodeId1_Entity);
            }
            if (target.ipg_CPTCodeId2_Entity != null)
            {
                cptCodes.Add(target.ipg_CPTCodeId2_Entity);
            }
            if (target.ipg_CPTCodeId3_Entity != null)
            {
                cptCodes.Add(target.ipg_CPTCodeId3_Entity);
            }
            if (target.ipg_CPTCodeId4_Entity != null)
            {
                cptCodes.Add(target.ipg_CPTCodeId4_Entity);
            }
            if (target.ipg_CPTCodeId5_Entity != null)
            {
                cptCodes.Add(target.ipg_CPTCodeId5_Entity);
            }
            if (target.ipg_CPTCodeId6_Entity != null)
            {
                cptCodes.Add(target.ipg_CPTCodeId6_Entity);
            }
            return cptCodes;
        }

        private List<Guid> GetCptCodeIds(Entity target)
        {
            var cptCodes = new List<Guid>();
            var cptAttributesNames = new List<string>() { "ipg_cptcodeid1", "ipg_cptcodeid2", "ipg_cptcodeid3", "ipg_cptcodeid4", "ipg_cptcodeid5", "ipg_cptcodeid6" };

            foreach (var attributeName in cptAttributesNames)
            {
                if (target.Contains(attributeName))
                {
                    var cptCode = target.GetAttributeValue<EntityReference>(attributeName);
                    if (cptCode != null)
                    {
                        cptCodes.Add(target.GetAttributeValue<EntityReference>(attributeName).Id);
                    }
                }
            }

            return cptCodes;
        }
    }
}
