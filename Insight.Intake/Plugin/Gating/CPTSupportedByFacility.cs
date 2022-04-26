using Insight.Intake.Extensions;
using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Gating
{
    /// <summary>
    /// Deprecated. logic is moved to CptCodesValidation
    /// </summary>
    [Obsolete]
    public class CPTSupportedByFacility : GatingPluginBase
    {
        public CPTSupportedByFacility() : base("ipg_IPGGatingCPTSupportedByFacility") { }

        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx)
        {
            if (targetRef.LogicalName == ipg_referral.EntityLogicalName)
            {
                return ValidateForReferral(gateManager);
            }
            else
            {
                return ValidateForIncident(gateManager);
            }
        }

        private GatingResponse ValidateForReferral(GateManager gateManager)
        {
            var referral = crmService.Retrieve(ipg_referral.EntityLogicalName, targetRef.Id, new ColumnSet(
                LogicalNameof<ipg_referral>.Property(x => x.ipg_CPTCodeId1),
                LogicalNameof<ipg_referral>.Property(x => x.ipg_CPTCodeId2),
                LogicalNameof<ipg_referral>.Property(x => x.ipg_CPTCodeId3),
                LogicalNameof<ipg_referral>.Property(x => x.ipg_CPTCodeId4),
                LogicalNameof<ipg_referral>.Property(x => x.ipg_CPTCodeId5),
                LogicalNameof<ipg_referral>.Property(x => x.ipg_CPTCodeId6),
                LogicalNameof<ipg_referral>.Property(x => x.ipg_FacilityId),
                LogicalNameof<ipg_referral>.Property(x => x.ipg_SurgeryDate))).ToEntity<ipg_referral>();

            List<Guid> cptIdList = new List<Guid>();

            if (referral.ipg_CPTCodeId1 != null) cptIdList.Add(referral.ipg_CPTCodeId1.Id);
            if (referral.ipg_CPTCodeId2 != null) cptIdList.Add(referral.ipg_CPTCodeId2.Id);
            if (referral.ipg_CPTCodeId3 != null) cptIdList.Add(referral.ipg_CPTCodeId3.Id);
            if (referral.ipg_CPTCodeId4 != null) cptIdList.Add(referral.ipg_CPTCodeId4.Id);
            if (referral.ipg_CPTCodeId5 != null) cptIdList.Add(referral.ipg_CPTCodeId5.Id);
            if (referral.ipg_CPTCodeId6 != null) cptIdList.Add(referral.ipg_CPTCodeId6.Id);

            if (cptIdList.Count > 0)
            {
                QueryExpression query = new QueryExpression();
                query.EntityName = ipg_facilitycpt.EntityLogicalName;
                query.ColumnSet = new ColumnSet(LogicalNameof<ipg_facilitycpt>.Property(x => x.ipg_FacilityId),
                                                LogicalNameof<ipg_facilitycpt>.Property(x => x.ipg_CptCodeId));
                query.Criteria.AddCondition(LogicalNameof<ipg_facilitycpt>.Property(x => x.ipg_FacilityId), ConditionOperator.Equal, referral.ipg_FacilityId.Id);
                query.Criteria.AddCondition(LogicalNameof<ipg_facilitycpt>.Property(x => x.ipg_CptCodeId), ConditionOperator.In, cptIdList.ConvertAll<String>(x => x.ToString()).ToArray<String>());
                query.Criteria.AddCondition(LogicalNameof<ipg_facilitycpt>.Property(x => x.ipg_EffectiveDate), ConditionOperator.LessEqual, referral.ipg_SurgeryDate);
                query.Criteria.AddCondition(LogicalNameof<ipg_facilitycpt>.Property(x => x.ipg_ExpirationDate), ConditionOperator.GreaterEqual, referral.ipg_SurgeryDate);
                var queryResponse = crmService.RetrieveMultiple(query);
                if (queryResponse.Entities.Any())
                {
                    var cptName = string.Join(",", queryResponse.Entities.Select(x => x.ToEntity<ipg_facilitycpt>().ipg_CptCodeId.Name));
                    var facilityName = referral.ipg_FacilityId.Name;
                    var cim = gateManager.GetFacilityCIM(referral.ipg_FacilityId.Id);
                    return new GatingResponse(false, "No CPTs Supported by Facility.", $"IPG has rejected this case due to the patient’s procedure {cptName} not being supported by the facility, {facilityName}. If the physician uses a covered implant, please notify your CIM, {(cim != null ? cim.FullName : "")}, for reconsideration of this case.");
                }
            }

            return new GatingResponse(true, "CPT supported by the Facility.");
        }
        
        private GatingResponse ValidateForIncident(GateManager gateManager)
        {
            var incident = crmService.Retrieve(Incident.EntityLogicalName, targetRef.Id, new ColumnSet(
                LogicalNameof<Incident>.Property(x => x.ipg_CPTCodeId1),
                LogicalNameof<Incident>.Property(x => x.ipg_CPTCodeId2),
                LogicalNameof<Incident>.Property(x => x.ipg_CPTCodeId3),
                LogicalNameof<Incident>.Property(x => x.ipg_CPTCodeId4),
                LogicalNameof<Incident>.Property(x => x.ipg_CPTCodeId5),
                LogicalNameof<Incident>.Property(x => x.ipg_CPTCodeId6),
                LogicalNameof<Incident>.Property(x => x.ipg_FacilityId),
                LogicalNameof<Incident>.Property(x => x.ipg_SurgeryDate),
                LogicalNameof<Incident>.Property(x => x.ipg_ActualDOS))).ToEntity<Incident>();

            List<Guid> cptIdList = new List<Guid>();

            if (incident.ipg_CPTCodeId1 != null) cptIdList.Add(incident.ipg_CPTCodeId1.Id);
            if (incident.ipg_CPTCodeId2 != null) cptIdList.Add(incident.ipg_CPTCodeId2.Id);
            if (incident.ipg_CPTCodeId3 != null) cptIdList.Add(incident.ipg_CPTCodeId3.Id);
            if (incident.ipg_CPTCodeId4 != null) cptIdList.Add(incident.ipg_CPTCodeId4.Id);
            if (incident.ipg_CPTCodeId5 != null) cptIdList.Add(incident.ipg_CPTCodeId5.Id);
            if (incident.ipg_CPTCodeId6 != null) cptIdList.Add(incident.ipg_CPTCodeId6.Id);

            if (cptIdList.Count > 0)
            {
                QueryExpression query = new QueryExpression();
                query.EntityName = ipg_facilitycpt.EntityLogicalName;
                query.ColumnSet = new ColumnSet(LogicalNameof<ipg_facilitycpt>.Property(x => x.ipg_FacilityId),
                                                LogicalNameof<ipg_facilitycpt>.Property(x => x.ipg_CptCodeId));
            
                query.Criteria.AddCondition(LogicalNameof<ipg_facilitycpt>.Property(x => x.ipg_FacilityId), ConditionOperator.Equal, incident.ipg_FacilityId.Id);
                query.Criteria.AddCondition(LogicalNameof<ipg_facilitycpt>.Property(x => x.ipg_CptCodeId), ConditionOperator.In, cptIdList.ConvertAll<String>(x => x.ToString()).ToArray<String>());
                query.Criteria.AddCondition(LogicalNameof<ipg_facilitycpt>.Property(x => x.ipg_EffectiveDate), ConditionOperator.LessEqual, incident.GetCaseDos());
                query.Criteria.AddCondition(LogicalNameof<ipg_facilitycpt>.Property(x => x.ipg_ExpirationDate), ConditionOperator.GreaterEqual, incident.GetCaseDos());

                var queryResponse = crmService.RetrieveMultiple(query);
                if (queryResponse.Entities.Any())
                {
                    var cptName = string.Join(",", queryResponse.Entities.Select(x => x.ToEntity<ipg_facilitycpt>().ipg_CptCodeId.Name));
                    var facilityName = incident.ipg_FacilityId.Name;
                    var cim = gateManager.GetFacilityCIM(incident.ipg_FacilityId.Id);
                    return new GatingResponse(false, "No CPTs Supported by Facility.", $"IPG has rejected this case due to the patient’s procedure {cptName} not being supported by the facility, {facilityName}. If the physician uses a covered implant, please notify your CIM, {cim?.FullName}, for reconsideration of this case.");
                }
            }
            return new GatingResponse(true, "CPT supported by the Facility.");
        }
    }
}
