/**
 * @namespace Intake.Utility
 */
namespace Intake.Utility {
  /**
   * Called when the form is loaded. See D365 configuration for details.
   * @function Intake.Utility.FormatPhoneNumber
   * @returns {void}
   */
  export function FormatPhoneNumber(fieldKey: string): void {
    const attribute: Xrm.Attributes.StringAttribute = Xrm.Page.getAttribute(fieldKey);
    // Listener.
    function changeListener(): void {
      const phoneNumber = attribute.getValue();
      const fixedNumber = (phoneNumber || '').replace(/[^0-9]/g, '');
      const substring = (start: number, stop: number): string => fixedNumber.substring(start,  stop);
      let output;
      switch (fixedNumber.length) {
      // Expected: 123-4567
      case 7:
        output = `${substring(0, 3)}-${substring(3, 7)}`;
        break;
      // Expected: (123) 456-7890
      case 10:
        output = `(${substring(0, 3)}) ${substring(3, 6)}-${substring(6, 10)}`;
        break;
      // Expected: (234) 567 8901
      case 11:
        output = (substring(0, 1) === '1') ? `(${substring(1, 4)}) ${substring(4, 7)} ${substring(7, 11)}` : fixedNumber;
        break;
      default:
        // Error No Output
        output = phoneNumber;
        break;
      }
      attribute.setValue(output);
    }
    attribute.addOnChange(changeListener);
  }
}
