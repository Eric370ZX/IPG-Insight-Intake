import { IEntity } from "./IEntity";

export interface IEntityCollection {
    '@odata.context': string;
    '@odata.count': number;
    value: IEntity[];
}