using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;

namespace Insight.Intake.Plugin.Gating
{
    public class CheckDMEBenefitsJQUPrefix : PluginBase
    {
        public CheckDMEBenefitsJQUPrefix() : base(typeof(CheckDMEBenefitsJQUPrefix))
        {
            RegisterEvent(PipelineStages.PostOperation, "ipg_IPGGatingCheckDMEBenefitsJQUPrefix", null, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            var service = localPluginContext.OrganizationService;
            EntityReference targetRef = context.InputParameters["Target"] as EntityReference;

            context.OutputParameters["Succeeded"] = false;
            context.OutputParameters["CaseNote"] = string.Empty;
            context.OutputParameters["PortalNote"] = string.Empty;

            var requiredFields = new[] { Incident.Fields.ipg_deductiblemet, Incident.Fields.ipg_deductibleremaining,
                Incident.Fields.ipg_oopmax,Incident.Fields.ipg_oopmet,Incident.Fields.ipg_patientcoinsurance , Incident.Fields.ipg_payercoinsurance };
            var columns = new List<string>() { Incident.Fields.ipg_BenefitTypeCode, Incident.Fields.ipg_MemberIdNumber };
            columns.AddRange(requiredFields);
            var initTarget = service.Retrieve(targetRef.LogicalName, targetRef.Id, new ColumnSet(columns.ToArray()));
            var memberIdNumber = initTarget.GetAttributeValue<string>(Incident.Fields.ipg_MemberIdNumber) ?? "";


            if (!memberIdNumber.ToUpper().StartsWith("JQU"))
            {
                context.OutputParameters["Succeeded"] = true;
                context.OutputParameters["CodeOutput"] = 1;
                return;
            }
            var benefitTypeCode = initTarget.GetAttributeValue<OptionSetValue>(Incident.Fields.ipg_BenefitTypeCode)?.Value ?? -1;
            ipg_BenefitType? benefitType = benefitTypeCode != -1 ? (ipg_BenefitType?)benefitTypeCode : null;
            if (benefitType != ipg_BenefitType.DurableMedicalEquipment_DME)
            {
                context.OutputParameters["Succeeded"] = false;
                return;
            }

            context.OutputParameters["Succeeded"] = true;
            context.OutputParameters["CodeOutput"] = 2;
        }
    }
}
