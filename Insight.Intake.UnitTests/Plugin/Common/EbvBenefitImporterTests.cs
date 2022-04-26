using FakeXrmEasy;
using Insight.Intake.Data;
using Insight.Intake.UnitTests.Fakes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using Xunit;
using Insight.Intake.Plugin.Common.Benefits;
using System.Linq;

namespace Insight.Intake.UnitTests.Plugin.Common
{
    public class EbvBenefitImporterTests : PluginTestsBase
    {
        private readonly string HealthServiceTypeCode = "30";
        private readonly string DmeServiceTypeCode = "12";
        private readonly string InNetworkCode = "Y";
        private readonly string IndividualCoverageLevelCode = "IND";

        [Fact]
        public void ImportsPrimaryBenefits()
        {
            ExecutePrimaryOrSecondaryCarrierTest(CarrierNumbers.First);
        }

        [Fact]
        public void ImportsSecondaryBenefits()
        {
            ExecutePrimaryOrSecondaryCarrierTest(CarrierNumbers.Second);
        }

        private void ExecutePrimaryOrSecondaryCarrierTest(CarrierNumbers carrierNumber)
        {
            //ARRANGE

            Guid ebvResponseId = Guid.NewGuid();
            Guid currentUserId = Guid.NewGuid();

            var carrier = new Intake.Account().Fake(CustomerTypeCodeOptionSet.Carrier).Generate();
            var incidentFake = new Incident().Fake()
                .WithBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage);

            const string memberId = "XYZ123";
            if(carrierNumber == CarrierNumbers.First)
            {
                incidentFake
                    .WithCarrierReference(carrier)
                    .WithMemberId(memberId);
            }
            else if(carrierNumber == CarrierNumbers.Second)
            {
                incidentFake
                    .WithSecondaryCarrierReference(carrier)
                    .WithSecondaryMemberId(memberId);
            }
            var incident = incidentFake.Generate();

            var gwServiceTypes = new List<ipg_GWServiceTypeCode>() {
                new ipg_GWServiceTypeCode().Fake().WithServiceType(HealthServiceTypeCode).WithBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage).WithImportBenefits(true),
                new ipg_GWServiceTypeCode().Fake().WithServiceType(DmeServiceTypeCode).WithBenefitType(ipg_BenefitType.DurableMedicalEquipment_DME).WithImportBenefits(true),
            };
            var benefit = new ipg_benefit().Fake()
                .WithCaseReference(incident)
                .WithCarrierReference(carrier)
                .WithMemberId(memberId)
                .WithBenefitType(ipg_BenefitType.HealthBenefitPlanCoverage)
                .WithInOutNetwork(true)
                .WithCoverageLevel(ipg_BenefitCoverageLevels.Individual)
                .WithIndividualBenefits()
                .Generate();
            var ebvHealthCoinsurance = new ipg_EBVBenefit().FakeCoinsurance(ebvResponseId, HealthServiceTypeCode, InNetworkCode, IndividualCoverageLevelCode).Generate();
            var ebvHealthDeductibleMet = new ipg_EBVBenefit().FakeDeductibleMet(ebvResponseId, HealthServiceTypeCode, InNetworkCode, IndividualCoverageLevelCode).Generate();
            var ebvHealthDeductibleMax = new ipg_EBVBenefit().FakeDeductibleMax(ebvResponseId, HealthServiceTypeCode, InNetworkCode, IndividualCoverageLevelCode).Generate();
            var ebvHealthOopMet = new ipg_EBVBenefit().FakeOopMet(ebvResponseId, HealthServiceTypeCode, InNetworkCode, IndividualCoverageLevelCode).Generate();
            var ebvHealthOopMax = new ipg_EBVBenefit().FakeOopMax(ebvResponseId, HealthServiceTypeCode, InNetworkCode, IndividualCoverageLevelCode).Generate();
            var ebvDmeCoinsurance = new ipg_EBVBenefit().FakeCoinsurance(ebvResponseId, DmeServiceTypeCode, InNetworkCode, IndividualCoverageLevelCode).Generate();
            var ebvDmeDeductibleMet = new ipg_EBVBenefit().FakeDeductibleMet(ebvResponseId, DmeServiceTypeCode, InNetworkCode, IndividualCoverageLevelCode).Generate();
            var ebvDmeDeductibleMax = new ipg_EBVBenefit().FakeDeductibleMax(ebvResponseId, DmeServiceTypeCode, InNetworkCode, IndividualCoverageLevelCode).Generate();
            var ebvDmeOopMet = new ipg_EBVBenefit().FakeOopMet(ebvResponseId, DmeServiceTypeCode, InNetworkCode, IndividualCoverageLevelCode).Generate();
            var ebvDmeOopMax = new ipg_EBVBenefit().FakeOopMax(ebvResponseId, DmeServiceTypeCode, InNetworkCode, IndividualCoverageLevelCode).Generate();

            var fakedContext = new XrmFakedContext();
            var entities = new List<Entity> { carrier, incident, benefit,
                ebvHealthCoinsurance, ebvHealthDeductibleMet, ebvHealthDeductibleMax, ebvHealthOopMet, ebvHealthOopMax,
                ebvDmeCoinsurance, ebvDmeDeductibleMet, ebvDmeDeductibleMax, ebvDmeOopMet, ebvDmeOopMax
            };
            entities.AddRange(gwServiceTypes);
            fakedContext.Initialize(entities);

            //ACT
            var ebvBenefitImporter = new EbvBenefitImporter(fakedContext.GetOrganizationService(), fakedContext.GetFakeTracingService());
            ebvBenefitImporter.ImportEBVBenefitsToD365(incident.Id, carrierNumber, ebvResponseId, currentUserId);

            //ASSERT
            IEnumerable<ipg_benefit> updatedBenefits;
            using (var crmServiceContext = new CrmServiceContext(fakedContext.GetOrganizationService()))
            {
                updatedBenefits = (from b in crmServiceContext.CreateQuery<ipg_benefit>()
                                   where b.ipg_CaseId != null && b.ipg_CaseId.Id == incident.Id
                                        && b.ipg_CarrierId != null && b.ipg_CarrierId.Id == carrier.Id
                                        && b.ipg_MemberID == memberId
                                   select b).ToList();
            }

            Assert.Equal(2, updatedBenefits.Count());

            var healthBenefit = updatedBenefits.FirstOrDefault(b => b.ipg_BenefitTypeEnum == ipg_BenefitType.HealthBenefitPlanCoverage);
            Assert.Equal(ebvHealthCoinsurance.ipg_Percentage * 100, Convert.ToDecimal(healthBenefit.ipg_CarrierCoinsurance));
            Assert.Equal(ebvHealthDeductibleMet.ipg_MonetaryAmount, healthBenefit.ipg_DeductibleMet.Value);
            Assert.Equal(ebvHealthDeductibleMax.ipg_MonetaryAmount, healthBenefit.ipg_Deductible.Value);
            Assert.Equal(ebvHealthOopMet.ipg_MonetaryAmount, healthBenefit.ipg_MemberOOPMet.Value);
            Assert.Equal(ebvHealthOopMax.ipg_MonetaryAmount, healthBenefit.ipg_MemberOOPMax.Value);

            var dmeBenefit = updatedBenefits.FirstOrDefault(b => b.ipg_BenefitTypeEnum == ipg_BenefitType.DurableMedicalEquipment_DME);
            Assert.Equal(ebvDmeCoinsurance.ipg_Percentage * 100, Convert.ToDecimal(dmeBenefit.ipg_CarrierCoinsurance));
            Assert.Equal(ebvDmeDeductibleMet.ipg_MonetaryAmount, dmeBenefit.ipg_DeductibleMet.Value);
            Assert.Equal(ebvDmeDeductibleMax.ipg_MonetaryAmount, dmeBenefit.ipg_Deductible.Value);
            Assert.Equal(ebvDmeOopMet.ipg_MonetaryAmount, dmeBenefit.ipg_MemberOOPMet.Value);
            Assert.Equal(ebvDmeOopMax.ipg_MonetaryAmount, dmeBenefit.ipg_MemberOOPMax.Value);
        }
    }
}
