/**
 * @namespace Intake.Document
 */
namespace Intake.Document {
  /**
   * Called from the form of Document
   * @function Intake.Document.CallCloneDocumentActionForm
   * @returns {void}
   */
  export function CallCloneDocumentActionForm() {
    const entityId = Intake.Utility.removeCurlyBraces(Xrm.Page.data.entity.getId());
    CallCloneDocumentAction(entityId);
  }

  /**
   * Called from the view of Document
   * @function Intake.Document.CallCloneDocumentActionView
   * @returns {void}
   */
  export function CallCloneDocumentActionView(selectedItemId: string) {
    const entityId = Intake.Utility.removeCurlyBraces(selectedItemId);
    CallCloneDocumentAction(entityId);
  }

  function CallCloneDocumentAction(entityId: string) {
    if (entityId) {
      let request = {
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
        .then((response) => {
          Xrm.Utility.closeProgressIndicator();
          if (response.ok) {
            return response.json(); // the method 'json' has not been declared in types, but it works such way
          }
          else {
            Xrm.Navigation.openErrorDialog({ message: response.statusText });
          }
        },
        (error) => {
          Xrm.Navigation.openErrorDialog({ message: error.message });
        })
        .then((result) => {
          if (result.ipg_documentid) {
            Xrm.Utility.openEntityForm("ipg_document", result.ipg_documentid);
          }
        });
    }
  }
}
