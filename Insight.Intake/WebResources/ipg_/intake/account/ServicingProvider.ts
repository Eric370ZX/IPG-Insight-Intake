

/**
 * @namespace Intake.Account
 */
namespace Intake.ServiceProvider {

  export function OnLoadForm(executionContext) {
    let formContext = executionContext.getFormContext();

    let guid = formContext.data.entity.getId();
    if (guid != "") {
      guid = guid.replace("{", "").replace("}", "");
      Xrm.WebApi.retrieveRecord("account", guid, "?$select=ipg_useclaimsaddressfieldsonclaims,ipg_useclaimsaddresstofieldbutfacilityaddress,address2_primarycontactname,ipg_claimspostalcode,address2_line1,ipg_claimsaddresscity,ipg_claimsaddressstateid,ipg_claimsaddressstatename&$expand=ipg_billingstateid($select=ipg_stateid)").then(
        function success(results) {
          let addressFieldsOnClaims = results["ipg_useclaimsaddressfieldsonclaims"];
          let addressToFieldButFacilityAddress = results["ipg_useclaimsaddresstofieldbutfacilityaddress"]
          let primaryContactName = results["address2_primarycontactname"];
          let addressPostalCode = results["ipg_claimspostalcode"];
          let city = results["ipg_claimsaddresscity"];
          let state = results["ipg_claimsaddressstatename"];
          let address = results["address2_line1"];
          GetServiceProviderName(formContext, addressFieldsOnClaims, addressToFieldButFacilityAddress, primaryContactName, addressPostalCode, city, state, address);
        },
        function (error) {
          Xrm.Utility.alertDialog(error.message, null);
        }
      );

      OnChangeAddressType(executionContext);
    }

  }

  export function OnChangeAddressType(executionContext) {
    let formContext = executionContext.getFormContext();
    SetVisibleTypes(formContext, false, false, false)
    let addrType = formContext.getAttribute("ipg_claimsservicingprovideraddress").getValue();
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
      formContext.getAttribute("ipg_claimspostalcode")?.setRequiredLevel("required");
      formContext.getAttribute("address2_line1")?.setRequiredLevel("required");
      var tabObj = formContext.ui.tabs.get("tab_11");
      var sectionObj = tabObj.sections.get("tab_11_section_7");
      sectionObj.setVisible(false);
    }

  }

  export function GetServiceProviderName(formContext: Xrm.FormContext,
    addressFieldsOnClaims: boolean,
    addressToFieldButFacilityAddress: boolean,
    primaryContactName: string,
    addressPostalCode: string,
    city: string,
    state: string,
    address: string) {
    if (addressFieldsOnClaims == false && addressToFieldButFacilityAddress == true)                             // #1
    {
      if (city == null && address == null) {
        SetVisibleTypes(formContext, false, false, false);
      }

      if (city != null || address != null) {

        SetVisibleTypes(formContext, true, true, false)

        formContext.getAttribute("ipg_claimsservicingprovidername").setValue(923720000);
        formContext.getAttribute("ipg_claimsservicingprovideraddress").setValue(923720001);
      }
    }
    else if (addressFieldsOnClaims == true && addressToFieldButFacilityAddress == true)                            // #2
    {
      SetVisibleTypes(formContext, true, true, false)
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

    else if (addressFieldsOnClaims == true && addressToFieldButFacilityAddress == false)                              // #3
    {
      SetVisibleTypes(formContext, true, false, true)
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
      SetVisibleTypes(formContext, false, false, false)

    }
    else                                                                                                                 // #5
    {
      SetVisibleTypes(formContext, true, true, false);
      formContext.getAttribute("ipg_claimsservicingprovidername").setValue(923720000);
      formContext.getAttribute("ipg_claimsservicingprovideraddress").setValue(923720001);
    }


  }
  export function SetVisibleTypes(formContext: Xrm.FormContext, blType: boolean, blTab: boolean, blAddress: boolean) {

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
}
