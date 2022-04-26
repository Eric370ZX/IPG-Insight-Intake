import { ICase } from "../ICase";
import { IGenericPart } from "../IGenericPart";
import { TemporaryService } from "../../services/temp-service";
import { TaskService } from "../../services/task-service";
import {INotification} from "../../models/INotification";
import { ISearch } from "../ISearch";

export interface IActualPartsCardsGridHeaderProps {
    taskService: TaskService;
    refreshGrid: Function;
    toggleProgressBar: Function;
    addTemporaryParts: Function;
    case: ICase;
    tempService: TemporaryService;
    actualParts: IGenericPart[];
    setNotification:(value:INotification) => void;
    searchparts:ISearch;
    disabled:boolean;
}