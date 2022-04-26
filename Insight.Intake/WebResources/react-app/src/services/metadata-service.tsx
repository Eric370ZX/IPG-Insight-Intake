export class MetadataService {
    private _requestHeaders: any = {
        "Prefer": 'odata.include-annotations="OData.Community.Display.V1.FormattedValue"'
    };

    async getOptionSetValues(entityName: string, attributeName: string): Promise<Xrm.Metadata.OptionMetadata[]> {
        const requestUrl = `/api/data/v9.1/EntityDefinitions(LogicalName='${entityName}')/Attributes/Microsoft.Dynamics.CRM.PicklistAttributeMetadata?$select=LogicalName&$expand=OptionSet&$filter=LogicalName eq '${attributeName}'`;

        const requestParams = {
            method: "GET",
            headers: this._requestHeaders
        };

        const response = await fetch(requestUrl, requestParams);

        const result = await response.json();
        return result.value[0].OptionSet.Options as Xrm.Metadata.OptionMetadata[];
    }
}