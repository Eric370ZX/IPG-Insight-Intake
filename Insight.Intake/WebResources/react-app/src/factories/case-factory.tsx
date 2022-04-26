import { ICase } from "../models/ICase";
import {IFactory} from "./factory";

export class CaseFactory implements IFactory {
    getFromWebApi(record: any): ICase {
        let caseObj: ICase = record as ICase;

        if (record.ipg_surgerydate)
        {
            caseObj.ipg_surgerydate = new Date(record.ipg_surgerydate);
        }
        if(record.ipg_actualdos)
        {
            caseObj.ipg_actualdos = new Date(record.ipg_actualdos);
        }

        return caseObj;
    }
}