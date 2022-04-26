using Bogus;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeCarrierNetworkCptRule
    {
        public static Faker<ipg_carriernetworkcptrule> Fake(this ipg_carriernetworkcptrule self) =>
            new Faker<ipg_carriernetworkcptrule>()
                       .RuleFor(x => x.Id, x => Guid.NewGuid())
                       .RuleFor(x => x.StatusCode, x => new OptionSetValue(1))
                       .RuleFor(x => x.StatusCode, x => new OptionSetValue(1));

        public static Faker<ipg_carriernetworkcptrule> WithEffectivesDates(this Faker<ipg_carriernetworkcptrule> self, DateTime startDate, DateTime endDate) =>
            self.RuleFor(x => x.ipg_effectivedate, x => startDate)
                .RuleFor(x => x.ipg_expirationdate, x => endDate);

        public static Faker<ipg_carriernetworkcptrule> WithNetworkCarrierRelationshipId(this Faker<ipg_carriernetworkcptrule> self, EntityReference entityReference) =>
            self.RuleFor(x => x.ipg_CarrierNetworksId, x => entityReference);
    }
}