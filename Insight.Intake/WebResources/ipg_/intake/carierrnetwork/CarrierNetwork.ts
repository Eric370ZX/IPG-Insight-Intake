namespace Intake.CarrierNetwork
{
  export function OnLoadForm(executionContext) {    
    var formContext = executionContext.getFormContext();
    var CName = formContext.getAttribute("ipg_name").getValue();
    localStorage.CarrierNetworkName = CName;
  }
  export function quickCreateOnLoad(executionContext) { 
    let formContext = executionContext.getFormContext(); 
    let CarrierNetwork = localStorage.CarrierNetworkName;
    localStorage.removeItem("CarrierNetworkName");
    if (CarrierNetwork != null) {
      Xrm.WebApi.retrieveMultipleRecords("ipg_carriernetwork", "?$select=ipg_displayname,ipg_expirationdate,ipg_effectivedate,ipg_legacyname &$filter=ipg_name eq '" + CarrierNetwork + "'").then(function success(results) {
        if (results.entities.length) {
          let expirationdate = new Date(results.entities[0]["ipg_expirationdate@OData.Community.Display.V1.FormattedValue"]);
          let effectivedate = new Date(results.entities[0]["ipg_effectivedate@OData.Community.Display.V1.FormattedValue"]);
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

  export function UpdateCarrierNetwork(formContext) { 
    let carrierObject = Xrm.Page.getAttribute("ipg_customcarriernetworkid").getValue();
    if (carrierObject) {
      let updtName = carrierObject[0].name;
      formContext.getAttribute("ipg_name").setValue(updtName);
      formContext.getAttribute("ipg_displaystatus").setValue(true);
    }
  }

  export function UpdateCarrierNetworkStatus(primaryControl) {
    let formContext = primaryControl;
    formContext.getAttribute("ipg_isactive").setValue(false);
  }
  
}
