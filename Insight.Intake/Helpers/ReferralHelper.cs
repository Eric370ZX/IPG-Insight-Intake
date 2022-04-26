using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Helpers
{
    internal static class ReferralHelper
    {
        public static SystemUser GetFacilityMDM(ipg_referral referral, IOrganizationService orgService)
        {
            if (referral.ipg_FacilityId?.Id == Guid.Empty)
            {
                throw new Exception("Referral should have a Facility Reference");
            }

            var fetchXml = $@"
                <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' top='1'>
                  <entity name='{SystemUser.EntityLogicalName}'>
                    <attribute name='{SystemUser.Fields.FullName}' />
                    <link-entity name='{Intake.Account.EntityLogicalName}' from='{Intake.Account.Fields.ipg_FacilityCimId}' to='{SystemUser.Fields.Id}' link-type='inner' alias='al'>
                      <link-entity name='{ipg_referral.EntityLogicalName}' from='{ipg_referral.Fields.ipg_FacilityId}' to='{Intake.Account.Fields.Id}' link-type='inner' alias='am'>
                        <filter type='and'>
                          <condition attribute='{ipg_referral.Fields.Id}' operator='eq' value='{referral.Id}' />
                        </filter>
                      </link-entity>
                    </link-entity>
                  </entity>
                </fetch>";

            return orgService.RetrieveMultiple(new FetchExpression(fetchXml)).Entities?.Select(e => e.ToEntity<SystemUser>()).FirstOrDefault();
        }

        public static ipg_ReferralType CalculateReferralType(DateTime dos, DateTime createdDate)
        {
            dos = dos.ToLocalTime().Date;
            var nextBusinessDay = createdDate.ToLocalTime().Date;

            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                nextBusinessDay = nextBusinessDay.AddDays(2);
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                nextBusinessDay = nextBusinessDay.AddDays(1);
            }

            if (dos < nextBusinessDay)
            {
                if ((nextBusinessDay - dos).TotalDays >= 1)
                {
                    return ipg_ReferralType.Retro;
                }
                else
                {
                    return ipg_ReferralType.Standard;
                }
            }
            else
            {
                if ((dos - nextBusinessDay).TotalHours <= 24)
                {
                    return ipg_ReferralType.Stat;
                }
                else if ((dos - nextBusinessDay).TotalHours <= 48)
                {
                    return ipg_ReferralType.Urgent;
                }
                else
                {
                    return ipg_ReferralType.Standard;
                }

            }
        }
    }
}
