
namespace Intake.Account {

  export function controlFormRibbon() {

    let formLabel;
    let currForm = Xrm.Page.ui.formSelector.getCurrentItem();
    formLabel = currForm.getLabel();
    if (formLabel != "Manufacturer") {
      return true;
    }
    else {
      return false;
    }

  }
}
