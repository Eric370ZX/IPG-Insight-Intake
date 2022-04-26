import { ILookup } from "./ILookup";

export interface IInvoice {
    Id: string;
    Number: string;
    Case: ILookup;
}