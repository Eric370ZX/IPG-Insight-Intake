/**
 * @namespace Intake.Utility
 */
var Intake;
(function (Intake) {
    var Utility;
    (function (Utility) {
        var Ribbon;
        (function (Ribbon) {
            /**
             * Called on Cancel button click
             * @function Intake.Utility.Ribbon.btnCancel
             * @returns {void}
            */
            function btnCancel(primaryControl) {
                var formContext = primaryControl;
                formContext.data.refresh(false).then(function success() { formContext.ui.close(); /*window.history.back();*/ }, function error(e) { console.log(e.message); });
            }
            Ribbon.btnCancel = btnCancel;
        })(Ribbon = Utility.Ribbon || (Utility.Ribbon = {}));
    })(Utility = Intake.Utility || (Intake.Utility = {}));
})(Intake || (Intake = {}));
