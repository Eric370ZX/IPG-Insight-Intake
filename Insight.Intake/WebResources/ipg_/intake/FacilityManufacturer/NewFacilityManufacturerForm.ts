namespace Intake.FacilityManufacturerRelationship {
  export function DisplayFacilityManufactuerNewForm(primaryControl) {
    
    let formContext = primaryControl;
    let name = formContext.getAttribute("name").getValue();
    let  formid = "3dF6BC24ED-298D-4453-AEC3-31EE8E6EE3A1";
    let  url = window.parent.location.href;
    let strLocation = url.indexOf("&id=");
    let id = url.substr(strLocation + 4, 36);

    localStorage.setItem("accountId", id);  
    localStorage.setItem("accountName", name);

    let  parameters = {
      formid: formid

    };
    let  entityFormOptions = {};
    entityFormOptions["entityName"] = "ipg_facilitymanufacturerrelationship";
    Xrm.Navigation.openForm(entityFormOptions, parameters);
  }


  export function SetManufacturerView() {    
    localStorage.setItem("ManufacturerView", "true");
  }


  export function SetFacilityView() {    
    localStorage.setItem("FacilityView", "true");
  }


}
