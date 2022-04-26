import { IDocument } from "../IDocument";

export interface IDocumentCollection {
    '@odata.context': string;
    '@odata.count': number;
    value: IDocument[];
}