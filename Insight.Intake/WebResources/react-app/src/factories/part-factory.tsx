import { IPart } from "../models/IPart";
import WebApiService from "../services/web-api-service";
import {IFactory} from "./factory";

export class PartFactory implements IFactory {
    getFromWebApi(record: any): IPart {
        let part: IPart = record as IPart;

        part.id = part.productid;
        part.Manufacturer = WebApiService.getAttributeValue(record, "ipg_manufacturerid");
        part.Keyword = WebApiService.getFormattedValue(record, "ipg_ipgpartnumber");
        part.ClaimTier = WebApiService.getFormattedValue(record, "ipg_claimtier");

        if (part.ipg_effectivedate)
            part.ipg_effectivedate = new Date(record.ipg_effectivedate);

        if (part.ipg_discontinuedon)
            part.ipg_discontinuedon = new Date(record.ipg_discontinuedon);
           
        let namePieces = [];

        part.ipg_manufacturerpartnumber && namePieces.push(part.ipg_manufacturerpartnumber);
        part.name && namePieces.push(part.name);

        part.CompositeName = namePieces.join(", ");

        return part;
    }
}
