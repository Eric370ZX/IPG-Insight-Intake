import _ from "lodash";
import { IActualPart } from "../models/IActualPart";
import { ICombinePartsResult } from "../models/ICombinePartsResult";
import { IGenericPart } from "../models/IGenericPart";
import { BaseService } from "./base-service";
import { POType } from "../enums/po-types";
import { ICase } from "../models/ICase";

export class TemporaryService {
    public readonly removedParts:IActualPart[];
    private readonly _caseId: string = "";
    private readonly _storageKey: string = "ipg-manage-parts";
    private readonly _currentCaseKey: string = "";
    public readonly _service: BaseService
    private readonly _onChangeParts?:()=>void;
    
    public get Service() : BaseService {
        return this._service;
    }
    

    public constructor(caseId: string, service: BaseService, onChangeParts?:()=>void) {
        caseId = caseId.toLowerCase();
        this._service = service;
        this._currentCaseKey = `${this._storageKey}-${caseId}-${this._service._entityName}`;
        this._caseId = caseId;
        this._onChangeParts = onChangeParts;
        this.removedParts = [];
    }

    addNewPart(actualPart: any): boolean {
        const storageItem = localStorage.getItem(this._currentCaseKey);
        let tempItems: any[] = (storageItem && JSON.parse(storageItem)) || [];

        if (tempItems.findIndex(t => t.Part?.productid === actualPart.Part?.productid) < 0) {
            tempItems.splice(0, 0, actualPart);

            localStorage.setItem(this._currentCaseKey, JSON.stringify(tempItems));
            
            this._onChangeParts && this._onChangeParts();
            return true;
        }

        return false;
    }

    addFieldForUpdate(Part: IGenericPart, field: string, value: any) {
        
        const storageItem = localStorage.getItem(this._currentCaseKey);
        let tempItems: any[] = (storageItem && JSON.parse(storageItem)) || [];

        const index = tempItems.findIndex(p =>
            (Part[this._service._keyField] && p[this._service._keyField] === Part[this._service._keyField]) ||
            (Part.Part?.productid && p.Part?.productid === Part.Part?.productid));

        if (index < 0) {
            tempItems.push({
                [this._service._keyField]: Part[this._service._keyField],
                Part: Part.Part,
                [field]: value
            });
        }
        else {
            tempItems[index][field] = value;
        }

        localStorage.setItem(this._currentCaseKey, JSON.stringify(tempItems));
        
        this._onChangeParts && this._onChangeParts();
        return true;
    }

    getAll(): any[] {
        const storageItem = localStorage.getItem(this._currentCaseKey);
        return (storageItem && JSON.parse(storageItem)) || [];
    }

    getPartById(id:string): any {
        const allParts = this.getAll();
        console.log(allParts);
        
        return allParts.find(p => p[this._service._keyField] == id);
    }

    getPartByProductId(productid:string): any {
        const allParts = this.getAll();
        console.log(allParts);
        
        return allParts.find(p => p.Part?.productid == productid);
    }

    removePartFromStorage(Part: any):boolean
    {
        const storageItem = localStorage.getItem(this._currentCaseKey);
        let tempItems: any[] = (storageItem && JSON.parse(storageItem)) || [];

        const index = Part[this._service._keyField] ?
        tempItems.findIndex(t => t[this._service._keyField] === Part[this._service._keyField]) :
        tempItems.findIndex(t => t.Part?.productid === Part.Part?.productid);

        if (index >= 0)
        {
            tempItems.splice(index, 1);
            localStorage.setItem(this._currentCaseKey, JSON.stringify(tempItems));
            return true;
        }

        return false;
    }

    removePart(Part: any): boolean {        
        if(Part["ipg_casepartdetailid"])
        {
            this.removedParts.push(Part);
            return true;
        }
        else
        {
            return this.removePartFromStorage(Part);
        }
    }

    combineTempPartsAndActualParts(tempParts: any[], Parts: any[]): ICombinePartsResult {
        let dirtyParts: Map<string, string[]> = new Map<string, string[]>();

        Parts = Parts.filter(p => !this.removedParts.find(rp => rp.ipg_casepartdetailid === p.ipg_casepartdetailid));

        if (tempParts.length > 0) {
            tempParts.forEach(tempPart => {
                const index = Parts.findIndex(a => a[this._service._keyField] && a[this._service._keyField] === tempPart[this._service._keyField]);

                if (index < 0) {
                    Parts.splice(0, 0, tempPart);
                }
                else {
                    dirtyParts.set(Parts[index][this._service._keyField], []);

                    for (const attr in tempPart) {
                        if (!this.isValuesEqual(Parts[index][attr], tempPart[attr])) {
                            dirtyParts.get(Parts[index][this._service._keyField])?.push(attr);
                        }

                        Parts[index][attr] = tempPart[attr];
                    }
                }
            });
        }

        return {
            actualParts: Parts,
            dirtyParts: dirtyParts
        } as ICombinePartsResult;
    }

    async createTempParts(): Promise<boolean> {
        const allParts = this.getAll();
        const partsForCreate = allParts.filter(p => !p[this._service._keyField]);
        let partsLeft = [...allParts];

        for (let i = 0; i < partsForCreate.length; i++) {
            let actualPart = partsForCreate[i];

            try {
                const productid = actualPart.Part?.productid;
                actualPart["ipg_productid@odata.bind"] = `/products(${actualPart?.Part?.productid})`;
                actualPart["ipg_caseid@odata.bind"] = `/incidents(${this._caseId})`;

                actualPart = _.omit(actualPart, "Part") as IActualPart;

                await this._service.create(actualPart);

                const index = actualPart[this._service._keyField] ?
                    partsLeft.findIndex(t => t[this._service._keyField] === actualPart[this._service._keyField]) :
                    partsLeft.findIndex(t => t.Part?.productid === productid);

                partsLeft.splice(index, 1);
            }
            catch (exception) {
                console.error(exception);
                throw exception;
            }
        }

        if (partsLeft.length > 0) {
            localStorage.setItem(this._currentCaseKey, JSON.stringify(partsLeft));
        }
        else {
            localStorage.removeItem(this._currentCaseKey);
        }

        return true;
    }

    async updateTempParts(): Promise<boolean> {
        const allParts = this.getAll();
        const partsForUpdate = allParts.filter(p => p[this._service._keyField]);
        let partsLeft = [...allParts];

        for (let i = 0; i < partsForUpdate.length; i++) {
            let actualPart = partsForUpdate[i];

            try {
                const productid = actualPart.Part?.productid;
                await this._service.update(actualPart);

                const index = actualPart[this._service._keyField] ?
                    partsLeft.findIndex(t => t[this._service._keyField] === actualPart[this._service._keyField]) :
                    partsLeft.findIndex(t => t.Part?.productid === productid);

                partsLeft.splice(index, 1);
            }
            catch (exception) {
                console.error(exception);
                throw exception;
            }
        }

        if (partsLeft.length > 0) {
            localStorage.setItem(this._currentCaseKey, JSON.stringify(partsLeft));
        }
        else {
            localStorage.removeItem(this._currentCaseKey);
        }

        return true;
    }
    async deleteRemovedParts(): Promise<boolean> {

        for (let i = 0; i < this.removedParts.length; i++) {
            let actualPart = this.removedParts[i];

            try {
                await this._service.remove(actualPart.ipg_casepartdetailid);
                this.removedParts.slice(this.removedParts.indexOf(actualPart));
                this.removePartFromStorage(actualPart);
            }
            catch (exception) {
                console.error(exception);
                throw exception;
            }
        }
        return true;
    }

    isValuesEqual(value1: any, value2: any): boolean {
        if (!value1 && !value2) {
            return true;
        }
        else if ((!value1 && value2) || (value1 && !value2)) {
            return false;
        }

        // Use == instead of === on purpose, because webapi object has number value, but controls have text values
        // eslint-disable-next-line eqeqeq
        return value1 == value2;
    }
    getValidPOTypes = (part:IGenericPart, incident:ICase):POType[] =>
    {
        let casePart = part;
        var availiblePOTypes:POType[] = [];
        let dateNow = new Date();
        let currentDate = new Date(dateNow.getFullYear(), dateNow.getMonth(), dateNow.getDate());
        let surgeryDate = new Date(incident.ipg_surgerydate.getFullYear(), incident.ipg_surgerydate.getMonth(), incident.ipg_surgerydate.getDate());
        
        let cpaonly = incident.ipg_FacilityId?.ipg_cpaonlyfacility 
        let payManufacturer = (part?.Manufacturer?.ipg_manufactureraallowdirectbilling === undefined) 
            || part.actualPart.Part.Manufacturer.ipg_manufactureraallowdirectbilling;    
                
        let dtmmember = incident.ipg_FacilityId?.ipg_dtmmember;

        if(this.isNotActualPart()){
            availiblePOTypes.push(POType.CPA, POType.ZPO, POType.MPO, POType.TPO)
            console.log(availiblePOTypes, "availiblePOTypes");
            
            return availiblePOTypes;
        }

        if((cpaonly || !payManufacturer || dtmmember == false || (casePart.Part?.ipg_boxquantity || 0) > 1))
        {      
            availiblePOTypes.push(POType.CPA);
        }
        else if(currentDate <= surgeryDate)
        {
            availiblePOTypes.push(POType.TPO);
        }
        else
        {
            availiblePOTypes.push(POType.CPA, POType.ZPO, POType.MPO);
        }
        
        return availiblePOTypes;
    }

    getDefaultPOType = (part:IGenericPart, incident:ICase) => {
        const validPoTypes = this.getValidPOTypes(part, incident);
        const includesTPO = validPoTypes.includes(POType.TPO);
        let result = includesTPO  
        ? POType.TPO
        : validPoTypes.length == 1
            ? validPoTypes[0]
            : null;
            
        if(result == null && validPoTypes.includes(POType.CPA))
            result = POType.CPA;
            
         return result;         
    }
    
    isNotActualPart = ():boolean => this._service._entityName != "ipg_casepartdetail";
}