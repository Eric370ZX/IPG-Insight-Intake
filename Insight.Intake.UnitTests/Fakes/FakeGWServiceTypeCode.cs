using Bogus;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeGWServiceTypeCode
    {
        public static Faker<ipg_GWServiceTypeCode> Fake(this ipg_GWServiceTypeCode self)
        {
            return new Faker<ipg_GWServiceTypeCode>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<ipg_GWServiceTypeCode> WithBenefitType(this Faker<ipg_GWServiceTypeCode> self, ipg_BenefitType benefitType)
        {
            return self.
                RuleFor(x => x.ipg_BenefitTypeCode, x => new OptionSetValue((int)benefitType));
        }

        public static Faker<ipg_GWServiceTypeCode> WithServiceType(this Faker<ipg_GWServiceTypeCode> self, string serviceTypeCode)
        {
            return self.
                RuleFor(x => x.ipg_name, x => serviceTypeCode);
        }

        public static Faker<ipg_GWServiceTypeCode> WithImportBenefits(this Faker<ipg_GWServiceTypeCode> self, bool importBenefits)
        {
            return self.
                RuleFor(x => x.ipg_ImportBenefits, x => importBenefits);
        }
    }
}
