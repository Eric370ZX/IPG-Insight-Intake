/**
 * @namespace Intake.Referral
 */
namespace Intake.Referral {
  //Timeline section is not needed anymore
  /**
   * Called when the form is loaded. See D365 configuration for details.
   * @function Intake.Referral.ShowTimelineSectionIfRecordCreated
   * @returns {void}
   */
  //export function ShowTimelineSectionIfRecordCreated(): void {
  //  const formType = Xrm.Page.ui.getFormType();
  //  const tab = Xrm.Page.ui.tabs.get('Referral');
  //  const section = tab && tab.sections.get('Timeline');
  //  if (section) {
  //    const isRecordCreated = XrmEnum.FormType.Update === formType || XrmEnum.FormType.Disabled === formType || XrmEnum.FormType.ReadOnly === formType;
  //    section.setVisible(isRecordCreated);
  //  }
  //}
}
