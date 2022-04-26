
/**
 * @namespace Intake.Referral
 */
namespace Intake.Referral {
  export function CreateFullName(executionContext) {

    let formContext = executionContext.getFormContext();
    let middleName = formContext.getAttribute("ipg_patientmiddlename").getValue();
    formContext.getControl("ipg_patientfirstname").setVisible(true);

    if (middleName != null) {

      let firstChar = middleName.charAt(0);
      {
        middleName = firstChar.toUpperCase();
      }
      let fullName = formContext.getAttribute("ipg_patientfirstname").getValue() + " " + middleName + ". " + formContext.getAttribute("ipg_patientlastname").getValue();
      formContext.getAttribute("ipg_patientfullname").setValue(fullName);

    }
  }
}
