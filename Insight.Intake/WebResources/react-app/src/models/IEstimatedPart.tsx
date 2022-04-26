import {IGenericPart} from "./IGenericPart";

export interface IEstimatedPart extends IGenericPart {
    ipg_estimatedcasepartdetailid:string;

    ipg_quantity?: number;

    ipg_potypecode?: number;
    ["ipg_potypecode@OData.Community.Display.V1.FormattedValue"]?: string;
    ipg_unitcost?: number;
    ipg_unitshipping?: number;
    ipg_unittax?: number;

    modifiedon?: Date;
}