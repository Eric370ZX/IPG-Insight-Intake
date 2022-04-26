import { IDocument } from "../models/IDocument";
import WebApiService from "../services/web-api-service";
import {IFactory} from "./factory";

export class DocumentFactory implements IFactory {
    getFromWebApi(record: any) : IDocument {
        return {
            Id: WebApiService.getAttributeValue(record, "ipg_documentid"),
            Name: WebApiService.getAttributeValue(record, "ipg_name"),
            CreatedOn: WebApiService.getAttributeValue(record, "createdon"),
            CreatedOnFormatted: WebApiService.getAttributeValue(record, `createdon@${WebApiService.formattedValueSuffix}`),
            ["actualPart.ipg_purchaseorderid"]:{
                name: WebApiService.getAttributeValue(record, `actualPart.ipg_purchaseorderid@${WebApiService.formattedValueSuffix}`),
                id: WebApiService.getAttributeValue(record, `actualPart.ipg_purchaseorderid`),
                entityName: "account"
                },
            ["as.ipg_procedureid"]:{
                name: WebApiService.getAttributeValue(record, `as.ipg_procedureid@${WebApiService.formattedValueSuffix}`),
                id: WebApiService.getAttributeValue(record, `as.ipg_procedureid`),
                entityName: "ipg_procedurename"
            },
            ["actualPart.ipg_quantity"]: WebApiService.getAttributeValue(record,"actualPart.ipg_quantity"),
            ["actualPart.ipg_enteredunitcost"]: WebApiService.getAttributeValue(record,"actualPart.ipg_enteredunitcost"),
            ["annotation.annotationid"]: WebApiService.getAttributeValue(record,"annotation.annotationid"),
            ["as.title"]: WebApiService.getAttributeValue(record,"as.title"),
            ["as.incidentid"]:  WebApiService.getAttributeValue(record,"as.incidentid")
            } as IDocument;
    }
}