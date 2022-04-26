/**
 * @namespace Intake
 */
namespace Intake.Incident {

  declare let $: typeof import("jquery");
  if (typeof ($) === 'undefined') {
    $ = (<any>window.parent).$;
  }

  let data = window.location.search.split('=');
  let params = decodeURIComponent(data[1]);
  let keyValue = params.split('=');
  let parameters = JSON.parse(keyValue[1]);

  /**
  * Fill PDL
  * @function Intake.Incident.FillPDL
  * @returns {void}
  */
  export function FillPDL() {
    FillIpgAddress();
    let baseUrl = "/XRMServices/2011/OrganizationData.svc/";
    let caseRecord = [];

    let url = baseUrl
      + "IncidentSet?"
      + "$select=ipg_PatientFirstName,ipg_PatientLastName,ipg_SurgeryDate,Title,ipg_PhysicianId,ipg_Facility,ipg_CPTCodeId1,ipg_DxCodeId1"
      + "&$filter=IncidentId eq (guid'" + parameters.caseId + "')";
    GetRecords(url, caseRecord);
    if (caseRecord.length > 0) {
      $('#patient').html(caseRecord[0].ipg_PatientFirstName + ' ' + caseRecord[0].ipg_PatientLastName);
      $('#surgeryDate').html(formatDate(ToDateTime(caseRecord[0].ipg_SurgeryDate)));
      $('#title').html(caseRecord[0].Title);
      $('#physician').html(caseRecord[0].ipg_PhysicianId.Name);
      $('#facility').html(caseRecord[0].ipg_Facility);
      $('#cpt').html(caseRecord[0].ipg_CPTCodeId1.Name);
      $('#dx').html(caseRecord[0].ipg_DxCodeId1.Name);
    }

    let parts = [];
    let sum = 0;
    url = baseUrl
      + "SalesOrderDetailSet?"
      + "$select=ProductId,Quantity,ExtendedAmount"
      + "&$filter=ipg_caseid/Id eq (guid'" + parameters.caseId + "')";
    GetRecords(url, parts);
    parts.forEach(function (part) {
      let partRecord = [];
      url = baseUrl
        + "ProductSet?"
        + "$select=ipg_HCPCSCodeId,ipg_manufacturerpartnumber"
        + "&$filter=ProductId eq (guid'" + part.ProductId.Id + "')";
      GetRecords(url, partRecord);
      let HCPCS = "";
      let partNumber = ""
      if (partRecord.length > 0) {
        if (partRecord[0].ipg_HCPCSCodeId.Name)
          HCPCS = partRecord[0].ipg_HCPCSCodeId.Name;
        if (partRecord[0].ipg_manufacturerpartnumber)
          partNumber = partRecord[0].ipg_manufacturerpartnumber;
      }
      let markup = "<tr><td width='15%'>" + HCPCS + "</td><td width='10%' style='text-align:center'>" + parseFloat(part.Quantity) + "</td><td width='45%'>" + part.ProductId.Name + "</td><td width='15%'>" + partNumber + "</td><td width='15%'  style='text-align:right'>$" + parseFloat(part.ExtendedAmount.Value).toLocaleString('en-us', { minimumFractionDigits: 2 }) + "</td></tr>";
      sum += parseFloat(part.ExtendedAmount.Value);
      $("#parts").append(markup);
    });
    let markup = "<tr><td colspan='4' style='text-align:right'>Total Billed</td><td style='text-align:right'>$" + sum.toLocaleString('en-us', { minimumFractionDigits: 2 }) + "</td></tr>";
    $("#parts").append(markup);
    $('#currentdate').html(formatDate(new Date()));
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

  async function FillIpgAddress() {
    const ipgAddress = await Intake.Utility.GetIPGAddress(parent.Xrm.WebApi);
    $("#IPGAddress").html(`${ipgAddress.street.replace('-', '*')} * ${ipgAddress.city}, ${ipgAddress.state} ${ipgAddress.zip}`);
  }
}
