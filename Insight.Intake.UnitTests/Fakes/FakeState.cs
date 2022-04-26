using Bogus;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class FakeState
    {
        public static Faker<ipg_state> Fake(this ipg_state self)
        {
            return new Faker<ipg_state>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_name, x => x.Random.String())
                .RuleFor(x => x.ipg_abbreviation, x => x.Random.String())
                .RuleFor(x => x.StateCode, x =>  ipg_stateState.Active)
                .RuleFor(x => x.StatusCodeEnum, x =>  ipg_state_StatusCode.Active);
        }
    }
}
