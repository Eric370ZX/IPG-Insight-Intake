using Bogus;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeQueue
    {
        public static Faker<Queue> Fake(this Queue self)
        {
            return new Faker<Queue>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<Queue> WithOwner(this Faker<Queue> self, Entity owner)
        {
            self.RuleFor(x => x.OwnerId, x => owner.ToEntityReference());
            return self;
        }
    }
}
