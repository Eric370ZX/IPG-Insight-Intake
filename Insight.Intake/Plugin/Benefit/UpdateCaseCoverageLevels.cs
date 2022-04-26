using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Insight.Intake.Plugin.Benefit
{
    public class UpdateCaseCoverageLevels : PluginBase
    {
        public UpdateCaseCoverageLevels() : base(typeof(UpdateCaseCoverageLevels))
        {
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Update, ipg_benefit.EntityLogicalName, PostOperationHandler);
            RegisterEvent(PipelineStages.PostOperation, MessageNames.Create, ipg_benefit.EntityLogicalName, PostOperationHandler);
        }

        private void PostOperationHandler(LocalPluginContext localPluginContext)
        {
            var service = localPluginContext.OrganizationService;

            ipg_benefit benefit;
            if (localPluginContext.PluginExecutionContext.MessageName == MessageNames.Create)
            {
                localPluginContext.Trace("Getting Target");
                benefit = localPluginContext.Target<ipg_benefit>();
            }
            else if (localPluginContext.PluginExecutionContext.MessageName == MessageNames.Update)
            {
                localPluginContext.Trace("Getting PostImage");
                benefit = localPluginContext.PostImage<ipg_benefit>();
            }
            else
            {
                throw new InvalidPluginExecutionException("Unexpected message name: " + localPluginContext.PluginExecutionContext.MessageName);
            }

            localPluginContext.Trace("Check if CaseId is set");
            if (benefit.ipg_CaseId != null)
            {
                ipg_BenefitCoverageLevels? newDeductibleCoverageLevel = null;
                if (benefit.ipg_FamilyDeductibleRemainingCalculated != null
                    && benefit.ipg_DeductibleRemainingCalculated != null)
                {
                    if(benefit.ipg_FamilyDeductibleRemainingCalculated.Value < benefit.ipg_DeductibleRemainingCalculated.Value)
                    {
                        localPluginContext.Trace("Choose Family Deductible");
                        newDeductibleCoverageLevel = ipg_BenefitCoverageLevels.Family;
                    }
                    else
                    {
                        localPluginContext.Trace("Choose Individual Deductible");
                        newDeductibleCoverageLevel = ipg_BenefitCoverageLevels.Individual;
                    }
                }

                ipg_BenefitCoverageLevels? newOopCoverageLevel = null;
                if (benefit.ipg_FamilyOOPRemainingCalculated != null
                    && benefit.ipg_MemberOOPRemainingCalculated != null)
                {
                    if (benefit.ipg_FamilyOOPRemainingCalculated.Value < benefit.ipg_MemberOOPRemainingCalculated.Value)
                    {
                        localPluginContext.Trace("Choose Family OOP");
                        newOopCoverageLevel = ipg_BenefitCoverageLevels.Family;
                    }
                    else
                    {
                        localPluginContext.Trace("Choose Individual OOP");
                        newOopCoverageLevel = ipg_BenefitCoverageLevels.Individual;
                    }
                }

                if (newDeductibleCoverageLevel.HasValue || newOopCoverageLevel.HasValue)
                {
                    localPluginContext.Trace("Getting Incident");
                    var incident = service.Retrieve(Incident.EntityLogicalName, benefit.ipg_CaseId.Id, new ColumnSet(
                        Incident.Fields.ipg_BenefitTypeCode,
                        Incident.Fields.ipg_inoutnetwork,
                        Incident.Fields.ipg_CoverageLevelDeductible,
                        Incident.Fields.ipg_CoverageLevelOOP)).ToEntity<Incident>();

                    if(incident.ipg_BenefitTypeCodeEnum == benefit.ipg_BenefitTypeEnum
                        && incident.ipg_inoutnetwork == benefit.ipg_InOutNetwork
                        && (incident.ipg_CoverageLevelDeductibleEnum != newDeductibleCoverageLevel || incident.ipg_CoverageLevelOOPEnum != newOopCoverageLevel))
                    {
                        localPluginContext.Trace("Updating Incident");

                        var caseUpdate = new Incident
                        {
                            Id = benefit.ipg_CaseId.Id
                        };

                        if (newDeductibleCoverageLevel.HasValue && incident.ipg_CoverageLevelDeductibleEnum != newDeductibleCoverageLevel)
                        {
                            caseUpdate.ipg_CoverageLevelDeductibleEnum = newDeductibleCoverageLevel;
                        }
                        if(newOopCoverageLevel.HasValue && incident.ipg_CoverageLevelOOPEnum != newOopCoverageLevel)
                        {
                            caseUpdate.ipg_CoverageLevelOOPEnum = newOopCoverageLevel;
                        }
                        
                        service.Update(caseUpdate);
                    }
                }
            }
        }
    }
}