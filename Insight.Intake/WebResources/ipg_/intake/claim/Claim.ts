/**
 * @namespace Intake.Claim
 */
namespace Intake.Claim {
  
  /**
   * Called on load form
   * @function Intake.Claim.OnLoadForm
   * @returns {void}
  */
  export function OnLoadForm(executionContext) {
    let formContext = executionContext.getFormContext();
    if (formContext.getAttribute('ipg_status').getValue() == 427880003) { //voided
      formContext.getControl().forEach((control: Xrm.Controls.Control): void => {
        control.setDisabled(true);
      });
    }
  }

}
