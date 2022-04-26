"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.SetCPTDescription = exports.SetFacility = exports.LoadFacilityCPTName = void 0;
function LoadFacilityCPTName(executionContext) {
    var _a;
    var formContext = executionContext.getFormContext();
    formContext.getControl("ipg_cptcodeid").setDisabled(true);
    var effective = formContext.getAttribute("ipg_effectivedate").getValue();
    var expration = formContext.getAttribute("ipg_expirationdate").getValue();
    var currFacilityName = formContext.getAttribute("ipg_facilityid").getValue();
    if (formContext.ui.getFormType() == 1) { // 1 is for create
        formContext.getControl("ipg_facilityid").setVisible(true);
        formContext.getControl("ipg_name").setVisible(false);
        formContext.getControl("ipg_facilityid").setDisabled(false);
        formContext.getControl("ipg_facilitycptname").setDisabled(false);
        (_a = formContext.getAttribute("ipg_facilitycptname")) === null || _a === void 0 ? void 0 : _a.setRequiredLevel("required");
    }
    else {
        formContext.getControl("ipg_name").setVisible(false);
        formContext.getControl("ipg_facilityid").setDisabled(true);
        formContext.getControl("ipg_cptcodeid").setDisabled(true);
        formContext.getControl("ipg_cptdescription").setDisabled(true);
        formContext.getControl("ipg_facilitycptname").setDisabled(true);
    }
}
exports.LoadFacilityCPTName = LoadFacilityCPTName;
function SetFacility(executionContext) {
    var formContext = executionContext.getFormContext();
    var facilityName = formContext.getAttribute("ipg_facilityid").getValue();
    if (facilityName != null) {
        formContext.getControl("ipg_cptcodeid").setDisabled(false);
    }
}
exports.SetFacility = SetFacility;
function SetCPTDescription(executionContext) {
    var formContext = executionContext.getFormContext();
    var facilityObject = formContext.getAttribute("ipg_facilityid").getValue();
    var facilityname = facilityObject[0].name;
    var cptCodeObject = Xrm.Page.getAttribute("ipg_cptcodeid").getValue();
    if (cptCodeObject != null) {
        var cptCodeId = cptCodeObject[0];
        var cptId = cptCodeId.id;
        var epirationdate_1_1_1 = new Date('12/31/9999');
        var todayDate_1_1_1 = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
        Xrm.WebApi.retrieveRecord("ipg_cptcode", cptId, "?$select=ipg_cptcode,ipg_description,ipg_cptname").then(function success(results) {
            if (results != null) {
                Xrm.Page.getAttribute("ipg_facilitycptname").setValue(facilityname + " - " + results["ipg_cptcode"]);
                Xrm.Page.getAttribute("ipg_name").setValue(facilityname + " - " + results["ipg_cptcode"]);
                Xrm.Page.getAttribute("ipg_cptdescription").setValue(results["ipg_description"]);
                Xrm.Page.getAttribute("ipg_effectivedate").setValue(todayDate_1_1_1);
                Xrm.Page.getAttribute("ipg_expirationdate").setValue(epirationdate_1_1_1);
            }
        }, function (error) {
        });
    }
}
exports.SetCPTDescription = SetCPTDescription;
