using Insight.Intake.Plugin.Common;
using System;

namespace Insight.Intake.Plugin.EstimatedCasePartDetail
{
    public class EstimatedCasePartValidationPlugin : PluginBase
    {
        public EstimatedCasePartValidationPlugin() : base(typeof(EstimatedCasePartValidationPlugin))
        {
            RegisterEvent(PipelineStages.PreValidation, "Update", ipg_estimatedcasepartdetail.EntityLogicalName, PreValidationHandler);
            RegisterEvent(PipelineStages.PreValidation, "Create", ipg_estimatedcasepartdetail.EntityLogicalName, PreValidationHandler);
        }

        private void PreValidationHandler(LocalPluginContext context)
        {
            var target = context.Target<ipg_estimatedcasepartdetail>();
            var preImage = context.PreImage<ipg_estimatedcasepartdetail>();
            CasePartValidator validator = new CasePartValidator(target, preImage, context.OrganizationService, context.TracingService);
            Validate(validator);
        }

        /// <summary>
        /// Check that MP part can't be created for ZPO
        /// </summary>
        private void Validate(CasePartValidator casePartValidator)
        {
            if (!casePartValidator.IsValidMultipackForZPO()) throw new Exception("ZPO can't have MultiPack Part!");
            if (!casePartValidator.IsValidAsPerCarrierPart()) throw new Exception("Part is not billable as per carrier part mapping.");
            if (!casePartValidator.IsValidAsPerPartId()) throw new Exception("Part is not billable as per chargecenter.");
            if (!casePartValidator.IsValidAsPerPartHCPCS()) throw new Exception("Part is not billable as per HCPCS.");
        }
    }
}
