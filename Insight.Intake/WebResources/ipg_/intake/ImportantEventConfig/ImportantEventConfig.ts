/**
 * @namespace Intake.ImportantEventConfig
 */
namespace Intake.ImportantEventConfig {
    /**
      * Called on Form Load event
      * @function Intake.ImportantEventConfig.OnLoadForm
      * @returns {void}
    */
    export function OnLoadForm(executionContext) {
        let formContext = executionContext.getFormContext();
        const EventTypeIdField: string = 'ipg_name';
        if (formContext.ui.getFormType() !== 1) {
            formContext.getControl(EventTypeIdField).setDisabled(true);
        }
    }
}
