/**
 * @namespace Intake.Utility
 */
namespace Intake.Utility.Ribbon {

/**
 * Called on Cancel button click
 * @function Intake.Utility.Ribbon.btnCancel
 * @returns {void}
*/
  export function btnCancel(primaryControl) {

    let formContext: Xrm.FormContext = primaryControl;

    formContext.data.refresh(false).then(function success() { formContext.ui.close(); /*window.history.back();*/}, function error(e) { console.log(e.message); });

  }
}
