import {IPortalComment} from "../models/IPortalComment";
import WebApiService from "../services/web-api-service";
import { IFactory } from "./factory";

export class PortalCommentfactory implements IFactory {
    getFromWebApi(record: any): IPortalComment {
        let pc: IPortalComment = record as IPortalComment;

        if (record.createdon)
        {
            pc.createdon = new Date(record.createdon);
        }

        pc.id = WebApiService.getAttributeValue(record, "activityid");
        
        if(record.regardingobjectid_incident_adx_portalcomment)
        {
            pc.RegardingId = 
            {
                id: record.regardingobjectid_incident_adx_portalcomment.incidentid,
                name: record.regardingobjectid_incident_adx_portalcomment.title,
                entityName: 'incident'
            };
            
            pc.casestatus = record.regardingobjectid_incident_adx_portalcomment.ipg_casestatus;

        }
        else if(record.regardingobjectid_ipg_referral_adx_portalcomment)
        {
            pc.RegardingId = 
            {
                id: record.regardingobjectid_ipg_referral_adx_portalcomment.ipg_referralid,
                name: record.regardingobjectid_ipg_referral_adx_portalcomment.ipg_referralcasenumber,
                entityName: 'ipg_referral'
            };
            pc.casestatus = record.regardingobjectid_ipg_referral_adx_portalcomment.ipg_casestatus;
        }

        pc.Facillity = WebApiService.getLookupValue(record, "ipg_facilityid", "account");

        return {
            id: pc.id, 
            Facillity: pc.Facillity, 
            RegardingId: pc.RegardingId,
            description: pc.description,
            createdon: pc.createdon, 
            casestatus:pc.casestatus,
            statuscode:pc.statuscode};
    }
}
