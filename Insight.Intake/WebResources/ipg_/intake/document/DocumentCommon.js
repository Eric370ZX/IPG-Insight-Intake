/**
 * @namespace Intake.Document
 */
var Intake;
(function (Intake) {
    var Document;
    (function (Document) {
        var xrm = window.Xrm || parent.Xrm;
        /**
         * Open Document Preview HTML web resource.
         * @function Intake.Document.OpenDocumentPreview
         * @returns {void}
         */
        function previewById(ipgDocumentId) {
            if (ipgDocumentId) {
                var env = Intake.Utility.GetEnvironment();
                var docAppEnvSuffix_1;
                if (env) {
                    docAppEnvSuffix_1 = '-' + env;
                }
                else {
                    docAppEnvSuffix_1 = '';
                }
                //todo: use single URI in the web app to get rid of the annotation request below
                xrm.WebApi.retrieveMultipleRecords("annotation", "?$filter=_objectid_value eq '" + ipgDocumentId + "'")
                    .then(function (result) {
                    var height = 600;
                    var width = 800;
                    if (result && result.entities.length) {
                        var annotationId = result.entities[0].annotationid;
                        xrm.Navigation.openUrl("https://insight-documents" + docAppEnvSuffix_1 + ".azurewebsites.net/documents/" + annotationId, { height: height, width: width });
                    }
                    else {
                        xrm.Navigation.openUrl("https://insight-documents" + docAppEnvSuffix_1 + ".azurewebsites.net/legacydocuments/" + ipgDocumentId, { height: height, width: width });
                    }
                }, function (error) {
                    xrm.Navigation.openErrorDialog({ message: error.message });
                });
            }
        }
        Document.previewById = previewById;
    })(Document = Intake.Document || (Intake.Document = {}));
})(Intake || (Intake = {}));
