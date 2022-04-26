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
    public class CheckPreAuthCarrierPreCertCPT : GatingPluginBase
    {
        public CheckPreAuthCarrierPreCertCPT() : base("ipg_IPGGatingCheckPreAuthCarrierPreCertCPT") { }
        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx = null)
        {
            var columns = new[] {
                ipg_referral.Fields.ipg_CarrierId,
                ipg_referral.Fields.ipg_SurgeryDate,
                ipg_referral.Fields.ipg_actualdos,
                ipg_referral.Fields.ipg_CPTCodeId1,
                ipg_referral.Fields.ipg_CPTCodeId2,
                ipg_referral.Fields.ipg_CPTCodeId3,
                ipg_referral.Fields.ipg_CPTCodeId4,
                ipg_referral.Fields.ipg_CPTCodeId5,
                ipg_referral.Fields.ipg_CPTCodeId6};
            var target = ctx.SystemOrganizationService.Retrieve(ctx.TargetRef().LogicalName, ctx.TargetRef().Id, new ColumnSet(columns));

            var dos = target.GetCaseDos();
            if (dos == null)
            {
                return new GatingResponse(false, "DOS is empty");
            }
            if (target.GetAttributeValue<EntityReference>(ipg_referral.Fields.ipg_CarrierId) == null)
            {
                return new GatingResponse(false, "Carrier is empty");
            }

            var hasAnyValidPrecertCPT = GetValidCarrierPrecertCPT(ctx.SystemOrganizationService, dos.Value, target.GetAttributeValue<EntityReference>(ipg_referral.Fields.ipg_CarrierId).Id, target)
                ?.Any() ?? false;

            if (hasAnyValidPrecertCPT)
            {
                return new GatingResponse(true);
            }
            return new GatingResponse(false);

        }

        private IEnumerable<Entity> GetValidCarrierPrecertCPT(IOrganizationService service, DateTime dos, Guid carrierId, Entity target)
        {
            var cptCodes = new[] {
                ipg_referral.Fields.ipg_CPTCodeId1,
                ipg_referral.Fields.ipg_CPTCodeId2,
                ipg_referral.Fields.ipg_CPTCodeId3,
                ipg_referral.Fields.ipg_CPTCodeId4,
                ipg_referral.Fields.ipg_CPTCodeId5,
                ipg_referral.Fields.ipg_CPTCodeId6};

            var hasAtLeastOneCPT = cptCodes.Any(p => target.GetAttributeValue<EntityReference>(p) != null);
            if (!hasAtLeastOneCPT) {
                return null;
            }

            var cptCriteria = string.Join("",
                    cptCodes
                    .Where(p => target.GetAttributeValue<EntityReference>(p) != null)
                    .Select(p => $"<value>{target.GetAttributeValue<EntityReference>(p).Id}</value>")
                );
            var fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='ipg_carrierprecertcpt'>
                                <attribute name='ipg_carrierprecertcptid' />
                                <attribute name='ipg_name' />
                                <attribute name='createdon' />
                                <order attribute='ipg_name' descending='false' />
                                <filter type='and'>
                                  <condition attribute='ipg_cptid' operator='in'>
                                    {cptCriteria}
                                  </condition>
                                  <condition attribute='ipg_carrierid' operator='eq' value='{carrierId}' />
                                  <condition attribute='ipg_effectivestartdate' operator='on-or-before' value='{dos}' />
                                  <condition attribute='ipg_effectiveenddate' operator='on-or-after' value='{dos}' />
                                  <condition attribute='statecode' operator='eq' value='0' />
                                </filter>
                              </entity>
                            </fetch>";

            var result = service.RetrieveMultiple(new FetchExpression(fetch));
            return result.Entities;
        }
    }
}

