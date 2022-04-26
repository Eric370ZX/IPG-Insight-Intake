/**
 * @namespace Intake.Account
 */
namespace Intake.Account {
  /**
   * Set ipg_active field value depending on dates.
   * @function Intake.Account.SetStatus
   * @returns {void}
   */
  export function SetStatus() {
    var ipg_effectivedate = Xrm.Page.getAttribute("ipg_effectivedate").getValue();
    var ipg_expirationdate = Xrm.Page.getAttribute("ipg_expirationdate").getValue();
    var today = Date.now();
    if ((ipg_effectivedate > today) && (today > ipg_expirationdate)) {
      Xrm.Page.getAttribute("ipg_active").setValue(false);
    }
    else {
      Xrm.Page.getAttribute("ipg_active").setValue(true);
    }
  }
}
