/**
 * @namespace Intake.Referral
 */

namespace Intake.Referral {
  const tabKey = 'Referral';
  const sectionKey = 'CPTCodes';
  const listOfIPGBlockedCPTCodesKey = 'ipg_listofipgblockedcptcodes';
  const listOfIPGUnsuppoertedCPTCodesKey = 'ipg_listofipgunsupportedcptcodes';
  const listOfFacilityBlockedCPTCodesKey = 'ipg_listoffacilityblockedcptcodes';
  const listOfCarrierBlockedCPTCodesKey = 'ipg_listofcarrierblockedcptcodes';
  /**
   * Messages for the reasons for the blocking.
   * @interface Resons
   */
  interface Reasons {
    blockedPerIPG: string;
    unsupportedByIPG: string;
    blockedPerFacility: string;
    blockedPerCarrier: string;
  }
  function waitUntilQuickViewFormLoaded(quickViewForm: Xrm.Controls.QuickFormControl, callback: () => void): void {
    setTimeout(() => {
      if (quickViewForm.isLoaded()) {
        callback();
      }
      else {
        return waitUntilQuickViewFormLoaded(quickViewForm, callback);
      }
    }, 100);
  }
  /**
   * Called when the form is loaded. See D365 configuration for details.
   * @function Intake.Referral.ShowCPTCodeReasonForBlocking
   * @returns {void}
   */
  export function ShowCPTCodeReasonForBlocking(jsonString: string) {
    const reasons: Reasons = JSON.parse(jsonString);
    const tab = Xrm.Page.ui.tabs.get(tabKey);
    const section = tab && tab.sections.get(sectionKey);
    const contorls = section && section.controls.get() as Array<Xrm.Controls.StringControl>;
    if (contorls && contorls.length) {
      const listOfIPGBlockedCPTCodesAttribute: Xrm.Attributes.StringAttribute = Xrm.Page.getAttribute(listOfIPGBlockedCPTCodesKey);
      const listOfIPGUnsupportedCPTCodesAttribute: Xrm.Attributes.StringAttribute = Xrm.Page.getAttribute(listOfIPGUnsuppoertedCPTCodesKey);
      const listOfFacilityBlockedCPTCodesAttribute: Xrm.Attributes.StringAttribute = Xrm.Page.getAttribute(listOfFacilityBlockedCPTCodesKey);
      const listOfCarrierBlockedCPTCodesAttribute: Xrm.Attributes.StringAttribute = Xrm.Page.getAttribute(listOfCarrierBlockedCPTCodesKey);
      // Create a CPT code map for each cause of the block.
      const listOfIPGBlockedCPTCodesValue = listOfIPGBlockedCPTCodesAttribute.getValue();
      const listOfIPGUnsupportedCPTCodesValue = listOfIPGUnsupportedCPTCodesAttribute.getValue();
      const listOfFacilityBlockedCPTCodesValue = listOfFacilityBlockedCPTCodesAttribute.getValue();
      const listOfCarrierBlockedCPTCodesValue = listOfCarrierBlockedCPTCodesAttribute.getValue();
      const listOfIPGBlockedCPTCodes = (listOfIPGBlockedCPTCodesValue || '').split(', ');
      const listOfIPGUnsupportedCPTCodes = (listOfIPGUnsupportedCPTCodesValue || '').split(', ');
      const listOfFacilityBlockedCPTCodes = (listOfFacilityBlockedCPTCodesValue || '').split(', ');
      const listOfCarrierBlockedCPTCodes = (listOfCarrierBlockedCPTCodesValue || '').split(', ');
      for (let index = 0, length = contorls.length; index < length; index++) {
        const control: Xrm.Controls.StringControl = contorls[index];
        const attribute: Xrm.Attributes.StringAttribute = control.getAttribute();
        const cptCode = attribute.getValue();
        // Set the message based on the occurrence of the CPT code in the list of blocked CPT codes.
        let message: string;
        if (listOfIPGBlockedCPTCodes.indexOf(cptCode) >= 0) message = reasons.blockedPerIPG;
        if (listOfIPGUnsupportedCPTCodes.indexOf(cptCode) >= 0) message = reasons.unsupportedByIPG;
        if (listOfFacilityBlockedCPTCodes.indexOf(cptCode) >= 0) message = reasons.blockedPerFacility;
        if (listOfCarrierBlockedCPTCodes.indexOf(cptCode) >= 0) message = reasons.blockedPerCarrier;
        if (message) {
          const quickViewForm = Xrm.Page.ui.quickForms.get(`CPTCode${index + 1}`);
          quickViewForm.refresh();
          waitUntilQuickViewFormLoaded(quickViewForm, () => {
            const cptCodeNameControl = quickViewForm.ui.controls.get('ipg_cptname') as Xrm.Controls.StringControl;
            const cptCodeNameAttribute = cptCodeNameControl.getAttribute();
            const cptCodeNameValue = cptCodeNameAttribute.getValue();
            cptCodeNameAttribute.setValue(`${cptCodeNameValue}, Block reason: ${message}`);
          });
        }
      }
    }
  }
}
