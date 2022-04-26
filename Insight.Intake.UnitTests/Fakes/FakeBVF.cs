using Bogus;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class FakeBVF
    {
        public static Faker<ipg_benefitsverificationform> Fake(this ipg_benefitsverificationform self)
        {
            return new Faker<ipg_benefitsverificationform>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<ipg_benefitsverificationform> WithFormType(this Faker<ipg_benefitsverificationform> self, ipg_benefitsverificationform_ipg_formtype formType)
        {
            return self.RuleFor(x => x.ipg_formtype, x => new OptionSetValue((int)formType));
        }

        public static Faker<ipg_benefitsverificationform> WithCase(this Faker<ipg_benefitsverificationform> self, EntityReference caseRef)
        {
            return self.RuleFor(x => x.ipg_parentcaseid, x => caseRef);
        }

        public static Faker<ipg_benefitsverificationform> WithCarrierReference(this Faker<ipg_benefitsverificationform> self, EntityReference carrierReference)
        {
            return self.RuleFor(x => x.ipg_CarrierId, x => carrierReference);
        }

        public static Faker<ipg_benefitsverificationform> WithMemberId(this Faker<ipg_benefitsverificationform> self, string memberId = null)
        {
            return self.RuleFor(x => x.ipg_MemberIdNumber, x => memberId ?? x.Random.Int(10000, 99999).ToString());
        }

        public static Faker<ipg_benefitsverificationform> WithBenefit(this Faker<ipg_benefitsverificationform> self,
            ipg_BenefitType benefitType, 
            ipg_inn_or_oon inOrOutNetwork,
            DateTime? effectiveDate,
            DateTime? expirationDate,
            ipg_BenefitCoverageLevels deductibleCoverageLevel,
            ipg_BenefitCoverageLevels oopCoverageLevel
            )
        {
            return self
                .RuleFor(x => x.ipg_BenefitTypeCode, x => new OptionSetValue((int)benefitType))
                .RuleFor(x => x.ipg_inn_or_oon_code, x => new OptionSetValue((int)inOrOutNetwork))
                .RuleFor(x => x.ipg_coverageeffectivedate, x => effectiveDate)
                .RuleFor(x => x.ipg_coverageexpirationdate, x => expirationDate)
                .RuleFor(x => x.ipg_deductibletypecode, x => new OptionSetValue((int)deductibleCoverageLevel))
                .RuleFor(x => x.ipg_deductible, x => new Money(x.Finance.Random.Decimal()))
                .RuleFor(x => x.ipg_deductiblemet, x => new Money(x.Finance.Random.Decimal()))
                .RuleFor(x => x.ipg_oopmaxtypecode, x => new OptionSetValue((int)oopCoverageLevel))
                .RuleFor(x => x.ipg_oopmax, x => new Money(x.Finance.Random.Decimal()))
                .RuleFor(x => x.ipg_oopmaxmet, x => new Money(x.Finance.Random.Decimal()))
                ;
        }

    }
}
