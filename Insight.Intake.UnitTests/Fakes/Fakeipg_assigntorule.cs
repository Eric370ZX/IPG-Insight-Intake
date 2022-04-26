using Bogus;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class Fakeipg_assigntorule
    {
        public static Faker<ipg_assigntorule> Fake(this ipg_assigntorule self)
        {
            return new Faker<ipg_assigntorule>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }
    }
}