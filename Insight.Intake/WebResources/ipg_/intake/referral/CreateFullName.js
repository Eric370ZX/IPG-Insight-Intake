/**
 * @namespace Intake.Referral
 */
var Intake;
(function (Intake) {
    var Referral;
    (function (Referral) {
        function CreateFullName(executionContext) {
            var formContext = executionContext.getFormContext();
            var middleName = formContext.getAttribute("ipg_patientmiddlename").getValue();
            formContext.getControl("ipg_patientfirstname").setVisible(true);
            if (middleName != null) {
                var firstChar = middleName.charAt(0);
                {
                    middleName = firstChar.toUpperCase();
                }
                var fullName = formContext.getAttribute("ipg_patientfirstname").getValue() + " " + middleName + ". " + formContext.getAttribute("ipg_patientlastname").getValue();
                formContext.getAttribute("ipg_patientfullname").setValue(fullName);
            }
        }
        Referral.CreateFullName = CreateFullName;
    })(Referral = Intake.Referral || (Intake.Referral = {}));
})(Intake || (Intake = {}));
