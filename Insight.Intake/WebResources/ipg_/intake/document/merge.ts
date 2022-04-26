/**
 * @namespace Intake.Document
 */
namespace Intake.Document {
  //Custom formatter for the document name column
  var docLinkFormatter = function (cellvalue, options, row): string {
    let link = '';

    if (cellvalue) {
      link = '<a href="#" onclick="Intake.Document.OpenDocumentForm(\'' + row.ipg_documentid + '\')">' +
        cellvalue +
        '</a>';
    }

    return link
  }

  //Custom formatter for datetime columns
  var dateTimeFormatter = function (cellvalue, options, row): string {
    return new Date(cellvalue).toLocaleString();
  }

  export function OpenDocumentForm(docId) {
    parent.Xrm.Navigation.openForm({ entityName: "ipg_document", entityId: docId, openInNewWindow: true });
  }

  function concatDocIds(docIds): string {
    return (docIds as string[]).join();
  }

  export async function InitMergePage() {
    let data = window.location.search.split('=');
    let params = decodeURIComponent(data[1]);
    let keyValue = params.split('=');
    inputParams = JSON.parse(keyValue[1]);


    (<any>$('#docGrid')).jqGrid({
      datatype: 'local',
      height: '400px',
      autowidth: true,
      rowNum: '100',
      resizable: true,
      multiselect: true,
      colNames: ['Row ID', 'Name', 'Type', 'Date', /*'Order'*/],
      colModel: [
        { name: 'ipg_documentid', index: 'ipg_documentid', key: true, hidden: true },
        {
          name: 'ipg_name', index: 'ipg_name', width: 150,
          formatter: docLinkFormatter
        },
        { name: 'ipg_documenttypename', index: 'ipg_documenttypename', width: 150 },
        {
          name: 'modifiedon', index: 'modifiedon', width: 75, formatter: dateTimeFormatter
        },
      ],
      loadComplete: function (data) {
        if (data.records === 1) {
          (<any>$(this)).jqGrid('setSelection', data.rows[0].ipg_documentid);
        }
      },
      caption: 'Assemble Documents',
      sortname: 'Name',
      sortorder: "asc",
      viewrecords: true
    });
    (<any>jQuery("#docGrid")).jqGrid('sortableRows');
    (<any>$('#docGrid')).jqGrid('setGridParam', { data:  (await GetDocs()).entities }).trigger('reloadGrid');
    FillDocumentTypes();
  }
  function GetDocs(){
  return parent.Xrm.WebApi.retrieveMultipleRecords('ipg_document', 
      "?$select=ipg_documentid,ipg_name,modifiedon "+
      "&$expand=ipg_DocumentTypeId($select=ipg_name,ipg_documenttypeabbreviation) " +
      "&$filter="+
      `${Intake.Utility.FormatODataLogicIn('ipg_documentid',inputParams.docIds)}`);
  }
  export async function Merge() {
    let docGridVar: any = (<any>$('#docGrid'))
    const data = docGridVar.jqGrid('getGridParam', 'data');
  
    let globalContext: Xrm.GlobalContext = parent.Xrm.Utility.getGlobalContext();
    let clientUrl: string = globalContext.getClientUrl();

    let docIdsString: string = concatDocIds(inputParams.docIds);

    let postObject: any = {
      CaseId: {
        '@data.type': 'mscrm.incident',
        'incidentid': inputParams.caseId
      },
      PackageName: (<HTMLInputElement>document.getElementById('assemblyNameInput')).value,
      DocIds: docIdsString,
      TypeId: $("#docTypeSelect").val(),
      Description: $("#mergedDocumentNameInput").val()
    };

    parent.Xrm.Utility.showProgressIndicator(null);
    (<any>parent).$.ajax({
      method: "POST",
      url: clientUrl + '/api/data/v9.0/ipg_IPGIntakeActionsMergeDocuments',
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify(postObject),
      dataType: 'json',
      success: function (response): void {
        parent.Xrm.Utility.closeProgressIndicator();
        Close();
      },
      error: function (xhr, ajaxOptions, thrownError): void {
        parent.Xrm.Utility.closeProgressIndicator();
        parent.Xrm.Navigation.openErrorDialog({ message: "Could not merge the documents" }); //todo: clear validation messages from server side
      }
    });
  }
  export function Close(){
    window.close();
    parent.opener.Xrm.Page.getControl("Documents_Attached").refresh();
  }
  function deactivateDocs(docIds: string[]): void {

    let batchId = 'batch_123456';
    let changeSetId = 'changeset_BBB456';

    let data: string[] = [];
    data.push('--' + batchId);
    data.push('Content-Type: multipart/mixed;boundary=' + changeSetId);

    for (var i = 0; i < docIds.length; i++) {
      //add a request

      data.push('');
      data.push('--' + changeSetId);
      data.push('Content-Type:application/http');
      data.push('Content-Transfer-Encoding:binary');
      data.push('Content-ID:' + (i + 1));
      data.push('');
      data.push('PATCH ' + parent.Xrm.Page.context.getClientUrl() + '/api/data/v9.0/ipg_documents(' + docIds[i] + ') HTTP/1.1');
      data.push('Content-Type:application/json;type=entry');
      data.push('');
      data.push('{ "statecode":1 }');
    }

    //end of changeset
    data.push('--' + changeSetId + '--');
    //end of batch
    data.push('--' + batchId + '--');
    let payload: string = data.join('\r\n');

    parent.Xrm.Utility.showProgressIndicator(null);

    (<any>$).ajax(
      {
        type: 'POST',
        url: parent.Xrm.Page.context.getClientUrl() + '/api/data/v9.0/$batch',
        headers: {
          'Content-Type': 'multipart/mixed;boundary=' + batchId,
          'Accept': 'application/json',
          'Odata-MaxVersion': '4.0',
          'Odata-Version': '4.0'
        },

        data: payload,
        async: false,
        success: function (data, textStatus, xhr): void {
          parent.Xrm.Utility.closeProgressIndicator();
          parent.Xrm.Navigation.openAlertDialog({ text: 'The documents have been deactivated' });
        },
        error: function (xhr, data, textStatus, errorThrown) {
          parent.Xrm.Utility.closeProgressIndicator();
          parent.Xrm.Navigation.openErrorDialog({ message: textStatus + " " + errorThrown });
        }
      });
  }

  async function GetPossibleTypesForMegeDocument(){
    return (await parent.Xrm.WebApi.retrieveMultipleRecords("ipg_documenttype", "?$select=ipg_documenttypeid,ipg_name&$filter=ipg_packet eq true")).entities.sort((firstEl,secondEl)=> firstEl.ipg_name > secondEl.ipg_name ? 1: -1);
  }
  async function FillDocumentTypes(){
    const types : Array<any> = await GetPossibleTypesForMegeDocument();
    let documentTypesSelect: HTMLSelectElement = <HTMLSelectElement>document.getElementById("docTypeSelect");
    types.forEach(type => {
      let newOption: HTMLOptionElement = document.createElement("option");
      newOption.value = type.ipg_documenttypeid;
      newOption.text = type.ipg_name;
      documentTypesSelect.add(newOption);
    });
  }
  interface MergeInputParams{
    caseId: string,
    docIds : string[]
  }
  let inputParams: MergeInputParams;
}
