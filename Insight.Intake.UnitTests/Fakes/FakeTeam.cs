using Bogus;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeTeam
    {
        public static Faker<Team> Fake(this Team self,string teamName)
        {
            return new Faker<Team>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x=>x.Name, x => teamName);
        }
    }
}
