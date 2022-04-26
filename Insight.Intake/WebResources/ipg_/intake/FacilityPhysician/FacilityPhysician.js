var Intake;
(function (Intake) {
    var FacilityPhysician;
    (function (FacilityPhysician) {
        function OnLoadQuickCreateForm(executionContext) {
            var formContext = executionContext.getFormContext();
            var facilityAttr = formContext.getAttribute("ipg_facilityid");
            var facilityControl = formContext.getControl("ipg_facilityid");
            var physicianAttr = formContext.getAttribute("ipg_physicianid");
            var physicianControl = formContext.getControl("ipg_physicianid");
            if (facilityAttr && facilityControl &&
                physicianAttr && physicianControl) {
                if (facilityAttr.getValue()) {
                    facilityControl.setVisible(false);
                    physicianControl.setVisible(true);
                }
                else if (physicianAttr.getValue()) {
                    facilityControl.setVisible(true);
                    physicianControl.setVisible(false);
                }
                else {
                    facilityControl.setVisible(true);
                    physicianControl.setVisible(true);
                }
            }
        }
        FacilityPhysician.OnLoadQuickCreateForm = OnLoadQuickCreateForm;
    })(FacilityPhysician = Intake.FacilityPhysician || (Intake.FacilityPhysician = {}));
})(Intake || (Intake = {}));
