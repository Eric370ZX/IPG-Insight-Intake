"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetStateAbbrevivation = exports.SetUpdatedVendorID = void 0;
function SetUpdatedVendorID() {
    var manufacturer = Xrm.Page.getAttribute("name").getValue().trim();
    var state = Xrm.Page.getAttribute("ipg_stateid").getValue();
    if (state == null) {
        return;
    }
    var stateObject = state[0];
    var stateName = stateObject.name;
    var currVendorID = Xrm.Page.getAttribute("ipg_vendoridnumber").getValue();
    if (manufacturer.length == 0 || currVendorID)
        return;
    var vendorID = "F" + manufacturer.charAt(0).toUpperCase();
    var index = 0;
    var pos = manufacturer.indexOf(" ");
    var maxLen = manufacturer.split(' ').length;
    while (pos != -1 && index < maxLen) {
        manufacturer = manufacturer.substr(pos).trim();
        if (manufacturer.length > 0)
            vendorID = vendorID + manufacturer.charAt(0).toUpperCase();
        pos = manufacturer.indexOf(" ");
        index++;
    }
    Xrm.WebApi.retrieveMultipleRecords("account", "?$select=ipg_vendoridnumber&$filter=ipg_vendoridnumber eq '" + vendorID + "'").then(function success(result) {
        if (result.entities.length == 0) {
            Xrm.Page.getAttribute("ipg_vendoridnumber").setValue(vendorID + "" + localStorage.Abv);
            Xrm.Page.getControl("ipg_vendoridnumber").setDisabled(true);
        }
        else {
            Xrm.Page.getAttribute("ipg_vendoridnumber").setValue(result.entities[0]["ipg_vendoridnumber"]);
            Xrm.Page.getControl("ipg_manufacturergreatplainsvendorid").setDisabled(true);
        }
        localStorage.removeItem("Abv");
    }, function (error) {
    });
    GetStateAbbrevivation(stateName);
}
exports.SetUpdatedVendorID = SetUpdatedVendorID;
function GetStateAbbrevivation(stateName) {
    Xrm.WebApi.retrieveMultipleRecords("ipg_state", "?$select=ipg_abbreviation&$filter=ipg_name eq '" + stateName + "'").then(function success(results) {
        if (results.entities.length == 0) {
            return "--";
        }
        else {
            localStorage.Abv = results.entities[0]["ipg_abbreviation"];
        }
    }, function (error) {
    });
}
exports.GetStateAbbrevivation = GetStateAbbrevivation;
