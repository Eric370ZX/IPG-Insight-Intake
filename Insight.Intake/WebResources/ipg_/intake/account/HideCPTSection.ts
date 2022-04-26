namespace Account {

  export function CPTCodeFormOnLoad(executionContext) {
    HideCPTSection(executionContext);
    UpdateCPTRequiredLevel(executionContext);
    SetDefaultExpirationDate(executionContext);
    SetHighDollarDefaultValue(executionContext);
  }

  function HideCPTSection(executionContext) {
   
    let formContext = executionContext.getFormContext();
    let tabObj = formContext.ui.tabs.get("CPTCode");
    let sectionObj = tabObj.sections.get("BlockedByCarrier"); 
    sectionObj.setVisible(false);
  }

  function SetHighDollarDefaultValue(executionContext) {
    let formContext = executionContext.getFormContext();
    let highDollar = formContext.getAttribute("ipg_highdollar").getValue();
    if (highDollar == null) {
      formContext.getAttribute("ipg_highdollar").setValue(false);
    }
  }
  export function UpdateCPTRequiredLevel(executionContext) {
   
    let formContext = executionContext.getFormContext();
    let implantUsed = formContext.getAttribute("ipg_implantused").getValue();
    if (implantUsed != 923720001) {
      formContext.getAttribute("ipg_procedurename").setRequiredLevel("required");
      formContext.getAttribute("ipg_cptroupreporting").setRequiredLevel("required");
      formContext.getAttribute("ipg_cptgroupgeneral").setRequiredLevel("required");
      formContext.getAttribute("ipg_cptgroupdetail").setRequiredLevel("required");
      formContext.getAttribute("ipg_apcweight").setRequiredLevel("required");
    }
    else {
      formContext.getAttribute("ipg_procedurename").setRequiredLevel("none");
      formContext.getAttribute("ipg_cptroupreporting").setRequiredLevel("none");
      formContext.getAttribute("ipg_cptgroupgeneral").setRequiredLevel("none");
      formContext.getAttribute("ipg_cptgroupdetail").setRequiredLevel("none");
      formContext.getAttribute("ipg_apcweight").setRequiredLevel("none");
    }

  }

  function SetDefaultExpirationDate(executionContext) {
   
    let formContext = executionContext.getFormContext();
    let expDate = formContext.getAttribute("ipg_expirationdate").getValue()
    let epirationdate = new Date('12/31/9999');
    if (expDate == null) {
      formContext.getAttribute("ipg_expirationdate").setValue(epirationdate);
    }

  }

}
