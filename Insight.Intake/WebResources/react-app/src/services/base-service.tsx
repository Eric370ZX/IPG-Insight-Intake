import {IFactory} from "../factories/factory";
import { IEntityCollection } from "../models/IEntityCollection";
import { IActionResponse } from "../models/IActionResponse";
import { GridView } from "../enums/grid-views";
import _ from "lodash";
import { IGenericPart } from "../models/IGenericPart";
import { TurnedInTwoTone } from "@material-ui/icons";

export class BaseService
{
    private readonly _webApi = "/api/data/v9.1/";
    public readonly _entityName:string;
    protected readonly _entitySetName:string;
    public readonly _keyField:string;
    protected readonly _factory:IFactory;
    protected readonly _getRequestHeaders: any = {
        "Prefer": 'odata.include-annotations="OData.Community.Display.V1.FormattedValue"',
    };
    protected readonly _guidRegex= /([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})/;
    protected readonly _postRequestHeaders: any = {
        "Content-Type": "application/json",
    };
    public readonly _columns:string[];

    constructor(entityname:string, columns: string[], factory:IFactory, keyfield: string = "")
    {
        this._entityName = entityname;
        this._entitySetName = entityname + 's';
        this._keyField = keyfield != "" ? keyfield : entityname + 'id';
        this._columns = columns;
        this._factory = factory;
    }

    async remove(id:string)
    {
        const requestUrl = `${this._webApi}${this._entitySetName}(${id})`;

        const requestParams:RequestInit = {
            method: "DELETE",
            headers: this._postRequestHeaders
        };

        const response = await fetch(requestUrl, requestParams);
        
        if(response.ok)
        {
            return {IsSuccess:true}
        }
        else{
            const respjson = await response.json();
            return {IsSuccess:false, Message: respjson?.error?.message}
        }
    }
    
    async create(entity:any)
    {
        const requestUrl = `${this._webApi}${this._entitySetName}`;

        const requestParams: RequestInit = {
            method: "POST",
            headers: this._postRequestHeaders,
            body: JSON.stringify(entity)
        };

        const response = await fetch(requestUrl, requestParams);
        
        if(!response.ok)
        {
         const respjson = await response.json();
         throw new Error(respjson?.message || respjson?.error?.message || "Record not Created");
        }

        const entityUrl = response.headers.get("odata-entityid");
        if (entityUrl) {
            const matches = entityUrl.match(this._guidRegex);
            if (matches && matches.length > 0)
                return matches[0];
        }

        return "";
    }

    async update(entity:any): Promise<void> {
        const requestUrl = `${this._webApi}${this._entitySetName}(${entity[this._keyField]})`;

        entity = _.omit(entity, this._keyField);
        entity = _.omit(entity, "Part");

        const requestParams: RequestInit = {
            method: "PATCH",
            headers: this._postRequestHeaders,
            body: JSON.stringify(entity)
        };

       var response = await fetch(requestUrl, requestParams);

       if(!response.ok)
       {
        const respjson = await response.json();
        throw new Error(respjson?.message || respjson?.error?.message || "Record not Updated");
       }
    }

    async retrieve(query:string): Promise<any[]>
    {
        const requestUrl = `${this._webApi}${this._entitySetName}?${query}`;

        const requestParams:RequestInit = {
            method: "GET",
            headers: this._getRequestHeaders
        };

        const response = await fetch(requestUrl, requestParams);
        const entityCollection = await response.json() as IEntityCollection;

        return (entityCollection.value || []).map((record) => this._factory.getFromWebApi(record));
    }

    async retrieveentity(entityCollectionName:string, query:string): Promise<any[]>
    {
        const requestUrl = `${this._webApi}${entityCollectionName}?${query}`;

        const requestParams:RequestInit = {
            method: "GET",
            headers: this._getRequestHeaders
        };

        const response = await fetch(requestUrl, requestParams);
        const entityCollection = await response.json() as IEntityCollection;

        return (entityCollection.value || []).map((record) => entityCollectionName.indexOf(this._entityName) > -1 ? this._factory.getFromWebApi(record) : record);
    }

    async retrieveWithCount(query:string): Promise<any>
    {
        const requestUrl = `${this._webApi}${this._entitySetName}?${query}`;

        const requestParams:RequestInit = {
            method: "GET",
            headers: this._getRequestHeaders
        };

        const response = await fetch(requestUrl, requestParams);
        const entityCollection = await response.json() as IEntityCollection;

        return {
            "@odata.count": entityCollection["@odata.count"],
            value: (entityCollection.value || []).map((record) => this._factory.getFromWebApi(record))
        };
    }

    async retrieveById(id:string, query:string=''):Promise<any>
    {
        query = query || `$select=${this._columns.join(",")}`;

        const requestUrl = `${this._webApi}${this._entitySetName}(${id})?${query}`;

        const requestParams:RequestInit = {
            method: "GET",
            headers: this._getRequestHeaders
        };

        const response = await fetch(requestUrl, requestParams);

        return this._factory.getFromWebApi(await response.json());
    }

    async getPartsByView(view: GridView, caseId: string):Promise<any[]>
    {
        return Promise.resolve([]);
    }

    async isNoFacilityManufacturerRelationship(incidentid?:string, facilityId?:string):Promise<boolean>
    {
        if(!facilityId)
        {
            return true;
        }
        
        const result = await this.retrieveentity("ipg_casepartdetails", encodeURI(`fetchXml=<fetch top="1" >
        <entity name="ipg_casepartdetail" >
          <filter>
            <condition attribute="ipg_caseid" operator="eq" value="${incidentid}" />
            <condition entityname="ipg_facilitymanufacturerrelationship" attribute="ipg_facilitymanufacturerrelationshipid" operator="null" />
            <condition attribute="ipg_potypecode" operator="in" >
              <value>923720001</value>
              <value>923720004</value>
              <value>923720000</value>
            </condition>
          </filter>
          <link-entity name="ipg_facilitymanufacturerrelationship" from="ipg_manufacturerid" to="ipg_manufacturerid" link-type="outer" alias="ipg_facilitymanufacturerrelationship" >
            <filter>
              <condition attribute="ipg_facilityid" operator="eq" value="${facilityId}" />
            </filter>
          </link-entity>
        </entity>
      </fetch>`));

        return (result?.length || 0) > 0;
    }

    async isOrderExistForPart(partid:string):Promise<boolean>
    {
        const part = await this.retrieveById(partid,`$select=_ipg_purchaseorderid_value`);

        return part._ipg_purchaseorderid_value != null;
    }
}