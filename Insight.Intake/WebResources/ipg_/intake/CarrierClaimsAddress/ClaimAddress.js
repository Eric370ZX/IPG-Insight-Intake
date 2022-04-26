var Intake;
(function (Intake) {
    var CarrierClaimAddress;
    (function (CarrierClaimAddress) {
        function GetClaimAddress(executionContext) {
            var formContext = executionContext.getFormContext();
            var CarrierName = formContext.getAttribute("ipg_carrierclaimname").getValue();
            var electricPayerId = formContext.getAttribute("ipg_electronicpayerid").getValue();
            var CarrierNameObject = formContext.getAttribute("ipg_addresscarrierclaimmailingid").getValue();
            var name = CarrierNameObject[0].name;
            if (CarrierName != null) {
                Xrm.WebApi.retrieveMultipleRecords("ipg_carrierclaimsmailingaddress", "?$select=ipg_electronicpayerid,ipg_claimsmailingaddress,ipg_claimsmailingcity,ipg_claimsmailingstate,ipg_carriername,ipg_carrierclaimname&$filter=ipg_carrierclaimname eq '" + CarrierName + "' and ipg_carriername eq  '" + name + "'").then(function success(results) {
                    if (results.entities.length) {
                        formContext.getAttribute("ipg_carrierclaimname").setValue() == null;
                        formContext.ui.setFormNotification("Carrier Claim Name Exists,", "ERROR");
                    }
                }, function (error) {
                    console.log(error.message);
                });
            }
        }
        CarrierClaimAddress.GetClaimAddress = GetClaimAddress;
        function SetCarrierName(executionContext) {
            var formContext = executionContext.getFormContext();
            var CarrierNameObject = formContext.getAttribute("ipg_addresscarrierclaimmailingid").getValue();
            var name = CarrierNameObject[0].name;
            formContext.getAttribute("ipg_carriername").setValue(name);
        }
        CarrierClaimAddress.SetCarrierName = SetCarrierName;
    })(CarrierClaimAddress = Intake.CarrierClaimAddress || (Intake.CarrierClaimAddress = {}));
})(Intake || (Intake = {}));
