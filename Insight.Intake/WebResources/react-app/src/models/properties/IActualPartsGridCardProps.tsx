import {ICase} from "../ICase";
import { IGenericPart } from "../IGenericPart";
import { TemporaryService } from "../../services/temp-service";
import { INotification } from "../INotification";

export interface IActualPartsGridCardProps {
    caseId: string;
    case: ICase;
    actualPart: IGenericPart;
    dirtyAttributes: string[];
    refreshGrid: Function;
    disabled?:boolean;
    tempService: TemporaryService;
    setNotification:(value:INotification) => void;
}