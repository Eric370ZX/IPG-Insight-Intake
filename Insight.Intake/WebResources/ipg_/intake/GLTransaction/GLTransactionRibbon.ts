/**
 * @namespace Intake.GLTransaction.Ribbon
 */
namespace Intake.GLTransaction.Ribbon {

  /**
   * Called on Additional Details button click
   * @function Intake.GLTransaction.Ribbon.OnAdditionalDetailsClick
   * @returns {void}
  */
  export function OnAdditionalDetailsClick(firstSelectedItemId, primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;
    if (firstSelectedItemId == null)
      return;

    var entityFormOptions = {};
    entityFormOptions["entityName"] = "ipg_gltransaction";
    entityFormOptions["entityId"] = firstSelectedItemId.replace(/[{}]/g, "");
    entityFormOptions["formId"] = "cfd50ddf-8d0d-4072-8a83-ab41bdba30c1";
    Xrm.Navigation.openForm(entityFormOptions);
  }

  /**
   * Called on New Adjustment button click
   * @function Intake.GLTransaction.Ribbon.onNewAdjustmentClick
   * @returns {void}
  */
  export function onNewAdjustmentClick(formContext) {
    let entityFormOptions = {
      entityName: "ipg_adjustment",
      openInNewWindow: true
    };
    let formParameters = {
      ipg_caseid: formContext.data.entity.getId().replace(/[{}]/g, ""),
      ipg_caseidname: formContext.getAttribute("title").getValue()
    };
    Xrm.Navigation.openForm(entityFormOptions, formParameters);
  }

  /**
   * Called on New Adjustment button click
   * @function Intake.GLTransaction.Ribbon.onNewPaymentBatchClick
   * @returns {void}
  */
  export function onNewPaymentBatchClick(formContext) {
    let entityFormOptions = {
      entityName: "ipg_claimresponsebatch",
      openInNewWindow: true
    };
    Xrm.Navigation.openForm(entityFormOptions);
  }

}
