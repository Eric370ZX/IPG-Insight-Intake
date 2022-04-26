import { InvoiceFactory } from "../factories/invoice-factory";
import { IEntityCollection } from "../models/IEntityCollection";
import { IInvoice } from "../models/IInvoice";

export class InvoiceService {
    entityName: string = "invoice";

    columns: string[] = [
        "invoiceid", "invoicenumber", "_ipg_caseid_value"
    ];

    requestHeaders: any = {
        "Prefer": 'odata.include-annotations="OData.Community.Display.V1.FormattedValue"'
    };

    _invoiceFactory: InvoiceFactory;

    constructor(){
        this._invoiceFactory = new InvoiceFactory();
    }

    async getInvoices(topCount:number): Promise<IInvoice[]> {
        const select = `$select=${this.columns.join(",")}`;
        const top = `$top=${topCount || 100}`;
        const requestUrl = `/api/data/v9.1/${this.entityName}s?${select}&${top}`;

        const requestParams = {
            method: "GET",
            headers: this.requestHeaders
        };

        const response = await fetch(requestUrl, requestParams);
        const entityCollection = await response.json() as IEntityCollection;

        return (entityCollection.value || []).map((record) => this._invoiceFactory.getFromWebApi(record));
    }
}