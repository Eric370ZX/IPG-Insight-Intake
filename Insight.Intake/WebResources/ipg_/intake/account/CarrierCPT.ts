function OnLoadForm(executionContext) {
  let formContext = executionContext.getFormContext();
  formContext.getControl("ipg_cptcodeid").setDisabled(true);
  LoadCarrierCPTName(formContext);

}



function LoadCarrierCPTName(formContext) {

  let effective = formContext.getAttribute("ipg_effectivedate").getValue();
  let expration = formContext.getAttribute("ipg_expirationdate").getValue()
  let carrierName = formContext.getAttribute("ipg_carrierid").getValue();
  if ((carrierName != null && effective == null && expration == null) || (carrierName == null && effective == null && expration == null)) {

    formContext.getControl("ipg_carriercptname").setDisabled(true);
    formContext.getControl("ipg_carrierid").setVisible(true);
    formContext.getControl("ipg_name").setVisible(false);  

  }
  else {
    formContext.getControl("ipg_name").setVisible(false);
    formContext.getControl("ipg_carrierid").setDisabled(true);
    formContext.getControl("ipg_cptcodeid").setDisabled(true);
    formContext.getControl("ipg_cptdescription").setDisabled(true);
  }
}


function SetCarrier(executionContext) {
  let formContext = executionContext.getFormContext();
  let carrierName = formContext.getAttribute("ipg_carrierid").getValue();

  if (carrierName != null) {
    formContext.getControl("ipg_cptcodeid").setDisabled(false);
    formContext.getAttribute("ipg_displaycarriername").setValue(carrierName[0].name);
  }
}
function UpdateCPTDescription(formContext) {
  var cptCodeObject = Xrm.Page.getAttribute("ipg_cptcodeid").getValue();
  if (cptCodeObject != null) {
    var cptCodeId = cptCodeObject[0];
    var cptId = cptCodeId.id;
    var expirationdate = new Date('12/31/9999');
  
    var todayDate = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
    var carrierNameCPT = Xrm.Page.getAttribute("ipg_displaycarriername").getValue();
    Xrm.WebApi.retrieveRecord("ipg_cptcode", cptId, "?$select=ipg_cptcode,ipg_description,ipg_cptname").then(function success(results) {
      Xrm.Page.getAttribute("ipg_carriercptname").setValue(carrierNameCPT + " - " + results["ipg_cptcode"]);
      Xrm.Page.getAttribute("ipg_cptdescription").setValue(results["ipg_description"]);
      Xrm.Page.getAttribute("ipg_effectivedate").setValue(todayDate);
      Xrm.Page.getAttribute("ipg_expirationdate").setValue(expirationdate);
    }, function (error) {
    });
  }
}

