/**
 * @namespace Intake.Case
 */

namespace Intake.Case {
  export function executeCreateClaimAction() {
    //const caseId = Intake.Utility.removeCurlyBraces(Xrm.Page.data.entity.getId());
    const caseId = Xrm.Page.data.entity.getId();
    const primaryCarrier = Xrm.Page.getAttribute("ipg_carrierid").getValue();
    if (primaryCarrier == null) {
      console.log("Primary Carrier not Selected");
      Xrm.Navigation.openAlertDialog({ text: "Primary Carrier not Selected" });
      return;
    }
    const primaryCarrierId = primaryCarrier[0].id;
    const useSecondaryCarrier = Xrm.Page.getAttribute("ipg_secondarycarrier").getValue();

    const secondaryCarrier = Xrm.Page.getAttribute("ipg_secondarycarrierid").getValue();
    if (useSecondaryCarrier === true && secondaryCarrier == null) {
      console.log("Secondary Carrier not Selected");
      Xrm.Navigation.openAlertDialog({ text: "Secondary Carrier Name must be selected if SecondaryCarrier = 'Yes'." });
      return;
    }
    const secondaryCarrierId = (useSecondaryCarrier === true) ? secondaryCarrier[0].id : "{00000000-0000-0000-0000-000000000000}";
    let entityType;
    let claimGenerationParametersId;

    let entityFormOptions = {};
    entityFormOptions["entityName"] = "ipg_claimgenerationparameters";
    entityFormOptions["useQuickCreateForm"] = true;

    // Set default values for the Contact form
    let formParameters = {};
    formParameters["ipg_primarycarrierid"] = primaryCarrierId;
    formParameters["ipg_secondarycarrierid"] = secondaryCarrierId;
    formParameters["ipg_usesecondarycarrier"] = useSecondaryCarrier;
    formParameters["ipg_caseid"] = caseId;

    // Open the form.
    Xrm.Navigation.openForm(entityFormOptions, formParameters)
    .then((result) => {
      if (result != null && result.savedEntityReference.length == 1) {
        entityType = result.savedEntityReference[0].entityType;
        claimGenerationParametersId = result.savedEntityReference[0].id;

        return Xrm.WebApi.retrieveRecord(entityType, claimGenerationParametersId);
      }
    })
    .then((response) => {
      if (response) {
        const target = { entityType: "incident", id: caseId };
        const reqObject = {
          entity: target,
          IsPrimaryOrSecondaryClaim: response.ipg_primaryorsecondaryclaim,
          GenerateClaimFlag: response.ipg_generatepdfonly,
          GeneratePdfFlag: true,
          //ClaimType: response.ipg_claimtype,
          Icn: response.ipg_icn != null ? response.ipg_icn : "",
          Box32: response.ipg_box32.toString(),
          Reason: response.ipg_reason != null ? response.ipg_reason : "",
          IsReplacementClaim: response.ipg_neworreplacementclaim,
          ManualClaim: true,
          getMetadata: function () {
            return {
              boundParameter: "entity",
              operationType: 0,
              operationName: "ipg_IPGCaseActionsCreateClaim",
              parameterTypes: {
                "entity": { 
                  typeName: "mscrm.incident",
                  structuralProperty: 5
                }, "IsPrimaryOrSecondaryClaim": {
                  typeName: "Edm.Boolean",
                  structuralProperty: 1
                }, "GenerateClaimFlag": {
                  typeName: "Edm.Boolean",
                  structuralProperty: 1
                }, "GeneratePdfFlag": {
                  typeName: "Edm.Boolean",
                  structuralProperty: 1
                //}, "ClaimType": {
                //  typeName: "Edm.Int32",
                //  structuralProperty: 1
                }, "Icn": {
                  typeName: "Edm.String",
                  structuralProperty: 1
                }, "Box32": {
                  typeName: "Edm.String",
                  structuralProperty: 1
                }, "Reason": {
                  typeName: "Edm.String",
                  structuralProperty: 1
                }, "IsReplacementClaim": {
                  typeName: "Edm.Boolean",
                  structuralProperty: 1
                }, "ManualClaim": {
                  typeName: "Edm.Boolean",
                  structuralProperty: 1
                }
              }
            }
          }
        }

        Xrm.Utility.showProgressIndicator("Generating Claim...");
        Xrm.WebApi.online.execute(reqObject)
        .then((response) => {
          Xrm.Utility.closeProgressIndicator();
          if (response.ok) {
            Xrm.WebApi.deleteRecord(entityType, claimGenerationParametersId);
            return response.json();
          }
          else {
            Xrm.Navigation.openAlertDialog({ text: response.statusText });
            return;
          }
        },
        (error) => {
          Xrm.Utility.closeProgressIndicator();
          console.log(error.message);
          Xrm.Navigation.openAlertDialog({ text: error.message });
        })
          .then((result) => {
            if (result.HasErrors == true) {
              Xrm.Navigation.openAlertDialog({ text: result.Message });
            }
            else {
              console.log(result.PdfFileBase64);

              let blob = b64toBlob(result.PdfFileBase64, "application/pdf");
              let blobUrl = URL.createObjectURL(blob); 
              window.open(blobUrl, "_blank");
            }
        });
      }
    }),
    ((error) => {
      console.log(error);
      Xrm.Navigation.openAlertDialog({ text: error.message });
    });
  }

  function b64toBlob(b64Data, contentType, sliceSize = 512) {
    let byteCharacters = atob(b64Data);
    let byteArrays = [];

    for (var offset = 0; offset < byteCharacters.length; offset += sliceSize) {
      let slice = byteCharacters.slice(offset, offset + sliceSize);
      let byteNumbers = new Array(slice.length);
      
      for (var i = 0; i < slice.length; i++) {
        byteNumbers[i] = slice.charCodeAt(i);
      }

      let byteArray = new Uint8Array(byteNumbers);
      byteArrays.push(byteArray);
    }

    const blob = new Blob(byteArrays, { type: contentType });
    return blob;
  }
}
