/**
 * @namespace Intake.Case
 */
namespace Intake.Case {
  /**
   * Called on saving case
   * @function Intake.Case.RemoveDraftCasePartDetailsAfterApproving
   * @returns {void}
  */

  export function RemoveDraftCasePartDetailsAfterApproving(context : any) {
    var saveEvent = context.getEventArgs();

    if (saveEvent.getSaveMode() == 70) {
      saveEvent.preventDefault();
      return;
    }

    let partViewIsChanged = Xrm.Page.getAttribute("ipg_partviewischanged");
    if (partViewIsChanged.getValue()) {
      saveEvent.preventDefault();
      let entityId = Intake.Utility.removeCurlyBraces(Xrm.Page.data.entity.getId());
      Xrm.WebApi.retrieveMultipleRecords("ipg_casepartdetail", "?$select=ipg_casepartdetailid&$filter=ipg_caseid/incidentid eq " + entityId + " and statuscode eq 1").then(
        function success(result) {
          if (result && result.entities.length) {
            let promises = [];
            for (let part of result.entities) {
              promises.push(Xrm.WebApi.deleteRecord("ipg_casepartdetail", part.ipg_casepartdetailid));
            }
            Promise.all(promises).then(() => {
              partViewIsChanged.setValue(false);
              Xrm.Page.data.save();
            });
          }
        },
        function (error) {
          Xrm.Navigation.openErrorDialog({ message: error.message });
        }
      );      
    }    
  }
}
