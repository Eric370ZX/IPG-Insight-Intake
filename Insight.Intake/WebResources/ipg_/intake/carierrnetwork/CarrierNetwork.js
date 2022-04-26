var Intake;
(function (Intake) {
    var CarrierNetwork;
    (function (CarrierNetwork_1) {
        function OnLoadForm(executionContext) {
            var formContext = executionContext.getFormContext();
            var CName = formContext.getAttribute("ipg_name").getValue();
            localStorage.CarrierNetworkName = CName;
        }
        CarrierNetwork_1.OnLoadForm = OnLoadForm;
        function quickCreateOnLoad(executionContext) {
            var formContext = executionContext.getFormContext();
            var CarrierNetwork = localStorage.CarrierNetworkName;
            localStorage.removeItem("CarrierNetworkName");
            if (CarrierNetwork != null) {
                Xrm.WebApi.retrieveMultipleRecords("ipg_carriernetwork", "?$select=ipg_displayname,ipg_expirationdate,ipg_effectivedate,ipg_legacyname &$filter=ipg_name eq '" + CarrierNetwork + "'").then(function success(results) {
                    if (results.entities.length) {
                        var expirationdate = new Date(results.entities[0]["ipg_expirationdate@OData.Community.Display.V1.FormattedValue"]);
                        var effectivedate = new Date(results.entities[0]["ipg_effectivedate@OData.Community.Display.V1.FormattedValue"]);
                        formContext.getAttribute("ipg_displayname").setValue(results.entities[0]["ipg_displayname"]);
                        formContext.getAttribute("ipg_expirationdate").setValue(expirationdate);
                        formContext.getAttribute("ipg_effectivedate").setValue(effectivedate);
                        formContext.getAttribute("ipg_legacyname").setValue(results.entities[0]["ipg_legacyname"]);
                        UpdateCarrierNetwork(formContext);
                    }
                }, function (error) {
                    console.log(error.message);
                });
            }
        }
        CarrierNetwork_1.quickCreateOnLoad = quickCreateOnLoad;
        function UpdateCarrierNetwork(formContext) {
            var carrierObject = Xrm.Page.getAttribute("ipg_customcarriernetworkid").getValue();
            if (carrierObject) {
                var updtName = carrierObject[0].name;
                formContext.getAttribute("ipg_name").setValue(updtName);
                formContext.getAttribute("ipg_displaystatus").setValue(true);
            }
        }
        CarrierNetwork_1.UpdateCarrierNetwork = UpdateCarrierNetwork;
        function UpdateCarrierNetworkStatus(primaryControl) {
            var formContext = primaryControl;
            formContext.getAttribute("ipg_isactive").setValue(false);
        }
        CarrierNetwork_1.UpdateCarrierNetworkStatus = UpdateCarrierNetworkStatus;
    })(CarrierNetwork = Intake.CarrierNetwork || (Intake.CarrierNetwork = {}));
})(Intake || (Intake = {}));
