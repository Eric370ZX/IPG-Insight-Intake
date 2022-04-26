export function AutoBenefitsExhausted(executionContext) {
 
  let formContext = executionContext.getFormContext();
  let SetExhaustedBenefits = formContext.getAttribute("ipg_autobenefitsexhausted").getValue(); 
  localStorage.ExhaustedBenefits = SetExhaustedBenefits;
}
