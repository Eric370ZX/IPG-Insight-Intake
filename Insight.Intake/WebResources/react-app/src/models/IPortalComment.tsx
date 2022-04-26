import { ILookup } from "./ILookup";

export interface IPortalComment {
    id:string;
    RegardingId?: ILookup;
    Facillity?: ILookup;
    createdon?:Date
    description?: string;
    statuscode?: number;
    casestatus?:number;
}