using Insight.Intake.Data;
using Insight.Intake.Extensions;
using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Gating
{
    public class CptCodesValidation : GatingPluginBase
    {
        private static readonly string ActionName = "ipg_IPGGatingCptCodesValidation";

        public CptCodesValidation() : base(ActionName) { }
        public CptCodesValidation(LocalPluginContext localPluginContext) : base(localPluginContext, ActionName) { }
        readonly string[] _cptCodes =
            {
            Incident.Fields.ipg_CPTCodeId1,
            Incident.Fields.ipg_CPTCodeId2,
            Incident.Fields.ipg_CPTCodeId3,
            Incident.Fields.ipg_CPTCodeId4,
            Incident.Fields.ipg_CPTCodeId5,
            Incident.Fields.ipg_CPTCodeId6
            };
        readonly string[] _caseRequiredColumns = {
            Incident.Fields.ipg_BilledCPTId,
            Incident.Fields.ipg_ActualDOS,
            Incident.Fields.ipg_SurgeryDate,
            Incident.Fields.ipg_CarrierId,
            Incident.Fields.ipg_FacilityId
        };
        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx)
        {
            //return new GatingResponse(true, positiveResult, positiveResult);
            var columnSet = new ColumnSet(_cptCodes);
            columnSet.AddColumns(_caseRequiredColumns);
            Entity target = crmService.Retrieve(targetRef.LogicalName, targetRef.Id, columnSet);

            var resultSet = new List<CptCodesValidationResult>();
            /// Case only
            /*if (target.LogicalName == Incident.EntityLogicalName)
            {
                var incident = target.ToEntity<Incident>();
                //resultSet.Add(CheckCptCodesIfNoTRFDoc(incident));
                //resultSet.Add(CheckHasBilledCPT(incident));
            }*/
            /// Referral only
            if (target.LogicalName == ipg_referral.EntityLogicalName)
            {
                var referral = target.ToEntity<ipg_referral>();
                resultSet.Add(CPTNotProvided(referral));
            }
            ///Case & Referral
            resultSet.Add(CheckCptCodeSupportedByGender(target));
            resultSet.Add(ValidateCPTCodesByExpirationDate(target));
            resultSet.Add(CPTSupportedByCarrier(target, gateManager));
            resultSet.Add(CPTSupportedByFacility(target, gateManager));
            resultSet.Add(CPTSupportedByFacilityCarrier(target));
            resultSet.Add(CPTSupportedByIPG(target, gateManager));

            var validCptCodes = resultSet
                .Select(p => p.ValidCptIds)
                .Aggregate((prev, next) => next != null ? prev.Intersect(next).ToList() : prev);

            DetermineBillableCPT(targetRef, validCptCodes, ctx);

            return resultSet.All(x => x.Response.Succeeded)
                 ? new GatingResponse(true, codeOutput: (validCptCodes.Any() ? 1 : 2))
                 : new GatingResponse(false, "No billable CPT Code provided: " + String.Join(", ", resultSet.Where(x => !x.Response.Succeeded).Select(x => x.Response.CaseNote)));
        }

        private void DetermineBillableCPT(EntityReference targetRef, List<Guid> validCptCodes, LocalPluginContext ctx)
        {
            if (!validCptCodes.Any())
            {
                return;
            }

            var query = new QueryExpression(ipg_chargecenter.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(ipg_chargecenter.Fields.ipg_CPTNameId, ipg_chargecenter.Fields.ipg_CPTRank),
                Criteria = new FilterExpression()
                {
                    Conditions = {
                        new ConditionExpression(ipg_chargecenter.Fields.ipg_CPTNameId, ConditionOperator.In, validCptCodes.ToArray()),
                        new ConditionExpression(ipg_chargecenter.Fields.StateCode, ConditionOperator.Equal, (int)ipg_chargecenterState.Active),
                        new ConditionExpression(ipg_chargecenter.Fields.ipg_Supported, ConditionOperator.Equal, true)
                    }
                }
            };
            var chargeCenters = ctx.SystemOrganizationService
                .RetrieveMultiple(query)
                .Entities
                .Select(p => p.ToEntity<ipg_chargecenter>())
                .OrderBy(p => p.ipg_CPTRank);
            var topChargeCenter = chargeCenters.FirstOrDefault();
            if (topChargeCenter == null)
            {
                ctx.TracingService.Trace($"No charge centers are found for CPT codes: {string.Join(",", validCptCodes.Select(p => p.ToString()))}");
                return;
            }

            ctx.OrganizationService.Update(new Entity(targetRef.LogicalName, targetRef.Id)
            {
                [ipg_referral.Fields.ipg_billedcptid] = topChargeCenter.ipg_CPTNameId
            });
        }

        #region Case only
        private CptCodesValidationResult CheckCptCodesIfNoTRFDoc(Incident incident)
        {
            var errorNumbers = D365Helpers.GetGlobalSettingValueByKey(crmService, Insight.Intake.Helpers.Constants.Settings.TissueRequestForm_CPTCodes).Split(',');

            var codeReferences = GetNotEmptyCptReferences(incident);
            var hasInvalidCode = codeReferences.Any(codeRef => errorNumbers.Any(v => codeRef.Name != null && codeRef.Name.StartsWith(v)));
            if (codeReferences.Any() && hasInvalidCode)
            {
                return new CptCodesValidationResult
                {
                    Response = new GatingResponse(false),
                };
            }
            else
            {
                return new CptCodesValidationResult
                {
                    Response = new GatingResponse(true),
                    ValidCptIds = codeReferences.Select(code => code.Id).ToList()
                };
            }
        }

        private CptCodesValidationResult CheckHasBilledCPT(Incident incident) => new CptCodesValidationResult
        {
            Response = new GatingResponse(incident.ipg_BilledCPTId != null),
            ValidCptIds = (incident.ipg_BilledCPTId != null)
            ? GetNotEmptyCptCodes(incident)
            : null,
        };

        private CptCodesValidationResult ValidateCPTCodesByExpirationDate(Entity target)
        {
            var validCptCodes = RetrieveCtpCodesByExpDate(target);
            if (validCptCodes.Any())
            {
                var cptReferenses = GetNotEmptyCptReferences(target);
                var caseNote = string.Join(",", cptReferenses.Select(x => $"CPT {x.Name} is expired for DOS {target.GetCaseDos()}"));
                return new CptCodesValidationResult
                {
                    Response = new GatingResponse(false, caseNote),
                };
            }
            else
            {
                return new CptCodesValidationResult
                {
                    Response = new GatingResponse(true),
                    ValidCptIds = validCptCodes.Select(p => p.Id).ToList(),
                };
            }
        }
        #endregion

        #region Referral only

        private CptCodesValidationResult CheckForCarrierAcceptCPT(ipg_referral referral)
        {
            var dos = referral.GetCaseDos();
            if (referral.ipg_CarrierId == null || dos == null)
            {
                return new CptCodesValidationResult
                {
                    Response = new GatingResponse(false),
                };
            }

            var cptCodesIds = GetNotEmptyCptCodes(referral);

            if (cptCodesIds.Any())
            {
                var query = new QueryExpression(ipg_associatedcpt.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet(ipg_associatedcpt.Fields.ipg_CPTCodeId),
                    Criteria =
                    {
                        Conditions =
                        {
                            new ConditionExpression(ipg_associatedcpt.Fields.ipg_CarrierId, ConditionOperator.Equal, referral.ipg_CarrierId.Id),
                            new ConditionExpression(ipg_associatedcpt.Fields.ipg_EffectiveDate, ConditionOperator.LessEqual, dos),
                            new ConditionExpression(ipg_associatedcpt.Fields.ipg_ExpirationDate, ConditionOperator.GreaterEqual, dos),
                            new ConditionExpression(ipg_associatedcpt.Fields.ipg_CPTCodeId, ConditionOperator.In, cptCodesIds),
                        }
                    }
                };
                var associatedCpt = crmService.RetrieveMultiple(query).Entities;
                if (associatedCpt.Any())
                {
                    return new CptCodesValidationResult
                    {
                        Response = new GatingResponse
                        {
                            Succeeded = false,
                            CaseNote = "Carrier Does not Accept Carrier CPT Code(s)",
                        }
                    };
                }
            }
            return new CptCodesValidationResult
            {
                Response = new GatingResponse(true),
                ValidCptIds = cptCodesIds,
            };
        }

        private CptCodesValidationResult CPTNotProvided(ipg_referral referral)
        {
            var validCptCodes = GetNotEmptyCptCodes(referral);
            if (validCptCodes.Any())
            {
                return new CptCodesValidationResult
                {
                    Response = new GatingResponse(true),
                    ValidCptIds = validCptCodes,
                };
            }
            else
            {
                return new CptCodesValidationResult
                {
                    Response = new GatingResponse
                    {
                        Succeeded = false,
                        CaseNote = "No CPT provided",
                    }
                };
            }
        }

        #endregion

        #region Case & Refferal
        private CptCodesValidationResult CheckCptCodeSupportedByGender(Entity target)
        {
            var patientGenderFieldName = target.LogicalName == ipg_referral.EntityLogicalName ? ipg_referral.Fields.ipg_gender : Incident.Fields.ipg_PatientGender;

            var initTarget = crmService.Retrieve(target.LogicalName, target.Id,
            new ColumnSet("ipg_billedcptid", patientGenderFieldName));

            var billedCptId = initTarget.GetAttributeValue<EntityReference>(Incident.Fields.ipg_BilledCPTId);
            var patientGenderCode = initTarget.GetAttributeValue<OptionSetValue>(patientGenderFieldName);

            if (billedCptId == null)
            {
                return new CptCodesValidationResult
                {
                    Response = new GatingResponse(true, "Billed CPT is not populated"),
                    ValidCptIds = GetNotEmptyCptCodes(target),
                };
            }
            if (patientGenderCode == null)
            {
                return new CptCodesValidationResult
                {
                    Response = new GatingResponse(true, "Patient Gender is not populated"),
                    ValidCptIds = GetNotEmptyCptCodes(target),
                };
            }
            var caseGender = patientGenderCode.Value;
            var primaryCptCode = crmService.Retrieve(billedCptId.LogicalName, billedCptId.Id, new ColumnSet("ipg_gender"))
                .ToEntity<ipg_cptcode>();
            if (primaryCptCode.ipg_GenderEnum == ipg_cptcode_ipg_Gender.Both || caseGender == (int)ipg_Gender.Other)
            {
                return new CptCodesValidationResult
                {
                    Response = new GatingResponse(true),
                    ValidCptIds = GetNotEmptyCptCodes(target),
                };
            }

            if ((primaryCptCode.ipg_GenderEnum == ipg_cptcode_ipg_Gender.Male && caseGender == (int)ipg_Gender.Male) ||
                (primaryCptCode.ipg_GenderEnum == ipg_cptcode_ipg_Gender.Female && caseGender == (int)ipg_Gender.Female))
            {
                return new CptCodesValidationResult
                {
                    Response = new GatingResponse(true),
                    ValidCptIds = GetNotEmptyCptCodes(target),
                };
            }

            return new CptCodesValidationResult
            {
                Response = new GatingResponse(false, "CPT code is not supported for patient's gender"),
            };
        }

        private CptCodesValidationResult CheckCPTWithProcedure(Entity target)
        {
            var procedureId = (target.LogicalName == Incident.EntityLogicalName)
                ? target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_procedureid)
                : target.GetAttributeValue<EntityReference>(ipg_referral.Fields.ipg_ProcedureNameId);

            if (target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_CPTCode1) != null)
            {
                return new CptCodesValidationResult
                {
                    Response = new GatingResponse(true),
                    ValidCptIds = GetNotEmptyCptCodes(target)
                };
            }
            var note = "";
            if (procedureId != null)
            {
                var facilityCPTProcedureCode = RetrieveFacilityCPTProcedureCode(target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_FacilityId), procedureId);
                if (facilityCPTProcedureCode != null)
                {
                    if (facilityCPTProcedureCode.ipg_CptCodeId != null)
                    {
                        var targetManager = new TargetManaget(crmService);
                        targetManager.SetTargetCptCode1(target.ToEntityReference(), facilityCPTProcedureCode.ipg_CptCodeId);
                        return new CptCodesValidationResult
                        {
                            Response = new GatingResponse(true),
                            ValidCptIds = GetNotEmptyCptCodes(target),
                        };
                    }
                    else
                    {
                        note = "No Implant per Procedure";
                    }
                }
                else
                {
                    note = "CPT Not Provided";
                }
            }
            else
            {
                note = "Procedure name and CPT code are not provided";
            }
            return new CptCodesValidationResult
            {
                Response = new GatingResponse(false, note, note),
            };

        }

        private CptCodesValidationResult CPTSupportedByCarrier(Entity target, GateManager gateManager)
        {
            var cptRefList = GetNotEmptyCptReferences(target);
            if (!cptRefList.Any())
            {
                return new CptCodesValidationResult
                {
                    Response = new GatingResponse(true),
                    ValidCptIds = null,
                };
            }
            else
            {
                var dos = target.GetAttributeValue<DateTime>(Incident.Fields.ipg_SurgeryDate);
                var carrierRef = target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_CarrierId);
                var notSupportedCpts = cptRefList
                    .Where(r => CptNotExist1stCase(carrierRef, r, dos)).ToList();
                if (notSupportedCpts.Any())
                {
                    return new CptCodesValidationResult
                    {
                        Response = new GatingResponse(true),
                        ValidCptIds = notSupportedCpts.Select(code => code.Id).ToList(),
                    };
                }
                else
                {
                    var cim = gateManager.GetFacilityCIM(target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_FacilityId).Id);
                    return new CptCodesValidationResult
                    {
                        Response = new GatingResponse
                        {
                            Succeeded = false,
                            CaseNote = string.Join(",", cptRefList.Select(x => $"CPT {x.Name} is not supported by {carrierRef.Name}.Case rejected and communication sent to the facility.")),
                            PortalNote = $"IPG has rejected this case due to the patient’s procedure {string.Join(",", notSupportedCpts.Select(x => x.Name))} not being supported by the patient's health plan {carrierRef.Name}. If the physician uses a covered implant, please notify your CIM, {cim?.FullName}, for reconsideration of this case."
                        }
                    };
                }
            }
        }

        private CptCodesValidationResult CPTSupportedByFacility(Entity target, GateManager gateManager)
        {
            List<Guid> cptIdList = GetNotEmptyCptCodes(target);
            if (cptIdList.Any())
            {
                var sugeryDate = target.LogicalName == Incident.EntityLogicalName
                    ? target.GetCaseDos()
                    : target.GetAttributeValue<DateTime>(Incident.Fields.ipg_SurgeryDate);
                QueryExpression query = new QueryExpression
                {
                    EntityName = ipg_facilitycpt.EntityLogicalName,
                    ColumnSet = new ColumnSet(ipg_facilitycpt.Fields.ipg_FacilityId, ipg_facilitycpt.Fields.ipg_CptCodeId),
                    Criteria = {
                        Conditions = {
                            new ConditionExpression(ipg_facilitycpt.Fields.ipg_FacilityId, ConditionOperator.Equal, target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_FacilityId)?.Id),
                            new ConditionExpression(ipg_facilitycpt.Fields.ipg_CptCodeId, ConditionOperator.In, cptIdList.ConvertAll<String>(x => x.ToString()).ToArray<String>()),
                            new ConditionExpression(ipg_facilitycpt.Fields.ipg_EffectiveDate, ConditionOperator.LessEqual,  sugeryDate),
                            new ConditionExpression(ipg_facilitycpt.Fields.ipg_ExpirationDate, ConditionOperator.GreaterEqual,  sugeryDate),
                        }
                    }
                };
                var queryResponse = crmService.RetrieveMultiple(query);
                if (queryResponse.Entities.Any())
                {
                    var facility = target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_FacilityId);
                    var cim = gateManager.GetFacilityCIM(facility.Id);
                    return new CptCodesValidationResult
                    {
                        Response = new GatingResponse(false, "No CPTs Supported by Facility.", $"IPG has rejected this case due to the patient’s procedurenot being supported by the facility, {facility.Name}. If the physician uses a covered implant, please notify your CIM, {cim?.FullName}, for reconsideration of this case.")

                    };
                }
            }

            return new CptCodesValidationResult
            {
                Response = new GatingResponse(true, "CPT supported by the Facility."),
                ValidCptIds = cptIdList
            };
        }

        internal CptCodesValidationResult CPTSupportedByFacilityCarrier(Entity target)
        {
            var validationResult = new CptCodesValidationResult
            {
                Response = new GatingResponse(false, "There is no facility-carrier CPT rule")
            };

            //Pick actual date of surgery, if actual dos is null then pick scheduled dos.
            var dos = target.GetCaseDos();
            var facilityFieldName = target.LogicalName == ipg_referral.EntityLogicalName ? ipg_referral.Fields.ipg_FacilityId : Incident.Fields.ipg_FacilityId;
            var carrierFieldName = target.LogicalName == ipg_referral.EntityLogicalName ? ipg_referral.Fields.ipg_CarrierId : Incident.Fields.ipg_CarrierId;

            if (target.GetAttributeValue<EntityReference>(facilityFieldName) != null && target.GetAttributeValue<EntityReference>(carrierFieldName) != null && dos != null)
            {
                var cptIds = GetNotEmptyCptCodes(target);
                //Look for facility carrier rule applicable on DOS.
                var crmContext = new OrganizationServiceContext(crmService);
                var facilityCarrierContracts = (from facilityCarrierCptRuleContract in crmContext.CreateQuery<ipg_facilitycarriercptrulecontract>()
                                                join facilityCarriercontract in crmContext.CreateQuery<Entitlement>()
                                                on facilityCarrierCptRuleContract.ipg_EntitlementId.Id equals facilityCarriercontract.Id
                                                where facilityCarriercontract.ipg_CarrierId.Id == target.GetAttributeValue<EntityReference>(carrierFieldName).Id
                                                && facilityCarriercontract.ipg_FacilityId.Id == target.GetAttributeValue<EntityReference>(facilityFieldName).Id
                                                && facilityCarriercontract.StartDate <= dos
                                                && facilityCarriercontract.EndDate >= dos
                                                && facilityCarrierCptRuleContract.ipg_EffectiveDate <= dos
                                                && facilityCarrierCptRuleContract.ipg_ExpirationDate >= dos
                                                select new { facilityCarriercontract, facilityCarrierCptRuleContract }).ToList();

                foreach (var iContract in facilityCarrierContracts)
                {
                    var contractRules = (from rule in crmContext.CreateQuery<ipg_facilitycarriercptrule>()
                                         where rule.ipg_FacilityCarrierCPTRuleContractId.Id == iContract.facilityCarrierCptRuleContract.Id
                                         && rule.ipg_EffectiveDate <= dos && rule.ipg_ExpirationDate >= dos
                                         select rule).ToList();

                    if (iContract.facilityCarrierCptRuleContract.ipg_CPTInclusionType == ipg_cptinclusiontyperule.Excluded)
                    {
                        /// Here we are looking for one good CPT code which is not in the exclusion list.
                        /// or 
                        /// For referral CPT code is not required.
                        validationResult.ValidCptIds = cptIds.Where(cptId => !contractRules.Any(rule => rule.ipg_CptId.Id.Equals(cptId))).ToList();

                        if (validationResult.ValidCptIds.Any() || (target.LogicalName == ipg_referral.EntityLogicalName && target.GetAttributeValue<EntityReference>(ipg_referral.Fields.ipg_CPTCodeId1) == null))
                        {
                            validationResult.Response.Succeeded = true;
                        }
                        else
                        {
                            validationResult.Response = GetGatingResponseForCPTNotSupportedByFacilityCarrier(target, dos, iContract.facilityCarriercontract);
                        }
                    }
                    else if (iContract.facilityCarrierCptRuleContract.ipg_CPTInclusionType == ipg_cptinclusiontyperule.Included)
                    {
                        /// Here we are looking for one good CPT code which is in the inclusion list.
                        /// or
                        /// For referral CPT code is not required.
                        validationResult.ValidCptIds = cptIds.Where(cptId => contractRules.Any(x => x.ipg_CptId.Id.Equals(cptId))).ToList();
                        if (validationResult.ValidCptIds.Any() || (target.LogicalName == ipg_referral.EntityLogicalName && target.GetAttributeValue<EntityReference>(ipg_referral.Fields.ipg_CPTCodeId1) == null))
                        {
                            validationResult.Response.Succeeded = true;
                        }
                        else
                        {
                            validationResult.Response = GetGatingResponseForCPTNotSupportedByFacilityCarrier(target, dos, iContract.facilityCarriercontract);
                        }
                    }
                    else //Default is excluded
                    {
                        validationResult.ValidCptIds = cptIds.Where(cptId => !contractRules.Any(rule => rule.ipg_CptId.Id.Equals(cptId))).ToList();

                        if (validationResult.ValidCptIds.Any() || (target.LogicalName == ipg_referral.EntityLogicalName && target.GetAttributeValue<EntityReference>(ipg_referral.Fields.ipg_CPTCodeId1) == null))
                        {
                            validationResult.Response.Succeeded = true;
                        }
                        else
                        {
                            validationResult.Response = GetGatingResponseForCPTNotSupportedByFacilityCarrier(target, dos, iContract.facilityCarriercontract);
                        }
                    }

                }
            }
            return validationResult;
        }

        private CptCodesValidationResult CPTSupportedByIPG(Entity target, GateManager gateManager)
        {
            if (!_cptCodes.Any(code => target.Contains(code)))
            {
                return new CptCodesValidationResult
                {
                    Response = new GatingResponse(true)
                };
            }
            else
            {
                DateTime dos = target.GetAttributeValue<DateTime>(Incident.Fields.ipg_SurgeryDate);

                var cptRreferenses = GetNotEmptyCptReferences(target);
                var supportedCpts = cptRreferenses.Where(cpt => IsValidCptByImplantUser(cpt.Id)).ToList();

                if (!supportedCpts.Any())
                {
                    var caseNote = string.Join(",", cptRreferenses.Select(x => $"Billable CPT {x.Name} contains and implant that is not supported by IPG. Case rejected and communication sent to facility. "));
                    var cim = gateManager.GetFacilityCIM(target.GetAttributeValue<EntityReference>("ipg_facilityid").Id);
                    var portalNote = $"IPG is unable to approve this case because the billable CPT, {string.Join(",", supportedCpts.Select(x => x.Name))}, does not include a supported implant. If the physician uses a supported implant, please notify {cim?.FullName} for reconsideration of this case.";
                    return new CptCodesValidationResult
                    {
                        Response = new GatingResponse(false, caseNote, portalNote),
                    };
                }
                else
                {
                    return new CptCodesValidationResult
                    {
                        Response = new GatingResponse(true),
                        ValidCptIds = supportedCpts.Select(x => x.Id).ToList(),
                    };
                }
            }
        }

        #endregion

        #region Helpers
        private ipg_facilitycptprocedurecode RetrieveFacilityCPTProcedureCode(EntityReference facilityRef, EntityReference procedureRef)
        {
            if (facilityRef == null && procedureRef == null)
            {
                return null;
            }
            var query = new QueryExpression
            {
                EntityName = ipg_facilitycptprocedurecode.EntityLogicalName,
                ColumnSet = new ColumnSet(ipg_facilitycptprocedurecode.Fields.ipg_CptCodeId),
                TopCount = 1,
                Criteria = {
                    Conditions = {
                        new ConditionExpression(ipg_facilitycptprocedurecode.Fields.StateCode, ConditionOperator.Equal, (int)ipg_facilitycptprocedurecodeState.Active),
                        new ConditionExpression(ipg_facilitycptprocedurecode.Fields.ipg_FacilityId, ConditionOperator.Equal, facilityRef?.Id),
                        new ConditionExpression(ipg_facilitycptprocedurecode.Fields.ipg_ProcedureDescription, ConditionOperator.Equal, procedureRef?.Name),
                    }
                },
            };
            return crmService.RetrieveMultiple(query).Entities.FirstOrDefault()?.ToEntity<ipg_facilitycptprocedurecode>();
        }

        private bool CptNotExist1stCase(EntityReference carrierRef, EntityReference cptRef, DateTime dosDate)
        {
            QueryExpression query = new QueryExpression
            {
                EntityName = ipg_associatedcpt.EntityLogicalName,
                ColumnSet = new ColumnSet(ipg_associatedcpt.Fields.ipg_name),
                Criteria = {
                    Conditions = {
                        new ConditionExpression(ipg_associatedcpt.Fields.StateCode, ConditionOperator.Equal, (int)ipg_associatedcptState.Active),
                        new ConditionExpression(ipg_associatedcpt.Fields.ipg_CarrierId, ConditionOperator.Equal, carrierRef.Id),
                        new ConditionExpression(ipg_associatedcpt.Fields.ipg_CPTCodeId, ConditionOperator.Equal, cptRef.Id),
                        new ConditionExpression(ipg_associatedcpt.Fields.ipg_EffectiveDate, ConditionOperator.LessEqual, dosDate),
                        new ConditionExpression(ipg_associatedcpt.Fields.ipg_ExpirationDate, ConditionOperator.GreaterEqual, dosDate),
                    }
                }
            };
            var result = crmService.RetrieveMultiple(query);
            return !result.Entities.Any();
        }

        private List<Guid> GetNotEmptyCptCodes(Entity target) => _cptCodes
                    .Where(code => target.Contains(code) && target[code] != null)
                    .Select(code => target.GetAttributeValue<EntityReference>(code).Id)
                    .ToList();

        private List<EntityReference> GetNotEmptyCptReferences(Entity target) => _cptCodes
            .Where(code => target.Contains(code) && target[code] != null)
            .Select(code => target.GetAttributeValue<EntityReference>(code))
            .ToList();

        private GatingResponse GetGatingResponseForCPTNotSupportedByFacilityCarrier(Entity target, DateTime? dos, Entitlement entitlement)
        {
            var facility = new Intake.Account();
            facility = crmService.Retrieve(facility.LogicalName, target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_FacilityId).Id,
                       new ColumnSet(facility.GetAttributeLogicalName(x => x.ipg_FacilityCimId)))
                       .ToEntity<Intake.Account>();

            string facilityCIM = facility.ipg_FacilityCimId != null ? facility.ipg_FacilityCimId.Name : "CIM";

            EntityCollection cptCodes = RetrieveCptCodes(target);
            var response = new GatingResponse();
            response.Succeeded = false;
            response.CaseNote = $"None of the provided CPT code(s) {string.Join(",", cptCodes.Entities.Select(x => ((ipg_cptcode)x).ipg_cptcode1))} " +
                                $"are supported by this {entitlement.Name} relationship for this DOS {dos.Value.ToShortDateString()}. " +
                                $"Case rejected and facility notified.";
            response.PortalNote = $"IPG has rejected this case due to none of the provided CPT " +
                                  $"{string.Join(",", cptCodes.Entities.Select(x => ((ipg_cptcode)x).ipg_cptcode1))} being supported by " +
                                  $"{target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_CarrierId)?.Name} for this facility, {target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_FacilityId)?.Name} for DOS {dos.Value.ToShortDateString()}. " +
                                  $"If the physician uses a supported implant, please notify your CIM, {facilityCIM} for reconsideration of this case.";
            return response;

        }

        private EntityCollection RetrieveCptCodes(Entity target)
        {
            var filter = new FilterExpression
            {
                FilterOperator = LogicalOperator.Or,
            };
            filter.Conditions.AddRange(_cptCodes.Where(code => target.Contains(code) && target[code] != null).Select(code => new ConditionExpression(ipg_cptcode.EntityLogicalName, ConditionOperator.Equal, target.GetAttributeValue<EntityReference>(code))));
            var query = new QueryExpression
            {
                EntityName = ipg_cptcode.EntityLogicalName,
                ColumnSet = new ColumnSet(ipg_cptcode.Fields.ipg_cptcode1),
                Criteria = filter,
            };
            return crmService.RetrieveMultiple(query);
        }

        private bool IsValidCptByImplantUser(Guid cptId)
        {
            var cptCode = crmService.Retrieve(ipg_cptcode.EntityLogicalName, cptId,
                new ColumnSet(ipg_cptcode.Fields.ipg_ImplantUsed)).ToEntity<ipg_cptcode>();

            return (cptCode.ipg_ImplantUsed?.Value == ImplantUsedOptionSet.Yes || cptCode.ipg_ImplantUsed?.Value == ImplantUsedOptionSet.Maybe);
        }

        private IEnumerable<Entity> RetrieveCtpCodesByExpDate(Entity target)
        {
            var nonEmptyCptCodes = GetNotEmptyCptCodes(target);
            if (nonEmptyCptCodes == null || !nonEmptyCptCodes.Any())
            {
                return new List<Entity>();
            }
            var query = new QueryExpression()
            {
                EntityName = ipg_cptcode.EntityLogicalName,
                ColumnSet = new ColumnSet(ipg_cptcode.Fields.ipg_name),
                Criteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Filters = {
                        new FilterExpression {
                            Conditions = {
                                new ConditionExpression(ipg_cptcode.Fields.ipg_cptcodeId, ConditionOperator.In, nonEmptyCptCodes)
                            }
                        },
                        new FilterExpression{
                            FilterOperator = LogicalOperator.Or,
                            Conditions = {
                                new ConditionExpression(ipg_cptcode.Fields.ipg_ExpirationDate, ConditionOperator.LessThan, target.GetCaseDos()),
                                new ConditionExpression(ipg_cptcode.Fields.ipg_EffectiveDate, ConditionOperator.GreaterThan, target.GetCaseDos()),
                            }
                        }
                    }
                }
            };
            return crmService.RetrieveMultiple(query).Entities;
        }
        #endregion
    }

    class CptCodesValidationResult
    {
        internal GatingResponse Response { get; set; }
        internal List<Guid> ValidCptIds { get; set; } = new List<Guid>();
    }
}
