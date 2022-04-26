import { IPart } from "../IPart";

export interface IPartCollection {
    '@odata.context': string;
    '@odata.count': number;
    value: IPart[];
}