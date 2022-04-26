using Bogus;
using System;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.UnitTests.Fakes
{
    internal static class FakeAccountUser
    {
        public static Faker<ipg_accountuser> Fake(this ipg_accountuser self, SystemUser user, Account account, bool isprimary = false)
        {
            return new Faker<ipg_accountuser>()
                .RuleFor(x => x.Id, x => Guid.NewGuid())
                .RuleFor(x => x.ipg_userid, x => user.ToEntityReference())
                .RuleFor(x => x.ipg_accountid, x => account.ToEntityReference())
                .RuleFor(x => x.ipg_rolecodeEnum, x => ipg_accountuser_ipg_rolecode.CaseManager)
                .RuleFor(x => x.ipg_isprimary, x => isprimary);
        }
    }
}
