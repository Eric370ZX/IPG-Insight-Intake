using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeWorkflowTask
    {
        public static Faker<ipg_workflowtask> Fake(this ipg_workflowtask self)
        {
            return new Faker<ipg_workflowtask>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

    }
}
