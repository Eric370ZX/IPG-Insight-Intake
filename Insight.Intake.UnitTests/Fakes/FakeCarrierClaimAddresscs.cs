using Bogus;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class FakeCarrierClaimAddresscs
    {
        public static Faker<ipg_carrierclaimsmailingaddress> Fake(this ipg_carrierclaimsmailingaddress self, ipg_zipcode zip)
        {
            return new Faker<ipg_carrierclaimsmailingaddress>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_ClaimsMailingZipCodeIdId, x => zip.ToEntityReference())
                .RuleFor(x => x.ipg_carrierclaimname, x => x.Random.String())
                .RuleFor(x => x.ipg_claimsmailingcity, x => x.Random.String())
                .RuleFor(x => x.ipg_claimsmailingstate, x => x.Random.String())
                .RuleFor(x => x.ipg_claimsmailingaddress, x => x.Random.String());
        }

    }
}
