using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;
namespace Insight.Intake.Plugin.PortalComment
{
    public class AssignRecordToFacilityCaseManagerPlugin : PluginBase
    {
        private IOrganizationService service;
        public AssignRecordToFacilityCaseManagerPlugin() : base(typeof(AssignRecordToFacilityCaseManagerPlugin))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, adx_portalcomment.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            service = localPluginContext.OrganizationService;
            var comment = localPluginContext.Target<adx_portalcomment>();
            if (comment.RegardingObjectId != null)
            {
                string fetch = null;
                string facilityAttr = null;
                if (comment.RegardingObjectId.LogicalName == Incident.EntityLogicalName)
                {
                    fetch = GetCaseFetch(comment.RegardingObjectId.Id.ToString());
                    facilityAttr = Incident.Fields.ipg_FacilityId;
                }
                if (comment.RegardingObjectId.LogicalName == ipg_referral.EntityLogicalName)
                {
                    fetch = GetReferralFetch(comment.RegardingObjectId.Id.ToString());
                    facilityAttr = ipg_referral.Fields.ipg_FacilityId;
                }
                if (!string.IsNullOrEmpty(fetch))
                {
                    var regardingObject = service.RetrieveMultiple(new FetchExpression(fetch)).Entities.FirstOrDefault();

                    if (regardingObject != null && regardingObject.Contains(facilityAttr) && !string.IsNullOrEmpty(facilityAttr))
                    {
                        comment.ipg_FacilityId = regardingObject.GetAttributeValue<EntityReference>(facilityAttr);
                    }

                    if (regardingObject != null && regardingObject.Contains($"{Intake.Account.EntityLogicalName}.{Intake.Account.Fields.ipg_FacilityCaseMgrId}"))
                    {
                        comment.ipg_assignedto = regardingObject
                             .GetAttributeValue<AliasedValue>($"{Intake.Account.EntityLogicalName}.{Intake.Account.Fields.ipg_FacilityCaseMgrId}")?.Value as EntityReference;
                    }

                    service.Update(comment);
                }
            }
        }

        private string GetCaseFetch(string caseId)
        {
            return $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' >
                          <entity name='{Incident.EntityLogicalName}' >
                              <attribute name='{Incident.Fields.ipg_FacilityId}' />
                              <filter type='and' >
                                  <condition attribute='{Incident.Fields.Id}' operator='eq' value='{caseId}' />
                              </filter>
                              <link-entity name='{Intake.Account.EntityLogicalName}' from='{Intake.Account.Fields.AccountId}' to='{Incident.Fields.ipg_FacilityId}' link-type='inner' alias='{Intake.Account.EntityLogicalName}' >
                                   <attribute name='{Intake.Account.Fields.ipg_FacilityCaseMgrId}' />
                              </link-entity>
                          </entity>
                      </fetch>";
        }
        private string GetReferralFetch(string referralId)
        {
            return $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' >
                          <entity name='{ipg_referral.EntityLogicalName}' >
                              <attribute name='{ipg_referral.Fields.ipg_FacilityId}' />
                              <filter type='and' >
                                  <condition attribute='{ipg_referral.Fields.Id}' operator='eq' value='{referralId}' />
                              </filter>
                             <link-entity name='{Intake.Account.EntityLogicalName}' from='{Intake.Account.Fields.AccountId}' to='{ipg_referral.Fields.ipg_FacilityId}' link-type='inner' alias='{Intake.Account.EntityLogicalName}' >
                                  <attribute name='{Intake.Account.Fields.ipg_FacilityCaseMgrId}' />
                              </link-entity>
                          </entity>
                      </fetch>";
        }
    }
}