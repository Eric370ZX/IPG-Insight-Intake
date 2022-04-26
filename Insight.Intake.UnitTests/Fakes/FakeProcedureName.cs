using Bogus;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeProcedureName
    {
        public static Faker<ipg_procedurename> Fake(this ipg_procedurename self)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            return new Faker<ipg_procedurename>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_name, x => x.Random.String(10));
        }

        public static Faker<ipg_procedurename> WithActiveValue(this Faker<ipg_procedurename> self, bool isActive)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            return self;
        }

    }
}