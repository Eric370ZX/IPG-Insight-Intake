/**
 * @namespace Intake.Contact
 */
namespace Intake.Contact {
    declare let $: typeof import("jquery");
    class Document { ipg_documentid: string; ipg_name: string; ipg_documenttypename: string; createdon: string; }

    if (typeof ($) === 'undefined') {
        $ = (<any>window.parent).$;
    }
    const GRID_ID = '#documentsGrid';
    const Xrm = window.parent.Xrm;
    //for DocumentRibbon script
    window.Xrm = Xrm;
    const PATIENT_ID = Xrm.Page.data.entity.getId();
    const DOCS_CASE_FETCH = `<fetch>
  <entity name="ipg_document" >
    <attribute name="createdon" />
    <attribute name="ipg_documentid" />
    <attribute name="ipg_documenttypeid" />
    <attribute name="ipg_name" />
    <filter type="or" >
      <condition entityname="referral" attribute="ipg_patientid" operator="eq" value="${PATIENT_ID}" />
    </filter>
    <link-entity name="ipg_referral" from="ipg_referralid" to="ipg_referralid" link-type="inner" alias="referral" />
  </entity>
</fetch>`;
    const DOCS_REFERRALS_FETCH = `<fetch>
  <entity name="ipg_document" >
    <attribute name="createdon" />
    <attribute name="ipg_documentid" />
    <attribute name="ipg_documenttypeid" />
    <attribute name="ipg_name" />
    <filter type="or" >
      <condition entityname="case" attribute="ipg_patientid" operator="eq" value="${PATIENT_ID}" />
    </filter>
    <link-entity name="incident" from="incidentid" to="ipg_caseid" link-type="inner" alias="case" />
  </entity>
</fetch>`;

    const docLinkFormatter = function (cellvalue, options, row): string {
        let link = '';

        if (cellvalue) {
            link = '<a href="#" onclick="Intake.Contact.OpenDocumentForm(\'' + row.ipg_documentid + '\')">' +
                cellvalue +
                '</a>';
        }

        return link
    }

    const dateTimeFormatter = function (cellvalue, options, row): string {
        return new Date(cellvalue).toLocaleString();
    }

    export function OpenDocumentForm(docId) {
        Xrm.Navigation.openForm({ entityName: "ipg_document", entityId: docId, openInNewWindow: true });
    }

    function GetRowsFromResponse(response: Xrm.RetrieveMultipleResult, rows: Document[]) {
        for (var i = 0; i < response.entities.length; i++) {
            var newDoc: Document =
            {
                ipg_documentid: response.entities[i].ipg_documentid,
                ipg_name: response.entities[i].ipg_name,
                ipg_documenttypename: response.entities[i]["_ipg_documenttypeid_value@OData.Community.Display.V1.FormattedValue"] || "",
                createdon: response.entities[i].createdon
            };

            if (!rows.find(r => r.ipg_documentid === newDoc.ipg_documentid)) {
                rows.push(newDoc);
            }
        }
        return rows;
    }
    //Fetch Data From CRM
    async function GetDataForGrid(): Promise<any[]> {
        let rows = [];

        try {
            let result = await Promise.all([Xrm.WebApi.retrieveMultipleRecords('ipg_document', `?fetchXml=${DOCS_CASE_FETCH}`)
                , Xrm.WebApi.retrieveMultipleRecords('ipg_document', `?fetchXml=${DOCS_REFERRALS_FETCH}`)]);

            result.map(r => GetRowsFromResponse(r, rows));
        } catch (error) {
            console.log(error);
        }
        finally {
            return rows;
        }
    }

    //Grid Initialization
    export async function initGrid() {
        (<any>$(GRID_ID)).jqGrid({
            datatype: 'local',
            height: 'auto',
            data: await GetDataForGrid(),
            autowidth: true,
            rowNum: '100',
            resizable: true,
            hidegrid: false,
            reload: true,
            colNames: ['Row ID', 'Name', 'Type', 'Date'],
            colModel: [
                { name: 'ipg_documentid', index: 'ipg_documentid', key: true, hidden: true },
                {
                    name: 'ipg_name', index: 'ipg_name', width: 100,
                    formatter: docLinkFormatter
                },
                { name: 'ipg_documenttypename', index: 'ipg_documenttypename', width: 100 },
                {
                    name: 'createdon', index: 'createdon', width: 75, formatter: dateTimeFormatter
                }
            ],
            //when user sort rows new data fetched from CRM
            onSortCol: async function (index, columnIndex, sortOrder) {
                await LoadData();
            },
            caption: 'Documents',
            sortname: 'createdon',
            sortorder: "desc",
            pgbuttons: false,
            viewrecords: false,
            pgtext: "",
            pginput: false,
            multiselect: true,
            toppager: true,
            pagerpos:"right",
            beforeSelectRow: function (rowid, e) {
                (<any>jQuery(GRID_ID)).jqGrid('resetSelection');
                return true;
            }
        }).navGrid(`${GRID_ID}_toppager`, { refresh: false, search: false, edit: false, view: false, del: false, add: false, position:"left"})
            .navButtonAdd(`${GRID_ID}_toppager`, {
                caption: "Preview Document",
                buttonicon: "ui-icon-script",
                onClickButton: function () {
                    const selRowIds: string[] = (<any>jQuery(GRID_ID)).jqGrid('getGridParam', 'selarrrow');
                    if (!selRowIds || !selRowIds.length) {
                        Xrm.Navigation.openErrorDialog({ message: "Please select document" });
                        return;
                    }
                    else {
                        Intake.Document.OpenDocumentPreview(Xrm.Page, selRowIds[0]);
                    }
                }
            });

        //Hide select all checkbox
        (<any>$(`${GRID_ID}_cb`)).remove();
    }
    //Refresh Grid
    async function LoadData() {
        const JQGrid = (<any>jQuery(GRID_ID));
        const docs = await GetDataForGrid();

        JQGrid.jqGrid('setGridParam', { data: docs }).trigger('reloadGrid');
    }
}
