/**
 * @namespace Intake.Adjustment.Ribbon
 */
namespace Intake.Adjustment.Ribbon {

  /**
     * Called on Adjustment Reversal button click
     * @function Intake.Adjustment.Ribbon.OnAdjustmentReversalClick
     * @returns {void}
    */
  export function OnAdjustmentReversalClick(primaryControl, selectedItems) {

    let formContext: Xrm.FormContext = primaryControl;
    let caseId = formContext.data.entity.getId();
    if (!caseId)
      return;

    var entityFormOptions = {};
    entityFormOptions["entityName"] = "ipg_adjustment";

    Xrm.WebApi.retrieveRecord("incident", caseId, "?$select=title,ipg_remainingcarrierbalance,ipg_remainingsecondarycarrierbalance,ipg_remainingpatientbalance,ipg_casebalance").then(function (incident) {
      let formParameters = {};
      formParameters["ipg_caseid"] = caseId;
      formParameters["ipg_caseidname"] = incident.title;
      formParameters["ipg_caseidtype"] = formContext.data.entity.getEntityName();
      formParameters["ipg_amounttype"] = true;
      formParameters["ipg_remainingcarrierbalance"] = incident.ipg_remainingcarrierbalance;
      formParameters["ipg_remainingsecondarycarrierbalance"] = incident.ipg_remainingsecondarycarrierbalance;
      formParameters["ipg_remainingpatientbalance"] = incident.ipg_remainingpatientbalance;
      formParameters["ipg_casebalance"] = incident.ipg_casebalance;
      formParameters["ipg_adjustmenttype"] = selectedItems[0].ipg_AdjustmentType;
      formParameters["ipg_reverse"] = true;
      Xrm.WebApi.retrieveRecord("ipg_adjustment", selectedItems[0].Id, "?$select=ipg_applyto,ipg_amounttoapply,ipg_adjustmenttype,_ipg_tocase_value,ipg_transferofpaymenttype,ipg_from,ipg_to,_ipg_payment_value").then(
        function success(result) {
          formParameters["ipg_applyto"] = result["ipg_applyto"];
          formParameters["ipg_amount"] = result["ipg_amounttoapply"];
          formParameters["ipg_adjustmenttype"] = result["ipg_adjustmenttype"];
          if (result["_ipg_tocase_value"]) {
            formParameters["ipg_tocase"] = result["_ipg_tocase_value"];
            formParameters["ipg_tocasename"] = result["_ipg_tocase_value@OData.Community.Display.V1.FormattedValue"];
            formParameters["ipg_tocasetype"] = result["_ipg_tocase_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
          }
          formParameters["ipg_transferofpaymenttype"] = result["ipg_transferofpaymenttype"];
          formParameters["ipg_from"] = result["ipg_from"];
          formParameters["ipg_to"] = result["ipg_to"];
          if (result["_ipg_payment_value"]) {
            formParameters["ipg_payment"] = result["_ipg_payment_value"];
            formParameters["ipg_paymentname"] = result["_ipg_payment_value@OData.Community.Display.V1.FormattedValue"];
            formParameters["ipg_paymenttype"] = result["_ipg_payment_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
          }
          Xrm.Navigation.openForm(entityFormOptions, formParameters);
        },
        function (error) {
          Xrm.Utility.alertDialog(error.message, null);
        }
      );
    }, function (error) {
      Xrm.Navigation.openErrorDialog({ message: error.message });
    });

  }

  /**
   * Called on Cancel button click
   * @function Intake.Adjustment.Ribbon.OnCancelClick
   * @returns {void}
  */
  export function OnCancelClick(primaryControl) {
    window.history.back();
  }

}
