using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class FakeEbvResponse
    {
        public static Faker<ipg_EBVResponse> Fake(this ipg_EBVResponse self)
        {
            return new Faker<ipg_EBVResponse>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }
        public static Faker<ipg_EBVResponse> WithSuscriberReference(this Faker<ipg_EBVResponse> self, ipg_EBVSubscriber subscriberEntity)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            if (subscriberEntity == null) throw new ArgumentNullException(nameof(subscriberEntity));

            self.RuleFor(
                x => x.ipg_SubscriberId,
                x => subscriberEntity.ToEntityReference()
            );

            return self;
        }
    }
}
