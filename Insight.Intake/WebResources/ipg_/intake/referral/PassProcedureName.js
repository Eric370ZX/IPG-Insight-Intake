var Intake;
(function (Intake) {
    var Referral;
    (function (Referral) {
        function SetProcedureName(executionContext) {
            var formContext = executionContext.getFormContext();
            var procedureObject;
            if (formContext.getAttribute("ipg_procedurenameid").getValue()) {
                procedureObject = formContext.getAttribute("ipg_procedurenameid").getValue();
                localStorage.ProcdureName = procedureObject[0].name;
                localStorage.ProcdureId = procedureObject[0].id;
            }
        }
        Referral.SetProcedureName = SetProcedureName;
    })(Referral = Intake.Referral || (Intake.Referral = {}));
})(Intake || (Intake = {}));
