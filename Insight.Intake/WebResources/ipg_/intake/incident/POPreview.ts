/**
 * @namespace Intake
 */
 namespace Intake.Incident {
  const WebApi = parent.Xrm.WebApi;
  const notRequired = 'Not Required';

  class Facility {
    name: string;
    stateZip: string;
    shipingAddress: string;
    contact: Contact;
    FacilityMdd: string;
    equipmentListRcv: string;
  };
  class Contact {
    name: string;
    phone: string;
    email: string
  }
  class Header {
    constructor() {
      this.title = '';
      this.manufacturer = { name: '', number: '', mfgRep: '', suppresshipaa: false, email: '' };
      this.physician = { name: '' };
      this.facility = { name: '', stateZip: '', shipingAddress: '', equipmentListRcv: '', FacilityMdd: '', contact: { name: '', phone: '', email: '' } };
      this.patient = { name: '', firstName: '', lastName: '' };
      this.ipgAddress = { zip: '', state: '', city: '', street: '' }
    };
    title: string;
    manufacturer: { name: string, number: string, email: string, suppresshipaa: boolean, mfgRep: string };
    physician: { name: string };
    facility: Facility;
    patient: { name: string, firstName: string, lastName: string };
    poDate: string;
    surgeryDate: string;
    procedureName: string;
    ipgAddress: Intake.Utility.Address;
  }

  declare let $: typeof import("jquery");
  declare let caseId;
  declare let manufacturerId;
  declare let salesOrderId;
  declare let POType;
  declare let estimated;
  if (typeof ($) === 'undefined') {
    $ = (<any>window.parent).$;
  }

  let data = new URLSearchParams(window.location.search).get("data");
  let parameters = JSON.parse(decodeURIComponent(data));

  function RefreshParent() {
    if (window.top.opener && window.top.opener.Xrm) {
      let xrm = window.top.opener.Xrm;
      xrm.Page.ui.refreshRibbon();

      let PatientProcedureDetailsTab = xrm.Page.ui.tabs.get("PatientProcedureDetails");

      if (PatientProcedureDetailsTab) {
        let purchaseOrderProductsSection = PatientProcedureDetailsTab.sections.get("PurchaseOrderProducts");

        if (purchaseOrderProductsSection) {
          purchaseOrderProductsSection.setVisible(true);
        }
      }
    }
  }
  /**
  * PO Preview
  * @function Intake.Incident.POPreview
  * @returns {void}
  */
  export async function POPreview() {
    Xrm.Utility.showProgressIndicator("");

    caseId = parameters.caseId;
    manufacturerId = parameters.accountId;
    salesOrderId = parameters.salesOrderId;
    POType = parameters.POType;
    estimated = parameters.estimated;
    let baseUrl = "/XRMServices/2011/OrganizationData.svc/";

    const headerData = await GetHeader(baseUrl, caseId, manufacturerId, POType);


    $('#GeneratePO').prop('value', 'Generate PO ' + POType);

    let POtypeValue: string = "";
    switch (POType) {
      case "TPO":
        GenerateZPO_TPOHeader(headerData);
        POtypeValue = "923720000";
        break;
      case "ZPO":
        GenerateZPO_TPOHeader(headerData);
        POtypeValue = "923720001";
        break;
      case "CPA":
        GenerateMPOHeader(headerData);
        POtypeValue = "923720002";
        break;
      case "MPO":
        $("#communicateToRow").css("display", "table-row");
        GenerateMPOHeader(headerData);
        POtypeValue = "923720004";
        break;
    }
    let dataLines = GetDataLines(baseUrl, caseId, manufacturerId, salesOrderId, POtypeValue, estimated);

    if (dataLines && dataLines.parts && dataLines.products) {
      let modelNumbers = '';

      dataLines.parts.forEach(function (part) {
        modelNumbers += part.ipg_manufacturerpartnumber && modelNumbers.indexOf(part.ipg_manufacturerpartnumber) < 0 ? `${(modelNumbers.length > 0 ? ', ' : '')}${part.ipg_manufacturerpartnumber}` : '';
        let quantity: number = 0;
        for (var j = 0; j < dataLines.products.length; j++) {
          if (dataLines.products[j].key == part.ProductId)
            quantity += parseFloat(dataLines.products[j].value);
        }
        let markup = "<tr><td style='text-align:center'>" + ((part.Description) ? part.Description : '') + "</td><td style='text-align:center'>" + part.ipg_manufacturerpartnumber + "</td><td style='text-align:center'></td><td style='text-align:center'>" + quantity.toLocaleString('en-us', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + "</td></tr>";
        $("#parts").append(markup);
      });

      let mfgRep = $('#manufacturerRep');

      if (mfgRep) {
        mfgRep.html(modelNumbers);
      }
    }

    Xrm.Utility.closeProgressIndicator();
  }

  /**
  * type cast OData DateTime type to CRM DateTime
  * @function Intake.Incident.ToDateTime
  * @returns {Date}
  */
  function ToDateTime(dt) {
    dt = dt.replace("/Date(", "");
    dt = dt.replace(") /", "");
    return new Date(parseInt(dt, 10));
  }

  /**
  * type cast DateTime to string
  * @function Intake.Incident.ToDateTime
  * @returns {string}
  */
  function formatDate(date) {
    let d = new Date(date),
      month = '' + (d.getMonth() + 1),
      day = '' + d.getDate(),
      year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [month, day, year].join('/');
  }

  /**
  * Gets records using oData
  * @function Intake.Incident.GetRecords
  * @returns {array}
  */
  function GetRecords(url, entities) {
    $.ajax(
      {
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: url,
        async: false,
        beforeSend: function (XMLHttpRequest) {
          XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        success: function (data, textStatus, XmlHttpRequest) {
          if (data && data.d != null && data.d.results != null) {
            AddRecordsToArray(data.d.results, entities);
            FetchRecordsCallBack(data.d, entities);
          }
        },
        error: function (XmlHttpRequest, textStatus, errorThrown) {
          alert("Error :  has occurred during retrieval of the records ");
          console.log(XmlHttpRequest.responseText);
        }
      });
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
  };

  function IsValidEmail(): boolean {
    var emailStr = $("#emailaddress").val().toString();
    if (emailStr) {
      var emails = emailStr.split(';');
      return emails.every(checkEmail)
    }
    else {
      return false;
    }
  }

  function checkEmail(email) {
    if (email.trim()) {
      return validateEmail(email.trim());
    }
    else {
      return true;
    }
  }

  function validateEmail(email) {
    const re = /^(([^<>()[\]\.,;:\s@\"]+(\.[^<>()[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/i;
    return re.test(email);
  }

  /**
  * Generate PO
  * @function Intake.Incident.POPreview
  * @returns {void}
  */
  export function GeneratePO() {
    if (POType == "MPO") {
      if (!IsValidEmail() && $("#communicate-po-checkbox").prop("checked")) {
        Xrm.Navigation.openConfirmDialog({text:"Manufacturer email address is missing or invalid!"}).then((success) => {
          if(success.confirmed)
          {
            GeneratePOContinue();
          }
        });
      }
      else {
        GeneratePOContinue();
      }
      }
    else if (POType == "TPO") {
      var CPTcodes: string[] = ["29881", "65426", "29888", "65755", "67255", "65756", "19342", "66180", "29877", "65710", "29882", "65755", "11970", "29870"];
      Xrm.WebApi.retrieveRecord("incident", caseId, "?$select=title&$expand=ipg_CPTCodeId1($select=ipg_cptcode),ipg_CPTCodeId2($select=ipg_cptcode),ipg_CPTCodeId3($select=ipg_cptcode),ipg_CPTCodeId4($select=ipg_cptcode),ipg_CPTCodeId5($select=ipg_cptcode),ipg_CPTCodeId6($select=ipg_cptcode)").then(
        function success(result) {
          if (result["ipg_CPTCodeId1"] && CPTcodes.find(x => x == result["ipg_CPTCodeId1"]["ipg_cptcode"])) {
            Xrm.WebApi.retrieveMultipleRecords("ipg_document", "?$select=ipg_documentid&$filter=_ipg_caseid_value eq " + caseId + " and  _ipg_documenttypeid_value eq 305A4E30-3AEA-7905-37DE-50537821D7AE and statecode eq 0")
              .then((result) => {
                if (result.entities.length == 0) {
                  var alertStrings = { confirmButtonLabel: "OK", text: "Please add Tissue Request Form the case. Tissue Request Form is required document for TPO." };
                  var alertOptions = { height: 150, width: 300 };
                  Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                    function success(result) {
                    },
                    function (error) {
                      console.log(error.message);
                    });
                }
                else GeneratePOContinue();
              },
                function (error) {
                  Xrm.Navigation.openErrorDialog({ message: error.message });
                }
              );
          }
          else GeneratePOContinue();
        });
    }
    else GeneratePOContinue();
  }

  export function GeneratePOContinue() {

    let email = $("#emailaddress").length > 0 && (<HTMLInputElement>$("#emailaddress")[0]).value;
    const communicatePo = (<HTMLInputElement>document.getElementById("communicate-po-checkbox"))?.checked;

    let parameters = {
      entity: {
        id: caseId,
        entityType: "incident"
      },
      manufacturer: {
        id: manufacturerId,
        entityType: "account"
      },
      previousPO: {
        id: salesOrderId,
        entityType: "salesorder"
      },
      POType: POType,
      EstimatedPO: estimated
    };

    let parameterTypes = {
      "entity": {
        "typeName": "mscrm.incident",
        "structuralProperty": 5
      },
      "POType": {
        "typeName": "Edm.String",
        "structuralProperty": 1
      },
      "CommunicatePo": {
        "typeName": "Edm.boolean",
        "structuralProperty": 1
      },
      "previousPO": {
        "typeName": "mscrm.salesorder",
        "structuralProperty": 5
      },
      "manufacturer": {
        "typeName": "mscrm.account",
        "structuralProperty": 5
      },
      "EstimatedPO": {
        "typeName": "Edm.boolean",
        "structuralProperty": 1
      }
    };

    let request = {
      entity: parameters.entity,
      previousPO: parameters.previousPO,
      manufacturer: parameters.manufacturer,
      POType: parameters.POType,
      EstimatedPO: parameters.EstimatedPO,
      CommunicatePo: communicatePo,
      getMetadata: function () {
        return {
          boundParameter: "entity",
          parameterTypes: parameterTypes,
          operationType: 0,
          operationName: "ipg_IPGIntakeCaseActionsGeneratePO"
        };
      }
    };

    IsValidEmail()
    let confirmStrings = {
      text: "The system will attempt to generate POs for Manufacturers\n based on the rules defined for the Carrier, \n Manufacturer and Facility. Continue?",
      title: ""
    };

    let confirmOptions = { height: 200, width: 450 };

    Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
      function (success) {
        if (success.confirmed) {
          Xrm.Utility.showProgressIndicator(null);
          Xrm.WebApi.online.execute(request)
            .then((response) => {
              if (response.ok) {
                return response.json().then((responseData) => {
                  if (responseData.Success && responseData.poId) {
                    let  afterGenTypes =
                    {
                      "entity": {
                        "typeName": "mscrm.salesorder",
                        "structuralProperty": 5
                      },
                      "CommunicatePo": {
                        "typeName": "Edm.boolean",
                        "structuralProperty": 1
                      },
                    };
                    
                    let afterGenRequest = {
                      entity: {
                        id: responseData.poId.salesorderid,
                        entityType: "salesorder"
                      },
                      CommunicatePo: communicatePo,
                      getMetadata: function()
                      {
                        return {
                          boundParameter: "entity",
                          parameterTypes: afterGenTypes,
                          operationType: 0,
                          operationName: "ipg_IPGIntakeOrderActionsAfterGeneration"
                        };
                      }
                    }

                    if (POType == "MPO" && email)
                    {
                      afterGenTypes["CommunicateTo"] = {
                        "typeName": "Edm.String",
                        "structuralProperty": 1
                      };

                      afterGenRequest["CommunicateTo"] = email;
                    }

                    Xrm.WebApi.online.execute(afterGenRequest)
                    .then((response) =>  {
                      RefreshParent();
                      Xrm.Utility.closeProgressIndicator();
                      Xrm.Navigation.openForm({ 'entityName': 'salesorder', 'entityId': responseData.poId.salesorderid })
                    })
                    .catch((error) =>  {
                      console.log(JSON.stringify(error));
                      RefreshParent();
                      Xrm.Utility.closeProgressIndicator();
                      Xrm.Navigation.openForm({ 'entityName': 'salesorder', 'entityId': responseData.poId.salesorderid, cmdbar:true, navBar: 'on', openInNewWindow: false})
                    });
                  }
                  else {
                    Xrm.Utility.closeProgressIndicator();
                    const alertStrings = { confirmButtonLabel: "OK", text: "All Parts has quantity of 0.\n Previous POs have been voided!" };
                    Xrm.Navigation.openAlertDialog(alertStrings);
                  }
                });
              }
              else {
                Xrm.Utility.closeProgressIndicator();
                Xrm.Navigation.openErrorDialog({ message: response.statusText });
              }
            },
              (error) => {
                Xrm.Navigation.openErrorDialog({ message: error.message });
                Xrm.Utility.closeProgressIndicator();
              });
        }
      });
  }

  /**
  * Generate Header for ZPO
  * @function Intake.Incident.GenerateZPOHeader
  * @returns {void}
  */
  function GenerateZPO_TPOHeader(data: Header) {
    $('#zpoManufacturerName').html(data.manufacturer.name);
    $('#zpofacilityName').html(data.facility.name);
    $('#zpocontactName').html(data.facility.contact.name);
    $('#zposhippingAddress').html(data.facility.shipingAddress);
    $('#zpofacilityStateZip').html(data.facility.stateZip);
    $('#zpoPhone').html(data.facility.contact.phone);
    $('#zpoEmail').html(data.facility.contact.email);
    $('#zpoIPGAccountName').html("IPG ACCOUNT with " + data.manufacturer.name);
    $('#zpoIPGAccountNumber').html(data.manufacturer.number);
    $("#IPGAddress").html(data.ipgAddress.street);
    $("#IPGCityStateZip").html(`${data.ipgAddress.city}, ${data.ipgAddress.state} ${data.ipgAddress.zip}`);

    $("#zpoHeader").css("display", "table");
  }

  /**
  * Generate Header for MPO
  * @function Intake.Incident.GenerateMPOHeader
  * @returns {void}
  */
  function GenerateMPOHeader(data: Header) {
    $('#manufacturerRep').html(data.manufacturer.mfgRep);
    $('#IPGRep').html(data.facility.FacilityMdd);
    $('#equipmentListRcv').html(data.facility.equipmentListRcv);
    $('#manufacturer').html(data.manufacturer.name);
    $('#accountNumber').html(data.manufacturer.number);
    $('#title').html(data.title);
    $('#physician').html(data.physician.name);
    $('#facility').html(data.facility.name);
    $('#facilityStateZip').html(data.facility.stateZip);
    $('#patient').html(data.patient.name);
    $('#datePO').html(data.poDate);
    $('#surgeryDate').html(data.surgeryDate);
    $('#procedure').html('Procedure: ' + data.procedureName);
    $('#emailaddress').val(data.manufacturer.email);
    
    $("#mpoHeader").css("display", "table");
  }

  /**
  * Get information for PO Header
  * @function Intake.Incident.GetHeader
  * @returns Promise<Header>
  */
  async function GetHeader(baseUrl: string, caseId: string, manufacturerId: string, poType: string): Promise<Header> {
    let header = new Header();
    header.poDate = formatDate(new Date());

    const _case = GetCase(baseUrl, caseId);
    const facility = await GetFacility(baseUrl, _case.facility.Id);
    const manufacturer = await GetManufacturer(baseUrl, manufacturerId, _case.facility.Id);

    if (facility && !facility.contact) {
      facility.contact = header.facility.contact;
    }

    header.surgeryDate = _case.surgeryDate || header.surgeryDate;

    header.procedureName = _case.procedureName || header.procedureName;
    header.physician.name = _case.physician.name || header.physician.name;
    header.title = _case.title || header.title;

    header.facility.name = facility && facility.name || header.facility.name;
    header.facility.stateZip = facility && facility.stateZip || header.facility.stateZip;
    header.facility.shipingAddress = facility && facility.shipingAddress || header.facility.shipingAddress;
    header.facility.contact.name = facility && facility.contact && facility.contact.name || header.facility.contact.name;
    header.facility.contact.email = facility && facility.contact && facility.contact.email || header.facility.contact.email;
    header.facility.contact.phone = facility && facility.contact && facility.contact.phone || header.facility.contact.phone;
    header.facility.equipmentListRcv = facility && facility.equipmentListRcv || header.facility.equipmentListRcv;

    header.manufacturer.name = manufacturer && manufacturer.name || header.manufacturer.name;
    header.manufacturer.email = manufacturer && manufacturer.email || header.manufacturer.email;
    header.manufacturer.mfgRep = manufacturer && manufacturer.mfgRep || header.manufacturer.mfgRep;
    header.manufacturer.suppresshipaa = manufacturer && manufacturer.suppresshipaa || header.manufacturer.suppresshipaa;

    if (poType != "CPA") {
      if (header.manufacturer.suppresshipaa) {
        header.patient.name = "";
      }
      else if (_case.patient) {
        header.patient.name = (_case.patient.lastName.slice(0, 3) + '_' + _case.patient.firstName.slice(0, 3)).toUpperCase();
      }
      else {
        header.patient.name = (header.patient.lastName.slice(0, 3) + '_' + header.patient.firstName.slice(0, 3)).toUpperCase();
      }

      header.manufacturer.number = manufacturer && manufacturer.number || header.manufacturer.number;
    }
    else {
      header.patient.name = _case.patient && _case.patient.name || header.patient.name;
      header.manufacturer.number = notRequired;
    }

    header.ipgAddress = await Intake.Utility.GetIPGAddress(WebApi) || header.ipgAddress;

    return header;
  }

  /**
 * Gets Manufacturer Info
 * @function Intake.Incident.GetManufacturer
 * @returns {name: string, number: string}
 */
  async function GetManufacturer(baseUrl: string, manufacturerId: string, facilityId: string): Promise<{ name: string, number: string, email: string, suppresshipaa: boolean, mfgRep: string }> {
    if (manufacturerId) {
      let manufacturerRecord = [];
      let url = baseUrl
        + "AccountSet?"
        + "$select=Name,ipg_manufactureraccountnumber, ipg_manufacturerprimaryemail,ipg_manufacturerprimaryoptions, ipg_ManufacturerIsFacilityAcctRequired, ipg_ParentAccound, ipg_suppresshipaa, ipg_Ccmmunicateposto"
        + "&$filter=AccountId eq (guid'" + manufacturerId + "')";
      
      GetRecords(url, manufacturerRecord);

      if (manufacturerRecord.length > 0) {
        let mfg = manufacturerRecord[0];

        let manufacturer = {
          name: mfg.Name,
          number: '',
          email: mfg.ipg_Ccmmunicateposto,
          singlesource: mfg.ipg_manufacturerprimaryoptions,
          suppresshipaa: mfg.ipg_suppresshipaa,
          mfgRep: ''
        };


        while (mfg && mfg.ipg_ParentAccound && mfg.ipg_ParentAccound.Id) {
          const query = baseUrl
            + "AccountSet?"
            + "$select= ipg_ManufacturerIsFacilityAcctRequired"
            + "&$filter=AccountId eq (guid'" + mfg.ipg_ParentAccound.Id + "')";

          let manufacturers = [];
          GetRecords(query, manufacturers);

          mfg = manufacturers[0];
        }

        let facilityManufactureRelationShip = await Xrm.WebApi.retrieveMultipleRecords("ipg_facilitymanufacturerrelationship"
            , `?$top=1&$select=ipg_manufactureraccountnumber,ipg_manufacturerrepemail&$filter=_ipg_facilityid_value eq ${facilityId} and _ipg_manufacturerid_value eq ${manufacturerId}`);

        //manufacturer.email = manufacturer.singlesource ? manufacturer.email : (facilityManufactureRelationShip.entities.length > 0 && facilityManufactureRelationShip.entities[0].ipg_manufacturerrepemail);
        
        if (!mfg.ipg_ManufacturerIsFacilityAcctRequired) {
          manufacturer.number = notRequired;
        }
        else {
          manufacturer.number = facilityManufactureRelationShip.entities.length > 0 && facilityManufactureRelationShip.entities[0].ipg_manufactureraccountnumber || '';
        }

        return manufacturer;
      }
    }

    return null;
  }

  function GetMfgModelNumbers(baseUrl: string, manufacturerId: string): string {
    if (manufacturerId) {
      let manufacturerPartRecord = [];
      let url = `${baseUrl}ProductSet?$select=ipg_manufacturerpartnumber&$filter=ipg_manufacturerid/Id eq (guid'${manufacturerId}')`;

      GetRecords(url, manufacturerPartRecord);
      let result = '';

      manufacturerPartRecord.forEach(p => result += p.ipg_manufacturerpartnumber ? `, ${p.ipg_manufacturerpartnumber}` : '');
      return result;
    }
    return null;
  }

  /**
  * Gets Case Info
  * @function Intake.Incident.GetCase
  * @returns any
  */
  function GetCase(baseUrl: string, caseId: string): any {
    let caseRecord = [];

    const url = baseUrl
      + "IncidentSet?"
      + "$select=Title,ipg_PhysicianId,ipg_FacilityId,ipg_PatientFirstName,ipg_PatientLastName,ipg_SurgeryDate,ipg_procedureid,ipg_accurate_equipment_list_received"
      + "&$filter=IncidentId eq (guid'" + caseId + "')";

    GetRecords(url, caseRecord);

    if (caseRecord.length > 0) {
      let _case: any = {};

      if (caseRecord[0].Title) {
        _case.title = caseRecord[0].Title;
      }

      if (caseRecord[0].ipg_PhysicianId) {
        _case.physician = {};
        _case.physician.name = caseRecord[0].ipg_PhysicianId.Name;
        _case.physician.Id = caseRecord[0].ipg_PhysicianId.Id
      }

      if (caseRecord[0].ipg_FacilityId) {
        _case.facility = {};
        _case.facility.name = caseRecord[0].ipg_FacilityId.Name;
        _case.facility.Id = caseRecord[0].ipg_FacilityId.Id;
      }

      if (caseRecord[0].ipg_PatientFirstName) {
        _case.patient = {};
        _case.patient.firstName = caseRecord[0].ipg_PatientFirstName || '';
      }

      if (caseRecord[0].ipg_PatientLastName) {
        _case.patient.lastName = caseRecord[0].ipg_PatientLastName || '';
      }

      _case.patient.name = caseRecord[0].ipg_PatientFirstName + ' ' + caseRecord[0].ipg_PatientLastName;

      if (caseRecord[0].ipg_SurgeryDate) {
        _case.surgeryDate = formatDate(ToDateTime(caseRecord[0].ipg_SurgeryDate));
      }

      if (caseRecord[0].ipg_procedureid) {
        _case.procedureName = caseRecord[0].ipg_procedureid.Name;
      }

      if (caseRecord[0].ipg_accurate_equipment_list_received) {
        _case.equipmentListRcv = formatDate(ToDateTime(caseRecord[0].ipg_accurate_equipment_list_received));
      }

      return _case;
    }

    return null;
  }

  /**
 * Gets Facility Info
 * @function Intake.Incident.GetFacility
 * @returns Promise<Facility>
 */
  async function GetFacility(baseUrl: string, facilityId: string): Promise<Facility> {
    if (facilityId) {
      let facilityRecord = [];
      let Geturl = (id) => baseUrl
        + "AccountSet?"
        + "$select=Name,Address1_Line1,Address1_City,Address1_PostalCode,Address1_Telephone1,EMailAddress1,ipg_StateId,ParentAccountId,ipg_FacilityMddId,Address1_PrimaryContactName"
        + "&$filter=AccountId eq (guid'" + id + "')";

      GetRecords(Geturl(facilityId), facilityRecord);

      if (facilityRecord.length > 0) {
        if (facilityRecord[0].ParentAccountId && facilityRecord[0].ParentAccountId.Id) {
          let query = Geturl(facilityRecord[0].ParentAccountId.Id);
          facilityRecord = [];
          GetRecords(query, facilityRecord);
        }
        let facility = new Facility();
        facility.contact = new Contact();
        facility.stateZip = "";
        if (facilityRecord[0].ipg_FacilityMddId) {
          facility.FacilityMdd = facilityRecord[0].ipg_FacilityMddId.Name;
        }
        if (facilityRecord[0].Address1_City) {
          facility.stateZip = facilityRecord[0].Address1_City;
        }
        if (facilityRecord[0].ipg_StateId && facilityRecord[0].ipg_StateId.Name) {
          facility.stateZip += (facility.stateZip ? ", " : "") + facilityRecord[0].ipg_StateId.Name;
        }
        if (facilityRecord[0].Address1_PostalCode) {
          facility.stateZip += (facility.stateZip ? ", " : "") + facilityRecord[0].Address1_PostalCode;
        }

        if (facilityRecord[0].Address1_Line1) {
          facility.shipingAddress = facilityRecord[0].Address1_Line1;
        }
        if (facilityRecord[0].Name) {
          facility.name = facilityRecord[0].Name;
        }
        if (facilityRecord[0].Address1_Telephone1) {
          facility.contact.phone = facilityRecord[0].Address1_Telephone1;
        }
        if (facilityRecord[0].EMailAddress1) {
          facility.contact.email = facilityRecord[0].EMailAddress1;
        }
        if (facilityRecord[0].Address1_PrimaryContactName) {
          facility.contact.name = facilityRecord[0].Address1_PrimaryContactName;
        }
        //facility.contact = await GetMatManager(facilityId);

        return facility;
      }
    }
    return null;
  }

  /**
 * Gets Contact with role MatManager
 * @function Intake.Incident.GetMatManager
 * @returns Promise<{ name: string, phone: string, email: string }>
 */
  async function GetMatManager(accountId: string): Promise<{ name: string, phone: string, email: string }> {
    const materialCode = 923720004;
    if (accountId) {
      let contactRecord = { name: null, phone: null, email: null };

      try {
        const result = await WebApi.retrieveMultipleRecords('ipg_contactsaccounts'
          , '?$top=1&$select=ipg_contactsaccountsid&$expand=ipg_contactid($select=fullname,emailaddress1,telephone1)'
          + `&$filter=ipg_contactrolecode eq '${materialCode}' and _ipg_accountid_value eq ${accountId} and ipg_contactid / contactid ne null`);

        if (result.entities && result.entities.length >= 1) {
          contactRecord.name = result.entities[0].ipg_contactid.fullname;
          contactRecord.phone = result.entities[0].ipg_contactid.telephone1;
          contactRecord.email = result.entities[0].ipg_contactid.emailaddress1;
        }
      }
      catch (e) {
        alert("Error :  has occurred during retrieval of the records ");
        console.log(e.error);
      }

      return contactRecord;
    }
    return null;
  }

  /**
* Gets Data to display Lines
* @function Intake.Incident.GetDataLines
* @returns any
*/
  function  GetDataLines(baseUrl: string, caseId: string, manufacturerId: string, salesOrderId: string, poTypeValue: string, estimated:boolean = false): any {
    let url = baseUrl
      + "ipg_casepartdetailSet?"
      + "$select=ipg_productid,ipg_quantity"
      + "&$filter=ipg_caseid/Id eq (guid'" + caseId + "') and ipg_potypecode/Value eq " + poTypeValue
      + " and ipg_IsChanged eq true";
    if(salesOrderId){
      url += " and ipg_PurchaseOrderId/Id eq (guid'" + salesOrderId + "')";
    }
    else {
      url += " and ipg_PurchaseOrderId eq null"
    }
    let casepartdetails = [];
    let products = [];

    if (poTypeValue === "923720000" && estimated) {
      url = baseUrl
        + "ipg_estimatedcasepartdetailSet?"
        + "$select=ipg_productid,ipg_quantity"
        + "&$filter=ipg_caseid/Id eq (guid'" + caseId + "') and ipg_potypecode/Value eq " + poTypeValue;
    }

    GetRecords(url, casepartdetails);

    for (let i = 0; i < casepartdetails.length; i++) {
      //products.push(casepartdetails[i].ipg_productid.Id)
      let flag = true;
      for (var j = 0; j < products.length; j++) {
        if (products[j].key == casepartdetails[i].ipg_productid.Id) {
          flag = false;
          products[j].value += casepartdetails[i].ipg_quantity;
        }
      }
      if (flag)
        products.push({ key: casepartdetails[i].ipg_productid.Id, value: casepartdetails[i].ipg_quantity });
    }

    if (products.length) {
      url = baseUrl
        + "ProductSet?"
        + "$select=ipg_manufacturerpartnumber,Description,ProductId"
      if (manufacturerId)
        url += "&$filter=ipg_manufacturerid/Id eq (guid'" + manufacturerId + "')" + (POType != "TPO" ? " and ipg_status/Value eq 923720000" : "") + " and (";
      else
        url += "&$filter=(";
      for (let i = 0; i < products.length; i++) {
        if (i < products.length - 1)
          url += "ProductId eq (guid'" + products[i].key + "') or ";
        else
          url += "ProductId eq (guid'" + products[i].key + "'))";
      }
      let parts = [];
      GetRecords(url, parts);

      return {
        parts: parts,
        products: products
      };
    }

    return null;
  }
}
