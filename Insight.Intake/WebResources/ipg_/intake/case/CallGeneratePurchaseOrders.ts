/**
 * @namespace Intake.Case
 */
 namespace Intake.Case {
  /**
   * Called from the form of Case
   * @function Intake.Case.CallGeneratePurchaseOrdersForm
   * @returns {void}
   */
  export async function CallGeneratePurchaseOrdersForm() {
    const entityId = Intake.Utility.removeCurlyBraces(Xrm.Page.data.entity.getId());
    var casePartDetails = GetCasePartDetailsAndManufacturerbyCase(entityId);
    if (casePartDetails.length > 0) {
      for (let POtypeValue of Object.keys(POtypeEnum)) {
        console.log(POtypeEnum[POtypeValue]);
        var POtypeName = POtypeEnum[POtypeValue];
        //safeguard to isolate technical enum keys
        if (typeof (POtypeName) === "string") {
          if (POtypeName === "CPA") {
            var CPACasePartDetails = casePartDetails.filter(x => x.ipg_potypecode.Value === POtypeEnum.CPA);
            var CasePartDetailsDistinctByPreviousPO = CPACasePartDetails.filter(
              (thing, i, arr) => arr.findIndex(t => t.ipg_salesorder_ipg_casepartdetail_PurchaseOrderId?.SalesOrderId === thing.ipg_salesorder_ipg_casepartdetail_PurchaseOrderId?.SalesOrderId) === i
            );

            for (let distinctCasePartDetail of CasePartDetailsDistinctByPreviousPO) {
              await CallGeneratePurchaseOrdersAction(entityId, "", distinctCasePartDetail.ipg_salesorder_ipg_casepartdetail_PurchaseOrderId?.SalesOrderId, POtypeName, false);
            }
          }
          else {
            var POtypeNameCasePartDetails = casePartDetails.filter(x => x.ipg_potypecode.Value === POtypeEnum[POtypeName]);
            var CasePartDetailsDistinctByManufacturer = POtypeNameCasePartDetails.filter(
              (thing, i, arr) => arr.findIndex(t => t.ipg_product_ipg_casepartdetail_productid.ipg_manufacturerid.Id === thing.ipg_product_ipg_casepartdetail_productid.ipg_manufacturerid.Id && t.ipg_salesorder_ipg_casepartdetail_PurchaseOrderId?.SalesOrderId === thing.ipg_salesorder_ipg_casepartdetail_PurchaseOrderId?.SalesOrderId) === i
            );
            
            for (let distinctCasePartDetail of CasePartDetailsDistinctByManufacturer) {
              await CallGeneratePurchaseOrdersAction(entityId, distinctCasePartDetail.ipg_product_ipg_casepartdetail_productid.ipg_manufacturerid.Id, distinctCasePartDetail.ipg_salesorder_ipg_casepartdetail_PurchaseOrderId?.SalesOrderId, POtypeName, false);
            }
          }
        }
      }
    }
    else {
      alert("No CasePartDetails found for the case");
    }
  }

  export async function CallGeneratePurchaseOrdersFormTPO() {
    const entityId = Intake.Utility.removeCurlyBraces(Xrm.Page.data.entity.getId());
    var estimatedCasePartDetails = GetEstimatedCasePartDetailsAndManufacturerbyCase(entityId);

    await ConvertNotTPOTOTPO(estimatedCasePartDetails);

    if (estimatedCasePartDetails.length > 0) {
      for (let POtypeValue of Object.keys(POtypeEnum)) {
        console.log(POtypeEnum[POtypeValue]);
        var POtypeName = POtypeEnum[POtypeValue];
        if (typeof (POtypeName) === "string") {
          if (POtypeName === "TPO") {
            var POtypeNameCasePartDetails = estimatedCasePartDetails.filter(x => x.ipg_potypecode.Value === POtypeEnum[POtypeName]);
            var CasePartDetailsDistinctByManufacturer = POtypeNameCasePartDetails.filter(
              (thing, i, arr) => arr.findIndex(t => t.ipg_product_ipg_estimatedcasepartdetail_productid.ipg_manufacturerid.Id === thing.ipg_product_ipg_estimatedcasepartdetail_productid.ipg_manufacturerid.Id) === i
            );

            for (let distinctCasePartDetail of CasePartDetailsDistinctByManufacturer) {
              await CallGeneratePurchaseOrdersAction(entityId, distinctCasePartDetail.ipg_product_ipg_estimatedcasepartdetail_productid.ipg_manufacturerid.Id, "",POtypeName, true);
            }
          }
        }
      }
    }
    else {
      alert("No Estimated Case Part Details found for the case");
    }
  }

  /**
   * Called from the form of Case
   * @function Intake.Case.CallGeneratePurchaseOrdersFormChooseClick
   * @returns {void}
   */
  export function CallGeneratePurchaseOrdersFormChooseClick(commandProperties, POType) {
    const entityId = Intake.Utility.removeCurlyBraces(Xrm.Page.data.entity.getId());
    let manufacturerId = (POType == "CPA" ? "" : commandProperties.SourceControlId.split(".")[3]);
    CallGeneratePurchaseOrdersAction(entityId, manufacturerId, "", POType, false);
  }

  export function CallGeneratePurchaseOrdersFormPOClick(commandProperties) {
    let arr = commandProperties.SourceControlId.split(".");
    let menuXml = '<Menu Id="ipg.incident.Button.' + arr[arr.length - 1] + '.Menu">';
    menuXml += '<MenuSection Id="ipg.incident.Section2.Section" Sequence="5" DisplayMode="Menu16">';
    menuXml += '<Controls Id="ipg.incident.Section2.Section.Controls">';

    let baseUrl = "/XRMServices/2011/OrganizationData.svc/";
    let arr2 = commandProperties.SourceControlId.split("_");

    let POtypeValue = arr2.length == 2 ? POtypeEnum[arr2[1]] : "";


    let casepartdetails = [];
    let products = [];
    let manufacturers = [];
    const entityId = Intake.Utility.removeCurlyBraces(Xrm.Page.data.entity.getId());
    let url = baseUrl
      + "ipg_casepartdetailSet?"
      + "$select=ipg_productid"
      + "&$filter=ipg_caseid/Id eq (guid'" + entityId + "')"
      + "and ipg_productid/Id ne null "
      + " and ipg_IsChanged eq true";
    if (POtypeValue)
      url += " and ipg_potypecode/Value eq " + POtypeValue;
    GetRecords(url, casepartdetails);
    for (let i = 0; i < casepartdetails.length; i++) {
      products.push(casepartdetails[i].ipg_productid.Id)
    }
    if (products.length) {
      url = baseUrl
        + "ProductSet?"
        + "$select=ipg_manufacturerid"
        + "&$filter=";
      for (let i = 0; i < products.length; i++) {
        if (i < products.length - 1)
          url += "ProductId eq (guid'" + products[i] + "') or ";
        else
          url += "ProductId eq (guid'" + products[i] + "')";
      }
      GetRecords(url, manufacturers);

      manufacturers = manufacturers.filter((value, index, self) => self.map(m => m.Id).indexOf(value.Id) === index);

      for (let i = 0; i < manufacturers.length; i++) {
        menuXml += '<Button Alt="' + manufacturers[i].ipg_manufacturerid.Name + '" Command="incident|NoRelationship|Form|ipg.incident.Commands.' + arr[arr.length - 1] + '" CommandValueId="" Id="ipg.incident.Buttons.' + manufacturers[i].ipg_manufacturerid.Id + '" LabelText="' + manufacturers[i].ipg_manufacturerid.Name + '" Sequence="10" ToolTipTitle="' + manufacturers[i].ipg_manufacturerid.Name + '" />';
      }
    }
    menuXml += "</Controls>";
    menuXml += "</MenuSection>";
    menuXml += "</Menu>";
    commandProperties["PopulationXML"] = menuXml;
  }

  export function IsPOButtonEnabled(POType) {
    if (POType == "TPO") {
      const entityId = Intake.Utility.removeCurlyBraces(Xrm.Page.data.entity.getId());
      const tpo = GetPObyCase(POType, entityId);

      if (tpo != null) {
        return false;
      }
    }
    return true;
  }

  enum POtypeEnum {
    TPO = 923720000,
    ZPO = 923720001,
    CPA = 923720002,
    MPO = 923720004
  }

  function GetCasePartDetailsAndManufacturerbyCase(caseId): any {
    const baseUrl = "/XRMServices/2011/OrganizationData.svc/";
    const url = baseUrl + `ipg_casepartdetailSet?$select=ipg_potypecode,ipg_product_ipg_casepartdetail_productid/ipg_manufacturerid,ipg_salesorder_ipg_casepartdetail_PurchaseOrderId/SalesOrderId&$expand=ipg_product_ipg_casepartdetail_productid,ipg_salesorder_ipg_casepartdetail_PurchaseOrderId&$filter=ipg_caseid/Id eq (guid'${caseId}') and ipg_IsChanged eq true`;

    let casePartDetails = [];
    GetRecords(url, casePartDetails);
    return casePartDetails;
  }

  function GetEstimatedCasePartDetailsAndManufacturerbyCase(caseId): any {
    const baseUrl = "/XRMServices/2011/OrganizationData.svc/";
    const url = baseUrl + `ipg_estimatedcasepartdetailSet?$select=ipg_estimatedcasepartdetailId,ipg_potypecode,ipg_product_ipg_estimatedcasepartdetail_productid/ipg_manufacturerid&$expand=ipg_product_ipg_estimatedcasepartdetail_productid&$filter=ipg_caseid/Id eq (guid'${caseId}')`;

    let casePartDetails = [];
    GetRecords(url, casePartDetails);
    return casePartDetails;
  }

  function GetPObyCase(poType, caseId): any {
    const baseUrl = "/XRMServices/2011/OrganizationData.svc/";
    const url = baseUrl + `SalesOrderSet?$top=1&$select=SalesOrderId&$filter=ipg_potypecode/Value eq ${POtypeEnum[poType]} and ipg_CaseId/Id eq (guid'${caseId}')`;

    let salesOrders = [];
    GetRecords(url, salesOrders);
    if (salesOrders.length > 0) {
      return salesOrders[0];
    }

    return null;
  }

  export function IsAddCasePartDetailButtonEnabled() {

    let entityName = Xrm.Page.data.entity.getEntityName();
    let caseId = Xrm.Page.data.entity.getId();

    if (entityName === "incident") {
      const tpo = GetPObyCase("TPO", caseId);

      if (tpo != null && !IsCaseOn6Gate(caseId)) {
        return false;
      }

    }
    return true;
  }

  function IsCaseOn6Gate(caseId) {
    const baseUrl = "/XRMServices/2011/OrganizationData.svc/";
    const url = baseUrl + `IncidentSet?$top=1&$select=IncidentId,ipg_ipg_lifecyclestep_incident_ipg_lifecyclestepid/ipg_gateconfigurationid&$expand=ipg_ipg_lifecyclestep_incident_ipg_lifecyclestepid&$filter=IncidentId eq (guid'${caseId}')`;

    let cases = [];
    GetRecords(url, cases);

    if (cases.length > 0) {
      return cases[0].ipg_ipg_lifecyclestep_incident_ipg_lifecyclestepid
        && cases[0].ipg_ipg_lifecyclestep_incident_ipg_lifecyclestepid.ipg_gateconfigurationid
        && cases[0].ipg_ipg_lifecyclestep_incident_ipg_lifecyclestepid.ipg_gateconfigurationid.Name.toLowerCase() === "gate 6";
    }

    return false;
  }
  /**
  * Gets records using oData
  * @function Intake.Case.GetRecords
  * @returns {array}
  */
  function GetRecords(url, entities) {
    let xhr = new XMLHttpRequest();
    xhr.open("GET", encodeURI(url), false);
    xhr.setRequestHeader("OData-MaxVersion", "4.0");
    xhr.setRequestHeader("OData-Version", "4.0");
    xhr.setRequestHeader("Accept", "application/json");
    xhr.setRequestHeader("Content-Type", "application/json; charset=utf-8");

    xhr.onreadystatechange = function () {
      if (this.readyState === 4) {
        xhr.onreadystatechange = null;
        switch (this.status) {
          case 200: // Success with content returned in response body.
          case 204: // Success with no content returned in response body.
          case 304: // Success with Not Modified
            const data = JSON.parse(this.responseText || "{}");
            if (data && data.d && data.d.results) {
              AddRecordsToArray(data.d.results, entities);
              FetchRecordsCallBack(data.d, entities);
            }
            break;
          default: // All other statuses are error cases.
            let error;
            try {
              error = JSON.parse(xhr.response).error;
            } catch (e) {
              error = new Error("Unexpected Error");
            }
            alert("Error :  has occurred during retrieval of the records ");
            console.error(error);
            break;
        }
      }
    };

    xhr.send();
  }

  function AddRecordsToArray(records, entities) {
    for (var i = 0; i < records.length; i++) {
      entities.push(records[i]);
    }
  }

  function FetchRecordsCallBack(records, entities) {
    if (records.__next != null) {
      var url = records.__next;
      GetRecords(url, entities);
    }
  }

  async function CallGeneratePurchaseOrdersAction(entityId: string, manufacturerId: string, previousOrderId: string, POType: string, estimated: boolean) {
    if (entityId
      && IsPOValidForGeneration(entityId, manufacturerId, POType, estimated)
      && await IsManufacturerValid(entityId, manufacturerId, POType)) {

      let entity = {
        id: entityId,
        entityType: "incident"
      };
      let parameters = {
        entity: entity
      };

      let data =
      {
        caseId: entityId,
        accountId: manufacturerId,
        salesOrderId: previousOrderId,
        POType: POType,
        estimated: estimated
      };

      const myUrlWithParams = new URL(
        Xrm.Utility.getGlobalContext().getCurrentAppUrl()
      );
      myUrlWithParams.searchParams.append("pagetype", "webresource");
      myUrlWithParams.searchParams.append(
        "webresourceName",
        "ipg_/intake/incident/POPreview.html"
      );
      myUrlWithParams.searchParams.append("cmdbar", "true");
      myUrlWithParams.searchParams.append("navbar", "off");
      myUrlWithParams.searchParams.append("data", JSON.stringify(data));
      myUrlWithParams.searchParams.append("newWindow", "true");

      window.open(
        myUrlWithParams.toString(),
        "_blank"
      );
    }
  }

  function IsPOValidForGeneration(entityId: string, manufacturerId: string, POType: string, estimated: boolean): boolean {
    var baseUrl = "/XRMServices/2011/OrganizationData.svc/";
    let POs = [];
    let potypeCode = POtypeEnum[POType];

    if(potypeCode == POtypeEnum.TPO)
    {
      return true;
    }
   
      let casepartdetails = []
      let url = baseUrl
        + "ipg_casepartdetailSet?"
        + "$select=ipg_productid,ipg_product_ipg_casepartdetail_productid/StateCode,ipg_product_ipg_casepartdetail_productid/ipg_status&$expand=ipg_product_ipg_casepartdetail_productid"
        + "&$filter=ipg_caseid/Id eq (guid'" + entityId + "')"
        + " and ipg_potypecode/Value eq " + potypeCode
        + " and ipg_productid/Id ne null"
        + " and ipg_IsChanged eq true"
        + " and statecode/Value eq 0";

      GetRecords(url, casepartdetails);

      if (!casepartdetails.some(ap => ap.ipg_product_ipg_casepartdetail_productid.ipg_status
        //check that part approved and active
        && ap.ipg_product_ipg_casepartdetail_productid.ipg_status.Value === 923720000
        && ap.ipg_product_ipg_casepartdetail_productid.StateCode.Value === 0)) {
        //prevent the PO generation instead of -> alert(`${POType} doesn't have approved Parts!`);???
        return false;
      }

    return true;
  }

  async function IsManufacturerValid(entityId: string, manufacturerId: string, POType: string): Promise<boolean> {
    if (POType != POtypeEnum[POtypeEnum.CPA] && manufacturerId && entityId) {
      let entity = {
        id: entityId,
        entityType: "incident"
      };
      let manufacturerRef = {
        id: manufacturerId,
        entityType: "account"
      };
      let parameters = {
        entity: entity,
        ManufacturerId: manufacturerRef,
        Validation: "manufacturer",
      };

      let parameterTypes = {
        "entity": {
          "typeName": "mscrm.incident",
          "structuralProperty": 5
        },
        "ManufacturerId": {
          "typeName": "mscrm.account",
          "structuralProperty": 5
        },
        "Validation": {
          "typeName": "Edm.String",
          "structuralProperty": 1
        },
      }

      let request = {
        entity: parameters.entity,
        ManufacturerId: parameters.ManufacturerId,
        Validation: parameters.Validation,
        getMetadata: function () {
          return {
            boundParameter: "entity",
            parameterTypes: parameterTypes,
            operationType: 0,
            operationName: "ipg_IPGCaseActionsPreValidatePO"
          };
        }
      };

      try {
        var result = await Xrm.WebApi.online.execute(request);
      } catch (e) {
        console.log(e);
      }

      return true;
    }
    return true;
  }

  //enable rules
  export function IsLifeCycleStep(primaryControl, lifecycleStepNames: string) {
    let formContext: Xrm.FormContext = primaryControl;

    let lifecycleStep = formContext.getAttribute("ipg_lifecyclestepid").getValue();
    if (lifecycleStep && lifecycleStep.length && lifecycleStepNames.indexOf(lifecycleStep[0].name) != -1) {
      return true;
    }
    return false;
  }

  export function IsScheduledDOSGreaterOrEqualsToday(primaryControl, selectedControl) {
    let formContext: Xrm.FormContext = primaryControl;
    let today = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
    let surgeryDate = formContext.getAttribute("ipg_surgerydate").getValue();
    if (surgeryDate >= today) {
      return true;
    }
    return false;
  }

  export function IsDOSGreaterToday(primaryControl) {
    let formContext: Xrm.FormContext = primaryControl;
    let today = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
    let dos = formContext.getAttribute("ipg_actualdos")?.getValue() ?? formContext.getAttribute("ipg_surgerydate")?.getValue();
    if (dos > today) {
      return true;
    }
    return false;
  }

  

  export function IsActualDOSEmpty(primaryControl, selectedControl) {
    let formContext: Xrm.FormContext = primaryControl;
    let actualdos = formContext.getAttribute("ipg_actualdos").getValue();
    if (!actualdos) {
      return true;
    }
    return false;
  }

  var countOfEstimatedParts = null;
  export function HasCaseAtLeastOneEstimatedPart(primaryControl, selectedControl) {
    let formContext: Xrm.FormContext = primaryControl;
    let caseId = formContext.data.entity.getId().replace("{", "").replace("}", "");

    if (countOfEstimatedParts === null) {
      Xrm.WebApi.retrieveMultipleRecords("ipg_estimatedcasepartdetail", "?$select=ipg_estimatedcasepartdetailid&$filter=_ipg_caseid_value eq " + caseId).then(
        (result) => {
          countOfEstimatedParts = result.entities.length;
          selectedControl ? selectedControl.refreshRibbon() : formContext.ui.refreshRibbon();
        });
    }
    else if (countOfEstimatedParts) {
      return true;
    }
    return false;
  }

  export async function HasCaseAtLeastOneTPOEstimatedPart(primaryControl):Promise<boolean> {
    let formContext: Xrm.FormContext = primaryControl;
    let caseId = formContext.data.entity.getId().replace("{", "").replace("}", "");

    if (countOfEstimatedParts === null) {
      const estimatedParts = await Xrm.WebApi.retrieveMultipleRecords("ipg_estimatedcasepartdetail", `?$select=ipg_estimatedcasepartdetailid&$top=1&$filter=_ipg_caseid_value eq ${caseId} and ipg_potypecode eq 923720000`);
      return estimatedParts?.entities?.length > 0;
  }
}

  var countOfActualParts = null;
  export function HasCaseAtLeastOneActualPart(primaryControl, selectedControl) {
    let formContext: Xrm.FormContext = primaryControl;
    let caseId = formContext.data.entity.getId().replace("{", "").replace("}", "");

    if (countOfActualParts === null) {
      Xrm.WebApi.retrieveMultipleRecords("ipg_casepartdetail", "?$select=ipg_casepartdetailid&$filter=_ipg_caseid_value eq " + caseId).then(
        (result) => {
          countOfActualParts = result.entities.length;
          selectedControl ? selectedControl.refreshRibbon() : formContext.ui.refreshRibbon();
        });
    }
    else if (countOfActualParts) {
      return true;
    }
    return false;
  }

  var countOfOpenedTissueRequestForm = null;
  export function HasOpenedTissueRequestForm(primaryControl, selectedControl) {
    let formContext: Xrm.FormContext = primaryControl;
    let caseId = formContext.data.entity.getId().replace("{", "").replace("}", "");

    if (countOfOpenedTissueRequestForm === null) {
      Xrm.WebApi.retrieveMultipleRecords("task", "?$select=activityid&$filter=ipg_tasktypecode eq 427880048 and  statecode eq 0 and  _regardingobjectid_value eq " + caseId).then(
        (result) => {
          countOfOpenedTissueRequestForm = result.entities.length;
          selectedControl ? selectedControl.refreshRibbon() : formContext.ui.refreshRibbon();
        });
    }
    else if (countOfOpenedTissueRequestForm) {
      return true;
    }
    return false;
  }

  export function IsAllReceived(entityId) {
    let allReceived = null;

    entityId = Utility.removeCurlyBraces(entityId);

    let req = new XMLHttpRequest();
    req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/incidents(" + entityId + ")?$select=ipg_isallreceived", false);
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
    req.onreadystatechange = function () {
      if (this.readyState === 4) {
        const result = JSON.parse(this.response);
        req.onreadystatechange = null;

        if (this.status === 200) {
          allReceived = result["ipg_isallreceived"];
        } else {
          const errorMessage: string = result.error && result.error.message || "Error during checking is all received property";
          Xrm.Utility.alertDialog(errorMessage, null);
        }
      }
    };
    req.send();
    return allReceived;
  }


  export function IsActualDOSLessThanToday(primaryControl, selectedControl) {
    let formContext: Xrm.FormContext = primaryControl;
    let today = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
    let actualdos = formContext.getAttribute("ipg_actualdos").getValue();
    if (actualdos && actualdos < today) {
      return true;
    }
    return false;
  }

  async function ConvertNotTPOTOTPO(EstimatedParts:any[]) {
    var notTPOPArts = EstimatedParts.filter(part => part.ipg_potypecode?.Value !== POtypeEnum.TPO);
    for (let notTPO of notTPOPArts)
    {
      await Xrm.WebApi.updateRecord("ipg_estimatedcasepartdetail", notTPO.ipg_estimatedcasepartdetailId, {"ipg_potypecode": Number(POtypeEnum.TPO)});
      notTPO.ipg_potypecode.Value = Number(POtypeEnum.TPO);
    }
  }
}