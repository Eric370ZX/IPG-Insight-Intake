export interface ISearchValues {
    partNumber: string;
    hcpcs: string;
    partDescription: string;
    category: number | null;
    manufacturerName: string;
    keyword: number | null;

    [key: string]: any;
}