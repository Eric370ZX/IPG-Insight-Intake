/**
 * @namespace Intake.Case
 */
namespace Intake.Case {
  const lastEbvCheckDateKey = 'ipg_lastebvcheckdatetime';
  // Get diff between two dates in days format.
  function getDateDiffInDays(date1: Date, date2: Date): number {
    const diff = Math.abs(date2.getTime() - date1.getTime());
    return Math.ceil(diff / (1000 * 3600 * 24));
  }
  /**
   * Called when the form is loaded. See D365 configuration for details.
   * @function Intake.Case.HideOrShowRetrieveBenefitsButton
   * @returns {boolean}
   */
  export function HideOrShowRetrieveBenefitsButton() {
    const lastEbvCheckDateAttribute: Xrm.Attributes.DateAttribute = Xrm.Page.getAttribute(lastEbvCheckDateKey);
    const lastEvcCheckDateValue: Date = lastEbvCheckDateAttribute.getValue();
    const todayDate = new Date();
    todayDate.setHours(0, 0, 0, 0);
    return !lastEvcCheckDateValue || (getDateDiffInDays(lastEvcCheckDateValue, todayDate) >= 3);
  }
}
