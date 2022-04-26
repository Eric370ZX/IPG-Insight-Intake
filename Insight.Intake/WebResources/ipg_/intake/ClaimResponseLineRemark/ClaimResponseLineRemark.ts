/**
 * @namespace Intake.ClaimResponseLineRemark
 */
namespace Intake.ClaimResponseLineRemark {

  /**
  * Called on RemarkCode Change event
  * @function Intake.ClaimResponseLineRemark.OnChangeRemarkCode
  * @returns {void}
*/
  export function OnChangeRemarkCode(executionContext) {
    //debugger;
    let formContext = executionContext.getFormContext();
    let remarkCode = formContext.getAttribute("ipg_remarkcode");

    if (remarkCode != null) {
      let remarkCodeName = remarkCode.getValue()[0].name;
      let codeAttr = formContext.getAttribute("ipg_code");
      if (codeAttr != null) {
        codeAttr.setValue(remarkCodeName);
      }
      let nameAttr = formContext.getAttribute("ipg_name");
      if (nameAttr != null) {
        nameAttr.setValue(remarkCodeName);
      }
    }
  }
}
