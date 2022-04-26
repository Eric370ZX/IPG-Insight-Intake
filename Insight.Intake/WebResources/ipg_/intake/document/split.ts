/**
 * @namespace Intake.Document.Split
 */
namespace Intake.Document.Split {
  let DocumentCategoryTypeName: string = "Patient Procedure";

  let docId: string;

  export function onSelectReferralOrCaseClick() {
    parent.Xrm.Utility.lookupObjects({
      entityTypes: ["incident", "ipg_referral"],
    }).then(onReferralOrCaseSelected, function () {
      parent.Xrm.Navigation.openErrorDialog({ message: "An error occured" });
    });
  }

  export async function InitPage() {
    docId = getDocIdParameter();
    if (!docId) {
      parent.Xrm.Navigation.openErrorDialog({
        message: "Document ID parameter is required",
      });
      return;
    }

    await populateDocumentInfo();

    //populate document types
    let documentTypes: ValueAndText[] = [];
    parent.Xrm.Utility.showProgressIndicator(null);
    await parent.Xrm.WebApi.retrieveMultipleRecords(
      "ipg_documenttype",
      `?$filter=ipg_DocumentCategoryTypeId/ipg_name eq '${encodeURIComponent(
        DocumentCategoryTypeName
      )}' and statecode eq 0&$orderby=ipg_name`
    ).then(function (result) {
      parent.Xrm.Utility.closeProgressIndicator();

      for (var i = 0; i < result.entities.length; i++) {
        documentTypes.push({
          value: result.entities[i].ipg_documenttypeid,
          text: result.entities[i].ipg_name,
        });
      }

      populateDocumentTypes(documentTypes);
    });
  }

  export function onPreviewClick() {
    if (!docId) {
      return;
    }

    Intake.Document.previewById(docId);
  }

  export function onProcessClick() {
    splitDocument(false, false);
  }

  export function onSplitAndInitiateClick() {
    splitDocument(false, true);
  }

  export function onCancelClick(): void {
    parent.Xrm.Utility.confirmDialog(
      "Do you want to close this page?",
      () => window.close(),
      null
    );
  }

  async function populateDocumentInfo(): Promise<void> {
    parent.Xrm.Utility.showProgressIndicator(null);
    return parent.Xrm.WebApi.retrieveRecord(
      "ipg_document",
      docId,
      "?$select=ipg_documentid,ipg_name&$expand=ipg_ReferralId($select=ipg_referralid,ipg_name),ipg_CaseId($select=incidentid,title)"
    ).then(function (doc) {
      parent.Xrm.Utility.closeProgressIndicator();

      //populate the document title hyperlink
      let docTitleHyperlink = <HTMLAnchorElement>(
        document.getElementById("documentTitleHyperlink")
      );
      docTitleHyperlink.innerText = doc.ipg_name;
      docTitleHyperlink.onclick = function () {
        parent.Xrm.Navigation.openForm({
          entityName: "ipg_document",
          entityId: doc.ipg_documentid,
          openInNewWindow: true,
        });
      };

      //populate the case control from the document
      if (doc.ipg_CaseId) {
        selectReferralOrCase(
          null,
          doc.ipg_CaseId.incidentid,
          doc.ipg_CaseId.title
        );
      } else if (doc.ipg_ReferralId) {
        selectReferralOrCase(
          doc.ipg_ReferralId.ipg_referralid,
          null,
          doc.ipg_ReferralId.ipg_name
        );
      }
    });
  }

  function onReferralOrCaseSelected(selectedEntities: Xrm.LookupValue[]): void {
    if (selectedEntities && selectedEntities.length) {
      if (selectedEntities[0].entityType == "ipg_referral") {
        selectReferralOrCase(
          selectedEntities[0].id,
          null,
          selectedEntities[0].name
        );
      } else if (selectedEntities[0].entityType == "incident") {
        selectReferralOrCase(
          null,
          selectedEntities[0].id,
          selectedEntities[0].name
        );
      }
    }
  }

  async function splitDocument(
    isContinue: boolean,
    initiateReferral: boolean
  ): Promise<void> {
    if (!docId) {
      return;
    }

    var pifCount = 0;
    let postObject: any = {};

    for (var i = 0; i < 10; i++) {
      let range: string = (<HTMLInputElement>(
        document.getElementById("rangeInput" + i)
      )).value.trim();
      if (range) {
        let docTypeSelectItem = <HTMLSelectElement>(
          document.getElementById("docTypeSelect" + i)
        );
        let docTypeId: string = docTypeSelectItem.value;
        let selectedOptionIndex = docTypeSelectItem.selectedIndex;
        if (
          docTypeSelectItem.options[selectedOptionIndex].innerHTML ===
          "Patient Information Form"
        )
          pifCount++;
        if (!docTypeId) {
          parent.Xrm.Navigation.openAlertDialog({
            text: "Please select a document type for this range: " + range,
          });
          return;
        }

        let description: string = (<HTMLInputElement>(
          document.getElementById("description" + i)
        )).value.trim();

        postObject["Range" + i] = range;
        postObject["DocTypeId" + i] = {
          "@data.type": "mscrm.ipg_documenttype",
          ipg_documenttypeid: docTypeId,
        };
        postObject["Description" + i] = description;
      } else {
        if (i == 0) {
          parent.Xrm.Navigation.openAlertDialog({
            text: "Please enter the range #1",
          });
          return;
        }
      }
    }

    let referralId: string = (<HTMLInputElement>(
      document.getElementById("referralIdInput")
    )).value;
    if (referralId) {
      postObject["ReferralId"] = {
        "@data.type": "Microsoft.Dynamics.CRM.ipg_referral",
        ipg_referralid: referralId,
      };
    }

    let caseId: string = (<HTMLInputElement>(
      document.getElementById("caseIdInput")
    )).value;
    if (caseId) {
      postObject["CaseId"] = {
        "@data.type": "Microsoft.Dynamics.CRM.incident",
        incidentid: caseId,
      };
    }

    const globalContext: Xrm.GlobalContext =
      parent.Xrm.Utility.getGlobalContext();
    const clientUrl: string = globalContext.getClientUrl();

    if (initiateReferral) {
      if (pifCount > 1) {
        parent.Xrm.Navigation.openAlertDialog({
          text: "Referral can be initiated only from one PIF Document.",
        });
        return;
      } else if (pifCount === 0) {
        parent.Xrm.Navigation.openAlertDialog({
          text: "Referral can be initiated only from PIF Document.",
        });
        return;
      }
    }

    parent.Xrm.Utility.showProgressIndicator(null);
    (<any>parent).$.ajax({
      method: "POST",
      url:
        clientUrl +
        "/api/data/v9.0/ipg_documents(" +
        docId +
        ")/Microsoft.Dynamics.CRM.ipg_IPGDocumentActionsSplit",
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify(postObject),
      dataType: "json",
      success: async function (response) {
        parent.Xrm.Utility.closeProgressIndicator();
        parent.Xrm.Navigation.openAlertDialog({
          text: "Document split has finished",
        });

        if (isContinue && response.NewDocumentId) {
          parent.Xrm.Navigation.openWebResource(
            "ipg_/intake/document/split.html",
            {
              width: window.innerWidth,
              height: window.innerHeight,
              openInNewWindow: false,
            },
            response.NewDocumentId
          );
          window.close();
        } else if (initiateReferral) {
          parent.Xrm.Utility.showProgressIndicator(null);
          parent.Xrm.WebApi.retrieveMultipleRecords(
            "ipg_document",
            "?$select=ipg_documentid&$filter=ipg_OriginalDocumentId/ipg_documentid eq " +
              docId
          ).then(
            function success(docIds) {
              let idsArray = docIds.entities.map(function (d) {
                return d.ipg_documentid;
              });
              parent.Xrm.Utility.closeProgressIndicator();
              Intake.Core.Document.InitiateReferral(idsArray);
            },
            function (error) {
              parent.Xrm.Utility.closeProgressIndicator();
              parent.Xrm.Navigation.openAlertDialog({
                text: "Cannot retrieve documents.",
              });
              return;
            }
          );
        } else {
          if (parent && parent.opener) {
            parent.opener.postMessage("REFRESH_DOCUMENTS_GRID");
          }

          window.close();
        }

        if(parent && parent.opener){
          parent.opener.NavigateToDocumentsView();
        }
      },
      error: function (xhr, ajaxOptions, thrownError) {
        parent.Xrm.Navigation.openErrorDialog({
          message: "Document split has failed",
        }); //todo: clear validation messages from server side
        parent.Xrm.Utility.closeProgressIndicator();
      },
    });
  }

  function populateDocumentTypes(documentTypes: ValueAndText[]) {
    filterDocumentTypes(documentTypes);

    for (var i = 0; i < 10; i++) {
      let selectElement: HTMLSelectElement = <HTMLSelectElement>(
        document.getElementById("docTypeSelect" + i)
      );
      for (var j = 0; j < documentTypes.length; j++) {
        let newOption: HTMLOptionElement = document.createElement("option");
        newOption.value = documentTypes[j].value;
        newOption.text = documentTypes[j].text;
        selectElement.add(newOption);
      }
    }
  }

  function filterDocumentTypes(documentTypes: Array<ValueAndText>) {
    const moveElements = (
      arr: Array<ValueAndText>,
      fromIndex: number,
      toIndex: number
    ) => {
      let element = arr[fromIndex];
      arr.splice(fromIndex, 1);
      arr.splice(toIndex, 0, element);
    };
    documentTypesOrder.reverse().forEach((orderType) => {
      let index = documentTypes.findIndex(
        (docType: ValueAndText) => docType.text == orderType
      );
      if (index != -1) moveElements(documentTypes, index, 0);
    });
  }

  function selectReferralOrCase(
    referralId: string,
    caseId: string,
    name: string
  ): void {
    let referralIdInput: HTMLInputElement = <HTMLInputElement>(
      document.getElementById("referralIdInput")
    );
    referralIdInput.value = referralId;
    let caseIdInput: HTMLInputElement = <HTMLInputElement>(
      document.getElementById("caseIdInput")
    );
    caseIdInput.value = caseId;
    let referralOrCaseNameInput: HTMLInputElement = <HTMLInputElement>(
      document.getElementById("referralOrCaseNameInput")
    );
    referralOrCaseNameInput.value = name;
  }

  function getDocIdParameter(): string {
    let docId: string;
    let dataParam: string = Intake.Utility.GetDataParam();
    if (dataParam) {
      let dataParamDecoded: string = decodeURIComponent(dataParam);
      docId = Intake.Utility.removeCurlyBraces(dataParamDecoded);
    }
    if (!docId) {
      parent.Xrm.Navigation.openErrorDialog({
        message: "Error. Document ID parameter is required",
      });
      return null;
    }

    return docId;
  }

  class ValueAndText {
    value: string;
    text: string;
  }
  const documentTypesOrder = [
    "Patient Information Form",
    "Implant Charge Sheet",
    "Manufacturer Invoice",
  ];
}
