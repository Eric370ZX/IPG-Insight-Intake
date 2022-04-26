using Bogus;
using System;
using System.Linq;

namespace Insight.Intake.UnitTests.Fakes
{
    public static class FakeSystemUser
    {
        public static Faker<SystemUser> Fake(this SystemUser self)
        {
            return new Faker<SystemUser>()
                .RuleFor(x => x.Id, x => Guid.NewGuid());
        }

        public static Faker<SystemUser> Fake(this SystemUser self, string name)
        {
            return new Faker<SystemUser>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.FullName, x => name)
                .RuleFor(x => x.FirstName, x => name.Split(' ').FirstOrDefault())
                .RuleFor(x => x.LastName, x => name.Split(' ').Skip(1).FirstOrDefault());
        }

        public static Faker<SystemUser> WithWmail(this Faker<SystemUser> self, string email)
        {
            return self.RuleFor(x => x.InternalEMailAddress, x => email);
        }
    }
}
