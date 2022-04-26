/**
 * @namespace Intake.Document
 */
var Intake;
(function (Intake) {
    var Document;
    (function (Document) {
        /**
         * Called from the form of Document
         * @function Intake.Document.CallCloneDocumentActionForm
         * @returns {void}
         */
        function CallCloneDocumentActionForm() {
            var entityId = Intake.Utility.removeCurlyBraces(Xrm.Page.data.entity.getId());
            CallCloneDocumentAction(entityId);
        }
        Document.CallCloneDocumentActionForm = CallCloneDocumentActionForm;
        /**
         * Called from the view of Document
         * @function Intake.Document.CallCloneDocumentActionView
         * @returns {void}
         */
        function CallCloneDocumentActionView(selectedItemId) {
            var entityId = Intake.Utility.removeCurlyBraces(selectedItemId);
            CallCloneDocumentAction(entityId);
        }
        Document.CallCloneDocumentActionView = CallCloneDocumentActionView;
        function CallCloneDocumentAction(entityId) {
            if (entityId) {
                var request = {
                    "Record": {
                        "@odata.type": "Microsoft.Dynamics.CRM.ipg_document",
                        "ipg_documentid": entityId
                    },
                    getMetadata: function () {
                        return {
                            boundParamter: null,
                            parameterTypes: {
                                "Record": {
                                    "typeName": "mscrm.ipg_document",
                                    "structuralProperty": 5
                                }
                            },
                            operationType: 0,
                            operationName: "ipg_IPGIntakeActionsCloneRecord"
                        };
                    }
                };
                Xrm.Utility.showProgressIndicator(null);
                Xrm.WebApi.online.execute(request)
                    .then(function (response) {
                    Xrm.Utility.closeProgressIndicator();
                    if (response.ok) {
                        return response.json(); // the method 'json' has not been declared in types, but it works such way
                    }
                    else {
                        Xrm.Navigation.openErrorDialog({ message: response.statusText });
                    }
                }, function (error) {
                    Xrm.Navigation.openErrorDialog({ message: error.message });
                })
                    .then(function (result) {
                    if (result.ipg_documentid) {
                        Xrm.Utility.openEntityForm("ipg_document", result.ipg_documentid);
                    }
                });
            }
        }
    })(Document = Intake.Document || (Intake.Document = {}));
})(Intake || (Intake = {}));
