namespace Intake.Order.Ribbon {
    export enum statecode {
        Closed = 3
    }

    export enum statuscode {
        InvoiceReceived = 923720006,
        InReview = 923720007,
        VerifiedForPayment = 923720008,
        Partial = 923720010
    }

    const _entityName: string = "salesorder";

    const _communicationExceptionViewId: string = "B2B608FF-9738-EB11-A813-00224802E4A2";
    const _communicationExceptionControlId: string = "PoCommunicationExceptions";

    export async function RemoveFromExceptionView(selectedControl, selectedRecordIds: string[]): Promise<void> {
        Xrm.Utility.showProgressIndicator("Removing from the view");

        for (let recordId of selectedRecordIds) {
            await Xrm.WebApi.updateRecord(_entityName, recordId, {
                ipg_isexcludedfromexceptionsdashboard: true
            });
        }

        Xrm.Utility.closeProgressIndicator();
        selectedControl.refresh();
    }

    export function IsRemoveFromExceptionViewEnabled(selected) {
        if (selected.controlDescriptor?.Id === _communicationExceptionControlId &&
            selected.controlDescriptor?.Parameters?.ViewId?.toLowerCase().includes(_communicationExceptionViewId.toLowerCase())) {
            return true;
        }
    }

    /**
    * enable rule for Generate PO
    * @function Intake.Order.Ribbon.IsLastCalcRevAfterLastActualPartDate
    * @returns {boolean}
    */
     export async function IsLastCalcRevAfterLastActualPartDate(primaryControl: Xrm.FormContext, caseId) {
        const lastCalcRevDate = new Date(primaryControl.getAttribute("ipg_lastcalcrevon").getValue());

        const actualParts = await (Xrm.WebApi.retrieveMultipleRecords(
            'ipg_casepartdetail',
            `?$select=ipg_casepartdetailid,modifiedon&$filter=_ipg_caseid_value eq ${caseId.replace("{", "").replace("}", "").toLowerCase()}`
        ));

        let latestPartDate: Date = new Date(actualParts.entities[0]["modifiedon"]);
        actualParts.entities.forEach(part => {
            const partDate = new Date(part["modifiedon"]);
            if (partDate > latestPartDate)
                latestPartDate = partDate;
        });

        if (latestPartDate < lastCalcRevDate)
            return true;
        return false;
    }

    /**
    * enable rule for Generate PO
    * @function Intake.Order.Ribbon.IsAllActualPartsLocked
    * @returns {boolean}
    */
    export async function IsAllActualPartsLocked(caseId) {
        const actualParts = (await (Xrm.WebApi.retrieveMultipleRecords(
            'ipg_casepartdetail',
            `?$select=ipg_casepartdetailid,_ipg_purchaseorderid_value&$filter=_ipg_caseid_value eq ${caseId.replace("{", "").replace("}", "").toLowerCase()}`
        ))).entities;
        
        let counter = 0;
        for (const part of actualParts) {
             if(await GetAmountOfLockedParts(part)){
                counter++;
            }
        }

        return counter != actualParts.length;
    }

    async function GetAmountOfLockedParts(part): Promise<boolean> {
        
        if (part["_ipg_purchaseorderid_value"] != null) {
            const order = await (Xrm.WebApi.retrieveRecord(
                'salesorder',
                `${part["_ipg_purchaseorderid_value"]}`,
                `?$select=statecode,statuscode`
            ));

            if (order['statecode'] === statecode.Closed ||
                order['statuscode'] === statuscode.InReview ||
                order['statuscode'] === statuscode.InvoiceReceived ||
                order['statuscode'] === statuscode.Partial ||
                order['statuscode'] === statuscode.VerifiedForPayment) {
                return true;
            }
        }
        return false;
    }

    export function DisplayIconTooltip(rowData, userLCID) {      
        const str = JSON.parse(rowData);  
        const coldata = str.ipg_potypecode;  
        const location = "ipg_/intake/img/";
        const etimated = str.ipg_isestimatedpo_Value?._val;

        let imgName = "";  
        let tooltip = coldata;  

        if(tooltip == "TPO" && etimated)
        {
            tooltip = "Estimated" + tooltip
        }

        switch (coldata) {
            case 'TPO'://923720000: //TPO  
                imgName = etimated ? "PO_ETicon.gif" : "PO_Ticon.gif" ;
                break;
            case 'ZPO': //ZPO
                imgName = "PO_Zicon.gif";
                break;
            case 'CPA': //CPA
                imgName = "PO_Cicon.gif";
                break;
            case 'MPO': //MPO
                imgName = "PO_MPOicon.gif";
                break;
            default:
                imgName = "";
                tooltip = "";
                break;
        }

      
        const resultarray = [imgName ? location + imgName : '', tooltip];
        
        return resultarray;  
    } 
    export async function UnLockPOs(POIds: string[], gridControl: Xrm.Controls.GridControl)
    {
        if(!POIds || POIds.length == 0)
        {
            Xrm.Navigation.openAlertDialog({text:"Please select at least 1 record!"});
            return;
        }

        Xrm.Utility.showProgressIndicator("");

        try{
            for(let i = 0; i < POIds.length; i ++)
            {
                await Xrm.WebApi.updateRecord(_entityName, POIds[i], 
                {
                    "ipg_commstatus":923720003, //Ready to Communicate
                    "ipg_invoicestatus":923720000, //Unverified
                    "statecode":0, //Open
                    "statuscode":923720015, //Unlocked
                    "ipg_mysurgpropostatus":427880001 //Open
                });
            }
        }
        catch(e)
        {
            Xrm.Navigation.openAlertDialog({text:"There was an error on unlock PO. Please try later or Contact System Administrator!"});
        }
        finally
        {
            gridControl.refresh();
            Xrm.Utility.closeProgressIndicator();
        }
    } 
}