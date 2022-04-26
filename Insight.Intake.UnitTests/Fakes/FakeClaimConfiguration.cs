using Bogus;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeClaimConfiguration
    {
        public static Faker<ipg_claimconfiguration> Fake(this ipg_claimconfiguration self)
        {
            return new Faker<ipg_claimconfiguration>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<ipg_claimconfiguration> WithEvent(this Faker<ipg_claimconfiguration> self, ipg_claimzirmedeventcode eventCode, int claimEvent, int claimSubEvent, int claimStatus, int claimReason)
        {
            self.RuleFor(
                x => x.ipg_EventCode,
                x => new EntityReference
                {
                    Id = eventCode.Id,
                    LogicalName = eventCode.LogicalName
                }
            ).RuleFor(
                x => x.ipg_ClaimEvent,
                x => new OptionSetValue(claimEvent)
            ).RuleFor(
                x => x.ipg_ClaimStatus,
                x => new OptionSetValue(claimStatus)
            ).RuleFor(
                x => x.ipg_ClaimSubEvent,
                x => new OptionSetValue(claimSubEvent)
            ).RuleFor(
                x => x.ipg_ClaimReason,
                x => new OptionSetValue(claimReason)
            );

            return self;
        }
    }
}