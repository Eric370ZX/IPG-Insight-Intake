var Intake;
(function (Intake) {
    var Case;
    (function (Case) {
        function GetProcedureName(executionContext) {
            var formContext = executionContext.getFormContext();
            var procedureName = formContext.getAttribute("ipg_procedureid").getValue();
            if ((typeof localStorage.ProcdureId != 'undefined') && procedureName == null) {
                var object = new Array();
                object[0] = new Object();
                object[0].id = localStorage.ProcdureId;
                object[0].name = localStorage.ProcdureName;
                object[0].entityType = "ipg_procedurename";
                formContext.getAttribute("ipg_procedureid").setValue(object);
            }
            localStorage.removeItem("ProcdureName");
            localStorage.removeItem("ProcdureId");
        }
        Case.GetProcedureName = GetProcedureName;
    })(Case = Intake.Case || (Intake.Case = {}));
})(Intake || (Intake = {}));
