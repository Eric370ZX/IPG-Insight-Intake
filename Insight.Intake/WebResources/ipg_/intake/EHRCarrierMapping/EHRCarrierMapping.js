/**
* @namespace Intake.EHRCarrierMapping
*/
var Intake;
(function (Intake) {
    var EHRCarrierMapping;
    (function (EHRCarrierMapping) {
        /**
         * Called on load form
         * @function Intake.EHRCarrierMapping.OnLoadForm
         * @returns {void}
        */
        function OnLoadForm(executionContext) {
            var formContext = executionContext.getFormContext();
            if (formContext.ui.getFormType() == 1) { //create form type
                setDisabledFields(formContext, false);
            }
            var effectiveDateAttr = formContext.getAttribute("ipg_effectivedate");
            effectiveDateAttr.addOnChange(function () {
                compareEffectiveAndExpirationDates(formContext);
            });
            var expirationDateAttr = formContext.getAttribute("ipg_expirationdate");
            expirationDateAttr.addOnChange(function () {
                compareEffectiveAndExpirationDates(formContext);
            });
        }
        EHRCarrierMapping.OnLoadForm = OnLoadForm;
        function OnSave(executionContext) {
            var formContext = executionContext.getFormContext();
            setDisabledFields(formContext, true);
        }
        EHRCarrierMapping.OnSave = OnSave;
        function setDisabledFields(formContext, isDisabled) {
            var fieldNames = ["ipg_name", "ipg_carrierposition", "ipg_facilityid"];
            fieldNames.forEach(function (fieldName) {
                var _a;
                (_a = formContext.getControl(fieldName)) === null || _a === void 0 ? void 0 : _a.setDisabled(isDisabled);
            });
        }
        function compareEffectiveAndExpirationDates(formContext) {
            var _a, _b, _c, _d;
            var effectiveDate = (_a = formContext.getAttribute("ipg_effectivedate")) === null || _a === void 0 ? void 0 : _a.getValue();
            var expirationDate = (_b = formContext.getAttribute("ipg_expirationdate")) === null || _b === void 0 ? void 0 : _b.getValue();
            if (effectiveDate && expirationDate && effectiveDate > expirationDate) {
                (_c = formContext.getControl("ipg_effectivedate")) === null || _c === void 0 ? void 0 : _c.setNotification("Effective Date should be earlier than Expiration Date.", "date");
            }
            else {
                (_d = formContext.getControl("ipg_effectivedate")) === null || _d === void 0 ? void 0 : _d.clearNotification("date");
            }
        }
    })(EHRCarrierMapping = Intake.EHRCarrierMapping || (Intake.EHRCarrierMapping = {}));
})(Intake || (Intake = {}));
