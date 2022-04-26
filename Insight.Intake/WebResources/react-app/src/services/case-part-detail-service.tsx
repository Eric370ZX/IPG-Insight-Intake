import { IActualPart } from "../models/IActualPart";
import { IActionResponse } from "../models/IActionResponse";
import { ISearch } from "../models/ISearch";
import { CasePartDetailFactory } from "../factories/case-part-detail-factory";
import { GridView } from "../enums/grid-views";
import { PartService } from "./parts-service";
import { BaseService } from "./base-service";

export class CasePartDetailService extends BaseService implements  ISearch {
    private readonly _partService = new PartService();
    private readonly _order = `$orderby=ipg_quantity desc, createdon desc`;
    private readonly _expand = `$expand=ipg_productid($select=${this._partService._columns.join(",")})`;

    constructor() {
        super("ipg_casepartdetail"
        ,[
            "_ipg_productid_value", "ipg_claimpricetier", "ipg_quantity", "ipg_enteredtax",
            "ipg_quantitywasted", "ipg_serialnumber", "ipg_lotnumber", "ipg_potypecode",
            "ipg_enteredunitcost", "ipg_enteredshipping", "_ipg_hcpcscode_value", "ipg_truecost", "ipg_extcost",
            "modifiedon", "ipg_casepartdetailid", "ipg_iscourtesyclaimplan", "ipg_islocked"
        ],
        new CasePartDetailFactory()
        );
    }

    async getPartsByView(view: GridView, caseId: string): Promise<IActualPart[]> {
        switch (view) {
            case GridView.ActiveParts: {
                return await this.getActiveCaseParts(caseId);
            }
            case GridView.DebitedParts: {
                return await this.getDebitedCaseParts(caseId);
            }
            default: {
                return await this.getAllCaseParts(caseId);
            }
        }
    }


    async getAllCaseParts(caseId: string): Promise<IActualPart[]> {
        const filter = `$filter=_ipg_caseid_value eq '${caseId}'`;
        const select = `$select=${this._columns.join(",")}`;

        return super.retrieve(`${filter}&${select}&${this._expand}&${this._order}`);
    }

    async getActiveCaseParts(caseId: string): Promise<IActualPart[]> {
        const filter = `$filter=_ipg_caseid_value eq '${caseId}' and statecode eq 0`;
        const select = `$select=${this._columns.join(",")}`;

        return super.retrieve(`${filter}&${select}&${this._expand}&${this._order}`);
    }

    async getDebitedCaseParts(caseId: string): Promise<IActualPart[]> {
        const filter = `$filter=_ipg_caseid_value eq '${caseId}' and statecode eq 0 and _ipg_debitpartid_value ne null`;
        const select = `$select=${this._columns.join(",")}`;

        return super.retrieve(`${filter}&${select}&${this._expand}&${this._order}`);
    }

    async getByProductAndCase(productId: string, caseId: string, topCount: number): Promise<IActualPart[]> {
        const filter = `$filter=_ipg_caseid_value eq '${caseId}' and _ipg_productid_value eq '${productId}'`;
        const select = `$select=ipg_casepartdetailid`;
        const top = `$top=${topCount}`;

        return super.retrieve(`${filter}&${select}&${top}`);
    }

    async voidPartAction(casePartId: string): Promise<IActionResponse> {
        const requestUrl = `/api/data/v9.1/${this._entitySetName}(${casePartId})/Microsoft.Dynamics.CRM.ipg_CasePartDetailActionVoid`;

        const requestParams = {
            method: "POST",
            headers: this._postRequestHeaders
        };

        const response = await fetch(requestUrl, requestParams);
        return await response.json() as IActionResponse;
    }

    async remove(casePartId: string): Promise<IActionResponse> {
        const requestUrl = `/api/data/v9.1/${this._entitySetName}(${casePartId})/Microsoft.Dynamics.CRM.ipg_CasePartDetailActionRemove`;

        const requestParams = {
            method: "POST",
            headers: this._postRequestHeaders
        };

        const response = await fetch(requestUrl, requestParams);
        return await response.json() as IActionResponse;
    }
}
