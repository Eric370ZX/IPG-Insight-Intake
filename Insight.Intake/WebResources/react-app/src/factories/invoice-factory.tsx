import { IInvoice } from "../models/IInvoice";
import WebApiService from "../services/web-api-service";
import {IFactory} from "./factory";

export class InvoiceFactory implements IFactory {
    getFromWebApi(record: any) : IInvoice {
        return {
            Id: WebApiService.getAttributeValue(record, "invoiceid"),
            Number: WebApiService.getAttributeValue(record, "invoicenumber"),
            Case: {
                name: WebApiService.getAttributeValue(record, `_ipg_caseid_value@${WebApiService.formattedValueSuffix}`),
                id: WebApiService.getAttributeValue(record, `_ipg_caseid_value`),
                entityName: "incident"
            }
        } as IInvoice;
    }
}