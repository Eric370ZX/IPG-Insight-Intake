import { IAccount } from "./IAccount";
export interface ICase {
  incidentid: string;
  title: string;
  ipg_surgerydate: Date;
  ipg_FacilityId?: IAccount;
  ipg_actualdos?: Date;
  ipg_lastebvcheckdatetime?: Date;
  _ipg_carrierid_value: string;
  _ipg_homeplancarrierid_value: string;
  ipg_iscourtesyclaimcase?: boolean;

  "_ipg_patientid_value@OData.Community.Display.V1.FormattedValue": string;
  "_ipg_procedureid_value@OData.Community.Display.V1.FormattedValue": string;
}
