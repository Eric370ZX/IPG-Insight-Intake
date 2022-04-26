/**
 * @namespace Intake.Case
 */
namespace Intake.Case {

  /**
   * Called on load form
   * @function Intake.Account.OnLoadForm
   * @returns {void}
  */
  export function OnLoadForm(executionContext) {
    let formContext = executionContext.getFormContext();
    SetNameFields(formContext);
  }
  Case.OnFormLoad = OnFormLoad;

  /**
 * Update the Case form with First, Middle, Last and DOB from Referral   * 
 * @returns {void}
 */



  function SetNameFields(formContext) {
    let caseId = formContext.getAttribute("title").getValue();
    let dob;

    Xrm.WebApi.retrieveMultipleRecords("ipg_referral", "?$select=ipg_referralcasenumber,ipg_name,ipg_patientfirstname,ipg_patientmiddlename,ipg_patientlastname,ipg_patientdateofbirth,ipg_referralid").then(function success(results) {

      if (results.entities.length != 0) {
        for (let i = 0; i < results.entities.length; i++) {
          if (caseId.includes(results.entities[i]["ipg_referralcasenumber"])) {
            formContext.getAttribute("ipg_insuredfirstname").setValue(results.entities[i]["ipg_patientfirstname"]);
            formContext.getAttribute("ipg_insuredmiddlename").setValue(results.entities[i]["ipg_patientmiddlename"]);
            formContext.getAttribute("ipg_insuredlastname").setValue(results.entities[i]["ipg_patientlastname"]);
            dob = new Date(results.entities[i]["ipg_patientdateofbirth@OData.Community.Display.V1.FormattedValue"]);
            formContext.getAttribute("ipg_insureddateofbirth").setValue(dob);
          }
        }
      }
    }, function (error) {
    });

  }

  }
  //) (Case = Intake.Case || (Intake.Case = {}));
  //})(Intake || (Intake = {}));



