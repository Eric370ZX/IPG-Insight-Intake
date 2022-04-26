import { IActualPart } from "./IActualPart";
import { IEstimatedPart } from "./IEstimatedPart";

export interface ICombinePartsResult {
    actualParts: IActualPart[] | IEstimatedPart[];
    dirtyParts: Map<string, string[]>;
}