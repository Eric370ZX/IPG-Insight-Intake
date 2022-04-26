import { ILookup } from "../models/ILookup";

export default class WebApiService {
    static formattedValueSuffix: string = "OData.Community.Display.V1.FormattedValue";

    async sendRequest(request: RequestInfo) : Promise<any> {
        const response = await fetch(request);
        return response.json();
    }

    static getAttributeValue(record: any, attribute: string): any {
        if (record && record.hasOwnProperty(attribute))
            return record[attribute];

        return null;
    }

    static getLookupValue(record: any, attribute: string, entName = ""): ILookup {
        const lookupAttrbiute = `_${attribute}_value`;
        const lookupObject: ILookup = {
            id: this.getAttributeValue(record, lookupAttrbiute),
            name: this.getFormattedValue(record, lookupAttrbiute),
            entityName:entName
        };

        return lookupObject;
    }

    static getFormattedValue(record: any, attribute: string): any {
        attribute = `${attribute}@${WebApiService.formattedValueSuffix}`;

        return WebApiService.getAttributeValue(record, attribute);
    }
}