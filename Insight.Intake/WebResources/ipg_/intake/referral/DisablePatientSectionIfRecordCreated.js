/**
 * @namespace Intake.Referral
 */
var Intake;
(function (Intake) {
    var Referral;
    (function (Referral) {
        /**
         * Called when the form is loaded. See D365 configuration for details.
         * @function Intake.Referral.DisablePatientSectionIfRecordCreated
         * @returns {void}
         */
        function DisablePatientSectionIfRecordCreated() {
            var formType = Xrm.Page.ui.getFormType();
            var tab = Xrm.Page.ui.tabs.get('Referral');
            var section = tab && tab.sections.get('Patient');
            var controls = section && section.controls.get();
            if (section && controls && controls.length) {
                var isRecordCreated_1 = 2 /* Update */ === formType || 4 /* Disabled */ === formType || 3 /* ReadOnly */ === formType;
                controls.forEach(function (control) {
                    if (control.getVisible()) {
                        control.setDisabled(isRecordCreated_1);
                    }
                });
            }
        }
        Referral.DisablePatientSectionIfRecordCreated = DisablePatientSectionIfRecordCreated;
    })(Referral = Intake.Referral || (Intake.Referral = {}));
})(Intake || (Intake = {}));
