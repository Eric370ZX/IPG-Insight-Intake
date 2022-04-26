/**
 * @namespace Intake.Case
 */

namespace Intake.Case {

  let accountId;
  let titleId;  
  export function ClaimOnLoad(executionContext) {
    let formContext = executionContext.getFormContext();
    CaseIncident(formContext);
  }


  export function CaseIncident(formContext) {
    let carrierObject = Xrm.Page.getAttribute("ipg_caseid").getValue();
    if (carrierObject != null) {
      let carrierId = carrierObject[0];
      let guid = carrierId.id.replace("{", "").replace("}", "");
      Xrm.WebApi.retrieveRecord("incident", guid, "?$select=title,incidentid&$expand=ipg_CarrierId($select=accountid)").then(function success(results) {
        accountId = results.ipg_CarrierId.accountid.replace("{", "").replace("}", "");
        titleId = results.title;
        CarrierAccount(formContext);
      },
        function (error) {
        });
    }
  }



  export function CarrierAccount(formContext) {
    Xrm.WebApi.retrieveRecord("account", accountId, "?$select=name,ipg_claimtype").then(function success(result) {
      formContext.getAttribute("ipg_claimformtype").setValue(result["ipg_claimtype"]);
      formContext.getAttribute("ipg_id").setValue(titleId);
      let object = new Array();
      object[0] = new Object();
      object[0].id = accountId;
      object[0].name = result.name;
      object[0].entityType = "account";
      formContext.getAttribute("ipg_carrierid").setValue(object);
    }, function (error) {
    });
  }

}
