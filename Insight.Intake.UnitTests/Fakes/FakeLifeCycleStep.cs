using Bogus;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeLifeCycleStep
    {
        public static Faker<ipg_lifecyclestep> Fake(this ipg_lifecyclestep self, ipg_gateconfiguration gate, string name = null)
        {
            return new Faker<ipg_lifecyclestep>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_gateconfigurationid, x => gate.ToEntityReference())
                .RuleFor(x => x.ipg_name, name ?? "test");
        }
    }
}
