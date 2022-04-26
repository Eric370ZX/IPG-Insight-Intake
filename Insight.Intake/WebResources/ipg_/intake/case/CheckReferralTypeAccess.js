/**
 * @namespace Intake.Case
 */
var Intake;
(function (Intake) {
    var Case;
    (function (Case) {
        /**
        * Enable the referral type field
        * if it has not previously been updated
        * and the scheduled dos has been changed during gates 1, 2, or 3.
        * This logic is handled in a server side plug in.
        *
        * @function Intake.Case.CheckReferralTypeField
        * @returns {void}
        */
        function CheckReferralTypeField(executionContext) {
            var formContext = executionContext.getFormContext();
            var referralTypeField = formContext.getControl("ipg_referraltype");
            var incidentId = Intake.Utility.removeCurlyBraces(Xrm.Page.data.entity.getId());
            if (incidentId) {
                Xrm.WebApi.online.retrieveRecord("incident", incidentId, "?$select=ipg_referraltypecanbeaccessed").then(function success(result) {
                    var ipg_referraltypecanbeaccessed = result["ipg_referraltypecanbeaccessed"];
                    if (ipg_referraltypecanbeaccessed) {
                        referralTypeField.setDisabled(false);
                    }
                    else {
                        referralTypeField.setDisabled(true);
                    }
                }, function (error) {
                    Xrm.Utility.alertDialog(error.message, null);
                });
            }
        }
        Case.CheckReferralTypeField = CheckReferralTypeField;
    })(Case = Intake.Case || (Intake.Case = {}));
})(Intake || (Intake = {}));
