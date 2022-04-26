import { IEstimatedPart } from "../models/IEstimatedPart";
import { CaseEstPartDetailFactory } from "../factories/case-estimated-part-factory";
import { GridView } from "../enums/grid-views";
import { PartService } from "./parts-service";
import { BaseService } from "./base-service";
import _ from "lodash";

export class CaseEstimatedPartDetailService extends BaseService {
    private readonly _partService = new PartService();
    constructor() {
        super("ipg_estimatedcasepartdetail"
        , [
            "_ipg_productid_value", "ipg_quantity", "ipg_unittax", "ipg_potypecode",
            "ipg_unitcost", "ipg_unitshipping",
            "modifiedon", "ipg_actualpartid","ipg_estimatedcasepartdetailid"],
            new CaseEstPartDetailFactory()
        );    
    }

    async getPartsByView(view: GridView, caseId: string): Promise<IEstimatedPart[]> {
        return this.getActiveCaseEstimatedParts(caseId);
    }

    async getActiveCaseEstimatedParts(caseId: string): Promise<IEstimatedPart[]> {
        const filter = `$filter=_ipg_caseid_value eq '${caseId}' and statecode eq 0`;
        const select = `$select=${this._columns.join(",")}`;
        const expand = `$expand=ipg_productid($select=${this._partService._columns.join(",")})`;
        const order = `$orderby=createdon desc`;

        return super.retrieve( `${filter}&${select}&${expand}&${order}`);
    }

    async getByProductAndCase(productId: string, caseId: string, topCount: number): Promise<IEstimatedPart[]> {
        const filter = `$filter=_ipg_caseid_value eq '${caseId}' and _ipg_productid_value eq '${productId}'`;
        const select = `$select=ipg_estimatedcasepartdetailid`;
        const top = `$top=${topCount}`;

        return super.retrieve( `${filter}&${select}&${top}`);
    }
}