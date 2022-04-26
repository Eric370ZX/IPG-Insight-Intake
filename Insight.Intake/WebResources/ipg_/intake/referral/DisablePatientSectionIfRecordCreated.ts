/**
 * @namespace Intake.Referral
 */
namespace Intake.Referral {
  type ControlType = Xrm.Controls.DateControl | Xrm.Controls.LookupControl | Xrm.Controls.StringControl;
  /**
   * Called when the form is loaded. See D365 configuration for details.
   * @function Intake.Referral.DisablePatientSectionIfRecordCreated
   * @returns {void}
   */
  export function DisablePatientSectionIfRecordCreated(): void {
    const formType = Xrm.Page.ui.getFormType();
    const tab = Xrm.Page.ui.tabs.get('Referral');
    const section = tab && tab.sections.get('Patient');
    const controls = section && section.controls.get();
    if (section && controls && controls.length) {
      const isRecordCreated = XrmEnum.FormType.Update === formType || XrmEnum.FormType.Disabled === formType || XrmEnum.FormType.ReadOnly === formType;
      controls.forEach((control: ControlType): void => {
        if (control.getVisible()) {
          control.setDisabled(isRecordCreated);
        }
      });
    }
  }
}
