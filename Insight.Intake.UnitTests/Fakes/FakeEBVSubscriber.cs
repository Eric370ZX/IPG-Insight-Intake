using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class FakeEBVSubscriber
    {
        public static Faker<ipg_EBVSubscriber> Fake(this ipg_EBVSubscriber self)
        {
            return new Faker<ipg_EBVSubscriber>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }
    }
}
