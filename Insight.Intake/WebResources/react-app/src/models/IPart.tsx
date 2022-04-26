import { IManufacturer } from "./IManufacturer";

export interface IPart {
    id: string;
    productid: string;
    name: string;
    ipg_partnumber: string;
    ipg_manufacturerpartnumber: string;
    description: string;
    ClaimTier: string;
    Keyword:string;

    ipg_boxquantity: number;
    ipg_maxquantity: number,
    ipg_enforcemaxquantity: number,
    ipg_msrp: number;
    ipg_manufacturerdiscountpricemoney: number;
    ipg_averageinvoicecost: number;

    _ipg_hcpcscodeid_value: string;
    ["_ipg_hcpcscodeid_value@OData.Community.Display.V1.FormattedValue"]?: string;

    ipg_ipgpartnumber: string;
    ["ipg_ipgpartnumber@OData.Community.Display.V1.FormattedValue"]?: string;

    producttypecode: number;
    ["producttypecode@OData.Community.Display.V1.FormattedValue"]?: string;

    _ipg_manufacturerid_value: string;
    ["_ipg_manufacturerid_value@OData.Community.Display.V1.FormattedValue"]?: string;

    ipg_effectivedate?: Date;
    ipg_discontinuedon?: Date;

    Manufacturer: IManufacturer;

    CompositeName: string;

    ipg_islocked: boolean;
}
