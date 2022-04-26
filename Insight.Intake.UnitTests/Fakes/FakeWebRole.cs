using Bogus;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    public static  class FakeWebRole
    {
        public static Faker<Adx_webrole> Fake(this Adx_webrole self, string name)
        {
            return new Faker<Adx_webrole>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x =>x.Adx_name, x => name);
        }
    }
}
