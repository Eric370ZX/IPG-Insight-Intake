using Bogus;
using System;
using static Insight.Intake.Helpers.Constants;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeRole
    {
        public static Faker<Role> Fake(this Role self, string name = SecurityRoleNames.SYS_ADMIN)
        {
            return new Faker<Role>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.Name, name);
        }
    }
}
