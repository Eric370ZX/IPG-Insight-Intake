using Bogus;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeInvoice
    {
        public static Faker<Invoice> Fake(this Invoice self)
        {
            return new Faker<Invoice>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }
    }
}