/**
 * @namespace Intake.Document
 */
namespace Intake.Document {
  const publishDateKey = 'createdon';
  /**
   * Called when the form is loaded. See D365 configuration for details.
   * @function Intake.Document.SetDefaultPublishDate
   * @returns {void}
   */
  export function SetDefaultPublishDate() {
    const publishDateAttribute: Xrm.Attributes.DateAttribute = Xrm.Page.getAttribute(publishDateKey);
    const publishDateValue = publishDateAttribute.getValue();
    if (!publishDateValue) {
      const todayDate = new Date();
      publishDateAttribute.setValue(todayDate);
    }
  }
}
