/**
 * @namespace Intake.Case
 */
var Intake;
(function (Intake) {
    var Case;
    (function (Case) {
        var patientWorkPhoneKey = 'ipg_patientworkphone';
        var patientHomePhoneKey = 'ipg_patienthomephone';
        var patientCellPhoneKey = 'ipg_patientcellphone';
        var patientWorkPhoneAttr = Xrm.Page.getAttribute(patientWorkPhoneKey);
        var patientHomePhoneAttr = Xrm.Page.getAttribute(patientHomePhoneKey);
        var patientCellPhoneAttr = Xrm.Page.getAttribute(patientCellPhoneKey);
        /**
         * Called when the form is loaded. See D365 configuration for details.
         * @function Intake.Case.ValidatePhoneNumber
         * @returns {void}
         */
        function ValidatePhoneNumber() {
            function changeListener() {
                var workPhone = patientWorkPhoneAttr.getValue();
                var homePhone = patientHomePhoneAttr.getValue();
                var cellPhone = patientCellPhoneAttr.getValue();
                if (workPhone == null && homePhone == null && cellPhone == null) {
                    patientWorkPhoneAttr.setRequiredLevel("required");
                    patientHomePhoneAttr.setRequiredLevel("required");
                    patientCellPhoneAttr.setRequiredLevel("required");
                }
                else {
                    patientWorkPhoneAttr.setRequiredLevel("none");
                    patientHomePhoneAttr.setRequiredLevel("none");
                    patientCellPhoneAttr.setRequiredLevel("none");
                }
            }
            [patientWorkPhoneAttr, patientHomePhoneAttr, patientCellPhoneAttr].forEach(function (x) { return x.addOnChange(changeListener); });
            ValidatePhoneNumber();
        }
        Case.ValidatePhoneNumber = ValidatePhoneNumber;
    })(Case = Intake.Case || (Intake.Case = {}));
})(Intake || (Intake = {}));
