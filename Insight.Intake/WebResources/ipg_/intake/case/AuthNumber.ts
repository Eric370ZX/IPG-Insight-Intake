
namespace Intake.Case {
  export function GetAuthNumber(executionContext) {
   
    let formContext = executionContext.getFormContext();
    if ((typeof localStorage.AuthNumberName != 'undefined')) {
      formContext.getAttribute("ipg_facility_auth2").setValue(localStorage.AuthNumberName);
      localStorage.removeItem("AuthNumberName");
    }
  }
}


