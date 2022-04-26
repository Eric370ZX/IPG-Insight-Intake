import { PartFactory } from "../factories/part-factory";
import { IPart } from "../models/IPart";
import { IPartCollection } from "../models/webapi/IPartCollection";
import { ISearchValues } from "../models/ISearchValues";
import { BaseService } from "./base-service";

export class PartService extends BaseService {
  constructor() {
    super("product",
      [
        "ipg_manufacturerpartnumber", "name", "ipg_partnumber", "_ipg_manufacturerid_value", "productid",
        "ipg_boxquantity", "ipg_claimtier", "ipg_msrp", "ipg_averageinvoicecost", "description",
        "ipg_effectivedate", "ipg_discontinuedon", "ipg_ipgpartnumber",
        "ipg_maxquantity", "ipg_enforcemaxquantity", "ipg_manufacturerdiscountpricemoney"
      ],
      new PartFactory()
    )
  }

  async getActive(dos?: Date): Promise<IPart[]> {
    const select = `$select=${this._columns.join(",")}`;
    let filter = `$filter=statecode eq 0 and (ipg_activeproduct ne 427880000)`;

    if (dos) {
      var dosStr = dos.toISOString();

      filter = filter + ` and ((ipg_effectivedate eq null or ipg_effectivedate le ${dosStr}) and (ipg_discontinuedon eq null or ipg_discontinuedon ge ${dosStr}))`;
    }

    const top = `$top=100`;

    return super.retrieve(`${select}&${filter}&${top}`);
  }

  /**
   * Function searches for top 50 active parts comparing string parameter to a part number, a part description and manufacturer using contains operator
   * @param searchClause Search clause
   */
  async search(searchClause: string, dos?: Date): Promise<IPart[]> {
    const select = `$select=${this._columns.join(",")}`;
    let filter = `$filter=statecode eq 0 and (contains(ipg_partnumber, '${searchClause}') or contains(name, '${searchClause}') or 
                            contains(ipg_manufacturerid/name, '${searchClause}') or contains(ipg_manufacturerpartnumber, '${searchClause}'))`;
    if (dos) {
      var dosStr = dos.toISOString();
      filter = filter + ` and ((ipg_effectivedate eq null or ipg_effectivedate le ${dosStr}) and (ipg_discontinuedon eq null or ipg_discontinuedon ge ${dosStr}))`;
    }

    const top = `$top=100`;

    return super.retrieve(`${select}&${filter}&${top}`);
  }

  async advancedSearch(searchValues: ISearchValues, count: number, page: number): Promise<IPartCollection> {
    let filterConditions = "";
    if (searchValues.category)
      filterConditions += `<condition attribute='producttypecode' operator='eq' value='${searchValues.category}' />`;
    if (searchValues.hcpcs)
      filterConditions += `<condition attribute='ipg_hcpcscodeidname' operator='like' value='%${searchValues.hcpcs}%' />`;
    if (searchValues.keyword)
      filterConditions += `<condition attribute='ipg_ipgpartnumber' operator='eq' value='${searchValues.keyword}' />`;
    if (searchValues.manufacturerName)
      filterConditions += `<condition attribute='ipg_manufactureridname' operator='like' value='%${searchValues.manufacturerName}%' />`;
    if (searchValues.partDescription)
      filterConditions += `<condition attribute='description' operator='like' value='%${searchValues.partDescription}%' />`;
    if (searchValues.partNumber)
      filterConditions += `<condition attribute='ipg_manufacturerpartnumber' operator='like' value='%${searchValues.partNumber}%' />`;

    const fetchXml =
      `<fetch count='${count || 5000}' distinct='true' no-lock='true' returntotalrecordcount='true' page='${page || 1}'>
                <entity name='${this._entityName}' >
                    <attribute name='productid' />
                    <attribute name='ipg_manufacturerpartnumber' />
                    <attribute name='name' />
                    <attribute name='ipg_manufacturerid' />
                    <attribute name='ipg_hcpcscodeid' />
                    <attribute name='ipg_ipgpartnumber' />
                    <attribute name='ipg_boxquantity' />
                    <attribute name='ipg_maxquantity' />
                    <attribute name='ipg_enforcemaxquantity' />                    
                    <attribute name='producttypecode' />
                    <attribute name='ipg_effectivedate' />
                    <attribute name='ipg_discontinuedon' />
                    <attribute name='ipg_claimtier' />
                    <order attribute='createdon' descending='true' />
                    <filter type='and' >
                        <condition attribute='ipg_activeproduct' operator='neq' value='427880000' />
                        <condition attribute='statecode' operator='eq' value='0' />${filterConditions}
                    </filter>
                </entity>
            </fetch>`;

    return super.retrieveWithCount(`fetchXml=${encodeURI(fetchXml)}`);
  }

  isWithinEffectiveDate(dateOfSurgery: Date, part: IPart): boolean {
    if (part.ipg_effectivedate && part.ipg_discontinuedon) {
      return dateOfSurgery >= part.ipg_effectivedate && dateOfSurgery <= part.ipg_discontinuedon;
    }
    else if (part.ipg_effectivedate) {
      return dateOfSurgery >= part.ipg_effectivedate;
    }
    else if (part.ipg_discontinuedon) {
      return dateOfSurgery <= part.ipg_discontinuedon
    }

    return true;
  }

  async IsValidByCarrier(dos: Date, partid: string, carrierid: string): Promise<Boolean> {
    var dosstring = dos.toISOString();

    var result = await super.retrieveentity('ipg_carrierparts', `$top=1&$select=ipg_carrierpartid&$filter=(_ipg_carrierid_value eq ${carrierid} and _ipg_partid_value eq ${partid} and ipg_effectivedate le ${dosstring} and (ipg_expirationdate eq null or ipg_expirationdate ge ${dosstring}))`);
    return !(result.length > 0);
  }

  async IsValidAsPerPartId(dos: Date, partid: string, carrierid: string): Promise<boolean> {
    var dosstring = dos.toISOString();
    var fetchXml = [
      "<fetch top='1'>",
      "  <entity name='ipg_chargecenter'>",
      "    <attribute name='ipg_supported' />",
      "    <filter>",
      "      <condition attribute='ipg_productid' operator='eq' value='", partid, "'/>",
      "      <condition attribute='ipg_effectivedate' operator='le' value='", dosstring, "'/>",
      "      <condition attribute='ipg_expirationdate' operator='ge' value='", dosstring, "'/>",
      "      <condition entityname='ipg_carrierfeeschedule' attribute='ipg_carrierid' operator='eq' value='", carrierid, "' />",
      "      <condition entityname='ipg_carrierfeeschedule' attribute='ipg_effectivedate' operator='le' value='", dosstring, "' />",
      "      <condition entityname='ipg_carrierfeeschedule' attribute='ipg_expiredate' operator='ge' value='", dosstring, "' />",
      "    </filter>",
      "    <link-entity name='ipg_feeschedule' from='ipg_feescheduleid' to='ipg_feescheduleid' link-type='inner'>",
      "      <link-entity name='ipg_carrierfeeschedule' from='ipg_feescheduleid' to='ipg_feescheduleid' link-type='inner' />",
      "    </link-entity>",
      "  </entity>",
      "</fetch>",
    ].join("");

    var result = await super.retrieveentity('ipg_chargecenters', `fetchXml=${encodeURI(fetchXml)}`);

    if (result.length > 0 && !result[0].ipg_supported) {
      return false;
    }
    else {
      return true;
    }
  }

  async IsValidAsPerPartHCPCS(dos: Date, productid: string, carrierid: string): Promise<boolean> {
    var dosstring = dos.toISOString();


    var fetchXml = [
      "<fetch top='1'>",
      "  <entity name='ipg_chargecenter'>",
      "    <attribute name='ipg_supported' />",
      "    <filter>",
      "      <condition attribute='ipg_effectivedate' operator='le' value='", dosstring, "'/>",
      "      <condition attribute='ipg_expirationdate' operator='ge' value='", dosstring, "'/>",
      "    </filter>",
      "    <link-entity name='ipg_feeschedule' from='ipg_feescheduleid' to='ipg_feescheduleid' link-type='inner'>",
      "      <link-entity name='ipg_carrierfeeschedule' from='ipg_feescheduleid' to='ipg_feescheduleid' link-type='inner'>",
      "        <filter>",
      "          <condition attribute='ipg_effectivedate' operator='le' value='", dosstring, "'/>",
      "          <condition attribute='ipg_expiredate' operator='ge' value='", dosstring, "'/>",
      "          <condition attribute='ipg_carrierid' operator='eq' value='", carrierid, "' />",
      "        </filter>",
      "      </link-entity>",
      "    </link-entity>",
      "    <link-entity name='ipg_masterhcpcs' from='ipg_masterhcpcsid' to='ipg_hcpcscodeid' link-type='inner'>",
      "      <link-entity name='product' from='ipg_hcpcscodeid' to='ipg_masterhcpcsid' link-type='inner'>",
      "        <filter>",
      "          <condition attribute='productid' operator='eq' value='", productid, "' />",
      "        </filter>",
      "      </link-entity>",
      "    </link-entity>",
      "  </entity>",
      "</fetch>",
    ].join("");

    var result = await super.retrieveentity('ipg_chargecenters', `fetchXml=${encodeURI(fetchXml)}`);

    if (result.length > 0 && !result[0].ipg_supported) {
      return false;
    }
    else {
      return true;
    }
  }

  async IsPartBillable(dos: Date, partid: string, carrierid: string, homePlanCarrierId: string): Promise<boolean> {
    var carrierIdForCarrierPart = homePlanCarrierId;
    if (carrierIdForCarrierPart == null || carrierIdForCarrierPart === '') {
      carrierIdForCarrierPart = carrierid;
    }
    return await this.IsValidByCarrier(dos, partid, carrierIdForCarrierPart)
      && await this.IsValidAsPerPartId(dos, partid, carrierid)
      && await this.IsValidAsPerPartHCPCS(dos, partid, carrierid);
  }

  async CheckIsPartsBillable(dos: Date, parts: IPart[], carrierid: string, homePlanCarrierId: string): Promise<boolean> {
    for (let i = 0; i < parts.length; i++) {
      if (!this.IsPartBillable(dos, parts[i].id, carrierid, homePlanCarrierId)) {
        throw new Error(`Part ${parts[i].ipg_manufacturerpartnumber}, ${parts[i].name} is not billable!`);
      }
    }

    return true;
  }

  CheckIsPartsWithinEffictiveDate(dos: Date, parts: IPart[]): boolean {
    for (let i = 0; i < parts.length; i++) {
      if (!this.isWithinEffectiveDate(dos, parts[i])) {
        throw new Error(`Part ${parts[i].ipg_manufacturerpartnumber}, ${parts[i].name} cannot be added to the Case. Case DOS outside of the Part effective Date Range!`);
      }
    }

    return true;
  }
}
