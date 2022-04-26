import {IEstimatedPart} from './IEstimatedPart';
import {IActualPart} from './IActualPart';

export interface ISearch
{
    getByProductAndCase(productId: string, caseId: string, topCount: number): Promise<IEstimatedPart[] | IActualPart[]>;
}