using Bogus;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeClaimResponseBatch
    {
        public static Faker<ipg_claimresponsebatch> Fake(this ipg_claimresponsebatch self)
        {
            return new Faker<ipg_claimresponsebatch>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }
    }
}