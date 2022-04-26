using Insight.Intake.Plugin.Common;
using System;

namespace Insight.Intake.Plugin.CasePartDetail
{
    public class CasePartValidationPlugin: PluginBase
    {
        public CasePartValidationPlugin() : base(typeof(CasePartValidationPlugin))
        {
            RegisterEvent(PipelineStages.PreValidation, "Update", ipg_casepartdetail.EntityLogicalName, PreValidationHandler);
            RegisterEvent(PipelineStages.PreValidation, "Create", ipg_casepartdetail.EntityLogicalName, PreValidationHandler);
        }

        private void PreValidationHandler(LocalPluginContext context)
        {
            var target = context.Target<ipg_casepartdetail>();
            var preImage = context.PreImage<ipg_casepartdetail>();
            CasePartValidator validator = new CasePartValidator(target, preImage, context.OrganizationService,context.TracingService);
            Validate(validator);           
        }

        /// <summary>
        /// Check that MP part can't be created for ZPO
        /// </summary>
        private void Validate(CasePartValidator casePartValidator)
        {
            if (casePartValidator.IsDuplicate()) throw new Exception("Part with same product already exists!");
            if (!casePartValidator.IsPOTypeValid()) throw new Exception("PO Type not Valid!");
            if (!casePartValidator.IsValidAsPerCarrierPart()) throw new Exception("Part is not billable as per carrier part mapping.");
            if (!casePartValidator.IsValidAsPerPartId()) throw new Exception("Part is not billable as per chargecenter.");
            if (!casePartValidator.IsValidAsPerPartHCPCS()) throw new Exception("Part is not billable as per HCPCS.");
            if (!casePartValidator.IsPOTypeChangeValid()) throw new Exception("You cannot Modify PO type as Parent PO is not Open!");
        }
    }
}
