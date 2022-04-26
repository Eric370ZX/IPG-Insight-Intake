import { IManufacturer } from "../models/IManufacturer";
import {IFactory} from "./factory";

export class ManufacturerFactory implements IFactory {
    getFromWebApi(record: any): IManufacturer{
        return {
            id: record.accountid,
            name: record?.name,
            entityName: 'account',
            ipg_manufactureraallowdirectbilling : record.ipg_manufactureraallowdirectbilling,
            ipg_manufacturerisparticipating: record.ipg_manufacturerisparticipating
        } as IManufacturer
    }
}