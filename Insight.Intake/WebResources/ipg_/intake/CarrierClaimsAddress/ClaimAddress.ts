namespace Intake.CarrierClaimAddress
{
  export function GetClaimAddress(executionContext) {
    let formContext = executionContext.getFormContext();
    let CarrierName = formContext.getAttribute("ipg_carrierclaimname").getValue();
    let electricPayerId = formContext.getAttribute("ipg_electronicpayerid").getValue();
    let CarrierNameObject = formContext.getAttribute("ipg_addresscarrierclaimmailingid").getValue();
    let name = CarrierNameObject[0].name;
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

  export function SetCarrierName(executionContext) {  
    let formContext = executionContext.getFormContext();
    let CarrierNameObject = formContext.getAttribute("ipg_addresscarrierclaimmailingid").getValue();
    let name = CarrierNameObject[0].name;
    formContext.getAttribute("ipg_carriername").setValue(name);
  }
}

