/**
 * @namespace Intake.Referral
 */
var Intake;
(function (Intake) {
    var Referral;
    (function (Referral) {
        /**
         * Called when the form is loaded. See D365 configuration for details.
         * @function Intake.Referral.ShowStateSectionIfRecordCreated
         * @returns {void}
         */
        function ShowStateSectionIfRecordCreated() {
            var formType = Xrm.Page.ui.getFormType();
            var tab = Xrm.Page.ui.tabs.get('Referral');
            var section = tab && tab.sections.get('Status');
            if (section) {
                var isRecordCreated = 2 /* Update */ === formType || 4 /* Disabled */ === formType || 3 /* ReadOnly */ === formType;
                section.setVisible(isRecordCreated);
            }
        }
        Referral.ShowStateSectionIfRecordCreated = ShowStateSectionIfRecordCreated;
    })(Referral = Intake.Referral || (Intake.Referral = {}));
})(Intake || (Intake = {}));
