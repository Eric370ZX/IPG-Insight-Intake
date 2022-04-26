namespace Intake.FacilityManufacturerRelationship {
  export function RepresentativeContactInfo(executionContext) {
    
    let formContext = executionContext.getFormContext();
    let rep = formContext.getAttribute("ipg_manufacturerrep").getValue();
    let repPhone = formContext.getAttribute("ipg_manufacturerrepphone").getValue();
    let repEmail = formContext.getAttribute("ipg_manufacturerrepemailmask").getValue();
    formContext.getAttribute("ipg_manufacturerrepemailmask")?.setRequiredLevel("none");
    formContext.getAttribute("ipg_manufacturerrepphone")?.setRequiredLevel("none");
    formContext.getAttribute("ipg_manufacturerrep")?.setRequiredLevel("none");


    if (rep === null && repPhone === null && repEmail === null) {
      formContext.getAttribute("ipg_manufacturerrepemailmask")?.setRequiredLevel("required");
      formContext.getAttribute("ipg_manufacturerrepphone")?.setRequiredLevel("required");
      formContext.getAttribute("ipg_manufacturerrep")?.setRequiredLevel("required");

    }

    if (rep != null) {
      formContext.getAttribute("ipg_manufacturerrepphone")?.setRequiredLevel("none");
      formContext.getAttribute("ipg_manufacturerrepemailmask")?.setRequiredLevel("none");
      formContext.getAttribute("ipg_manufacturerrep")?.setRequiredLevel("required");
    }
   


    if (repPhone != null && repEmail != null) {
      formContext.getAttribute("ipg_manufacturerrep")?.setRequiredLevel("none");
    }
   
  }

}

