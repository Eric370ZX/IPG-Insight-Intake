namespace Intake.Contact {
export function controlFormRibbon() {
  
  let formLabel;
  let currForm = Xrm.Page.ui.formSelector.getCurrentItem();
  formLabel = currForm.getLabel();
  if (formLabel != "Patient") {
    return true;
  }
  else {
    return false;
  }

}


export function controlViewRibbon() {

  let viewId;
  viewId = window.parent.location.href;
  let currView = "7bdf8c63-d694-e811-a961-000d3a3702ca";
  if (!viewId.includes(currView)) {
    return true;
  }
  else {
    return false;
  }

}


}
