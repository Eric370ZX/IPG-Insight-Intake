using Bogus;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class FakeClaimStatusCode
    {
        public static Faker<ipg_claimstatuscode> FakeNewClaimStatusCode(this ipg_claimstatuscode claimstatuscode)
        {
            return new Faker<ipg_claimstatuscode>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_name, x => "temp new claim status");
        }

        public static Faker<ipg_claimstatuscode> FakeClosedClaimStatusCode(this ipg_claimstatuscode claimstatuscode)
        {
            return new Faker<ipg_claimstatuscode>()
                 .RuleFor(x => x.Id, x => Guid.NewGuid())
                 .RuleFor(x => x.ipg_name, x => "temp closed claim status");
        }
    }
}
