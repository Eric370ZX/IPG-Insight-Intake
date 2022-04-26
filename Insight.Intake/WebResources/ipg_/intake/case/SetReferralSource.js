/**
 * @namespace Intake.Case
 */
var Intake;
(function (Intake) {
    var Case;
    (function (Case) {
        /**
        * Set the referral source on the case
        *
        * @function Intake.Case.SetReferralSource
        * @returns {void}
        */
        function SetReferralSource(executionContext) {
            //debugger;
            var formContext = executionContext.getFormContext();
            var caseId = executionContext.getContext().getQueryStringParameters().id;
            if (caseId) {
                Xrm.WebApi.online.retrieveRecord("incident", Intake.Utility.removeCurlyBraces(caseId), "?$select=_ipg_referralid_value").then(function success(result) {
                    var _ipg_referralid_value = result["_ipg_referralid_value"];
                    if (_ipg_referralid_value) {
                        Xrm.WebApi.online.retrieveRecord("ipg_referral", Intake.Utility.removeCurlyBraces(_ipg_referralid_value), "?$select=ipg_origin").then(function success(result) {
                            var ipg_origin = result["ipg_origin"];
                            if (ipg_origin) {
                                var referralSource = formContext.getAttribute("ipg_intakemethod");
                                if (referralSource.getValue() == null) {
                                    referralSource.setValue(ipg_origin);
                                }
                            }
                        }, function (error) {
                            Xrm.Utility.alertDialog(error.message, null);
                        });
                    }
                }, function (error) {
                    Xrm.Utility.alertDialog(error.message, null);
                });
            }
        }
        Case.SetReferralSource = SetReferralSource;
    })(Case = Intake.Case || (Intake.Case = {}));
})(Intake || (Intake = {}));
