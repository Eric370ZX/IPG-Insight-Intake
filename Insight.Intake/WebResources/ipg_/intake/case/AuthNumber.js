var Intake;
(function (Intake) {
    var Case;
    (function (Case) {
        function GetAuthNumber(executionContext) {
            var formContext = executionContext.getFormContext();
            if ((typeof localStorage.AuthNumberName != 'undefined')) {
                formContext.getAttribute("ipg_facility_auth2").setValue(localStorage.AuthNumberName);
                localStorage.removeItem("AuthNumberName");
            }
        }
        Case.GetAuthNumber = GetAuthNumber;
    })(Case = Intake.Case || (Intake.Case = {}));
})(Intake || (Intake = {}));
