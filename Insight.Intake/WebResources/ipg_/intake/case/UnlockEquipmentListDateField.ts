/**
 * @namespace Intake.Case
 */
namespace Intake.Case {
  /**
  * Enable the accurate equipment list field received
  * date field if the user is a case manager member.
  *
  * @function Intake.Case.UnlockEquipmentListDateField
  * @returns {void}
  */
  export function UnlockEquipmentListDateField(executionContext: Xrm.Events.EventContext) {
    //debugger;
    var formContext = executionContext.getFormContext();
    var userId = Xrm.Utility.getGlobalContext().getUserId().replace("{", "").replace("}", "");
    let equipmentListDateField = <Xrm.Controls.StandardControl>formContext.getControl("ipg_accurate_equipment_list_received");

    if (equipmentListDateField) {
      Xrm.WebApi.online.retrieveMultipleRecords("systemuser", "?$expand=teammembership_association($select=teamid,name)&$filter=systemuserid eq " + userId).then(
        (results) => {
          if (results && results.entities && results.entities.length) {
            var teammembership_associations = results.entities[0].teammembership_association;
            var caseManagerTeam = teammembership_associations.filter((team) => {
              return team.name === "Case Management";
            });
            if (caseManagerTeam.length) {
              equipmentListDateField.setDisabled(false);
              CheckApprovedPartsOnPartsList(executionContext);
            }
            else {
              equipmentListDateField.setDisabled(true);
            }
          }
        },
        (error: Xrm.Navigation.ErrorDialogOptions) => {
          Xrm.Navigation.openErrorDialog(error);
        });
    }
  }



  /**
  * Prevent the accurate equipment list field from being set
  * if there is not at least one approved part added to
  * the case.
  * @function Intake.Case.CheckApprovedPartsOnPartsList
  * @returns {void}
  */
  export function CheckApprovedPartsOnPartsList(executionContext: Xrm.Events.EventContext) {

    //debugger;

    var formContext = executionContext.getFormContext();

    let caseId = Xrm.Page.data.entity.getId().replace("{", "").replace("}", "");

    let equipmentListDateField = <Xrm.Controls.StandardControl>formContext.getControl("ipg_accurate_equipment_list_received");

    let approvedPartAdded = false;

    let partApprovedStatus = 923720000;

    if (equipmentListDateField) {
      Xrm.WebApi.online.retrieveMultipleRecords("ipg_casepartdetail", "?$select=ipg_casepartdetailid&$expand=ipg_productid($select=ipg_status)&$filter=_ipg_caseid_value eq " + caseId).then(function (results) {
        for (var i = 0; i < results.entities.length; i++) {
          var status = results.entities[i].ipg_productid.ipg_status;
          if (status === partApprovedStatus) {
            approvedPartAdded = true;
            break;
          }
        }
        if (!approvedPartAdded) {
          equipmentListDateField.setDisabled(true);
        }
        else {
          equipmentListDateField.setDisabled(false);
        }
      },
        function (error) {
          Xrm.Navigation.openErrorDialog(error);
        });
    }
  }
}

