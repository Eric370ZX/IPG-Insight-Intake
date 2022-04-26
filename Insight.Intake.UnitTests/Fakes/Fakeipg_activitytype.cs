using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Bogus;
using Insight.Intake.Data;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class Fakeipg_activitytype
    {
        public static Faker<ipg_activitytype> Fake(this ipg_activitytype self)
        {
            return new Faker<ipg_activitytype>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }
    }
}
