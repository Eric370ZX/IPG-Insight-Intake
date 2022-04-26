using Bogus;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeMasterHCPCS
    {
        public static Faker<ipg_masterhcpcs> Fake(this ipg_masterhcpcs self)
        {
            return new Faker<ipg_masterhcpcs>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(m => m.ipg_name, m => m.Random.Int(1000, 9999).ToString())
                .RuleFor(m => m.StateCode, m => ipg_masterhcpcsState.Active);
        }

        public static Faker<ipg_masterhcpcs> Fake(this ipg_masterhcpcs self, string name, DateTime? effectiveDate, DateTime? expirationDate)
        {
            return new Faker<ipg_masterhcpcs>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(m => m.StateCode, m => ipg_masterhcpcsState.Active)
                .RuleFor(x => x.ipg_name, x => name)
                .RuleFor(x => x.ipg_EffectiveDate, x => effectiveDate)
                .RuleFor(x => x.ipg_ExpirationDate, x => expirationDate);
        }
    }
}
