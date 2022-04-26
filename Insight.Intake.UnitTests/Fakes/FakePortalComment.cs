using Bogus;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakePortalComment
    {
        public static Faker<adx_portalcomment> Fake(this adx_portalcomment self, Guid? guid = null)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            return new Faker<adx_portalcomment>()
                .RuleFor(x => x.Id, x => guid ?? Guid.NewGuid());
        }
        public static Faker<adx_portalcomment> WithOwner(this Faker<adx_portalcomment> self, Entity owner)
        {
            self.RuleFor(x => x.OwnerId, x => owner.ToEntityReference());
            return self;
        }
        public static Faker<adx_portalcomment> WithPortalOwner(this Faker<adx_portalcomment> self, Entity portalOwner)
        {
            self.RuleFor(x => x.ipg_owningportaluserid, x => portalOwner.ToEntityReference());
            return self;
        }
    }
}
