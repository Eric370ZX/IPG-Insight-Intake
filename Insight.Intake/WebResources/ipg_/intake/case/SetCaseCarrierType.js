var Intake;
(function (Intake) {
    var Case;
    (function (Case) {
        function GetCarrierType(executionContext) {
            var formContext = executionContext.getFormContext();
            var tabObj = formContext.ui.tabs.get("PatientProcedureDetails"); //the tab where your section is
            var sectionObj = tabObj.sections.get("additionalInfornaionSection"); // section name
            sectionObj.setVisible(false);
            var carrierName = formContext.getAttribute("ipg_carrierid").getValue();
            var carrierObject = carrierName[0];
            var guid = carrierObject.id;
            ;
            if (carrierName != null) {
                Xrm.WebApi.retrieveRecord("account", guid, "?$select=ipg_carriersupportedplantypes").then(function success(results) {
                    var _a, _b, _c;
                    if (results) {
                        if ((results["ipg_carriersupportedplantypes@OData.Community.Display.V1.FormattedValue"] === "Auto") || (results["ipg_carriersupportedplantypes@OData.Community.Display.V1.FormattedValue"] === "Workers Comp")) {
                            sectionObj.setVisible(true);
                            (_a = formContext.getAttribute("ipg_autodateofincident")) === null || _a === void 0 ? void 0 : _a.setRequiredLevel("required");
                            (_b = formContext.getAttribute("ipg_autoadjustername")) === null || _b === void 0 ? void 0 : _b.setRequiredLevel("required");
                            (_c = formContext.getAttribute("ipg_adjusterphone")) === null || _c === void 0 ? void 0 : _c.setRequiredLevel("required");
                            formContext.getControl("ipg_homeplancarrierid").setVisible(false);
                            formContext.getControl("ipg_primarycarriergroupidnumber").setVisible(false);
                            formContext.getControl("ipg_plansponsor").setVisible(false);
                        }
                    }
                }, function (error) {
                    console.log(error.message);
                });
            }
        }
        Case.GetCarrierType = GetCarrierType;
        function GetReferralType(executionContext) {
            var formContext = executionContext.getFormContext();
            var carrierName = formContext.getAttribute("ipg_carrierid").getValue();
            if (carrierName != null) {
                var carrierObject = carrierName[0];
                var guid = carrierObject.id;
                ;
                Xrm.WebApi.retrieveRecord("account", guid, "?$select=ipg_carriertype").then(function success(results) {
                    var _a, _b, _c;
                    if (results) {
                        if ((results["ipg_carriertype@OData.Community.Display.V1.FormattedValue"] === "Auto") || (results["ipg_carriertype@OData.Community.Display.V1.FormattedValue"] === "Workers Comp")) {
                            (_a = formContext.getAttribute("ipg_autodateofincident")) === null || _a === void 0 ? void 0 : _a.setRequiredLevel("required");
                            (_b = formContext.getAttribute("ipg_autoadjustername")) === null || _b === void 0 ? void 0 : _b.setRequiredLevel("required");
                            (_c = formContext.getAttribute("ipg_autoadjusterphone")) === null || _c === void 0 ? void 0 : _c.setRequiredLevel("required");
                        }
                    }
                }, function (error) {
                    console.log(error.message);
                });
            }
        }
        Case.GetReferralType = GetReferralType;
    })(Case = Intake.Case || (Intake.Case = {}));
})(Intake || (Intake = {}));
