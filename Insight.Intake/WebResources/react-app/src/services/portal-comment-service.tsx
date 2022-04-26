import { BaseService } from "./base-service";
import {PortalCommentfactory} from "../factories/portal-comment-factory"
import _, { forEach } from "lodash";
import { threadId } from "node:worker_threads";

export class PortalCommentService extends BaseService {
    constructor() {
        super("adx_portalcomment"
        , [
            "_regardingobjectid_value", "_ipg_facilityid_value", "description", "statuscode",
            "createdon","adx_portalcommentid", "adx_portalcommentid"],
            new PortalCommentfactory(),
            "activityid"
        );    
    }

    async getOpenPortalComments(): Promise<any> {
      return await super.retrieveWithCount("$select=_ipg_facilityid_value,statuscode,description,createdon,activityid&$expand=regardingobjectid_incident_adx_portalcomment($select=title,ipg_casestatus),regardingobjectid_ipg_referral_adx_portalcomment($select=ipg_referralcasenumber,ipg_casestatus)&$filter=ipg_referralcreated ne true and (adx_portalcommentdirectioncode eq 1 and statecode eq 0 and statuscode eq 923720000 and (regardingobjectid_incident_adx_portalcomment/title ne null or regardingobjectid_ipg_referral_adx_portalcomment/ipg_referralnumber ne null))&$orderby=createdon desc");
    }

    async markCommentAsRead(commentsids:string[]): Promise<void> {
        for(let i = 0; i < commentsids.length; i++)
        {
            await this.update({
                "activityid": commentsids[i],
                statuscode: 427880000, //Viewed
                statecode: 1,
                ipg_markedread: true
            });
        }
    }
}
