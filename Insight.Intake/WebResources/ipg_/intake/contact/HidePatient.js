var Intake;
(function (Intake) {
    var Contact;
    (function (Contact) {
        function controlFormRibbon() {
            var formLabel;
            var currForm = Xrm.Page.ui.formSelector.getCurrentItem();
            formLabel = currForm.getLabel();
            if (formLabel != "Patient") {
                return true;
            }
            else {
                return false;
            }
        }
        Contact.controlFormRibbon = controlFormRibbon;
        function controlViewRibbon() {
            var viewId;
            viewId = window.parent.location.href;
            var currView = "7bdf8c63-d694-e811-a961-000d3a3702ca";
            if (!viewId.includes(currView)) {
                return true;
            }
            else {
                return false;
            }
        }
        Contact.controlViewRibbon = controlViewRibbon;
    })(Contact = Intake.Contact || (Intake.Contact = {}));
})(Intake || (Intake = {}));
