using Bogus;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeEBVBenefit
    {
        public static Faker<ipg_EBVBenefit> Fake(this ipg_EBVBenefit self, Guid ebvResponseId, string serviceTypeCode, string networkCode,
            string coverageLevelCode)
        {
            return new Faker<ipg_EBVBenefit>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_ResponseId, x => new EntityReference(ipg_EBVResponse.EntityLogicalName, ebvResponseId))
                .RuleFor(x => x.ipg_ServiceType, x => serviceTypeCode)
                .RuleFor(x => x.ipg_InPlanNetwork, x => networkCode)
                .RuleFor(x => x.ipg_CoverageLevel, x => coverageLevelCode);
        }

        public static Faker<ipg_EBVBenefit> FakeCoinsurance(this ipg_EBVBenefit self, Guid ebvResponseId, string serviceTypeCode, string networkCode,
            string coverageLevelCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            var fake = self.Fake(ebvResponseId, serviceTypeCode, networkCode, coverageLevelCode);

            fake
                .RuleFor(x => x.ipg_Status, x => "A")
                .RuleFor(x => x.ipg_Percentage, x => x.Random.Decimal());

            return fake;
        }

        public static Faker<ipg_EBVBenefit> FakeDeductibleMet(this ipg_EBVBenefit self, Guid ebvResponseId, string serviceTypeCode, string networkCode,
            string coverageLevelCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            var fake = self.Fake(ebvResponseId, serviceTypeCode, networkCode, coverageLevelCode);

            fake
                .RuleFor(x => x.ipg_Status, x => "C")
                .RuleFor(x => x.ipg_TimePeriodQualifier, x => "24")
                .RuleFor(x => x.ipg_MonetaryAmount, x => x.Random.Decimal(1, 1000));

            return fake;
        }

        public static Faker<ipg_EBVBenefit> FakeDeductibleMax(this ipg_EBVBenefit self, Guid ebvResponseId, string serviceTypeCode, string networkCode,
            string coverageLevelCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            var fake = self.Fake(ebvResponseId, serviceTypeCode, networkCode, coverageLevelCode);

            fake
                .RuleFor(x => x.ipg_Status, x => "C")
                .RuleFor(x => x.ipg_TimePeriodQualifier, x => "22")
                .RuleFor(x => x.ipg_MonetaryAmount, x => x.Random.Decimal(1, 1000));

            return fake;
        }

        public static Faker<ipg_EBVBenefit> FakeOopMet(this ipg_EBVBenefit self, Guid ebvResponseId, string serviceTypeCode, string networkCode,
            string coverageLevelCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            var fake = self.Fake(ebvResponseId, serviceTypeCode, networkCode, coverageLevelCode);

            fake
                .RuleFor(x => x.ipg_Status, x => "G")
                .RuleFor(x => x.ipg_TimePeriodQualifier, x => "24")
                .RuleFor(x => x.ipg_MonetaryAmount, x => x.Random.Decimal(1, 1000));

            return fake;
        }

        public static Faker<ipg_EBVBenefit> FakeOopMax(this ipg_EBVBenefit self, Guid ebvResponseId, string serviceTypeCode, string networkCode,
            string coverageLevelCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            var fake = self.Fake(ebvResponseId, serviceTypeCode, networkCode, coverageLevelCode);

            fake
                .RuleFor(x => x.ipg_Status, x => "G")
                .RuleFor(x => x.ipg_TimePeriodQualifier, x => "22")
                .RuleFor(x => x.ipg_MonetaryAmount, x => x.Random.Decimal(1, 1000));

            return fake;
        }
    }
}