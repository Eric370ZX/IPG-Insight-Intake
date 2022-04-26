import { IEstimatedPart } from "../models/IEstimatedPart";
import { IPart } from "../models/IPart";
import WebApiService from "../services/web-api-service";
import { PartFactory } from "./part-factory";
import {IFactory} from "./factory";

export class CaseEstPartDetailFactory implements IFactory {
    getFromWebApi(record: any): IEstimatedPart {
        let actualPart: IEstimatedPart = record as IEstimatedPart;

        const product = WebApiService.getAttributeValue(record, "ipg_productid");
        if(product)
        {
            actualPart.Part = new PartFactory().getFromWebApi(product);
        }

        return actualPart;        
    }

    getTempFromPart(part: IPart) : IEstimatedPart {
        let actualPart: any = {};

        actualPart.Part = part;

        return actualPart;
    }
}