using Bogus;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeClaimResponseHeader
    {
        public static Faker<ipg_claimresponseheader> Fake(this ipg_claimresponseheader self)
        {
            return new Faker<ipg_claimresponseheader>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<ipg_claimresponseheader> WithClaimResponseBatchReference(this Faker<ipg_claimresponseheader> self, ipg_claimresponsebatch batch)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (batch == null) throw new ArgumentNullException(nameof(batch));

            self.RuleFor(
                x => x.ipg_ClaimResponseBatchId,
                x => new EntityReference
                {
                    Id = batch.Id,
                    LogicalName = batch.LogicalName
                }
            );

            return self;
        }
    }
}