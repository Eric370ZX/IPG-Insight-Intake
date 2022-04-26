using Bogus;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeReferralBPF
    {
        public static Faker<ipg_ipgreferralbpfmainflow> Fake(this ipg_ipgreferralbpfmainflow self, EntityReference activeStageId, Guid? guid = null)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            return new Faker<ipg_ipgreferralbpfmainflow>()
                .RuleFor(x => x.Id, x => guid ?? Guid.NewGuid())
                .RuleFor(x => x.ActiveStageId, x => activeStageId);
        }
    }
}
