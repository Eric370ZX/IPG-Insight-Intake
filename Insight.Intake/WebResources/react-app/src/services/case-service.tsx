import { ICase } from "../models/ICase";
import { CaseFactory } from "../factories/case-factory";
import { BaseService } from "./base-service";

export class CaseService extends BaseService {

    constructor(){
        super("incident",
        [
          "_ipg_patientid_value",
          "title",
          "ipg_surgerydate",
          "ipg_actualdos",
          "incidentid",
          "ipg_lastebvcheckdatetime",
          "_ipg_carrierid_value",
          "_ipg_homeplancarrierid_value",
          "_ipg_procedureid_value",
          "ipg_iscourtesyclaimcase"
        ], new CaseFactory()
        )
    }

    async getCaseData(caseId:string): Promise<ICase> {
        return super.retrieveById(caseId, `$select=${this._columns.join(",")}&$expand=ipg_FacilityId($select=name,ipg_dtmmember,ipg_cpaonlyfacility)`);
    }
}
