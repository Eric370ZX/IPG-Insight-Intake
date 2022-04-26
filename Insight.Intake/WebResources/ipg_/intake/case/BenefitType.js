var Intake;
(function (Intake) {
    var Case;
    (function (Case) {
        function SetBenefitType(executionContext) {
            var formContext = executionContext.getFormContext();
            var i;
            var benefitType = formContext.getAttribute("ipg_memberidnumber").getValue();
            if (benefitType != null) {
                if (benefitType.includes("JQU")) {
                    var benfitOptionSetValue = formContext.getAttribute("ipg_benefittypecode").getValue();
                    if (benfitOptionSetValue === "427880040") {
                        var optionSetValues = formContext.getAttribute("ipg_benefittypecode").getOptions();
                        for (i = 0; i < optionSetValues.length; i++) {
                            if (optionSetValues[i].value === benfitOptionSetValue) {
                                var getBenefit = Xrm.Page.getControl("ipg_benefittypecode");
                                getBenefit.addOption({ text: optionSetValues[i].text, value: optionSetValues[i].value });
                            }
                        }
                    }
                }
            }
        }
        Case.SetBenefitType = SetBenefitType;
    })(Case = Intake.Case || (Intake.Case = {}));
})(Intake || (Intake = {}));
