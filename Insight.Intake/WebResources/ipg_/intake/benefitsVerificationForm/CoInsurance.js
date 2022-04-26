function AddCoinsurance(executionContext) {
    var formContext = executionContext.getFormContext();
    var carrierInsurance;
    var patientInsurance;
    carrierInsurance = formContext.getAttribute("ipg_carriercoinsurance").getValue();
    patientInsurance = formContext.getAttribute("ipg_patientcoinsurance").getValue();
    if (carrierInsurance + patientInsurance > 100) {
        alert("The Carrier and Patient Insurance must = 100%");
        formContext.getAttribute("ipg_carriercoinsurance").setValue(null);
        formContext.getAttribute("ipg_patientcoinsurance").setValue(null);
    }
}
