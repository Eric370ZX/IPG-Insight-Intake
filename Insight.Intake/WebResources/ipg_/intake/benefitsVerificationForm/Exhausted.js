"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.AutoBenefitsExhausted = void 0;
function AutoBenefitsExhausted(executionContext) {
    var formContext = executionContext.getFormContext();
    var SetExhaustedBenefits = formContext.getAttribute("ipg_autobenefitsexhausted").getValue();
    localStorage.ExhaustedBenefits = SetExhaustedBenefits;
}
exports.AutoBenefitsExhausted = AutoBenefitsExhausted;
