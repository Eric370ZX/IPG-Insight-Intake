namespace intake.Account {
  export function SetCarrierCPTDescription(formContext) {

    let  cptCodeObject = Xrm.Page.getAttribute("ipg_cptcodeid").getValue();

    if (cptCodeObject != null) {
      let  cptCodeId = cptCodeObject[0];
      let  cptId;
      
      let  epirationdate = new Date('12/31/9999');
      let  todayDate = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());     
      let  guid = cptId.id.replace("{", "").replace("}", "");
      Xrm.WebApi.retrieveRecord("ipg_cptcode", guid, "?$select=ipg_cptcode,ipg_description,ipg_cptname").then(function success(results) {

        Xrm.Page.getAttribute("ipg_carriercptname").setValue(results["ipg_cptname"] + " - " + results["ipg_cptcode"]);
        Xrm.Page.getAttribute("ipg_cptdescription").setValue(results["ipg_description"]);
        Xrm.Page.getAttribute("ipg_effectivedate").setValue(todayDate);
        Xrm.Page.getAttribute("ipg_expirationdate").setValue(epirationdate);
      }, function (error) {

      });

    }

  }
 


  export function DisplayCarrierCPTNewForm(primaryControl) {

    let  formContext = primaryControl;
    let  entityFormOptions = {};
    entityFormOptions["entityName"] = "ipg_associatedcpt";
    entityFormOptions["useQuickCreateForm"] = true;
    let  formParameters = {};
    formParameters["ipg_accountid"] = formContext.data.entity.getId();
    formParameters["ipg_accountidname"] = formContext.getAttribute("name").getValue();
    Xrm.Navigation.openForm(entityFormOptions, formParameters).then(function (success) {
      console.log(success);
    }, function (error) {
      console.log(error);
    });

  }


  export function DisplayFacilityCPTNewForm(primaryControl) {

    let  formid = "3d6E272CCE-A0FD-4F48-B0E7-72B28769E290";
    let  formContext = primaryControl;
    let  parameters = { formid: formid };
    let  entityFormOptions = {};
    entityFormOptions["entityName"] = "ipg_facilitycpt";
    let  recordId = formContext.data.entity.getId();
    Xrm.Navigation.openForm(entityFormOptions, parameters);
  }

  export function SaveAndClose() {

    Xrm.Page.data.entity.save("saveandclose")

  }

}
