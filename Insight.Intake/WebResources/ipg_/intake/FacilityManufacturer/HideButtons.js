var Intake;
(function (Intake) {
    var FacilityManufacturerRelationship;
    (function (FacilityManufacturerRelationship) {
        function controlFacilityManButton(commandProperties) {
            var viewId;
            viewId = window.parent.location.href;
            var currView = "account";
            if (viewId.includes(currView)) {
                return true;
            }
            else {
                return false;
            }
        }
        function controlContactButton(commandProperties) {
            var viewId;
            viewId = window.parent.location.href;
            var currView = "ipg_facilitymanufacturerrelationship";
            if (viewId.includes(currView)) {
                return true;
            }
            else {
                return false;
            }
        }
    })(FacilityManufacturerRelationship = Intake.FacilityManufacturerRelationship || (Intake.FacilityManufacturerRelationship = {}));
})(Intake || (Intake = {}));
