var Intake;
(function (Intake) {
    var Account;
    (function (Account) {
        function controlFormRibbon() {
            var formLabel;
            var currForm = Xrm.Page.ui.formSelector.getCurrentItem();
            formLabel = currForm.getLabel();
            if (formLabel != "Manufacturer") {
                return true;
            }
            else {
                return false;
            }
        }
        Account.controlFormRibbon = controlFormRibbon;
    })(Account = Intake.Account || (Intake.Account = {}));
})(Intake || (Intake = {}));
