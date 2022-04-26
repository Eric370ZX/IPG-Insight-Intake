/**
 * @namespace Intake.Document
 */
var Intake;
(function (Intake) {
    var Document;
    (function (Document) {
        if (typeof ($) === 'undefined') {
            $ = window.parent.$;
        }
        /**
         * Sets Inline Content Disposition for Note (Annotation entity) attachments. They will open in a new window from Insight Documents web service.
         * @function Intake.Document.SetInlineContentDisposition
         * @returns {void}
         */
        function SetInlineContentDisposition() {
            var attachmentItemId = 'notescontrol-timeline_record_attachment_Item';
            $((window.parent || window).document).find('#editFormRoot').on('click', "[id*=" + attachmentItemId + "]", function (ev) {
                var env = Intake.Utility.GetEnvironment();
                var envSuffix;
                if (env) {
                    envSuffix = '-' + env;
                }
                else {
                    envSuffix = '';
                }
                var docId = $(ev.currentTarget).data('id').replace(attachmentItemId, '');
                if (docId) {
                    ev.stopPropagation();
                    ev.preventDefault();
                    window.open("https://insight-documents" + envSuffix + ".azurewebsites.net/documents/" + docId, '_blank');
                }
            });
        }
        Document.SetInlineContentDisposition = SetInlineContentDisposition;
    })(Document = Intake.Document || (Intake.Document = {}));
})(Intake || (Intake = {}));
