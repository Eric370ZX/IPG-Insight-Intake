using System;
using System.Collections.Generic;
using System.Linq;
using Insight.Intake.Extensions;
using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Runtime.Serialization.Json;
using System.IO;
using Microsoft.Xrm.Sdk.Client;

namespace Insight.Intake.Plugin.Gating
{
    public class GateManager
    {
        private readonly IOrganizationService crmService;



        private readonly ITracingService tracingService;



        private readonly EntityReference _targetRef;

        //can be deleted. Logic replaced to plugin file.
        public GatingResponse CheckDMEBenefitsJQUPrefix()
        {
            var requiredFields = new[] { Incident.Fields.ipg_deductiblemet, Incident.Fields.ipg_deductibleremaining,
                Incident.Fields.ipg_oopmax,Incident.Fields.ipg_oopmet,Incident.Fields.ipg_patientcoinsurance , Incident.Fields.ipg_payercoinsurance };
            var columns = new List<string>() { Incident.Fields.ipg_BenefitTypeCode, Incident.Fields.ipg_MemberIdNumber };
            columns.AddRange(requiredFields);
            var initTarget = crmService.Retrieve(_targetRef.LogicalName, _targetRef.Id, new ColumnSet(columns.ToArray()));
            var memberIdNumber = initTarget.GetAttributeValue<string>(Incident.Fields.ipg_MemberIdNumber) ?? "";


            if (!memberIdNumber.ToUpper().StartsWith("JQU"))
            {
                return new GatingResponse(true, codeOutput: 1);
            }
            var benefitTypeCode = initTarget.GetAttributeValue<OptionSetValue>(Incident.Fields.ipg_BenefitTypeCode)?.Value ?? -1;
            ipg_BenefitType? benefitType = benefitTypeCode != -1 ? (ipg_BenefitType?)benefitTypeCode : null;
            if (benefitType != ipg_BenefitType.DurableMedicalEquipment_DME)
            {
                return new GatingResponse(false);
            }
            /*foreach (var iField in requiredFields)
            {
                if (!initTarget.Contains(iField) || initTarget[iField] == null)
                {
                    return new GatingResponse(false, $"DME Benefits are Not Populated. Field {iField} is empty");
                }
            }*/
            return new GatingResponse(true, codeOutput: 2);
        }

        internal Incident GetCase(Guid caseId, string[] columns)
        {
            return crmService.Retrieve(Incident.EntityLogicalName, caseId, new ColumnSet(columns)).ToEntity<Incident>();
        }

        public GatingResponse CheckPatientPrimaryInfo()
        {
            if (_targetRef.LogicalName != ipg_referral.EntityLogicalName)
            {
                return new GatingResponse(true);
            }
            var requiredFields = new[] { ipg_referral.Fields.ipg_PatientFirstName,
                ipg_referral.Fields.ipg_PatientLastName,
                ipg_referral.Fields.ipg_PatientDateofBirth};

            var referral = crmService.Retrieve(_targetRef.LogicalName, _targetRef.Id, new ColumnSet(requiredFields));
            if (requiredFields.Any(p => !referral.Contains(p) || referral[p] == null))
            {
                return new GatingResponse
                {
                    Succeeded = false,
                    CustomMessage = "Some of required fields are empty",
                };
            }
            return new GatingResponse(true);
        }

        public GatingResponse CheckPropelNasalStentOnProcedureName()
        {
            var preauthManager = new PreAuthGatingManager(crmService, tracingService, _targetRef);
            Func<IEnumerable<ipg_cptcode>, GatingResponse> validate =
                (IEnumerable<ipg_cptcode> caseCptCodes) =>
                {
                    if (caseCptCodes.Any())
                    {
                        return new GatingResponse(true);
                    }
                    EntityReference procedure = null;
                    if(_targetRef.LogicalName.Equals(Incident.EntityLogicalName))
                    {
                        procedure = (crmService.Retrieve(_targetRef.LogicalName, _targetRef.Id, new ColumnSet(Incident.Fields.ipg_procedureid)).ToEntity<Incident>()).ipg_procedureid;
                    }
                    else
                    {
                        procedure = (crmService.Retrieve(_targetRef.LogicalName, _targetRef.Id, new ColumnSet(ipg_referral.Fields.ipg_ProcedureNameId)).ToEntity<ipg_referral>()).ipg_ProcedureNameId;
                    }
                    if (procedure == null)
                        return new GatingResponse(true);
                    var procCptCodesQuery = new QueryExpression(ipg_cptcode.EntityLogicalName);
                    procCptCodesQuery.Criteria.AddCondition("ipg_procedurename", ConditionOperator.Equal, procedure.Id);
                    procCptCodesQuery.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)ipg_cptcodeState.Active);
                    procCptCodesQuery.ColumnSet = new ColumnSet(true);
                    var procedureCptCodes = crmService.RetrieveMultiple(procCptCodesQuery).Entities.Select(p => p.ToEntity<ipg_cptcode>());
                    return new GatingResponse(procedureCptCodes.All(p => p.ipg_PropelNasalStent != true));
                };

            return preauthManager.ValidateWithCPT(GetTasks(ipg_TaskType1.PropelPreAuthCheck_ProcedureName), validate);

        }
        public GatingResponse CheckPreAuthMemberId()
        {
            var preauthManager = new PreAuthGatingManager(crmService, tracingService, _targetRef);
            Func<GatingResponse> validate = () =>
            {
                var initTarget = crmService.Retrieve(_targetRef.LogicalName, _targetRef.Id, new ColumnSet(Incident.Fields.ipg_MemberIdNumber));
                var memberidnumber = initTarget.GetAttributeValue<string>(Incident.Fields.ipg_MemberIdNumber);

                if (string.IsNullOrEmpty(memberidnumber))
                {
                    return new GatingResponse(true);
                }
                if (memberidnumber.ToUpper().StartsWith("PXN") || memberidnumber.ToUpper().StartsWith("PBB"))
                {
                    return new GatingResponse(false);
                }
                return new GatingResponse(true);
            };
            return preauthManager.SimpleValidate(GetTasks(ipg_TaskType1.CaserequiresPublixGroupAuthorization), validate);
        }
        public GatingResponse CheckPropelNasalStentOnCPT()
        {
            var preauthManager = new PreAuthGatingManager(crmService, tracingService, _targetRef);
            Func<IEnumerable<ipg_cptcode>, GatingResponse> validate =
                (IEnumerable<ipg_cptcode> cptCodes) => new GatingResponse(cptCodes.All(p => p.ipg_PropelNasalStent != true));

            return preauthManager.ValidateWithCPT(GetTasks(ipg_TaskType1.PropelPreAuthCheck), validate);
        }


        private static class Constants
        {
            public static readonly string Task_CreditBalance = "Patient Credit Balance Remaining";
            public static readonly string Task_CreditBalanceCarrier = "Carrier Credit Balance Remaining";
        }

        public GatingResponse CheckHighDollarAuth()
        {
            var existingTasks = GetTasks(ipg_TaskType1.HighDollarAuthorization);
            if (existingTasks.Any(p => p.StateCode != TaskState.Open))
            {
                return new GatingResponse(true);
            }
            var cptCodesFields = new[] { "ipg_cptcodeid1", "ipg_cptcodeid2", "ipg_cptcodeid3", "ipg_cptcodeid4", "ipg_cptcodeid5", "ipg_cptcodeid6" };
            const string categoryField = "ipg_cptcategory";
            var sourceCase = crmService
                .Retrieve(_targetRef.LogicalName, _targetRef.Id, new ColumnSet(cptCodesFields));
            Func<EntityReference, Entity> getCpt = (EntityReference cptRef) => cptRef != null ? crmService.Retrieve(cptRef.LogicalName, cptRef.Id, new ColumnSet(categoryField)) : null;
            var cptCodes = cptCodesFields
                .Select(p => getCpt(sourceCase.GetAttributeValue<EntityReference>(p)))
                .Where(p => p != null);
            if (cptCodes.Any(p => p.FormattedValues.Contains(categoryField) && p.FormattedValues[categoryField].Contains("Auth and Reporting")))
            {
                return new GatingResponse(false);
            }
            return new GatingResponse(true);
        }

        public GatingResponse CheckCasePartDetails()
        {
            var query = new QueryExpression(ipg_casepartdetail.EntityLogicalName);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)ipg_casepartdetailState.Active);
            query.Criteria.AddCondition("ipg_caseid", ConditionOperator.Equal, _targetRef.Id);
            var parts = crmService.RetrieveMultiple(query).Entities;
            if (parts.Any())
            {
                return new GatingResponse(true);
            }
            return new GatingResponse(false);
        }

        public GatingResponse ValidatePreAuthHCPCS()
        {
            var sourceCase = crmService.Retrieve(_targetRef.LogicalName, _targetRef.Id, new ColumnSet(Incident.Fields.ipg_CarrierId, Incident.Fields.ipg_SurgeryDate, Incident.Fields.ipg_ActualDOS))
                .ToEntity<Incident>();
            if (sourceCase.ipg_CarrierId == null || sourceCase.GetCaseDos() == null)
            {
                return new GatingResponse(false, "Carrier is missing on the case");
            }
            var fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                  <entity name='ipg_masterhcpcs'>
                    <attribute name='ipg_masterhcpcsid' />
                    <attribute name='ipg_name' />
                    <filter type='and'>
                      <condition attribute='statecode' operator='eq' value='0' />
                    </filter>
                    <link-entity name='ipg_casepartdetail' from='ipg_hcpcscode' to='ipg_masterhcpcsid' link-type='inner' alias='aa'>
                      <filter type='and'>
                        <condition attribute='ipg_caseid' operator='eq' value='{sourceCase.Id}' />
                      </filter>
                    </link-entity>
                    <link-entity name='ipg_carrierprecerthcpcs' from='ipg_chargecenterhcpcs' to='ipg_masterhcpcsid' link-type='inner' alias='ab'>
                        <filter type='and'>
                          <condition attribute='ipg_effectiveenddate' operator='on-or-after' value='{sourceCase.GetCaseDos().Value.ToString("yyyy-MM-dd")}' />
                          <condition attribute='ipg_effectivestartdate' operator='on-or-before' value='{sourceCase.GetCaseDos().Value.ToString("yyyy-MM-dd")}' />
                        </filter>
                      <link-entity name='ipg_carrierprecertcpt' from='ipg_carrierprecertcptid' to='ipg_carrierprecertcptid' link-type='inner' alias='ac'>
                        <filter type='and'>
                          <condition attribute='ipg_carrierid' operator='eq' value='{sourceCase.ipg_CarrierId.Id}' />
                        </filter>
                      </link-entity>
                    </link-entity>
                  </entity>
                </fetch>";

            var relatedHCPCS = crmService.RetrieveMultiple(new FetchExpression(fetchXml));
            if (relatedHCPCS.Entities.Any())
            {
                var hcpcsNames = relatedHCPCS.Entities.Select(p => p.ToEntity<ipg_masterhcpcs>().ipg_name);
                return new GatingResponse(false, $"HCPCS {string.Join(", ", hcpcsNames)} Require Auth for {sourceCase.ipg_CarrierId?.Name}")
                {
                    TaskDescripton = $"HCPCS {string.Join(", ", hcpcsNames)} Require Auth for {sourceCase.ipg_CarrierId?.Name}"
                };
            }
            return new GatingResponse(true);
        }

        public GatingResponse CheckRequiredAuthForHMO()
        {

            var columns = new[] { "ipg_benefitplantypecode", "ipg_memberidnumber", "ipg_ipgauthnumber" };
            if (_targetRef.LogicalName == ipg_referral.EntityLogicalName)
            {
                columns = new[] { "ipg_memberidnumber", "ipg_ipgauthnumber" };

            }
            var source = crmService.Retrieve(_targetRef.LogicalName, _targetRef.Id, new ColumnSet(columns));
            if (string.IsNullOrEmpty(source.GetAttributeValue<string>(Incident.Fields.ipg_MemberIdNumber)))
            {
                return new GatingResponse(false, "MemberId is empty");
            }

            if (source.FormattedValues.Contains("ipg_benefitplantypecode")
                && source.FormattedValues["ipg_benefitplantypecode"] != null
                && source.FormattedValues["ipg_benefitplantypecode"].Contains("HMO"))
            {
                if (string.IsNullOrEmpty(source.GetAttributeValue<string>(Incident.Fields.ipg_ipgauthnumber)))
                {
                    return new GatingResponse(false);
                }
                else
                {
                    return new GatingResponse(true);
                }
            }
            return new GatingResponse(true);
        }

        public GatingResponse CheckDxGender()
        {
            var patientGenderFieldName = _targetRef.LogicalName == ipg_referral.EntityLogicalName ? ipg_referral.Fields.ipg_gender : Incident.Fields.ipg_PatientGender;
            var targetName = _targetRef.LogicalName == Incident.EntityLogicalName ? "case" : "referral";

            var dxCodes = new List<string>() { Incident.Fields.ipg_DxCodeId1, Incident.Fields.ipg_DxCodeId2, Incident.Fields.ipg_DxCodeId3, Incident.Fields.ipg_DxCodeId4, Incident.Fields.ipg_DxCodeId5, Incident.Fields.ipg_DxCodeId6 };
            var columns = new List<string>() { patientGenderFieldName };
            columns.AddRange(dxCodes);
            var sourceTarget = crmService.Retrieve(_targetRef.LogicalName, _targetRef.Id, new ColumnSet(columns.ToArray()));
            var result = false;

            var patientGenderCode = sourceTarget.GetAttributeValue<OptionSetValue>(patientGenderFieldName)?.Value;

            if (patientGenderCode == null)
            {
                return new GatingResponse()
                {
                    Succeeded = false,
                    CaseNote = $"No patient gender on the {targetName}",
                    PortalNote = $"No patient gender on the {targetName}",
                    GatingOutcome = GatingOutcomes.NoPatientGenderOnTheCase
                };
            }

            if (patientGenderCode == (int)ipg_Gender.Other)
            {
                result = true;
            }
            else
            {
                bool allDxGendersAreValid = true;
                foreach (var iFieldName in dxCodes)
                {
                    var dxCodeRef = sourceTarget.GetAttributeValue<EntityReference>(iFieldName);
                    if (dxCodeRef == null)
                    {
                        continue;
                    }
                    var sourceDxCode = crmService.Retrieve(ipg_dxcode.EntityLogicalName, dxCodeRef.Id, new ColumnSet(ipg_dxcode.Fields.ipg_gender))
                        .ToEntity<ipg_dxcode>();
                    if (sourceDxCode.ipg_genderEnum == ipg_Gender.Other)
                    {
                        continue;
                    }
                    if (!((sourceDxCode.ipg_genderEnum == ipg_Gender.Male && patientGenderCode == (int)ipg_Gender.Male)
                        || (sourceDxCode.ipg_genderEnum == ipg_Gender.Female && patientGenderCode == (int)ipg_Gender.Female)))
                    {
                        allDxGendersAreValid = false;
                    }
                }
                result = allDxGendersAreValid;
            }
            if (result)
            {
                return new GatingResponse(true, "Primary Dx Appropriate for Patient Gender", "Primary Dx Appropriate for Patient Gender");
            }
            return new GatingResponse(false, "Primary Dx Mismatch to Patient Gender",
                "Primary Dx Mismatch to Patient Gender", GatingOutcomes.PrimaryDxMismatchToPatientGender);
        }

        public bool HasUserTask(string subject)
        {
            var query = new QueryExpression("task");
            query.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, _targetRef.Id);
            query.Criteria.AddCondition("subject", ConditionOperator.Equal, subject);
            var tasks = crmService.RetrieveMultiple(query);
            return tasks.Entities.Any();
        }

        public Intake.Account GetCarrier()
        {
            var fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                          <entity name='account'>
                            <attribute name='name' />
                            <attribute name='accountid' />
                            <attribute name='ipg_timelyfilingrule' />
                            <order attribute='name' descending='false' />";
            fetch += '\n';
            if (_targetRef.LogicalName == Incident.EntityLogicalName)
            {
                fetch += $@"<link-entity name='incident' from='ipg_carrierid' to='accountid' link-type='inner' alias='aa'>
                              <filter type='and'>
                                <condition attribute='incidentid' operator='eq' value='{_targetRef.Id}' />
                              </filter>
                            </link-entity>
                          </entity>
                        </fetch>";
            }
            else
            {
                fetch += $@"<link-entity name='ipg_referral' from='ipg_carrierid' to='accountid' link-type='inner' alias='aa'>
                                <filter type='and'>
                                    <condition attribute='ipg_referralid' operator='eq' value='{_targetRef.Id}' />
                                </filter>
                            </link-entity>
                        </entity>
                    </fetch>";
            }
            var accounts = crmService.RetrieveMultiple(new FetchExpression(fetch)).Entities;
            return accounts.FirstOrDefault()?.ToEntity<Intake.Account>();
        }

        public GatingResponse CheckIncidentBalances()
        {
            var sourceCase = crmService.Retrieve(_targetRef.LogicalName, _targetRef.Id, new ColumnSet("ipg_remainingpatientbalance", "ipg_remainingcarrierbalance", "ipg_totalbalance"))
               .ToEntity<Incident>();
            var hasPatientBalance = sourceCase.ipg_RemainingPatientBalance != null && sourceCase.ipg_RemainingPatientBalance.Value != 0;
            var hasPayerBalance = sourceCase.ipg_RemainingCarrierBalance != null && sourceCase.ipg_RemainingCarrierBalance.Value != 0;
            var hasTotalBalance = sourceCase.ipg_TotalBalance != null && sourceCase.ipg_TotalBalance.Value != 0;
            if (!(hasPatientBalance || hasPayerBalance || hasTotalBalance))
            {
                return new GatingResponse(true);
            }
            return new GatingResponse(false, $"There is balance left on the case. Patient balance: {sourceCase.ipg_RemainingPatientBalance?.Value}, Payer balance: {sourceCase.ipg_RemainingCarrierBalance?.Value}, Total Balance: {sourceCase.ipg_TotalBalance?.Value}");
        }
        public GatingReponsePatientBalance CheckPayerPatientBalance()
        {
            var sourceCase = crmService.Retrieve(_targetRef.LogicalName, _targetRef.Id, new ColumnSet(nameof(Incident.ipg_RemainingCarrierBalance).ToLower(), nameof(Incident.ipg_RemainingSecondaryCarrierBalance).ToLower(), nameof(Incident.ipg_RemainingPatientBalance).ToLower(), nameof(Incident.ipg_CaseBalance).ToLower()))
               .ToEntity<Incident>();
            var hasPatientBalance = sourceCase.ipg_RemainingPatientBalance != null && sourceCase.ipg_RemainingPatientBalance.Value != 0;
            var hasPayerBalance = (sourceCase.ipg_RemainingCarrierBalance != null && sourceCase.ipg_RemainingCarrierBalance.Value != 0) || (sourceCase.ipg_RemainingSecondaryCarrierBalance != null && sourceCase.ipg_RemainingSecondaryCarrierBalance.Value != 0);
            var hasTotalBalance = sourceCase.ipg_CaseBalance != null && sourceCase.ipg_CaseBalance.Value != 0;
            if (!hasPatientBalance && !hasPayerBalance && hasTotalBalance)
            {
                var description = $"Payer and Patient Balances are $0 but Total Balance <> $0";
                var subject = "Balance Errors - 1";

                return new GatingReponsePatientBalance(false, description, null, subject);

            }
            if ((hasPatientBalance || hasPayerBalance) && !hasTotalBalance)
            {
                var description = $"Total Balance is $0 but Either Payer or Balance Balance is <> $0";
                var subject = "Balance Errors - 2";
                return new GatingReponsePatientBalance(false, description, null, subject);
            }
            return new GatingReponsePatientBalance(true);
        }
        private IEnumerable<Task> GetTasks(int[] missingInformationTypes)
        {
            var conditionsValue = string.Join("", missingInformationTypes.Select(p => $"<value>{p}</value>"));
            var fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                          <entity name='task'>
                            <all-attributes />
                            <order attribute='subject' descending='false' />
                            <filter type='and'>
                              <condition attribute='regardingobjectid' operator='eq' uitype='incident' value='{_targetRef.Id}' />
                              <condition attribute='ipg_tasktypecode' operator='in'>
                                {conditionsValue}
                              </condition>
                            </filter>
                          </entity>
                        </fetch>";
            var docs = crmService.RetrieveMultiple(new FetchExpression(fetchXml))
               .Entities
               .Select(p => p.ToEntity<Task>());
            return docs;
        }
        private IEnumerable<Task> GetTasks(ipg_TaskType1 taskType)
        {
            var fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                          <entity name='task'>
                            <all-attributes />
                            <order attribute='subject' descending='false' />
                            <filter type='and'>
                              <condition attribute='regardingobjectid' operator='eq' uitype='incident' value='{_targetRef.Id}' />
                              <condition attribute='ipg_tasktypecode' operator='eq' value='{(int)taskType}' />
                            </filter>
                          </entity>
                        </fetch>";
            var docs = crmService.RetrieveMultiple(new FetchExpression(fetchXml))
               .Entities
               .Select(p => p.ToEntity<Task>());
            return docs;
        }

        public IEnumerable<ipg_document> GetDocuments(string docTypeAbbr, string primaryEntityLookupName = "ipg_caseid")
        {
            var fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='ipg_document'>
                        <attribute name='ipg_documentid' />
                        <filter type='and'>
                          <condition attribute='{primaryEntityLookupName}' operator='eq' value='{_targetRef.Id}' />
                          <condition attribute='statuscode' operator='eq' value='1' />
                        </filter>
                        <link-entity name='ipg_documenttype' from='ipg_documenttypeid' to='ipg_documenttypeid' link-type='inner' alias='aa'>
                          <filter type='and'>
                            <condition attribute='ipg_documenttypeabbreviation' operator='eq' value='{docTypeAbbr}' />
                          </filter>
                        </link-entity>
                      </entity>
                    </fetch>";
            var docs = crmService.RetrieveMultiple(new FetchExpression(fetchXml))
                .Entities
                .Select(p => p.ToEntity<ipg_document>());
            return docs;
        }

        public GatingResponse ValidateBSAVerification()
        {
            var sourceEntityName = _targetRef.LogicalName;
            var source = crmService.Retrieve(_targetRef.LogicalName, _targetRef.Id,
                new ColumnSet(Incident.Fields.ipg_ActualDOS, Incident.Fields.ipg_SurgeryDate))
                .ToEntity<Incident>();
            var dos = source.GetCaseDos();

            var query = new QueryExpression("account");
            query.ColumnSet = new ColumnSet("ipg_facilitysignedbsa", "name", "createdon", "ipg_bsasigneddate", "ipg_bsanotrequiredreason");

            var linkEntity = new LinkEntity("account", sourceEntityName, "accountid", "ipg_facilityid", JoinOperator.Inner);
            linkEntity.LinkCriteria.AddCondition($"{sourceEntityName}id", ConditionOperator.Equal, _targetRef.Id);
            query.LinkEntities.Add(linkEntity);

            var facility = crmService.RetrieveMultiple(query).Entities.FirstOrDefault().ToEntity<Intake.Account>();

            var signedBSA = facility.ipg_FacilitySignedBSA;
            if (signedBSA?.Value == (int)ipg_SignedBSA.Yes || signedBSA?.Value == (int)ipg_SignedBSA.NotRequired)
            {
                return new GatingResponse(true);
            }
            return new GatingResponse() { Succeeded = false };
        }
        private IEnumerable<Entitlement> GetBSA(Guid facilityId, DateTime? dos)
        {
            if (dos == null)
            {
                return Enumerable.Empty<Entitlement>();
            }
            var queryExpression = new QueryExpression(Entitlement.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(Entitlement.ipg_EntitlementType).ToLower(), ConditionOperator.Equal, (int)ipg_EntitlementTypes.BSA),
                            new ConditionExpression(nameof(Entitlement.ipg_FacilityId).ToLower(), ConditionOperator.Equal, facilityId),
                            new ConditionExpression(nameof(Entitlement.StartDate).ToLower(), ConditionOperator.LessEqual, dos),
                            new ConditionExpression(nameof(Entitlement.EndDate).ToLower(), ConditionOperator.GreaterEqual, dos)
                        }
                }
            };
            var entitlements = crmService.RetrieveMultiple(queryExpression).Entities.Select(p => p.ToEntity<Entitlement>());
            return entitlements;
        }

        public bool ValidateBVFForm()
        {
            var result = GetDocuments(Insight.Intake.Helpers.Constants.DocumentTypeAbbreviations.BVF).Any();
            return result;
        }
        public bool CheckCarrierIsAcceptedReferral(Guid referralId)
        {
            var fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
              <entity name='account'>
                <attribute name='ipg_carrieraccepted' />
                <link-entity name='ipg_referral' from='ipg_carrierid' to='accountid' link-type='inner' alias='aj'>
                  <filter type='and'>
                    <condition attribute='ipg_referralid' operator='eq' value='{referralId}' />
                  </filter>
                </link-entity>
              </entity>
            </fetch>";
            var carrier = crmService.RetrieveMultiple(new FetchExpression(fetch)).Entities.FirstOrDefault();
            if (carrier == null)
            {
                return false;
            }
            return carrier.GetAttributeValue<bool>("ipg_carrieraccepted");
        }
        public bool CheckCarrierIsAccepted()
        {
            var fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
              <entity name='account'>
                <attribute name='ipg_carrieraccepted' />
                <link-entity name='{_targetRef.LogicalName}' from='ipg_carrierid' to='accountid' link-type='inner' alias='aj'>
                  <filter type='and'>
                    <condition attribute='{_targetRef.LogicalName}id' operator='eq' value='{_targetRef.Id}' />
                  </filter>
                </link-entity>
              </entity>
            </fetch>";
            var carrier = crmService.RetrieveMultiple(new FetchExpression(fetch)).Entities.FirstOrDefault();
            if (carrier == null)
            {
                return false;
            }
            return carrier.GetAttributeValue<bool>("ipg_carrieraccepted");
        }

        public BalanceValidationResult CheckCarrierBalance()
        {
            var sourceCase = crmService.Retrieve(_targetRef.LogicalName, _targetRef.Id, new ColumnSet(nameof(Incident.ipg_RemainingCarrierBalance).ToLower(), nameof(Incident.ipg_RemainingSecondaryCarrierBalance).ToLower())).ToEntity<Incident>();

            var result = new BalanceValidationResult()
            {
                IsSuccess = false
            };
            //if (sourceCase.ipg_ActualCarrierResponsibility == null || sourceCase.ipg_ActualCarrierResponsibility?.Value == 0)
            if ((sourceCase.ipg_RemainingCarrierBalance == null || sourceCase.ipg_RemainingCarrierBalance?.Value == 0) && (sourceCase.ipg_RemainingSecondaryCarrierBalance == null || sourceCase.ipg_RemainingSecondaryCarrierBalance?.Value == 0))
            {
                result.IsSuccess = true;
                result.BalanceValidation = BalanceValidation.Zero;
            }
            else
            {
                result.IsSuccess = false;
                result.BalanceValidation = ((sourceCase.ipg_RemainingCarrierBalance != null ? sourceCase.ipg_RemainingCarrierBalance?.Value : 0) + (sourceCase.ipg_RemainingSecondaryCarrierBalance != null ? sourceCase.ipg_RemainingSecondaryCarrierBalance?.Value : 0) >= 0 ? BalanceValidation.Positive : BalanceValidation.Negative);
            }
            return result;
        }

        public void CreateUserTask(string subject, string body, ipg_TaskType1? taskType = null)
        {
            var task = new Task()
            {
                RegardingObjectId = _targetRef,
                Subject = subject,
                Description = body,
                ipg_tasktypecodeEnum = taskType
            };
            crmService.Create(task);
        }
        public GateManager(IOrganizationService organizationService, ITracingService tracingService, EntityReference caseRef)
        {
            this.crmService = organizationService;
            this.tracingService = tracingService;
            this._targetRef = caseRef;
        }

        public bool HasSubmittedClaims()
        {
            var query = new QueryExpression(Invoice.EntityLogicalName);
            query.Criteria.AddCondition(Invoice.Fields.ipg_caseid, ConditionOperator.Equal, _targetRef.Id);
            query.Criteria.AddCondition(Invoice.Fields.ipg_active, ConditionOperator.Equal, true);
            query.Criteria.AddCondition(Invoice.Fields.ipg_Status, ConditionOperator.Equal, (int)ipg_ClaimStatus.Submitted);
            var result = crmService.RetrieveMultiple(query).Entities;
            return result.Count > 0;
        }

        public BalanceValidationResult CheckPatientBalance()
        {
            var sourceCase = crmService.Retrieve(_targetRef.LogicalName, _targetRef.Id, new ColumnSet(nameof(Incident.ipg_RemainingPatientBalance).ToLower())).ToEntity<Incident>();

            var result = new BalanceValidationResult()
            {
                IsSuccess = false
            };
            if (sourceCase.ipg_RemainingPatientBalance == null || sourceCase.ipg_RemainingPatientBalance?.Value == 0)
            {
                result.IsSuccess = true;
                result.BalanceValidation = BalanceValidation.Zero;
            }
            if (sourceCase.ipg_RemainingPatientBalance?.Value > 0)
            {
                result.IsSuccess = false;
                result.BalanceValidation = BalanceValidation.Positive;
            }
            if (sourceCase.ipg_RemainingPatientBalance?.Value < 0)
            {
                result.IsSuccess = false;
                result.BalanceValidation = BalanceValidation.Negative;
            }
            return result;
        }

        public bool HasUserTask_CreditBalance()
        {
            return HasUserTask(Constants.Task_CreditBalance);
        }
        public bool HasUserTask_CreditBalanceCarrier()
        {
            return HasUserTask(Constants.Task_CreditBalanceCarrier);
        }


        public void CreateUserTask_CreditBalance()
        {
            CreateUserTask(Constants.Task_CreditBalance, "Patient Credit Balance Remaining");
        }

        public void CreateUserTask_CreditBalanceCarrier()
        {
            CreateUserTask(Constants.Task_CreditBalanceCarrier, "Carrier Credit Balance Remaining");
        }

        public void CloseOutstandingTasks()
        {
            var statementTasks = GetStatementTasks(_targetRef);
            foreach (var iTask in statementTasks)
            {
                var taskStateRequest = new SetStateRequest()
                {
                    EntityMoniker = iTask.ToEntityReference(),
                    State = new OptionSetValue((int)ipg_statementgenerationtaskState.Inactive),
                    Status = new OptionSetValue((int)ipg_statementgenerationtask_StatusCode.Completed)
                };
                crmService.Execute(taskStateRequest);
            }
        }

        private IEnumerable<Intake.ipg_statementgenerationtask> GetStatementTasks(EntityReference caseRef)
        {
            var query = new QueryExpression(Intake.ipg_statementgenerationtask.EntityLogicalName);
            query.Criteria.AddCondition("ipg_caseid", ConditionOperator.Equal, caseRef.Id);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)ipg_statementgenerationtaskState.Active);
            query.Criteria.AddCondition("ipg_issent", ConditionOperator.NotEqual, true);
            var result = crmService.RetrieveMultiple(query)
                .Entities
                .Select(p => p.ToEntity<Intake.ipg_statementgenerationtask>());
            return result;

        }
        private IEnumerable<Intake.ipg_casepartdetail> GetCasePartDetails()
        {
            var query = new QueryExpression(ipg_casepartdetail.EntityLogicalName);
            query.ColumnSet = new ColumnSet(true);
            query.Criteria.AddCondition("ipg_caseid", ConditionOperator.Equal, _targetRef.Id);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)ipg_casepartdetailState.Active);
            return crmService.RetrieveMultiple(query).Entities.Select(p => p.ToEntity<ipg_casepartdetail>());
        }

        //Soumitra: I am adding this function here because I believe this can be used by multiple gate plugins. 
        public SystemUser GetFacilityCIM(Guid facilityId)
        {
            var facility = crmService.Retrieve(Intake.Account.EntityLogicalName,
                                               facilityId,
                                               new ColumnSet(LogicalNameof<Intake.Account>
                                               .Property(x => x.ipg_FacilityCimId))).ToEntity<Intake.Account>();
            if (facility != null && facility.ipg_FacilityCimId != null)
            {
                Entity customerInterfaceManager = crmService.Retrieve(SystemUser.EntityLogicalName, facility.ipg_FacilityCimId.Id, new ColumnSet(LogicalNameof<SystemUser>.Property(x => x.FirstName),
                    LogicalNameof<SystemUser>.Property(x => x.LastName),
                    LogicalNameof<SystemUser>.Property(x => x.FullName)));
                if (customerInterfaceManager != null) return customerInterfaceManager.ToEntity<SystemUser>();
            }
            return null;
        }

        public bool CheckValidatedAcceptedClaim()
        {
            var query = new QueryExpression(Insight.Intake.Invoice.EntityLogicalName);
            query.Criteria.AddCondition("ipg_caseid", ConditionOperator.Equal, _targetRef.Id);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)Insight.Intake.InvoiceState.Active);
            query.Criteria.AddCondition("ipg_status", ConditionOperator.Equal, (int)Insight.Intake.ipg_ClaimStatus.Validated);
            query.Criteria.AddCondition("ipg_reason", ConditionOperator.In, (int)Insight.Intake.ipg_ClaimReason.RejectedbyIntermediary, (int)Insight.Intake.ipg_ClaimReason.RejectedbyPayor);
            var result = crmService.RetrieveMultiple(query).Entities;
            return result.Count == 0;
        }
        public bool CheckSupportedParts()
        {
            var result = true;
            var casePartDetails = GetCasePartDetails();
            foreach (var iDetail in casePartDetails)
            {
                var unsupportedParts = getUnsupportedParts(iDetail);
                if (unsupportedParts.Any())
                {
                    continue;
                }
                if (iDetail.ipg_PurchaseOrderId == null)
                {
                    return false;
                }
            }
            var groupedByProduct = casePartDetails.GroupBy(p => p.ipg_productid?.Id);
            foreach (var iGProduct in groupedByProduct)
            {
                if (iGProduct.Sum(p => p.ipg_quantity) == 0 && iGProduct.Any(p => p.ipg_quantity != null && p.ipg_quantity != 0))
                {
                    return false;
                }
            }
            return result;
        }

        private IEnumerable<ipg_unsupportedpart> getUnsupportedParts(ipg_casepartdetail partDetail)
        {
            var sourceCase = crmService.Retrieve(_targetRef.LogicalName, _targetRef.Id, new ColumnSet("ipg_carrierid", "ipg_homeplancarrierid")).ToEntity<Incident>();
            var query = new QueryExpression(ipg_unsupportedpart.EntityLogicalName);
            if (partDetail.ipg_productid != null)
            {
                query.Criteria.AddCondition("ipg_productid", ConditionOperator.Equal, partDetail.ipg_productid.Id);
            }
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)ipg_unsupportedpartState.Active);
            query.Criteria.AddCondition("ipg_effectivedate", ConditionOperator.LessEqual, DateTime.Now);
            var carrierExpr = new FilterExpression(LogicalOperator.Or);
            if (sourceCase.ipg_CarrierId != null)
            {
                carrierExpr.AddCondition("ipg_carrierid", ConditionOperator.Equal, sourceCase.ipg_CarrierId.Id);
            }
            if (sourceCase.ipg_HomePlanCarrierId != null)
            {
                carrierExpr.AddCondition("ipg_carrierid", ConditionOperator.Equal, sourceCase.ipg_HomePlanCarrierId.Id);
            }
            query.Criteria.AddFilter(carrierExpr);
            return crmService.RetrieveMultiple(query).Entities.Select(p => p.ToEntity<ipg_unsupportedpart>());
        }

        public enum BalanceValidation
        {
            Negative,
            Zero,
            Positive
        }
        public class BalanceValidationResult
        {
            public BalanceValidation BalanceValidation { get; set; }
            public bool IsSuccess { get; set; }
        }

        public Dictionary<string, string> SplitArguments(string arguments)
        {
            return JSONDesrilize<Dictionary<string, string>>(arguments).ToDictionary(x => x.Key, x => x.Value);
        }

        private T JSONDesrilize<T>(string JSONdata)
        {
            DataContractJsonSerializer jsonSer = new DataContractJsonSerializer(typeof(T), new DataContractJsonSerializerSettings()
            {
                UseSimpleDictionaryFormat = true
            });
            using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JSONdata)))
            {
                return (T)jsonSer.ReadObject(stream);
            }
        }

        public DateTime ArgumentToDate(string argument)
        {
            return DateTime.ParseExact(argument, "M/d/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
        }

        public EntityReference ArgumentToEntityReference(string argument, string logicalName)
        {
            if (string.IsNullOrEmpty(argument))
            {
                return null;
            }
            return new EntityReference(logicalName, new Guid(argument));
        }

        public string ObjectToString(object o)
        {
            var result = "";
            if (o != null)
            {
                if (o.GetType() == typeof(OptionSetValue))
                {
                    result = ((OptionSetValue)o).Value.ToString();
                }
                else if (o.GetType() == typeof(EntityReference))
                {
                    result = ((EntityReference)o).Id.ToString();
                }
                else if (o.GetType() == typeof(bool))
                {
                    result = (bool)o ? "1" : "0";
                }
                else if (o.GetType() == typeof(DateTime))
                {
                    result = ((DateTime)o).ToString("G", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                }
                else if (o.GetType() == typeof(Money))
                {
                    result = o == null ? string.Empty : ((Money)o).Value.ToString();
                }
                else
                {
                    result = o.ToString();
                }
            }
            return result;
        }

        public string ObjectPresentation(object o)
        {
            var result = "";
            if (o != null)
            {
                if (o.GetType() == typeof(EntityReference))
                {
                    var entityRef = (EntityReference)o;
                    var entityName = entityRef.LogicalName.StartsWith("ipg") ? entityRef.LogicalName : entityRef.LogicalName.First().ToString().ToUpper() + entityRef.LogicalName.Substring(1);
                    var primaryNameAttribute = Type.GetType("Insight.Intake." + entityName).GetField("PrimaryNameAttribute").GetValue(new Entity(entityRef.LogicalName, (entityRef.Id)));
                    var entity = crmService.Retrieve(entityRef.LogicalName, entityRef.Id, new ColumnSet((string)primaryNameAttribute));
                    if (entity.Contains((string)primaryNameAttribute) && entity[(string)primaryNameAttribute] != null)
                    {
                        result = (string)entity[(string)primaryNameAttribute];
                    }
                }
                else
                {
                    result = ObjectToString(o);
                }
            }
            return result;
        }

        public string GetCaseNumber(Entity entity, string messageName)
        {
            if (string.Equals(entity.LogicalName, Incident.EntityLogicalName))
            {
                var numberField = nameof(Incident.Title).ToLower();
                if (entity.Contains(numberField))
                {
                    return entity.GetAttributeValue<string>(numberField);
                }
                var incident = crmService.Retrieve(entity.LogicalName, entity.Id, new ColumnSet(numberField)).ToEntity<Incident>();
                if (incident.Contains(numberField))
                {
                    return incident.GetAttributeValue<string>(numberField);
                }
                return string.Empty;
            }
            else if (string.Equals(entity.LogicalName, ipg_referral.EntityLogicalName))
            {
                if (entity.Contains(nameof(ipg_referral.ipg_referralnumber).ToLower()))
                {
                    return (string)entity[nameof(ipg_referral.ipg_referralnumber).ToLower()];
                }
                else if (entity.Contains(nameof(ipg_referral.ipg_referralcasenumber).ToLower()))
                {
                    return (string)entity[nameof(ipg_referral.ipg_referralcasenumber).ToLower()];
                }
                else
                {
                    if (messageName == MessageNames.Update)
                    {
                        var referral = crmService.Retrieve(entity.LogicalName, entity.Id, new ColumnSet(nameof(ipg_referral.ipg_referralnumber).ToLower(), nameof(ipg_referral.ipg_referralcasenumber).ToLower())).ToEntity<ipg_referral>();
                        if (referral.Contains(nameof(ipg_referral.ipg_referralnumber).ToLower()) && !string.IsNullOrEmpty(referral.ipg_referralnumber))
                        {
                            return referral.ipg_referralnumber;
                        }
                        else if (referral.Contains(nameof(ipg_referral.ipg_referralcasenumber).ToLower()) && !string.IsNullOrEmpty(referral.ipg_referralcasenumber))
                        {
                            return referral.ipg_referralcasenumber;
                        }
                    }
                }
            }
            return string.Empty;
        }

        public string GetGlobalSettingValueByKey(string key)
        {
            QueryByAttribute query = new QueryByAttribute(ipg_globalsetting.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(ipg_globalsetting.ipg_value).ToLower())
            };
            query.AddAttributeValue(nameof(ipg_globalsetting.ipg_name).ToLower(), key);
            EntityCollection ec = crmService.RetrieveMultiple(query);
            if (ec.Entities.Count == 1)
            {
                return ec.Entities[0].GetAttributeValue<string>(nameof(ipg_globalsetting.ipg_value).ToLower());
            }
            return string.Empty;
        }

        public ipg_caseworkflowtask GetCaseWorkflowTask(ipg_gateconfigurationdetail gateConfigurationDetail, string caseNumber)
        {
            if (gateConfigurationDetail == null || gateConfigurationDetail.ipg_WorkflowTaskId == null)
            {
                return null;
            }
            var _crmContext = new OrganizationServiceContext(crmService);
            return (from caseWorkflowTask in _crmContext.CreateQuery<ipg_caseworkflowtask>()
                    join workflowtask in _crmContext.CreateQuery<ipg_workflowtask>() on caseWorkflowTask.ipg_WorkflowTaskId.Id equals workflowtask.ipg_workflowtaskId
                    where workflowtask.ipg_workflowtaskId == gateConfigurationDetail.ipg_WorkflowTaskId.Id
                    && caseWorkflowTask.ipg_CaseNumber == caseNumber
                    select caseWorkflowTask).FirstOrDefault();
        }

        public ipg_caseworkflowtask GetCaseWorkflowTask(EntityReference workflowTaskId, string caseNumber)
        {
            if (workflowTaskId == null)
            {
                return null;
            }
            var _crmContext = new OrganizationServiceContext(crmService);
            return (from caseWorkflowTask in _crmContext.CreateQuery<ipg_caseworkflowtask>()
                    join workflowtask in _crmContext.CreateQuery<ipg_workflowtask>() on caseWorkflowTask.ipg_WorkflowTaskId.Id equals workflowtask.ipg_workflowtaskId
                    where workflowtask.ipg_workflowtaskId == workflowTaskId.Id
                    && caseWorkflowTask.ipg_CaseNumber == caseNumber
                    select caseWorkflowTask).FirstOrDefault();
        }

        public string GetCaseNumber(Entity entity)
        {
            if (string.Equals(entity.LogicalName, Incident.EntityLogicalName))
            {
                if (entity.Contains(nameof(Incident.Title).ToLower()))
                {
                    return (string)entity[nameof(Incident.Title).ToLower()];
                }
            }
            else if (string.Equals(entity.LogicalName, ipg_referral.EntityLogicalName))
            {
                if (entity.Contains(nameof(ipg_referral.ipg_referralnumber).ToLower()))
                {
                    return (string)entity[nameof(ipg_referral.ipg_referralnumber).ToLower()];
                }
                else if (entity.Contains(nameof(ipg_referral.ipg_referralcasenumber).ToLower()))
                {
                    return (string)entity[nameof(ipg_referral.ipg_referralcasenumber).ToLower()];
                }
            }
            return string.Empty;
        }

    }
}