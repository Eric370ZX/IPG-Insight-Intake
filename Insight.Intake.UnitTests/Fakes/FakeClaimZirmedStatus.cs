using Bogus;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeClaimZirmedStatus
    {
        public static Faker<ipg_claimzirmedstatus> Fake(this ipg_claimzirmedstatus self)
        {
            return new Faker<ipg_claimzirmedstatus>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<ipg_claimzirmedstatus> FakeClaimZirmedStatusForClaims(this Faker<ipg_claimzirmedstatus> self, Invoice claim, ipg_claimzirmedeventcode eventCode)
        {
            self.RuleFor(
                x => x.ipg_ClaimId,
                x => new EntityReference
                {
                    Id = claim.Id,
                    LogicalName = claim.LogicalName
                }
            ).RuleFor(
                x => x.ipg_EventCode,
                x => new EntityReference
                {
                    Id = eventCode.Id,
                    LogicalName = eventCode.LogicalName
                }
            );

            return self;
        }
    }
}