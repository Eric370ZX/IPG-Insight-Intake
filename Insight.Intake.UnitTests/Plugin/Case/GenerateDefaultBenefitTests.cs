using FakeXrmEasy;
using Insight.Intake.Plugin.Case;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using Xunit;

namespace Insight.Intake.UnitTests.Plugin.Case
{
    public class GenerateDefaultBenefitTests : PluginTestsBase
    {
        [Fact]
        public void GeneratesDefaultBenefitIfAuto()
        {
            GeneratesDefaultBenefitIfCarrierType(ipg_CarrierType.Auto, ipg_BenefitType.Auto);
        }

        [Fact]
        public void DoesNotUpdateExistingBenefitIfExists()
        {
            //ARRANGE
            var carrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier)
                  .WithCarrierType(ipg_CarrierType.Auto)
                    .Generate();
            var incident = new Incident().Fake()
                    .WithCarrierReference(carrier)
                    .Generate();
            decimal deductible = 2, deductibleMet = 1,
                oopMax = 3, oopMet = 2;
            var existingBenefit = new ipg_benefit().Fake()
                    .WithCaseReference(incident)
                    .WithCarrierReference(carrier)
                    .WithBenefitType(ipg_BenefitType.Auto)
                    .WithInOutNetwork(false)
                    .WithStateCode(ipg_benefitState.Active)
                    .WithIndividualBenefits(deductible, deductibleMet, oopMax, oopMet)
                    .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { carrier, incident, existingBenefit });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PreEntityImages = null,
                PostEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<GenerateDefaultBenefit>(pluginContext);

            //ASSERT
            var organizationService = fakedContext.GetOrganizationService();
            var benefitAfterUpdate = organizationService.Retrieve(ipg_benefit.EntityLogicalName, existingBenefit.Id, new ColumnSet(true)).ToEntity<ipg_benefit>();
            Assert.Equal(incident.Id, benefitAfterUpdate.ipg_CaseId.Id);
            Assert.Equal(carrier.Id, benefitAfterUpdate.ipg_CarrierId.Id);
            Assert.Equal(ipg_BenefitType.Auto, benefitAfterUpdate.ipg_BenefitTypeEnum);
            Assert.False(benefitAfterUpdate.ipg_InOutNetwork);
            Assert.Equal(deductible, benefitAfterUpdate.ipg_Deductible.Value);
            Assert.Equal(deductibleMet, benefitAfterUpdate.ipg_DeductibleMet.Value);
            Assert.Equal(oopMax, benefitAfterUpdate.ipg_MemberOOPMax.Value);
            Assert.Equal(oopMet, benefitAfterUpdate.ipg_MemberOOPMet.Value);
        }

        [Fact]
        public void ChecksSecondCarriers()
        {
            //ARRANGE
            var carrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier)
                    .WithCarrierType(ipg_CarrierType.Auto)
                    .Generate();
            var incident = new Incident().Fake()
                    .WithCarrierReference(carrier, isPrimaryCarrier: false)
                    .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { carrier, incident });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PreEntityImages = null,
                PostEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<GenerateDefaultBenefit>(pluginContext);

            //ASSERT
            var organizationService = fakedContext.GetOrganizationService();
            var benefits = organizationService.RetrieveMultiple(new QueryExpression(ipg_benefit.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            });
            Assert.Single(benefits.Entities);
            var benefit = benefits.Entities[0].ToEntity<ipg_benefit>();
            Assert.Equal(incident.Id, benefit.ipg_CaseId.Id);
            Assert.Equal(carrier.Id, benefit.ipg_CarrierId.Id);
            Assert.Equal(ipg_BenefitType.Auto, benefit.ipg_BenefitTypeEnum);
            Assert.False(benefit.ipg_InOutNetwork);
            ValidateBenefit(benefit);
        }

        [Fact]
        public void GeneratesDefaultBenefitIfWC()
        {
            GeneratesDefaultBenefitIfCarrierType(ipg_CarrierType.WorkersComp, ipg_BenefitType.WorkersComp);
        }

        [Fact]
        public void DoesNoGeneratesDefaultBenefitIfOtherCarrierType()
        {
            //ARRANGE
            var carrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier)
                    .WithCarrierType(ipg_CarrierType.Commercial)
                    .Generate();
            var incident = new Incident().Fake()
                    .WithCarrierReference(carrier)
                    .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { carrier, incident });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PreEntityImages = null,
                PostEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<GenerateDefaultBenefit>(pluginContext);

            //ASSERT
            var organizationService = fakedContext.GetOrganizationService();
            var benefits = organizationService.RetrieveMultiple(new QueryExpression(ipg_benefit.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            });
            Assert.Empty(benefits.Entities);
        }


        private void ValidateBenefit(ipg_benefit benefit)
        {
            Assert.Equal(0, benefit.ipg_Deductible.Value);
            Assert.Equal(0, benefit.ipg_DeductibleMet.Value);
            Assert.Equal(0, benefit.ipg_MemberOOPMax.Value);
            Assert.Equal(0, benefit.ipg_MemberOOPMet.Value);
            Assert.Equal(100, benefit.ipg_CarrierCoinsurance);
            Assert.Equal(0, benefit.ipg_MemberCoinsurance);
        }

        private void GeneratesDefaultBenefitIfCarrierType(ipg_CarrierType carrierType, ipg_BenefitType benefitType)
        {
            //ARRANGE
            var carrier = new Intake.Account().Fake((int)Account_CustomerTypeCode.Carrier)
                    .WithCarrierType(carrierType)
                    .Generate();
            var incident = new Incident().Fake()
                    .WithCarrierReference(carrier)
                    .Generate();

            var fakedContext = new XrmFakedContext();
            fakedContext.Initialize(new List<Entity> { carrier, incident });

            var inputParameters = new ParameterCollection { { "Target", incident } };

            var pluginContext = new XrmFakedPluginExecutionContext()
            {
                MessageName = MessageNames.Create,
                Stage = PipelineStages.PostOperation,
                PrimaryEntityName = Incident.EntityLogicalName,
                InputParameters = inputParameters,
                PreEntityImages = null,
                PostEntityImages = null
            };

            //ACT
            fakedContext.ExecutePluginWith<GenerateDefaultBenefit>(pluginContext);

            //ASSERT
            var organizationService = fakedContext.GetOrganizationService();
            var benefits = organizationService.RetrieveMultiple(new QueryExpression(ipg_benefit.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            });
            Assert.Single(benefits.Entities);
            var benefit = benefits.Entities[0].ToEntity<ipg_benefit>();
            Assert.Equal(incident.Id, benefit.ipg_CaseId.Id);
            Assert.Equal(carrier.Id, benefit.ipg_CarrierId.Id);
            Assert.Equal(benefitType, benefit.ipg_BenefitTypeEnum);
            Assert.False(benefit.ipg_InOutNetwork);
            ValidateBenefit(benefit);
        }
    }
}
