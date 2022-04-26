using Bogus;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class FakeWorkFlow
    {
        public static Faker<Workflow> Fake(this Workflow self, string uniqName)
        {
            return new Faker<Workflow>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.UniqueName, x => uniqName);
        }
    }
}
