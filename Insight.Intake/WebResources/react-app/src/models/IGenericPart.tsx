import { IPart } from "./IPart";

export interface IGenericPart
{
    Part?: IPart;
    [key: string]: any;
}