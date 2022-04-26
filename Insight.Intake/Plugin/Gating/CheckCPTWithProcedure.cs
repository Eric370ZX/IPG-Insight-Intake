using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckCPTWithProcedure : GatingPluginBase
    {
        public CheckCPTWithProcedure() : base("ipg_IPGGatingCheckCPTWithProcedure")
        {
        }

        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext context)
        {
            var target = crmService.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet("ipg_cptcodeid1", "ipg_procedurenameid", "ipg_facilityid"));
            var cptWithProcedureManager = new CPTWithProcedureManager(target);
            if (cptWithProcedureManager.IsCptCode1Provided)
            {
                return GetResponse(true, "");
            }
            if (cptWithProcedureManager.IsProcedureNameProvided)
            {
                var facilityCPTProcedureCode = RetrieveFacilityCPTProcedureCode(cptWithProcedureManager.Target.FacilityId, cptWithProcedureManager.Target.ProcedureNameId);
                if (facilityCPTProcedureCode != null)
                {
                    if (facilityCPTProcedureCode.ipg_CptCodeId != null)
                    {
                        var targetManager = new TargetManaget(crmService);
                        targetManager.SetTargetCptCode1(target.ToEntityReference(), facilityCPTProcedureCode.ipg_CptCodeId);
                        return GetResponse(true, "");
                    }
                    else
                    {
                        return GetResponse(false, "No Implant per Procedure");
                    }
                }
                else
                {
                    return GetResponse(false, "CPT Not Provided");
                }
            }
            else
            {
                return GetResponse(false, "Procedure name and CPT code are not provided");
            }
        }

        private ipg_facilitycptprocedurecode RetrieveFacilityCPTProcedureCode(EntityReference facilityRef, EntityReference procedureRef)
        {
            if (facilityRef == null && procedureRef == null)
            {
                return null;
            }
            var query = new QueryExpression
            {
                EntityName = ipg_facilitycptprocedurecode.EntityLogicalName,
                ColumnSet = new ColumnSet("ipg_cptcodeid"),
                TopCount = 1,
                Criteria = {
                    Conditions = {
                        new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                        new ConditionExpression("ipg_facilityid", ConditionOperator.Equal, facilityRef?.Id),
                        new ConditionExpression("ipg_proceduredescription", ConditionOperator.Equal, procedureRef?.Name),
                    }
                },
            };
            var result = crmService.RetrieveMultiple(query).Entities;
            return result.Any()
                ? crmService.RetrieveMultiple(query).Entities.First().ToEntity<ipg_facilitycptprocedurecode>()
                : null;
        }

        private GatingResponse GetResponse(bool succeeded, string note) => new GatingResponse
        {
            Succeeded = succeeded,
            CaseNote = note,
            PortalNote = note
        };
    }

    class CPTWithProcedureManager
    {
        internal UnifiedTarget Target { get; set; }
        public CPTWithProcedureManager(Entity target)
        {
            switch (target.LogicalName)
            {
                case Incident.EntityLogicalName:
                    var incident = target.ToEntity<Incident>();
                    Target = new UnifiedTarget
                    {
                        CptCode1 = incident.ipg_CPTCodeId1,
                        ProcedureNameId = incident.ipg_procedureid,
                        FacilityId = incident.ipg_FacilityId,
                    };
                    break;
                case ipg_referral.EntityLogicalName:
                    var referral = target.ToEntity<ipg_referral>();
                    Target = new UnifiedTarget
                    {
                        CptCode1 = referral.ipg_CPTCodeId1,
                        ProcedureNameId = referral.ipg_ProcedureNameId,
                        FacilityId = referral.ipg_FacilityId,
                    };
                    break;
            }
        }
        internal bool IsCptCode1Provided => (Target.CptCode1 != null);
        internal bool IsProcedureNameProvided => (Target.ProcedureNameId != null);
    }

    class TargetManaget
    {
        IOrganizationService Service { get; set; }
        public TargetManaget(IOrganizationService service)
        {
            Service = service;
        }
        internal void SetTargetCptCode1(EntityReference targetRef, EntityReference cptCodeRef)
        {
            var target = new Entity
            {
                LogicalName = targetRef.LogicalName,
                Id = targetRef.Id,
            };
            target["ipg_cptcodeid1"] = cptCodeRef;
            Service.Update(target);

        }
    }

    class UnifiedTarget
    {
        internal EntityReference CptCode1;
        internal EntityReference ProcedureNameId;
        internal EntityReference FacilityId;
    }
}
