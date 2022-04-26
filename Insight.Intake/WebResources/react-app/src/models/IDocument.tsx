import { ILookup } from "./ILookup";

export interface IDocument {
    Id: string;
    Name: string;
    CreatedOn: string;
    CreatedOnFormatted: string;
    ["actualPart.ipg_purchaseorderid"]?: ILookup,
    ["as.ipg_procedureid"]?: ILookup,
    ["as.incidentid"]:string,
    ["actualPart.ipg_quantity"]?:number,
    ["actualPart.ipg_enteredunitcost"]?:number,
    ["annotation.annotationid"]?:string,
    ["as.title"]?:string
}