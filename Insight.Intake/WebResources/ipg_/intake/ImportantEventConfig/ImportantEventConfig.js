/**
 * @namespace Intake.ImportantEventConfig
 */
var Intake;
(function (Intake) {
    var ImportantEventConfig;
    (function (ImportantEventConfig) {
        /**
          * Called on Form Load event
          * @function Intake.ImportantEventConfig.OnLoadForm
          * @returns {void}
        */
        function OnLoadForm(executionContext) {
            var formContext = executionContext.getFormContext();
            var EventTypeIdField = 'ipg_name';
            if (formContext.ui.getFormType() !== 1) {
                formContext.getControl(EventTypeIdField).setDisabled(true);
            }
        }
        ImportantEventConfig.OnLoadForm = OnLoadForm;
    })(ImportantEventConfig = Intake.ImportantEventConfig || (Intake.ImportantEventConfig = {}));
})(Intake || (Intake = {}));
