/**
 * @namespace Intake.Case
 */
namespace Intake.Case {
  /**
  * Enable the referral type field
  * if it has not previously been updated
  * and the scheduled dos has been changed during gates 1, 2, or 3.
  * This logic is handled in a server side plug in.
  *
  * @function Intake.Case.CheckReferralTypeField
  * @returns {void}
  */
  export function CheckReferralTypeField(executionContext) {
    let formContext = executionContext.getFormContext();
    let referralTypeField = <Xrm.Controls.StandardControl>formContext.getControl("ipg_referraltype");
    let incidentId = Intake.Utility.removeCurlyBraces(Xrm.Page.data.entity.getId());

    if (incidentId) {
      Xrm.WebApi.online.retrieveRecord("incident", incidentId, "?$select=ipg_referraltypecanbeaccessed").then(
        function success(result) {
          var ipg_referraltypecanbeaccessed = result["ipg_referraltypecanbeaccessed"];

          if (ipg_referraltypecanbeaccessed) {
            referralTypeField.setDisabled(false);
          }
          else {
            referralTypeField.setDisabled(true);
          }
        },
        function (error) {
          Xrm.Utility.alertDialog(error.message, null);
        }
      );
    }
  }
}

