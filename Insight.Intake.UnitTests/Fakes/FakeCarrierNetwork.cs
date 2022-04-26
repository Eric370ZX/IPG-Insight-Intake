using Bogus;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeCarrierNetwork
    {
        public static Faker<ipg_carriernetwork> Fake(this ipg_carriernetwork self)
        {
            return new Faker<ipg_carriernetwork>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }
    }
}