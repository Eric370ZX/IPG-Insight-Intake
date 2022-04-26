"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.SetBenefitExhaust = void 0;
function SetBenefitExhaust(executionContext) {
    var formContext = executionContext.getFormContext();
    var a = localStorage.ExhaustedBenefits;
    if (a === "1") {
        formContext.getAttribute("ipg_autobenefitsexhausted").setValue(true);
    }
    else {
        formContext.getAttribute("ipg_autobenefitsexhausted").setValue(false);
    }
    localStorage.removeItem('ExhaustedBenefits');
    localStorage.clear();
}
exports.SetBenefitExhaust = SetBenefitExhaust;
