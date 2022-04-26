using FakeXrmEasy;
using Insight.Intake.Plugin.Benefit;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class UpdateCaseCoverageLevelsTest : PluginTestsBase
    {
        [Fact]
        public void UpdatesCoverageLevelsOnBenefitCreation()
        {
            //ARRANGE
            var incident = new Incident().Fake()
                    .WithBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage)
                    .WithInOutNetwork(true)
                    .WithCoverageLevels(ipg_BenefitCoverageLevels.Individual, ipg_BenefitCoverageLevels.Individual)
                    .Generate();
            var benefit = new ipg_benefit().Fake().WithCaseReference(incident)
                            .WithBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage)
                            .WithInOutNetwork(true)
                            .WithIndividualBenefits(100, 70, 100, 70)
                            .WithFamilyBenefits(90, 70, 90, 70)
                            .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { incident, benefit });

            var inputParameters = new ParameterCollection { { "Target", benefit } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = ipg_benefit.EntityLogicalName,
                InputParameters = inputParameters,
                PreEntityImages = null,
                PostEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<UpdateCaseCoverageLevels>(pluginContext);

            //ASSERT
            var updatedIncident = fakedContext.GetOrganizationService().Retrieve(Incident.EntityLogicalName, incident.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(true)).ToEntity<Incident>();
            Assert.Equal(ipg_BenefitCoverageLevels.Family, updatedIncident.ipg_CoverageLevelDeductibleEnum);
            Assert.Equal(ipg_BenefitCoverageLevels.Family, updatedIncident.ipg_CoverageLevelOOPEnum);
        }

        
    }
}
