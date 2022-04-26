
export function LoadFacilityCPTName(executionContext) {

  let formContext = executionContext.getFormContext();
  formContext.getControl("ipg_cptcodeid").setDisabled(true);
  let effective = formContext.getAttribute("ipg_effectivedate").getValue();
  let expration = formContext.getAttribute("ipg_expirationdate").getValue()
  let currFacilityName = formContext.getAttribute("ipg_facilityid").getValue();

  if (formContext.ui.getFormType() == 1) { // 1 is for create
    formContext.getControl("ipg_facilityid").setVisible(true);
    formContext.getControl("ipg_name").setVisible(false);
    formContext.getControl("ipg_facilityid").setDisabled(false);
    formContext.getControl("ipg_facilitycptname").setDisabled(false);
    formContext.getAttribute("ipg_facilitycptname")?.setRequiredLevel("required");
  }
  else {
    formContext.getControl("ipg_name").setVisible(false);
    formContext.getControl("ipg_facilityid").setDisabled(true);
    formContext.getControl("ipg_cptcodeid").setDisabled(true);
    formContext.getControl("ipg_cptdescription").setDisabled(true);
    formContext.getControl("ipg_facilitycptname").setDisabled(true);
  }

}


export function SetFacility(executionContext) {
  let formContext = executionContext.getFormContext();
  let facilityName = formContext.getAttribute("ipg_facilityid").getValue();

  if (facilityName != null) {
    formContext.getControl("ipg_cptcodeid").setDisabled(false);
  }
}


export function SetCPTDescription(executionContext) {

  let formContext = executionContext.getFormContext();
  let facilityObject = formContext.getAttribute("ipg_facilityid").getValue();
  let facilityname = facilityObject[0].name;


  let cptCodeObject = Xrm.Page.getAttribute("ipg_cptcodeid").getValue();
  if (cptCodeObject != null) {
    let cptCodeId = cptCodeObject[0];
    let cptId = cptCodeId.id;
    let epirationdate_1_1 = new Date('12/31/9999');
    let todayDate_1_1 = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
    Xrm.WebApi.retrieveRecord("ipg_cptcode", cptId, "?$select=ipg_cptcode,ipg_description,ipg_cptname").then(function success(results) {
      if (results != null) {
        Xrm.Page.getAttribute("ipg_facilitycptname").setValue(facilityname + " - " + results["ipg_cptcode"]);
        Xrm.Page.getAttribute("ipg_name").setValue(facilityname + " - " + results["ipg_cptcode"]);
        Xrm.Page.getAttribute("ipg_cptdescription").setValue(results["ipg_description"]);
        Xrm.Page.getAttribute("ipg_effectivedate").setValue(todayDate_1_1);
        Xrm.Page.getAttribute("ipg_expirationdate").setValue(epirationdate_1_1);
      }
    }, function (error) {
    });
  }
}

