/**
 * @namespace Intake.Utility
 */
namespace Intake.Utility {
  const dummyId = '00000000-0000-0000-0000-000000000000';
  const controls: Array<[Xrm.Controls.StringControl, Xrm.Controls.LookupControl]> = [];
  /**
   * Called when the form is loaded. See D365 configuration for details.
   * @function Intake.Utility.ShowQuickViewFormIfRecordNotFound
   * @returns {void}
   */
  export function ShowQuickViewFormIfRecordNotFound(fieldKey: string, lookupFieldKey: string, quickViewFormKey: string, entityType: string) {
    const masterFieldAttribute: Xrm.Attributes.StringAttribute = Xrm.Page.getAttribute(fieldKey);
    const lookupFieldAttribute: Xrm.Attributes.LookupAttribute = Xrm.Page.getAttribute(lookupFieldKey);
    function addDummyLookupValue() {
      const lookupValue = lookupFieldAttribute.getValue();
      if (!lookupValue || lookupValue.length === 0) {
        lookupFieldAttribute.setValue([
          {
            id: dummyId,
            name: 'Dummy Data',
            entityType: entityType
          }
        ]);
      }
    }
    function wait(): Promise<boolean> {
      return new Promise<boolean>((resolve: () => void): number => setTimeout(resolve, 0));
    }
    // Master fields change listener
    function changeListener(): void {
      addDummyLookupValue();
      // Wait until the business rule is executed.
      wait().then((): void => {
        controls.forEach(([ masterControl, lookupControl ]: [ Xrm.Controls.StringControl, Xrm.Controls.LookupControl ]) => {
          if (!masterControl.getVisible()) {
            lookupControl.getAttribute().setValue(null);
          }
        });
      });
    }
    masterFieldAttribute.addOnChange(changeListener);
    // We should set the dummy data, if the master field contains data and
    // the related lookup field does not contains data.
    if (masterFieldAttribute.getValue()) {
      addDummyLookupValue();
    }
  }
  /**
   * @namespace Intake.Utility.ShowQuickViewFormIfRecordNotFound
   */
  export namespace ShowQuickViewFormIfRecordNotFound {
    /**
     * Called when the form is loaded. See D365 configuration for details.
     * @function Intake.Utility.ShowQuickViewFormIfRecordNotFound
     * @returns {void}
     */
    export function FromJSON(jsonString: string): void {
      const payload: Array<[string, string, string]> = JSON.parse(jsonString);
      payload.forEach((tuple: [string, string, string]): void => {
        const [ fieldKey, lookupFieldKey ] = tuple;
        const control: Xrm.Controls.StringControl = Xrm.Page.getControl(fieldKey);
        const lookupControl: Xrm.Controls.LookupControl = Xrm.Page.getControl(lookupFieldKey);
        controls.push([ control, lookupControl ]);
        ShowQuickViewFormIfRecordNotFound.apply(this, tuple);
      });
    }
  }
}
