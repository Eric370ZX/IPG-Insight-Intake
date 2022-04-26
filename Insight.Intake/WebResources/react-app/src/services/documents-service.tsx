import { DocumentFactory } from "../factories/document-factory";
import { IDocumentCollection } from "../models/webapi/IDocumentCollection";
import { ICase } from "../models/ICase";
import { POType } from "../enums/po-types"
import { BaseService } from "./base-service";

export class DocumentsService extends BaseService {
    
  constructor(){
      super("ipg_document",
      [
          "ipg_documentid"
      ], new DocumentFactory()
      )
  }


    async getManufacturerInvoicesByProduct(incident:ICase, productId: string, count: number, page: number): Promise<IDocumentCollection> {
        const facilitiid = incident.ipg_FacilityId?.accountid;
        const dos = new Date(incident.ipg_actualdos || incident.ipg_surgerydate);
        dos.setMonth(dos.getMonth() - 6);

        const stringDos6MonthBehind = `${dos.getFullYear()}-${dos.getMonth() +1}-${dos.getDate()}`;
        const manufacturerInvoiceDocType = `E89B45D3-9348-22D9-A871-48F34BD1912A`;

        const fetchXml =
            `<fetch count="10" distinct="true" no-lock="true" returntotalrecordcount="true" page="1" >
              <entity name="ipg_document" >
              <attribute name="ipg_name" />
              <attribute name="createdon" />
              <attribute name="ipg_documentid" />
              <order attribute="createdon" descending="true" />
              <filter type="and" >
                <condition attribute="statecode" operator="eq" value="0" />
                <condition attribute="ipg_documenttypeid" operator="eq" value="${manufacturerInvoiceDocType}" />
                <condition attribute="createdon" operator="le" value="${stringDos6MonthBehind}" />
              </filter>
              <link-entity name="incident" from="incidentid" to="ipg_caseid" link-type="inner" alias="as" >
                <attribute name="title" />
                <attribute name="incidentid" />
                <attribute name="ipg_procedureid" />
                <filter type="and" >
                <condition attribute="ipg_facilityid" operator="eq" value="${facilitiid}" />
                </filter>
                <link-entity name="ipg_casepartdetail" from="ipg_caseid" to="incidentid" link-type="outer" alias="actualPart" >
                  <attribute name="ipg_purchaseorderid" />
                  <attribute name="ipg_enteredunitcost" />
                  <attribute name="ipg_quantity" />
                  <filter type="and" >
                    <condition attribute="ipg_productid" operator="eq" value="${productId}" />
                    <condition attribute="ipg_potypecode" operator="eq" value="${POType.CPA}" />
                    <condition attribute="statecode" operator="eq" value="0" />
                  </filter>
                </link-entity>
                <link-entity name="ipg_casepartdetail" from="ipg_caseid" to="incidentid" link-type="inner" alias="at" >
                  <filter type="and" >
                    <condition attribute="ipg_productid" operator="eq" value="${productId}" />
                    <condition attribute="statecode" operator="eq" value="0" />
                  </filter>
                </link-entity>
              </link-entity>
            <link-entity name="annotation" from="objectid" to="ipg_documentid" alias="annotation">
              <attribute name="annotationid" />
            </link-entity>        
            </entity>
          </fetch>`;

        const response = await super.retrieveWithCount(`fetchXml=${encodeURI(fetchXml)}`);

        return response as IDocumentCollection;
    }

    async associateWithActualPart(documentId: string, actualPartId: string): Promise<void> {
        documentId = documentId.replace("{", "").replace("}", "");
        actualPartId = actualPartId.replace("{", "").replace("}", "");

        const relationshipName = "ipg_ipg_casepartdetail_ipg_document";
        const requestUrl = `/api/data/v9.1/${this._entityName}s(${documentId})/${relationshipName}/$ref`;
        const host = window.location.host.includes("localhost") ? "insight-dev.crm.dynamics.com" : window.location.host;

        const requestBody = {
            "@odata.id": `https://${host}/api/data/v9.1/ipg_casepartdetails(${actualPartId})`
        };

        const requestParams: RequestInit = {
            method: "POST",
            headers: this._postRequestHeaders,
            body: JSON.stringify(requestBody)
        };

        await fetch(requestUrl, requestParams);
    }

    async associateWithCase(documentId: string, caseid?: string): Promise<void> {
      if(caseid)
      {
        documentId = documentId.replace("{", "").replace("}", "");
        caseid = caseid.replace("{", "").replace("}", "");

        const relationshipName = "ipg_incident_ipg_document";
        const requestUrl = `/api/data/v9.1/${this._entityName}s(${documentId})/${relationshipName}/$ref`;
        const host = window.location.host.includes("localhost") ? "insight-dev.crm.dynamics.com" : window.location.host;

        const requestBody = {
            "@odata.id": `https://${host}/api/data/v9.1/incidents(${caseid})`
        };

        const requestParams: RequestInit = {
            method: "POST",
            headers: this._postRequestHeaders,
            body: JSON.stringify(requestBody)
        };

        await fetch(requestUrl, requestParams);
      }
  }

    async getNoteFromDocument(documentid:string): Promise<string>
    {
      var result = await super.retrieveentity("annotations", `$select=annotationid&$filter=(_objectid_value eq ${documentid})&$orderby=_createdby_value desc&$top=1`);
      
      return result.length > 0 && result[0].annotationid;
    }
}