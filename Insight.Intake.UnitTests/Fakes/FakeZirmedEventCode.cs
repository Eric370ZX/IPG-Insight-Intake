using Bogus;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeZirmedEventCode
    {
        public static Faker<ipg_claimzirmedeventcode> Fake(this ipg_claimzirmedeventcode self)
        {
            return new Faker<ipg_claimzirmedeventcode>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }
    }
}