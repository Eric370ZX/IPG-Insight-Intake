
import {IGenericPart} from "./IGenericPart";
export interface IActualPart extends IGenericPart{
    ipg_casepartdetailid: string;

    ipg_claimpricetier?: string;

    ipg_quantity?: number;
    ipg_quantitywasted?: number;
    ipg_serialnumber?: string;
    ipg_lotnumber?: string;

    ipg_potypecode?: number;
    ["ipg_potypecode@OData.Community.Display.V1.FormattedValue"]?: string;
    ipg_enteredunitcost?: number;
    ipg_enteredshipping?: number;
    ipg_enteredtax?: number;

    PoTotalCost?: number;
    _ipg_hcpcscode_value?: string;
    Category?: string;
    modifiedon?: Date;
    ipg_iscourtesyclaimplan?: boolean;
    ipg_islocked?:boolean;
}
