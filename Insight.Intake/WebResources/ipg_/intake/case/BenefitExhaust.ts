export function SetBenefitExhaust(executionContext) {
 
  let formContext = executionContext.getFormContext();
  let a = localStorage.ExhaustedBenefits;

  if (a === "1") {
    formContext.getAttribute("ipg_autobenefitsexhausted").setValue(true);
  }
  else {
    formContext.getAttribute("ipg_autobenefitsexhausted").setValue(false);
  }
  localStorage.removeItem('ExhaustedBenefits');
  localStorage.clear();
}


