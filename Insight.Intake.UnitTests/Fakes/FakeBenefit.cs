using Bogus;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeBenefit
    {
        public static Faker<ipg_benefit> Fake(this ipg_benefit self)
        {
            return new Faker<ipg_benefit>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<ipg_benefit> WithCaseReference(this Faker<ipg_benefit> self, Incident caseEntity)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (caseEntity == null) throw new ArgumentNullException(nameof(caseEntity));

            self.RuleFor(
                x => x.ipg_CaseId,
                x => caseEntity.ToEntityReference()
            );

            return self;
        }

        public static Faker<ipg_benefit> WithBenefitType(this Faker<ipg_benefit> self, ipg_BenefitType benefitType)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.ipg_BenefitTypeEnum,
                x => benefitType
            );

            self.RuleFor(
                x => x.ipg_BenefitType,
                x => new OptionSetValue((int)benefitType)
            );

            return self;
        }

        public static Faker<ipg_benefit> WithInOutNetwork(this Faker<ipg_benefit> self, bool inOutNetwork)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.ipg_InOutNetwork,
                x => inOutNetwork
            );

            return self;
        }

        public static Faker<ipg_benefit> WithIndividualBenefits(this Faker<ipg_benefit> self, decimal deductible, decimal deductibleMet,
            decimal oopMax, decimal oopMet)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.ipg_Deductible,
                x => new Money(deductible)
            );

            self.RuleFor(
                x => x.ipg_DeductibleMet,
                x => new Money(deductibleMet)
            );

            self.RuleFor(
                x => x.ipg_DeductibleRemainingCalculated,
                x => new Money(deductible - deductibleMet)
            );

            self.RuleFor(
                x => x.ipg_MemberOOPMax,
                x => new Money(oopMax)
            );

            self.RuleFor(
                x => x.ipg_MemberOOPMet,
                x => new Money(oopMet)
            );

            self.RuleFor(
                x => x.ipg_MemberOOPRemainingCalculated,
                x => new Money(oopMax - oopMet)
            );

            return self;
        }

        public static Faker<ipg_benefit> WithIndividualBenefits(this Faker<ipg_benefit> self)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            double memberCoinsurance = new Randomizer().Double();

            self.RuleFor(x => x.ipg_MemberCoinsurance, x => memberCoinsurance)
                .RuleFor(x => x.ipg_CarrierCoinsurance, x => 1 - memberCoinsurance);

            self.RuleFor(
                x => x.ipg_Deductible,
                x => new Money(x.Finance.Random.Decimal(0, 10000))
            );

            self.RuleFor(
                x => x.ipg_DeductibleMet,
                x => new Money(x.Finance.Random.Decimal(0, 10000))
            );

            self.RuleFor(
                x => x.ipg_MemberOOPMax,
                x => new Money(x.Finance.Random.Decimal(0, 10000))
            );

            self.RuleFor(
                x => x.ipg_MemberOOPMet,
                x => new Money(x.Finance.Random.Decimal(0, 10000))
            );

            return self;
        }

        public static Faker<ipg_benefit> WithFamilyBenefits(this Faker<ipg_benefit> self, decimal deductible, decimal deductibleMet,
            decimal oopMax, decimal oopMet)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.ipg_FamilyDeductible,
                x => new Money(deductible)
            );

            self.RuleFor(
                x => x.ipg_FamilyDeductibleMet,
                x => new Money(deductibleMet)
            );

            self.RuleFor(
                x => x.ipg_FamilyDeductibleRemainingCalculated,
                x => new Money(deductible - deductibleMet)
            );

            self.RuleFor(
                x => x.ipg_FamilyOOPMax,
                x => new Money(oopMax)
            );

            self.RuleFor(
                x => x.ipg_FamilyOOPMet,
                x => new Money(oopMet)
            );

            self.RuleFor(
                x => x.ipg_FamilyOOPRemainingCalculated,
                x => new Money(oopMax - oopMet)
            );

            return self;
        }

        public static Faker<ipg_benefit> WithCarrierReference(this Faker<ipg_benefit> self, Intake.Account carrier)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (carrier == null) throw new ArgumentNullException(nameof(carrier));

            self.RuleFor(
                x => x.ipg_CarrierId,
                x => carrier.ToEntityReference()
            );

            return self;
        }

        public static Faker<ipg_benefit> WithMemberId(this Faker<ipg_benefit> self, string memberId)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.ipg_MemberID,
                x => memberId
            );

            return self;
        }

        public static Faker<ipg_benefit> WithStateCode(this Faker<ipg_benefit> self, ipg_benefitState stateCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.StateCode,
                x => stateCode
            );

            return self;
        }

        public static Faker<ipg_benefit> WithBenefitSource(this Faker<ipg_benefit> self, ipg_BenefitSources benefitSource)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.ipg_BenefitSource,
                x => new OptionSetValue((int)benefitSource)
            );

            return self;
        }
        public static Faker<ipg_benefit> WithCoverageLevel(this Faker<ipg_benefit> self, ipg_BenefitCoverageLevels coverageLevel)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(
                x => x.ipg_CoverageLevelEnum,
                x => coverageLevel
            );

            return self;
        }
    }
}