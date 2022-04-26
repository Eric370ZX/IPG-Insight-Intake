using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class Fakeipg_workflowtaskoutputconfig
    {
        public static Faker<ipg_workflowtaskoutputconfig> Fake(this ipg_workflowtaskoutputconfig self)
        {
            return new Faker<ipg_workflowtaskoutputconfig>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }
    }
}
