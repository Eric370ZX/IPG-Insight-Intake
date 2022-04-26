/**
 * @namespace Intake.ClaimResponseHeader.Ribbon
 */
namespace Intake.ClaimResponseHeader.Ribbon {
  let isButtonEnabled = false;
  const BatchStatus_New = 1;
  const BatchStatus_InProgress = 427880001;

  /**
   * Enable \ Disable Subgrid Add New button
   * @function Intake.ClaimResponseHeader.Ribbon.SubgridAddButtonEnabled
   * @returns {boolean}
  */
  export function SubgridAddButtonEnabled(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;

    let isManualBatch = formContext.getAttribute("ipg_ismanualbatch");
    if (isManualBatch != null && isManualBatch.getValue() == true) {
      let statusCode = formContext.getAttribute("statuscode");
      if (statusCode != null && (statusCode.getValue() == BatchStatus_New || statusCode.getValue() == BatchStatus_InProgress)) {
        isButtonEnabled = true;
      }
      else
      {
        isButtonEnabled = false;
      }
    }

    return isButtonEnabled;
  }

  /**
   * Apply button OnClick
   * @function Intake.ClaimResponseHeader.Ribbon.OnClickApply
   * @returns {void}
  */
  export function OnClickApply(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;
    let claimResponseHeaders = [];
    claimResponseHeaders.push({ Id: formContext.data.entity.getId() });

    Xrm.Utility.showProgressIndicator("Processing...");

    formContext.data.save().then(
      function () {
        Xrm.Utility.closeProgressIndicator();
        Intake.Claim.Ribbon.OnPostClick(claimResponseHeaders);
      },
      function (error) {
        Xrm.Utility.closeProgressIndicator();
        Xrm.Navigation.openAlertDialog({text:error?.message ?? error });
      });
    }
   

  /**
   * Cancel button OnClick
   * @function Intake.ClaimResponseHeader.Ribbon.OnClickCancel
   * @returns {void}
  */
  export function OnClickCancel(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;
    let claimResponseHeaderId = formContext.data.entity.getId();
    let claimResponseBatchAttr = formContext.getAttribute("ipg_claimresponsebatchid");
    Xrm.Navigation.openConfirmDialog({ title: "Cancel Payment", text: "Do you want to Cancel this payment and return to the Batch form?" }, { height: 300, width: 400 })
      .then((success) => {
        if (success.confirmed) {
          if (claimResponseBatchAttr != null) {
            let claimResponseBatchId = claimResponseBatchAttr.getValue()[0].id;
            Xrm.WebApi.deleteRecord("ipg_claimresponseheader", claimResponseHeaderId)
              .then((result) => {
                if (claimResponseBatchAttr != null) {
                  var entityFormOptions = {};
                  entityFormOptions["entityName"] = "ipg_claimresponsebatch";
                  entityFormOptions["entityId"] = claimResponseBatchId
                  Xrm.Navigation.openForm(entityFormOptions);
                }
              }, (error) => {
                console.log(error);
              });
          }
          else {
            console.log("Batch not found.");
          }
        }
      });
  }

  /**
   * Determinr if Payment is manual or not
   * @function Intake.ClaimResponseHeader.Ribbon.IsManualPayment
   * @returns {boolean}
  */
  export function IsManualPayment(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;
    let isManualPayment = formContext.getAttribute("ipg_ismanualpayment");
    if (isManualPayment != null && isManualPayment.getValue() == true) {
      return true;
    }
    else {
      return false;
    }
  }
}
