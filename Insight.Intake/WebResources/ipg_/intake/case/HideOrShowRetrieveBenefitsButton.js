/**
 * @namespace Intake.Case
 */
var Intake;
(function (Intake) {
    var Case;
    (function (Case) {
        var lastEbvCheckDateKey = 'ipg_lastebvcheckdatetime';
        // Get diff between two dates in days format.
        function getDateDiffInDays(date1, date2) {
            var diff = Math.abs(date2.getTime() - date1.getTime());
            return Math.ceil(diff / (1000 * 3600 * 24));
        }
        /**
         * Called when the form is loaded. See D365 configuration for details.
         * @function Intake.Case.HideOrShowRetrieveBenefitsButton
         * @returns {boolean}
         */
        function HideOrShowRetrieveBenefitsButton() {
            var lastEbvCheckDateAttribute = Xrm.Page.getAttribute(lastEbvCheckDateKey);
            var lastEvcCheckDateValue = lastEbvCheckDateAttribute.getValue();
            var todayDate = new Date();
            todayDate.setHours(0, 0, 0, 0);
            return !lastEvcCheckDateValue || (getDateDiffInDays(lastEvcCheckDateValue, todayDate) >= 3);
        }
        Case.HideOrShowRetrieveBenefitsButton = HideOrShowRetrieveBenefitsButton;
    })(Case = Intake.Case || (Intake.Case = {}));
})(Intake || (Intake = {}));
