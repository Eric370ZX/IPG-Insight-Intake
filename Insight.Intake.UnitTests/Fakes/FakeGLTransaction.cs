using Bogus;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeGLTransaction
    {
        public static Faker<ipg_GLTransaction> Fake(this ipg_GLTransaction self)
        {
            return new Faker<ipg_GLTransaction>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<ipg_GLTransaction> WithCase(this Faker<ipg_GLTransaction> self, Incident incident)
        {
            return self.RuleFor(x => x.ipg_IncidentId, x => incident.ToEntityReference());
        }
    }
}
