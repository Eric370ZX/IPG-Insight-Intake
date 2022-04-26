/**
 * @namespace Intake.Document
 */
 namespace Intake.Document {

  const enum DocumentCategoryId{
    PatientProcedure = "db8c208e-c02f-72f1-47db-48a0a28a0fe9",
    Claim = "979a4281-dbeb-eb11-bacb-000d3a5aaa66",
    PurchaseOrder = "63187393-dbeb-eb11-bacb-000d3a5aaa66",
    PatientStatement = "10464d75-dbeb-eb11-bacb-000d3a5aaa66"  
  }

  declare let $: typeof import("jquery");
  if (typeof ($) === 'undefined') {
    $ = (<any>window.parent).$;
  }

  /**
   * Called on load form
   * @function Intake.Document.OnLoadForm
   * @returns {void}
  */
  export function OnLoadForm(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    let formType = formContext.ui.getFormType();   
    if (formType !== XrmEnum.FormType.Create) {
      
      if (formContext.getAttribute("ipg_name").getValue()) {
        formContext.getControl("ipg_name").setDisabled(true);
      }

      if (formContext.getAttribute("ipg_source").getValue()) {
        formContext.getControl("ipg_source").setDisabled(true);
      }

      if (formContext.getAttribute("createdon").getValue()) {
        formContext.getControl("createdon").setDisabled(true);
      }

      formContext.getAttribute("ipg_documenttypecategoryid").fireOnChange();

      ShowHideDocumentCategoryTypes(formContext);
    }
    else {
      formContext.getControl('WebResource_DocumentFile').setVisible(false);
      let documentBody = formContext.getAttribute("ipg_documentbody");
      if (documentBody) {
        documentBody.setRequiredLevel("required");
        formContext.getControl("ipg_documentbody").setVisible(true);
      }
      formContext.getAttribute("ipg_source").setValue(923720002);

      formContext.ui.tabs.get("Document").sections.get("Document_section_4").setVisible(false);
      formContext.ui.tabs.get("Document").sections.get("Document_section_2").setVisible(false);

      formContext.getAttribute("ipg_filterdocumenttypes").setValue(false);
      if (formContext.getAttribute("ipg_facilityid").getValue() != null
      || formContext.getAttribute("ipg_carrierid").getValue() != null
      || formContext.getAttribute("ipg_ipg_manufacturerid").getValue() != null) {
        formContext.getAttribute("ipg_documenttypecategoryid").fireOnChange();
      }
      else if(formContext.getAttribute("ipg_caseid").getValue() != null
      || formContext.getAttribute("ipg_referralid").getValue() != null) {
        function addCustomLookupFilter(formContext: Xrm.FormContext) {
          const filters: Array<string> = [];          
          filters.push(`<condition attribute="ipg_documentcategorytypeid" operator="eq" value="${DocumentCategoryId.PatientProcedure}" />`);
          filters.push(`<condition attribute="ipg_documentcategorytypeid" operator="eq" value="${DocumentCategoryId.Claim}" />`);
          filters.push(`<condition attribute="ipg_documentcategorytypeid" operator="eq" value="${DocumentCategoryId.PurchaseOrder}" />`); 
          filters.push(`<condition attribute="ipg_documentcategorytypeid" operator="eq" value="${DocumentCategoryId.PatientStatement}" />`); 
          let filterXml = `<filter type="or">${filters.join('')}</filter>`;
          formContext.getControl("ipg_documenttypeid").addCustomFilter(filterXml, "ipg_documenttype");
        }    
        let filterFunction = function () {
          addCustomLookupFilter(formContext);
        };
        formContext.getControl("ipg_documenttypeid").addPreSearch(filterFunction);
      }
    }

    ChangeNameCreatedByModifiedBy(formContext);

    showHideEbvResponse(formContext);

    checkDocType(formContext);

    let documentTypeFilterConditions = "";
    if (formType !== XrmEnum.FormType.Create) {        
      documentTypeFilterConditions ="<condition attribute='ipg_name' value='%Generic%' operator='not-like'/>"+
      "<condition attribute='ipg_name' value='%Fax%' operator='not-like'/>";
    }
    else {
      documentTypeFilterConditions = "<condition attribute='ipg_name' value='%Portal%' operator='not-like'/>"+
      "<condition attribute='ipg_name' value='%Fax%' operator='not-like'/>";           
    }
    formContext.getControl("ipg_documenttypeid").addPreSearch(function () {
      addDocumentTypeFilter(formContext, documentTypeFilterConditions);
    });
    function addDocumentTypeFilter(formContext: Xrm.FormContext, documentTypeFilter : string) {   
      let filterXML = `<filter type='and'>${documentTypeFilter}</filter>`; 
      formContext.getControl("ipg_documenttypeid").addCustomFilter(filterXML, "ipg_documenttype");
    }

    if (formType !== XrmEnum.FormType.Create) {
      OpenRelatedCaseOrReferral(executionContext);
      //ShowAccountsTabByCategory(formContext);
    }
    
  }

  // Maybe it will be use in the future
  // /**
  //  * Show Accounts Tab By Category
  //  * @function Intake.Document.ShowAccountsTabByCategory
  //  * @returns {void}
  // */
  // function ShowAccountsTabByCategory(formContext: Xrm.FormContext) {
  //   let category = formContext.getAttribute("ipg_documenttypecategoryid").getValue();
  //   if(category == null)
  //     return;
  //   let categoryName = category[0].name;
  //   formContext.ui.tabs.get(categoryName + "Tab")?.setVisible(true);        
  // }

  /**
   * on change document type
   * @function Intake.Document.OnChangeDocumentType
   * @returns {void}
  */
  export function OnChangeDocumentType(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    const lookUp = executionContext.getEventSource() as Xrm.Attributes.LookupAttribute;
    formContext.ui.refreshRibbon();
    const DocumentType = lookUp.getValue();
    if (DocumentType != null && DocumentType.length > 0) {
      Xrm.WebApi.retrieveRecord(DocumentType[0].entityType, DocumentType[0].id, "?$select=_ipg_documentcategorytypeid_value")
        .then(response => {
          if (response._ipg_documentcategorytypeid_value) {
            const docCategory = formContext.getAttribute<Xrm.Attributes.LookupAttribute>("ipg_documenttypecategoryid");
            const lookUp: Xrm.LookupValue =
            {
              id: response["_ipg_documentcategorytypeid_value"]
              , name: response["_ipg_documentcategorytypeid_value@OData.Community.Display.V1.FormattedValue"]
              , entityType: response["_ipg_documentcategorytypeid_value@Microsoft.Dynamics.CRM.lookuplogicalname"]
            };

            docCategory && docCategory.setValue([lookUp]);
            if(formContext.ui.getFormType() !== XrmEnum.FormType.Create){
              formContext.getAttribute("ipg_documenttypecategoryid").fireOnChange();
            }
            if (response["_ipg_documentcategorytypeid_value@OData.Community.Display.V1.FormattedValue"].toLowerCase() == "carrier") {

              formContext.getControl("ipg_carrierid").setVisible(true);
              formContext.getControl("ipg_facilityid").setVisible(false);
              formContext.getControl("ipg_ipg_manufacturerid").setVisible(false);

              formContext.getAttribute("ipg_carrierid").setRequiredLevel("required");
              formContext.getAttribute("ipg_facilityid").setRequiredLevel("none");
              formContext.getAttribute("ipg_ipg_manufacturerid").setRequiredLevel("none");
            }
            else
              if (response["_ipg_documentcategorytypeid_value@OData.Community.Display.V1.FormattedValue"].toLowerCase() == "facility") {
                formContext.getControl("ipg_carrierid").setVisible(false);
                formContext.getControl("ipg_facilityid").setVisible(true);
                formContext.getControl("ipg_ipg_manufacturerid").setVisible(false);

                formContext.getAttribute("ipg_carrierid").setRequiredLevel("none");
                formContext.getAttribute("ipg_facilityid").setRequiredLevel("required");
                formContext.getAttribute("ipg_ipg_manufacturerid").setRequiredLevel("none");
              }
              else
                if (response["_ipg_documentcategorytypeid_value@OData.Community.Display.V1.FormattedValue"].toLowerCase() == "manufacturer") {

                  formContext.getControl("ipg_carrierid").setVisible(false);
                  formContext.getControl("ipg_facilityid").setVisible(false);
                  formContext.getControl("ipg_ipg_manufacturerid").setVisible(true);

                  formContext.getAttribute("ipg_carrierid").setRequiredLevel("none");
                  formContext.getAttribute("ipg_facilityid").setRequiredLevel("none");
                  formContext.getAttribute("ipg_ipg_manufacturerid").setRequiredLevel("required");
                }
                else {
                  formContext.getControl("ipg_carrierid").setVisible(false);
                  formContext.getControl("ipg_facilityid").setVisible(false);
                  formContext.getControl("ipg_ipg_manufacturerid").setVisible(false);

                  formContext.getAttribute("ipg_carrierid").setRequiredLevel("none");
                  formContext.getAttribute("ipg_facilityid").setRequiredLevel("none");
                  formContext.getAttribute("ipg_ipg_manufacturerid").setRequiredLevel("none");
                }

          }
        })
        .catch(error => {
          console.log(error)
        });
    }

    showHideEbvResponse(formContext);
    checkDocType(formContext);
  }

  /**
   * Called on save form
   * @function Intake.Document.OnSaveForm
   * @returns {void}
  */

  var preventSave = true;
  export function OnSaveForm(executionContext: any) {
    let formContext = executionContext.getFormContext();


    var docType = formContext.getAttribute("ipg_documenttypeid").getValue();
    var facility = formContext.getAttribute("ipg_facilityid").getValue();
    if (formContext.data.entity.getIsDirty() === false) {
      return;
    }
    //let saveEventArgs: Xrm.Events.SaveEventArguments = executionContext.getEventArgs();
    //saveEventArgs.preventDefault();

    checkForChangesAndReopenCase(executionContext);

    if (formContext.ui.getFormType() === 1
      && formContext.getAttribute("ipg_reviewstatus")) {
      formContext.getAttribute("ipg_reviewstatus").setValue(427880001);
    }

    if (docType) {
      let docName = docType[0].name;
      formContext.getAttribute("ipg_textdocumenttype").setValue(docName);
    }

    if (facility) {
      let facName = facility[0].name;
      formContext.getAttribute("ipg_textfacilityname").setValue(facName);
    }
   
  }


  function SetDocumentCategory(formContext: Xrm.FormContext) {
    let carrier = formContext.getAttribute("ipg_carrierid")?.getValue();
    if (carrier) {
      Xrm.WebApi.retrieveMultipleRecords("ipg_documentcategorytype", "?$select=ipg_name, ipg_documentcategorytypeid&$filter=ipg_name eq 'Carrier'").then(
        function success(results) {
          if (results.entities.length > 0) {
            let object = new Array();
            object[0] = new Object();
            object[0].id = results.entities[0]["ipg_documentcategorytypeid"];
            object[0].name = results.entities[0]["ipg_name"];
            object[0].entityType = "ipg_documentcategorytype";
            let documentCategoryAttr = formContext.getAttribute("ipg_documenttypecategoryid");
            documentCategoryAttr.setValue(object);
            documentCategoryAttr.fireOnChange();
            formContext.getControl("ipg_documenttypecategoryid").setDisabled(true);

          }
        },
        function (error) {
        }
      );
    }
  }



  //function Intake.Document.refreshCrmForm
  export function refreshCrmForm(executionContext: Xrm.Events.EventContext) {
    let formContext = executionContext.getFormContext();
    setTimeout(function () {

      var windowOptions = {
        openInNewWindow: false,
        entityName: formContext.data.entity.getEntityName(),
        entityId: formContext.data.entity.getId()
      };
      Xrm.Navigation.openForm(windowOptions);
    }, 1000);
  }


  /**
   * on change document type category
   * @function Intake.Document.OnChangeDocumentCategoryType
   * @returns {void}
  */
  export function OnChangeDocumentCategoryType(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    //formContext.getAttribute("ipg_documenttypeid").setValue(null);

    ShowHideDocumentCategoryTypes(formContext);

    addFiltersToDocumentType(formContext);
  }

  /**
   * add filters to doucument type field
   * @function Intake.Referral.OnChangePatientDOB
   * @returns {void}
  */
  function addFiltersToDocumentType(formContext: Xrm.FormContext) {
    function addCustomLookupFilter(formContext: Xrm.FormContext) {
      const filters: Array<string> = [];
      let documentCategoryType: Xrm.LookupValue[] = formContext.getAttribute("ipg_documenttypecategoryid").getValue();
      if (documentCategoryType) {
        filters.push(`<condition attribute="ipg_documentcategorytypeid" operator="eq" value="${Intake.Utility.removeCurlyBraces(documentCategoryType[0].id)}" />`);
      }
      let filterXml = `<filter type="and">${filters.join('')}</filter>`;
      formContext.getControl("ipg_documenttypeid").addCustomFilter(filterXml, "ipg_documenttype");
    }

    let filterFunction = function () {
      addCustomLookupFilter(formContext);
    };
    let documentCategoryType: Xrm.LookupValue[] = formContext.getAttribute("ipg_documenttypecategoryid").getValue();
    if (documentCategoryType) {
      formContext.getControl("ipg_documenttypeid").addPreSearch(filterFunction);
    } else {
      formContext.getControl("ipg_documenttypeid").removePreSearch(filterFunction);
    }
  }

  function ChangeNameCreatedByModifiedBy(formContext: Xrm.FormContext) {
    const createdByAttr = formContext.getAttribute<Xrm.Attributes.LookupAttribute>("createdby");
    const modifiedByAttr = formContext.getAttribute<Xrm.Attributes.LookupAttribute>("modifiedby");
    const sourceAttr = formContext.getAttribute<Xrm.Attributes.OptionSetAttribute>("ipg_source");
    const sourceVal = sourceAttr.getText();

    switch (sourceVal) {
      case "Portal":
      case "Fax":
        const createdByVal = createdByAttr.getValue();
        const modifiedByVal = modifiedByAttr.getValue();
        createdByVal[0].name = sourceVal;
        createdByAttr.setValue(createdByVal);

        if (modifiedByVal[0].id === createdByVal[0].id) modifiedByAttr.setValue(createdByVal);
        break;
      default:
    }
  }

  export function populateFileInfo() {
    $('#file-info').empty();
    let documentId = Intake.Utility.removeCurlyBraces(parent.Xrm.Page.data.entity.getId());
    if (documentId) {
      parent.Xrm.WebApi.retrieveMultipleRecords('annotation', `?$select=annotationid,filename&$filter=_objectid_value eq ${documentId}`).then(
        function success(results) {
          if (results.entities.length) {
            $('#file-info').append(`<label id='${results.entities[0]['annotationid']}'><a href='#' class='attachment'>${results.entities[0]['filename']}</a></label>&nbsp;<button class='file-delete'>X</button>`);
          }
        },
        function (error) {
          parent.Xrm.Utility.alertDialog(error.message, null);
        }
      );
    }
  }

  export function onFileClick(e, node) {
    e.preventDefault();
    let annotationId = $(node).closest('label').attr('id');
    let URL = `${parent.Xrm.Page.context.getClientUrl()}/userdefined/edit.aspx?etc=5&id=${annotationId}`;
    $.get(URL, function (data) {
      var WRPCTokenElement = $(data).find('[WRPCTokenUrl]');
      if (WRPCTokenElement) {
        var WRPCTokenUrl = WRPCTokenElement.attr('WRPCTokenUrl');
        if (WRPCTokenUrl) {
          URL = `${parent.Xrm.Page.context.getClientUrl()}/Activities/Attachment/download.aspx?AttachmentType=5&AttachmentId=${annotationId}&IsNotesTabAttachment=undefined${WRPCTokenUrl}`;
          window.open(URL);
        }
      }
      return false;
    });
  }

  export function onFileDelete(node) {
    if (confirm('Are you sure you want to delete this File?')) {
      let annotationId = $(node).parent().find('label').attr('id');
      parent.Xrm.WebApi.deleteRecord('annotation', annotationId).then(
        function success(result) {
          parent.Xrm.Page.getAttribute('ipg_filename').setValue(null);
          parent.Xrm.Page.getAttribute('ipg_name').setValue(null);
          parent.Xrm.Page.getAttribute('ipg_documentbody').setValue(null);
          parent.Xrm.Page.getControl('ipg_documentbody').setVisible(true);
          parent.Xrm.Page.getControl('WebResource_DocumentFile').setVisible(false);
        },
        function (error) {
          parent.Xrm.Utility.alertDialog(error.message, null);
        }
      );
    }
  }

  function ShowHideDocumentCategoryTypes(formContext: Xrm.FormContext) {

    let documentCategoryType: Xrm.LookupValue[] = formContext.getAttribute("ipg_documenttypecategoryid").getValue();
    if (documentCategoryType) {

      if (documentCategoryType[0].name.toLowerCase() == "carrier") {

        formContext.getControl("ipg_carrierid").setVisible(true);
        formContext.getControl("ipg_facilityid").setVisible(false);
        formContext.getControl("ipg_ipg_manufacturerid").setVisible(false);

        formContext.getAttribute("ipg_carrierid").setRequiredLevel("required");
        formContext.getAttribute("ipg_facilityid").setRequiredLevel("none");
        formContext.getAttribute("ipg_ipg_manufacturerid").setRequiredLevel("none");
      }
      else
        if (documentCategoryType[0].name.toLowerCase() == "facility") {
          formContext.getControl("ipg_carrierid").setVisible(false);
          formContext.getControl("ipg_facilityid").setVisible(true);
          formContext.getControl("ipg_ipg_manufacturerid").setVisible(false);

          formContext.getAttribute("ipg_carrierid").setRequiredLevel("none");
          formContext.getAttribute("ipg_facilityid").setRequiredLevel("required");
          formContext.getAttribute("ipg_ipg_manufacturerid").setRequiredLevel("none");
        }
        else
          if (documentCategoryType[0].name.toLowerCase() == "manufacturer") {

            formContext.getControl("ipg_carrierid").setVisible(false);
            formContext.getControl("ipg_facilityid").setVisible(false);
            formContext.getControl("ipg_ipg_manufacturerid").setVisible(true);

            formContext.getAttribute("ipg_carrierid").setRequiredLevel("none");
            formContext.getAttribute("ipg_facilityid").setRequiredLevel("none");
            formContext.getAttribute("ipg_ipg_manufacturerid").setRequiredLevel("required");
          }
          else {
            formContext.getControl("ipg_carrierid").setVisible(false);
            formContext.getControl("ipg_facilityid").setVisible(false);
            formContext.getControl("ipg_ipg_manufacturerid").setVisible(false);

            formContext.getAttribute("ipg_carrierid").setRequiredLevel("none");
            formContext.getAttribute("ipg_facilityid").setRequiredLevel("none");
            formContext.getAttribute("ipg_ipg_manufacturerid").setRequiredLevel("none");
          }
    }
    else {
      formContext.getControl("ipg_carrierid").setVisible(false);
      formContext.getControl("ipg_facilityid").setVisible(false);
      formContext.getControl("ipg_ipg_manufacturerid").setVisible(false);
    }
  }

  async function showHideEbvResponse(formContext: Xrm.FormContext) {
    let docTypeAttribute = formContext.getAttribute('ipg_documenttypeid');
    let ebvResponseControl = formContext.getControl("ipg_ebvresponseid");
    if (docTypeAttribute && ebvResponseControl) {
      let visible: boolean = false;

      let docTypeValue = docTypeAttribute.getValue()
      if (docTypeValue && docTypeValue.length) {

        let docType = await Xrm.WebApi.retrieveRecord('ipg_documenttype', docTypeValue[0].id, '?$select=ipg_documenttypeabbreviation');
        if (docType.ipg_documenttypeabbreviation == 'EBV') {
          visible = true;
        }
      }

      ebvResponseControl.setVisible(visible);
    }
  }

  function checkForChangesAndReopenCase(executionContext: any) {

    let formContext: Xrm.FormContext = executionContext.getFormContext();

    if (formContext.getAttribute("ipg_caseid")) {
      let CaseId: Xrm.LookupValue[] = formContext.getAttribute("ipg_caseid").getValue();
      if (CaseId) {
        if (preventSave) {
          executionContext.getEventArgs().preventDefault();
        }

        Xrm.WebApi.retrieveRecord('incident', CaseId[0].id, "?$select=ipg_casestatus")
          .then(response => {
            if (!preventSave) {
              return;
            }
            preventSave = false;
            var CaseStatus = response["ipg_casestatus@OData.Community.Display.V1.FormattedValue"];

            if (CaseStatus == 'Closed') {
              var confirmStrings = { text: "This case is currently closed. Do you want to reopen?", title: "Confirm", confirmButtonLabel: "Yes - Reopen", cancelButtonLabel: "No - Leave closed" };
              Xrm.Navigation.openConfirmDialog(confirmStrings, null).then(
                function (success) {
                  if (success.confirmed) {
                    console.log("Yes - Reopen");
                    formContext.data.save();
                  }
                  else {

                    console.log("No - Leave closed");
                    formContext.data.save();
                  }
                });
            }
            else {
              formContext.data.save();
            }
          })
          .catch(error => {
            console.log(error)
          });
      }
    }

  }

  async function checkDocType(formContext: Xrm.FormContext) {
    let docTypeAttribute = formContext.getAttribute('ipg_documenttypeid');
    if (docTypeAttribute && docTypeAttribute.getValue() && docTypeAttribute.getValue().length) {
      let docTypeValue = docTypeAttribute.getValue();
      let relatedCaseControl = formContext.getControl("ipg_caseid");
      if (docTypeValue[0].name === "User Uploaded Generic Document") {
        if (relatedCaseControl) {
          formContext.getControl("ipg_caseid").setDisabled(true);
        }
      }
      else if (docTypeValue[0].name === "Fax") {
        if (relatedCaseControl) {
          formContext.getControl("ipg_caseid").setDisabled(true);
        }
      } else {
        if (relatedCaseControl) {
          formContext.getControl("ipg_caseid").setDisabled(false);
        }
      }
    }
  }

  /**
   * Called on 'Case Source' field change
   * @function Intake.Document.OnCaseSourceChange
   * @returns {void}
  */
  export function OnCaseSourceChange(executionContext: Xrm.Events.EventContext) {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    formContext.ui.clearFormNotification("case");
    let incident = formContext.getAttribute("ipg_caseid").getValue();
    let caseSource = formContext.getAttribute("ipg_casesourceid").getValue();
    if (incident && caseSource) {
      if (incident[0].id == caseSource[0].id) {
        formContext.ui.setFormNotification("Please, select a document from an another case", "WARNING", "case");
      }
    }
  }

   export function OpenRelatedCaseOrReferral(executionContext: Xrm.Events.EventContext)
   {
    let formContext: Xrm.FormContext = executionContext.getFormContext();
    let formType = formContext.ui.getFormType();

    if (formType === XrmEnum.FormType.Create) { return; }
    var back = window.localStorage.getItem('back');
    if (back !== 'true') { return; }
    
    var relatedCase = formContext.data.entity.attributes.get("ipg_caseid").getValue();
    var relatedReferral = formContext.data.entity.attributes.get("ipg_referralid").getValue();

    if (relatedCase != null && relatedCase.length > 0) {
        var regardingEntityName = relatedCase[0].entityType;
        var regardingId = relatedCase[0].id;
    }
    else if (relatedReferral != null && relatedReferral.length > 0) {
        var regardingEntityName = relatedReferral[0].entityType;
        var regardingId = relatedReferral[0].id;
    }
    else { return; }

    var entityFormOptions = {};
    entityFormOptions["entityName"] = regardingEntityName;
    entityFormOptions["entityId"] = regardingId;
   

    Xrm.Navigation.openForm(entityFormOptions).then(
        function (success) {
            console.log(success);
            window.localStorage.setItem('focusOnTab', 'Documents');
            if (formContext.data.entity.getEntityName() != 'ipg_document') {
              window.localStorage.removeItem('back');
            }
        },
        function (error) {
            console.log(error);
            window.localStorage.removeItem('back');
        }
    );
   }
   export function SetReviewStatus(formContext) {
   
     let categoryType = formContext.getAttribute("ipg_documenttypecategoryid").getValue();
     formContext.getAttribute("ipg_reviewstatus").setValue(null);
     if (categoryType)
     {
     let name = categoryType[0].name;
       switch (name) {
         case "Carrier":
           formContext.getAttribute("ipg_reviewstatus").setValue(427880001);
           break;
         case "Facility":
           formContext.getAttribute("ipg_reviewstatus").setValue(427880001);
           break;
         case "Manufacturer":
           formContext.getAttribute("ipg_reviewstatus").setValue(427880001);
           break;
       }
     }
   }

}
