/**
 * @namespace Intake.Utility
 */
namespace Intake.Utility {

  /**
    * Called on onchange event of name field. See D365 configuration for details.
    * @function Intake.Utility.CheckCPTForFormatting
    * @returns {void}
    */
  export function CheckCPTForFormatting(executionContext: Xrm.Events.EventContext) {

    //debugger;
    let regexp: RegExp = new RegExp(`^(\\d{4}([A-Za-z0-9]{1}))$`)

    var formContext = executionContext.getFormContext(); 
    let cptCodeAttribute: Xrm.Attributes.Attribute = formContext.getAttribute("ipg_cptcode");
    if (cptCodeAttribute) {
      let cptCodeControl: Xrm.Controls.StandardControl = <Xrm.Controls.StandardControl>formContext.getControl("ipg_cptcode");
      if (cptCodeControl) {
        if (!regexp.test(cptCodeAttribute.getValue())) {
          cptCodeControl.setNotification(`Incorrect CPT code format: must contain four numbers followed by one alphanumeric character`, 'cptcode');
        } else {
          cptCodeControl.clearNotification('cptcode');
        }
      }
    }
  }
}
