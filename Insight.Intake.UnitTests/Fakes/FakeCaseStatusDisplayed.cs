using Bogus;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeCaseStatusDisplayed
    {
        public static Faker<ipg_casestatusdisplayed> Fake(this ipg_casestatusdisplayed self)
        {
            return new Faker<ipg_casestatusdisplayed>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<ipg_casestatusdisplayed> WithName(this Faker<ipg_casestatusdisplayed> self, string name)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.ipg_name, x => name);

            return self;
        }
    }
}
