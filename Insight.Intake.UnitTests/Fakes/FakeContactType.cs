
using System;
using Bogus;
using Microsoft.Xrm.Sdk;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeContactType
    {
        public static Faker<ipg_contacttype> Fake(this ipg_contacttype self, string contactType)
        {
            return new Faker<ipg_contacttype>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_name, x => contactType)
            ;
        }
    }
}