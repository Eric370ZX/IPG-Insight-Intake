using System;
using Bogus;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class FakeUnsupportedPart
    {
        public static Faker<ipg_unsupportedpart> Fake(this ipg_unsupportedpart self)
        {
            return new Faker<ipg_unsupportedpart>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }
    }
}
