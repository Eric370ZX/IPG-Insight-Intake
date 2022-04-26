using Bogus;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeFacilityCPT
    {
        public static Faker<ipg_facilitycpt> Fake(this ipg_facilitycpt self)
        {
            return new Faker<ipg_facilitycpt>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_name, x => x.Name.FullName());
        }

        public static Faker<ipg_facilitycpt> WithCPT(this Faker<ipg_facilitycpt> self, ipg_cptcode cptCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_CptCodeId, x => cptCode.ToEntityReference());

            return self;
        }

        public static Faker<ipg_facilitycpt> WitFacility(this Faker<ipg_facilitycpt> self, Account facility)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_FacilityId, x => facility.ToEntityReference());

            return self;
        }
    }
}
