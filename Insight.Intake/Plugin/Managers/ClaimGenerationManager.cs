using Insight.Intake.Extensions;
using Insight.Intake.Helpers;
using Insight.Intake.Models;
using Insight.Intake.Repositories;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Insight.Intake.Plugin.Managers
{

    public class ClaimGenerationManager
    {
        private readonly string _adminHcpcs = "A9900";
        private readonly string _cms1500templateSettingName = "CMS1500_PDF_TEMPLATE";
        private readonly string _ub04templateSettingName = "UB04_PDF_TEMPLATE";
        private readonly string _claimDocumentTypeNameUB04 = "Claim UB04";
        private readonly string _claimDocumentTypeNameCMS1500 = "Claim CMS1500";
        //claim config
        private const string _UB04ReplacementTypeOfBillCode = "0837";
        private const string _UB04NewTypeOfBillCode = "0831";
        private readonly string TaskReasonIdPendingFacilityCarrierContractSettingName = "TASK_REASON_ID_PENDING_FACILITY_CARRIER_CONTRACT";

        private Guid _initiatingUser;
        private readonly IOrganizationService _crmService;
        private readonly ITracingService _tracingService;
        private readonly ipg_IPGCaseActionsCreateClaimRequest _request;
        private readonly ClaimPDFConfigurationRepository _configRepo;
        private Incident _incident;
        private Intake.Account _carrier;
        private Intake.Contact _physician;
        private Intake.Contact Physician { get
            {
                if (_physician != null)
                    return _physician;
                else
                {
                    _physician = _incident.ipg_PhysicianId != null ?
                        _crmService.Retrieve(_incident.ipg_PhysicianId.LogicalName, _incident.ipg_PhysicianId.Id, new ColumnSet(true)).ToEntity<Intake.Contact>() : null;
                    return _physician;
                }
            } 
        }

        private Intake.Account _facility;
        private Intake.Account Facility
        {
            get
            {
                if (_facility != null)
                    return _facility;
                else
                {
                    _facility = _incident.ipg_FacilityId != null ?
                        _crmService.Retrieve(_incident.ipg_FacilityId.LogicalName, _incident.ipg_FacilityId.Id, new ColumnSet(true)).ToEntity<Intake.Account>() : null;
                    return _facility;
                }
            }
        }

        private ipg_carrierclaimsmailingaddress _claimMailingAdress;
        private bool _isAutoCarrierGeneration;
        private List<ipg_casepartdetail> _casepartdetails = null;
        private List<Invoice> _existingClaims = null;
        private TaskManager _taskManager;
        private bool _isPrimaryCarrierPosition;
        private List<ipg_casepartdetail> CasePartDetails
        {
            get
            {
                _casepartdetails = _casepartdetails ?? GetActualPartsForCase();
                return _casepartdetails;
            }
        }

        private List<Invoice> ExistingClaims
        {
            get
            {
                _existingClaims = _existingClaims ?? GetExistingClaims();
                return _existingClaims;
            }
        }

        private Invoice LastPrimaryClaim
        {
            get
            {
                return ExistingClaims.Where(i => !string.IsNullOrWhiteSpace(i.Name) ?
                i.Name.Substring(i.Name.Length - 2, 1) == "1" : false).FirstOrDefault();
            }
        }
        private Invoice LastSecondaryClaim
        {
            get
            {
                return ExistingClaims.Where(i => !string.IsNullOrWhiteSpace(i.Name) ? 
                i.Name.Substring(i.Name.Length - 2, 1) ==  "2" : false).FirstOrDefault();
            }
        }


        public ClaimGenerationManager(IOrganizationService crmService, ITracingService tracingService, ipg_IPGCaseActionsCreateClaimRequest request, Guid initiatingUser)
        {
            _crmService = crmService;
            _tracingService = tracingService;
            _configRepo = new ClaimPDFConfigurationRepository(_crmService, _tracingService);
            _request = request;
            _initiatingUser = initiatingUser;
            _taskManager = new TaskManager(_crmService, _tracingService, null, _initiatingUser);

            _tracingService.Trace($"{nameof(ClaimGenerationManager)} created");
        }

        public void ProcessGeneration(ParameterCollection OutputParameters)
        {
            _tracingService.Trace("Retrieving Case");
            _incident = _crmService.Retrieve(_request.Target.LogicalName, _request.Target.Id, new ColumnSet(true)).ToEntity<Incident>();

            _isAutoCarrierGeneration = DetermineIsAutoCarrierGeneration();

            var validationResult = ValidateCase(out bool skipClaimGeneration, out Guid? skipTaskReasonId);

            OutputParameters["HasErrors"] = false;
            OutputParameters["Message"] = string.Empty;
            OutputParameters["SkipClaimGeneration"] = false;
            OutputParameters["PdfFileBase64"] = string.Empty;


            if (skipClaimGeneration == true)
            {
                _tracingService.Trace("Invoice Skipped");
                OutputParameters["HasErrors"] = false;
                OutputParameters["Message"] = string.Empty;
                OutputParameters["SkipClaimGeneration"] = true;
                OutputParameters["SkipTaskReasonId"] = skipTaskReasonId;
            }
            else if (validationResult.Count > 0)
            {
                _tracingService.Trace("There is Validation Errors");
                OutputParameters["HasErrors"] = true;
                OutputParameters["Message"] = string.Join(Environment.NewLine, validationResult);
            }
            else if (_request.GenerateClaimFlag || _request.GeneratePdfFlag)
            {
                try
                {
                    GenerateClaim(OutputParameters);

                    if (_request.GenerateClaimFlag && !_request.ManualClaim && (OutputParameters["HasErrors"] as bool?) != true)
                    {
                        var response = (ipg_IPGGatingStartGateProcessingResponse)_crmService.Execute(new ipg_IPGGatingStartGateProcessingRequest() { Target = _incident.ToEntityReference() });

                        if (!response.Succeeded)
                        {
                            throw new Exception($"Claim generated but Gating failfed. Gating Output: {response.Output}");
                        }
                    }
                }
                catch(Exception e)
                {
                    _tracingService.Trace($"There is error: {e.Message}");
                    _tracingService.Trace($"Tracce Log: {e.StackTrace}");

                    OutputParameters["HasErrors"] = true;
                    OutputParameters["Message"] = $"Internal error. Please Contact System Administrator! Error:{e.Message}";
                }
            }

            if ((OutputParameters["SkipClaimGeneration"] as bool?) == true || (OutputParameters["HasErrors"] as bool?) == true
                || string.IsNullOrEmpty(OutputParameters["PdfFileBase64"] as string))
            {
                CreateClaimGenerationErrorTask(OutputParameters["Message"] as string);
            }
        }

        private void CreateClaimGenerationErrorTask(string errors)
        {
            _tracingService.Trace("Claim not generated create ClaimGenerationErrorTask");
            var tasktype = _taskManager.GetTaskTypeById(TaskManager.TaskTypeIds.Claim_Generation_Errors);
            var description = tasktype.ipg_description?.Replace("{0}", _carrier?.Name ?? "") ?? "";
            
            if(!string.IsNullOrEmpty(errors))
            {
                description += Environment.NewLine + errors;
            }
            
            _taskManager.CreateTask(_incident.ToEntityReference(), tasktype.ToEntityReference(), new Task() { Description = description });
        }

        private bool DetermineIsAutoCarrierGeneration()
        {
            _tracingService.Trace("Determain if autocarrier Claim Generation");
            
            if (_request.ManualClaim == false)
            {
                _tracingService.Trace("Claim Generation executed by scheduled task");

                var carrierType = _incident.ipg_SecondaryCarrierId != null ? _crmService.Retrieve(_incident.ipg_SecondaryCarrierId.LogicalName, _incident.ipg_SecondaryCarrierId.Id, new ColumnSet(Intake.Account.Fields.ipg_CarrierType)).ToEntity<Intake.Account>().ipg_CarrierTypeEnum : null;

                if (carrierType == ipg_CarrierType.Auto)
                {
                    _tracingService.Trace($"Case {_incident.Title} have autocarrier {_incident.ipg_SecondaryCarrierId.Id}");

                    if (ExistingClaims.Any())
                    {
                        _tracingService.Trace($"Case {_incident.Title} already have claim");
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private List<string> ValidateCase(out bool skipClaimGeneration, out Guid? skipTaskReasonId)
        {
            var validationErrors = new List<string>();
            skipClaimGeneration = false;
            skipTaskReasonId = null;

            _isPrimaryCarrierPosition = _request.IsPrimaryOrSecondaryClaim && !_isAutoCarrierGeneration;

            var isCaseOnHold = _incident.StatusCode.Value == (int)Incident_StatusCode.OnHold;
            if (isCaseOnHold == true)
            {
                skipClaimGeneration = true;
                return validationErrors;
            }
            
            var carrierReference = _isPrimaryCarrierPosition ? _incident.ipg_CarrierId : _incident.ipg_SecondaryCarrierId;
            _carrier = carrierReference != null ?_crmService.Retrieve(carrierReference.LogicalName, carrierReference.Id, new ColumnSet(true)).ToEntity<Intake.Account>() : null;

            bool isFacilityCarrierContractPending = false; 
            if(_request.ManualClaim == false && carrierReference != null)
            {
                isFacilityCarrierContractPending = ValidateFacilityCarrierEntitlement(carrierReference);
            }
            
            if(isFacilityCarrierContractPending)
            {
                skipClaimGeneration = true;

                string taskReasonIdString = D365Helpers.GetGlobalSettingValueByKey(_crmService, TaskReasonIdPendingFacilityCarrierContractSettingName);
                if(Guid.TryParse(taskReasonIdString, out Guid taskReasonId) == false)
                {
                    throw new Exception("Invalid global setting " + TaskReasonIdPendingFacilityCarrierContractSettingName);
                }
                skipTaskReasonId = taskReasonId;

                validationErrors.Add($"Facility-Carrier entitlement is pending.");

                return validationErrors;
            }
            

            //6. Carrier does not have generation of the CMS1500 / UB04 enabled
            //7. Carrier Unidentified
            //14. Missing carrier claim name
            if (carrierReference != null)
            {
                if (!_carrier.ipg_ClaimsServicingProviderNameEnum.HasValue)
                {
                    validationErrors.Add($"- Carrier {_carrier.Name} has no Servicing Provider Name defined.");
                }

                if (!_carrier.ipg_ClaimsServicingProviderAddressEnum.HasValue)
                {
                    validationErrors.Add($"- Carrier {_carrier.Name} has no Servicing Provider Address Type defined.");
                }

                var claimMailingAddressRef = _isPrimaryCarrierPosition ? _incident.ipg_PrimaryCarrierClaimsMailingAddress : _incident.ipg_SecondaryCarrierClaimsMailingAddress;
                 _claimMailingAdress = claimMailingAddressRef != null ? _crmService.Retrieve(claimMailingAddressRef.LogicalName, claimMailingAddressRef.Id, new ColumnSet(true)).ToEntity<ipg_carrierclaimsmailingaddress>() : null;
                var claimMailingAddressEmpty = string.IsNullOrWhiteSpace(_claimMailingAdress?.ipg_claimsmailingaddress)
                    || string.IsNullOrWhiteSpace(_claimMailingAdress?.ipg_claimsmailingstate)
                    || string.IsNullOrWhiteSpace(_claimMailingAdress?.ipg_claimsmailingcity)
                    || string.IsNullOrWhiteSpace(_claimMailingAdress?.ipg_carrierclaimname)
                    || _claimMailingAdress?.ipg_ClaimsMailingZipCodeIdId == null;

                if(claimMailingAddressEmpty)
                {
                    validationErrors.Add($"- Carrier {_carrier.Name} has no Claims Mailing Address on Case.");
                }
            }

            //13. Secondary Insured Last Name, Secondary Insured First Name, Secondary Insured DOB, Secondary Insured Gender, Secondary Policy ID and Secondary Carrier are all required if Secondary Insurance Plan is checked \"Yes\"
            var benefitPlan = _incident.ipg_benefitplan;
            if (benefitPlan != null)
            {
                if (benefitPlan.Value == true)
                {
                    var secondaryInsuredLastName = _incident.ipg_secondaryinsuredlastname;
                    var secondaryInsuredFirstName = _incident.ipg_secondaryinsuredfirstname;
                    var secondaryInsuredGender = _incident.ipg_secondaryinsuredgender;
                    var secondaryInsuredDob = _incident.ipg_secondaryinsureddateofbirth;
                    var policyId2 = _incident.ipg_policyid2;
                    var secondaryCarrierId = _incident.ipg_SecondaryCarrierId;
                    if (string.IsNullOrWhiteSpace(secondaryInsuredFirstName) || string.IsNullOrWhiteSpace(secondaryInsuredLastName) || string.IsNullOrWhiteSpace(policyId2) || secondaryInsuredDob == null || secondaryInsuredGender == null || secondaryCarrierId == null)
                    {
                        validationErrors.Add("- Secondary Insured Last Name, Secondary Insured First Name, Secondary Insured DOB, Secondary Insured Gender, Secondary Policy ID and Secondary Carrier are all required if Secondary Insurance Plan is checked 'Yes'");
                    }
                }
            }

            Invoice lastclaim = _isAutoCarrierGeneration || !_request.IsPrimaryOrSecondaryClaim ? LastSecondaryClaim : LastPrimaryClaim;

            if (lastclaim != null)
            {
                short finalDigits;

                Int16.TryParse(lastclaim.Name.Substring(lastclaim.Name.Length - 1), out finalDigits);

                if (finalDigits == 9)
                {
                    validationErrors.Add($"There is already 9 {(_isAutoCarrierGeneration || _request.IsPrimaryOrSecondaryClaim ? "Primary" : "Secondary")} Claims!");
                }
            }

            var relationToInsured = _incident.ipg_RelationToInsured;
            if (relationToInsured == null)
            {
                validationErrors.Add("- Relation to Insurred must be selected");
            }

            if (!_request.IsPrimaryOrSecondaryClaim)
            {
                if (_incident.ipg_CaseBalance != null && _incident.ipg_CaseBalance.Value == 0)
                {
                    validationErrors.Add("- Secondary Claim can't be generated because Case Balance is 0");
                }
                
                var submittedPrimaryCarrier = ExistingClaims.Where(i => i.ipg_isprimaryorsecondaryclaim == true && i.ipg_StatusEnum == ipg_ClaimStatus.Submitted).FirstOrDefault();

                if (!_isAutoCarrierGeneration && submittedPrimaryCarrier == null)
                {
                    validationErrors.Add("- Secondary Claim can't be generated until response for Primary Claim is recieved");
                }
            }

            return validationErrors;
        }

        private bool ValidateFacilityCarrierEntitlement(EntityReference carrierReference)
        {
            var query = new QueryExpression(Entitlement.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(Entitlement.Fields.Id, Entitlement.Fields.Name),
                TopCount = 1,
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression(Entitlement.Fields.StateCode, ConditionOperator.NotEqual, (int)EntitlementState.Cancelled),
                        new ConditionExpression(Entitlement.Fields.ipg_contract_status, ConditionOperator.Equal, (int)Entitlement_ipg_contract_status.Pending),
                        new ConditionExpression(Entitlement.Fields.ipg_EntitlementType, ConditionOperator.Equal, (int)ipg_EntitlementTypes.FacilityCarrier),
                        new ConditionExpression(Entitlement.Fields.ipg_FacilityId, ConditionOperator.Equal, _incident.ipg_FacilityId.Id),
                        new ConditionExpression(Entitlement.Fields.ipg_CarrierId, ConditionOperator.Equal, carrierReference.Id),
                        new ConditionExpression(Entitlement.Fields.StartDate, ConditionOperator.LessEqual, _incident.ipg_ActualDOS),
                        new ConditionExpression(Entitlement.Fields.EndDate, ConditionOperator.GreaterEqual, _incident.ipg_ActualDOS),
                    }
                }
            };

            var entitlements = _crmService.RetrieveMultiple(query);
            return entitlements.Entities.Any();
        }

        private List<ipg_casepartdetail> GetActualPartsForCase()
        {
            var fetchData = new
            {
                statecode = "0",
                ipg_quantity = "0",
                ipg_unitprice = "0",
                incidentid = _incident.IncidentId,
                carriertypecode = "923720001", //Carrier
                manufacturertypecode = "923720002", //Manufacturer
                whichCarrier = _request.IsPrimaryOrSecondaryClaim ? "ipg_carrierid" : "ipg_secondarycarrierid"
            };

            var fetchXml = $@"
            <fetch>
              <entity name='ipg_casepartdetail'>
                <attribute name='ipg_multiplier' />
                <attribute name='ipg_costprice' />
                <attribute name='ipg_quantity' />
                <attribute name='ipg_unitprice' />
                <attribute name='ipg_billedchg' />
                <attribute name='ipg_extprice' />
                <attribute name='ipg_manufacturerpartnumber' />
                <attribute name='ipg_casepartdetailid' />
                <attribute name='ipg_uomid' />
                <attribute name='ipg_serialnumber' />
                <attribute name='ipg_lotnumber' />
                <attribute name='ipg_autobilledcharges' />
                <filter type='and'>
                  <condition attribute='statecode' operator='eq' value='{fetchData.statecode}'/>
                  <condition attribute='ipg_quantity' operator='gt' value='{fetchData.ipg_quantity}'/>
                  <condition attribute='ipg_unitprice' operator='gt' value='{fetchData.ipg_unitprice}'/>
                </filter>
                <link-entity name='incident' from='incidentid' to='ipg_caseid' link-type='outer' alias='Case'>
                  <filter type='and'>
                    <condition attribute='incidentid' operator='eq' value='{fetchData.incidentid}'/>
                  </filter>
                  <link-entity name='account' from='accountid' to='{fetchData.whichCarrier}' link-type='inner' alias='Carrier'>
                    <filter type='and'>
                      <condition attribute='customertypecode' operator='eq' value='{fetchData.carriertypecode}'/>
                    </filter>
                  </link-entity>
                </link-entity>
                <link-entity name='ipg_masterhcpcs' from='ipg_masterhcpcsid' to='ipg_hcpcscode' link-type='inner' alias='Hcpcs'>
                  <attribute name='ipg_masterhcpcsid' />
                  <attribute name='ipg_name' />
                  <attribute name='ipg_description' />
                </link-entity>
                <link-entity name='ipg_masterhcpcs' from='ipg_masterhcpcsid' to='ipg_hcpcscode2' link-type='outer' alias='Hcpcs2'>
                  <attribute name='ipg_masterhcpcsid' />
                  <attribute name='ipg_name' />
                  <attribute name='ipg_description' />
                </link-entity>
                <link-entity name='account' from='accountid' to='ipg_manufacturerid' link-type='outer' alias='Manufacturer'>
                  <attribute name='name' />
                  <attribute name='accountid' />
                  <filter type='and'>
                    <condition attribute='customertypecode' operator='eq' value='{fetchData.manufacturertypecode}'/>
                  </filter>
                </link-entity>
                <link-entity name='product' from='productid' to='ipg_productid' link-type='inner' alias='Product'>
                  <attribute name='name' />
                  <attribute name='defaultuomid' />
                  <attribute name='productid' />
                </link-entity>
              </entity>
            </fetch>";

            _tracingService.Trace($"ActualParts FetchXml: {fetchXml}");

            var result = _crmService.RetrieveMultiple(new FetchExpression(fetchXml));

            _tracingService.Trace($"ActualParts FetchXmlResultsCount: {result?.Entities.Count}");

            var records = result?.Entities.Select(entity => {
                var r = entity.ToEntity<ipg_casepartdetail>();

                r.ipg_billedchg = _isAutoCarrierGeneration ? r.ipg_autobilledcharges : r.ipg_billedchg;

                if (entity.Contains("Product.productid"))
                {
                    r.ipg_product_ipg_casepartdetail_productid = new Intake.Product()
                    {
                        Id = entity.GetAttributeValue<AliasedValue>("Product.productid")?.Value as Guid? ?? Guid.Empty,
                        Name = entity.GetAttributeValue<AliasedValue>("Product.name")?.Value?.ToString() ?? "",
                        DefaultUoMId = entity.GetAttributeValue<AliasedValue>("Product.defaultuomid")?.Value as EntityReference
                    };

                    r.ipg_productid = r.ipg_product_ipg_casepartdetail_productid.ToEntityReference();
                    r.ipg_uomid = r.ipg_uomid ?? r.ipg_product_ipg_casepartdetail_productid.DefaultUoMId;
                }


                if (entity.Contains("Hcpcs.ipg_masterhcpcsid"))
                {
                    r.ipg_ipg_masterhcpcs_ipg_casepartdetail_hcpcscode = new ipg_masterhcpcs()
                    {
                        Id = entity.GetAttributeValue<AliasedValue>("Hcpcs.ipg_masterhcpcsid")?.Value as Guid? ?? Guid.Empty,
                        ipg_name = entity.GetAttributeValue<AliasedValue>("Hcpcs.ipg_name")?.Value?.ToString() ?? "",
                        ipg_Description = entity.GetAttributeValue<AliasedValue>("Hcpcs.ipg_description")?.Value?.ToString() ?? "",
                    };

                    r.ipg_hcpcscode = r.ipg_ipg_masterhcpcs_ipg_casepartdetail_hcpcscode.ToEntityReference();
                }

                if (entity.Contains("Hcpcs2.ipg_masterhcpcsid"))
                {
                    r.ipg_ipg_masterhcpcs_ipg_casepartdetail_hcpcscode2 = new ipg_masterhcpcs()
                    {
                        Id = entity.GetAttributeValue<AliasedValue>("Hcpcs2.ipg_masterhcpcsid")?.Value as Guid? ?? Guid.Empty,
                        ipg_name = entity.GetAttributeValue<AliasedValue>("Hcpcs2.ipg_name")?.Value?.ToString() ?? "",
                        ipg_Description = entity.GetAttributeValue<AliasedValue>("Hcpcs2.ipg_description")?.Value?.ToString() ?? "",
                    };

                    r.ipg_hcpcscode2 = r.ipg_ipg_masterhcpcs_ipg_casepartdetail_hcpcscode2.ToEntityReference();
                }

                if (entity.Contains("Manufacturer.accountid"))
                {
                    r.ipg_account_ipg_casepartdetail_mManufacturerid = new Intake.Account()
                    {
                        Id = entity.GetAttributeValue<AliasedValue>("Manufacturer.accountid")?.Value as Guid? ?? Guid.Empty,
                        Name = entity.GetAttributeValue<AliasedValue>("Manufacturer.name")?.Value?.ToString() ?? "",
                    };

                    r.ipg_manufacturerid = r.ipg_account_ipg_casepartdetail_mManufacturerid.ToEntityReference();
                }

                return r;
            }).ToList() ?? new List<ipg_casepartdetail>();

            return records;
        }

        private List<Invoice> GetExistingClaims()
        {
            if (_incident == null)
            {
                throw new Exception("Retrieve Case before Invoices!");
            }


            var query = new QueryExpression(Invoice.EntityLogicalName)
            {
                Criteria = new FilterExpression(LogicalOperator.And),
                ColumnSet = new ColumnSet(true)
            };

            query.Criteria.AddCondition(Invoice.Fields.ipg_caseid, ConditionOperator.Equal, _incident.Id);
            query.AddOrder(Invoice.Fields.CreatedOn, OrderType.Descending);

            return _crmService.RetrieveMultiple(query)?.Entities.Cast<Invoice>().ToList() ?? new List<Invoice>();
        }

        private void GenerateClaim(ParameterCollection OutputParameters)
        {
            Guid newClaimId = Guid.Empty;

            var enableEDI = !string.IsNullOrEmpty(_claimMailingAdress.ipg_electronicpayerid);

            var claimType = _carrier.ipg_ClaimTypeEnum;

            var claimStatuses = GetTempNewClosedClaimStatuses();
            var tempNewClaimStatusId = claimStatuses.FirstOrDefault(s => string.Equals(s.ipg_name.Trim().ToLower(), "temp new claim status")).Id;
            var tempClosedClaimStatusId = claimStatuses.FirstOrDefault(s => string.Equals(s.ipg_name.Trim().ToLower(), "temp closed claim status")).Id;

            var caseTitle = _incident.Title;
            var caseId = _incident.Id;

            //get existing claims and actual parts for a case
            var existingClaims = ExistingClaims;
            var actualParts = CasePartDetails;

            _tracingService.Trace($"Actual Parts Count: {actualParts.Count}");

            _tracingService.Trace("Determine claim number based on existing claims");
            var tempClaimNumber = BuildClaimNumber();

            bool consolidateCodes = _carrier.ipg_ConsolidateLikeCodesonClaim ?? false;
            bool useAlternateHcpcs = _carrier.ipg_UseAlternateHCPCS ?? false;
            bool useSecondaryHcpcs = _carrier.ipg_usesecondaryhcpcs ?? false;
            var whichHcpcs = (useAlternateHcpcs || useSecondaryHcpcs) ? "Hcpcs2" : "Hcpcs";
            var isAdmin = false;
            decimal maxBilled = 0;
            Dictionary<string, object> maxBilledHcpcs = null;
            decimal adminQuantity = 0;
            decimal adminAmount = 0;
            int index = 0;
            Dictionary<string, Dictionary<string, object>> lineItems = new Dictionary<string, Dictionary<string, object>>();
            Dictionary<string, Dictionary<string, object>> lineItemsDetails = new Dictionary<string, Dictionary<string, object>>();
            decimal totalClaimTotal = 0;
            string[] consolidateCodesCarrierExceptions = { _crmService.GetGlobalSettingValueByKey("BCBSNC-CONTRACTED"), _crmService.GetGlobalSettingValueByKey("BCBS OF TX") };

            _tracingService.Trace("Building Parts Records");
            foreach (var casePart in actualParts)
            {
                var hcpcsName = whichHcpcs.Equals("Hcpcs") ? casePart.ipg_ipg_masterhcpcs_ipg_casepartdetail_hcpcscode?.ipg_name : casePart.ipg_ipg_masterhcpcs_ipg_casepartdetail_hcpcscode2?.ipg_name;
                var hcpcsDescription = whichHcpcs.Equals("Hcpcs") ? casePart.ipg_ipg_masterhcpcs_ipg_casepartdetail_hcpcscode?.ipg_Description : casePart.ipg_ipg_masterhcpcs_ipg_casepartdetail_hcpcscode2?.ipg_Description;
                var hcpcsId = whichHcpcs.Equals("Hcpcs") ? casePart.ipg_hcpcscode : casePart.ipg_hcpcscode2;

                if (string.IsNullOrWhiteSpace(hcpcsName))
                {
                    throw new Exception($"Case Part {casePart.ipg_product_ipg_casepartdetail_productid.Name} on case {caseTitle} must have {whichHcpcs} selected.");
                }

                //Post-Claim generation HCPCS updates (CPI-8758)
                if (_carrier.ipg_ClaimType.Value == (int)ipg_claim_type.UB04)
                {
                    if (string.Equals(_carrier.Id.ToString(), _crmService.GetGlobalSettingValueByKey("BCBS-ANTHEM-CT"), StringComparison.OrdinalIgnoreCase))
                    {
                        hcpcsName = "L8699";
                        hcpcsDescription = "PROSTHETIC IMPLANT, NOT OTHERWISE SPECIFIED";
                    }
                    else if (string.Equals(_carrier.Id.ToString(), _crmService.GetGlobalSettingValueByKey("BCBSNC-CONTRACTED"), StringComparison.OrdinalIgnoreCase))
                    {
                        if (hcpcsName.ToUpper().Equals("L8699") || hcpcsName.ToUpper().Equals("A4649"))
                        {
                            hcpcsDescription = casePart.ipg_product_ipg_casepartdetail_productid?.Name;
                        }
                        else if (hcpcsName.ToUpper().Equals("C9359"))
                        {
                            hcpcsName = "C1713";
                            hcpcsDescription = "ANCHOR/SCREW FOR OPPOSING BONE-TO-BONE OR SOFT TISSUE-TO-BONE (IMPLANTABLE)";
                        }
                    }
                    else if (string.Equals(_carrier.Id.ToString(), _crmService.GetGlobalSettingValueByKey("BCBS OF TX"), StringComparison.OrdinalIgnoreCase))
                    {
                        if (hcpcsName.ToUpper().Equals("L8699") || hcpcsName.ToUpper().Equals("Q1400"))
                        {
                            hcpcsDescription = casePart.ipg_product_ipg_casepartdetail_productid?.Name;
                        }
                    }
                }

                if (hcpcsName == _adminHcpcs)
                {
                    isAdmin = true;
                    adminQuantity = casePart.ipg_quantity ?? 0;
                    adminAmount = casePart.ipg_billedchg?.Value ?? 0;
                }
                else
                {
                    if (consolidateCodes && _carrier.ipg_ClaimTypeEnum == ipg_claim_type.UB04 && !consolidateCodesCarrierExceptions.Contains(_carrier.Id.ToString()))
                    {
                        if (lineItems.ContainsKey(hcpcsName))
                        {
                            lineItems[hcpcsName]["Quantity"] = (decimal)lineItems[hcpcsName]["Quantity"] + casePart.ipg_quantity ?? 0;
                            lineItems[hcpcsName]["BilledChg"] = (decimal)lineItems[hcpcsName]["BilledChg"] + casePart.ipg_billedchg?.Value ?? 0;
                            lineItems[hcpcsName]["UnitPrice"] = (decimal)lineItems[hcpcsName]["UnitPrice"] + casePart.ipg_unitprice?.Value ?? 0;
                            lineItems[hcpcsName]["ExpectedReimbursement"] = (decimal)lineItems[hcpcsName]["ExpectedReimbursement"] + casePart.ipg_extprice?.Value ?? 0;
                        }
                        else
                        {
                            lineItems.Add(hcpcsName, new Dictionary<string, object>
                            {
                                { "Quantity", casePart.ipg_quantity ?? 0 },
                                { "BilledChg", casePart.ipg_billedchg?.Value ?? 0 },
                                { "UnitPrice", casePart.ipg_unitprice?.Value ?? 0 },
                                { "ExpectedReimbursement", casePart.ipg_extprice?.Value ?? 0},
                                { "HcpcsId", hcpcsId },
                                { "HcpcsName", hcpcsName },
                                { "HcpcsDescription", hcpcsDescription }
                            });
                        }

                        if ((casePart.ipg_billedchg?.Value ?? 0) > maxBilled)
                        {
                            maxBilledHcpcs = lineItems[hcpcsName];
                            maxBilled = casePart.ipg_billedchg.Value;
                        }

                    }
                    else
                    {
                        lineItems.Add(index.ToString(), new Dictionary<string, object>
                        {
                            { "Quantity", casePart.ipg_quantity ?? 0 },
                            { "BilledChg", casePart.ipg_billedchg?.Value ?? 0 },
                            { "UnitPrice", casePart.ipg_unitprice?.Value ?? 0 },
                            { "ExpectedReimbursement",  casePart.ipg_extprice?.Value ?? 0 },
                            { "HcpcsId", hcpcsId },
                            { "HcpcsName", hcpcsName },
                            { "PartId", casePart.ipg_productid },
                            { "UoMId", casePart.ipg_uomid },
                            { "HcpcsDescription", hcpcsDescription }
                        });

                        if ((casePart.ipg_billedchg?.Value ?? 0) > maxBilled)
                        {
                            maxBilledHcpcs = lineItems[index.ToString()];
                            maxBilled = casePart.ipg_billedchg.Value;
                        }
                    }

                    totalClaimTotal += casePart.ipg_billedchg?.Value ?? 0;

                    if (!string.Equals(hcpcsName, _adminHcpcs))
                    {
                        lineItemsDetails.Add(index.ToString(), new Dictionary<string, object> {
                            { "Quantity", casePart.ipg_quantity ?? 0},
                            { "BilledChg", casePart.ipg_billedchg?.Value ?? 0 },
                            { "UnitPrice", casePart.ipg_unitprice?.Value ?? 0 },
                            { "ExpectedReimbursement", casePart.ipg_extprice?.Value ?? 0 },
                            { "HcpcsId", hcpcsId },
                            { "HcpcsName", hcpcsName },
                            { "PartId", casePart.ipg_productid},
                            { "UoMId", casePart.ipg_uomid },
                            { "PartNumber", casePart.ipg_manufacturerpartnumber },
                            { "PartName", casePart.ipg_product_ipg_casepartdetail_productid?.Name} ,
                            { "ManufacturerId", casePart.ipg_manufacturerid },
                            { "ManufacturerName", casePart.ipg_account_ipg_casepartdetail_mManufacturerid?.Name },
                            { "CostPrice", casePart.ipg_costprice?.Value ?? 0 }
                        });
                    }
                }
                index++;
            }

            if (isAdmin)
            {
                _tracingService.Trace("There is Admin Actual Part!");

                maxBilledHcpcs["BilledChg"] = (decimal)maxBilledHcpcs["BilledChg"] + adminAmount;
                maxBilledHcpcs["UnitPrice"] = (decimal)maxBilledHcpcs["UnitPrice"] + adminAmount;
                maxBilledHcpcs["ExpectedReimbursement"] = (decimal)maxBilledHcpcs["ExpectedReimbursement"] + adminAmount;

                if (consolidateCodes)
                {
                    var adminFee = GetAdminFeeProduct();
                    lineItemsDetails.Add(index.ToString(), new Dictionary<string, object> {
                        { "Quantity", (decimal)adminFee.ipg_quantity },
                        { "BilledChg", adminFee.Price.Value },
                        { "HcpcsId", maxBilledHcpcs["HcpcsId"] },
                        { "HcpcsName", maxBilledHcpcs["HcpcsName"] },
                        { "PartId", new EntityReference("product", adminFee.Id) },
                        { "UoMId", adminFee.DefaultUoMId },
                        { "PartNumber", adminFee.ProductNumber },
                        { "PartName", adminFee.Name }
                    });
                }
            }

            _tracingService.Trace("Build parts Records");

            if (_request.GenerateClaimFlag)
            {
                //update latest claim with replacing claim icn
                if (_request.IsReplacementClaim) //replacing Claim
                {
                    UpdateLatestSubmittedClaimICN(claimType, _request.Icn);

                    _tracingService.Trace("Icn Updated");
                }


                newClaimId = CreateClaim(tempClaimNumber, tempNewClaimStatusId, totalClaimTotal, claimType, _carrier.ToEntityReference(), out ipg_ClaimReason? claimReason);
                _tracingService.Trace("Claim Generated");

                UpdateCase(claimType);
                _tracingService.Trace("Case Updated");

                //update all existing active claims to inactive, and move them to history
                SetExistingClaimsInactiveAndMoveToHistory(existingClaims, newClaimId, tempClosedClaimStatusId);
                _tracingService.Trace("Claims moved to History");

                CreateClaimLineItems(lineItems, lineItemsDetails, consolidateCodes, newClaimId);
                _tracingService.Trace("Claims lines created");


                if (claimReason != ipg_ClaimReason.SubmittedPaper)
                {
                    CalculateResponsibility(newClaimId);
                    _tracingService.Trace("CalculateResponsibility Done");
                }

                if (_incident.ipg_PostedToAccount != true)
                {
                    CreateGLTransactions();
                    _tracingService.Trace("GL Transaction Created");
                }

                if (enableEDI)
                {
                    OutputParameters["IsCarrierEdiEnabled"] = true;
                }
                else
                {
                    OutputParameters["IsCarrierEdiEnabled"] = false;
                }

                OutputParameters["GeneratedClaimType"] = _carrier.ipg_ClaimType.Value;
            }

            if (!enableEDI && !_request.ManualClaim || _request.ManualClaim && _request.GeneratePdfFlag)
            {
                string base64 = "";
                string filename = $"{tempClaimNumber}.pdf";
                string taskName = "";

                Invoice claim = null;
                if (newClaimId != Guid.Empty)
                {
                    claim = _crmService.Retrieve(Invoice.EntityLogicalName, newClaimId, new ColumnSet(true)).ToEntity<Invoice>();
                }
                else
                {
                    claim = _existingClaims.FirstOrDefault();
                }
                
                if (claimType == ipg_claim_type.CMS1500)
                {
                    base64 = GenerateCms1500Pdf(tempClaimNumber, consolidateCodes, lineItems, claim, actualParts, whichHcpcs);
                    filename = "CMS1500-" + filename;
                    taskName = "Task.InstitutionalClaimsReadyToPrint";
                }
                else if (claimType == ipg_claim_type.UB04)
                {
                    base64 = GenerateUb04Pdf(tempClaimNumber, consolidateCodes, lineItems, claim, actualParts);
                    filename = "UB04-" + filename;
                    taskName = "Task.ProfessionalClaimsReadyToPrint";
                }

                _tracingService.Trace("Check that pdf created");

                if (!string.IsNullOrEmpty(base64))
                {
                    _tracingService.Trace("createdocument");
                    var docRef = CreateCaseDocument(caseId, base64, filename, claimType);

                    //bind doc to Invoice
                    if (newClaimId != Guid.Empty)
                    {
                        _crmService.Update(new Invoice() { Id = newClaimId, ipg_documentid = docRef });
                    }

                    CreateUserTask(taskName);

                    OutputParameters["PdfFileBase64"] = base64;
                    if (newClaimId != null && newClaimId != Guid.Empty)
                    {
                        ImportantEventManager eventManager = new ImportantEventManager(_crmService);
                        Guid eventPerformedBy = Guid.Empty;

                        eventManager.CreateImportantEventLog(_incident, _initiatingUser, "ET24");
                        eventManager.SetCaseOrReferralPortalHeader(_incident, "ET24");
                    }
                }
            }
        }

        private List<ipg_claimstatuscode> GetTempNewClosedClaimStatuses()
        {
            var query = new QueryExpression(ipg_claimstatuscode.EntityLogicalName)
            {
                Criteria = new FilterExpression(LogicalOperator.And),
                ColumnSet = new ColumnSet("ipg_name", "ipg_claimstatuscodeid"),
                Distinct = true,
                TopCount = 2
            };

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)ipg_claimStatusCodeState.Active);

            var filter = new FilterExpression(LogicalOperator.Or);
            filter.AddCondition("ipg_name", ConditionOperator.Equal, "temp new claim status");
            filter.AddCondition("ipg_name", ConditionOperator.Equal, "temp closed claim status");
            query.Criteria.AddFilter(filter);

            var ec = _crmService.RetrieveMultiple(query);
            if (ec.Entities.Count != 2)
            {
                throw new Exception("Claim Status Code: 'temp new claim status' or 'temp closed claim status' doesn't exist.");
            }
            return ec.Entities.Cast<ipg_claimstatuscode>().ToList();
        }
        private Annotation GetPdfTemplateAnnotation(string claimType)
        {
            var name = claimType.Equals("CMS1500") ? _cms1500templateSettingName : _ub04templateSettingName;
            var query = new QueryExpression(ipg_globalsetting.EntityLogicalName)
            {
                ColumnSet = new ColumnSet("ipg_name", "ipg_globalsettingid"),
                Criteria = new FilterExpression(LogicalOperator.And)
            };

            query.Criteria.AddCondition("ipg_name", ConditionOperator.Equal, name);
            var settings = _crmService.RetrieveMultiple(query);

            if (settings.Entities.Count == 0)
            {
                throw new Exception($"Global Setting {_cms1500templateSettingName} not defined. Please enter setting and add note with Pdf template to it.");
            }
            var settingId = settings.Entities[0].ToEntity<ipg_globalsetting>().Id;

            query = new QueryExpression(Annotation.EntityLogicalName)
            {
                Criteria = new FilterExpression(LogicalOperator.And),
                ColumnSet = new ColumnSet("filename", "filesize", "objecttypecode", "objectid", "mimetype", "documentbody", "annotationid")
            };

            query.Criteria.AddCondition("objectid", ConditionOperator.Equal, settingId);

            var result = _crmService.RetrieveMultiple(query);
            if (result.Entities.Count == 0)
            {

                throw new Exception($"Note with Pdf template file doesn't exist: {name}");
            }
            return result.Entities[0].ToEntity<Annotation>();
        }

        private void SetExistingClaimsInactiveAndMoveToHistory(List<Invoice> existingClaims, Guid newClaimId, Guid tempClosedClaimStatusId, IOrganizationService service)
        {
            foreach (var claim in existingClaims)
            {
                //update custom status field
                var updateEntity = new Invoice()
                {
                    Id = claim.Id
                };
                updateEntity.ipg_claimstatusid = new EntityReference("ipg_claimstatuscode", tempClosedClaimStatusId);
                service.Update(updateEntity);

                //set oob status field to inactive/closed
                SetStateRequest request = new SetStateRequest
                {
                    EntityMoniker = new EntityReference("invoice", claim.Id),
                    State = new OptionSetValue(1),
                    Status = new OptionSetValue(3)
                };
                service.Execute(request);

                //insert record in history table
                var historyEntity = new ipg_claimshistory()
                {
                    Id = Guid.NewGuid(),
                    ipg_name = claim.GetAttributeValue<string>("name"),
                    ipg_claimid = new EntityReference("invoice", newClaimId),
                    ipg_claimstatusid = claim.ipg_claimstatusid,
                    ipg_createddate = claim.CreatedOn,
                    ipg_claimamount = claim.TotalAmount,
                    ipg_createdby = claim.CreatedBy,
                    ipg_caseid = claim.ipg_caseid,
                    ipg_description = claim.Description,
                    ipg_denialcategorycode = claim.ipg_denialcategorycode,
                    ipg_icn = claim.ipg_icn,
                    ipg_isprimaryodsecondaryclaim = claim.ipg_isprimaryorsecondaryclaim,
                    ipg_claimtypecode = claim.ipg_claimtypecode,
                    ipg_carrierid = claim.CustomerId

                };

                service.Create(historyEntity);
            }
        }
        private void CalculateResponsibility(Guid claimId)
        {
            float carrierConisurance = 100;
            float memberConisurance = 0;
            decimal unmetdeductible = 0;
            var caseManager = new CaseManager(_crmService, _tracingService, _incident.ToEntityReference());
            var benefit = caseManager.GetBenefit(_crmService);
            if (benefit != null)
            {
                carrierConisurance = (float)benefit.GetAttributeValue<double>(nameof(ipg_benefit.ipg_CarrierCoinsurance).ToLower());
                memberConisurance = (float)benefit.GetAttributeValue<double>(nameof(ipg_benefit.ipg_MemberCoinsurance).ToLower());
                decimal deductible = (benefit.GetAttributeValue<Money>(nameof(ipg_benefit.ipg_Deductible).ToLower()) == null ? 0 : benefit.GetAttributeValue<Money>(nameof(ipg_benefit.ipg_Deductible).ToLower()).Value);
                decimal deductibleMet = (benefit.GetAttributeValue<Money>(nameof(ipg_benefit.ipg_DeductibleMet).ToLower()) == null ? 0 : benefit.GetAttributeValue<Money>(nameof(ipg_benefit.ipg_DeductibleMet).ToLower()).Value);
                unmetdeductible = deductible - deductibleMet;
            }

            var queryExpression = new QueryExpression(ipg_claimlineitem.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(nameof(ipg_claimlineitem.ipg_ExpectedReimbursement).ToLower()),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(nameof(ipg_claimlineitem.ipg_claimid).ToLower(), ConditionOperator.Equal, claimId)
                        }
                }
            };
            EntityCollection claimLines = _crmService.RetrieveMultiple(queryExpression);
            decimal sumExpectedReimbursement = 0;
            foreach (Entity claimLine in claimLines.Entities)
                sumExpectedReimbursement += (claimLine.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_ExpectedReimbursement).ToLower()) == null ? 0 : claimLine.GetAttributeValue<Money>(nameof(ipg_claimlineitem.ipg_ExpectedReimbursement).ToLower()).Value);

            var caseEntity = new Incident()
            {
                Id = _incident.Id
            };
            caseEntity.ipg_ActualCarrierResponsibility = new Money((sumExpectedReimbursement - unmetdeductible) * (decimal)carrierConisurance / 100);
            caseEntity.ipg_ActualMemberResponsibility = new Money((sumExpectedReimbursement - unmetdeductible) * (decimal)memberConisurance / 100);
            caseEntity.ipg_ActualRevenue = new Money(caseEntity.ipg_ActualCarrierResponsibility.Value + caseEntity.ipg_ActualMemberResponsibility.Value);
            _crmService.Update(caseEntity);
        }
        private void CreateUserTask(string globalSettingTaskName)
        {
            var taskTypeName = _crmService.GetGlobalSettingValueByKey(globalSettingTaskName);
            _taskManager.CreateTask(null, taskTypeName);
        }

        private Intake.Product GetAdminFeeProduct()
        {
            var query = new QueryExpression(Intake.Product.EntityLogicalName)
            {
                Criteria = new FilterExpression(LogicalOperator.And),
                ColumnSet = new ColumnSet(Intake.Product.Fields.Price, Intake.Product.Fields.Name, Intake.Product.Fields.ProductNumber, Intake.Product.Fields.ipg_quantity, Intake.Product.Fields.DefaultUoMId)
            };
            query.Criteria.AddCondition(Intake.Product.Fields.Name, ConditionOperator.Equal, "Admin Fee");

            var result = _crmService.RetrieveMultiple(query);
            if (result.Entities.Count == 0)
            {
                throw new Exception("Admin Fee product not defined.");
            }
            return result.Entities[0].ToEntity<Intake.Product>();
        }

        private void UpdateLatestSubmittedClaimICN(ipg_claim_type? claimType, string icn)
        {
            var query = new QueryExpression(Invoice.EntityLogicalName)
            {
                ColumnSet = new ColumnSet("ipg_icn"),
                Criteria = new FilterExpression(LogicalOperator.And)
            };

            query.Criteria.AddCondition("ipg_claimtypecode", ConditionOperator.Equal, (int?)claimType);
            query.Criteria.AddCondition("ipg_ismanuallysubmitted", ConditionOperator.Equal, true);
            query.Criteria.AddCondition("ipg_caseid", ConditionOperator.Equal, _incident.Id);
            query.AddOrder("lastbackofficesubmit", OrderType.Descending);

            var result = _crmService.RetrieveMultiple(query);
            if (result.Entities.Count == 0)
            {
                throw new Exception("Can't create replacement claim because there are no existing claims to replace.");
            }
            var latestSubmittedClaim = result.Entities[0].ToEntity<Invoice>();
            latestSubmittedClaim.ipg_icn = icn;
            latestSubmittedClaim.ipg_replacementicn = icn;
            _crmService.Update(latestSubmittedClaim);
        }

        private void UpdateCase(ipg_claim_type? claimType)
        {
            var caseEntity = new Incident()
            {
                Id = _incident.Id
            };
            caseEntity.ipg_claimtype = claimType == ipg_claim_type.CMS1500 ? "CMS15000" : "UB04";
            caseEntity.ipg_ispromotedforcollection = true;
            caseEntity.ipg_collectiondate = DateTime.Now;
            _crmService.Update(caseEntity);
        }

        private void SetExistingClaimsInactiveAndMoveToHistory(List<Invoice> existingClaims, Guid newClaimId, Guid tempClosedClaimStatusId)
        {
            foreach (var claim in existingClaims)
            {
                //update custom status field
                var updateEntity = new Invoice()
                {
                    Id = claim.Id,
                    ipg_active = false,
                    ipg_claimstatusid = new EntityReference("ipg_claimstatuscode", tempClosedClaimStatusId)
                };

                _crmService.Update(updateEntity);

                //insert record in history table
                var historyEntity = new ipg_claimshistory()
                {
                    Id = Guid.NewGuid(),
                    ipg_name = claim.GetAttributeValue<string>("name"),
                    ipg_claimid = new EntityReference("invoice", newClaimId),
                    ipg_claimstatusid = claim.ipg_claimstatusid,
                    ipg_createddate = claim.CreatedOn,
                    ipg_claimamount = claim.TotalAmount,
                    ipg_createdby = claim.CreatedBy,
                    ipg_caseid = claim.ipg_caseid,
                    ipg_description = claim.Description,
                    ipg_denialcategorycode = claim.ipg_denialcategorycode,
                    ipg_icn = claim.ipg_icn,
                    ipg_isprimaryodsecondaryclaim = claim.ipg_isprimaryorsecondaryclaim,
                    ipg_claimtypecode = claim.ipg_claimtypecode,
                    ipg_carrierid = claim.CustomerId

                };

                _crmService.Create(historyEntity);
            }
        }

        private void CreateGLTransactions()
        {
            _tracingService.Trace("Creating GL transactions");

            _tracingService.Trace("Determine Network name");
            string networkName = CaseHelper.DetermineNetworkName(_incident, _crmService, _tracingService);

            _tracingService.Trace("Creating a revenue transaction from Carrier and Member responsibilities");
            CreateGlTransaction(networkName, _incident.ipg_ActualCarrierResponsibility?.Value ?? 0, _incident.ipg_ActualMemberResponsibility?.Value ?? 0);

            //adjustment transaction
            if (Math.Abs((_incident.ipg_CarrierAdjustment?.Value ?? 0) + (_incident.ipg_MemberAdjustment?.Value ?? 0)) >= 1)
            {
                _tracingService.Trace("Creating a revenue transaction from Carrier and Member adjustments");
                CreateGlTransaction(networkName, -(_incident.ipg_CarrierAdjustment?.Value ?? 0), -((_incident.ipg_MemberAdjustment?.Value ?? 0)));
            }

            _tracingService.Trace("Setting PostedToAccount to true");
            var caseEntity = new Incident()
            {
                Id = _incident.IncidentId.Value,
                ipg_PostedToAccount = true
            };

            _crmService.Update(caseEntity);
        }

        private void CreateGlTransaction(string networkName, decimal payorRevenue, decimal patientRevenue)
        {
            var revenueTransaction = new ipg_GLTransaction()
            {
                ipg_IncidentId = new EntityReference(Incident.EntityLogicalName, _incident.Id),
                ipg_NetworkType = networkName,
                ipg_TransactionType = "S",
                ipg_name = _incident.Title,
                ipg_PayorRevenue = new Money(payorRevenue + patientRevenue),
                ipg_PatientRevenue = new Money(0)
            };
            _crmService.Create(revenueTransaction);
        }

        private EntityReference CreateCaseDocument(Guid caseId, string fileContent, string fileName, ipg_claim_type? claim_Type)
        {
            var claimDocumentTypeRef = GetClaimDocumentTypeId(claim_Type);
            var existingDocumentId = GetExistingDocumentId(claimDocumentTypeRef.Id);

            if (existingDocumentId != null)
            {
                _crmService.Update(new ipg_document() { Id = existingDocumentId.Value, ipg_CaseId = null });
            }

            var document = new ipg_document()
            {
                ipg_CaseId = new EntityReference(Incident.EntityLogicalName, caseId),
                ipg_FileName = fileName,
                ipg_name = fileName,
                ipg_Source = new OptionSetValue((int)ipg_DocumentSourceCode.User),
                ipg_DocumentTypeId = claimDocumentTypeRef
            };

            document.Id = _crmService.Create(document);

            var annotation = new Annotation()
            {
                FileName = fileName,
                ObjectId = new EntityReference(ipg_document.EntityLogicalName, document.Id),
                DocumentBody = fileContent,
                MimeType = "application/pdf"
            };

            _crmService.Create(annotation);

            return document.ToEntityReference();
        }

        private Guid CreateClaim(string claimName, Guid claimStatus, decimal claimTotal, ipg_claim_type? claimType, EntityReference carrierRef, out ipg_ClaimReason? claimReason)
        {

            var queryExpression = new QueryExpression(ipg_claimconfiguration.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(ipg_claimconfiguration.Fields.ipg_ClaimStatus, ipg_claimconfiguration.Fields.ipg_ClaimReason),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                        {
                            new ConditionExpression(ipg_claimconfiguration.Fields.ipg_ClaimEvent, ConditionOperator.Equal, (int)ipg_ClaimEvent.Creation),
                            new ConditionExpression(ipg_claimconfiguration.Fields.ipg_ClaimSubEvent, ConditionOperator.Equal, (_request.ManualClaim ? (int)ipg_ClaimSubEvent.ManualCreation : (int)ipg_ClaimSubEvent.AutomaticCreation)),
                            new ConditionExpression(ipg_claimconfiguration.Fields.ipg_PaperClaim, ConditionOperator.Equal, _request.GeneratePdfFlag)
                        }
                }
            };

            var claimConfiguration = _crmService.RetrieveMultiple(queryExpression).Entities.Select(e => e.ToEntity<ipg_claimconfiguration>())?.FirstOrDefault();

            var claim = new Invoice()
            {
                Name = claimName,
                ipg_claimstatusid = new EntityReference(ipg_claimstatuscode.EntityLogicalName, claimStatus),
                TotalAmount = new Money(claimTotal),
                ipg_caseid = new EntityReference(_incident.LogicalName, _incident.Id),
                ipg_patientid = _incident.ipg_PatientId,
                ipg_claimtypecode = claimType?.ToOptionSetValue(),
                ipg_isprimaryorsecondaryclaim = _isAutoCarrierGeneration || _request.IsPrimaryOrSecondaryClaim,
                CustomerId = carrierRef,
                ipg_physicianid = _incident.ipg_PhysicianId,
                ipg_facilityid = _incident.ipg_FacilityId,
                Description = _incident.Description,
                ipg_claimtotal = new Money(claimTotal)
            };

            if (claimType == ipg_claim_type.UB04)
            {
                claim.ipg_ipg_ub04typeofbillcode = _request.IsReplacementClaim ? _UB04ReplacementTypeOfBillCode : _UB04NewTypeOfBillCode;
            }

            claim.ipg_insuredfirstname = _isPrimaryCarrierPosition ? _incident.ipg_InsuredFirstName : _incident.ipg_secondaryinsuredfirstname;
            claim.ipg_insuredmi = _isPrimaryCarrierPosition ? _incident.ipg_InsuredMiddleName : _incident.ipg_secondaryinsuredmiddlename;
            claim.ipg_insuredlastname = _isPrimaryCarrierPosition ? _incident.ipg_InsuredLastName : _incident.ipg_secondaryinsuredlastname;
            claim.ipg_insuredgender = _isPrimaryCarrierPosition ? _incident.ipg_InsuredGender : _incident.ipg_secondaryinsuredgender;
            claim.ipg_insureddob = _isPrimaryCarrierPosition ? _incident.ipg_InsuredDateOfBirth : _incident.ipg_secondaryinsureddateofbirth;
            claim.ipg_insuredcity = _isPrimaryCarrierPosition ? _incident.ipg_insuredcity : _incident.ipg_secondaryinsuredcity;
            claim.ipg_insuredaddress = _isPrimaryCarrierPosition ? _incident.ipg_insuredaddress : _incident.ipg_secondaryinsuredaddress;
            claim.ipg_insuredstate = _isPrimaryCarrierPosition ? _incident.ipg_insuredstate : _incident.ipg_secondaryinsuredstate;
            claim.ipg_insuredphone = _isPrimaryCarrierPosition ? _incident.ipg_insuredphone : _incident.ipg_secondaryinsuredphone;
            claim.ipg_insuredzipcodeid = _isPrimaryCarrierPosition ?  _incident.ipg_insuredzipcodeid : _incident.ipg_secondaryinsuredzipcodeid;

            claimReason = null;
            if (claimConfiguration != null)
            {
                claim.ipg_Status = claimConfiguration.ipg_ClaimStatus;
                claim.ipg_Reason = claimConfiguration.ipg_ClaimReason;
                claimReason = claim.ipg_ReasonEnum;
            }

            if (_request.IsReplacementClaim)
            {
                claim.ipg_isreplacement = true;
                claim.ipg_icn = _request.Icn;
                claim.ipg_replacementicn = _request.Icn;
            }

            var claimId = _crmService.Create(claim);

            claim = new Invoice()
            {
                Id = claimId,
                TotalAmount = new Money(claimTotal)
            };
            _crmService.Update(claim);
            return claimId;
        }

        private EntityReference GetClaimDocumentTypeId(ipg_claim_type? claimType)
        {
            string claimDocumentTypeName = claimType == ipg_claim_type.CMS1500 ? _claimDocumentTypeNameCMS1500 : _claimDocumentTypeNameUB04;
            var query = new QueryExpression(ipg_documenttype.EntityLogicalName)
            {
                ColumnSet = new ColumnSet("ipg_documenttypeid"),
                Criteria = new FilterExpression(LogicalOperator.And),
                TopCount = 1
            };

            query.Criteria.AddCondition(ipg_document.Fields.ipg_name, ConditionOperator.Equal, claimDocumentTypeName);

            var result = _crmService.RetrieveMultiple(query);
            if (result.Entities.Count > 0)
            {
                return new EntityReference(ipg_documenttype.EntityLogicalName, result.Entities[0].Id);
            }
            else
            {
                throw new InvalidPluginExecutionException($"Document Type with name '{claimDocumentTypeName}' doesn't exist.");
            }
        }
        private Guid? GetExistingDocumentId(Guid claimDocumentTypeId)
        {
            var query = new QueryExpression(ipg_document.EntityLogicalName)
            {
                ColumnSet = new ColumnSet("ipg_documentid"),
                Criteria = new FilterExpression(LogicalOperator.And),
                TopCount = 1
            };

            query.Criteria.AddCondition("ipg_caseid", ConditionOperator.Equal, _incident.Id);
            query.Criteria.AddCondition("ipg_documenttypeid", ConditionOperator.Equal, claimDocumentTypeId);

            var result = _crmService.RetrieveMultiple(query);
            if (result.Entities.Count > 0)
            {
                return result.Entities[0].Id;
            }
            else
            {
                return null;
            }
        }

        private void CreateClaimLineItems(Dictionary<string, Dictionary<string, object>> lineItems, Dictionary<string, Dictionary<string, object>> lineItemsDetails, bool consoliateCodes, Guid claimId)
        {
            var lineNumber = 1;
            foreach (var lineItem in lineItems)
            {
                _tracingService.Trace("var claimLineItem");
                _tracingService.Trace($"{string.Join(",", lineItem.Value.Keys.Select(x => x))}");
                var claimLineItem = new ipg_claimlineitem()
                {
                    ipg_name = lineItem.Value.GetValueORDefault("HcpcsName")?.ToString(),
                    ipg_claimid = new EntityReference("invoice", claimId),
                    ipg_hcpcsid = (EntityReference)lineItem.Value.GetValueORDefault("HcpcsId"),
                    ipg_quantity = (decimal)lineItem.Value.GetValueORDefault("Quantity"),
                    ipg_billedchg = new Money((decimal)lineItem.Value.GetValueORDefault("BilledChg")),
                    ipg_unitprice = new Money((decimal)lineItem.Value.GetValueORDefault("UnitPrice") / (decimal)lineItem.Value.GetValueORDefault("Quantity")),
                    ipg_ExpectedReimbursement = new Money((decimal)lineItem.Value.GetValueORDefault("ExpectedReimbursement")),
                    ipg_linenumber = lineNumber++,
                };


                var cliId = _crmService.Create(claimLineItem);

                foreach (var liDetail in lineItemsDetails)
                {

                    _tracingService.Trace("var liDetail in lineItemsDetails");
                    var lineItemHcpcs = lineItem.Value.Keys.Contains("HcpcsName") ? lineItem.Value["HcpcsName"].ToString().ToUpper() : string.Empty;
                    var liDetailHcpcs = liDetail.Value.Keys.Contains("HcpcsName") ? liDetail.Value["HcpcsName"].ToString().ToUpper() : string.Empty;
                    var lineItemPartId = lineItem.Value.Keys.Contains("PartId") ? ((EntityReference)lineItem.Value["PartId"]).Id : Guid.Empty;
                    var liDetailPartId = liDetail.Value.Keys.Contains("PartId") ? ((EntityReference)liDetail.Value["PartId"]).Id : Guid.Empty;

                    if ((lineItemHcpcs.Equals(liDetailHcpcs) && consoliateCodes) || (lineItemPartId.Equals(liDetailPartId) && !consoliateCodes))
                    {
                        _tracingService.Trace("liDetailHcpcs consoliateCodes");
                        var claimLIDetail = new InvoiceDetail()
                        {
                            ProductName = liDetail.Value.GetValueORDefault("PartName")?.ToString(),
                            ipg_claimlineitemid = new EntityReference("ipg_claimlineitem", cliId),
                            PricePerUnit = new Money((decimal)liDetail.Value.GetValueORDefault("UnitPrice")),
                            ipg_billedchg = new Money((decimal)liDetail.Value.GetValueORDefault("BilledChg")),
                            Quantity = (decimal)liDetail.Value.GetValueORDefault("Quantity"),
                            ProductId = (EntityReference)liDetail.Value.GetValueORDefault("PartId"),
                            ipg_partnumber = liDetail.Value.GetValueORDefault("PartNumber")?.ToString(),
                            InvoiceId = new EntityReference("invoice", claimId),
                            ipg_manufacturerid = (EntityReference)liDetail.Value.GetValueORDefault("ManufacturerId"),
                            ipg_costprice = new Money((decimal)liDetail.Value.GetValueORDefault("CostPrice")),
                            UoMId = (EntityReference)liDetail.Value.GetValueORDefault("UoMId"),
                            BaseAmount = new Money((decimal)liDetail.Value.GetValueORDefault("BilledChg"))
                        };

                        _crmService.Create(claimLIDetail);
                    }
                }
            }
        }

        private string GenerateUb04Pdf(string tempClaimNumber, bool consolidateCodes, Dictionary<string, Dictionary<string, object>> lineItems, Invoice claim, List<ipg_casepartdetail> actualParts)
        {
            var pdfBase64 = string.Empty;
            var pdfData = _configRepo.GetConfigurations(ipg_claim_type.UB04, _incident, claim);

            var primaryCarrier = _incident.ipg_CarrierId != null ? _crmService.Retrieve(_incident.ipg_CarrierId.LogicalName, _incident.ipg_CarrierId.Id, new ColumnSet(true)).ToEntity<Intake.Account>() : null;
            var secondaryCarrier = _incident.ipg_SecondaryCarrierId != null ? _crmService.Retrieve(_incident.ipg_SecondaryCarrierId.LogicalName, _incident.ipg_SecondaryCarrierId.Id, new ColumnSet(true)).ToEntity<Intake.Account>() : null;
            var patient = _incident.ipg_PatientId != null ?  _crmService.Retrieve(_incident.ipg_PatientId.LogicalName, _incident.ipg_PatientId.Id, new ColumnSet(true)).ToEntity<Intake.Contact>() : null;
            var pClaimAddress = _incident.ipg_PrimaryCarrierClaimsMailingAddress != null ? _crmService.Retrieve(_incident.ipg_PrimaryCarrierClaimsMailingAddress.LogicalName, _incident.ipg_PrimaryCarrierClaimsMailingAddress.Id, new ColumnSet(true)).ToEntity<ipg_carrierclaimsmailingaddress>() : null;
            var sClaimAddress = _incident.ipg_SecondaryCarrierClaimsMailingAddress != null ? _crmService.Retrieve(_incident.ipg_SecondaryCarrierClaimsMailingAddress.LogicalName, _incident.ipg_SecondaryCarrierClaimsMailingAddress.Id, new ColumnSet(true)).ToEntity<ipg_carrierclaimsmailingaddress>() : null;
            var providerLocationInfo = GetProviderInfo();

            var relationToInsured = _isPrimaryCarrierPosition ? _incident.ipg_RelationToInsuredEnum : _incident.ipg_SecondaryCarrierRelationToInsuredEnum;
            var autoOrWc = _carrier.ipg_CarrierTypeEnum == ipg_CarrierType.WorkersComp || _carrier.ipg_CarrierTypeEnum == ipg_CarrierType.Auto;

            var where1 = $"{providerLocationInfo.Address}\r\n{providerLocationInfo.City}, {providerLocationInfo.State} {providerLocationInfo.ZipCode}";
            var where2 = $"{pdfData.GetValueORDefault("Billing Provider Info& PH# Address")}\r\n{pdfData.GetValueORDefault("Billing Provider Info& PH# City State Zip")}";

            pdfData.AddIfNotNull("one", $"{providerLocationInfo.LocationName}\r\n{where1}".ToUpper());
            pdfData.AddIfNotNull("two", $"{pdfData.GetValueORDefault("Billing Provider Info& PH# Name")}\r\n{where2}".ToUpper());

            pdfData.AddIfNotNull("patctrl.0", tempClaimNumber);


            //the below code will need to be changed as the type of bill will no longer be set on the carrier. 
            //the new process for determining the type of bill is still in discovery. See CPI-8881

            var typeOfBill = _request.IsReplacementClaim ? _UB04ReplacementTypeOfBillCode : _UB04NewTypeOfBillCode;
            pdfData.AddIfNotNull("type", typeOfBill);

            var actualDos = _incident.ipg_ActualDOS.Value.ToString("MMddyy");
            pdfData.AddIfNotNull("cid", actualDos);
            pdfData.AddIfNotNull("lrd", actualDos);
            pdfData.AddIfNotNull("ms", actualDos);

            var patientInfo = GetPatientInfo();

            pdfData.AddIfNotNull("patname", patientInfo.FullName.ToUpper());
            pdfData.AddIfNotNull("pataddress", patientInfo.Address);
            pdfData.AddIfNotNull("patcity", patientInfo.City);
            pdfData.AddIfNotNull("covd", patientInfo.StateAbbr);
            pdfData.AddIfNotNull("perthru.0.0", patientInfo.ShortZipCode);

            pdfData.AddIfNotNull("birthdate", patientInfo.DOB?.ToString("MM/dd/yyy"));
            pdfData.AddIfNotNull("sex", patientInfo.Gender == ipg_Gender.Male ? "M" : (_incident.ipg_PatientGenderEnum == ipg_Gender.Female ? "F" : ""));

            var insuredInfo = GetInsuredInfoFromCarrier(_carrier.ToEntityReference());
            var pInsuredInfo = GetInsuredInfoFromCarrier(_incident.ipg_CarrierId);
            var sInsuredInfo = GetInsuredInfoFromCarrier(_incident.ipg_SecondaryCarrierId);

             pdfData.AddIfNotNull("38.1", $"{insuredInfo.FullName}\r\n{insuredInfo.Address}\r\n{insuredInfo.City}, {insuredInfo.StateAbbr} {insuredInfo.ShortZipCode}");

            string relInfo = pdfData.ContainsKey("REL. INFO") ? pdfData["REL. INFO"] : null,
                asfBen = pdfData.ContainsKey("ASG. BEN.") ? pdfData["ASG. BEN."] : null,
                ipgNPI = pdfData.ContainsKey("IPG NPI") ? pdfData["IPG NPI"] : null,
                ipgTaX = pdfData.ContainsKey("IPG Tax") ? pdfData["IPG Tax"] : null,
                primaryrelationToInsuredKey = _incident.ipg_RelationToInsuredEnum?.ToString(),
                secondaryRelationToInsuredKey = _incident.ipg_SecondaryCarrierRelationToInsuredEnum?.ToString(),
                primaryAuthNumber = _incident.ipg_AuthorizationId != null ?
                _crmService.Retrieve(_incident.ipg_AuthorizationId.LogicalName, _incident.ipg_AuthorizationId.Id, new ColumnSet(ipg_authorization.Fields.ipg_ipgauthnumber)).ToEntity<ipg_authorization>().ipg_ipgauthnumber : "",
                secondaryAuthNumber = _incident.ipg_secondaryauthorizationid != null ?
                _crmService.Retrieve(_incident.ipg_secondaryauthorizationid.LogicalName, _incident.ipg_secondaryauthorizationid.Id, new ColumnSet(ipg_authorization.Fields.ipg_ipgauthnumber)).ToEntity<ipg_authorization>().ipg_ipgauthnumber : "",
                primarymostRecentICN = "",
                secondaryMostRecentICN = "",
                primaryPlanSponsor = "",
                secondaryPalnSpomsor = "";

            var totalNumParts = lineItems.Count();
            decimal claimTotal = lineItems.Sum(x => x.Value["BilledChg"] as decimal?) ?? 0;
            var carrierName = _carrier.Name;

            if(consolidateCodes && lineItems.ContainsKey("C1713") && (decimal)lineItems["C1713"]["Quantity"] > 20 
                || !consolidateCodes && lineItems.Keys.Any(key => string.Equals(lineItems[key]["HcpcsName"], "C1713") && (decimal)lineItems[key]["Quantity"] > 20))
            {
                var keyC1713 = "C1713";
                if(!consolidateCodes)
                {
                    keyC1713 = lineItems.Keys.First(key => string.Equals(lineItems[key]["HcpcsName"], "C1713"));
                }
                int count = Decimal.ToInt32((decimal)lineItems[keyC1713]["Quantity"] / 20);
                decimal billedChargesC1713 = (decimal)lineItems[keyC1713]["BilledChg"];
                decimal quantityC1713 = (decimal)lineItems[keyC1713]["Quantity"];
                lineItems[keyC1713]["BilledChg"] = (decimal)Math.Round(billedChargesC1713 / 20, 2);
                lineItems[keyC1713]["Quantity"] = (decimal)Math.Round(quantityC1713 / 20, 2);
                var obj = lineItems["C1713"];
                for (var i = 2; i < count; i++)
                {
                    lineItems.Add(keyC1713 + "_" + i.ToString(), obj);
                }
                obj["BilledChg"] = (decimal)(billedChargesC1713 - (count - 1) * (decimal)obj["BilledChg"]);
                obj["Quantity"] = (decimal)(quantityC1713 - (count - 1) * (decimal)obj["Quantity"]);
                lineItems.Add(keyC1713 + "_" + count.ToString(), obj);
            }

            decimal primaryPriorPayments = _incident.ipg_TotalReceivedfromCarrier?.Value ?? 0,
                secondaryPriorpayments = _incident.ipg_TotalReceivedfromSecondaryCarrier?.Value ?? 00,
                primaryEstAmountDue = primaryPriorPayments - claimTotal,
                secondaryEstAmountDue = secondaryPriorpayments - claimTotal;

            pdfData.AddIfNotNull("50payer.1", pClaimAddress?.ipg_carrierclaimname);
            pdfData.AddIfNotNull("51providernum.1", primaryCarrier?.ipg_HealthPlanIDBox51);
            pdfData.AddIfNotNull("54prior.1", primaryCarrier != null ? primaryPriorPayments.ToString("0.00") : null);
            pdfData.AddIfNotNull("55est.1", primaryCarrier != null ? primaryEstAmountDue.ToString("0.00") : null);
            pdfData.AddIfNotNull("56.2", primaryCarrier != null ? ipgTaX : null);

            pdfData.AddIfNotNull("50payer.2", sClaimAddress?.ipg_carrierclaimname);
            pdfData.AddIfNotNull("51providernum.2", secondaryCarrier?.ipg_HealthPlanIDBox51);
            pdfData.AddIfNotNull("54prior.2", secondaryCarrier != null ? secondaryPriorpayments.ToString("0.00") : null);
            pdfData.AddIfNotNull("55est.2", secondaryCarrier != null ? secondaryEstAmountDue.ToString("0.00") : null);
            pdfData.AddIfNotNull("56.3", secondaryCarrier != null ? ipgTaX : null);

            pdfData.AddIfNotNull("56.1", ipgNPI);

            pdfData.AddIfNotNull("57", pInsuredInfo?.FullNameForUB04);
            pdfData.AddIfNotNull("59prel.1", GetRelationToInsuredPresentation(_incident.ipg_RelationToInsured));
            pdfData.AddIfNotNull("60cert.1", pInsuredInfo?.MemberIdNumber);
            pdfData.AddIfNotNull("62insgroup.1", pInsuredInfo?.GroupNumber);

            pdfData.AddIfNotNull("58insname.1", sInsuredInfo?.FullNameForUB04);
            pdfData.AddIfNotNull("59prel.2", GetRelationToInsuredPresentation(_incident.ipg_SecondaryCarrierRelationToInsured));
            pdfData.AddIfNotNull("60cert.2",  sInsuredInfo?.MemberIdNumber);
            pdfData.AddIfNotNull("62insgroup.2",  sInsuredInfo?.GroupNumber);

            pdfData.AddIfNotNull("63treatment.1", primaryAuthNumber);
            pdfData.AddIfNotNull("65empname.1", primaryAuthNumber);
            pdfData.AddIfNotNull("66emploc.1", primaryPlanSponsor);

            pdfData.AddIfNotNull("63treatment.2", primarymostRecentICN);
            pdfData.AddIfNotNull("65empname.2", secondaryMostRecentICN);
            pdfData.AddIfNotNull("66emploc.2", secondaryPalnSpomsor);



            pdfData.AddIfNotNull("67prin", _incident.ipg_DxCodeId1?.Name);
            pdfData.AddIfNotNull("princode.0.0", _incident.ipg_DxCodeId1?.Name);
            pdfData.AddIfNotNull("68code", _incident.ipg_DxCodeId2?.Name);
            pdfData.AddIfNotNull("69code", _incident.ipg_DxCodeId3?.Name);
            pdfData.AddIfNotNull("71code", _incident.ipg_DxCodeId4?.Name);
            pdfData.AddIfNotNull("72code", _incident.ipg_DxCodeId5?.Name);
            pdfData.AddIfNotNull("73code", _incident.ipg_DxCodeId6?.Name);


            var dxCode = _incident.ipg_DxCodeId1 != null ? _crmService.Retrieve(_incident.ipg_DxCodeId1.LogicalName, _incident.ipg_DxCodeId1.Id, new ColumnSet(ipg_dxcode.Fields.ipg_ICDVersion)).ToEntity<ipg_dxcode>() : null;

            if (dxCode?.ipg_ICDVersionEnum == ipg_ICDVersion._10)
            {
                pdfData.AddIfNotNull("79pc", "0");
            }
            else
            {
                pdfData.AddIfNotNull("79pc", "9");
            }

            if (_isPrimaryCarrierPosition)
            {
                if (_carrier.ipg_CarrierTypeEnum == ipg_CarrierType.WorkersComp)
                {

                    pdfData.AddIfNotNull("37.2", _incident.ipg_SurgeryDate?.ToString("MMddyy"));
                }
                else if (_carrier.ipg_CarrierTypeEnum == ipg_CarrierType.Auto)
                {
                    if (_incident.ipg_SurgeryDate != null)
                    {
                        pdfData.AddIfNotNull("37.2", _incident.ipg_SurgeryDate?.ToString("MMddyy"));
                        pdfData.AddIfNotNull("32occ.1", _incident.ipg_SurgeryDate?.ToString("MMddyy"));
                        pdfData.AddIfNotNull("32.1", "02");
                    }
                }

                if (autoOrWc)
                {
                    pdfData.AddIfNotNull("other3date", _incident.ipg_DxCodeId1?.Name);
                    pdfData.AddIfNotNull("74code.1.0.0.1", _incident.ipg_DxCodeId2?.Name);
                    pdfData.AddIfNotNull("74code.0.1.1.0", _incident.ipg_DxCodeId3?.Name);
                }

                if (_incident.ipg_pos?.Value == 21)
                {
                    pdfData.AddIfNotNull("princode.1.0.0.0", _incident.ipg_CPTCodeId1?.Name);
                    pdfData.AddIfNotNull("princode.1.1", actualDos);
                }
            }

            if (string.Equals(_carrier.Id.ToString(), _crmService.GetGlobalSettingValueByKey("BCBSNC-CONTRACTED"), StringComparison.OrdinalIgnoreCase))
            {
                pdfData.AddIfNotNull("32.1", "01");
                pdfData.AddIfNotNull("32occ.1", _incident.ipg_ProcedureDateNew?.ToString("MMddyy"));
                pdfData.AddIfNotNull("33.1", "05");
                pdfData.AddIfNotNull("33code.1", _incident.ipg_ProcedureDateNew?.ToString("MMddyy"));
            }

            var healthPlanId = _carrier.ipg_HealthPlanIDBox51;
            if (string.IsNullOrWhiteSpace(healthPlanId))
                healthPlanId = Facility?.ipg_HealthPlanIDBox51;
            if (!string.IsNullOrWhiteSpace(healthPlanId))
            {
                pdfData.AddIfNotNull("51providernum.1", healthPlanId);
            }

            pdfData.AddIfNotNull("55est.1", decimal.Round(claimTotal, 2).ToString("0.00"));


            var carrierAuthNumber = _incident.ipg_carrierauthno;
            if (!string.IsNullOrWhiteSpace(carrierAuthNumber))
            {
                pdfData.AddIfNotNull("63treatment.1", carrierAuthNumber.Replace("\r\n", "").Replace("\n", ""));
            }

            var ippDescription = _request.IsReplacementClaim ? _request.Icn : _incident.ipg_Description;
            if (!string.IsNullOrWhiteSpace(ippDescription))
            {
                var newLineIndex = ippDescription.IndexOf("\r\n");
                if (newLineIndex != -1)
                {
                    pdfData.AddIfNotNull("108", ippDescription.Substring(0, newLineIndex));
                }
                else
                {
                    pdfData.AddIfNotNull("108", ippDescription);
                }
            }

            if (_request.IsPrimaryOrSecondaryClaim)
            {
                //For primary Claim there shouldn't be Secondary Carrier header
                if(pdfData.ContainsKey("header"))
                {
                    pdfData.Remove("header");
                }

                pdfData.AddIfNotNull("65empname.1", ippDescription);
            }
            else
            {
                pdfData.AddIfNotNull("65empname.2", ippDescription);
                var employer2 = _incident.ipg_secondaryinsuredemployer;
                if (!string.IsNullOrWhiteSpace(employer2))
                {
                    pdfData.AddIfNotNull("66emploc.2", employer2.ToUpper());
                }
            }

            var employer = patient.ipg_EmployerName;
            if (!string.IsNullOrWhiteSpace(employer))
            {
                pdfData.AddIfNotNull("66emploc.1", employer.ToUpper());
            }

            //https://eti-ipg.atlassian.net/browse/CPI-4848 
            //SB: For physicians who are performing procedures outside of the state their NPI is in, that the system can automatically apply the facility NPI for that case. 
            //(NOTES: Otherwise, the claim will fail for the physician NPI being in a different state than where the procedure took place. 
            //This occurs often in our TX market where there are teaching clinics and the physician listed on the case has an NPI whose state is outside of TX).
            var physicianNPI = (Facility?.ipg_StateId?.Id == Physician?.ipg_PhysicianNPIState?.Id) ? Physician?.ipg_PhysicianNPI : Facility?.ipg_FacilityNPI;

            pdfData.AddIfNotNull("NPI.1.0.1.1", physicianNPI);
            pdfData.AddIfNotNull("NPI.0.0", physicianNPI);

            pdfData.AddIfNotNull("NPI.1.0.0.1.0", Physician?.FirstName);
            pdfData.AddIfNotNull("NPI.1.1", Physician?.FirstName);

            pdfData.AddIfNotNull("NPI.1.0.1.0.0.0", Physician?.LastName);
            pdfData.AddIfNotNull("NPI.1.0.0.0", Physician?.LastName);

            pdfData.AddIfNotNull("princode.0.1.1.1.0.1.0.0", physicianNPI);
            pdfData.AddIfNotNull("princode.0.1.1.1.0.1.0.1", physicianNPI);

            //should be put to claim configs overrides conditions field# 80 https://eti-ipg.atlassian.net/wiki/spaces/IPG/pages/1399980045/User+Generation+of+Secondary+Claim+-+UB04+Phase+1
            pdfData.AddIfNotNull("remarks", _carrier.ipg_CarrierCPTPricing != null ? _incident.ipg_billedcpt : null);

            pdfData.AddIfNotNull("45servdate.22.1.0", DateTime.Now.ToString("MM/dd/yy"));

            var pdfTemplateNote = GetPdfTemplateAnnotation("UB04");
            var bytes = Convert.FromBase64String(pdfTemplateNote.DocumentBody);

            using (var ms = new MemoryStream(bytes))
            {
                using (var pdfDoc = PdfSharp.Pdf.IO.PdfReader.Open(ms, PdfSharp.Pdf.IO.PdfDocumentOpenMode.ReadOnly))
                {
                    PdfAcroField.PdfAcroFieldCollection pdfFields = pdfDoc.AcroForm.Fields;
                    var lastPage = pdfDoc.Pages[1].Clone() as PdfPage;
                    pdfDoc.Pages.RemoveAt(1);

                    var field42 = "0275";
                    if (string.Equals(_carrier.Id.ToString(), _crmService.GetGlobalSettingValueByKey("BC-Anthem-CA"), StringComparison.OrdinalIgnoreCase))
                    {
                        var hcpcsCodes = new string[] { "C1721", "C1722", "C1777", "C1785", "C1882", "C1895", "C1898", "C1900" };
                        if(consolidateCodes)
                        {
                            if(lineItems.Keys.Any(key => hcpcsCodes.Any(code => string.Equals(key, code, StringComparison.OrdinalIgnoreCase)))) {
                                field42 = "0278";
                            }
                        }
                        else
                        {
                            if (lineItems.Keys.Any(key => hcpcsCodes.Any(code => string.Equals((string)lineItems[key]["HcpcsName"], code, StringComparison.OrdinalIgnoreCase)))) {
                                field42 = "0278";
                            }
                        }
                    }
                    else
                    {
                        if (consolidateCodes)
                        {
                            if (lineItems.Keys.Any(key => string.Equals(key, "C2618", StringComparison.OrdinalIgnoreCase)))
                            {
                                field42 = "0278";
                            }
                        }
                        else
                        {
                            if (lineItems.Keys.Any(key => string.Equals((string)lineItems[key]["HcpcsName"], "C2618", StringComparison.OrdinalIgnoreCase)))
                            {
                                field42 = "0278";
                            }
                        }
                    }

                    int pages = lineItems.Count / 22 + (lineItems.Count % 22 > 0 ? 1 : 0);
                    pdfData.AddIfNotNull("revcd42.22.1.1", pages.ToString());

                    for (int page = 0; page < pages; page++)
                    {
                        var pdfLinesData = new Dictionary<string, string>();
                        var partCounter = 1;
                        decimal claimTotalForPage = 0;
                        var keys = lineItems.Keys.Skip(page * 22).Take(22);

                        pdfLinesData.Add("revcd42.22.0.1", (page + 1).ToString());

                        foreach (var key in keys)
                        {
                            var billed = (decimal)lineItems[key]["BilledChg"];
                            var hcpcsName = consolidateCodes ? key : (string)lineItems[key]["HcpcsName"];
                            var quantity = ((decimal)lineItems[key]["Quantity"]).ToString("0");
                            var partName = (string)lineItems[key]["HcpcsDescription"];

                            pdfLinesData.AddIfNotNull($"revcd42.{partCounter.ToString()}", field42);

                            var field43 = partName.ToUpper();
                            var hcpcsCodes = new string[] { "L8699", "C2618", "A4649", "Q4100" };
                            var flag = false;
                            if (consolidateCodes)
                            {
                                flag = lineItems.Keys.Any(e => hcpcsCodes.Any(code => string.Equals(e, code, StringComparison.OrdinalIgnoreCase)));
                            }
                            else
                            {
                                flag = lineItems.Keys.Any(e => hcpcsCodes.Any(code => string.Equals((string)lineItems[e]["HcpcsName"], code, StringComparison.OrdinalIgnoreCase)));
                            }
                            if(flag)
                            {
                                field43 = string.Empty;
                                foreach (var casePart in actualParts)
                                {
                                    var product = _crmService.Retrieve(casePart.ipg_productid.LogicalName, casePart.ipg_productid.Id, new ColumnSet(Intake.Product.Fields.Name, Intake.Product.Fields.ipg_manufacturerpartnumber)).ToEntity<Intake.Product>();
                                    field43 += (string.IsNullOrEmpty(field43) ? string.Empty : ", ") + product.ipg_manufacturerpartnumber + " " + product.Name;
                                }
                            }
                            pdfLinesData.AddIfNotNull($"43desc.{partCounter.ToString()}", field43);

                            var field44 = hcpcsName;
                            if (string.Equals(_carrier.Id.ToString(), _crmService.GetGlobalSettingValueByKey("BC-Anthem-CT"), StringComparison.OrdinalIgnoreCase))
                            {
                                field44 = "L8699";
                            }
                            else if ((string.Equals(_carrier.Id.ToString(), _crmService.GetGlobalSettingValueByKey("BCBSNC-CONTRACTED")) || string.Equals(_carrier.Id.ToString(), _crmService.GetGlobalSettingValueByKey("Aetna"), StringComparison.OrdinalIgnoreCase)) && _incident.ipg_BilledCPTId != null)
                            {
                                var billedCPT = _crmService.Retrieve(_incident.ipg_BilledCPTId.LogicalName, _incident.ipg_BilledCPTId.Id, new ColumnSet(ipg_cptcode.Fields.ipg_ClaimExceptionOverride)).ToEntity<ipg_cptcode>();
                                if(billedCPT.ipg_ClaimExceptionOverride ?? false && _incident.ipg_ActualDOS != null)
                                {
                                    field44 = _incident.ipg_ActualDOS < new DateTime(2021, 3, 1) ? "A4649" : "C2618";
                                }
                            }
                            pdfLinesData.AddIfNotNull($"44hcps.{partCounter.ToString()}", field44);

                            if (string.Equals(_carrier.Id.ToString(), _crmService.GetGlobalSettingValueByKey("BCBS-Anthem-CT"), StringComparison.OrdinalIgnoreCase))
                            {
                                pdfLinesData.AddIfNotNull($"44hcps.{partCounter.ToString()}", "L8699");
                            }
                            else
                            {
                                pdfLinesData.AddIfNotNull($"44hcps.{partCounter.ToString()}", hcpcsName);
                            }

                            pdfLinesData.AddIfNotNull($"45servdate.{partCounter.ToString()}", actualDos);
                            pdfLinesData.AddIfNotNull($"46servunits.{partCounter.ToString()}", quantity);


                            if (string.Equals(_carrier.Id.ToString(), _crmService.GetGlobalSettingValueByKey("Aetna"), StringComparison.OrdinalIgnoreCase) && lineItems.Keys.Any(e => (decimal)lineItems[e]["Quantity"] > 1))
                            {
                                if(string.Equals(hcpcsName, "C1781", StringComparison.OrdinalIgnoreCase))
                                {
                                    pdfLinesData.AddIfNotNull($"46servunits.{partCounter.ToString()}", "1");
                                    pdfLinesData.AddIfNotNull("remarks", "C1781 2 units used Bilateral Procedure");
                                }
                                else if (string.Equals(hcpcsName, "C1762", StringComparison.OrdinalIgnoreCase))
                                {
                                    pdfLinesData.AddIfNotNull($"46servunits.{partCounter.ToString()}", "1");
                                    pdfLinesData.AddIfNotNull("remarks", "C1762 2 units used Post Ant Ligament");
                                }
                                else
                                {
                                    var hcpcsCodes46 = new string[] { "C1781", "C1762" };
                                    var flag46 = false;
                                    if (consolidateCodes)
                                    {
                                        flag46 = !lineItems.Keys.Any(e => hcpcsCodes46.Any(code => string.Equals(e, code, StringComparison.OrdinalIgnoreCase)));
                                    }
                                    else
                                    {
                                        flag46 = !lineItems.Keys.Any(e => hcpcsCodes46.Any(code => string.Equals((string)lineItems[e]["HcpcsName"], code, StringComparison.OrdinalIgnoreCase)));
                                    }
                                    if(flag46)
                                    {
                                        pdfLinesData.AddIfNotNull("remarks", "Dev #20889. C1713 allows up to 20 units. C1776 allows up to 10 units.");
                                    }
                                }
                            }
                            else if (string.Equals(_carrier.Id.ToString(), _crmService.GetGlobalSettingValueByKey("Anthem"), StringComparison.OrdinalIgnoreCase))
                            {
                                pdfLinesData.AddIfNotNull("remarks", _incident.ipg_billedcpt);
                            }

                            pdfLinesData.AddIfNotNull($"47totalcharges.{partCounter.ToString()}", billed.ToString("0.00"));

                            claimTotalForPage += billed;

                            partCounter++;
                        }

                        pdfLinesData.AddIfNotNull("47totalcharges.23", decimal.Round(claimTotalForPage).ToString("0.00"));

                        AddPage(pdfDoc, bytes, pdfData.MergeWith(pdfLinesData), page);

                        partCounter = 0;
                        claimTotalForPage = 0;
                    }

                    using (var ms2 = new MemoryStream())
                    {
                        var roraldocpages = pdfDoc.PageCount; // Cannot save a PDF document with no pages.
                        pdfDoc.AddPage(lastPage);

                        pdfDoc.Flatten();
                        pdfDoc.Save(ms2);

                        var base64 = Convert.ToBase64String(ms2.GetBuffer(), 0, (int)ms2.Length);

                        return base64;
                    }
                }
            }
        }

        private string GenerateCms1500Pdf(string tempClaimNumber, bool consolidateCodes, Dictionary<string, Dictionary<string, object>> lineItems, Invoice claim, List<ipg_casepartdetail> actualParts, string whichHcpcs)
        {
            _tracingService.Trace("Read Claim pdf configs");
            var pdfData = _configRepo.GetConfigurations(ipg_claim_type.CMS1500, _incident, claim);

            var facilityStateAbbr = Facility?.ipg_StateId != null ? _crmService.Retrieve(Facility.ipg_StateId.LogicalName, Facility.ipg_StateId.Id, new ColumnSet(ipg_state.Fields.ipg_abbreviation)).ToEntity<ipg_state>().ipg_abbreviation : null;

            var patient = GetPatientInfo();

            pdfData.AddIfNotNull("patientName", patient.FullName);
            pdfData.AddIfNotNull("12", patient.DOBMonth);
            pdfData.AddIfNotNull("13", patient.DOBDay);
            pdfData.AddIfNotNull("14", patient.DOBYear);

            pdfData.AddIfNotNull("15", patient?.Gender == ipg_Gender.Male ? "Yes" : null);
            pdfData.AddIfNotNull("16", patient?.Gender == ipg_Gender.Female ? "Yes" : null);

            pdfData.AddIfNotNull("18", patient.Address);
            pdfData.AddIfNotNull("19", patient.City);
            pdfData.AddIfNotNull("20", patient.StateAbbr);
            pdfData.AddIfNotNull("21", patient.ShortZipCode);
            pdfData.AddIfNotNull("22", patient.PhoneCode);
            pdfData.AddIfNotNull("23", patient.PhoneMainPart);

            var mainCarrierRef = _carrier.ToEntityReference();
            var insuredInfo = GetInsuredInfoFromCarrier(mainCarrierRef);
            var otherInsuredInfo = GetOtherInsuredInfoFromCarrier(mainCarrierRef);
            
            pdfData.AddIfNotNull("10", insuredInfo.MemberIdNumber);
            pdfData.AddIfNotNull("17", insuredInfo.FullName);

            pdfData.AddIfNotNull("28", insuredInfo.Address);
            pdfData.AddIfNotNull("29", insuredInfo.City);
            pdfData.AddIfNotNull("30", insuredInfo.StateAbbr);
            pdfData.AddIfNotNull("31", insuredInfo.ShortZipCode);

            pdfData.AddIfNotNull("32", insuredInfo.PhoneCode);
            pdfData.AddIfNotNull("33", insuredInfo.PhoneMainPart);

            pdfData.AddIfNotNull("57", insuredInfo.DOBMonth);
            pdfData.AddIfNotNull("58", insuredInfo.DOBDay);
            pdfData.AddIfNotNull("59", insuredInfo.DOBYear);

            pdfData.AddIfNotNull("60", insuredInfo.Gender == ipg_Gender.Male ? "Yes" : null);
            pdfData.AddIfNotNull("61", insuredInfo .Gender == ipg_Gender.Female ? "Yes" : null);

            pdfData.AddIfNotNull("56_2", insuredInfo.GroupNumber);

            pdfData.AddIfNotNull("40", otherInsuredInfo.FullName);
            pdfData.AddIfNotNull("41", otherInsuredInfo.MemberIdNumber);



            if (_carrier.ipg_CarrierTypeEnum == ipg_CarrierType.WorkersComp)
            {
                pdfData.AddIfNotNull("49", "Yes");
            }
            else
            {
                pdfData.AddIfNotNull("50", "Yes");
            }

            if (_carrier.ipg_CarrierTypeEnum == ipg_CarrierType.Auto)
            {
                pdfData.AddIfNotNull("51", "Yes");
                pdfData.AddIfNotNull("53", facilityStateAbbr);
            }
            else
            {
                pdfData.AddIfNotNull("52", "Yes");
            }

            pdfData.AddIfNotNull("81", Physician?.FullName);
            pdfData.AddIfNotNull("84", Physician?.ipg_PhysicianNPI);

            if(string.Equals(_carrier.Id.ToString(), _crmService.GetGlobalSettingValueByKey("BCBS-Anthem-NH"), StringComparison.OrdinalIgnoreCase))
            {
                var field91 = string.Empty;
                foreach (var casePart in actualParts)
                {
                    var hcpcsName = whichHcpcs.Equals("Hcpcs") ? casePart.ipg_ipg_masterhcpcs_ipg_casepartdetail_hcpcscode?.ipg_name : casePart.ipg_ipg_masterhcpcs_ipg_casepartdetail_hcpcscode2?.ipg_name;
                    if(string.Equals(hcpcsName, "V2632", StringComparison.OrdinalIgnoreCase))
                    {
                        var product = _crmService.Retrieve(casePart.ipg_productid.LogicalName, casePart.ipg_productid.Id, new ColumnSet(Intake.Product.Fields.Name, Intake.Product.Fields.ipg_manufacturerpartnumber)).ToEntity<Intake.Product>();
                        field91 += (string.IsNullOrEmpty(field91) ? string.Empty : ", ") + product.ipg_manufacturerpartnumber + " " + product.Name;
                    }
                }
                if(!string.IsNullOrEmpty(field91))
                {
                    pdfData.AddIfNotNull("91", field91);
                }
                pdfData.AddIfNotNull("109", Physician?.FullName + (string.IsNullOrEmpty(Physician?.ipg_PhysicianNPI) ? string.Empty : " NPI " + Physician?.ipg_PhysicianNPI));
                if (Facility != null && string.Equals(Facility.Id.ToString(), _crmService.GetGlobalSettingValueByKey("Bedford Ambulatory Surgical Center"), StringComparison.OrdinalIgnoreCase))
                {
                    pdfData.AddIfNotNull("266", "1801526Y0NH01");
                }
            }

            string dxCodesDescription = string.Empty;
            var dxCodes = GetDxCodesForCase(_incident.ipg_DxCodeId1, _incident.ipg_DxCodeId2, _incident.ipg_DxCodeId3, _incident.ipg_DxCodeId4, _incident.ipg_DxCodeId5, _incident.ipg_DxCodeId6);

            if (_incident.ipg_DxCodeId1 != null)
            {
                dxCodesDescription = "A";
                pdfData.AddIfNotNull("95", dxCodes.Where(d=>d.Id == _incident.ipg_DxCodeId1.Id).First().ipg_DxCode);
            }

            if (_incident.ipg_DxCodeId2 != null)
            {
                dxCodesDescription = "AB";
                pdfData.AddIfNotNull("98", dxCodes.Where(d => d.Id == _incident.ipg_DxCodeId2.Id).First().ipg_DxCode);
            }

            if (_incident.ipg_DxCodeId3 != null)
            {
                dxCodesDescription = "ABC";
                pdfData.AddIfNotNull("101", dxCodes.Where(d => d.Id == _incident.ipg_DxCodeId3.Id).First().ipg_DxCode);
            }

            if (_incident.ipg_DxCodeId4 != null)
            {
                dxCodesDescription = "ABCD";
                pdfData.AddIfNotNull("104", dxCodes.Where(d => d.Id == _incident.ipg_DxCodeId4.Id).First().ipg_DxCode);
            }

            if (_incident.ipg_DxCodeId5 != null)
            {
                pdfData.AddIfNotNull("105", dxCodes.Where(d => d.Id == _incident.ipg_DxCodeId5.Id).First().ipg_DxCode);
            }

            if (_incident.ipg_DxCodeId6 != null)
            {
                pdfData.AddIfNotNull("106", dxCodes.Where(d => d.Id == _incident.ipg_DxCodeId6.Id).First().ipg_DxCode);
            }

            
            var isIcd10 = false;
            foreach (var dxCode in dxCodes)
            {
                if (dxCode.ipg_ICDVersion != null && dxCode.ipg_ICDVersion.Value == (int)ipg_ICDVersion._10)
                {
                    isIcd10 = true;
                    break;
                }
            }
            if (isIcd10 == true)
            {
                pdfData.AddIfNotNull("96", "0");
            }
            else
            {
                pdfData.AddIfNotNull("96", "9");
            }

            if (_request.IsReplacementClaim)
            {
                pdfData.AddIfNotNull("108", _request.Icn);
            }
            else
            {
                if (pdfData.ContainsKey("107"))
                {
                    pdfData.Remove("107");
                }
            }

            pdfData.AddIfNotNull("251", tempClaimNumber);

            pdfData.AddIfNotNull("63", _claimMailingAdress.ipg_carrierclaimname);

            var providerLocationInfo = GetProviderInfo();
            pdfData.AddIfNotNull("262", providerLocationInfo.LocationName);
            pdfData.AddIfNotNull("263", providerLocationInfo.Address);
            pdfData.AddIfNotNull("264", $"{providerLocationInfo.City}, {providerLocationInfo.State} {providerLocationInfo.ZipCode}");

            if (_request.IsPrimaryOrSecondaryClaim || _isAutoCarrierGeneration)
            {
                var primaryHeader = $"{_claimMailingAdress.ipg_carrierclaimname?.ToUpper()}\r\n{_claimMailingAdress.ipg_claimsmailingaddress?.ToUpper()}\r\n{_claimMailingAdress.ipg_claimsmailingcity?.ToUpper()} {_claimMailingAdress.ipg_claimsmailingstate?.ToUpper()} {_claimMailingAdress.ipg_ClaimsMailingZipCodeIdId?.Name?.ToUpper()}";
                if (pdfData.ContainsKey("2"))
                {
                    pdfData["2"] = primaryHeader;
                }
                else
                {
                    pdfData.AddIfNotNull("2", primaryHeader);
                }


                if (!string.Equals(_carrier.Id.ToString(), _crmService.GetGlobalSettingValueByKey("US Dept of Labor-TX"), StringComparison.OrdinalIgnoreCase)
                && !string.Equals(_carrier.Id.ToString(), _crmService.GetGlobalSettingValueByKey("US Dept of Labor-KY"), StringComparison.OrdinalIgnoreCase))
                {
                    pdfData.AddIfNotNull("272", "1114077419");
                }
                else
                {
                    pdfData.AddIfNotNull("272", "601365400");
                }

                var isBSofCA = (string.Equals(_carrier.Id.ToString(), _crmService.GetGlobalSettingValueByKey("BS OF CA"), StringComparison.OrdinalIgnoreCase) && facilityStateAbbr.ToUpper().Equals("CA"));

                if (_carrier.Name.ToUpper().Contains("GALLAGHER BASSETT"))
                {
                    pdfData.AddIfNotNull("273", "");
                }
                else if (isBSofCA)
                {
                    pdfData.AddIfNotNull("273", "ZZZ58806Y");
                }
                else
                {
                    pdfData.AddIfNotNull("273", "1BM2885");
                }
            }

            var actualDos = _incident.ipg_ActualDOS.Value;
            string posDescription = string.Empty;
            string renderingproviderid = string.Empty;

            if (_incident.ipg_posEnum == ipg_facilitytype._11PhysicianOffice)
            {
                posDescription = "11 - Physician Office";
            }
            if (_incident.ipg_posEnum == ipg_facilitytype._21HospitalInPatient)
            {
                posDescription = "21 - Hospital InPatient";
            }
            if (_incident.ipg_posEnum == ipg_facilitytype._22HospitalOutPatient)
            {
                posDescription = "22 - Hospital OutPatient";
            }
            if (_incident.ipg_posEnum == ipg_facilitytype._24AmbulatorySurgeryCenter)
            {
                posDescription = "24 - Ambulatory Surgery Center";
            }

            if (!_request.IsPrimaryOrSecondaryClaim)
            {
                if (pdfData.ContainsKey("PLACE OF SERVICE"))
                {
                    posDescription = pdfData["PLACE OF SERVICE"];
                }
            }

            if (pdfData.ContainsKey("RENDERING PROVIDER ID #"))
            {
                renderingproviderid = pdfData["RENDERING PROVIDER ID #"];
            }

            int pages = lineItems.Count / 6 + (lineItems.Count % 6 > 0 ? 1 : 0);

            var pdfTemplateNote = GetPdfTemplateAnnotation("CMS1500");
            var bytes = Convert.FromBase64String(pdfTemplateNote.DocumentBody);
            var DOSYear = actualDos.Year.ToString();
            var DOSYear2 = DOSYear.Substring(DOSYear.Length - 2, 2);

            using (var ms = new MemoryStream(bytes))
            {
                using (var pdfDoc = PdfSharp.Pdf.IO.PdfReader.Open(ms, PdfSharp.Pdf.IO.PdfDocumentOpenMode.ReadOnly))
                { 
                    for (int page = 0; page < pages; page++)
                    {
                        var pdfLinesData = new Dictionary<string, string>();
                        var partCounter = 0;
                        decimal totalClaimTotal = 0;
                        decimal claimTotal = 0;
                        var keys = lineItems.Keys.Skip(page * 6).Take(6);

                        foreach (var key in keys)
                        {
                            var billed = (decimal)lineItems[key]["BilledChg"];
                            var billedStr = billed.ToString("0.00").Split('.');
                            var hcpcsName = consolidateCodes ? key : (string)lineItems[key]["HcpcsName"];
                            var quantity = ((decimal)lineItems[key]["Quantity"]).ToString("0");
                            if (!string.Equals(_carrier.Id.ToString(), _crmService.GetGlobalSettingValueByKey("BCBS-Anthem-VA"), StringComparison.OrdinalIgnoreCase))
                            {
                                switch (partCounter)
                                {
                                    case 0:
                                        {
                                            pdfLinesData.Add("114", actualDos.Month.ToString());
                                            pdfLinesData.Add("117", actualDos.Month.ToString());
                                            pdfLinesData.Add("115", actualDos.Day.ToString());
                                            pdfLinesData.Add("118", actualDos.Day.ToString());
                                            pdfLinesData.Add("116", DOSYear2);
                                            pdfLinesData.Add("119", DOSYear2);
                                            pdfLinesData.Add("120", "24");
                                            pdfLinesData.Add("122", hcpcsName);
                                            pdfLinesData.Add("127", dxCodesDescription);
                                            pdfLinesData.Add("128", billedStr[0]);
                                            pdfLinesData.Add("129", billedStr[1]);
                                            pdfLinesData.Add("130", quantity);
                                            pdfLinesData.Add("132", renderingproviderid);
                                            break;
                                        }
                                    case 1:
                                        {
                                            pdfLinesData.Add("137", actualDos.Month.ToString());
                                            pdfLinesData.Add("140", actualDos.Month.ToString());
                                            pdfLinesData.Add("138", actualDos.Day.ToString());
                                            pdfLinesData.Add("141", actualDos.Day.ToString());
                                            pdfLinesData.Add("139", DOSYear2);
                                            pdfLinesData.Add("142", DOSYear2);
                                            pdfLinesData.Add("143", "24");
                                            pdfLinesData.Add("145", hcpcsName);
                                            pdfLinesData.Add("150", dxCodesDescription);
                                            pdfLinesData.Add("151", billedStr[0]);
                                            pdfLinesData.Add("152", billedStr[1]);
                                            pdfLinesData.Add("153", quantity);
                                            pdfLinesData.Add("155", renderingproviderid);
                                            break;
                                        }
                                    case 2:
                                        {
                                            pdfLinesData.Add("160", actualDos.Month.ToString());
                                            pdfLinesData.Add("163", actualDos.Month.ToString());
                                            pdfLinesData.Add("161", actualDos.Day.ToString());
                                            pdfLinesData.Add("164", actualDos.Day.ToString());
                                            pdfLinesData.Add("162", DOSYear2);
                                            pdfLinesData.Add("165", DOSYear2);
                                            pdfLinesData.Add("166", "24");
                                            pdfLinesData.Add("168", hcpcsName);
                                            pdfLinesData.Add("173", dxCodesDescription);
                                            pdfLinesData.Add("174", billedStr[0]);
                                            pdfLinesData.Add("175", billedStr[1]);
                                            pdfLinesData.Add("176", quantity);
                                            pdfLinesData.Add("178", renderingproviderid);
                                            break;
                                        }
                                    case 3:
                                        {
                                            pdfLinesData.Add("183", actualDos.Month.ToString());
                                            pdfLinesData.Add("186", actualDos.Month.ToString());
                                            pdfLinesData.Add("184", actualDos.Day.ToString());
                                            pdfLinesData.Add("187", actualDos.Day.ToString());
                                            pdfLinesData.Add("185", DOSYear2);
                                            pdfLinesData.Add("188", DOSYear2);
                                            pdfLinesData.Add("189", "24");
                                            pdfLinesData.Add("191", hcpcsName);
                                            pdfLinesData.Add("196", dxCodesDescription);
                                            pdfLinesData.Add("197", billedStr[0]);
                                            pdfLinesData.Add("198", billedStr[1]);
                                            pdfLinesData.Add("199", quantity);
                                            pdfLinesData.Add("201", renderingproviderid);
                                            break;
                                        }
                                    case 4:
                                        {
                                            pdfLinesData.Add("206", actualDos.Month.ToString());
                                            pdfLinesData.Add("209", actualDos.Month.ToString());
                                            pdfLinesData.Add("207", actualDos.Day.ToString());
                                            pdfLinesData.Add("210", actualDos.Day.ToString());
                                            pdfLinesData.Add("208", DOSYear2);
                                            pdfLinesData.Add("211", DOSYear2);
                                            pdfLinesData.Add("212", "24");
                                            pdfLinesData.Add("214", hcpcsName);
                                            pdfLinesData.Add("219", dxCodesDescription);
                                            pdfLinesData.Add("220", billedStr[0]);
                                            pdfLinesData.Add("221", billedStr[1]);
                                            pdfLinesData.Add("222", quantity);
                                            pdfLinesData.Add("224", renderingproviderid);
                                            break;
                                        }
                                    case 5:
                                        {
                                            pdfLinesData.Add("229", actualDos.Month.ToString());
                                            pdfLinesData.Add("232", actualDos.Month.ToString());
                                            pdfLinesData.Add("230", actualDos.Day.ToString());
                                            pdfLinesData.Add("233", actualDos.Day.ToString());
                                            pdfLinesData.Add("231", DOSYear2);
                                            pdfLinesData.Add("234", DOSYear2);
                                            pdfLinesData.Add("235", "24");
                                            pdfLinesData.Add("237", hcpcsName);
                                            pdfLinesData.Add("242", dxCodesDescription);
                                            pdfLinesData.Add("243", billedStr[0]);
                                            pdfLinesData.Add("244", billedStr[1]);
                                            pdfLinesData.Add("245", quantity);
                                            pdfLinesData.Add("247", renderingproviderid);
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                if (partCounter == 0)
                                {
                                    pdfLinesData.Add("114", actualDos.Month.ToString());
                                    pdfLinesData.Add("117", actualDos.Month.ToString());
                                    pdfLinesData.Add("115", actualDos.Day.ToString());
                                    pdfLinesData.Add("118", actualDos.Day.ToString());
                                    pdfLinesData.Add("116", DOSYear2);
                                    pdfLinesData.Add("119", DOSYear2);
                                    pdfLinesData.Add("120", posDescription);
                                    pdfLinesData.Add("122", "L8642");
                                    pdfLinesData.Add("127", dxCodesDescription);
                                    pdfLinesData.Add("130", "1");
                                    pdfLinesData.Add("132", renderingproviderid);
                                }
                            }

                            partCounter++;
                            claimTotal += billed;
                        }

                        var totalBilled = claimTotal.ToString("0.00").Split('.');

                        if (_carrier.Name.Equals(""))
                        {
                            if (pdfLinesData.ContainsKey("128"))
                            {
                                pdfLinesData["128"] = totalBilled[0];
                            }
                            else
                            {
                                pdfLinesData.Add("128", totalBilled[0]);
                            }

                            if (pdfLinesData.ContainsKey("129"))
                            {
                                pdfLinesData["129"] = totalBilled[1].PadLeft(2, '0');
                            }
                            else
                            {
                                pdfLinesData.Add("129", totalBilled[1].PadLeft(2, '0'));
                            }
                        }

                        pdfLinesData.Add("254", totalBilled[0]);
                        pdfLinesData.Add("255", totalBilled[1].PadLeft(2, '0'));
                        pdfLinesData.Add("258", totalBilled[0]);
                        pdfLinesData.Add("259", totalBilled[1].PadLeft(2, '0'));

                        AddPage(pdfDoc, bytes, pdfData.MergeWith(pdfLinesData), page);

                        totalClaimTotal += claimTotal;
                        partCounter = 0;
                        claimTotal = 0;
                    }

                    using (var ms2 = new MemoryStream())
                    {
                        var roraldocpages = pdfDoc.PageCount; // Cannot save a PDF document with no pages.

                        pdfDoc.Flatten();
                        pdfDoc.Save(ms2, false);

                        var base64 = Convert.ToBase64String(ms2.GetBuffer(), 0, (int)ms2.Length);

                        return base64;
                    }
                }
            }
        }

        private void AddPage(PdfDocument sourceDoc, byte[] template,  Dictionary<string, string> pdfData, int page)
        {
            if (page == 0)
            {
                if (sourceDoc.AcroForm.Elements.ContainsKey("/NeedAppearances"))
                {
                    sourceDoc.AcroForm.Elements["/NeedAppearances"] = new PdfBoolean(true);
                }
                else
                {
                    sourceDoc.AcroForm.Elements.Add("/NeedAppearances", new PdfBoolean(true));
                }
               
                var pdfFields = sourceDoc.AcroForm.Fields;
                
                MakeFieldsReadonly(pdfFields);

                MapFieldValues(pdfFields, pdfData);
            }
            else
            {
                var tempPdf = PdfSharp.Pdf.IO.PdfReader.Open(new MemoryStream(template), PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                var pdfFields = tempPdf.AcroForm.Fields;

                MakeFieldsReadonly(pdfFields);

                MapFieldValues(pdfFields, pdfData);
                RenameFields(pdfFields, page);

                sourceDoc.AddPage(tempPdf.Pages[0]);

                tempPdf.Close();
            }
        }

        private void MakeFieldsReadonly(PdfAcroField.PdfAcroFieldCollection pdfFields)
        {
            for (int i = 0; i < pdfFields.Count; i++)
            {
                var field = pdfFields[i];

                if(field.HasKids)
                {
                    MakeFieldsReadonly(field.Fields);
                }
                else
                {
                    field.ReadOnly = true;
                }
            }
        }

        private void MapFieldValues(PdfAcroField.PdfAcroFieldCollection pdfFields, Dictionary<string, string> pdfData)
        {
            try
            {
                foreach (var item in pdfData)
                {
                    var pdfField = pdfFields[item.Key];

                    if (pdfField != null)
                    {

                        if (pdfField.ReadOnly)
                        {
                            pdfField.ReadOnly = false;
                        }

                        if (pdfField is PdfCheckBoxField)
                        {
                            ((PdfCheckBoxField)pdfField).Checked = item.Value.Equals("Yes");
                        }
                        else if (pdfField is PdfTextField)
                        {
                            pdfField.Value = new PdfString(item.Value);
                        }
                        
                        pdfField.ReadOnly = true;
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static void RenameFields(PdfAcroField.PdfAcroFieldCollection pdfFields, int page)
        {
            var names = pdfFields.Names;

            for (int i = 0; i < names.Length; i++)
            {
                var fieldName = names[i];
                var field = pdfFields[fieldName];

                field.Elements.SetString("/T", $"{fieldName}_page{page}");
            }

        }

        private List<ipg_dxcode> GetDxCodesForCase(params EntityReference[] dxCodeRefs)
        {
            var query = new QueryExpression(ipg_dxcode.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(ipg_dxcode.Fields.ipg_ICDVersion, ipg_dxcode.PrimaryNameAttribute, ipg_dxcode.Fields.ipg_DxCode),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_dxcode.PrimaryIdAttribute, ConditionOperator.In, dxCodeRefs.Where(d=>d!= null).Select(d=>d.Id).ToArray())
                    }
                }
            };

            return _crmService.RetrieveMultiple(query).Entities.Cast<ipg_dxcode>().ToList();
        }

        private string GetRelationToInsuredPresentation(OptionSetValue relationtoInsured)
        {
            var result = string.Empty;
            if(relationtoInsured != null)
            {
                switch (relationtoInsured.Value)
                {
                    case (int)new_RelationtoInsured.Self:
                        result = "18";
                        break;
                    case (int)new_RelationtoInsured.Spouse:
                        result = "01";
                        break;
                    case (int)new_RelationtoInsured.Child:
                        result = "19";
                        break;
                    case (int)new_RelationtoInsured.Other:
                        result = "21";
                        break;
                }
            }
            return result;
        }

        private PersonInfo GetPatientInfo()
        {
            var patientInfo = new PersonInfo();

            patientInfo.FirstName = _incident.ipg_PatientFirstName ?? string.Empty;
            patientInfo.LastName = _incident.ipg_PatientLastName ?? string.Empty;
            patientInfo.MI = _incident.ipg_PatientMiddleName ?? string.Empty;
            patientInfo.DOB = _incident.ipg_PatientDateofBirth;

            var zipCode = _incident.ipg_CasePatientZipCodeId != null ? _crmService.Retrieve(_incident.ipg_CasePatientZipCodeId.LogicalName, _incident.ipg_CasePatientZipCodeId.Id,
                new ColumnSet(ipg_melissazipcode.Fields.ipg_name, ipg_melissazipcode.Fields.ipg_city, ipg_melissazipcode.Fields.ipg_state))
                .ToEntity<ipg_melissazipcode>() : null;

            if(zipCode == null)
            {
                throw new Exception("Patient Zip Code is Empty!");
            }

            patientInfo.ZipCode = zipCode.ipg_name;
            patientInfo.Address = (_incident.ipg_PatientAddress ?? string.Empty).Replace('\r', ' ').Replace('\n', ' ');
            patientInfo.City = zipCode.ipg_city;
            patientInfo.State = zipCode.ipg_state;
            patientInfo.StateAbbr = zipCode.ipg_state;
            patientInfo.Phone = (_incident.ipg_PatientHomePhone ?? _incident.ipg_PatientCellPhone ?? _incident.ipg_PatientWorkPhone)
                  .Replace("-", "").Replace(".", "").Replace("(", "").Replace(")", "");
            patientInfo.Gender = _incident.ipg_PatientGenderEnum;

            return patientInfo;
        }

        private PersonInfo GetOtherInsuredInfoFromCarrier(EntityReference entityReference)
        {
            if(_incident.ipg_CarrierId != entityReference)
            {
                return GetInsuredInfoFromCarrier(_incident.ipg_CarrierId);
            }
            else
            {
                return GetInsuredInfoFromCarrier(_incident.ipg_SecondaryCarrierId);
            }
        }

        private PersonInfo GetInsuredInfoFromCarrier(EntityReference carrier)
        {
            var insuredInfo = new PersonInfo();
            
            if(carrier == null)
            {
                return null;
            }
            else if(carrier == _incident.ipg_CarrierId)
            {
                if(_incident.ipg_RelationToInsuredEnum == new_RelationtoInsured.Self)
                {
                    insuredInfo = GetPatientInfo();
                }
                else
                {
                    insuredInfo.FirstName = _incident.ipg_InsuredFirstName;
                    insuredInfo.LastName = _incident.ipg_InsuredLastName;
                    insuredInfo.MI = _incident.ipg_InsuredMiddleName;
                    insuredInfo.DOB = _incident.ipg_InsuredDateOfBirth;

                    var zipCode = _incident.ipg_caseinsuredzipcodeId != null ? _crmService.Retrieve(_incident.ipg_caseinsuredzipcodeId.LogicalName, _incident.ipg_caseinsuredzipcodeId.Id,
                    new ColumnSet(ipg_melissazipcode.Fields.ipg_name, ipg_melissazipcode.Fields.ipg_city, ipg_melissazipcode.Fields.ipg_state))
                    .ToEntity<ipg_melissazipcode>() : null;

                    if (zipCode == null)
                    {
                        throw new Exception("Insured Zip Code is Empty!");
                    }

                    insuredInfo.Address = (_incident.ipg_insuredaddress ?? string.Empty).Replace('\r', ' ').Replace('\n', ' ');
                    insuredInfo.City = zipCode.ipg_city;
                    insuredInfo.State = zipCode.ipg_state;
                    insuredInfo.StateAbbr = zipCode.ipg_state;
                    insuredInfo.ZipCode = zipCode.ipg_name;
                    insuredInfo.Phone = _incident.ipg_insuredphone?.Replace("-", "").Replace(".", "").Replace("(", "").Replace(")", "");
                    insuredInfo.Gender = _incident.ipg_InsuredGenderEnum;
                }
                
                insuredInfo.MemberIdNumber = _incident.ipg_MemberIdNumber;
                insuredInfo.GroupNumber = _incident.ipg_primarycarriergroupidnumber;
            }
            else if(carrier == _incident.ipg_SecondaryCarrierId)
            {

                if (_incident.ipg_SecondaryCarrierRelationToInsuredEnum == new_RelationtoInsured.Self)
                {
                    insuredInfo = GetPatientInfo();
                }
                else
                {
                    var zipCode = _incident.ipg_melissacsecondaryZipCodeId != null ? _crmService.Retrieve(_incident.ipg_melissacsecondaryZipCodeId.LogicalName, _incident.ipg_melissacsecondaryZipCodeId.Id,
                      new ColumnSet(ipg_melissazipcode.Fields.ipg_name, ipg_melissazipcode.Fields.ipg_city, ipg_melissazipcode.Fields.ipg_state))
                      .ToEntity<ipg_melissazipcode>() : null;

                    if (zipCode == null)
                    {
                        throw new Exception("Secondary Insured Zip Code is Empty!");
                    }

                    insuredInfo.FirstName = _incident.ipg_secondaryinsuredfirstname;
                    insuredInfo.LastName = _incident.ipg_secondaryinsuredlastname;
                    insuredInfo.MI = _incident.ipg_secondaryinsuredmiddlename;
                    insuredInfo.DOB = _incident.ipg_secondaryinsureddateofbirth;

                    insuredInfo.Address = _incident.ipg_secondaryinsuredaddress?.Replace('\r', ' ').Replace('\n', ' ');
                    insuredInfo.City = zipCode.ipg_city;
                    insuredInfo.State = zipCode.ipg_state;
                    insuredInfo.StateAbbr = zipCode.ipg_state;
                    insuredInfo.ZipCode = zipCode.ipg_name;
                    insuredInfo.Phone = _incident.ipg_secondaryinsuredphone?.Replace("-", "").Replace(".", "").Replace("(", "").Replace(")", "");
                    insuredInfo.Gender = _incident.ipg_secondaryinsuredgenderEnum;
                }

                insuredInfo.MemberIdNumber = _incident.ipg_SecondaryMemberIdNumber;
                insuredInfo.GroupNumber = _incident.ipg_SecondaryCarrierGroupIdNumber;
            }

            return insuredInfo;
        }
        private LocationInfo GetProviderInfo()
        {
            var locationInfo = new LocationInfo();

            switch (_carrier.ipg_ClaimsServicingProviderNameEnum)
            {
                case ipg_ClaimServicingProviderNames.Facility:
                    locationInfo.LocationName = Facility.Name;
                    break;
                case ipg_ClaimServicingProviderNames.IPG:
                default:
                    locationInfo.LocationName = "Implantable Provider Group";
                    break;
            }

            switch (_carrier.ipg_ClaimsServicingProviderAddressEnum)
            {
                case ipg_ClaimServicingProviderAddresses.Facility:
                    {
                        locationInfo.Address = Facility.Address1_Line1 ?? string.Empty;
                        if (Facility.ipg_ZipCodeId != null)
                        {
                            var zipCode = _crmService.Retrieve(Facility.ipg_ZipCodeId.LogicalName, Facility.ipg_ZipCodeId.Id
                                , new ColumnSet(ipg_zipcode.Fields.ipg_City, ipg_zipcode.Fields.ipg_StateId, ipg_zipcode.Fields.ipg_name)).ToEntity<ipg_zipcode>();
                            locationInfo.City = zipCode.ipg_City;
                            locationInfo.State = zipCode.ipg_StateId != null ? zipCode.ipg_StateId.Name : string.Empty;
                            locationInfo.ZipCode = zipCode.ipg_name;
                        }
                        break;
                    }
                case ipg_ClaimServicingProviderAddresses.Other:
                    {
                        locationInfo.Address = _carrier?.Address2_Line2;
                        locationInfo.City = _carrier?.Address2_City; ;
                        locationInfo.State = _carrier?.Address2_StateOrProvince;
                        locationInfo.ZipCode = _carrier?.ipg_claimspostalcode?.Name; 
                        break;
                    }
                default:
                    {
                        var ipgAddress = D365Helpers.GetIPGAddressFromConfiguration(_crmService);

                        locationInfo.Address = ipgAddress.Item4;
                        locationInfo.City = ipgAddress.Item3;
                        locationInfo.State = ipgAddress.Item2;
                        locationInfo.ZipCode = ipgAddress.Item1;
                        break;
                    }
            }

            return locationInfo;
        }

        private string BuildClaimNumber()
        {
            string caseNumber = _incident.Title;

            bool isPrimaryClaim = _request.IsPrimaryOrSecondaryClaim && _isAutoCarrierGeneration == false;
            string primaryOrSecondaryCode = isPrimaryClaim ? "1" : "2";

            //get new claim index
            int newClaimIndex;
            var lastActiveClaim = isPrimaryClaim ? LastPrimaryClaim : LastSecondaryClaim;
            if (_request.GenerateClaimFlag && lastActiveClaim != null)
            {
                string lastActiveClaimName = lastActiveClaim.Name;
                Int16.TryParse(lastActiveClaimName.Substring(lastActiveClaimName.Length - 1), out short finalDigit);
                newClaimIndex = _request.GenerateClaimFlag ? (finalDigit + 1) : finalDigit;
            }
            else
            {
                newClaimIndex = 1;
            }

            return $"{caseNumber}{primaryOrSecondaryCode}{newClaimIndex}";
        }
    }
}