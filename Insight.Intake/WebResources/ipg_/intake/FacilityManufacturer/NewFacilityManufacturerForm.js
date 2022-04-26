var Intake;
(function (Intake) {
    var FacilityManufacturerRelationship;
    (function (FacilityManufacturerRelationship) {
        function DisplayFacilityManufactuerNewForm(primaryControl) {
            var formContext = primaryControl;
            var name = formContext.getAttribute("name").getValue();
            var formid = "3dF6BC24ED-298D-4453-AEC3-31EE8E6EE3A1";
            var url = window.parent.location.href;
            var strLocation = url.indexOf("&id=");
            var id = url.substr(strLocation + 4, 36);
            localStorage.setItem("accountId", id);
            localStorage.setItem("accountName", name);
            var parameters = {
                formid: formid
            };
            var entityFormOptions = {};
            entityFormOptions["entityName"] = "ipg_facilitymanufacturerrelationship";
            Xrm.Navigation.openForm(entityFormOptions, parameters);
        }
        FacilityManufacturerRelationship.DisplayFacilityManufactuerNewForm = DisplayFacilityManufactuerNewForm;
        function SetManufacturerView() {
            localStorage.setItem("ManufacturerView", "true");
        }
        FacilityManufacturerRelationship.SetManufacturerView = SetManufacturerView;
        function SetFacilityView() {
            localStorage.setItem("FacilityView", "true");
        }
        FacilityManufacturerRelationship.SetFacilityView = SetFacilityView;
    })(FacilityManufacturerRelationship = Intake.FacilityManufacturerRelationship || (Intake.FacilityManufacturerRelationship = {}));
})(Intake || (Intake = {}));
