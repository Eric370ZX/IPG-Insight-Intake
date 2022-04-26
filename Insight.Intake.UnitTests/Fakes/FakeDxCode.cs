using System;
using Bogus;
using Microsoft.Xrm.Sdk;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeDxCode
    {
        public static Faker<ipg_dxcode> Fake(this ipg_dxcode self)
        {
            return new Faker<ipg_dxcode>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_DxCode, x => x.Random.String())
                .RuleFor(x => x.ipg_name, x => x.Random.Number(1000, 10000).ToString());
        }

    }
}