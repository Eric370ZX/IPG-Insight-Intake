/**
 * @namespace Intake.Case
 */
namespace Intake.Case {
  /**
  * Show error message if Actual DOS field
  * contains a date value that is less than the
  * Accurate Equipment List Received field's date value.
  * @function Intake.Case.CheckEquipmentListDate
  * @returns {void}
  */
  export function CheckEquipmentListDate(executionContext: Xrm.Events.EventContext) {

    //debugger;

    var formContext = executionContext.getFormContext();

    let equipmentListDateAttribute: Xrm.Attributes.Attribute = formContext.getAttribute("ipg_accurate_equipment_list_received");

    if (equipmentListDateAttribute) {
      let equipmentListDate = equipmentListDateAttribute.getValue();
      if (equipmentListDate) {
        let actualDOSDateAttribute: Xrm.Attributes.Attribute = formContext.getAttribute("ipg_actualdos");
        let equipmentListDateField = <Xrm.Controls.StandardControl>formContext.getControl("ipg_accurate_equipment_list_received");
        {
          if (actualDOSDateAttribute && equipmentListDateField) {
            let actualDOSDate = actualDOSDateAttribute.getValue();
            if (actualDOSDate) {
              if (equipmentListDate < actualDOSDate) {
                equipmentListDateField.setNotification("Accurate Equipment List Received date can not be prior to Actual Date of Surgery.  Please correct and try again.", "equipmentListDate");
              }
              else {
                equipmentListDateField.clearNotification("equipmentListDate");
              }
            }
          }
        }
      }
    }
  }
}


