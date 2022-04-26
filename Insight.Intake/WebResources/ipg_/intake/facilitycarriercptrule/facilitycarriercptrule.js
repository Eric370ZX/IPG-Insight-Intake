/**
 * @namespace Intake.FacilityCarrierCPTRule
 */
var Intake;
(function (Intake) {
    var FacilityCarrierCPTRule;
    (function (FacilityCarrierCPTRule) {
        /**
          * Called on Form Save event
          * @function Intake.FacilityCarrierCPTRule.OnSaveForm
          * @returns {void}
        */
        function OnSaveForm(executionContext) {
            var formContext = executionContext.getFormContext();
            updateName(formContext);
            if (CheckConflictingRule(formContext) || CheckDateRange(formContext)) {
                var saveEventArgs = executionContext.getEventArgs();
                saveEventArgs.preventDefault();
            }
        }
        FacilityCarrierCPTRule.OnSaveForm = OnSaveForm;
        function updateName(formContext) {
            if (formContext.getAttribute("ipg_cptid").getValue() != null) {
                var cptCodeAttrbValue = formContext.getAttribute("ipg_cptid").getValue();
                if (cptCodeAttrbValue) {
                    var cptCodeId = cptCodeAttrbValue[0].id;
                    if (cptCodeId)
                        cptCodeId = cptCodeId.slice(1, -1);
                    if (cptCodeId != null) {
                        Xrm.WebApi.retrieveRecord("ipg_cptcode", cptCodeId, "?$select=ipg_cptcode").then(function success(result) {
                            if (result.ipg_cptcode) {
                                formContext.getAttribute("ipg_name").setValue(result.ipg_cptcode);
                            }
                        }, function (error) {
                            console.log(error.message);
                            // handle error conditions
                        });
                    }
                }
            }
        }
        function OnLoadForm(executionContext) {
            var formContext = executionContext.getFormContext();
            var FacilityCarrierCPTRuleContractId = 'ipg_facilitycarriercptrulecontractid';
            if (formContext.ui.getFormType() == 1) { //Create
                var facilityCarrierCPTRuleContractAttrbValue = formContext.getAttribute(FacilityCarrierCPTRuleContractId).getValue();
                if (facilityCarrierCPTRuleContractAttrbValue) {
                    var facilityCarrierCPTContract = facilityCarrierCPTRuleContractAttrbValue[0].id;
                    if (facilityCarrierCPTContract)
                        facilityCarrierCPTContract = facilityCarrierCPTContract.slice(1, -1);
                    if (facilityCarrierCPTContract != null) {
                        Xrm.WebApi.retrieveRecord("ipg_facilitycarriercptrulecontract", facilityCarrierCPTContract, "?$select=ipg_effectivedate,ipg_expirationdate").then(function success(result) {
                            if (result.ipg_effectivedate) {
                                formContext.getAttribute("ipg_effectivedate").setValue(new Date(result.ipg_effectivedate));
                            }
                            if (result.ipg_expirationdate) {
                                formContext.getAttribute("ipg_expirationdate").setValue(new Date(result.ipg_expirationdate));
                            }
                            if (result.ipg_expirationdate <= result.ipg_effectivedate) {
                                formContext.getControl("ipg_expirationdate").setNotification("Effective Date cannot be equal to greater than Expiration Date");
                            }
                        }, function (error) {
                            console.log(error.message);
                            // handle error conditions
                        });
                    }
                }
            }
        }
        FacilityCarrierCPTRule.OnLoadForm = OnLoadForm;
        /**
          * Called on change Effective Date
          * @function Intake.FacilityCarrierCPTRule.OnChangeEffectiveDate
          * @returns {void}
        */
        function OnChangeEffectiveDate(executionContext) {
            var formContext = executionContext.getFormContext();
            CheckConflictingRule(formContext);
            CheckDateRange(formContext);
        }
        FacilityCarrierCPTRule.OnChangeEffectiveDate = OnChangeEffectiveDate;
        /**
        * Called on change Expiration Date
        * @function Intake.FacilityCarrierCPTRule.OnChangeExpirationDate
        * @returns {void}
      */
        function OnChangeExpirationDate(executionContext) {
            var formContext = executionContext.getFormContext();
            CheckConflictingRule(formContext);
            CheckDateRange(formContext);
        }
        FacilityCarrierCPTRule.OnChangeExpirationDate = OnChangeExpirationDate;
        /**
      * Called on change Expiration Date
      * @function Intake.FacilityCarrierCPTRule.OnChangeFacilityCarrierCPTRuleContract
      * @returns {void}
      */
        function OnChangeFacilityCarrierCPTRuleContract(executionContext) {
            var formContext = executionContext.getFormContext();
            CheckConflictingRule(formContext);
            CheckDateRange(formContext);
        }
        FacilityCarrierCPTRule.OnChangeFacilityCarrierCPTRuleContract = OnChangeFacilityCarrierCPTRuleContract;
        /**
      * Called on change type
      * @function Intake.FacilityCarrierCPTRule.OnChangeCPTCode
      * @returns {void}
      */
        function OnChangeCPTCode(executionContext) {
            var formContext = executionContext.getFormContext();
            updateName(formContext);
            CheckConflictingRule(formContext);
            CheckDateRange(formContext);
        }
        FacilityCarrierCPTRule.OnChangeCPTCode = OnChangeCPTCode;
        function getODataUTCDateFilter(date) {
            var monthString;
            var rawMonth = (date.getUTCMonth() + 1).toString();
            if (rawMonth.length == 1) {
                monthString = "0" + rawMonth;
            }
            else {
                monthString = rawMonth;
            }
            var dateString;
            var rawDate = date.getUTCDate().toString();
            if (rawDate.length == 1) {
                dateString = "0" + rawDate;
            }
            else {
                dateString = rawDate;
            }
            var DateFilter = "";
            DateFilter += date.getUTCFullYear() + "-";
            DateFilter += monthString + "-";
            DateFilter += dateString;
            return DateFilter;
        }
        /**
        * Shows warning if conflicting rule exists.
        * @function Intake.FacilityCarrierCPTRule.CheckConflictingRule
        * @returns {void}
        */
        function CheckConflictingRule(formContext) {
            var EffectiveDateField = 'ipg_effectivedate';
            var ExpirationDateField = 'ipg_expirationdate';
            var FacilityCarrierCPTRuleContractId = 'ipg_facilitycarriercptrulecontractid';
            var CPTCodeId = 'ipg_cptid';
            if (formContext.getAttribute(FacilityCarrierCPTRuleContractId).getValue() == null || formContext.getAttribute(EffectiveDateField).getValue() == null || formContext.getAttribute(CPTCodeId).getValue() == null)
                return;
            var cptId = formContext.getAttribute(CPTCodeId).getValue()[0].id;
            if (cptId)
                cptId = cptId.slice(1, -1);
            var facilityCarrierCPTContract = formContext.getAttribute(FacilityCarrierCPTRuleContractId).getValue()[0].id;
            if (facilityCarrierCPTContract)
                facilityCarrierCPTContract = facilityCarrierCPTContract.slice(1, -1);
            var effectiveDate = formContext.getAttribute(EffectiveDateField).getValue();
            var effectiveDateFormat = getODataUTCDateFilter(effectiveDate);
            var expirationDate = formContext.getAttribute(ExpirationDateField).getValue();
            if (expirationDate == null)
                expirationDate = new Date(9998, 12, 31);
            var expirationDateFormat = getODataUTCDateFilter(expirationDate);
            var query = "?$select=ipg_name&$filter=_ipg_facilitycarriercptrulecontractid_value eq " + facilityCarrierCPTContract + "\n                 and _ipg_cptid_value eq " + cptId + "\n                 and " + ExpirationDateField + " ge " + effectiveDateFormat + "\n                 and " + EffectiveDateField + " le " + expirationDateFormat;
            var facilityCPTRuleId = Xrm.Page.data.entity.getId();
            if (facilityCPTRuleId) {
                facilityCPTRuleId = facilityCPTRuleId.slice(1, -1);
                query += " and ipg_facilitycarriercptruleid ne " + facilityCPTRuleId;
            }
            Xrm.WebApi.retrieveMultipleRecords('ipg_facilitycarriercptrule', query).then(function (result) {
                if (result.entities.length) {
                    if (result.entities.length >= 1) {
                        formContext.getControl(CPTCodeId).setNotification("There is a conflict with record " + result.entities[0].ipg_name + ".", CPTCodeId);
                        return true;
                    }
                }
                formContext.getControl(CPTCodeId).clearNotification();
            }, function (error) {
                Xrm.Navigation.openErrorDialog({ message: error.message });
            });
            return false;
        }
        /**
        * Shows warning if rule date range is outside Facility Carrier CPT Contract's date range.
        * @function Intake.FacilityCarrierCPTRule.CheckDateRange
        * @returns {void}
        */
        function CheckDateRange(formContext) {
            var EffectiveDateField = 'ipg_effectivedate';
            var ExpirationDateField = 'ipg_expirationdate';
            var FacilityCarrierCPTRuleContractId = 'ipg_facilitycarriercptrulecontractid';
            if (formContext.getAttribute(FacilityCarrierCPTRuleContractId).getValue() == null ||
                formContext.getAttribute(EffectiveDateField).getValue() == null ||
                formContext.getAttribute(ExpirationDateField).getValue() == null)
                return;
            var facilityCarrierCPTRuleContract = formContext.getAttribute(FacilityCarrierCPTRuleContractId).getValue()[0].id;
            if (facilityCarrierCPTRuleContract)
                facilityCarrierCPTRuleContract = facilityCarrierCPTRuleContract.slice(1, -1);
            var effectiveDate = formContext.getAttribute(EffectiveDateField).getValue();
            var expirationDate = formContext.getAttribute(ExpirationDateField).getValue();
            if (expirationDate == null)
                expirationDate = new Date(9998, 12, 31);
            if (expirationDate <= effectiveDate) {
                formContext.getControl("ipg_expirationdate").setNotification("Effective Date cannot be equal to greater than Expiration Date");
            }
            Xrm.WebApi.retrieveRecord('ipg_facilitycarriercptrulecontract', facilityCarrierCPTRuleContract, "?$select=ipg_effectivedate,ipg_expirationdate").then(function (result) {
                var startDate = new Date(result.startdate);
                startDate.setHours(0, 0, 0, 0);
                var endDate = new Date(result.enddate);
                endDate.setHours(0, 0, 0, 0);
                if (startDate > effectiveDate) {
                    formContext.getControl(EffectiveDateField).setNotification("Effective date is before Contracts's start date.", EffectiveDateField);
                    return true;
                }
                else if (endDate < expirationDate) {
                    formContext.getControl(ExpirationDateField).setNotification("Expiration date is after Contracts's end date.", ExpirationDateField);
                    return true;
                }
                else {
                    formContext.getControl(EffectiveDateField).clearNotification(EffectiveDateField);
                    formContext.getControl(ExpirationDateField).clearNotification(ExpirationDateField);
                    return false;
                }
            }, function (error) {
                Xrm.Navigation.openErrorDialog({ message: error.message });
            });
            return false;
        }
    })(FacilityCarrierCPTRule = Intake.FacilityCarrierCPTRule || (Intake.FacilityCarrierCPTRule = {}));
})(Intake || (Intake = {}));
