/**
 * @namespace Intake.Document
 */
var Intake;
(function (Intake) {
    var Document;
    (function (Document) {
        var publishDateKey = 'createdon';
        /**
         * Called when the form is loaded. See D365 configuration for details.
         * @function Intake.Document.SetDefaultPublishDate
         * @returns {void}
         */
        function SetDefaultPublishDate() {
            var publishDateAttribute = Xrm.Page.getAttribute(publishDateKey);
            var publishDateValue = publishDateAttribute.getValue();
            if (!publishDateValue) {
                var todayDate = new Date();
                publishDateAttribute.setValue(todayDate);
            }
        }
        Document.SetDefaultPublishDate = SetDefaultPublishDate;
    })(Document = Intake.Document || (Intake.Document = {}));
})(Intake || (Intake = {}));
