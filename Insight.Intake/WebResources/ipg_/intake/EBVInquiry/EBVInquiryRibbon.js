/**
 * @namespace Intake.EBVInquiry
 */
var Intake;
(function (Intake) {
    var EBVInquiry;
    (function (EBVInquiry) {
        /**
         * Opens EBVResponse\ViewDetails.html.
         * @function Intake.EBVInquiry.ViewResponseDetails
         * @returns {void}
         */
        function ViewResponseDetails(firstSelectedItemId) {
            if (!firstSelectedItemId) {
                alert('EBV Inquiry ID is required to view the response details');
                return;
            }
            Xrm.WebApi.retrieveRecord("ipg_ebvinquiry", firstSelectedItemId, "?$select=ipg_ebvinquiryid,_ipg_responseid_value").then(function success(inquiry) {
                if (!inquiry._ipg_responseid_value) {
                    alert('No response for the selected EBV Inquiry');
                    return;
                }
                Xrm.Navigation.openWebResource('ipg_/intake/EBVResponse/ViewDetails.html', { width: 1000, height: 800, openInNewWindow: true }, inquiry._ipg_responseid_value);
            }, function (error) {
                alert('Could not retrieve EBV Inquiry: ' + error.message);
            });
        }
        EBVInquiry.ViewResponseDetails = ViewResponseDetails;
    })(EBVInquiry = Intake.EBVInquiry || (Intake.EBVInquiry = {}));
})(Intake || (Intake = {}));
