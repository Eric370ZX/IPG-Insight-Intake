/**
 * @namespace Intake.FacilityCarrierCPTRuleContract
 */
var Intake;
(function (Intake) {
    var FacilityCarrierCPTRuleContract;
    (function (FacilityCarrierCPTRuleContract) {
        var todayDate = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
        var effectiveDate = todayDate;
        /**
          * Called on Form Save event
          * @function Intake.FacilityCarrierCPTRuleContract.OnSaveForm
          * @returns {void}
        */
        function OnSaveForm(executionContext) {
            var formContext = executionContext.getFormContext();
            if (CheckConflictingRule(formContext) || CheckDateRange(formContext)) {
                var saveEventArgs = executionContext.getEventArgs();
                saveEventArgs.preventDefault();
            }
            else {
                updateName(formContext);
            }
        }
        FacilityCarrierCPTRuleContract.OnSaveForm = OnSaveForm;
        function updateName(formContext) {
            if (formContext.getAttribute("ipg_entitlementid").getValue() != null &&
                formContext.getAttribute("ipg_effectivedate").getValue() != null &&
                formContext.getAttribute("ipg_expirationdate").getValue() != null) {
                var entitlementName = formContext.getAttribute("ipg_entitlementid").getValue()[0].name;
                var effectiveDate = formContext.getAttribute("ipg_effectivedate").getValue();
                var expirationDate = formContext.getAttribute("ipg_expirationdate").getValue();
                formContext.getAttribute("ipg_name").setValue(entitlementName + " - " + getODataUTCDateFilter(effectiveDate) + " - " + getODataUTCDateFilter(expirationDate));
            }
        }
        /**
        * Called on Form Load event
        * @function Intake.FacilityCarrierCPTRuleContract.OnLoadForm
        * @returns {void}
        */
        function OnLoadForm(executionContext) {
            var formContext = executionContext.getFormContext();
            var EntitlementId = 'ipg_entitlementid';
            if (formContext.ui.getFormType() == 1) { //Create
                var entitlementAttrbValue = formContext.getAttribute(EntitlementId).getValue();
                if (entitlementAttrbValue) {
                    var entitlementId = entitlementAttrbValue[0].id;
                    if (entitlementId)
                        entitlementId = entitlementId.slice(1, -1);
                    if (entitlementId != null) {
                        Xrm.WebApi.retrieveRecord("entitlement", entitlementId, "?$select=startdate,enddate").then(function success(result) {
                            if (result.startdate) {
                                formContext.getAttribute("ipg_effectivedate").setValue(new Date(result.startdate));
                            }
                            if (result.enddate) {
                                formContext.getAttribute("ipg_expirationdate").setValue(new Date(result.enddate));
                            }
                        }, function (error) {
                            console.log(error.message);
                            // handle error conditions
                        });
                    }
                }
            }
        }
        FacilityCarrierCPTRuleContract.OnLoadForm = OnLoadForm;
        function updateDate(executionContext) {
            var formContext = executionContext.getFormContext();
            if (formContext.getAttribute("ipg_effectivedate").getValue() != null) {
                var expirationDate_1 = formContext.getAttribute("ipg_expirationdate").getValue();
                var localEffectiveDate = formContext.getAttribute("ipg_effectivedate").getValue();
                if (localEffectiveDate === null) {
                    localEffectiveDate = effectiveDate;
                }
                if (function (localEffectiveDate) { return expirationDate_1; }) {
                    formContext.getControl("ipg_expirationdate").setNotification("Effective Date cannot be equal to greater than Expiration Date");
                }
            }
        }
        FacilityCarrierCPTRuleContract.updateDate = updateDate;
        /**
          * Called on change Effective Date
          * @function Intake.FacilityCarrierCPTRuleContract.OnChangeEffectiveDate
          * @returns {void}
        */
        function OnChangeEffectiveDate(executionContext) {
            var formContext = executionContext.getFormContext();
            CheckConflictingRule(formContext);
            CheckDateRange(formContext);
        }
        FacilityCarrierCPTRuleContract.OnChangeEffectiveDate = OnChangeEffectiveDate;
        /**
        * Called on change Expiration Date
        * @function Intake.FacilityCarrierCPTRuleContract.OnChangeExpirationDate
        * @returns {void}
      */
        function OnChangeExpirationDate(executionContext) {
            var formContext = executionContext.getFormContext();
            CheckConflictingRule(formContext);
            CheckDateRange(formContext);
        }
        FacilityCarrierCPTRuleContract.OnChangeExpirationDate = OnChangeExpirationDate;
        /**
      * Called on change Expiration Date
      * @function Intake.FacilityCarrierCPTRuleContract.OnChangeEntitlement
      * @returns {void}
      */
        function OnChangeEntitlement(executionContext) {
            var formContext = executionContext.getFormContext();
            CheckConflictingRule(formContext);
            CheckDateRange(formContext);
        }
        FacilityCarrierCPTRuleContract.OnChangeEntitlement = OnChangeEntitlement;
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
        * @function Intake.FacilityCarrierCPTRuleContract.CheckConflictingRule
        * @returns {void}
        */
        function CheckConflictingRule(formContext) {
            var EffectiveDateField = 'ipg_effectivedate';
            var ExpirationDateField = 'ipg_expirationdate';
            var EntitlementId = 'ipg_entitlementid';
            if (formContext.getAttribute(EntitlementId).getValue() == null || formContext.getAttribute(EffectiveDateField).getValue() == null)
                return;
            var entitlementId = formContext.getAttribute(EntitlementId).getValue()[0].id;
            if (entitlementId)
                entitlementId = entitlementId.slice(1, -1);
            var effectiveDate = formContext.getAttribute(EffectiveDateField).getValue();
            var effectiveDateFormat = getODataUTCDateFilter(effectiveDate);
            var expirationDate = formContext.getAttribute(ExpirationDateField).getValue();
            if (expirationDate == null)
                expirationDate = new Date(9998, 12, 31);
            var expirationDateFormat = getODataUTCDateFilter(expirationDate);
            var query = "?$select=ipg_name&$filter=_ipg_entitlementid_value eq " + entitlementId + "\n                 and " + ExpirationDateField + " ge " + effectiveDateFormat + "\n                 and " + EffectiveDateField + " le " + expirationDateFormat;
            var facilityCPTRuleContractId = Xrm.Page.data.entity.getId();
            if (facilityCPTRuleContractId) {
                facilityCPTRuleContractId = facilityCPTRuleContractId.slice(1, -1);
                query += " and ipg_facilitycarriercptrulecontractid ne " + facilityCPTRuleContractId;
            }
            Xrm.WebApi.retrieveMultipleRecords('ipg_facilitycarriercptrulecontract', query).then(function (result) {
                if (result.entities.length) {
                    if (result.entities.length >= 1) {
                        formContext.getControl(EntitlementId).setNotification("There is a conflict with record " + result.entities[0].ipg_name + ".", EntitlementId);
                        return true;
                    }
                }
                formContext.getControl(EntitlementId).clearNotification();
            }, function (error) {
                Xrm.Navigation.openErrorDialog({ message: error.message });
            });
            return false;
        }
        /**
        * Shows warning if rule date range is outside entitlement's date range.
        * @function Intake.FacilityCarrierCPTRuleContract.CheckDateRange
        * @returns {void}
        */
        function CheckDateRange(formContext) {
            var EffectiveDateField = 'ipg_effectivedate';
            var ExpirationDateField = 'ipg_expirationdate';
            var EntitlementId = 'ipg_entitlementid';
            if (formContext.getAttribute(EntitlementId).getValue() == null ||
                formContext.getAttribute(EffectiveDateField).getValue() == null ||
                formContext.getAttribute(ExpirationDateField).getValue() == null)
                return;
            var entitlementId = formContext.getAttribute(EntitlementId).getValue()[0].id;
            if (entitlementId)
                entitlementId = entitlementId.slice(1, -1);
            var effectiveDate = formContext.getAttribute(EffectiveDateField).getValue();
            var expirationDate = formContext.getAttribute(ExpirationDateField).getValue();
            if (expirationDate == null)
                expirationDate = new Date(9998, 12, 31);
            Xrm.WebApi.retrieveRecord('entitlement', entitlementId, "?$select=startdate,enddate").then(function (result) {
                var startDate = new Date(result.startdate);
                startDate.setHours(0, 0, 0, 0);
                var endDate = new Date(result.enddate);
                endDate.setHours(0, 0, 0, 0);
                if (startDate > effectiveDate) {
                    formContext.getControl(EffectiveDateField).setNotification("Effective date is before Entitlement's start date.", EffectiveDateField);
                    return true;
                }
                else if (endDate < expirationDate) {
                    formContext.getControl(ExpirationDateField).setNotification("Expiration date is after Entitlement's end date.", ExpirationDateField);
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
    })(FacilityCarrierCPTRuleContract = Intake.FacilityCarrierCPTRuleContract || (Intake.FacilityCarrierCPTRuleContract = {}));
})(Intake || (Intake = {}));
