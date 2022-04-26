using Insight.Intake.Extensions;
using Insight.Intake.Plugin.GatingV2.CommonWfTask;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insight.Intake.Plugin.GatingV2.WFTasks
{
    public class ValidateDxCode : WFTaskBase
    {
        public override WFTaskResult Run(WFTaskContext ctx)
        {
            var service = ctx.CrmService;
            var tracingService = ctx.TraceService;
            var dbCase = ctx.dbContext.Case;
            var dos = dbCase.GetCaseDos();
            //Run validation on each Dx code.
            var dxCode1GatingResponse = ValidateIndividualDxCodeOnCase(dbCase.ipg_DxCodeId1_Entity, dos);
            if (dxCode1GatingResponse.Succeeded) return dxCode1GatingResponse;

            var dxCode2GatingResponse = ValidateIndividualDxCodeOnCase(dbCase.ipg_DxCodeId2_Entity, dos);
            if (dxCode2GatingResponse.Succeeded) return dxCode2GatingResponse;

            var dxCode3GatingResponse = ValidateIndividualDxCodeOnCase(dbCase.ipg_DxCodeId3_Entity, dos);
            if (dxCode3GatingResponse.Succeeded) return dxCode3GatingResponse;

            var dxCode4GatingResponse = ValidateIndividualDxCodeOnCase(dbCase.ipg_DxCodeId4_Entity, dos);
            if (dxCode4GatingResponse.Succeeded) return dxCode4GatingResponse;

            var dxCode5GatingResponse = ValidateIndividualDxCodeOnCase(dbCase.ipg_DxCodeId5_Entity, dos);
            if (dxCode5GatingResponse.Succeeded) return dxCode5GatingResponse;

            var dxCode6GatingResponse = ValidateIndividualDxCodeOnCase(dbCase.ipg_DxCodeId6_Entity, dos);
            if (dxCode6GatingResponse.Succeeded) return dxCode6GatingResponse;

            //If the above code could not find a valid Dx code, return the message below.
            if (dbCase.ipg_ActualDOS != null && dbCase.ipg_ActualDOS <= DateTime.Now)
            {
                if (dbCase.ipg_DxCodeId1 == null)
                {
                    return new WFTaskResult(false, "No Valid Dx on Case", "Missing Dx Code on Case");
                }
                else
                {
                    var dxList = new List<EntityReference>();
                    if (dbCase.ipg_DxCodeId1 != null) dxList.Add(dbCase.ipg_DxCodeId1);
                    if (dbCase.ipg_DxCodeId2 != null) dxList.Add(dbCase.ipg_DxCodeId2);
                    if (dbCase.ipg_DxCodeId3 != null) dxList.Add(dbCase.ipg_DxCodeId3);
                    if (dbCase.ipg_DxCodeId4 != null) dxList.Add(dbCase.ipg_DxCodeId4);
                    if (dbCase.ipg_DxCodeId5 != null) dxList.Add(dbCase.ipg_DxCodeId5);
                    if (dbCase.ipg_DxCodeId6 != null) dxList.Add(dbCase.ipg_DxCodeId6);
                    return new WFTaskResult(false, "No Valid Dx on Case", $"Dx { string.Join(",", dxList.Select(x => x.Name)) } is not active for DOS {dos}. Please provide an updated Dx code.");
                }
            }
            return new WFTaskResult(false, "No Valid Dx on Case");
        }

        private WFTaskResult ValidateIndividualDxCodeOnCase(ipg_dxcode dxCode, DateTime? dos)
        {
            if (dxCode == null)
                return new WFTaskResult(false);
            //Concatenate effective dxcode field name with date field to fetch the related dx code effective and expiration date.
            var effectiveDate = dxCode.ipg_EffectiveDate ?? DateTime.MinValue;
            var expirationDate = dxCode.ipg_ExpirationDate ?? DateTime.MinValue;

            if (dxCode.ipg_name != null)
            {
                if (dxCode.ipg_name.ToString().ToLower().Contains("unspecified"))
                {
                    return new WFTaskResult(false);
                }
            }
            //Although effective date should not be null since its a required field, some dx code migrated from legacy might have
            //this issue. Just treat them as bad dx code.          

            if (effectiveDate != DateTime.MinValue && dos >= effectiveDate && (!dxCode.ipg_ExpirationDate.HasValue || (dos <= expirationDate)))
            {
                return new WFTaskResult(true, "Dx Active and Valid");
            }
            return new WFTaskResult(false);
        }
    }
}
