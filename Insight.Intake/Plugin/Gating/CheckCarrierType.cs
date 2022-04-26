using Insight.Intake.Extensions;
using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckCarrierType : GatingPluginBase
    {
        public CheckCarrierType() : base("ipg_IPGGatingCheckCarrierType") { }
        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx = null)
        {
            return new GatingResponse(!CheckCarrierTypeRules(targetRef));
        }

        private bool CheckCarrierTypeRules(EntityReference targetRef)
        {
            var targetPrimaryIdName = targetRef.LogicalName == Incident.EntityLogicalName
                ? Incident.PrimaryIdAttribute
                : ipg_referral.PrimaryIdAttribute;
            var target = crmService.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet(ipg_referral.Fields.ipg_CarrierId, Incident.Fields.ipg_SurgeryDate));
            var carrierRef = target.GetAttributeValue<EntityReference>(ipg_referral.Fields.ipg_CarrierId);
            if (carrierRef == null)
            {
                return false;
            }
            var carrier = crmService.Retrieve(carrierRef.LogicalName, carrierRef.Id, new ColumnSet(Intake.Account.Fields.ipg_CarrierType))
                .ToEntity<Intake.Account>();
            var dos = target.GetCaseDos();
            if (carrier.ipg_CarrierType == null || dos == null)
            {
                return true;
            }

            var query = new QueryExpression()
            {
                EntityName = ipg_carriertyperule.EntityLogicalName,
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression(ipg_carriertyperule.Fields.ipg_effectivedate, ConditionOperator.OnOrAfter, dos),
                        new ConditionExpression(ipg_carriertyperule.Fields.ipg_expirationdate, ConditionOperator.OnOrBefore, dos),
                        new ConditionExpression(ipg_carriertyperule.Fields.ipg_CarrierTypeNew, ConditionOperator.Equal, carrier.ipg_CarrierType.Value),
                        new ConditionExpression(ipg_carriertyperule.Fields.StateCode, ConditionOperator.Equal, (int)ipg_carriertyperuleState.Active),
                    }
                }
            };

            var authRules = crmService.RetrieveMultiple(query).Entities;
            return authRules.Any();
        }
    }
}
