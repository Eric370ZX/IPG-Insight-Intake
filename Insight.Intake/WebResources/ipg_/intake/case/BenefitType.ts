namespace Intake.Case {

  export function SetBenefitType(executionContext) {
  
    var formContext = executionContext.getFormContext();
    var i;
    var benefitType = formContext.getAttribute("ipg_memberidnumber").getValue();

    if (benefitType != null) {
      if (benefitType.includes("JQU")) {
        let benfitOptionSetValue = formContext.getAttribute("ipg_benefittypecode").getValue();
        if (benfitOptionSetValue === "427880040") {
          let optionSetValues = formContext.getAttribute("ipg_benefittypecode").getOptions();
          for (i = 0; i < optionSetValues.length; i++) {
            if (optionSetValues[i].value === benfitOptionSetValue) {
              let getBenefit = Xrm.Page.getControl("ipg_benefittypecode");
              getBenefit.addOption({ text: optionSetValues[i].text, value: optionSetValues[i].value });
            }
          }
        }
      }
    }
  }
}
  


