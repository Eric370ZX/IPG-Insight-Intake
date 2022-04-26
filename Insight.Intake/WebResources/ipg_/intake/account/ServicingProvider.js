/**
 * @namespace Intake.Account
 */
var Intake;
(function (Intake) {
    var ServiceProvider;
    (function (ServiceProvider) {
        function OnLoadForm(executionContext) {
            var formContext = executionContext.getFormContext();
            var guid = formContext.data.entity.getId();
            if (guid != "") {
                guid = guid.replace("{", "").replace("}", "");
                Xrm.WebApi.retrieveRecord("account", guid, "?$select=ipg_useclaimsaddressfieldsonclaims,ipg_useclaimsaddresstofieldbutfacilityaddress,address2_primarycontactname,ipg_claimspostalcode,address2_line1,ipg_claimsaddresscity,ipg_claimsaddressstateid,ipg_claimsaddressstatename&$expand=ipg_billingstateid($select=ipg_stateid)").then(function success(results) {
                    var addressFieldsOnClaims = results["ipg_useclaimsaddressfieldsonclaims"];
                    var addressToFieldButFacilityAddress = results["ipg_useclaimsaddresstofieldbutfacilityaddress"];
                    var primaryContactName = results["address2_primarycontactname"];
                    var addressPostalCode = results["ipg_claimspostalcode"];
                    var city = results["ipg_claimsaddresscity"];
                    var state = results["ipg_claimsaddressstatename"];
                    var address = results["address2_line1"];
                    GetServiceProviderName(formContext, addressFieldsOnClaims, addressToFieldButFacilityAddress, primaryContactName, addressPostalCode, city, state, address);
                }, function (error) {
                    Xrm.Utility.alertDialog(error.message, null);
                });
                OnChangeAddressType(executionContext);
            }
        }
        ServiceProvider.OnLoadForm = OnLoadForm;
        function OnChangeAddressType(executionContext) {
            var _a, _b;
            var formContext = executionContext.getFormContext();
            SetVisibleTypes(formContext, false, false, false);
            var addrType = formContext.getAttribute("ipg_claimsservicingprovideraddress").getValue();
            if (addrType == 923720001) {
                formContext.getControl("address2_line1").setVisible(false);
                formContext.getControl("ipg_claimspostalcode").setVisible(false);
                formContext.ui.quickForms.get("ClaimsZipCodeDetails").setVisible(false);
                formContext.getControl("ipg_claimsservicingprovideraddress").setVisible(true);
                formContext.getControl("ipg_claimsservicingprovidername").setVisible(true);
                var tabObj = formContext.ui.tabs.get("tab_11");
                var sectionObj = tabObj.sections.get("tab_11_section_7");
                sectionObj.setVisible(true);
            }
            else {
                formContext.getControl("address2_line1").setVisible(true);
                formContext.getControl("ipg_claimspostalcode").setVisible(true);
                formContext.ui.quickForms.get("ClaimsZipCodeDetails").setVisible(true);
                formContext.getControl("ipg_claimsservicingprovideraddress").setVisible(true);
                formContext.getControl("ipg_claimsservicingprovidername").setVisible(true);
                (_a = formContext.getAttribute("ipg_claimspostalcode")) === null || _a === void 0 ? void 0 : _a.setRequiredLevel("required");
                (_b = formContext.getAttribute("address2_line1")) === null || _b === void 0 ? void 0 : _b.setRequiredLevel("required");
                var tabObj = formContext.ui.tabs.get("tab_11");
                var sectionObj = tabObj.sections.get("tab_11_section_7");
                sectionObj.setVisible(false);
            }
        }
        ServiceProvider.OnChangeAddressType = OnChangeAddressType;
        function GetServiceProviderName(formContext, addressFieldsOnClaims, addressToFieldButFacilityAddress, primaryContactName, addressPostalCode, city, state, address) {
            if (addressFieldsOnClaims == false && addressToFieldButFacilityAddress == true) // #1
             {
                if (city == null && address == null) {
                    SetVisibleTypes(formContext, false, false, false);
                }
                if (city != null || address != null) {
                    SetVisibleTypes(formContext, true, true, false);
                    formContext.getAttribute("ipg_claimsservicingprovidername").setValue(923720000);
                    formContext.getAttribute("ipg_claimsservicingprovideraddress").setValue(923720001);
                }
            }
            else if (addressFieldsOnClaims == true && addressToFieldButFacilityAddress == true) // #2
             {
                SetVisibleTypes(formContext, true, true, false);
                if (primaryContactName) {
                    if (primaryContactName.toLowerCase() == "implantable provider group") {
                        formContext.getAttribute("ipg_claimsservicingprovidername").setValue(923720000);
                    }
                }
                if (primaryContactName == null) {
                    formContext.getAttribute("ipg_claimsservicingprovidername").setValue(923720001);
                }
                else {
                    formContext.getAttribute("ipg_claimsservicingprovidername").setValue(923720000);
                }
                formContext.getAttribute("ipg_claimsservicingprovideraddress").setValue(923720001);
            }
            else if (addressFieldsOnClaims == true && addressToFieldButFacilityAddress == false) // #3
             {
                SetVisibleTypes(formContext, true, false, true);
                if (primaryContactName) {
                    if (primaryContactName.toLowerCase() == "implantable provider group") {
                        formContext.getAttribute("ipg_claimsservicingprovidername").setValue(923720000);
                    }
                }
                if (primaryContactName == null) {
                    formContext.getAttribute("ipg_claimsservicingprovidername").setValue(923720001);
                }
                formContext.getAttribute("ipg_claimsservicingprovideraddress").setValue(923720003);
            }
            else if (addressFieldsOnClaims == false && addressToFieldButFacilityAddress == true && city == null && state == null) // #4
             {
                SetVisibleTypes(formContext, false, false, false);
            }
            else // #5
             {
                SetVisibleTypes(formContext, true, true, false);
                formContext.getAttribute("ipg_claimsservicingprovidername").setValue(923720000);
                formContext.getAttribute("ipg_claimsservicingprovideraddress").setValue(923720001);
            }
        }
        ServiceProvider.GetServiceProviderName = GetServiceProviderName;
        function SetVisibleTypes(formContext, blType, blTab, blAddress) {
            formContext.getControl("ipg_claimsservicingprovideraddress").setVisible(blAddress);
            formContext.getControl("address2_line1").setVisible(blAddress);
            var tabObj = formContext.ui.tabs.get("tab_11");
            var sectionObj = tabObj.sections.get("tab_11_section_7");
            sectionObj.setVisible(blTab);
            formContext.getControl("ipg_claimspostalcode").setVisible(blAddress);
            formContext.getControl("ipg_claimsservicingprovideraddress").setVisible(blType);
            formContext.getControl("ipg_claimsservicingprovidername").setVisible(blType);
            formContext.ui.quickForms.get("ClaimsZipCodeDetails").setVisible(blAddress);
        }
        ServiceProvider.SetVisibleTypes = SetVisibleTypes;
    })(ServiceProvider = Intake.ServiceProvider || (Intake.ServiceProvider = {}));
})(Intake || (Intake = {}));
