/**
 * @namespace Intake.Referral
 */
namespace Intake.Referral {
  /**
   * Called when the form is loaded. See D365 configuration for details.
   * @function Intake.Referral.ShowStateSectionIfRecordCreated
   * @returns {void}
   */
  export function ShowStateSectionIfRecordCreated(): void {
    const formType = Xrm.Page.ui.getFormType();
    const tab = Xrm.Page.ui.tabs.get('Referral');
    const section = tab && tab.sections.get('Status');
    if (section) {
      const isRecordCreated = XrmEnum.FormType.Update === formType || XrmEnum.FormType.Disabled === formType || XrmEnum.FormType.ReadOnly === formType;
      section.setVisible(isRecordCreated);
    }
  }
}