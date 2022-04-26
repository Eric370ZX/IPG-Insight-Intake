using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckPhysicianValue : GatingPluginBase
    {
        public CheckPhysicianValue() : base("ipg_IPGGatingCheckPhysicianValue") { }

        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx)
        {
            //if (ctx.TargetRef().LogicalName == ipg_referral.EntityLogicalName)
            //{// validation for referral moved to method in CheckReferralForm class
            //    return new GatingResponse(true);
            //}

            Guid? physicianId = null;
            Guid? facilityId = null;
            if (targetRef.LogicalName == Incident.EntityLogicalName)
            {
                var incident = crmService.Retrieve(targetRef.LogicalName, targetRef.Id,
                    new ColumnSet(nameof(Incident.ipg_PhysicianId).ToLower(), nameof(Incident.ipg_FacilityId).ToLower()))
                    .ToEntity<Incident>();
                physicianId = incident?.ipg_PhysicianId?.Id;
                facilityId = incident?.ipg_FacilityId?.Id;
            }else if (targetRef.LogicalName == ipg_referral.EntityLogicalName)
            {
                var referral = crmService.Retrieve(targetRef.LogicalName, targetRef.Id,
                    new ColumnSet(nameof(ipg_referral.ipg_PhysicianId).ToLower(), nameof(ipg_referral.ipg_FacilityId).ToLower()))
                    .ToEntity<ipg_referral>();
                physicianId = referral?.ipg_PhysicianId?.Id;
                facilityId = referral?.ipg_FacilityId?.Id;
            }
            if (physicianId == null || facilityId == null)
                return new GatingResponse(false);
            var physician = crmService.Retrieve(Intake.Contact.EntityLogicalName, physicianId.Value,
                    new ColumnSet(nameof(Intake.Contact.StateCode).ToLower(), nameof(Intake.Contact.StateCode).ToLower(), Intake.Contact.Fields.ipg_approved))
                    .ToEntity<Intake.Contact>();
            if (physician.StateCode != ContactState.Active || physician.ipg_approved != true)
                return new GatingResponse(false);
            var relateQuery = new QueryExpression(ipg_facilityphysician.EntityLogicalName)
            {
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_facilityphysician.Fields.ipg_facilityid, ConditionOperator.Equal, facilityId.Value),
                        new ConditionExpression(ipg_facilityphysician.Fields.ipg_physicianid, ConditionOperator.Equal, physicianId.Value),
                        new ConditionExpression(ipg_facilityphysician.Fields.ipg_status, ConditionOperator.Equal, true)
                    }
                }
            };
            if (!crmService.RetrieveMultiple(relateQuery).Entities.Any())
                return new GatingResponse(false);
            return new GatingResponse(true);
        }
    }
}