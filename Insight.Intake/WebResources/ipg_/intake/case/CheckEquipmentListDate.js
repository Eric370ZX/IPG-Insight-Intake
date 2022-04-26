/**
 * @namespace Intake.Case
 */
var Intake;
(function (Intake) {
    var Case;
    (function (Case) {
        /**
        * Show error message if Actual DOS field
        * contains a date value that is less than the
        * Accurate Equipment List Received field's date value.
        * @function Intake.Case.CheckEquipmentListDate
        * @returns {void}
        */
        function CheckEquipmentListDate(executionContext) {
            //debugger;
            var formContext = executionContext.getFormContext();
            var equipmentListDateAttribute = formContext.getAttribute("ipg_accurate_equipment_list_received");
            if (equipmentListDateAttribute) {
                var equipmentListDate = equipmentListDateAttribute.getValue();
                if (equipmentListDate) {
                    var actualDOSDateAttribute = formContext.getAttribute("ipg_actualdos");
                    var equipmentListDateField = formContext.getControl("ipg_accurate_equipment_list_received");
                    {
                        if (actualDOSDateAttribute && equipmentListDateField) {
                            var actualDOSDate = actualDOSDateAttribute.getValue();
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
        Case.CheckEquipmentListDate = CheckEquipmentListDate;
    })(Case = Intake.Case || (Intake.Case = {}));
})(Intake || (Intake = {}));
