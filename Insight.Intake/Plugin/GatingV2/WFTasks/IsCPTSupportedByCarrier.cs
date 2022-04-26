using Insight.Intake.Plugin.GatingV2.CommonWfTask;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.GatingV2.WFTasks
{
    public class IsCPTSupportedByCarrier : WFTaskBase
    {
        private readonly string[] cptCodes =
        {
            Incident.Fields.ipg_CPTCodeId1,
            Incident.Fields.ipg_CPTCodeId2,
            Incident.Fields.ipg_CPTCodeId3,
            Incident.Fields.ipg_CPTCodeId4,
            Incident.Fields.ipg_CPTCodeId5,
            Incident.Fields.ipg_CPTCodeId6
        };
        public override WFTaskResult Run(WFTaskContext ctx)
        {
            // return new WFTaskResult(ctx.dbContext.Case.ipg_casehold != true);

            var service = ctx.CrmService;
            var tracingService = ctx.TraceService;
            var dbcase = ctx.dbContext.Case;

            if (dbcase.ipg_CPTCodeId1 == null && dbcase.ipg_CPTCodeId2 == null && dbcase.ipg_CPTCodeId2 == null && dbcase.ipg_CPTCodeId4 == null && dbcase.ipg_CPTCodeId5 == null && dbcase.ipg_CPTCodeId6 == null)
            {
                return new WFTaskResult(true);
            }
            else
            {
                bool supportedCPTExists = false;
                var notSupportedCpts = new List<string>();
                foreach (var cptCode in cptCodes)
                {
                    if (dbcase.Contains(cptCode))
                    {
                        var cptRef = dbcase.GetAttributeValue<EntityReference>(cptCode);
                        if (CptSupported(service, dbcase.ipg_CarrierId, cptRef, dbcase.ipg_SurgeryDate))
                        {
                            supportedCPTExists = true;
                            break;
                        }
                        else
                        {
                            notSupportedCpts.Add(cptRef.Name);
                        }
                    }
                }

                if (!supportedCPTExists)
                {
                    var result = new WFTaskResult(false);
                    var caseNote = string.Join(",", notSupportedCpts.Select(x => string.Format("CPT {0} is not supported by {1}.Case rejected and communication sent to the facility.", x, dbcase.ipg_CarrierId?.Name)));
                    result.CaseNote = caseNote;
                    //Soumitra: I noticed the CIM name was not pulled here in the portal note. Needed to include gatemanager into this plugin.
                    var cim = dbcase.ipg_FacilityId_Entity?.ipg_FacilityCimId_Entity;
                    var portalNote = string.Format(@"IPG has rejected this case due to the patient’s procedure {0} not being supported by the patient's health plan {1}. If the physician uses a covered implant, please notify your CIM, {2}, for reconsideration of this case.", string.Join(",", notSupportedCpts), dbcase.ipg_CarrierId?.Name, cim?.FullName);
                    result.PortalNote = portalNote;
                    return result;
                }
                else
                {
                    return new WFTaskResult(true);
                }
            }

        }
        private bool CptSupported(IOrganizationService service, EntityReference carrierRef, EntityReference cptRef, DateTime? dos)
        {
            if (!dos.HasValue)
                return false;
            QueryExpression query = new QueryExpression(ipg_associatedcpt.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(false)
            };
            query.Criteria.AddCondition(ipg_associatedcpt.Fields.StateCode, ConditionOperator.Equal, 0);
            query.Criteria.AddCondition(ipg_associatedcpt.Fields.ipg_CarrierId, ConditionOperator.Equal, carrierRef.Id);
            query.Criteria.AddCondition(ipg_associatedcpt.Fields.ipg_CPTCodeId, ConditionOperator.Equal, cptRef.Id);
            query.Criteria.AddCondition(ipg_associatedcpt.Fields.ipg_EffectiveDate, ConditionOperator.LessEqual, dos.Value);
            query.Criteria.AddCondition(ipg_associatedcpt.Fields.ipg_ExpirationDate, ConditionOperator.GreaterEqual, dos.Value);

            return service.RetrieveMultiple(query).Entities.Count > 0 ? true : false;
        }
    }
}
