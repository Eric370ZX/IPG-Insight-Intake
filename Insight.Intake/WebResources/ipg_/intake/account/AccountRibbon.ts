/**
 * @namespace Intake.Account
 */
 namespace Intake.Account {
  
  const enum CustomerTypeCodes{
    Facility = 923720000,
    Carrier = 923720001,
    Manufacturer = 923720002
  }

  /**
   * Open new account form
   * @function Intake.Account.NewAccountForm
   * @returns {void}
   */
  export function NewAccountForm(PrimaryControl) {
    var viewSelector = PrimaryControl.getViewSelector();
    var viewName: string = viewSelector.getCurrentView().name.toLocaleLowerCase();
    var customertypecode: number;

    var entityFormOptions = {};
    entityFormOptions["entityName"] = "account";
    var formParameters = {};

    interface accountTypesValues {
      name: string;
      value: number;
      formId: string;
    }
    var accountTypes: accountTypesValues[] = [
      { name: 'carrier', value: 923720001, formId: 'BF7A66A7-F2ED-417D-821F-9F064E348EAD' },
      { name: 'manufacturer', value: 923720002, formId: '42DEB6BE-3ADB-4BBE-BCD2-90B46ED81AC8' },
      { name: 'facilit', value: 923720000, formId: '550FB40D-0D41-4728-989B-121EACCA2BE6' }, // not 'facilitY'!!!
      { name: 'distributor', value: 923720003, formId: '25B4ED80-3C95-4D29-8120-6384A813BA12' }
    ];

    accountTypes.forEach(function (element1) {
      if (viewName.indexOf(element1.name) >= 0) {
        var viewWithMultipleAccountTypes: boolean = false;
        accountTypes.forEach(function (element2) {
          if ((element1.name != element2.name) && (viewName.indexOf(element2.name) >= 0))
            viewWithMultipleAccountTypes = true;
        });
        if (!viewWithMultipleAccountTypes) {
          customertypecode = element1.value;
          entityFormOptions["formId"] = element1.formId;
        }
      }
    });

    if (customertypecode) {
      formParameters["customertypecode"] = customertypecode;
    }
    Xrm.Navigation.openForm(entityFormOptions, formParameters);
  }


  /**
   * Filter distributors
   * @function Intake.Account.FilterDistributors
   * @returns {void}
   */
  export function FilterDistributors(PrimaryControl, selectedEntityTypeName, selectedControl, firstPrimaryItemId) {
    const relationshipName: string = "ipg_manufacturer_distributor"
    if (selectedControl.getRelationship().name == relationshipName) {

      var options = {
        allowMultiSelect: false,
        defaultEntityType: "account",
        entityTypes: ["account"],
        defaultViewId: '1465144F-A620-E911-A979-000D3A37062B',
        disableMru: true
        //filters: [{ entityLogicalName: "account", filterXml: encodeURIComponent("<filter type='and'><condition attribute='customertypecode' operator='eq' value='923720003' /></filter>")}]
      };

      Xrm.Utility.lookupObjects(options);

    }
    //else
      //XrmCore.Commands.AddFromSubGrid.addExistingFromSubGridAssociated(selectedEntityTypeName, selectedControl);
  }

 
  /**
   * Enable rule for Add existing button
   * @function Intake.Account.HasAssociatedFacilities
   * @returns {bool}
   */
  export async function HasAssociatedFacilities(firstPrimaryItemId, selectedControl, singleAssociationAbbrs: string){    
    const document = await Xrm.WebApi.retrieveRecord(
      "ipg_document", 
      firstPrimaryItemId, 
      "?$select=_ipg_facilityid_value&$expand=ipg_DocumentTypeId($select=ipg_documenttypeabbreviation)");           
      const docTypesAbbr = singleAssociationAbbrs.split(',');
      let relatedEntitiesCount = selectedControl.getGrid().getTotalRecordCount();
    if((document["_ipg_facilityid_value"] != null || relatedEntitiesCount > 0) && docTypesAbbr.find(abbr => abbr === document.ipg_DocumentTypeId?.ipg_documenttypeabbreviation) ){
      return false;
    }
    return true;
  }

  /**
   * Enable rule for Add existing button
   * @function Intake.Account.HasAssociatedCarriers
   * @returns {bool}
   */
   export async function HasAssociatedCarriers(firstPrimaryItemId, selectedControl, multipleAssociationAbbrs: string){ 
    const document = await Xrm.WebApi.retrieveRecord(
      "ipg_document", 
      firstPrimaryItemId, 
      "?$select=_ipg_carrierid_value&$expand=ipg_DocumentTypeId($select=ipg_documenttypeabbreviation)");       
      const docTypesAbbr = multipleAssociationAbbrs.split(',');
      let relatedEntitiesCount = selectedControl.getGrid().getTotalRecordCount();
    if((document["_ipg_carrierid_value"] != null || relatedEntitiesCount > 0 ) && !docTypesAbbr.find(abbr => abbr === document.ipg_DocumentTypeId?.ipg_documenttypeabbreviation) ) {
      return false;
    }
    return true;
  }

  /**
   * Enable rule for Add existing button
   * @function Intake.Account.HasAssociatedManufacturers
   * @returns {bool}
   */
  export async function HasAssociatedManufacturers(firstPrimaryItemId, selectedControl, singleAssociationAbbrs: string){ 
    const document = await Xrm.WebApi.retrieveRecord(
      "ipg_document", 
      firstPrimaryItemId, 
      "?$select=_ipg_ipg_manufacturerid_value&$expand=ipg_DocumentTypeId($select=ipg_documenttypeabbreviation)");       
      const docTypesAbbr = singleAssociationAbbrs.split(',');
      let relatedEntitiesCount = selectedControl.getGrid().getTotalRecordCount();
    if((document["_ipg_ipg_manufacturerid_value"] != null || relatedEntitiesCount > 0 ) && docTypesAbbr.find(abbr => abbr === document.ipg_DocumentTypeId?.ipg_documenttypeabbreviation) ) {
      return false;
    }
    return true;
  }

  /**
   * Enable rule for show button on related tab
   * @function Intake.Account.ShowOnRelatedTab
   * @returns {bool}
   */
  export async function ShowOnRelatedTab(selectedControl: Xrm.Controls.GridControl, tabName: string){    
    return selectedControl["_controlName"] === tabName;
  }

  /**
   * Action for Add Existing Facility, Carrier, Manufacturer button
   * @function Intake.Account.AddExistingAccount
   * @returns {void}
   */
  export async function AddExistingAccount(firstPrimaryItemId, selectedControl: Xrm.Controls.GridControl){ 
    const tabName = selectedControl["_controlName"];
    let customerTypeCode;
    switch(tabName){
      case "Facilities": customerTypeCode = CustomerTypeCodes.Facility; break;
      case "Carriers": customerTypeCode = CustomerTypeCodes.Carrier; break;
      case "Manufacturers": customerTypeCode = CustomerTypeCodes.Manufacturer; break;
    }
    let options = {
      allowMultiSelect: true,
      defaultEntityType: "account",
      entityTypes: ["account"],
      disableMru: true,
      filters: [{
        filterXml: `<filter type='or'><condition attribute='customertypecode' operator='eq' value='${customerTypeCode}' /></filter>`,
        entityLogicalName: "account"
      }]
    };    
      
    try {
      let reference = await Xrm.Utility.lookupObjects(options);

      if (reference && reference.length) {
        Xrm.Utility.showProgressIndicator("Reassigning a document");

        try {
          let requests = [];
          reference.forEach(acc =>{
            if (acc.entityType === "account") {
              let accountId = Utility.removeCurlyBraces(acc.id);
              let documentId = Utility.removeCurlyBraces(firstPrimaryItemId);

              let associateRequest = {
                getMetadata: () => ({
                  boundParameter: null,
                  parameterTypes: {},
                  operationType: 2,
                  operationName: "Associate",
                }),

                relationship: "ipg_ipg_document_account",
                
                target: {
                  entityType: "account",
                  id: accountId,
                },       
                
                relatedEntities: [
                  {
                    entityType: "ipg_document",
                    id: documentId,
                  },
                ]
              };
              requests.push(associateRequest);
            }
          })
          Xrm.WebApi.online.executeMultiple(requests).then(
            function (success) {
              let alertStrings = {
                confirmButtonLabel: "Ok",
                text: "Adding process completed successfully",
                title: "Success",
              };
              let alertOptions = { height: 120, width: 260 };
              Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                function (success) {
                  selectedControl.refresh();
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
              e.Message || e.message || "Error during adding process",
          });
        }

        Xrm.Utility.closeProgressIndicator();
      }
    } catch (e) {
      Xrm.Navigation.openErrorDialog({
        message:
          e.Message || e.message || "Error during adding process",
      });
    }
  }

}
