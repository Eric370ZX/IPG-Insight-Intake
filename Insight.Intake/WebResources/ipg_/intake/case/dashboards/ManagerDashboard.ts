namespace Insight.Intake {
    const redCircle = "ipg_/intake/img/red-circle-32px.png";
    const yellowCircle = "ipg_/intake/img/yellow-circle-32px.png";
    const greenCircle = "ipg_/intake/img/green-circle-32px.png";
    const greenCheckMark = "ipg_/intake/img/check-mark-green-32px.png";
    const redCancel = "ipg_/intake/img/red-cancel-32px.png";

    export async function ShowSlaThresholdIcon(rowData, userLcid) {
        const rowObject = JSON.parse(rowData);

        const incidentId = rowObject.RowId.replace("{", "").replace("}", "");

        let imgLink = "";

        if (rowObject.ipg_slathresholddayscalculated_Value != undefined &&
            rowObject.ipg_slathresholddayscalculated_Value != null) {
            if (rowObject.ipg_slathresholddayscalculated_Value <= 0) {
                imgLink = redCircle;
            }
            else if (rowObject.ipg_slathresholddayscalculated_Value == 1) {
                imgLink = yellowCircle;
            }
            else if (rowObject.ipg_slathresholddayscalculated_Value > 1) {
                imgLink = greenCircle;
            }
        }
        else {
            const filter = `$filter=_ipg_caseid_value eq '${incidentId}' and ipg_slatypecode ne null and statecode eq 1`;
            const select = `$select=actualend,scheduledend`;
            const orderBy = `$orderby=scheduledend desc`;
            const top = `$top=1`;
            const url = `/api/data/v9.1/tasks?${filter}&${select}&${orderBy}&${top}`;

            const response = await fetch(url);
            const records = await response.json();

            if (records.value && records.value.length > 0) {
                if (records.value[0].actualend > records.value[0].scheduledend) {
                    imgLink = redCancel;
                }
                else {
                    imgLink = greenCheckMark;
                }
            }
        }

        return [imgLink];
    }
}