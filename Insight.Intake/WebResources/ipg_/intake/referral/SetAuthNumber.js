var Intake;
(function (Intake) {
    var Referral;
    (function (Referral) {
        function SetAuthNumber(executionContext) {
            var formContext = executionContext.getFormContext();
            var authNumberObject;
            if (formContext.getAttribute("ipg_facility_auth2").getValue()) {
                localStorage.AuthNumberName = formContext.getAttribute("ipg_facility_auth2").getValue();
            }
        }
        Referral.SetAuthNumber = SetAuthNumber;
    })(Referral = Intake.Referral || (Intake.Referral = {}));
})(Intake || (Intake = {}));
