using Bogus;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class FakeCarrierNetWorkState
    {
        public static Faker<ipg_ipg_carriernetwork_ipg_state> Fake(this ipg_ipg_carriernetwork_ipg_state self)
        {
            return new Faker<ipg_ipg_carriernetwork_ipg_state>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<ipg_ipg_carriernetwork_ipg_state> WithState(this Faker<ipg_ipg_carriernetwork_ipg_state> self, ipg_state state)
        {
            return self
                .RuleFor(x => x.ipg_stateid, x => state.Id);
        }

        
    }
}
