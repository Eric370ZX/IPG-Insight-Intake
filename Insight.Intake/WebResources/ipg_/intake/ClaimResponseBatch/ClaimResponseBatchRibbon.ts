/**
 * @namespace Intake.ClaimResponseBatch.Ribbon
 */
namespace Intake.ClaimResponseBatch.Ribbon {
  let isButtonEnabled = false;
  const BatchStatus_New = 1;

  /**
   * Enable \ Disable Delete button
   * @function Intake.ClaimResponseBatch.Ribbon.DeleteButtonEnabled
   * @returns {boolean}
  */
  export function DeleteButtonEnabled(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;

    let statusCode = formContext.getAttribute("statuscode");
    let recordCount = formContext.getAttribute("ipg_recordcount");
    if (statusCode != null && statusCode.getValue() == BatchStatus_New && recordCount != null && recordCount.getValue() == 0) {
      isButtonEnabled = true;
    }
    else {
      isButtonEnabled = false;
    }

    return isButtonEnabled;
  }

  /**
   * Called on Archive button click
   * @function Intake.ClaimResponseBatch.Ribbon.OnArchiveClick
   * @returns {void}
  */
  export function OnArchiveClick(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;
    if (confirm(`Are you sure you want to archive the claim response batch?`)) {
      var data =
      {
        statuscode: 427880003 //archive
      };
      Xrm.WebApi.updateRecord('ipg_claimresponsebatch', formContext.data.entity.getId(), data).then(
        function success(result) {
          formContext.data.refresh(false);
        },
        function (error) {
          console.log(error.message);
        }
      );
    }
  }
}
