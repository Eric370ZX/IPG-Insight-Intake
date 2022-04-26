/**
 * @namespace Intake.Case
 */
namespace Intake.Case {
  /**
  * Set the referral source on the case
  *
  * @function Intake.Case.SetReferralSource
  * @returns {void}
  */
  export function SetReferralSource(executionContext: Xrm.Events.EventContext) {
    //debugger;
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    let caseId = executionContext.getContext().getQueryStringParameters().id;
    if (caseId) {
      Xrm.WebApi.online.retrieveRecord("incident", Intake.Utility.removeCurlyBraces(caseId), "?$select=_ipg_referralid_value").then(
        function success(result) {
          let _ipg_referralid_value = result["_ipg_referralid_value"];
          if (_ipg_referralid_value) {
            Xrm.WebApi.online.retrieveRecord("ipg_referral", Intake.Utility.removeCurlyBraces(_ipg_referralid_value), "?$select=ipg_origin").then(
              function success(result) {
                let ipg_origin = result["ipg_origin"];
                if (ipg_origin) {
                  let referralSource = formContext.getAttribute("ipg_intakemethod");
                  if (referralSource.getValue() == null) {
                    referralSource.setValue(ipg_origin);
                  }
                }
              },
              function (error) {
                Xrm.Utility.alertDialog(error.message, null);
              }
            );
          }
        },
        function (error) {
          Xrm.Utility.alertDialog(error.message, null);
        }
      );
     }
    }
  }


