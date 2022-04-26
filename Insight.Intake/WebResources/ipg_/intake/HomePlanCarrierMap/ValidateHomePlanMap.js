var Intake;
(function (Intake) {
    var HomePlanCarrierMap;
    (function (HomePlanCarrierMap) {
        function OnSave(executionContext) {
            var formContext = executionContext.getFormContext();
            getHomePlan(formContext);
        }
        HomePlanCarrierMap.OnSave = OnSave;
        function formatDate(date) {
            var d = new Date(date), month = '' + (d.getMonth() + 1), day = '' + d.getDate(), year = d.getFullYear();
            if (month.length < 2)
                month = '0' + month;
            if (day.length < 2)
                day = '0' + day;
            return [month, day, year].join('/');
        }
        HomePlanCarrierMap.formatDate = formatDate;
        function getHomePlan(formContext) {
            var nameEntry = formContext.getAttribute("ipg_name").getValue();
            var code = formContext.getAttribute("ipg_code").getValue();
            var carrierName = formContext.getAttribute("ipg_carrierid").getValue();
            var effectiveDate = formatDate(formContext.getAttribute("ipg_effectivedate").getValue());
            var endDate = formatDate(formContext.getAttribute("ipg_enddate").getValue());
            var carrierNameObject = formContext.getAttribute("ipg_carrierid").getValue();
            var guid = carrierNameObject[0].id.replace("{", "").replace("}", "");
            var effDt;
            var endDt;
            if (formContext.ui.getFormType() == 1) {
                if (code != null && carrierName != null && effectiveDate != null && endDate != null) {
                    Xrm.WebApi.retrieveMultipleRecords("ipg_homeplancarriermap", "?$select=ipg_CarrierId,ipg_code ,ipg_effectivedate,ipg_name,ipg_enddate&$top=1&$expand=ipg_CarrierId($select=name,accountid)&$filter= and ipg_CarrierId/accountid eq " + guid).then(function success(results) {
                        var i = 0;
                        for (i = 0; i < results.entities.length; i++) {
                            effDt = formatDate(results.entities[i]["ipg_effectivedate@OData.Community.Display.V1.FormattedValue"]);
                            endDt = formatDate(results.entities[i]["ipg_enddate@OData.Community.Display.V1.FormattedValue"]);
                            var lCode = results.entities[i]["ipg_code"];
                            var lName = results.entities[i]["ipg_name"];
                            if (effDt == effectiveDate && endDt == endDate && lCode == code && lName == nameEntry) {
                                var alertStrings = { confirmButtonLabel: "Close", text: "This Prefix to Home Plan relationship already exists for this date range", title: "Home Plan Mapping" };
                                var alertOptions = { height: 120, width: 260 };
                                Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(function (success) {
                                    return;
                                }, function (error) {
                                    console.log(error.message);
                                });
                            }
                        }
                    }, function (error) {
                        Xrm.Navigation.openErrorDialog(error);
                    });
                }
            }
        }
        HomePlanCarrierMap.getHomePlan = getHomePlan;
    })(HomePlanCarrierMap = Intake.HomePlanCarrierMap || (Intake.HomePlanCarrierMap = {}));
})(Intake || (Intake = {}));
