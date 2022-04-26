using Bogus;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeTaskCategory
    {
        public static Faker<ipg_taskcategory> Fake(this ipg_taskcategory self)
        {
            return new Faker<ipg_taskcategory>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<ipg_taskcategory> Fake(this ipg_taskcategory self, Guid id)
        {
            return new Faker<ipg_taskcategory>()
                .RuleFor(x => x.Id, x => id);
        }

        public static Faker<ipg_taskcategory> WithName(this Faker<ipg_taskcategory> self, string name)
        {
            return self.RuleFor(x => x.ipg_name, x => name);
        }
    }
}
