import { ILookup } from "./ILookup";

export interface IManufacturer extends ILookup{
    ipg_manufactureraallowdirectbilling?: boolean,
    ipg_manufacturerisparticipating?:boolean
}