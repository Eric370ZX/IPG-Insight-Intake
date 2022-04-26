using Bogus;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class FakeMelissaZipCode
    {
        public static Faker<ipg_melissazipcode> Fake(this ipg_melissazipcode self, string name = null, string city = null)
        {
            return new Faker<ipg_melissazipcode>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_name, x => name ?? x.Random.String())
                .RuleFor(x => x.ipg_city, x => city ?? x.Random.String());
        }

        public static Faker<ipg_melissazipcode> WithState(this Faker<ipg_melissazipcode> self, ipg_state state)
        {
            return self
                .RuleFor(x => x.ipg_state, x => state.ipg_name)
                .RuleFor(x => x.ipg_stateid, x => state.ToEntityReference());
        }
    }
}
