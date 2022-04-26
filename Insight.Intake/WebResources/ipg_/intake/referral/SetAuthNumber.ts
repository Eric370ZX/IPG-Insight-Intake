
namespace Intake.Referral
{
export function SetAuthNumber(executionContext) {

  let formContext = executionContext.getFormContext();
  let authNumberObject;
  if (formContext.getAttribute("ipg_facility_auth2").getValue()) {
    localStorage.AuthNumberName = formContext.getAttribute("ipg_facility_auth2").getValue();
  }
}
}
