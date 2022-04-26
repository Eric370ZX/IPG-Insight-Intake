/**
 * @namespace Intake.ClaimResponseLineAdjustment
 */
namespace Intake.ClaimResponseLineAdjustment {

  /**
  * Called on ClaimStatus Change event
  * @function Intake.ClaimResponseLineAdjustment.OnChangeClaimStatus
  * @returns {void}
*/
  export function OnChangeClaimStatus(executionContext) {
    //debugger;
    let formContext = executionContext.getFormContext();
    let claimStatus = formContext.getAttribute("ipg_claimstatus");

    if (claimStatus != null) {
      let claimStatusName = claimStatus.getValue()[0].name;
      if (claimStatusName.includes(" - ")) {
        let adjustmentCode = claimStatusName.substring(0, claimStatusName.indexOf(" - "));
        let adjustmentCodeAttr = formContext.getAttribute("ipg_code");
        if (adjustmentCodeAttr != null) {
          adjustmentCodeAttr.setValue(adjustmentCode);
        }
      }
    }
  }
}
