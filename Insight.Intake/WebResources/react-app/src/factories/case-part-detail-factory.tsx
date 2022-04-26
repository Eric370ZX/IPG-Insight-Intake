import { IActualPart } from "../models/IActualPart";
import { IPart } from "../models/IPart";
import WebApiService from "../services/web-api-service";
import { PartFactory } from "./part-factory";
import {IFactory} from "./factory";
import {statecode as orderStates} from "../enums/order-states"
import {statuscode as orderStatuses} from "../enums/order-statuses"

export class CasePartDetailFactory implements IFactory{
    getFromWebApi(record: any): IActualPart {
        let actualPart: IActualPart = record as IActualPart;
        actualPart.ipg_islocked = actualPart?.ipg_islocked ?? false;
        const product = WebApiService.getAttributeValue(record, "ipg_productid");
        
        if(product)
        {
            actualPart.Part = new PartFactory().getFromWebApi(product);
        }

        return actualPart;        
    }

    getTempFromPart(part: IPart) : IActualPart {
        let actualPart: any = {};

        actualPart.Part = part;

        return actualPart;
    }
}
