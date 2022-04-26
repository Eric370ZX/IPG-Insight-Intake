using Insight.Intake.Repositories;
using Microsoft.Xrm.Sdk;
using System.Linq;

namespace Insight.Intake.Plugin.Document
{
    //Deprecated
    //TODO: Remove From D365 and Assembly
    public class PreventMultiplePifsPerReferralCase : PluginBase
    {
        public PreventMultiplePifsPerReferralCase() : base(typeof(PreventMultiplePifsPerReferralCase))
        {
            RegisterEvent(
                PipelineStages.PreOperation,
                "Create",
                ipg_document.EntityLogicalName,
                OnCreate);

            RegisterEvent(
                PipelineStages.PreOperation,
                "Update",
                ipg_document.EntityLogicalName,
                OnUpdate);
        }

        private void OnUpdate(LocalPluginContext pluginContext)
        {
            using (CrmServiceContext context = new CrmServiceContext(pluginContext.OrganizationService))
            {
                ipg_document target = (pluginContext.PluginExecutionContext.InputParameters["Target"] as Entity).ToEntity<ipg_document>();
                ipg_document document = context.ipg_documentSet.FirstOrDefault(x => x.Id == target.Id);

                EntityReference documentType = target.ipg_DocumentTypeId ?? document.ipg_DocumentTypeId;

                ipg_documenttype pifType = new DocumentTypeRepository(pluginContext.OrganizationService)
                    .GetByAbbreviation("PIF");

                if (documentType != null && documentType.Id == pifType.Id)
                {
                    if (target.ipg_CaseId != null)
                    {
                        ipg_document existingCasePif = context.ipg_documentSet.FirstOrDefault(x =>
                            x.ipg_documentId.Value != target.Id &&
                            x.ipg_DocumentTypeId != null && x.ipg_DocumentTypeId.Id == pifType.Id &&
                            x.ipg_CaseId != null && x.ipg_CaseId.Id == target.ipg_CaseId.Id);

                        if (existingCasePif != null)
                        {
                            throw new InvalidPluginExecutionException("Only one PIF per case is allowed");
                        }
                    }

                    if (target.ipg_ReferralId != null)
                    {
                        ipg_document existingReferralPif = context.ipg_documentSet.FirstOrDefault(x =>
                            x.ipg_documentId.Value != target.Id &&
                            x.ipg_DocumentTypeId != null && x.ipg_DocumentTypeId.Id == pifType.Id &&
                            x.ipg_ReferralId != null && x.ipg_ReferralId.Id == target.ipg_ReferralId.Id);

                        if (existingReferralPif != null)
                        {
                            throw new InvalidPluginExecutionException("Only one PIF per referral is allowed");
                        }
                    }
                }
            }
        }

        private void OnCreate(LocalPluginContext pluginContext)
        {
            ipg_document target = (pluginContext.PluginExecutionContext.InputParameters["Target"] as Entity).ToEntity<ipg_document>();

            ipg_documenttype pifType = new DocumentTypeRepository(pluginContext.OrganizationService)
                .GetByAbbreviation("PIF");

            if (target.ipg_DocumentTypeId != null && target.ipg_DocumentTypeId.Id == pifType.Id)
            {
                using (CrmServiceContext context = new CrmServiceContext(pluginContext.OrganizationService))
                {
                    if (target.ipg_CaseId != null)
                    {
                        ipg_document existingCasePif = context.ipg_documentSet.FirstOrDefault(x =>
                            x.ipg_DocumentTypeId != null && x.ipg_DocumentTypeId.Id == pifType.Id &&
                            x.ipg_CaseId != null && x.ipg_CaseId.Id == target.ipg_CaseId.Id);

                        if (existingCasePif != null)
                        {
                            throw new InvalidPluginExecutionException("Only one PIF per case is allowed");
                        }
                    }

                    if (target.ipg_ReferralId != null)
                    {
                        ipg_document existingReferralPif = context.ipg_documentSet.FirstOrDefault(x =>
                            x.ipg_DocumentTypeId != null && x.ipg_DocumentTypeId.Id == pifType.Id &&
                            x.ipg_ReferralId != null && x.ipg_ReferralId.Id == target.ipg_ReferralId.Id);

                        if (existingReferralPif != null)
                        {
                            throw new InvalidPluginExecutionException("Only one PIF per referral is allowed");
                        }
                    }
                }
            }
        }

    }
}
