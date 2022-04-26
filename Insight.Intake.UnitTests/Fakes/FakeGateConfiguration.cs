using Bogus;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeGateConfiguration
    {
        public static Faker<ipg_gateconfiguration> Fake(this ipg_gateconfiguration self, int executionOrder)
        {
            return new Faker<ipg_gateconfiguration>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_executionorder, x => executionOrder);
        }

        public static Faker<ipg_gateconfiguration> Fake(this ipg_gateconfiguration self, string name)
        {
            return self.Fake(0).RuleFor(x => x.ipg_name, name);
        }

        public static Faker<ipg_gateconfiguration> WithStateCode(this Faker<ipg_gateconfiguration> self, int stateCode)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            self.RuleFor(x => x.StateCode, x => (ipg_gateconfigurationState)stateCode);

            return self;
        }
    }
}