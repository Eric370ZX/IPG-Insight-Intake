/**
 * @namespace Intake.DxCode
 */
var Intake;
(function (Intake) {
    var DxCode;
    (function (DxCode) {
        function OnLoadForm(executionContext) {
            var formContext = executionContext.getFormContext();
            if (formContext.ui.getFormType() == 1) {
                SetDXCodeDates(formContext);
                ClearAndDisableDxCodeField(formContext, false);
            }
            else {
                ClearAndDisableDxCodeField(formContext, true);
            }
        }
        DxCode.OnLoadForm = OnLoadForm;
        function SetDXCodeDates(formContext) {
            var _a, _b;
            var effectiveDate = new Date();
            var expirationDate = new Date('12/31/9999');
            (_a = formContext.getAttribute("ipg_effectivedate")) === null || _a === void 0 ? void 0 : _a.setValue(effectiveDate);
            (_b = formContext.getAttribute("ipg_expirationdate")) === null || _b === void 0 ? void 0 : _b.setValue(expirationDate);
        }
        function ClearAndDisableDxCodeField(formContext, isDisabled) {
            var _a, _b;
            if (isDisabled) {
                (_a = formContext.getControl("ipg_dxcode")) === null || _a === void 0 ? void 0 : _a.setDisabled(isDisabled);
            }
            else {
                //  formContext.getAttribute("ipg_dxcode")?.setValue(null);
                (_b = formContext.getControl("ipg_dxcode")) === null || _b === void 0 ? void 0 : _b.setDisabled(isDisabled);
            }
        }
    })(DxCode = Intake.DxCode || (Intake.DxCode = {}));
})(Intake || (Intake = {}));
