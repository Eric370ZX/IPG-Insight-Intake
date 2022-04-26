/**
 * @namespace Intake.Utility
 */
namespace Intake.Utility {
  /**
   * Called when the form is loaded. See D365 configuration for details.
   * @function Intake.Utility.Lookup
   * @returns {void}
   */
  export function Lookup(fieldKey: string, lookupFieldKey: string, entityType: string): void {
    const attribute: Xrm.Attributes.StringAttribute = Xrm.Page.getAttribute(fieldKey);
    const lookupAttribute: Xrm.Attributes.LookupAttribute = Xrm.Page.getAttribute(lookupFieldKey);
    const attributeName = attribute.getName();
    // Listener.
    function onChange(): void {
      // WORKAROUND: Line below should be changed when Microsoft added API for receive selected item from the control.
      const customControl = window.parent.Ipg.CustomControls.CCManager.getControl(attribute.getName());
      if (customControl.selectedValue) {
        lookupAttribute.setValue([
          {
            id: customControl.selectedValue,
            name: `Automatically Selected Value (From: ${attributeName})`,
            entityType: entityType
          }
        ]);
      }
      else {
        lookupAttribute.setValue(null);
      }
      lookupAttribute.fireOnChange();
    }
    attribute.addOnChange(onChange);
  }
  /**
   * @namespace Intake.Utility.Lookup
   */
  export namespace Lookup {
    /**
     * Called when the form is loaded. See D365 configuration for details.
     * @function Intake.Utility.Lookup
     * @returns {void}
     */
    export function FromJSON(jsonString: string): void {
      const payload: Array<[string, string, string]> = JSON.parse(jsonString);
      payload.forEach((tuple: [string, string, string]): void => Lookup.apply(this, tuple));
    }
  }
}
