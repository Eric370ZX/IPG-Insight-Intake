export interface IOpenPortalCommentViewModel {
    id:string;
    RegardingId?: string;
    RegardingIdName?: string;
    RegardingIdEntity?: string;
    FacillityId?: string;
    FacillityIdName?:string;
    createdon?:Date
    description?: string;
    messagestatus?: string;
    casestatus?: string;
}