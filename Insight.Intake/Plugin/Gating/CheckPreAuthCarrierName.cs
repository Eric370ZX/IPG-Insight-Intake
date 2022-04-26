using Insight.Intake.Extensions;
using Insight.Intake.Helpers;
using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckPreAuthCarrierName : GatingPluginBase
    {
        public CheckPreAuthCarrierName() : base("ipg_IPGGatingCheckPreAuthCarrierName") { }
        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx = null)
        {

            var target = ctx.SystemOrganizationService.Retrieve(ctx.TargetRef().LogicalName, ctx.TargetRef().Id, new ColumnSet(ipg_referral.Fields.ipg_CarrierId, ipg_referral.Fields.ipg_SurgeryDate, ipg_referral.Fields.ipg_actualdos));
            var carrier = target.GetAttributeValue<EntityReference>(ipg_referral.Fields.ipg_CarrierId);
            var dos = target.GetCaseDos();
            var carrierRules = GetCarriernameRules(carrier?.Id, dos, ctx.SystemOrganizationService);
            if (carrierRules.Any())
            {
                return new GatingResponse(false);
            }
            return new GatingResponse(true);
        }
        public IEnumerable<ipg_carriernamerule> GetCarriernameRules(Guid? carrierId, DateTime? dos, IOrganizationService service)
        {
            if (carrierId == null || dos == null)
            {
                return new List<ipg_carriernamerule>();
            }
            var query = new QueryExpression()
            {
                NoLock = true,
                EntityName = ipg_carriernamerule.EntityLogicalName,
                Criteria = new FilterExpression()
                {
                    Conditions = {
                        new ConditionExpression(ipg_carriernamerule.Fields.StateCode, ConditionOperator.Equal, (int)ipg_carriernameruleState.Active),
                        new ConditionExpression(ipg_carriernamerule.Fields.ipg_CarrierId, ConditionOperator.Equal, carrierId),
                        new ConditionExpression(ipg_carriernamerule.Fields.ipg_effectivedate, ConditionOperator.LessEqual, dos),
                        new ConditionExpression(ipg_carriernamerule.Fields.ipg_expirationdate, ConditionOperator.GreaterEqual, dos),
                    },

                }
            };
            var entities = service.RetrieveMultiple(query).Entities;
            return entities.Select(p => p.ToEntity<ipg_carriernamerule>());
        }
    }
}
