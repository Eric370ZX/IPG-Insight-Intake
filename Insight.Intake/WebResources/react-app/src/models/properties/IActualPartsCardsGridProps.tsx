import {ICase} from "../ICase";
import { IGenericPart } from "../IGenericPart";
import { TemporaryService } from "../../services/temp-service";
import { INotification } from "../INotification";

export interface IActualPartsCardsGridProps {
    actualParts: IGenericPart[];
    dirtyParts: Map<string, string[]>;
    refreshGrid: Function;
    toggleProgressBar: Function;
    case: ICase;
    tempService:TemporaryService;
    setNotification:(value:INotification) => void;
    disabled:boolean;
}