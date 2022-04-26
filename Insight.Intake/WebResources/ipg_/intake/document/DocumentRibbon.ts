/**
 * @namespace Intake.Document
 */
 namespace Intake.Document {
   enum ReviewStatuses{
     Used= 427880006
    }
    const enum CustomerTypeCodes{
      Facility = 923720000,
      Carrier = 923720001,
      Manufacturer = 923720002
    }
    const enum DocumentCategoryId{
      Facility = "a31d69a3-2877-5ba0-298f-48a0a2dccf10",
      Carrier = "1c70ac42-b871-7557-d20e-48a0a21b8237",
      Manufacturer = "ccd48636-a66a-e911-a97b-000d3a370868"
    }
    /**
     * Opens Document Split HTML web resource.
     * @function Intake.Document.OpenDocumentSplit
     * @returns {void}
     */
  export function OpenDocumentSplit(primaryControl: any) {
    let firstSelectedItemId: string;
    if (primaryControl.getGrid) {
      let gridControl = primaryControl as Xrm.Controls.GridControl;
      let grid = gridControl.getGrid();
      let selectedRows = grid.getSelectedRows();
      if (selectedRows && selectedRows.getLength()) {
        firstSelectedItemId = selectedRows.getByIndex(0).data.entity.getId();
      }
      (window as any ).NavigateToDocumentsView = NavigateToDocumentsView;
      window.parent.addEventListener(
        "message",
        (event) => {
          if (event.data == "REFRESH_DOCUMENTS_GRID" && gridControl) {
            gridControl.refresh();
          }
        },
        false
      );
    } else {
      let formContext: Xrm.FormContext = primaryControl;
      firstSelectedItemId = formContext.data.entity.getId();
      window.parent.addEventListener(
        "message",
        (event) => {
          if (event.data == "REFRESH_DOCUMENTS_GRID") {
            formContext.data.refresh(true);
          }
        },
        false
      );
    }

    if (!firstSelectedItemId) {
      alert("Document ID is required to split the document");
      return;
    }

    Xrm.WebApi.retrieveRecord(
      "ipg_document",
      firstSelectedItemId,
      "?$select=ipg_documentid&$expand=ipg_DocumentTypeId($select=ipg_documenttypeid,ipg_documenttypeabbreviation)"
    ).then(
      function success(doc: any) {
        const myUrlWithParams = new URL(
          Xrm.Utility.getGlobalContext().getCurrentAppUrl()
        );
        myUrlWithParams.searchParams.append("pagetype", "webresource");
        myUrlWithParams.searchParams.append(
          "webresourceName",
          "ipg_/intake/document/split.html"
        );
        myUrlWithParams.searchParams.append("cmdbar", "true");
        myUrlWithParams.searchParams.append("navbar", "off");
        myUrlWithParams.searchParams.append("data", firstSelectedItemId);
        myUrlWithParams.searchParams.append("newWindow", "true");

        window.open(
          myUrlWithParams.toString(),
          "_blank",
          "height=700,width=800"
        );
      },
      (error: any) => {
        alert("Could not retrieve ipg_document: " + error.message);
      }
    );
  }
  function NavigateToDocumentsView(){
    let pageInput: Xrm.Navigation.PageInputEntityList = {
      entityName: "ipg_document",
      pageType: "entitylist"
    };

    let navigationOptions : Xrm.Navigation.NavigationOptions = {
      target: 1
    };

    Xrm.Navigation.navigateTo(pageInput, navigationOptions)
  }
  /**
   * enable rule for check selected document review status
   * @function Intake.Document.isEnableReviewStatus
   * @returns {boolean}
   */
  export async function isEnableReviewStatus(selectedRecordId: string) {
    const status = (
      await Xrm.WebApi.online.retrieveRecord(
        "ipg_document",
        selectedRecordId.toLocaleLowerCase().replace("{", "").replace("}", ""),
        "?$select=ipg_reviewstatus"
      )
    )?.ipg_reviewstatus;
    return status != ReviewStatuses.Used;
  }

/**
   * enable rule for Initiate Referral button
   * @function Intake.Document.hasNotMoreThanTwoReferrals
   * @returns {boolean}
   */
 export async function hasNotMoreThanTwoReferrals(firstPrimaryItemId){
    if (firstPrimaryItemId.replace("{", "").replace("}", "")) {
        var document = await Xrm.WebApi.retrieveRecord(
          "ipg_document",
          firstPrimaryItemId,
          "?$select=_ipg_referralid_value,ipg_documentid&$expand=ipg_DocumentTypeId($select=ipg_documenttypeabbreviation, ipg_name)"
        );

        if(document.ipg_DocumentTypeId?.ipg_documenttypeabbreviation === 'PIF')
        {
            var referrals = await Xrm.WebApi.retrieveMultipleRecords("ipg_referral", `?$select=ipg_referralid&$filter=_ipg_sourcedocumentid_value eq ${Utility.removeCurlyBraces(document.ipg_documentid) }`);
            if(referrals.entities.length > 1 ){
              return false;
            }
            else{
              return true;
            }
        }     
    }
 }

  /**
   * enable rule for buttons by document type's name
   * @function Intake.Document.isEnableByDocumentTypeName
   * @returns {boolean}
   */
  export function isEnableByDocumentTypeName(
    primaryControl,
    documentTypeNames: string,
    enableIfNoDocType: boolean
  ): boolean {
    let formContext: Xrm.FormContext = primaryControl;
    let documentType: Xrm.LookupValue[] = null;
    let referral: Xrm.LookupValue[] = null;
    let fileExists: boolean = false;
    let documentTypeNamesArray: Array<string> = null;

    if (primaryControl.getGrid) {
      var grid = primaryControl.getGrid();
      var selectedRows = grid.getSelectedRows();
      if (selectedRows && selectedRows.getLength()) {
        documentTypeNamesArray = selectedRows.getAll().map(function (d) {
          return d.getAttribute("ipg_documenttypeid").getValue()[0].name;
        });
        if (
          documentTypeNamesArray.filter(function (d) {
            return d === "Patient Information Form";
          }).length > 1
        ) {
          return false;
        }
        fileExists = true;
      }
    } else {
      var entityType = formContext.data.entity.getEntityName();
      if (entityType == "ipg_document") {
        var attribute = formContext.getAttribute("ipg_documenttypeid");
        referral = formContext.getAttribute("ipg_referralid").getValue();
        if (attribute && attribute.getIsDirty() == false) {
          documentTypeNamesArray = attribute.getValue().map(function (a) {
            return a.name;
          });
        }
      } else {
        throw new Error("Unexpected entityType: " + entityType);
      }
      fileExists = !!formContext.data.entity.getId();
    }
    if (
      enableIfNoDocType &&
      fileExists &&
      (!documentTypeNamesArray || !documentTypeNamesArray.length)
    ) {
      return true;
    }
    if (documentTypeNames == "Patient Information Form") {
      if (
        (!referral || !referral.length) &&
        documentTypeNamesArray != null &&
        documentTypeNamesArray.length &&
        documentTypeNamesArray.indexOf(documentTypeNames) >= 0
      ) {
        return true;
      }
    } else {
      var checker = function (arr, target) {
        return target.every(function (v) {
          return arr.includes(v);
        });
      };
      if (
        documentTypeNamesArray != null &&
        documentTypeNamesArray.length &&
        checker(documentTypeNames.split("|"), documentTypeNamesArray)
      ) {
        return true;
      }
    }
    return false;
  }

  export async function isEnableByNumberOfPages(primaryControl) {
    if (primaryControl.getGrid) {
      var grid = primaryControl.getGrid();
      var selectedRows = grid.getSelectedRows();
      if (selectedRows && selectedRows.getLength()) {
        var documentId = selectedRows.get(0).getData().getEntity().getId();
        var document = await Xrm.WebApi.retrieveRecord(
          "ipg_document",
          documentId,
          "?$select=ipg_numberofpages"
        );
        return document.ipg_numberofpages > 1;
      }
    } else {
      var numberOfPages = primaryControl
        .getAttribute("ipg_numberofpages")
        .getValue();
      if (numberOfPages) {
        return numberOfPages > 1;
      }
    }
    return false;
  }

  /**
   * Open Document Preview HTML web resource.
   * @function Intake.Document.OpenDocumentPreview
   * @returns {void}
   */
  export async function OpenDocumentPreview(
    primaryControl,
    firstSelectedRecordId
  ): Promise<void> {
    let formContext: Xrm.FormContext = primaryControl;

    let currentDocId: string = null;
    if (firstSelectedRecordId) {
      currentDocId = firstSelectedRecordId.replace("{", "").replace("}", "");
    } else {
      currentDocId = formContext.data.entity.getId();
    }

    if (currentDocId) {
      //DocumentCommon.js has been added to dependencies, but it did not load on 9/13/2020. Try later.
      //Intake.Document.previewById(currentReccord.id);

      let doc = await Xrm.WebApi.retrieveRecord(
        "ipg_document",
        currentDocId,
        "?$expand=ipg_DocumentTypeId($select=ipg_documenttypeabbreviation)&$select=_ipg_ebvresponseid_value"
      );
      if (doc && doc.ipg_DocumentTypeId.ipg_documenttypeabbreviation == "EBV") {
        //preview EBV doc

        if (doc._ipg_ebvresponseid_value) {
          Xrm.Navigation.openWebResource(
            "ipg_/intake/EBVResponse/ViewDetails.html",
            { width: 1000, height: 800, openInNewWindow: true },
            doc._ipg_ebvresponseid_value
          );
        } else {
          Xrm.Navigation.openAlertDialog({ text: "EBV Response is not set" });
        }

        return;
      }

      let env: string;
      if (location.host.indexOf("-dev") >= 0) {
        env = "dev";
      } else if (location.host.indexOf("-qa") >= 0) {
        env = "qa";
      } else if (location.host.indexOf("-prd") >= 0) {
        env = "prd";
      } else {
        env = "";
      }
      let docAppEnvSuffix: string;
      if (env) {
        docAppEnvSuffix = "-" + env;
      } else {
        docAppEnvSuffix = "";
      }

      Xrm.WebApi.retrieveMultipleRecords(
        "annotation",
        "?$filter=_objectid_value eq '" + currentDocId + "'"
      ).then(
        (result) => {
          const height: number = 600;
          const width: number = 800;

          if (result && result.entities.length) {
            let annotationId = result.entities[0].annotationid;
            Xrm.Navigation.openUrl(
              `https://insight-documents${docAppEnvSuffix}.azurewebsites.net/documents/${annotationId}`,
              { height: height, width: width }
            );
          } else {
            Xrm.Navigation.openUrl(
              `https://insight-documents${docAppEnvSuffix}.azurewebsites.net/legacydocuments/${currentDocId}`,
              { height: height, width: width }
            );
          }
        },
        (error) => {
          Xrm.Navigation.openErrorDialog({ message: error.message });
        }
      );
    }
  }

  /**
   * Opens "Merge Documents" HTML web resource.
   * @function Intake.Document.OpenDocumentMerge
   * @returns {void}
   */
  export async function OpenDocumentMerge(primaryControl: Xrm.FormContext, selectedRecordIds: string[]) {
    interface MergeInputParams{
      caseId: string,
      docIds : string[]
    }

    let incidentId: string = primaryControl.data.entity.getId();
    incidentId = incidentId.substring(1, incidentId.length - 1);
    let inputParams: MergeInputParams = {
      caseId : incidentId,
      docIds : selectedRecordIds
    };
    (window as any).parentControl = primaryControl; 

    if (await CheckPPPAvailability(incidentId)) {
      Xrm.Navigation.openWebResource(
        "ipg_/intake/document/merge.html",
        { width: 800, height: 670, openInNewWindow: true },
        "params=" + JSON.stringify(inputParams)
      );   
    }
  }

  /**
   * Open lookup form with "FacilityDocument" view by default
   * @function Intake.Document.AddExistingFacilityDocuments
   * @returns {void}
   */
  export function AddExistingFacilityDocuments(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;
    let options = {
      allowMultiSelect: false,
      defaultEntityType: "ipg_document",
      entityTypes: ["ipg_document"],
      defaultViewId: "98485AE2-3E8C-E911-A97E-000D3A37043B", //Facility Documents
      disableMru: true,
    };

    Xrm.Utility.lookupObjects(options).then(
      (documnet) => {
        if (documnet && documnet.length) {
          let account = formContext.data.entity.getEntityReference();
          associateRecords(
            "ipg_ipg_document_account",
            { EntityListName: "accounts", Id: account.id },
            { EntityListName: "ipg_documents", Id: documnet[0].id }
          ).then((response) => {
            formContext.data.refresh(true);
          });
        }
      },
      (error) => {
        Xrm.Navigation.openErrorDialog({ message: error.message });
      }
    );
  }

  /**
   * enable rule for "Add Facility Document" button
   * @function Intake.Document.addFacilityDocumentIsVisible
   * @returns {boolean}
   */
  export function addFacilityDocumentIsVisible(primaryControl): boolean {
    let formContext: Xrm.FormContext = primaryControl;
    let accountType = null;

    if (formContext.data.entity.getEntityName() == "account") {
      accountType = formContext.getAttribute("customertypecode").getValue();
      if (accountType && accountType === 923720000) {
        //facility
        return true;
      }
    }
    return false;
  }

  /**
   * call Web Api for creating a relationship between entities
   * @function Intake.Document.AssociateRecords
   * @returns {Intake.Utility.HttpRequest}
   */
  function associateRecords(
    relationshipName: string,
    targetEntity: any,
    sourceEntity: any
  ) {
    const globalContext = Xrm.Utility.getGlobalContext();
    const clientUrl = globalContext.getClientUrl();
    const requestOptions: Intake.Utility.HttpRequestOptions<{}> = {
      path: `${clientUrl}/api/data/v9.0/${
        targetEntity.EntityListName
      }(${Intake.Utility.removeCurlyBraces(
        targetEntity.Id
      )})/${relationshipName}/$ref`,
      body: {
        "@odata.id":
          clientUrl +
          `/api/data/v9.0/${
            sourceEntity.EntityListName
          }(${Intake.Utility.removeCurlyBraces(sourceEntity.Id)})`,
      },
      headers: {
        "OData-MaxVersion": "4.0",
        "OData-Version": "4.0",
        Accept: "application/json",
        "Content-Type": "application/json",
      },
    };
    return Intake.Utility.HttpRequest.post(requestOptions)
      .then((response: any) => response)
      .catch((response: { error: { message: string } }) => {
        return Xrm.Navigation.openErrorDialog({
          message: response.error.message,
        }).then(() => null);
      });
  }

  /**
   * call Web Api for creating a relationship between entities
   * @function Intake.Document.AssociateRecords
   * @returns {Intake.Utility.HttpRequest}
   */
  function disassociateRecords(
    relationshipName: string,
    targetEntity: any,
    sourceEntity: any
  ) {
    const globalContext = Xrm.Utility.getGlobalContext();
    const clientUrl = globalContext.getClientUrl();
    const requestOptions: Intake.Utility.HttpRequestOptions<{}> = {
      path: `${clientUrl}/api/data/v9.0/${
        targetEntity.EntityListName
      }(${Intake.Utility.removeCurlyBraces(
        targetEntity.Id
      )})/${relationshipName}(${Intake.Utility.removeCurlyBraces(
        sourceEntity.Id
      )})/$ref`,
      headers: {
        "OData-MaxVersion": "4.0",
        "OData-Version": "4.0",
        Accept: "application/json",
        "Content-Type": "application/json",
      },
    };
    return Intake.Utility.HttpRequest.delete(requestOptions)
      .then((response: any) => response)
      .catch((response: { error: { message: string } }) => {
        return Xrm.Navigation.openErrorDialog({
          message: response.error.message,
        }).then(() => null);
      });
  }

  /**
   * Associate case part detail and document
   * @function Intake.Document.AssociateDocumentToCasePartDetail
   * @returns {Intake.Utility.HttpRequest}
   */
  export function associateDocumentToCasePartDetail(
    primaryControl,
    selectedItemsReferences
  ) {
    let formContext: Xrm.FormContext = primaryControl;
    let orderProductsControl = formContext.getControl("PurchaseOrderDetails");
    if (orderProductsControl) {
      if (orderProductsControl.getGrid().getSelectedRows().getLength()) {
        let selectedOrderProduct = orderProductsControl
          .getGrid()
          .getSelectedRows()
          .getByIndex(0);
        let product: Xrm.LookupValue[] = selectedOrderProduct
          .getAttribute("productid")
          .getValue();
        let caseRef: Xrm.LookupValue[] = formContext
          .getAttribute("ipg_caseid")
          .getValue();
        if (product && product.length && caseRef && caseRef.length) {
          Xrm.Utility.showProgressIndicator("Please wait");
          CallCloneDocumentAction(selectedItemsReferences[0].Id).then(
            (result1) => {
              if (result1.ipg_documentid) {
                let associateInvoiceToCase = associateRecords(
                  "ipg_incident_ipg_document_CaseId",
                  {
                    EntityListName: "incidents",
                    Id: caseRef[0].id.replace("{", "").replace("}", ""),
                  },
                  {
                    EntityListName: "ipg_documents",
                    Id: result1.ipg_documentid,
                  }
                );
                let associateInvoiceToCasePartDetail =
                  Xrm.WebApi.retrieveMultipleRecords(
                    "ipg_casepartdetail",
                    "?$select=ipg_casepartdetailid&$filter=ipg_productid/productid eq " +
                      product[0].id +
                      " and ipg_caseid/incidentid eq " +
                      caseRef[0].id
                  ).then((result) => {
                    if (result && result.entities.length) {
                      associateRecords(
                        "ipg_ipg_casepartdetail_ipg_document",
                        {
                          EntityListName: "ipg_casepartdetails",
                          Id: result.entities[0].ipg_casepartdetailid,
                        },
                        {
                          EntityListName: "ipg_documents",
                          Id: result1.ipg_documentid,
                        }
                      );
                    }
                  });
                Promise.all([
                  associateInvoiceToCase,
                  associateInvoiceToCasePartDetail,
                ])
                  .then((results) => {
                    Xrm.Utility.alertDialog(
                      "The invoice was associated to the Case Part Detail.",
                      function () {
                        formContext
                          .getControl("PurchaseOrderDetails")
                          .refresh();
                        Xrm.Utility.closeProgressIndicator();
                      }
                    );
                  })
                  .catch((error) => {
                    Xrm.Utility.alertDialog(error.Message, null);
                    Xrm.Utility.closeProgressIndicator();
                  });
              }
            }
          );
        }
      }
    }
  }

  function CallCloneDocumentAction(entityId: string) {
    if (entityId) {
      let request = {
        Record: {
          "@odata.type": "Microsoft.Dynamics.CRM.ipg_document",
          ipg_documentid: entityId,
        },
        getMetadata: function () {
          return {
            boundParamter: null,
            parameterTypes: {
              Record: {
                typeName: "mscrm.ipg_document",
                structuralProperty: 5,
              },
            },
            operationType: 0,
            operationName: "ipg_IPGIntakeActionsCloneRecord",
          };
        },
      };

      return Xrm.WebApi.online.execute(request).then(
        (response) => {
          Xrm.Utility.closeProgressIndicator();
          if (response.ok) {
            return response.json();
          } else {
            Xrm.Navigation.openErrorDialog({ message: response.statusText });
          }
        },
        (error) => {
          Xrm.Navigation.openErrorDialog({ message: error.message });
        }
      );
    }
  }

  /**
   * Action to reassign a document from one case to another
   * @function Intake.Document.executeReassingAction
   */
  export async function executeReassingAction(
    incidentId: string,
    documentId: string
  ): Promise<any> {
    if (incidentId && documentId) {
      const request = {
        entity: {
          id: documentId,
          entityType: "ipg_document",
        },
        CaseRef: {
          id: incidentId,
          entityType: "incident",
        },
        getMetadata: function () {
          return {
            boundParameter: "entity",
            parameterTypes: {
              entity: {
                typeName: "mscrm.ipg_document",
                structuralProperty: 5,
              },
              CaseRef: {
                typeName: "mscrm.incident",
                structuralProperty: 5,
              },
            },
            operationType: 0,
            operationName: "ipg_IPGDocumentActionsReassignToCase",
          };
        },
      };

      return Xrm.WebApi.online.execute(request);
    }
  }

  /**
   * enable rule for "Associate To Actual Part" button
   * @function Intake.Document.associateToActualPartIsVisible
   * @returns {boolean}
   */
  export function associateToActualPartIsVisible(selectedControl): boolean {
    if (selectedControl.name == "Invoices") {
      return true;
    }
    return false;
  }

  export async function CheckPPPAvailability(caseId: string): Promise<boolean> {
    try {
      const documents = await parent.Xrm.WebApi.retrieveMultipleRecords(
        "ipg_document",
        `?$select=ipg_documentid&$expand=ipg_DocumentTypeId($select=ipg_documenttypeabbreviation)&$filter=_ipg_caseid_value eq ${Intake.Utility.removeCurlyBraces(
          caseId
        )} and ipg_DocumentTypeId/ipg_documenttypeid ne null and statecode eq 0`
      );

      if (!documents || documents.entities.length == 0) {
        parent.Xrm.Navigation.openErrorDialog({
          message: "Case does not have documents!",
        });
        return false;
      }
    } catch (e) {
      parent.Xrm.Navigation.openErrorDialog(e);
      return false;
    }

    return true;
  }
  export const ChargeSheet = "ICS";
  export const Invoice = "MFG INV";
  export async function GetCpaPOByCase(caseId: string): Promise<boolean> {
    const cpaTypeCode = 923720002;

    const result = await parent.Xrm.WebApi.retrieveMultipleRecords(
      "salesorder",
      `?$top=1&$filter=_ipg_caseid_value eq ${Intake.Utility.removeCurlyBraces(
        caseId
      )} and ipg_potypecode eq ${cpaTypeCode}  and statecode eq 0`
    );

    if (result && result.entities.length > 0) {
      return true;
    } else {
      return false;
    }
  }

  /**
   * command for "Reassign another Case" button
   * @function Intake.Document.MoveDocumentToAnotherCase
   * @returns {boolean}
   */
  export async function MoveDocumentToAnotherCase(
    primaryControl,
    selectedItemIds,
    selectedControl
  ) {
    let formContext: Xrm.FormContext = primaryControl;
    let options = {
      allowMultiSelect: false,
      defaultEntityType: "incident",
      entityTypes: ["incident"],
      disableMru: true,
    };

    try {
      const incident = await Xrm.Utility.lookupObjects(options);

      if (incident && incident.length) {
        Xrm.Utility.showProgressIndicator("Reassigning a document");

        for (let i = 0; i < selectedItemIds.length; i++) {
          const doc = await Xrm.WebApi.retrieveRecord(
            "ipg_document",
            selectedItemIds[i],
            "?$select=ipg_documentid&$expand=ipg_DocumentTypeId($select=ipg_documenttypeid,ipg_documenttypeabbreviation)"
          );

          if (
            !doc.ipg_DocumentTypeId ||
            doc.ipg_DocumentTypeId.ipg_documenttypeabbreviation !== "PIF"
          ) {
            try {
              const reassignedIncidentId = incident[0].id;

              const result = await Intake.Document.executeReassingAction(
                reassignedIncidentId,
                doc.ipg_documentid
              );

              const Case = await Xrm.WebApi.retrieveRecord(
                "incident",
                reassignedIncidentId,
                "?$select=_ownerid_value"
              );
              const ownerType =
                Case["_ownerid_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
              let data = {
                "ownerid@odata.bind":
                  "/" + ownerType + "s(" + Case["_ownerid_value"] + ")",
              };
              Xrm.WebApi.updateRecord(
                "ipg_document",
                doc.ipg_documentid,
                data
              ).then(
                function success(result) {},
                function (error) {
                  console.log(error.message);
                }
              );

              selectedControl.refresh();
              formContext
                ?.getControl<Xrm.Controls.GridControl>("CommonDocuments")
                ?.refresh();
            } catch (e) {
              Xrm.Navigation.openErrorDialog({
                message:
                  e.Message ||
                  e.message ||
                  "Error during reassinging a document",
              });
            }
          } else {
            const alertStrings = {
              confirmButtonLabel: "Ok",
              text: "Document type must not be PIF document. Skipped",
            };

            const alertOptions = {
              height: 120,
              width: 260,
            };

            Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
          }
        }

        Xrm.Utility.closeProgressIndicator();
      }
    } catch (e) {
      Xrm.Navigation.openErrorDialog({
        message:
          e.Message || e.message || "Error during reassinging a document",
      });
    }
  }

  /**
   * command for "RemoveDocument" button
   * @function Intake.Document.MoveDocumentToAnotherCase
   * @returns {boolean}
   */
  export function RemoveDocument(
    primaryControl,
    selectedItemIds,
    selectedControl
  ) {
    let formContext: Xrm.FormContext = primaryControl;
    let options = {
      allowMultiSelect: false,
      defaultEntityType: "incident",
      entityTypes: ["incident"],
      disableMru: true,
    };
    try {
      for (let i = 0; i < selectedItemIds.length; i++) {
        Xrm.WebApi.retrieveRecord(
          "ipg_document",
          selectedItemIds[i],
          "?$select=ipg_documentid,_ipg_caseid_value&$expand=ipg_DocumentTypeId($select=ipg_documenttypeid,ipg_documenttypeabbreviation)"
        ).then(
          function success(doc: any) {
            if (
              !doc.ipg_DocumentTypeId ||
              !doc._ipg_caseid_value ||
              doc.ipg_DocumentTypeId.ipg_documenttypeabbreviation !== "PIF"
            ) {
              disassociateRecords(
                "ipg_incident_ipg_document_CaseId",
                {
                  EntityListName: "incidents",
                  Id: doc._ipg_caseid_value.replace("{", "").replace("}", ""),
                },
                {
                  EntityListName: "ipg_documents",
                  Id: selectedItemIds[i].replace("{", "").replace("}", ""),
                }
              ).then((response) => {});
            } else {
              alert("Document type must not be PIF document. Skipped");
            }
          },
          (error: any) => {
            console.log("Could not retrieve ipg_document: " + error.message);
          }
        );
      }
      //wait before grid refresh
      setTimeout(function () {
        selectedControl.refresh();
      }, 1000);
    } catch (error) {
      Xrm.Navigation.openErrorDialog({ message: error.message });
    }
  }

  /**
   * enable rule for "Reassign another Case" button
   * @function Intake.Document.IsCaseDocumentGrid
   * @returns {boolean}
   */
  export function IsCaseDocumentGrid(primaryControl, selectedControl) {
    if (selectedControl.getName() === "Documents_Attached") {
      return true;
    }
    return false;
  }
  /**
   * enable rule for not displaying button on Patient Statements subgrid
   * @function Intake.Document.IsStatementDocumentGrid
   * @returns {boolean}
   */
  export function IsStatementDocumentGrid(control){
   const cpntrolName = control.getName();
   return cpntrolName === "PatientStatements" || cpntrolName == "StatementDocuments";
  }
  /**
   * Opens "EBV Response" form. This function can be called for EBV documents from Grid or Form.
   * @function Intake.Document.OpenDocumentMerge
   * @returns {void}
   */
  export async function OpenEbvResponse(selectedControl: any) {
    let firstSelectedItemId: string;
    if (selectedControl.getGrid) {
      let gridControl = selectedControl as Xrm.Controls.GridControl;
      let grid = gridControl.getGrid();
      let selectedRows = grid.getSelectedRows();
      if (selectedRows && selectedRows.getLength()) {
        firstSelectedItemId = selectedRows.getByIndex(0).data.entity.getId();
      }
    } else {
      let formContext: Xrm.FormContext = selectedControl;
      firstSelectedItemId = formContext.data.entity.getId();
    }

    if (!firstSelectedItemId) {
      alert("Document ID is required to open EBV Response details");
      return;
    }

    Xrm.WebApi.retrieveRecord("ipg_document", firstSelectedItemId).then(
      (doc) => {
        if (doc._ipg_ebvresponseid_value) {
          Xrm.Navigation.openForm({
            entityName: "ipg_ebvresponse",
            entityId: doc._ipg_ebvresponseid_value,
            openInNewWindow: true,
          });
        } else {
          Xrm.Utility.alertDialog("No EBV Response", null);
        }
      },
      (error) => {
        console.log(error);
        Xrm.Utility.alertDialog("Could not retrieve Document", null);
      }
    );
  }

  /**
   * Open a new document form
   * @function Intake.Document.NewDocument
   * @returns {void}
   */
  export function NewDocument() {
    Xrm.WebApi.retrieveMultipleRecords(
      "ipg_documenttype",
      "?$select=ipg_documenttypeid,ipg_name&$filter=ipg_name eq 'User Uploaded Generic Document'"
    ).then(
      function success(results) {
        let entityFormOptions = {};
        entityFormOptions["entityName"] = "ipg_document";
        let formParameters = {};
        if (results.entities.length) {
          formParameters["ipg_documenttypeid"] =
            results.entities[0]["ipg_documenttypeid"];
          formParameters["ipg_documenttypeidname"] =
            results.entities[0]["ipg_name"];
          formParameters["ipg_documenttypeidtype"] = "ipg_documenttype";
          //formParameters["ipg_filterdocumenttypes"] = false;
        }
        Xrm.Navigation.openForm(entityFormOptions, formParameters);
      },
      function (error) {
        Xrm.Navigation.openErrorDialog({ message: error.message });
      }
    );
  }

  /**
   * Called on 'AddDocumentFromAnotherCase' button click
   * @function Intake.Document.AddDocumentFromAnotherCase
   * @returns {void}
   */
  export function AddDocumentFromAnotherCase(formContext: any) {
    let entityFormOptions = {};
    entityFormOptions["entityName"] = "ipg_documentrecordcopy";
    entityFormOptions["useQuickCreateForm"] = true;
    entityFormOptions["formId"] = "f5f24590-0c2e-4c51-9ee4-dba509aebc46";

    let formParameters = {};
    formParameters["ipg_caseidentityType"] = "incident";

    Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
      function (success) {
        console.log(success);
        let documentCopyRecordId = null;
        documentCopyRecordId = Intake.Utility.removeCurlyBraces(
          success.savedEntityReference[0].id
        );
        Xrm.WebApi.retrieveRecord(
          "ipg_documentrecordcopy",
          documentCopyRecordId,
          "?$select=_ipg_caseid_value,_ipg_documentid_value"
        ).then(
          function success(result) {
            let _ipg_caseid_value = result["_ipg_caseid_value"];
            let _ipg_documentid_value = result["_ipg_documentid_value"];
            var association = {
              "@odata.id":
                formContext.context.getClientUrl() +
                `/api/data/v9.1/ipg_documents(${_ipg_documentid_value})`,
            };
            var req = new XMLHttpRequest();
            req.open(
              "POST",
              formContext.context.getClientUrl() +
                `/api/data/v9.1/incidents(${_ipg_caseid_value})/ipg_incident_ipg_document/$ref`,
              true
            );
            req.setRequestHeader("Accept", "application/json");
            req.setRequestHeader(
              "Content-Type",
              "application/json; charset=utf-8"
            );
            req.setRequestHeader("OData-MaxVersion", "4.0");
            req.setRequestHeader("OData-Version", "4.0");
            req.onreadystatechange = function () {
              if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 204 || this.status === 1223) {
                  Xrm.Page.getControl("CommonDocuments").refresh();
                }
                // else
                //   {
                //      Xrm.Utility.alertDialog(this.statusText, null);
                //    }
              }
            };
            req.send(JSON.stringify(association));
          },
          function (error) {
            Xrm.Utility.alertDialog(error.message, null);
          }
        );
      },
      function (error) {
        console.log(error);
      }
    );
  }

  /**
   * enable rule for "Reassign another Referral" button
   * @function Intake.Document.IsReferralDocumentGrid
   * @returns {boolean}
   */
  export function IsReferralDocumentGrid(primaryControl, selectedControl) {
    if (selectedControl.getName() === "sgDocumentsForReferral") {
      return true;
    }
    return false;
  }

  export function IsReferral(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;

    const referral = formContext
      .getAttribute<Xrm.Attributes.LookupAttribute>("ipg_referralid")
      .getValue();

    if (referral === null) {
      return false;
    }
    return true;
  }

  export function IsCase(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;

    const Case = formContext
      .getAttribute<Xrm.Attributes.LookupAttribute>("ipg_caseid")
      .getValue();

    if (Case === null) {
      return false;
    }
    return true;
  }

  /**
   * command for "Reassign another Case" button
   * @function Intake.Document.MoveDocumentToAnotherReferral
   * @returns {boolean}
   */
  export async function MoveDocumentToAnotherReferral(
    primaryControl,
    selectedItemIds,
    selectedControl
  ) {
    let formContext: Xrm.FormContext = primaryControl;
    let options = {
      allowMultiSelect: false,
      defaultEntityType: "ipg_referral",
      entityTypes: ["ipg_referral"],
      disableMru: true,
    };

    try {
      const referral = await Xrm.Utility.lookupObjects(options);

      if (referral && referral.length) {
        Xrm.Utility.showProgressIndicator("Reassigning a document");

        for (let i = 0; i < selectedItemIds.length; i++) {
          const doc = await Xrm.WebApi.retrieveRecord(
            "ipg_document",
            selectedItemIds[i],
            "?$select=ipg_documentid&$expand=ipg_DocumentTypeId($select=ipg_documenttypeid,ipg_documenttypeabbreviation)"
          );

          if (
            !doc.ipg_DocumentTypeId ||
            doc.ipg_DocumentTypeId.ipg_documenttypeabbreviation !== "PIF"
          ) {
            try {
              const result =
                await Intake.Document.executeReassingToReferralAction(
                  referral[0].id,
                  doc.ipg_documentid
                );
              selectedControl.refresh();
            } catch (e) {
              Xrm.Navigation.openErrorDialog({
                message:
                  e.Message ||
                  e.message ||
                  "Error during reassinging a document",
              });
            }
          } else {
            const alertStrings = {
              confirmButtonLabel: "Ok",
              text: "Document type must not be PIF document. Skipped",
            };

            const alertOptions = {
              height: 120,
              width: 260,
            };

            Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
          }
        }

        Xrm.Utility.closeProgressIndicator();
      }
    } catch (e) {
      Xrm.Navigation.openErrorDialog({
        message:
          e.Message || e.message || "Error during reassinging a document",
      });
    }
  }

  /**
   * Action to reassign a document from one case to another
   * @function Intake.Document.executeReassingAction
   */
  export async function executeReassingToReferralAction(
    incidentId: string,
    documentId: string
  ): Promise<any> {
    if (incidentId && documentId) {
      const request = {
        entity: {
          id: documentId,
          entityType: "ipg_document",
        },
        ReferralRef: {
          id: incidentId,
          entityType: "ipg_referral",
        },
        getMetadata: function () {
          return {
            boundParameter: "entity",
            parameterTypes: {
              entity: {
                typeName: "mscrm.ipg_document",
                structuralProperty: 5,
              },
              ReferralRef: {
                typeName: "mscrm.ipg_referral",
                structuralProperty: 5,
              },
            },
            operationType: 0,
            operationName: "ipg_IPGDocumentActionsReassignToReferral",
          };
        },
      };

      return Xrm.WebApi.online.execute(request);
    }
  }

  /**
   * command for "Reassign another Case" button
   * @function Intake.Document.ReassignDocumentToAnotherCase
   * @returns {boolean}
   */
  export async function ReassignDocumentToAnotherCase(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;
    let options = {
      allowMultiSelect: false,
      defaultEntityType: "incident",
      entityTypes: ["incident"],
      disableMru: true,
    };

    try {
      const incident = await Xrm.Utility.lookupObjects(options);

      //const incident = formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_caseid");
      if (incident && incident.length) {
        Xrm.Utility.showProgressIndicator("Reassigning a document");

        const result = await Xrm.WebApi.retrieveRecord(
          "ipg_document",
          formContext.data.entity.getId(),
          "?$select=ipg_documentid&$expand=ipg_DocumentTypeId($select=ipg_documenttypeid,ipg_documenttypeabbreviation)"
        );

        if (
          !result.ipg_DocumentTypeId ||
          result.ipg_DocumentTypeId.ipg_documenttypeabbreviation !== "PIF"
        ) {
          try {
            const reassignedIncidentId = incident[0].id;

            const res = await Intake.Document.executeReassingAction(
              reassignedIncidentId,
              result.ipg_documentid
            );
            const Case = await Xrm.WebApi.retrieveRecord(
              "incident",
              reassignedIncidentId,
              "?$select=_ownerid_value"
            );

            var data = {
              "ownerid@odata.bind":
                "/systemusers(" + Case["_ownerid_value"] + ")",
            };
            Xrm.WebApi.updateRecord(
              "ipg_document",
              result.ipg_documentid,
              data
            ).then(
              function success(result) {
                formContext.data.refresh(true);
              },
              function (error) {
                console.log(error.message);
              }
            );
          } catch (e) {
            Xrm.Navigation.openErrorDialog({
              message:
                e.Message || e.message || "Error during reassinging a document",
            });
          }
        } else {
          const alertStrings = {
            confirmButtonLabel: "Ok",
            text: "Document type must not be PIF document. Skipped",
          };

          const alertOptions = {
            height: 120,
            width: 260,
          };

          Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
        }

        Xrm.Utility.closeProgressIndicator();
      }
    } catch (e) {
      Xrm.Navigation.openErrorDialog({
        message:
          e.Message || e.message || "Error during reassinging a document",
      });
    }
  }

  export async function IsAllDocumentsActive(
    selectedDocumentsIds: Array<string>
  ) {
    if (selectedDocumentsIds && selectedDocumentsIds.length > 0) {
      var fetchXML =
        generateFetchXmlToRetrieveDocumentsByIds(selectedDocumentsIds);
      var docs = await Xrm.WebApi.retrieveMultipleRecords(
        "ipg_document",
        fetchXML
      );
      if (docs && docs.entities.length > 0) {
        return docs.entities.every(function (d) {
          return d.statecode === 0;
        });
      }
    }
    return false;
  }

  function generateFetchXmlToRetrieveDocumentsByIds(documentIds) {
    var filterValues = "";
    documentIds.forEach((id) => {
      filterValues += "\n<value>" + id + "</value>";
    });
    var fetchXml =
      `<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="false">
                  <entity name="ipg_document">
                    <attribute name="ipg_documentid" />
                    <attribute name="ipg_name" />
                    <attribute name="statecode" />
                    <attribute name="ipg_caseid" />
                    <attribute name="ipg_referralid" />
                    <filter type="and">
                      <condition attribute="ipg_documentid" operator="in">` +
      filterValues +
      `
                      </condition>
                    </filter>
                    <link-entity name="ipg_documenttype" from="ipg_documenttypeid" to="ipg_documenttypeid" visible="false" link-type="outer" alias="ipg_documenttypeid">
                        <attribute name="ipg_documenttypeid" />
                        <attribute name="ipg_documenttypeabbreviation" />
                        <attribute name="ipg_name" />
                      </link-entity>
                  </entity>
                </fetch>`;
    fetchXml = "?fetchXml=" + encodeURIComponent(fetchXml);
    return fetchXml;
  }

  export async function AttachDocuments(primaryControl, selectedDocumentsIds) {
    var urlParameters = getUrlParameters();
    if (urlParameters && (urlParameters['etn'] === 'incident' || urlParameters['etn'] === 'ipg_referral')) {
      await AttachToCurrentCaseOrReferral(primaryControl, selectedDocumentsIds, urlParameters);
      $("button[data-id='dialogCloseIconButton']", parent.document).click();
      return;
    }

    var Sdk = {
      UpdateRequest: function (entityTypeName, id, payload) {
        this.etn = entityTypeName;
        this.id = id;
        this.payload = payload;
        this.getMetadata = function () {
          return {
            boundParameter: null,
            parameterTypes: {},
            operationType: 2,
            operationName: "Update",
          };
        };
      },
    };

    let options = {
      allowMultiSelect: false,
      defaultEntityType: "incident",
      entityTypes: ["incident", "ipg_referral"],
      disableMru: true,
    };

    try {
      var reference = await Xrm.Utility.lookupObjects(options);

      if (reference && reference.length) {
        Xrm.Utility.showProgressIndicator("Reassigning a document");

        try {
          var incidentId, referralId;
          var requests = [];
          if (reference[0].entityType === "incident") {
            incidentId = reference[0].id.replace(/[{|}]/g, "");

            let newDocData = {
              "ipg_CaseId@odata.bind": "/incidents(" + incidentId + ")",
            };

            var fetchXml =
              generateFetchXmlToRetrieveDocumentsByIds(selectedDocumentsIds);
            var docs = await Xrm.WebApi.retrieveMultipleRecords(
              "ipg_document",
              fetchXml
            );
            if (docs && docs.entities.length > 0) {
              docs.entities.forEach((doc) => {
                if (
                  doc["ipg_documenttypeid.ipg_documenttypeabbreviation"] ===
                  "MFG INV"
                ) {
                  var associateRequest = {
                    getMetadata: () => ({
                      boundParameter: null,
                      parameterTypes: {},
                      operationType: 2,
                      operationName: "Associate",
                    }),

                    relationship: "ipg_incident_ipg_document",

                    target: {
                      entityType: "incident",
                      id: incidentId,
                    },

                    relatedEntities: [
                      {
                        entityType: "ipg_document",
                        id: doc.ipg_documentid,
                      },
                    ],
                  };
                  requests.push(associateRequest);
                } else {
                  requests.push(
                    new Sdk.UpdateRequest(
                      "ipg_document",
                      doc.ipg_documentid,
                      newDocData
                    )
                  );
                }
              });
            }
          } else if (reference[0].entityType === "ipg_referral") {
            referralId = reference[0].id.replace(/[{|}]/g, "");

            let newDocData = {
              "ipg_ReferralId@odata.bind": "/ipg_referrals(" + referralId + ")",
            };

            selectedDocumentsIds.forEach((docId) => {
              requests.push(
                new Sdk.UpdateRequest("ipg_document", docId, newDocData)
              );
            });
          }
          Xrm.WebApi.online.executeMultiple(requests).then(
            function (success) {
              var alertStrings = {
                confirmButtonLabel: "Ok",
                text: "The documents are attached successfully",
                title: "Success",
              };
              var alertOptions = { height: 120, width: 260 };
              Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                function (success) {
                  primaryControl.refresh(true);
                }
              );
            },
            function (error) {
              console.log(error.message);
            }
          );
        } catch (e) {
          Xrm.Navigation.openErrorDialog({
            message:
              e.Message || e.message || "Error during reassinging a document",
          });
        }

        Xrm.Utility.closeProgressIndicator();
      }
    } catch (e) {
      Xrm.Navigation.openErrorDialog({
        message:
          e.Message || e.message || "Error during reassinging a document",
      });
    }
  }

  export async function IfDocumentsHaveNoRelatedReferralAndCase(
    selectedDocumentsIds: Array<string>
  ) {
    if (selectedDocumentsIds && selectedDocumentsIds.length > 0) {
      var fetchXML =
        generateFetchXmlToRetrieveDocumentsByIds(selectedDocumentsIds);
      var docs = await Xrm.WebApi.retrieveMultipleRecords(
        "ipg_document",
        fetchXML
      );
      if (docs && docs.entities.length > 0) {
        return docs.entities.some(function (d) {
          return (
            (d._ipg_referralid_value == null && d._ipg_caseid_value == null) ||
            d["ipg_documenttypeid.ipg_documenttypeabbreviation"] === "MFG INV"
          );
        });
      }
    }
    return false;
  }

  export function SoftDeleteDocument(primaryControl) {
    var formContext: Xrm.FormContext = primaryControl;
    var confirmStrings = {
      text: "Select 'OK' to deactivate this document or 'Cancel' to cancel and return to previous screen. \nNote: system will retain this document as an inactive document for 180 days.",
      title: "Confirm Deletion",
    };
    var confirmOptions = { height: 200, width: 450 };
    Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
      function (success) {
        if (success.confirmed) {
          formContext.getAttribute("statecode").setValue(1);
          formContext.getAttribute("ipg_reviewstatus").setValue(427880004); // Unable to Process
          formContext.data.save();
        }
      }
    );
  }

  export function SoftDeleteMultiple(primaryControl, selectedRecordIds: string[]) {
    var formContext: Xrm.FormContext = primaryControl;
    var Sdk = {
      UpdateRequest: function (entityTypeName, id, payload) {
        this.etn = entityTypeName;
        this.id = id;
        this.payload = payload;
        this.getMetadata = function () {
          return {
            boundParameter: null,
            parameterTypes: {},
            operationType: 2,
            operationName: "Update",
          };
        };
      },
    };
    var confirmStrings = {
      text: "Select 'OK' to deactivate these documents or 'Cancel' to cancel and return to previous screen. \nNote: system will retain these documents as an inactive documents for 180 days.",
      title: "Confirm Deletion",
    };
    var confirmOptions = { height: 200, width: 450 };
    Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
        function (success) {
            if (success.confirmed) {
                let newDocData = {
                    "statecode": "1",
                    "ipg_reviewstatus": "427880004", // Unable to Process
                };
                var requests = [];
                selectedRecordIds.forEach((docId) => {
                    requests.push(new Sdk.UpdateRequest("ipg_document", docId, newDocData));
                });

                Xrm.WebApi.online.executeMultiple(requests).then(
                    function (success) {
                      var alertStrings = {
                        confirmButtonLabel: "Ok",
                        text: "The documents are deactivated",
                        title: "Success",
                      };
                      var alertOptions = { height: 120, width: 260 };
                      Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                        function (success) {
                          primaryControl.refresh(true);
                        }
                      );
                    },
                    function (error) {
                      console.log(error.message);
                    }
                  );
            }
        }
    );
  }

  export async function IfUserHasTeam(teamName: string) {
    var currentUserId =
      Xrm.Utility.getGlobalContext().userSettings.userId.replace(/[{|}]/g, "");
    var teams = await Xrm.WebApi.retrieveMultipleRecords(
      "team",
      "?$select=name&$expand=teammembership_association($filter=systemuserid eq " +
        currentUserId +
        ")"
    );
    if (teams && teams.entities.length > 0) {
      return teams.entities.some((team) => {
        return team.name == "IT Admin";
      });
    }
    return false;
  }
  
  export function OpenGeneratePatientStatementPage(selectedControl: Xrm.Controls.GridControl, primaryControl:Xrm.FormContext): void {
    var pageInput:Xrm.Navigation.PageInputHtmlWebResource = {
      pageType: 'webresource',
      webresourceName: 'ipg_/intake/incident/GeneratePatientStatement.html',
      data: primaryControl.data.entity.getId().replace(/[{}]/g, "")
    };
    var navigationOptions:Xrm.Navigation.NavigationOptions = {
        target: 2,
        height: 300,
        width: 600,
        position: 1,
        
    };

    Xrm.Navigation.navigateTo(pageInput, navigationOptions).then(() => {
      selectedControl.refresh();
      selectedControl.refreshRibbon();
    });
  }

  export async function EnableGeneratePSButton(selectedControl: Xrm.Controls.GridControl, primaryControl:Xrm.FormContext): Promise<boolean> {   
    return IsStatementDocumentGrid(selectedControl);
  }

  export function CloseForm(primaryControl) {
    primaryControl.ui.close();
  }

  export function AttachFromComputer(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;

    var regardingRef = {
      entityType: formContext.data.entity.getEntityName(),
      id: formContext.data.entity.getId()
    };
  
    var entityFormOptions = {};
    entityFormOptions["entityName"] = "ipg_document";
    entityFormOptions["createFromEntity"] = regardingRef;

    var formParameters = {};   

    if(formContext.data.entity.attributes.get("customertypecode")?.getValue() === CustomerTypeCodes.Facility){
      formParameters["ipg_documenttypecategoryid"] = {entityType: 'ipg_documentcategorytype', id: `${DocumentCategoryId.Facility}`, name: 'Facility'};
    }
    else if(formContext.data.entity.attributes.get("customertypecode")?.getValue() === CustomerTypeCodes.Carrier){ 
      formParameters["ipg_documenttypecategoryid"] = {entityType: 'ipg_documentcategorytype', id: `${DocumentCategoryId.Carrier}`, name: 'Carrier'};
    }
    else if(formContext.data.entity.attributes.get("customertypecode")?.getValue() === CustomerTypeCodes.Manufacturer){   
      formParameters["ipg_documenttypecategoryid"] = {entityType: 'ipg_documentcategorytype', id: `${DocumentCategoryId.Manufacturer}`, name: 'Manufacturer'};
    }
      
    window.localStorage.setItem('back', 'true');
    Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
        function (success) {
        },
        function (error) {
            console.log(error);
        }
    );
  }
  export function AttachFromDocProcessing(primaryControl, selectedDocumentsIds, params) {
    Xrm.Navigation.navigateTo({pageType: "entitylist", entityName: "ipg_document"}, {target: 2, position: 1, width: {value: 95, unit:"%"}}).then(
        function success(success) {
        },
        function error(error) {
          console.log(error);
        }
    );
  }

  async function AttachToCurrentCaseOrReferral(primaryControl, selectedDocumentsIds, params) {
    var Sdk = {
        UpdateRequest: function (entityTypeName, id, payload) {
          this.etn = entityTypeName;
          this.id = id;
          this.payload = payload;
          this.getMetadata = function () {
            return {
              boundParameter: null,
              parameterTypes: {},
              operationType: 2,
              operationName: "Update",
            };
          };
        },
    };

    if (params['id'] == null || params['undefined']) {
        console.log('Unable to get Case/Referral Id');
        return;
    }

    var newDocData;
    if (params['etn'] === 'incident') {
        newDocData = {
            "ipg_CaseId@odata.bind": "/incidents(" + params['id'] + ")",
            "ipg_reviewstatus": "427880001" // Approved
        };
    }
    else if (params['etn'] === 'ipg_referral') {
        newDocData = {
            "ipg_ReferralId@odata.bind": "/incidents(" + params['id'] + ")",
            "ipg_reviewstatus": "427880001" // Approved
        };
    }

    var fetchXml = generateFetchXmlToRetrieveDocumentsByIds(selectedDocumentsIds);
    var docs = await Xrm.WebApi.retrieveMultipleRecords(
        "ipg_document",
        fetchXml
    );

    if (docs && docs.entities.length > 0) {
        docs.entities.forEach((doc) => {
          if ((doc["_ipg_caseid_value"] != null || doc["_ipg_referralid_value"] != null)
              && doc["ipg_documenttypeid.ipg_documenttypeabbreviation"] != "MFG INV"
          ) {
            var alertStrings = {
                confirmButtonLabel: "Ok",
                text: "Document Type '" + doc["ipg_documenttypeid.ipg_name"] + "' is already associated to another Referral or Case and cannot be associated to more than one record",
                title: "Action Cancelled",
            };
            var alertOptions = { height: 120, width: 260 };
            Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                function (success) {
                },
                function (error) {
                    console.log(error.message);
                }
            );
          } else {
            Xrm.WebApi.online.execute(
                new Sdk.UpdateRequest(
                    "ipg_document",
                    doc.ipg_documentid,
                    newDocData
                )
            ).then(
                function (success) {
                  var alertStrings = {
                    confirmButtonLabel: "Ok",
                    text: "Attached successfully",
                    title: "Success",
                  };
                  var alertOptions = { height: 120, width: 260 };
                  Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                    function (success) {
                      primaryControl.refresh(true);
                    }
                  );
                },
                function (error) {
                  console.log(error.message);
                }
              );
            }
        });
    }
  }

  function getUrlParameters() {
    var queryString = parent.location.search.substring(1);
    var params = {};
    var queryStringParts = queryString.split("&");
    for (var i = 0; i < queryStringParts.length; i++) {
      var pieces = queryStringParts[i].split("=");
      params[pieces[0].toLowerCase()] = pieces.length === 1 ? null : decodeURIComponent(pieces[1]);
    }
  
    return params;
  }

  export function IsCreateForm(primaryControl) {
    var formContext: Xrm.FormContext = primaryControl;
    if (formContext.ui.getFormType() === 1 /*Create*/) {
      return false;
    }
    return true;
  }

  export function MarkApproved(primaryControl, selectedDocumentsIds) {
    let gridControl = primaryControl as Xrm.Controls.GridControl;
    var Sdk = {
        UpdateRequest: function (entityTypeName, id, payload) {
          this.etn = entityTypeName;
          this.id = id;
          this.payload = payload;
          this.getMetadata = function () {
            return {
              boundParameter: null,
              parameterTypes: {},
              operationType: 2,
              operationName: "Update",
            };
          };
        },
    };

    var newDocData = {"ipg_reviewstatus": "427880001" /*Approved*/ };
        
    selectedDocumentsIds.forEach((docId) => {
      Xrm.WebApi.online.execute(
          new Sdk.UpdateRequest(
              "ipg_document",
              docId,
              newDocData
          )
      ).then(
          function (success) {
            gridControl.refresh();
          },
          function (error) {
            console.log(error.message);
          }
        );
      }
    );
  }
}
