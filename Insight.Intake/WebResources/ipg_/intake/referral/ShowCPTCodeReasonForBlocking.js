/**
 * @namespace Intake.Referral
 */
var Intake;
(function (Intake) {
    var Referral;
    (function (Referral) {
        var tabKey = 'Referral';
        var sectionKey = 'CPTCodes';
        var listOfIPGBlockedCPTCodesKey = 'ipg_listofipgblockedcptcodes';
        var listOfIPGUnsuppoertedCPTCodesKey = 'ipg_listofipgunsupportedcptcodes';
        var listOfFacilityBlockedCPTCodesKey = 'ipg_listoffacilityblockedcptcodes';
        var listOfCarrierBlockedCPTCodesKey = 'ipg_listofcarrierblockedcptcodes';
        function waitUntilQuickViewFormLoaded(quickViewForm, callback) {
            setTimeout(function () {
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
        function ShowCPTCodeReasonForBlocking(jsonString) {
            var reasons = JSON.parse(jsonString);
            var tab = Xrm.Page.ui.tabs.get(tabKey);
            var section = tab && tab.sections.get(sectionKey);
            var contorls = section && section.controls.get();
            if (contorls && contorls.length) {
                var listOfIPGBlockedCPTCodesAttribute = Xrm.Page.getAttribute(listOfIPGBlockedCPTCodesKey);
                var listOfIPGUnsupportedCPTCodesAttribute = Xrm.Page.getAttribute(listOfIPGUnsuppoertedCPTCodesKey);
                var listOfFacilityBlockedCPTCodesAttribute = Xrm.Page.getAttribute(listOfFacilityBlockedCPTCodesKey);
                var listOfCarrierBlockedCPTCodesAttribute = Xrm.Page.getAttribute(listOfCarrierBlockedCPTCodesKey);
                // Create a CPT code map for each cause of the block.
                var listOfIPGBlockedCPTCodesValue = listOfIPGBlockedCPTCodesAttribute.getValue();
                var listOfIPGUnsupportedCPTCodesValue = listOfIPGUnsupportedCPTCodesAttribute.getValue();
                var listOfFacilityBlockedCPTCodesValue = listOfFacilityBlockedCPTCodesAttribute.getValue();
                var listOfCarrierBlockedCPTCodesValue = listOfCarrierBlockedCPTCodesAttribute.getValue();
                var listOfIPGBlockedCPTCodes = (listOfIPGBlockedCPTCodesValue || '').split(', ');
                var listOfIPGUnsupportedCPTCodes = (listOfIPGUnsupportedCPTCodesValue || '').split(', ');
                var listOfFacilityBlockedCPTCodes = (listOfFacilityBlockedCPTCodesValue || '').split(', ');
                var listOfCarrierBlockedCPTCodes = (listOfCarrierBlockedCPTCodesValue || '').split(', ');
                var _loop_1 = function (index, length_1) {
                    var control = contorls[index];
                    var attribute = control.getAttribute();
                    var cptCode = attribute.getValue();
                    // Set the message based on the occurrence of the CPT code in the list of blocked CPT codes.
                    var message;
                    if (listOfIPGBlockedCPTCodes.indexOf(cptCode) >= 0)
                        message = reasons.blockedPerIPG;
                    if (listOfIPGUnsupportedCPTCodes.indexOf(cptCode) >= 0)
                        message = reasons.unsupportedByIPG;
                    if (listOfFacilityBlockedCPTCodes.indexOf(cptCode) >= 0)
                        message = reasons.blockedPerFacility;
                    if (listOfCarrierBlockedCPTCodes.indexOf(cptCode) >= 0)
                        message = reasons.blockedPerCarrier;
                    if (message) {
                        var quickViewForm_1 = Xrm.Page.ui.quickForms.get("CPTCode" + (index + 1));
                        quickViewForm_1.refresh();
                        waitUntilQuickViewFormLoaded(quickViewForm_1, function () {
                            var cptCodeNameControl = quickViewForm_1.ui.controls.get('ipg_cptname');
                            var cptCodeNameAttribute = cptCodeNameControl.getAttribute();
                            var cptCodeNameValue = cptCodeNameAttribute.getValue();
                            cptCodeNameAttribute.setValue(cptCodeNameValue + ", Block reason: " + message);
                        });
                    }
                };
                for (var index = 0, length_1 = contorls.length; index < length_1; index++) {
                    _loop_1(index, length_1);
                }
            }
        }
        Referral.ShowCPTCodeReasonForBlocking = ShowCPTCodeReasonForBlocking;
    })(Referral = Intake.Referral || (Intake.Referral = {}));
})(Intake || (Intake = {}));
