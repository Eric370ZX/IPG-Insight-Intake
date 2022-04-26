/**
 * @namespace Intake.Claim
 */
var Intake;
(function (Intake) {
    var Claim;
    (function (Claim) {
        /**
         * Called on load form
         * @function Intake.Claim.OnLoadForm
         * @returns {void}
        */
        function OnLoadForm(executionContext) {
            var formContext = executionContext.getFormContext();
            if (formContext.getAttribute('ipg_status').getValue() == 427880003) { //voided
                formContext.getControl().forEach(function (control) {
                    control.setDisabled(true);
                });
            }
        }
        Claim.OnLoadForm = OnLoadForm;
    })(Claim = Intake.Claim || (Intake.Claim = {}));
})(Intake || (Intake = {}));
