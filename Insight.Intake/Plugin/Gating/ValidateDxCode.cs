using Insight.Intake.Extensions;
using Insight.Intake.Plugin.Gating.Common;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.Gating
{
    public class ValidateDxCode : GatingPluginBase
    {
        public ValidateDxCode() : base("ipg_IPGGatingValidateDxCode") { }

        public override GatingResponse Validate(GateManager gateManager, LocalPluginContext ctx) =>
            targetRef.LogicalName == Incident.EntityLogicalName ? ValidateDxCodeOnCaseIsActive() : ValidateDxCodeOnReferralIsActive();

        //Entry point of the plugin to validate active dx code on the case.
        private GatingResponse ValidateDxCodeOnCaseIsActive()
        {
            QueryExpression query = GetQueryToRetrieveTargetWithDxCodeEffectiveAndExpirationDate();

            var target = crmService.RetrieveMultiple(query).Entities.FirstOrDefault()?.ToEntity<Incident>();

            //Run validation on each Dx code.
            var dxCode1GatingResponse = ValidateIndividualDxCode(target, Incident.Fields.ipg_DxCodeId1);

            var dxCode2GatingResponse = ValidateIndividualDxCode(target, Incident.Fields.ipg_DxCodeId2);

            var dxCode3GatingResponse = ValidateIndividualDxCode(target, Incident.Fields.ipg_DxCodeId3);

            var dxCode4GatingResponse = ValidateIndividualDxCode(target, Incident.Fields.ipg_DxCodeId4);

            var dxCode5GatingResponse = ValidateIndividualDxCode(target, Incident.Fields.ipg_DxCodeId5);

            var dxCode6GatingResponse = ValidateIndividualDxCode(target, Incident.Fields.ipg_DxCodeId6);

            if (target.ipg_DxCodeId1 == null)
            {
                return new GatingResponse(false, "No Valid Dx on Case", "Missing Dx Code on Case");
            }

            var validationResults = new[] { dxCode1GatingResponse, dxCode2GatingResponse, dxCode2GatingResponse, dxCode3GatingResponse, dxCode4GatingResponse, dxCode5GatingResponse, dxCode6GatingResponse };
            if (validationResults.All(p => p.Succeeded))
                return new GatingResponse(true);

            var dxList = new List<EntityReference>();
            if (target.ipg_DxCodeId1 != null) dxList.Add(target.ipg_DxCodeId1);
            if (target.ipg_DxCodeId2 != null) dxList.Add(target.ipg_DxCodeId2);
            if (target.ipg_DxCodeId3 != null) dxList.Add(target.ipg_DxCodeId3);
            if (target.ipg_DxCodeId4 != null) dxList.Add(target.ipg_DxCodeId4);
            if (target.ipg_DxCodeId5 != null) dxList.Add(target.ipg_DxCodeId5);
            if (target.ipg_DxCodeId6 != null) dxList.Add(target.ipg_DxCodeId6);
            return new GatingResponse(false, "No Valid Dx on Case", $"Dx { string.Join(",", dxList.Select(x => x.Name)) } is not active for DOS {target.GetCaseDos()}. Please provide an updated Dx code.")
            {
                CodeOutput = 1
            };
        }

        private GatingResponse ValidateDxCodeOnReferralIsActive()
        {
            QueryExpression query = GetQueryToRetrieveTargetWithDxCodeEffectiveAndExpirationDate();

            var target = crmService.RetrieveMultiple(query).Entities.FirstOrDefault()?.ToEntity<ipg_referral>();

            //Run validation on each Dx code.
            var dxCode1GatingResponse = ValidateIndividualDxCode(target, ipg_referral.Fields.ipg_DxCodeId1);
            if (dxCode1GatingResponse.Succeeded && target.Attributes.Contains(ipg_referral.Fields.ipg_DxCodeId1)) return dxCode1GatingResponse;

            var dxCode2GatingResponse = ValidateIndividualDxCode(target, ipg_referral.Fields.ipg_DxCodeId2);
            if (dxCode2GatingResponse.Succeeded && target.Attributes.Contains(ipg_referral.Fields.ipg_DxCodeId2)) return dxCode2GatingResponse;

            var dxCode3GatingResponse = ValidateIndividualDxCode(target, ipg_referral.Fields.ipg_DxCodeId3);
            if (dxCode3GatingResponse.Succeeded && target.Attributes.Contains(ipg_referral.Fields.ipg_DxCodeId3)) return dxCode3GatingResponse;

            var dxCode4GatingResponse = ValidateIndividualDxCode(target, ipg_referral.Fields.ipg_dxcodeid4);
            if (dxCode4GatingResponse.Succeeded && target.Attributes.Contains(ipg_referral.Fields.ipg_dxcodeid4)) return dxCode4GatingResponse;

            var dxCode5GatingResponse = ValidateIndividualDxCode(target, ipg_referral.Fields.ipg_dxcodeid5);
            if (dxCode5GatingResponse.Succeeded && target.Attributes.Contains(ipg_referral.Fields.ipg_dxcodeid5)) return dxCode5GatingResponse;

            var dxCode6GatingResponse = ValidateIndividualDxCode(target, ipg_referral.Fields.ipg_dxcodeid6);
            if (dxCode6GatingResponse.Succeeded && target.Attributes.Contains(ipg_referral.Fields.ipg_dxcodeid6)) return dxCode6GatingResponse;

            //If the above code could not find a valid Dx code, return the message below.
            if (target.ipg_SurgeryDate != null && target.ipg_SurgeryDate <= DateTime.Now)
            {
                if (target.ipg_DxCodeId1 == null)
                {
                    return new GatingResponse(false, "No Valid Dx on Referral", "Missing Dx Code on Referral");
                }
                else
                {
                    var dxList = new List<EntityReference>();
                    if (target.ipg_DxCodeId1 != null) dxList.Add(target.ipg_DxCodeId1);
                    if (target.ipg_DxCodeId2 != null) dxList.Add(target.ipg_DxCodeId2);
                    if (target.ipg_DxCodeId3 != null) dxList.Add(target.ipg_DxCodeId3);
                    if (target.ipg_dxcodeid4 != null) dxList.Add(target.ipg_dxcodeid4);
                    if (target.ipg_dxcodeid5 != null) dxList.Add(target.ipg_dxcodeid5);
                    if (target.ipg_dxcodeid6 != null) dxList.Add(target.ipg_dxcodeid6);
                    return new GatingResponse(false, "No Valid Dx on Referral", $"Dx { string.Join(",", dxList.Select(x => x.Name)) } is not active for DOS {target.GetCaseDos()}. Please provide an updated Dx code.")
                    {
                        CodeOutput = 1
                    };
                }
            }
            return new GatingResponse(false, "No Valid Dx on Referral");
        }

        private GatingResponse ValidateIndividualDxCode(Entity target, string dxCodeAttibuteName)
        {
            if (target.Attributes.Contains(dxCodeAttibuteName))
            {
                var dos = target.GetCaseDos();
                //Concatenate effective dxcode field name with date field to fetch the related dx code effective and expiration date.
                var effectiveDateAttributeName = $"{dxCodeAttibuteName.Replace("id", string.Empty)}.{ipg_dxcode.Fields.ipg_EffectiveDate}";
                var expirationDateAttrbuteName = $"{dxCodeAttibuteName.Replace("id", string.Empty)}.{ipg_dxcode.Fields.ipg_ExpirationDate}";

                var dxCodeNameAttributeName = $"{dxCodeAttibuteName.Replace("id", string.Empty)}.{ipg_dxcode.Fields.ipg_name}";
                if (target.Attributes.Contains(dxCodeNameAttributeName))
                {
                    var dxName = target.GetAttributeValue<AliasedValue>(dxCodeNameAttributeName).Value;
                    if (dxName.ToString().ToLower().Contains("unspecified"))
                    {
                        return new GatingResponse(false);
                    }
                }


                //Although effective date should not be null since its a required field, some dx code migrated from legacy might have
                //this issue. Just treat them as bad dx code.
                DateTime effectiveDate = DateTime.MinValue;
                if (target.Attributes.Contains(effectiveDateAttributeName))
                {
                    effectiveDate = (DateTime)target.GetAttributeValue<AliasedValue>(effectiveDateAttributeName).Value;
                }

                DateTime expirationDate = DateTime.MinValue;
                if (target.Attributes.Contains(expirationDateAttrbuteName))
                {
                    expirationDate = (DateTime)target.GetAttributeValue<AliasedValue>(expirationDateAttrbuteName).Value;
                }

                if (effectiveDate != DateTime.MinValue && dos >= effectiveDate && (!target.Attributes.Contains(expirationDateAttrbuteName) || (dos <= expirationDate)))
                {
                    return new GatingResponse(true, "Dx Active and Valid");
                }
                return new GatingResponse(false);
            }
            return new GatingResponse(true);
        }

        private QueryExpression GetQueryToRetrieveTargetWithDxCodeEffectiveAndExpirationDate()
        {
            QueryExpression query = new QueryExpression();
            query.EntityName = targetRef.LogicalName;
            query.ColumnSet = new ColumnSet(Incident.Fields.ipg_SurgeryDate,
                                            Incident.Fields.ipg_DxCodeId1,
                                            Incident.Fields.ipg_DxCodeId2,
                                            Incident.Fields.ipg_DxCodeId3,
                                            Incident.Fields.ipg_DxCodeId4,
                                            Incident.Fields.ipg_DxCodeId5,
                                            Incident.Fields.ipg_DxCodeId6);

            //Add linked enity to fetch related dx code values. 
            if (targetRef.LogicalName == Incident.EntityLogicalName)
            {
                query.ColumnSet.AddColumn(Incident.Fields.ipg_ActualDOS);
            }

            query.LinkEntities.Add(GetLinkedEntityForDxCodeOnCase(Incident.Fields.ipg_DxCodeId1));
            query.LinkEntities.Add(GetLinkedEntityForDxCodeOnCase(Incident.Fields.ipg_DxCodeId2));
            query.LinkEntities.Add(GetLinkedEntityForDxCodeOnCase(Incident.Fields.ipg_DxCodeId3));
            query.LinkEntities.Add(GetLinkedEntityForDxCodeOnCase(Incident.Fields.ipg_DxCodeId4));
            query.LinkEntities.Add(GetLinkedEntityForDxCodeOnCase(Incident.Fields.ipg_DxCodeId5));
            query.LinkEntities.Add(GetLinkedEntityForDxCodeOnCase(Incident.Fields.ipg_DxCodeId6));
            query.Criteria.AddCondition(targetRef.LogicalName == Incident.EntityLogicalName ? Incident.PrimaryIdAttribute : ipg_referral.PrimaryIdAttribute, ConditionOperator.Equal, targetRef.Id);
            return query;
        }

        private LinkEntity GetLinkedEntityForDxCodeOnCase(string dxCodeAttributeNameOnCase)
        {
            var linkedDxCode = new LinkEntity(Incident.EntityLogicalName
                , ipg_dxcode.EntityLogicalName,
                dxCodeAttributeNameOnCase,
                LogicalNameof<ipg_dxcode>.Property(x => x.ipg_dxcodeId),
                JoinOperator.LeftOuter);
            linkedDxCode.Columns = new ColumnSet(LogicalNameof<ipg_dxcode>.Property(x => x.ipg_name),
                                                  LogicalNameof<ipg_dxcode>.Property(x => x.ipg_EffectiveDate),
                                                  LogicalNameof<ipg_dxcode>.Property(x => x.ipg_ExpirationDate));
            return linkedDxCode;
        }
    }
}
