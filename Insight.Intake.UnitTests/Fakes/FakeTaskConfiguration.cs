using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeTaskConfiguration
    {
        public static Faker<ipg_taskconfiguration> Fake(this ipg_taskconfiguration self)
        {
            return new Faker<ipg_taskconfiguration>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }
    }
}
