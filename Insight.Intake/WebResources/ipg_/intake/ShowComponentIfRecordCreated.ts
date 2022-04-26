/**
 * @namespace Intake.Utility
 */
namespace Intake.Utility {
  export enum ComponentType {
    Tab = 'Tab',
    Section = 'Section',
    Control = 'Control',
  }
  export function ShowComponentIfRecordCreated(componentType: ComponentType.Tab, ...keys: [string]): void;
  export function ShowComponentIfRecordCreated(componentType: ComponentType.Section, ...keys: [string, string]): void;
  export function ShowComponentIfRecordCreated(componentType: ComponentType.Control, ...keys: [string]): void;
  /**
   * Called when the form is loaded. See D365 configuration for details.
   * @function Intake.Utility.ShowComponentIfRecordCreated
   * @returns {void}
   */
  export function ShowComponentIfRecordCreated(componentType: ComponentType, ...keys: string[]): void {
    const formType = Xrm.Page.ui.getFormType();
    const isRecordCreated = XrmEnum.FormType.Update === formType || XrmEnum.FormType.Disabled === formType || XrmEnum.FormType.ReadOnly === formType;
    let component: any;
    switch (componentType) {
    case ComponentType.Tab:
      {
        const [ tabKey ] = keys;
        component = Xrm.Page.ui.tabs.get(tabKey);
      }
      break;
    case ComponentType.Section:
      {
        const [ tabKey, sectionKey ] = keys;
        const tab = Xrm.Page.ui.tabs.get(tabKey);
        component = tab && tab.sections.get(sectionKey);
      }
      break;
    case ComponentType.Control:
      {
        const [ controlKey ] = keys;
        component = Xrm.Page.getControl(controlKey);
      }
      break;
    default:
      break;
    }
    if (component) {
      component.setVisible(isRecordCreated);
    }
  }
}
