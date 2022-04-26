using Insight.Intake.Extensions;
using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckCarrierNetworkCPTRules : GatingPluginBase
    {
        public CheckCarrierNetworkCPTRules() : base("ipg_IPGGatingCheckCarrierNetworkCPTRules") { }

        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx)
        {
            var positiveResponse = new GatingResponse(true, "Carrier Network CPT Does Not Require Auth");
            var negativeResponse = new GatingResponse(false, "Carrier Network CPT Requires Auth");

            var target = crmService.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet(
                Incident.Fields.ipg_ActualDOS,
                Incident.Fields.ipg_SurgeryDate,
                Incident.Fields.ipg_CPTCodeId1,
                Incident.Fields.ipg_CPTCodeId2,
                Incident.Fields.ipg_CPTCodeId3,
                Incident.Fields.ipg_CPTCodeId4,
                Incident.Fields.ipg_CPTCodeId5,
                Incident.Fields.ipg_CPTCodeId6,
                Incident.Fields.ipg_CarrierId));

            var dos = target.GetCaseDos();
            var cptCodes = (new[] {
                target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_CPTCodeId1),
                target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_CPTCodeId2),
                target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_CPTCodeId3),
                target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_CPTCodeId4),
                target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_CPTCodeId5),
                target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_CPTCodeId6),
            }).Where(p => p != null);
            if (!cptCodes.Any())
            {
                return new GatingResponse(false, $"There is no CPT codes on the case/referral");
            }

            if (target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_CarrierId) == null || dos == null)
            {
                return new GatingResponse(false, $"Carrier Network CPT Requires Authю Carrier: {target.GetAttributeValue<EntityReference>(Incident.Fields.ipg_CarrierId)}, DOS: {dos}");
            }

            var carrier = crmService.Retrieve(Intake.Account.EntityLogicalName,
                                              target.GetAttributeValue<EntityReference>(ipg_referral.Fields.ipg_CarrierId).Id,
                                              new ColumnSet(Intake.Account.Fields.ipg_carriernetworkid))?.
                                              ToEntity<Intake.Account>();
            if (carrier != null && carrier.ipg_carriernetworkid != null && IsCarrierNetworkCPTRuleIsValid(carrier.ipg_carriernetworkid.Id, dos,cptCodes, carrier))
            {
                return positiveResponse;
            }
            return negativeResponse;
        }

        private bool IsCarrierNetworkCPTRuleIsValid(Guid carrierNetworkId, DateTime? dos, System.Collections.Generic.IEnumerable<EntityReference> cptCodes, Intake.Account carrier)
        {
            var isCarrierNetworkCPTRuleInvalid = crmService.RetrieveMultiple(new QueryExpression(ipg_carriernetworkcptrule.EntityLogicalName)
            {
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_carriernetworkcptrule.Fields.ipg_CarrierNetworksId,
                                                ConditionOperator.Equal,
                                                carrier.GetAttributeValue<EntityReference>(Intake.Account.Fields.ipg_carriernetworkid).Id),
                        new ConditionExpression(ipg_carriernetworkcptrule.Fields.ipg_CptCodeId, ConditionOperator.In, cptCodes.Select(p=>p.Id).ToArray()),
                        new ConditionExpression(ipg_carriernetworkcptrule.Fields.ipg_effectivedate, ConditionOperator.LessEqual, dos),
                        new ConditionExpression(ipg_carriernetworkcptrule.Fields.ipg_expirationdate, ConditionOperator.GreaterEqual, dos)
                    }
                }
            }).Entities.Any();
            return !isCarrierNetworkCPTRuleInvalid;
        }
    }
}