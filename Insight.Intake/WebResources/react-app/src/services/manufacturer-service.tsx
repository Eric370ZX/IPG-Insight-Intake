import { BaseService } from "./base-service";
import {ManufacturerFactory} from "../factories/manufacturer-factory"

export class ManufacturerService extends BaseService {
    constructor() {
        super("account"
        , ["ipg_manufactureraallowdirectbilling","ipg_manufacturerisparticipating"]
        , new ManufacturerFactory()
        );    
    }

    async getById(id: string): Promise<any> {
      return await super.retrieveById(id);
    }
}
