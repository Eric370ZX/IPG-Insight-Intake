/**
 * @namespace Intake.Utility
 */
namespace Intake.Utility {
  const dummyId = '00000000-0000-0000-0000-000000000000';
  /**
   * Called when the form is loaded. See D365 configuration for details.
   * @function Intake.Utility.ShowQuickViewFormIfRecordNotFound
   * @returns {void}
   */
  export function ClearDummyLookupValue(lookupFieldKey: string) {
    const lookupFieldAttribute: Xrm.Attributes.LookupAttribute = Xrm.Page.getAttribute(lookupFieldKey);
    let lookupFieldValue = lookupFieldAttribute.getValue();
    if (lookupFieldValue && lookupFieldValue.length) {
      lookupFieldValue = lookupFieldValue.slice();
      for (let index = 0, length = lookupFieldValue.length; index < length; index++) {
        const value = lookupFieldValue[index];
        if (value.id === dummyId || value.id === `{${dummyId}}`) {
          lookupFieldValue.splice(index, 1);
        }
      }
      lookupFieldAttribute.setValue(lookupFieldValue);
    }
  }
  /**
   * @namespace Intake.Utility.ClearDummyLookupValue
   */
  export namespace ClearDummyLookupValue {
    /**
     * Called when the form is loaded. See D365 configuration for details.
     * @function Intake.Utility.ClearDummyLookupValue.FromJSON
     * @returns {void}
     */
    export function FromJSON(jsonString: string): void {
      const payload: [string] = JSON.parse(jsonString);
      payload.forEach((lookupFieldKey: string) => ClearDummyLookupValue(lookupFieldKey));
    }
  }
}
