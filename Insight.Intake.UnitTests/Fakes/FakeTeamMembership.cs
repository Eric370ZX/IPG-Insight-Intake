using Bogus;
using Microsoft.Xrm.Sdk;
using System;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeTeamMembership
    {
        public static Faker<TeamMembership> Fake(this TeamMembership self, Team team, SystemUser member)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (team == null) throw new ArgumentNullException(nameof(team));
            if (member == null) throw new ArgumentNullException(nameof(member));

            var fakeself = new Faker<TeamMembership>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.Attributes, x => new AttributeCollection()
                {
                    { "teamid", team.TeamId},
                    { "systemuserid", member.SystemUserId},
                });

            return fakeself;
        }
    }
}
