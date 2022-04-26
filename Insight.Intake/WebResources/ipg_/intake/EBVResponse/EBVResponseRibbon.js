/**
 * @namespace Intake.EBVInquiry
 */
var Intake;
(function (Intake) {
    var EBVResponse;
    (function (EBVResponse) {
        /**
         * Opens EBVResponse\ViewDetails.html.
         * @function Intake.EBVResponse.ViewDetails
         * @returns {void}
         */
        function ViewDetails(firstSelectedItemId) {
            if (!firstSelectedItemId) {
                alert('EBV Response ID is required to view the response details');
                return;
            }
            Xrm.Navigation.openWebResource('ipg_/intake/EBVResponse/ViewDetails.html', { width: 1000, height: 800, openInNewWindow: true }, firstSelectedItemId);
        }
        EBVResponse.ViewDetails = ViewDetails;
    })(EBVResponse = Intake.EBVResponse || (Intake.EBVResponse = {}));
})(Intake || (Intake = {}));
