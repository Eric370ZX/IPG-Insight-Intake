namespace Intake.FacilityManufacturerRelationship {

  function controlFacilityManButton(commandProperties) {
    let viewId;
    viewId = window.parent.location.href;
    let currView = "account";
    if (viewId.includes(currView)) {
      return true;
    }
    else {
      return false;
    }

  }

  function controlContactButton(commandProperties) {
    var viewId;
    viewId = window.parent.location.href;
    var currView = "ipg_facilitymanufacturerrelationship";
    if (viewId.includes(currView)) {
      return true;
    }
    else {
      return false;
    }
  }
}
